# Kingdom.OrTools

This repository provides an easy to use wrapper for the [Google Operational Research Tools](http://github.com/google/or-tools), or *OrTools* for short.

The vision here is to *configure* the problem solver, which amounts to making some *variables*, *constraints*, *optimizations*, and so on, prepare the *search*, and respond to resulting *assignments* and/or *variables* constituting each *solution*.

At present I have re-factored the *search* capability and am considering how best to conduct the *configuration* aspects. Virtually overridden methods are employed at present, however, in my opinion this is not directed enough. But for now the repo is at least stable such as it is.

## Constraint Problem Solver Usage

I have published [Kingdom.OrTools.ConstraintSolver.Core](http://www.nuget.org/packages/Kingdom.OrTools.ConstraintSolver.Core/) and [Kingdom.OrTools.Core](https://www.nuget.org/packages/Kingdom.OrTools.Core/) for public consumption. Comments, issues, and otherwise constructive contributions are welcome.

### Derive from ``OrProblemSolverBase``

In it's present form, start by deriving from the base class.

```C#
public class MyProblemSolver : OrProblemSolverBase
{
    public MyProblemSolver(string modelName)
        : base(modelName)
    {
    }
}
```

For future reference, I am considering whether to expose problem solver, or simply expose a *solution* *assignment* and/or *variables* via configuration.

#### Re-seed the problem solver

The default implementation passes a the value from ``System.Random.Next()`` to ``Google.OrTools.ConstraintSolver.Solver.ReSeed()``. However, if you have a different strategy in mind, then you may override ``protected virtual void ReSeed(Solver solver)``. Or you may also override the ``public virtual int GetSolverSeed()``.

#### Prepare the *variables* and *constraints*

Preparing *variables* and *constraints* is a little plainer in that we will expect an enumeration of returned *variables* and *constraints*.

For starters, we will expose specific problem variables for use throughout preparation. We will track the specific *variables* themselves for convenience during the example:

```C#
private IntVar X { get; set; }
private IntVar Y { get; set; }
```

Now, override ``protected abstract IEnumerable<IntVar> PrepareVariables(Solver solver)`` in order to prepare the *variables* involved in the solver model:

```C#
protected override IEnumerable<IntVar> PrepareVariables(Solver solver)
{
    yield return X = solver.MakeIntVar(0, 10, "x");
    yield return Y = solver.MakeIntVar(0, 10, "y");
}
```

Also override ``protected abstract IEnumerable<Constraint> PrepareConstraints(Solver solver)`` in order to prepare the *constraints* involved in the solver model:

```C#
protected override IEnumerable<Constraint> PrepareVariables(Solver solver)
{
    yield return X + Y == 5;
}
```

For future reference, due to the nature of the call, there's nothing stopping the undisciplined from preparing both variables and/or constraints, or anything else ``Google.OrTools.ConstraintSolver.Solver`` related. This is something I'm not as happy with, so I want to re-consider a more directed ``configuration`` based approach.

#### Prepare the *search monitors*

The default *search monitor* is the all-solutions ``Google.OrTools.ConstraintSolver.SolutionCollector``, which is a kind of *search monitor*. Override ``protected virtual ISearchAgent PrepareSearchMonitors(ISearchAgent agent, params IntVar[] variables)`` in order to prepare this or additional search monitors. Invoke the base class virtual method in order to include the default *search monitor*:

```C#
protected virtual ISearchAgent PrepareSearchMonitors(ISearchAgent agent, params IntVar[] variables)
{
    if (!agent.HasSolutionCollector<Google.OrTools.ConstraintSolver.SolutionCollector>())
    {
        // Start with a single all-solution-collector.
        agent.Monitor(a =>
        {
            var m = a.Solver.MakeAllSolutionCollector();
            foreach (var v in variables) m.Add(v);
            return m;
        });
    }

    return agent;
}
```

We introduced an ``ISearchAgent`` which encapsulates key search boundary ingredients, including specifying search monitors, and conducting the search over the ``Google.OrTools.ConstraintSolver.Solver.NewSearch(...)`` to ``Google.OrTools.ConstraintSolver.Solver.EndSearch()`` boundary.

Additionally, unlike *variables* or *constraints*, *optimizations* are a kind of ``Google.OrTools.ConstraintSolver.SearchMonitor``. Optimizations can include directives such as *minimizing* or *maximizing* some *variable(s)* or *expression(s)*.

For future reference, even this effort is a bit more undisciplined than I would like it to be. I would like to design this to make it evident that we are expecting one or more *search monitors*.

#### Prepare the *search*

The default behavior is to prepare the ``ISearchAgent``, which includes the prepared *variables*. Override ``protected virtual ISearchAgent PrepareSearch(Solver solver, params IntVar[] variables)`` in order to take additional preparation steps, however, it is expected that the base method be called with appropriate timing to your problem solver:

```C#
protected virtual ISearchAgent PrepareSearch(params IntVar[] variables)
{
    var a = new SearchAgent(this, variables);
    PrepareSearchMonitors(a, variables);
    return a;
}
```

Any overrides of this method should at least invoke the base, bare minimum.

#### Conduct a *new search*

The default behavior prepares a ``Google.OrTools.ConstraintSolver.DecisionBuilder`` with ``Kingdom.OrTools.IntVarStrategy.IntVarSimple`` and ``Kingdom.OrTools.IntValueStrategi.IntValueSimple`` strategies, which map to ``Google.OrTools.ConstraintSolver.Solver.INT_VAR_SIMPLE`` and ``Google.OrTools.ConstraintSolver.Solver.INT_VALUE_SIMPLE``, respectively.

Override ``protected virtual ISearchAgent NewSearch(ISearchAgent agent)`` in order to specify a different *decision builder*. This is heavily leveraged against the provided ``ISearchAgent``, which includes the associated ``ISearchAgent.Solver``, ``ISearchAgent.Variables``, and so on:

```C#
using static IntVarStrategy;
using static IntValueStrategy;

protected virtual ISearchAgent NewSearch(ISearchAgent agent)
{
    agent.ProcessVariables -= OnProcessVariables;
    agent.ProcessVariables += OnProcessVariables;

    return agent.NewSearch((ISearchAgent a) => a.Solver.MakePhase(a.Variables, IntVarSimple, IntValueSimple));
}
```

Also critical is that the base method connects the ``protected virtual void OnProcessVariables(object sender, ProcessVariablesEventArgs e)`` event handler to the ``ISearchAgent.ProcessVariables`` event. Additionally, the default ``OnProcessVariables`` event handler invokes the ``OrProblemSolverBase.Solved`` event.

```C#
protected override void OnProcessVariables(object sender, ProcessVariablesEventArgs e)
{
    // Report the solution to the console for purposes of this example.
    Console.WriteLine($"{X.ToString()} + {Y.ToString()} == 5");

    base.OnProcessVariables(sender, e);
}
```

### ***Critical: Tracking CLR objects***

During the search process, the CLR can lose track of objects, which results in unexpected behavior, ``AccessViolationExceptions``, and the like. This is due to the wrapping of unmanaged SWIG-generated C++ resources. To my knowledge, the remedy for this issue is to track mission critical objects made out of the ``Google.OrTools.ConstraintSolver.Solver`` somehow, whether in property, field, or collection.

I have provided a value-added solution, ``IClrObjectHost``, which is implemented by the ``ProblemSolverBase``. When you prepare *variables*, *constraints*, *decision builder*, and so on, make sure to invoke the ``TrackClrObject`` extension method, which collects trackable objects for use during the search.

It is these tracked objects from which *Variables* and *Optimizations* and so forth may be reported for preparation steps.

Now, with the background being established, your task when you adopt this approach is to focus on your variables and constraints and, under specialized conditions, your search monitors, optimizations, and decision builder. The base class will handle tracking the CLR objects for you when they are received by the base class.

### Pulling it all together

Last but not least, let's actually use ``MyProblemSolver``.

```C#
using (var ps = new MyProblemSolver())
{
    Debug.Assert(ps.TryResolve());
}
```

Usage is pretty straightforward, really, with this approach. The defaults, details, and other problem solving concerns will have been neatly tucked. In practice this could run in something like a console or service process, worker thread, or other concurrent parallelism, potentially.

## Sudoku Example

I've prepared a problem solver for the famous [Sudoku](http://en.wikipedia.org/wiki/Sudoku) problem and provided a couple of unit tests that demonstrate the problem solver in action. Besides being blindingly fast on my 8-core desktop machine, the puzzles start from a known problem statement, and which results are reported to ``Console.Out`` upon completion. I haven't timed it yet, but I might like to see just how quickly it does solve, perhaps averaging over some volume of solved problems.

## Linear Solver Problem Solver

*Caution, the Linear Solver approach has been re-factored into a separate assembly. I do not foresee backtracking on that decision, and for the time being Linear Solver is on the shelf for my purposes. I may revisit it at a later date, but for now my focus is on refining the ConstraintSolver problem solver approach.*

The latest version adds support for the Google OrTools LinearSolver. The adapter fits along the same lines as for the ConstraintSolver, as a disposable usage.

```C#
using (var frps = new FeasibleRegionProblemSolver())
{
    frps.Solved += (sender, e) =>
    {
        // Do something with the (SolutionEventArgs e)
    };

    Debug.Assert(frps.TryResolve());
}
```

### ``SolutionEventArgs``

Besides being able to ``VerifySolution`` ``LinearResultStatus``, ``SolutionEventArgs`` is populated with a couple of key elements.

Property|Type|Description
--------|----|-----------
``SolutionEventArgs.VariableCount``|``System.Int32``|Gets the ``Solver.NumVariables`` modeled
``SolutionEventArgs.ConstraintCount``|``System.Int32``|Gets the ``Solver.NumConstraints`` modeled
``SolutionEventArgs.ResultStatus``|``System.Int32``|Gets the ``Solver.Solve`` result
``SolutionEventArgs.Solution``|``System.Double``|Gets the ``Solver.Solve`` result
``SolutionEventArgs.SolutionValues``|``dynamic`` (*ExpandoObject*) |Gets the [``ExpandoObject``](http://msdn.microsoft.com/en-us/library/system.dynamic.expandoobject.aspx) ``SolutionValues`` result

There is also a *without solution* version of the problem solver examples, but I am not sure why anyone would want one without a calculated solution, per se, even one populated with a default value.

The key member of the event arguments is the ``dynamic`` ``SolutionValues``, which is a dynamic container of the actual values of the solution. When you prepare your model variables, you have an opportunity to connect these variables with an ``ExpandoObject`` instance via calls to ``SetProblemComponent``. It is up to you, the modeler, to keep the names and types of variables clear when you prepare them.

### An Example

Let's consider an example:

```C#
var x = solver.MakeNumVar(NegativeInfinity, PositiveInfinity, "x");
SetProblemComponent(x, (p, c) => p.x = c);
```

Also, if the ``LinearSolver`` is anything like the ``ConstraintSolver``, then you likely need to keep a reference to any variables and constraints. This is because any disposable instances will be invalidated as the decision builder decision tree is resolved.

```C#
ClrCreatedObjects.Add(x);
```

Afterwards, assuming that your model has a solution, you will have an ``e.SolutionValues.x`` that you may report on or otherwise use further in your solution.

### Optimization Problem Types

At the moment, there is support for three programming options. I am not set up to custom build Google OrTools for any other the other models at this time. If you have a special requirement for that, contact me offline and let's discuss how we can best integrate that dependency.

The solver options include:

```C#
public enum OptimizationProblemType
{
    // Solver.GLOP_LINEAR_PROGRAMMING
    GlopLinearProgramming,
    // Solver.CLP_LINEAR_PROGRAMMING
    ClpLinearProgramming,
    // Solver.CBC_MIXED_INTEGER_PROGRAMMING
    CbcMixedIntegerProgramming
}
```

### Linear Result Statuses

And results may be tested against the following results:

```C#
public enum LinearResultStatus
{
    // Solver.OPTIMAL
    Optimal,
    // Solver.FEASIBLE
    Feasible,
    // Solver.INFEASIBLE
    Infeasible,
    // Solver.ABNORMAL
    Abnormal,
    // Solver.NOT_SOLVED
    NotSolved,
    // Solver.UNBOUNDED
    Unbounded
}
```

## Conclusions

If there are any other extensible features you would like to see added, by all means do not hesitate to contact me, clone and create a pull request, and I'd be happy to take a look. I try to keep up to date with the latest OrTools developments.
