//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Google.OrTools.Sat;
//using Newtonsoft.Json.Linq;
//using Solver = Google.OrTools.ConstraintSolver.Solver;

//// TODO: TBD: actually, I'm not positive, but I think much of this is obsolete with the current versions of Google OrTools...
//namespace Kingdom.Constraints.Sample.Fixturing.Tests
//{
//    using Google.OrTools.ConstraintSolver;
//    using Google.Protobuf;
//    using Xunit;
//    using static FileMode;
//    using static Solver;

//    public class OrToolsTests : TestFixtureBase
//    {
//        private Lazy<int> LazyChooseFirstUnbound { get; set; }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <see cref="Solver.CHOOSE_FIRST_UNBOUND"/>
//        private int ChooseFirstUnbound => LazyChooseFirstUnbound.Value;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <see cref="Solver.ASSIGN_MIN_VALUE"/>
//        private Lazy<int> LazyAssignMinValue { get; set; }

//        private int AssignMinValue => LazyAssignMinValue.Value;

//        private static List<IDisposable> Clr { get; set; }


//        public override void SetUp()
//        {
//            base.SetUp();

//            Clr = Clr ?? new List<IDisposable>();

//            LazyChooseFirstUnbound = new Lazy<int>(() => CHOOSE_FIRST_UNBOUND);
//            LazyAssignMinValue= new Lazy<int>(() => ASSIGN_MIN_VALUE);
//        }

//        public override void TearDown()
//        {
//            foreach (var item in Clr.ToArray())
//            {
//                item.Dispose();
//                Clr.Remove(item);
//            }
//        }

//        public override void TearDownFixture()
//        {
//            if (File.Exists("AnotherSimpleTest.dat"))
//            {
//                File.Delete("AnotherSimpleTest.dat");
//            }

//            base.TearDownFixture();
//        }

//        private class SolverContext : IDisposable
//        {
//            internal Solver Solver { get; }

//            internal IList<IntExpr> Expressions { get; }

//            internal IList<Constraint> Constraints { get; }

//            internal IList<SearchMonitor> Monitors { get; }

//            internal DecisionBuilder DecisionBuilder { get; private set; }

//            internal SolverContext(string modelName)
//            {
//                Solver = new Solver(modelName);
//                Assert.Equal(modelName, Solver.ModelName());
//                Expressions = new List<IntExpr>();
//                Constraints = new List<Constraint>();
//                Monitors = new List<SearchMonitor>();
//            }

//            internal SolverContext Expression<T>(Func<Solver, T> expr)
//                where T : IntExpr
//            {
//                var x = expr(Solver);
//                Expressions.Add(x);
//                Clr.Add(x);
//                return this;
//            }

//            internal SolverContext Constrain(Func<Solver, Constraint> constrain)
//            {
//                var c = constrain(Solver);
//                Solver.Add(c);
//                Constraints.Add(c);
//                Clr.Add(c);
//                return this;
//            }

//            internal SolverContext Monitor(Func<Solver, SearchMonitor> monitor)
//            {
//                var m = monitor(Solver);
//                Clr.Add(m);
//                Monitors.Add(m);
//                return this;
//            }

//            internal SolverContext CreateDecisionBuilder(Func<IEnumerable<IntExpr>, IntVarVector> filter,
//                Func<Solver, IntVarVector,  DecisionBuilder> create)
//            {
//                var vector = filter(Expressions);

//                Clr.Add(vector);

//                if (DecisionBuilder != null)
//                {
//                    Assert.True(Clr.Remove(DecisionBuilder));
//                    DecisionBuilder.Dispose();
//                }

//                DecisionBuilder = create(Solver, vector);
//                Clr.Add(DecisionBuilder);

//                return this;
//            }

//            internal TResult Get<TResult>(Func<Solver, TResult> getter)
//            {
//                return getter(Solver);
//            }

//            internal void NewSearch()
//            {
//                Solver.NewSearch(DecisionBuilder, Monitors.ToArray());
//            }

//            internal bool NextSolution()
//            {
//                return Solver.NextSolution();
//            }

//            internal SolverContext Save(string path)
//            {
//                CpModel model;

//                if (!Monitors.Any())
//                {
//                    model = Solver.ExportModel();
//                }
//                else if (DecisionBuilder == null)
//                {
//                    using (var vector = new SearchMonitorVector())
//                    {
//                        foreach (var m in Monitors) vector.Add(m);
//                        model = Solver.ExportModelWithSearchMonitors(vector);
//                    }
//                }
//                else
//                {
//                    using (var vector = new SearchMonitorVector())
//                    {
//                        foreach (var m in Monitors) vector.Add(m);
//                        model = Solver.ExportModelWithSearchMonitorsAndDecisionBuilder(vector, DecisionBuilder);
//                    }
//                }

//                using (var ws = File.Open(path, Create))
//                {
//                    Assert.NotNull(model);
//                    model.WriteTo(ws);
//                }

//                return this;
//            }

//            internal SolverContext Load(string path)
//            {
//                var model = new CpModel();

//                using (var ms = File.Open(path, Open))
//                {
//                    model.MergeFrom(ms);
//                }

//                // TODO: TBD: careful that ModelLoader is disposable...
//                Assert.That(Solver.ModelLoader(), Is.Null);

//                if (Monitors.Any())
//                {
//                    using (var vector = new SearchMonitorVector())
//                    {
//                        foreach (var monitor in Monitors) vector.Add(monitor);

//                        Assert.That(Solver.LoadModelWithSearchMonitors(model, vector), Is.True);
//                    }
//                }
//                else
//                {
//                    // TODO: TBD: or s.LoadModelWithSearchMonitors; which also implies that somehow the model has been at least prepared, with variables, monitors, and/or decision builder
//                    Assert.That(Solver.LoadModel(model), Is.True);
//                }

//                var loader = Solver.ModelLoader();
//                Assert.That(loader, Is.Not.Null);

//                return this;
//            }

//            public void Dispose()
//            {
//                Solver.Dispose();
//                Monitors.Clear();
//                Expressions.Clear();
//            }
//        }

//        private static SolverContext GetSampleContext(string modelName)
//        {
//            var strats = new {Var = INT_VAR_SIMPLE, Val = INT_VALUE_SIMPLE};

//            const int vmin = 0, vmax = 10;

//            const string xName = "x";
//            const string yName = "y";
//            // TODO: TBD: so apparently naming constraints may not be such a good thing...
//            const string constraintName = "equation";

//            var sc = new SolverContext(modelName);

//            Assert.That(sc.Get(s => s.ConstraintCount()), Is.EqualTo(0));

//            var exprs = sc.Expressions;
//            Assert.That(exprs, Has.Count.EqualTo(0));

//            sc
//                .Expression(s =>
//                {
//                    var x = s.MakeIntVar(vmin, vmax, xName);
//                    Assert.That(x.Name(), Is.EqualTo(xName));
//                    return x;
//                })
//                .Expression(s =>
//                {
//                    var y = s.MakeIntVar(vmin, vmax, yName);
//                    Assert.That(y.Name(), Is.EqualTo(yName));
//                    return y;
//                })
//                ;

//            Assert.That(exprs, Has.Count.EqualTo(2));
//            Assert.That(sc.Constraints, Has.Count.EqualTo(0));

//            sc
//                .Constrain(s =>
//                {
//                    var x = (IntVar) exprs.First(e => e.Name() == xName);
//                    var y = (IntVar) exprs.First(e => e.Name() == yName);

//                    var c = x + y == 5;

//                    Assert.That(c, Is.TypeOf<WrappedConstraint>());
//                    Assert.That(c.Cst.ToString(), Is.EqualTo("((x(0..10) + y(0..10)) == 5)"));

//                    c.Cst.SetName(constraintName);
//                    Assert.That(c.Cst.Name(), Is.EqualTo(constraintName));

//                    return c;
//                })
//                ;

//            Assert.That(sc.Constraints, Has.Count.EqualTo(1));
//            Assert.That(sc.Monitors, Has.Count.EqualTo(0));

//            sc
//                .Monitor(s => s.MakeAllSolutionCollector())
//                ;

//            Assert.That(sc.Monitors, Has.Count.EqualTo(1));
//            Assert.Null(sc.DecisionBuilder);

//            sc
//                .CreateDecisionBuilder(
//                    e => new IntVarVector(e.OfType<IntVar>().ToList())
//                    , (s, v) => s.MakePhase(v, strats.Var, strats.Val)
//                )
//                ;

//            Assert.NotNull(sc.DecisionBuilder);

//            return sc;
//        }

//        //[Test]
//        //public void Tests()
//        //{
//        //    using (var s = new Solver("Test"))
//        //    {
//        //        s.ExportModel(@"G:\Source\Kingdom Software\Kingdom.ConstraintSolvers\Working\src\Kingdom.Constraints.Sample.Fixturing.Tests\ExportedSolverModel.gz");
//        //    }
//        //}

//        private static IntVar GetIntVarFromIntExpr(CpModelLoader loader, string name)
//        {
//            var e = loader.IntegerExpressionByName(name);
//            Assert.That(e, Is.Not.Null);
//            var v = e.Var();
//            Assert.That(v.Name(), Is.EqualTo(name));
//            return v;
//        }

//        // TODO: TBD: ignored for the time being; may reconsider overall merit/value of exporting/loading models when Google.OrTools is published to NuGet
//        [Test]
//        [Ignore]
//        public void VerifySolverModelSerialization()
//        {
//            const string solverName1 = "Test1";
//            const string solverName2 = "Test2";

//            const string path =
//                @"G:\Source\Kingdom Software\Kingdom.ConstraintSolvers\Working\src\Kingdom.Constraints.Sample.Fixturing.Tests\ExportedSolverModel.dat";

//            using (var sc = GetSampleContext(solverName1))
//            {
//                sc.NewSearch();

//                var vars = sc.Expressions.OfType<IntVar>().ToArray();

//                var x = vars.SingleOrDefault(v => v.Name() == "x");
//                var y = vars.SingleOrDefault(v => v.Name() == "y");

//                Assert.That(x, Is.Not.Null);
//                Assert.That(y, Is.Not.Null);

//                var collector = sc.Monitors.OfType<SolutionCollector>().SingleOrDefault();

//                Assert.That(collector, Is.Not.Null);

//                var count = 0;

//                while (sc.NextSolution())
//                {
//                    // ReSharper disable once PossibleNullReferenceException
//                    Assert.That(collector.SolutionCount(), Is.GreaterThan(0));

//                    Assert.That(collector.SolutionCount(), Is.EqualTo(++count));

//                    // ReSharper disable once PossibleNullReferenceException
//                    Console.WriteLine("Solution: {0} + {1} = 5", x.Value(),
//                        // ReSharper disable once PossibleNullReferenceException
//                        y.Value()
//                    );

//                    break;
//                }

//                sc.Save(path);
//            }

//            //const string constraintName = "equation";

//            using (var sc = new SolverContext(solverName2))
//            {
//                sc
//                    .Monitor(s => s.MakeAllSolutionCollector())
//                    ;

//                var collector = sc.Monitors.OfType<SolutionCollector>().SingleOrDefault();

//                sc.Load(path);

//                //Assert.That(sc.Solver.ConstraintCount(), Is.EqualTo(1));

//                var x = GetIntVarFromIntExpr(sc.Solver.ModelLoader(), "x");
//                var y = GetIntVarFromIntExpr(sc.Solver.ModelLoader(), "y");

//                var strats = new {Var = Solver.INT_VAR_SIMPLE, Val = Solver.INT_VALUE_SIMPLE};

//                sc
//                    .Expression(delegate { return x; })
//                    .Expression(delegate { return y; })
//                    .Constrain(s =>
//                    {
//                        var c = x + y == 5;

//                        //c.Cst.SetName(constraintName);
//                        //Assert.That(c.Cst.Name(), Is.EqualTo(constraintName));
//                        Assert.That(c.Cst.Name(), Is.EqualTo(string.Empty));

//                        Assert.That(c, Is.TypeOf<WrappedConstraint>());
//                        Assert.That(c.Cst.ToString(), Is.Not.EqualTo("TrueConstraint()"));
//                        Assert.That(c.Cst.ToString(), Is.EqualTo("((x(0..10) + y(0..10)) == 5)"));

//                        return c;
//                    })
//                    .CreateDecisionBuilder(
//                        e => new IntVarVector(e.OfType<IntVar>().ToList())
//                        , (s, vect) => s.MakePhase(vect, strats.Var, strats.Val)
//                    )
//                    ;

//                // TODO: TBD: new search? or ability to pick up where we left off?
//                sc.NewSearch();

//                // TODO: TBD: currently, "re-loading" does not include state, i.e. previously covered ground, so to speak.
//                var count = 0;

//                while (sc.NextSolution())
//                {
//                    // ReSharper disable once PossibleNullReferenceException
//                    Assert.That(collector.SolutionCount(), Is.GreaterThan(0));

//                    Assert.That(collector.SolutionCount(), Is.EqualTo(++count));

//                    // ReSharper disable once PossibleNullReferenceException
//                    Console.WriteLine("Solution: {0} + {1} = 5", x.Value(),
//                        // ReSharper disable once PossibleNullReferenceException
//                        y.Value()
//                    );

//                    break;
//                }


//                // TODO: TBD: we seem to have the Constraints loaded. What about the variables?
//            }
//        }

//        private static string VerifyJsonText(string modelText)
//        {
//            Assert.That(modelText, Is.Not.Null.And.Not.Empty);
//            TestDelegate parser = () =>
//            {
//                var parsed = JToken.Parse(modelText);
//                Assert.That(parsed, Is.Not.Null);
//            };
//            Assert.That(parser, Throws.Nothing);
//            return modelText;
//        }

//        [Test]
//        public void VerifyThatInMemoryExportToProtoWorks()
//        {
//            CpModel model;

//            // Model name must be the same because loading does not re-set it.
//            const string modelName = "TestModelLoader";

//            string modelText;

//            using (var s = new Solver(modelName))
//            {
//                var x = s.MakeIntVar(0, 10, "x");
//                var y = s.MakeIntVar(0, 10, "y");

//                s.Add(x + y == 5);
//                // Verify that adding one Constraint appears in the Count.
//                Assert.That(s.ConstraintCount(), Is.EqualTo(1));

//                // Capture the ExportedModel textual (JSON) representation.
//                model = s.ExportModel();
//                Assert.That(model, Is.Not.Null);
//                modelText = VerifyJsonText(model.ToString());
//            }

//            using (var s = new Solver(modelName))
//            {
//                Assert.That(s.LoadModel(model), Is.True);

//                // Straight after load the Constraints should report the same number.
//                Assert.That(s.ConstraintCount(), Is.EqualTo(1));

//                // The textual representation must be the same.
//                var actual = s.ExportModel();
//                var actualText = VerifyJsonText(actual.ToString());
//                Assert.That(actualText, Is.EqualTo(modelText));
//            }
//        }

//        [Test]
//        public void VerifyThatInMemoryExportToProtoAfterSolutionFoundWorks()
//        {
//            CpModel model;

//            // Model name must be the same because loading does not re-set it.
//            const string modelName = "TestModelLoader";

//            string modelText;

//            using (var s = new Solver(modelName))
//            {
//                var x = s.MakeIntVar(0, 10, "x");
//                var y = s.MakeIntVar(0, 10, "y");

//                s.Add(x + y == 5);
//                // Verify that adding one Constraint appears in the Count.
//                Assert.That(s.ConstraintCount(), Is.EqualTo(1));

//                var db = s.MakePhase(x, y, ChooseFirstUnbound, AssignMinValue);

//                {
//                    // TODO: TBD: consider adding a disposable search wrapper to hide that detail a bit...
//                    // Ending the new search after next solution block is CRITICAL.
//                    s.NewSearch(db);
//                    while (s.NextSolution())
//                    {
//                        Console.WriteLine($"Found next solution: {x.ToString()} + {y.ToString()} == 5");
//                        break;
//                    }
//                    s.EndSearch();
//                }

//                // Capture the ExportedModel textual (JSON) representation.
//                model = s.ExportModel();
//                Assert.That(model, Is.Not.Null);
//                modelText = VerifyJsonText(model.ToString());
//            }

//            using (var s = new Solver(modelName))
//            {
//                Assert.That(s.LoadModel(model), Is.True);

//                // Straight after load the Constraints should report the same number.
//                Assert.That(s.ConstraintCount(), Is.EqualTo(1));

//                // The textual representation must be the same.
//                var actual = s.ExportModel();
//                var actualText = VerifyJsonText(actual.ToString());
//                Assert.That(actualText, Is.EqualTo(modelText));
//            }
//        }

//        // TODO: TBD: ignored for the time being; may reconsider overall merit/value of exporting/loading models when Google.OrTools is published to NuGet
//        [Test]
//        [Ignore]
//        public void SimpleTestWithSearchMonitorsAndDecisionBuilder()
//        {
//            CpModel model;

//            string modelText;

//            const string modelName = "TestModelLoader";
//            const string equationText = "((x(0..10) + y(0..10)) == 5)";

//            using (var s = new Solver(modelName))
//            {
//                var x = s.MakeIntVar(0, 10, "x");
//                var y = s.MakeIntVar(0, 10, "y");
//                var c = x + y == 5;
//                Assert.That(c.Cst.ToString(), Is.EqualTo(equationText));
//                s.Add(c);
//                Assert.That(s.ConstraintCount(), Is.EqualTo(1));
//                var collector = s.MakeAllSolutionCollector();
//                var db = s.MakePhase(x, y, ChooseFirstUnbound, AssignMinValue);
//                Console.WriteLine("First search...");
//                s.NewSearch(db, collector);
//                while (s.NextSolution())
//                {
//                    Console.WriteLine($"{x.ToString()} + {y.ToString()} == 5");
//                    break;
//                }
//                s.EndSearch();
//                using (var vect = new SearchMonitorVector())
//                {
//                    vect.Add(collector);
//                    model = s.ExportModelWithSearchMonitorsAndDecisionBuilder(vect, db);
//                    modelText = model.ToString();
//                }
//            }

//            using (var s = new Solver(modelName))
//            {
//                // TODO: TBD: load but without any monitors and/or DB ...
//                s.LoadModel(model);

//                var loader = s.ModelLoader();
//                // Do a quick sanity check that we at least have the proper constraint loaded.
//                Assert.That(s.ConstraintCount(), Is.EqualTo(1));

//                var x = loader.IntegerExpressionByName("x").Var();
//                var y = loader.IntegerExpressionByName("y").Var();

//                {
//                    var c = x + y == 5;
//                    // These should PASS as well...
//                    Assert.That(c.Cst.ToString(), Is.Not.EqualTo("TrueConstraint()"));
//                    Assert.That(c.Cst.ToString(), Is.EqualTo(equationText));
//                }

//                {
//                    /* I dare say that THIS should PASS as well, but due to the fact that IntVar and
//                     * derivatives are treated as IntExpr, it is FAILING. */
//                    var actual = s.ExportModel();
//                    Assert.That(actual.ToString(), Is.EqualTo(modelText));
//                }

//                var db = s.MakePhase(x, y, ChooseFirstUnbound, AssignMinValue);
//                Console.WriteLine("Second search...");
//                s.NewSearch(db);
//                while (s.NextSolution())
//                {
//                    Console.WriteLine($"{x.ToString()} + {y.ToString()} == 5");
//                }
//                s.EndSearch();
//            }
//        }

//        [Fact]
//        public void VerifyModelOnlySerializesToFile()
//        {
//            const string path = "AnotherSimpleTest.dat";
//            const string modelName = "TestModelLoader";

//            {
//                using (var s = new Solver(modelName))
//                {
//                    var x = s.MakeIntVar(0, 10, "x");
//                    var y = s.MakeIntVar(0, 10, "y");

//                    var c = x + y == 5;

//                    //c.Cst.SetName("equation");

//                    s.Add(c);

//                    Assert.Equal(1, s.Constraints());

//                    var model = s.ExportModel();

//                    using (var stream = File.Open(path, Create))
//                    {
//                        model.WriteTo(stream);
//                    }
//                }
//            }

//            {
//                var model = new CpModel();

//                using (var s = new Solver(modelName))
//                {
//                    using (var stream = File.Open(path, Open))
//                    {
//                        model.MergeFrom(stream);
//                    }

//                    s.LoadModel(model);

//                    var loader = s.ModelLoader();
//                    Assert.That(loader, Is.Not.Null);

//                    var x = loader.IntegerExpressionByName("x").Var();
//                    var y = loader.IntegerExpressionByName("y").Var();

//                    // Just check that all things are equivalent.
//                    var c = x + y == 5;

//                    Assert.That(c.Cst.ToString(), Is.Not.EqualTo("TrueConstraint()"));

//                    // Constraints should reflect what was actually there.
//                    Assert.That(s.ConstraintCount(), Is.EqualTo(1));
//                }
//            }
//        }
//    }
//}
