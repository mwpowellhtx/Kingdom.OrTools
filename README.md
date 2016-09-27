# Kingdom.ConstraintSolvers

## Overview

Provides an easy to use wrapper for the [Google OrTools](http://github.com/google/or-tools).

A lot of what one faces integrating OrTools within .NET involves callbacks and garbage collection, among other idiosyncocies. Additionally, the wrapper provides an easy to extend interface that guides the modeler step-by-step in the main points.

## How To Use

It's quite simple, really. Let's say you have a solver modeled and ready to go, called ``TravelPlansProblemSolver``. Usage is quite simple.

```C#
using (var tpps = new TravelPlansProblemSolver())
{
    tpps.Resolve();
}
```

I have also included a ``TryResolve`` feature, which makes it easier to assert when things are successful.


```C#
using (var tpps = new TravelPlansProblemSolver())
{
    Debug.Assert(tpps.TryResolve());
}
```

That's it. Pretty straightforward.

After that, you are free to inject whatever data, services or controllers as you see fit; or connect with whatever events or other hooks you would like in order to report progress and outcomes from problem solver with your intended audience.

## Sudoku Example

I've modeled the famous [Sudoku](http://en.wikipedia.org/wiki/Sudoku) problem and provided a couple of unit tests that demonstrate the problem solver in action. Besides being blindingly fast on my 8-core desktop machine, the puzzles start from a known problem statement, and which results are reported to ``Console.Out`` upon completion. I haven't timed it yet, but I might like to see just how quickly it does solve, perhaps averaging over some volume of solved problems.

## Linear Solver

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

#Property#Type#Description#
|```SolutionEventArgs.VariableCount```|``System.Int32``|Gets the ``Solver.NumVariables`` modeled|
|```SolutionEventArgs.ConstraintCount```|``System.Int32``|Gets the ``Solver.NumConstraints`` modeled|
|```SolutionEventArgs.ResultStatus```|``System.Int32``|Gets the ``Solver.Solve`` result|
|```SolutionEventArgs.Solution```|``System.Double``|Gets the ``Solver.Solve`` result|
|```SolutionEventArgs.SolutionValues```|``dynamic`` (*ExpandoObject*) |Gets the [``ExpandoObject``](http://msdn.microsoft.com/en-us/library/system.dynamic.expandoobject.aspx) ``SolutionValues`` result|

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
