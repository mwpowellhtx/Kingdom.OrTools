using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.OrTools.ConstraintSolver;

// ReSharper disable once IdentifierTypo
namespace Kingdom.Constraints.Sample.Fixturing.Tests
{
    using Google.OrTools.ConstraintSolver;
    using Google.Protobuf;
    using Newtonsoft.Json.Linq;
    using Xunit;
    using Xunit.Abstractions;
    using static FileMode;
    using static Solver;
    using static String;

    public class OrToolsTests : TestFixtureBase
    {
        private Lazy<int> LazyChooseFirstUnbound { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="CHOOSE_FIRST_UNBOUND"/>
        private int ChooseFirstUnbound => LazyChooseFirstUnbound.Value;

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="ASSIGN_MIN_VALUE"/>
        private Lazy<int> LazyAssignMinValue { get; set; }

        private int AssignMinValue => LazyAssignMinValue.Value;

        private static List<IDisposable> Clr { get; set; }

        public OrToolsTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            Clr = new List<IDisposable>();

            LazyChooseFirstUnbound = new Lazy<int>(() => CHOOSE_FIRST_UNBOUND);
            LazyAssignMinValue = new Lazy<int>(() => ASSIGN_MIN_VALUE);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                foreach (var item in Clr.ToArray())
                {
                    item.Dispose();
                }

                Clr.Clear();
            }

            // TODO: TBD: was part of "fixture tear down" ...
            // TODO: TBD: refactor to a class fixture?
            if (File.Exists("AnotherSimpleTest.dat"))
            {
                File.Delete("AnotherSimpleTest.dat");
            }

            base.Dispose(disposing);
        }

        private class SolverContext : IDisposable
        {
            internal Solver Solver { get; }

            internal IList<IntExpr> Expressions { get; }

            internal IList<Constraint> Constraints { get; }

            internal IList<SearchMonitor> Monitors { get; }

            internal DecisionBuilder DecisionBuilder { get; private set; }

            internal SolverContext(string modelName)
            {
                Solver = new Solver(modelName);
                Assert.Equal(modelName, Solver.ModelName());
                Expressions = new List<IntExpr>();
                Constraints = new List<Constraint>();
                Monitors = new List<SearchMonitor>();
            }

            internal delegate T SolverGetterCallback<out T>(Solver s);

            internal SolverContext Expression<T>(SolverGetterCallback<T> expr)
                where T : IntExpr
            {
                var x = expr(Solver);
                Expressions.Add(x);
                Clr.Add(x);
                return this;
            }

            internal delegate Constraint SolverConstrainCallback(Solver s);

            internal SolverContext Constrain(SolverConstrainCallback constrain)
            {
                var c = constrain(Solver);
                Solver.Add(c);
                Constraints.Add(c);
                Clr.Add(c);
                return this;
            }

            internal delegate SearchMonitor SolverMonitorCallback(Solver s);

            internal SolverContext Monitor(SolverMonitorCallback monitor)
            {
                var m = monitor(Solver);
                Clr.Add(m);
                Monitors.Add(m);
                return this;
            }

            internal SolverContext CreateDecisionBuilder(Func<IEnumerable<IntExpr>, IntVarVector> filter,
                Func<Solver, IntVarVector,  DecisionBuilder> create)
            {
                var vector = filter(Expressions);

                Clr.Add(vector);

                if (DecisionBuilder != null)
                {
                    Assert.True(Clr.Remove(DecisionBuilder));
                    DecisionBuilder.Dispose();
                }

                DecisionBuilder = create(Solver, vector);
                Clr.Add(DecisionBuilder);

                return this;
            }

            internal TResult Get<TResult>(SolverGetterCallback<TResult> getter)
            {
                return getter(Solver);
            }

            internal void NewSearch()
            {
                Solver.NewSearch(DecisionBuilder, Monitors.ToArray());
            }

            internal bool NextSolution()
            {
                return Solver.NextSolution();
            }

            internal SolverContext Save(string path)
            {
                CpModel model;

                if (!Monitors.Any())
                {
                    model = Solver.ExportModel();
                }
                else if (DecisionBuilder == null)
                {
                    using (var vector = new SearchMonitorVector())
                    {
                        foreach (var m in Monitors) vector.Add(m);
                        model = Solver.ExportModelWithSearchMonitors(vector);
                    }
                }
                else
                {
                    using (var vector = new SearchMonitorVector())
                    {
                        foreach (var m in Monitors) vector.Add(m);
                        model = Solver.ExportModelWithSearchMonitorsAndDecisionBuilder(vector, DecisionBuilder);
                    }
                }

                using (var ws = File.Open(path, FileMode.Create))
                {
                    Assert.NotNull(model);
                    model.WriteTo(ws);
                }

                return this;
            }

            internal SolverContext Load(string path)
            {
                var model = new CpModel();

                using (var ms = File.Open(path, Open))
                {
                    model.MergeFrom(ms);
                }

                // TODO: TBD: careful that ModelLoader is disposable...
                Assert.Null(Solver.ModelLoader());

                if (Monitors.Any())
                {
                    using (var vector = new SearchMonitorVector())
                    {
                        foreach (var monitor in Monitors) vector.Add(monitor);

                        Assert.True(Solver.LoadModelWithSearchMonitors(model, vector));
                    }
                }
                else
                {
                    // TODO: TBD: or s.LoadModelWithSearchMonitors; which also implies that somehow the model has been at least prepared, with variables, monitors, and/or decision builder
                    Assert.True(Solver.LoadModel(model));
                }

                var loader = Solver.ModelLoader();
                Assert.NotNull(loader);

                return this;
            }

            public void Dispose()
            {
                Solver.Dispose();
                Monitors.Clear();
                Expressions.Clear();
            }
        }

        private static SolverContext GetSampleContext(string modelName)
        {
            var strategies = new {Var = INT_VAR_SIMPLE, Val = INT_VALUE_SIMPLE};

            const int vMin = 0, vMax = 10;

            const string xName = "x";
            const string yName = "y";
            // TODO: TBD: so apparently naming constraints may not be such a good thing...
            const string constraintName = "equation";

            var sc = new SolverContext(modelName);

            // TODO: TBD: was ConstraintCount?
            Assert.Equal(0, sc.Get(s => s.Constraints()));
            // Assert.That(sc.Get(s => s.ConstraintCount()), Is.EqualTo(0));

            var expressions = sc.Expressions;
            Assert.Empty(expressions);

            sc
                .Expression(s =>
                {
                    var x = s.MakeIntVar(vMin, vMax, xName);
                    Assert.Equal(xName, x.Name());
                    return x;
                })
                .Expression(s =>
                {
                    var y = s.MakeIntVar(vMin, vMax, yName);
                    Assert.Equal(yName, y.Name());
                    return y;
                })
                ;

            Assert.Equal(2, expressions.Count);
            Assert.Empty(sc.Constraints);

            sc
                .Constrain(s =>
                {
                    var x = (IntVar) expressions.First(e => e.Name() == xName);
                    var y = (IntVar) expressions.First(e => e.Name() == yName);

                    var c = x + y == 5;

                    Assert.IsType<WrappedConstraint>(c);
                    Assert.Equal("((x(0..10) + y(0..10)) == 5)", c.Cst.ToString());

                    c.Cst.SetName(constraintName);
                    Assert.Equal(constraintName, c.Cst.Name());

                    return c;
                })
                ;

            Assert.Collection(sc.Constraints, Assert.NotNull);
            Assert.Empty(sc.Monitors);

            sc
                .Monitor(s => s.MakeAllSolutionCollector())
                ;

            Assert.Collection(sc.Monitors, Assert.NotNull);
            Assert.Null(sc.DecisionBuilder);

            sc
                .CreateDecisionBuilder(
                    e => new IntVarVector(e.OfType<IntVar>().ToList())
                    , (s, v) => s.MakePhase(v, strategies.Var, strategies.Val)
                )
                ;

            Assert.NotNull(sc.DecisionBuilder);

            return sc;
        }

        //[Test]
        //public void Tests()
        //{
        //    using (var s = new Solver("Test"))
        //    {
        //        s.ExportModel(@"G:\Source\Kingdom Software\Kingdom.ConstraintSolvers\Working\src\Kingdom.Constraints.Sample.Fixturing.Tests\ExportedSolverModel.gz");
        //    }
        //}

        private static IntVar GetIntVarFromIntExpr(CpModelLoader loader, string name)
        {
            var e = loader.IntegerExpressionByName(name);
            Assert.NotNull(e);
            var v = e.Var();
            Assert.Equal(name, v.Name());
            return v;
        }

        // TODO: TBD: this one may have been tripping up over Constraint naming issues in the actual failure...
#pragma warning disable xUnit1004 // Test methods should not be skipped
        [Fact(Skip = "I am uncertain why this one was ignored, but I may reconsider downstream from here")]
#pragma warning restore xUnit1004 // Test methods should not be skipped
        public void VerifySolverModelSerialization()
        {
            const string solverName1 = "Test1";
            const string solverName2 = "Test2";
            const string path = "ExportedSolverModel.dat";
            using (var sc = GetSampleContext(solverName1))
            {
                sc.NewSearch();
                var vars = sc.Expressions.OfType<IntVar>().ToArray();
                var x = vars.SingleOrDefault(v => v.Name() == "x");
                var y = vars.SingleOrDefault(v => v.Name() == "y");
                Assert.NotNull(x);
                Assert.NotNull(y);
                var collector = sc.Monitors.OfType<SolutionCollector>().SingleOrDefault();
                Assert.NotNull(collector);
                var count = 0;
                while (sc.NextSolution())
                {
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.True(collector.SolutionCount() > 0);
                    Assert.Equal(++count, collector.SolutionCount());

                    WriteLine($"Solution: {x.Value()} + {y.Value()} = 5");
                    break;
                }

                sc.Save(path);
            }

            //const string constraintName = "equation";
            using (var sc = new SolverContext(solverName2))
            {
                sc
                    .Monitor(s => s.MakeAllSolutionCollector())
                    ;
                var collector = sc.Monitors.OfType<SolutionCollector>().SingleOrDefault();
                sc.Load(path);
                //Assert.That(sc.Solver.ConstraintCount(), Is.EqualTo(1));
                var x = GetIntVarFromIntExpr(sc.Solver.ModelLoader(), "x");
                var y = GetIntVarFromIntExpr(sc.Solver.ModelLoader(), "y");
                var strategies = new {Var = INT_VAR_SIMPLE, Val = INT_VALUE_SIMPLE};
                sc
                    // ReSharper disable once ImplicitlyCapturedClosure
                    .Expression(delegate { return x; })
                    // ReSharper disable once ImplicitlyCapturedClosure
                    .Expression(delegate { return y; })
                    .Constrain(s =>
                    {
                        var c = x + y == 5;
                        //c.Cst.SetName(constraintName);
                        //Assert.That(c.Cst.Name(), Is.EqualTo(constraintName));
                        Assert.Equal(Empty, c.Cst.Name());
                        Assert.IsType<WrappedConstraint>(c);
                        WriteLine($"c.Cst is actually [[ {c.Cst.ToString()} ]]");
                        /* TODO: TBD: the failure:
                         * Assert.NotEqual() Failure
                         * Expected: Not "TrueConstraint()"
                         * Actual:   "TrueConstraint()" */
                        Assert.NotEqual("TrueConstraint()", c.Cst.ToString());
                        Assert.Equal("((x(0..10) + y(0..10)) == 5)", c.Cst.ToString());
                        return c;
                    })
                    .CreateDecisionBuilder(
                        e => new IntVarVector(e.OfType<IntVar>().ToList())
                        , (s, v) => s.MakePhase(v, strategies.Var, strategies.Val)
                    )
                    ;
                // TODO: TBD: new search? or ability to pick up where we left off?
                sc.NewSearch();
                // TODO: TBD: currently, "re-loading" does not include state, i.e. previously covered ground, so to speak.
                var count = 0;
                while (sc.NextSolution())
                {
                    // ReSharper disable once PossibleNullReferenceException
                    Assert.True(collector.SolutionCount() > 0);
                    Assert.Equal(++count, collector.SolutionCount());
                    // ReSharper disable once PossibleNullReferenceException
                    WriteLine($"Solution: {x.Value()} + {y.Value()} = 5");
                    break;
                }

                // TODO: TBD: we seem to have the Constraints loaded. What about the variables?
            }
        }

        private static string VerifyJsonText(string modelText)
        {
            Assert.NotNull(modelText);
            Assert.NotEmpty(modelText);
            Assert.NotNull(JToken.Parse(modelText));
            return modelText;
        }

        [Fact]
        public void VerifyThatInMemoryExportToProtoWorks()
        {
            CpModel model;

            // Model name must be the same because loading does not re-set it.
            const string modelName = "TestModelLoader";

            string modelText;

            using (var s = new Solver(modelName))
            {
                var x = s.MakeIntVar(0, 10, "x");
                var y = s.MakeIntVar(0, 10, "y");

                s.Add(x + y == 5);
                // Verify that adding one Constraint appears in the Count.
                Assert.Equal(1, s.Constraints());

                // Capture the ExportedModel textual (JSON) representation.
                Assert.NotNull(model = s.ExportModel());
                modelText = VerifyJsonText(model.ToString());
            }

            using (var s = new Solver(modelName))
            {
                Assert.True(s.LoadModel(model));

                // Straight after load the Constraints should report the same number.
                Assert.Equal(1, s.Constraints());

                // The textual representation must be the same.
                var actual = s.ExportModel();
                var actualText = VerifyJsonText(actual.ToString());
                Assert.Equal(modelText, actualText);
            }
        }

        [Fact]
        public void VerifyThatInMemoryExportToProtoAfterSolutionFoundWorks()
        {
            CpModel model;

            // Model name must be the same because loading does not re-set it.
            const string modelName = "TestModelLoader";

            string modelText;

            using (var s = new Solver(modelName))
            {
                var x = s.MakeIntVar(0, 10, "x");
                var y = s.MakeIntVar(0, 10, "y");

                s.Add(x + y == 5);
                // Verify that adding one Constraint appears in the Count.
                Assert.Equal(1, s.Constraints());

                var db = s.MakePhase(x, y, ChooseFirstUnbound, AssignMinValue);

                {
                    // TODO: TBD: consider adding a disposable search wrapper to hide that detail a bit...
                    // Ending the new search after next solution block is CRITICAL.
                    s.NewSearch(db);
                    while (s.NextSolution())
                    {
                        Console.WriteLine($"Found next solution: {x.ToString()} + {y.ToString()} == 5");
                        break;
                    }
                    s.EndSearch();
                }

                // Capture the ExportedModel textual (JSON) representation.
                Assert.NotNull(model = s.ExportModel());
                modelText = VerifyJsonText(model.ToString());
            }

            using (var s = new Solver(modelName))
            {
                Assert.True(s.LoadModel(model));

                // Straight after load the Constraints should report the same number.
                Assert.Equal(1, s.Constraints());

                // The textual representation must be the same.
                var actual = s.ExportModel();
                var actualText = VerifyJsonText(actual.ToString());
                Assert.Equal(modelText, actualText);
            }
        }

        // TODO: TBD: plan on migrating to CP-SAT solver as a path from CP solver...
        // TODO: TBD: anticipating v6.10 (minor fixes) as well as v7 (including heavy CP-SAT work)
#pragma warning disable xUnit1004 // Test methods should not be skipped
        [Fact(Skip = "Bypassing this unit test for the time being, not least of which because there have definitely been some changes since I last looking it")]
#pragma warning restore xUnit1004 // Test methods should not be skipped
        public void SimpleTestWithSearchMonitorsAndDecisionBuilder()
        {
            CpModel model;

            string modelText;

            const string modelName = "TestModelLoader";
            const string equationText = "((x(0..10) + y(0..10)) == 5)";

            using (var s = new Solver(modelName))
            {
                var x = s.MakeIntVar(0, 10, "x");
                var y = s.MakeIntVar(0, 10, "y");
                var c = x + y == 5;
                Assert.Equal(equationText, $"{c.Cst.ToString()}");
                s.Add(c);
                Assert.Equal(1, s.Constraints());
                var collector = s.MakeAllSolutionCollector();
                var db = s.MakePhase(x, y, ChooseFirstUnbound, AssignMinValue);
                Console.WriteLine("First search...");
                s.NewSearch(db, collector);
                while (s.NextSolution())
                {
                    Console.WriteLine($"{x.ToString()} + {y.ToString()} == 5");
                    break;
                }
                s.EndSearch();
                using (var vector = new SearchMonitorVector())
                {
                    vector.Add(collector);
                    model = s.ExportModelWithSearchMonitorsAndDecisionBuilder(vector, db);
                    modelText = model.ToString();
                }
            }

            using (var s = new Solver(modelName))
            {
                // TODO: TBD: load but without any monitors and/or DB ...
                s.LoadModel(model);

                var loader = s.ModelLoader();
                // Do a quick sanity check that we at least have the proper constraint loaded.
                Assert.Equal(1, s.Constraints());

                var x = loader.IntegerExpressionByName("x").Var();
                var y = loader.IntegerExpressionByName("y").Var();

                {
                    var c = x + y == 5;
                    // These should PASS as well...
                    Assert.NotEqual("TrueConstraint()", c.Cst.ToString());
                    Assert.Equal(equationText, c.Cst.ToString());
                }

                {
                    /* I dare say that THIS should PASS as well, but due to the fact that IntVar and
                     * derivatives are treated as IntExpr, it is FAILING. */
                    var actual = s.ExportModel();
                    WriteLine($"{nameof(modelText)} == [[ {modelText} ]]");
                    WriteLine($"{nameof(actual)} == [[ {actual} ]]");
                    Assert.Equal(modelText, $"{actual}");
                }

                var db = s.MakePhase(x, y, ChooseFirstUnbound, AssignMinValue);
                WriteLine("Second search...");
                s.NewSearch(db);
                while (s.NextSolution())
                {
                    WriteLine($"{x.ToString()} + {y.ToString()} == 5");
                }
                s.EndSearch();
            }
        }

        [Fact]
        public void VerifyModelOnlySerializesToFile()
        {
            const string path = "AnotherSimpleTest.dat";
            const string modelName = "TestModelLoader";

            {
                using (var s = new Solver(modelName))
                {
                    var x = s.MakeIntVar(0, 10, "x");
                    var y = s.MakeIntVar(0, 10, "y");

                    var c = x + y == 5;

                    //c.Cst.SetName("equation");

                    s.Add(c);

                    Assert.Equal(1, s.Constraints());

                    var model = s.ExportModel();

                    using (var stream = File.Open(path, FileMode.Create))
                    {
                        model.WriteTo(stream);
                    }
                }
            }

            {
                var model = new CpModel();

                using (var s = new Solver(modelName))
                {
                    using (var stream = File.Open(path, Open))
                    {
                        model.MergeFrom(stream);
                    }

                    s.LoadModel(model);

                    var loader = s.ModelLoader();
                    Assert.NotNull(loader);

                    var x = loader.IntegerExpressionByName("x").Var();
                    var y = loader.IntegerExpressionByName("y").Var();

                    // Just check that all things are equivalent.
                    var c = x + y == 5;

                    Assert.NotEqual("TrueConstraint()", c.Cst.ToString());

                    // Constraints should reflect what was actually there.
                    Assert.Equal(1, s.Constraints());
                }
            }
        }
    }
}
