using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;
    using static IntVarStrategy;
    using static IntValueStrategy;

    // TODO: TBD: consider a fluent configuration-based approach...

    /// <summary>
    /// or-tools-based Constraint Programming problem solver.
    /// </summary>
    /// <inheritdoc cref="ProblemSolverBase{TSolver,TVariable}"/>
    /// <see cref="!:http://code.google.com/p/or-tools/"/>
    public abstract class OrProblemSolverBase : ProblemSolverBase<Solver, IntVar>, IOrProblemSolver
    {
        private readonly Lazy<ProblemSolverImplementation> _lazyImplementation;

        private ProblemSolverImplementation Implementation => _lazyImplementation.Value;

        /// <inheritdoc />
        protected OrProblemSolverBase(string modelName)
            : base(modelName)
        {
            _lazyImplementation = new Lazy<ProblemSolverImplementation>(() => new ProblemSolverImplementation());
        }

        /// <summary>
        /// Returns a random seed.
        /// </summary>
        /// <returns></returns>
        protected virtual int GetRandomSeed() => Implementation.GetRandomSeed();

        /// <summary>
        /// Initializes the problem solver.
        /// </summary>
        /// <param name="solver"></param>
        protected virtual void ReSeed(Solver solver) => Implementation.ReSeed(solver, GetRandomSeed());

        /* TODO: TBD: otherwise "free-standing" calls like PrepareVariables PrepareConstraints or
         * even ReSeed might be better exposed via an exposed fluent configuration... */

        /// <summary>
        /// Prepares the solver constraints. No need to add the Constraints to the
        /// <paramref name="solver"/>. Just prepare and return them. The parent class will handle
        /// the responsibility of adding them to the <paramref name="solver"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        protected abstract IEnumerable<Constraint> PrepareConstraints(Solver solver);

        // TODO: TBD: PrepareSearchMonitors is perhaps closer to a fluent style...

        /// <summary>
        /// Override to prepare the <see cref="SearchMonitor"/> or monitors for the
        /// <paramref name="agent"/> and corresponding <see cref="Solver"/>. If called, the base
        /// method should be called after preparing any other required Monitors. By default, a
        /// single <see cref="SolutionCollector"/> is prepared.
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        protected virtual ISearchAgent PrepareSearchMonitors(ISearchAgent agent, params IntVar[] variables)
        {
            if (!agent.HasSolutionCollector<SolutionCollector>())
            {
                // Start with a single all-solution-collector.
                agent.Monitor(a =>
                {
                    var m = a.Solver.MakeAllSolutionCollector();
                    var vector = new IntVarVector().TrackClrObject(this);
                    foreach (var v in variables) vector.Add(v);
                    m.Add(vector);
                    return m;
                });
            }

            return agent;
        }

        /// <summary>
        /// Prepares the <see cref="ISearchAgent"/> corresponding with the solver as a
        /// <see cref="IClrObjectHost"/>.
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
        protected virtual ISearchAgent PrepareSearch(params IntVar[] variables)
        {
            var a = new SearchAgent(this, variables);
            PrepareSearchMonitors(a, variables);
            return a;
        }

        /// <summary>
        /// Solved event.
        /// </summary>
        public event EventHandler<EventArgs> Solved;

        /// <summary>
        /// Raises the <see cref="Solved"/> event.
        /// </summary>
        /// <param name="e"></param>
        private void OnSolved(EventArgs e) => Solved?.Invoke(this, e);

        /// <summary>
        /// <see cref="ISearchAgent"/> ProcessVariables event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnProcessVariables(object sender, ProcessVariablesEventArgs e)
            => OnSolved(EventArgs.Empty);

        /// <summary>
        /// Begins a New Search corresponding with the <see cref="Solver"/> and
        /// <paramref name="agent"/>.
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        protected virtual ISearchAgent NewSearch(ISearchAgent agent)
        {
            agent.ProcessVariables -= OnProcessVariables;
            agent.ProcessVariables += OnProcessVariables;

            return agent.NewSearch(a => a.Solver.MakePhase(a.Variables, IntVarSimple, IntValueSimple));
        }

        /// <summary>
        /// Gets the <see cref="OptimizeVar"/> instances from the
        /// <see cref="ProblemSolverBase{TSolver,TVariable}.ClrCreatedObjects"/>.
        /// </summary>
        protected IEnumerable<OptimizeVar> Optimizations => ClrCreatedObjects.OfType<OptimizeVar>().ToArray();

        /// <inheritdoc />
        public override bool TryResolve()
        {
            try
            {
                using (Solver = new Solver(ModelName))
                {
                    // Capture the Solver instance for local usage.
                    var s = Solver;

                    ReSeed(s);

                    /* Capture the Variables and Constraints. At the same time, Track them as CLR objects,
                     * and Add the Constraints to the Solver. */

                    var variables = Variables.ToArray();

                    foreach (var c in PrepareConstraints(s).ToArray())
                    {
                        s.Add(c.TrackClrObject(this));
                    }

                    var e = EventArgs.Empty;

                    /* It is important that search preparation include allocation of monitor(s) and
                     * DecisionBuilders. However, these cannot truly be known until the moment when
                     * the search is about ready to get under way. */

                    /* Additionally, it is also very important to leave resolution of NewSearch to the
                     * derived class, since we cannot really know what sort of Phase DecisionBuilder
                     * will be required until that moment. */

                    using (var sa = PrepareSearch(variables))
                    {
                        OnResolving(e);

                        NewSearch(sa);
                    }

                    OnResolved(e);
                }
            }
            finally
            {
                Solver = null;
            }
            return true;
        }

        // TODO: TBD: Model Export/Load features are not yet available in the current NuGet published package.
        // TODO: TBD: when they do become available, consider whether there is value exporting the model...

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            // TODO: TBD: dispose of disposable objects here?
        }
    }

    /// <summary>
    /// <typeparamref name="TAspect"/> based Operational Research problem solver.
    /// </summary>
    /// <typeparam name="TAspect"></typeparam>
    /// <inheritdoc cref="ProblemSolverBase{TSolver,TVariable}"/>
    public abstract class OrProblemSolverBase<TAspect>
        : ProblemSolverBase<Solver, Solver, IntVar, Constraint, TAspect>
            , IOrProblemSolver<TAspect>
        where TAspect : IProblemSolverAspect<Solver, Solver, IntVar, Constraint, TAspect>
    {
        /// <inheritdoc />
        /// <remarks>In this case, Source is the Solver.</remarks>
        /// <see cref="ProblemSolverBase{TSolver,TVariable}.Solver"/>
        protected override Solver Source => Solver;

        /// <inheritdoc />
        protected OrProblemSolverBase(string modelName, IEnumerable<TAspect> aspects)
            : base(modelName, aspects)
        {
        }

        /// <summary>
        /// Returns a random seed.
        /// </summary>
        /// <returns></returns>
        protected virtual int GetRandomSeed() => new Random().Next();

        /// <summary>
        /// Reseeds the <paramref name="solver"/>.
        /// </summary>
        /// <param name="solver"></param>
        protected virtual void ReSeed(Solver solver) => solver.ReSeed(GetRandomSeed());

        /// <summary>
        /// Intersects the <paramref name="first"/> with the <paramref name="second"/>, excluding
        /// the one that appeared in the <paramref name="first"/>.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="intersect"></param>
        /// <returns></returns>
        private static IEnumerable<TResult> Intersect<TResult>(IEnumerable<TAspect> first, IEnumerable<TAspect> second,
            Func<TAspect, TAspect, IEnumerable<TResult>> intersect)
        {
            // Get all the B's that are not A and Intersect their Variables.
            return (from a in first.ToArray()
                from b in second.Where(s => !ReferenceEquals(s, a)).ToArray()
                select intersect(a, b)).SelectMany(x => x);
        }

        // TODO: TBD: there are some repeated concerns here as compared/contrasted with the non-Aspect oriented Problem Solver. could look into not duplicating that effort somehow... of for multiple inheritance...
 
        /// <summary>
        /// Override to prepare the <see cref="SearchMonitor"/> or monitors for the
        /// <paramref name="agent"/> and corresponding <see cref="Solver"/>. If called, the base
        /// method should be called after preparing any other required Monitors. By default, a
        /// single <see cref="SolutionCollector"/> is prepared.
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        protected virtual ISearchAgent PrepareSearchMonitors(ISearchAgent agent, params IntVar[] variables)
        {
            if (!agent.HasSolutionCollector<SolutionCollector>())
            {
                // Start with a single all-solution-collector.
                agent.Monitor(a =>
                {
                    var m = a.Solver.MakeAllSolutionCollector();
                    var vector = new IntVarVector().TrackClrObject(this);
                    foreach (var v in variables) vector.Add(v);
                    m.Add(vector);
                    return m;
                });
            }

            return agent;
        }

        /// <summary>
        /// Prepares the <see cref="ISearchAgent"/> corresponding with the solver as a
        /// <see cref="IClrObjectHost"/>.
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
        protected virtual ISearchAgent PrepareSearch(params IntVar[] variables)
        {
            var a = new SearchAgent(this, variables);
            PrepareSearchMonitors(a, variables);
            return a;
        }

        /// <summary>
        /// Solved event.
        /// </summary>
        public event EventHandler<EventArgs> Solved;

        /// <summary>
        /// Raises the <see cref="Solved"/> event.
        /// </summary>
        /// <param name="e"></param>
        private void OnSolved(EventArgs e) => Solved?.Invoke(this, e);

        /// <summary>
        /// <see cref="ISearchAgent"/> ProcessVariables event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnProcessVariables(object sender, ProcessVariablesEventArgs e)
            => OnSolved(EventArgs.Empty);

        /// <summary>
        /// Begins a New Search corresponding with the <see cref="Solver"/> and
        /// <paramref name="agent"/>.
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        protected virtual ISearchAgent NewSearch(ISearchAgent agent)
        {
            agent.ProcessVariables -= OnProcessVariables;
            agent.ProcessVariables += OnProcessVariables;

            return agent.NewSearch(a => a.Solver.MakePhase(a.Variables, IntVarSimple, IntValueSimple));
        }

        /// <inheritdoc />
        public override bool TryResolve()
        {
            try
            {
                using (Solver = new Solver(ModelName))
                {
                    // Capture the Solver instance for local usage.
                    var solver = Solver;
                    var source = Source;

                    ReSeed(solver);

                    /* Capture the Variables and Constraints. At the same time, Track them
                     * as CLR objects, and Add the Constraints to the Solver. */

                    var variables = Variables.ToList();

                    {
                        // Get all the B's that are not A and Intersect their Variables.
                        var intersected = Intersect(Aspects, Aspects, (a, b) => a.IntersectVariables(source, b)).ToArray();

                        // Track these Variables in the scope of the Problem Solver.
                        variables.AddRange(intersected.Select(x => x.TrackClrObject(this)));
                    }

                    /* Now coordinate the Constraints from each of the Aspects,
                     *  and track them with each Aspect. */
                    var constraints = Aspects.SelectMany(a => a.GetConstraints(source).Select(x => x.TrackClrObject(a))).ToList();

                    {
                        // Now, Intersect the Constraints along the same lines.
                        var intersected = Intersect(Aspects, Aspects, (a, b) => a.Intersect(source, b)).ToArray();

                        /* Track these Constraints in the scope of the Problem Solver.
                         * Oops, additionally, track the Constraints not the Array. */
                        constraints.AddRange(intersected.Select(x => x.TrackClrObject(this)));
                    }

                    foreach (var c in constraints)
                    {
                        solver.Add(c);
                    }

                    var e = EventArgs.Empty;

                    /* It is important that search preparation include allocation of monitor(s) and
                     * DecisionBuilders. However, these cannot truly be known until the moment when
                     * the search is about ready to get under way. */

                    /* Additionally, it is also very important to leave resolution of NewSearch to the
                     * derived class, since we cannot really know what sort of Phase DecisionBuilder
                     * will be required until that moment. */

                    using (var sa = PrepareSearch(variables.ToArray()))
                    {
                        OnResolving(e);

                        NewSearch(sa);
                    }

                    OnResolved(e);
                }
            }
            finally
            {
                Solver = null;
            }
            return true;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                // TODO: TBD: dispose of the object here...
            }

            base.Dispose(disposing);
        }
    }
}
