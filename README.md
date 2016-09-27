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

##

I've modeled the famous [Sudoku](http://en.wikipedia.org/wiki/Sudoku) problem and provided a couple of unit tests that demonstrate the problem solver in action. Besides being blindingly fast on my 8-core desktop machine, the puzzles start from a known problem statement, and which results are reported to ``Console.Out`` upon completion. I haven't timed it yet, but I might like to see just how quickly it does solve, perhaps averaging over some volume of solved problems.

## Conclusions

If there are any other extensible features you would like to see added, by all means do not hesitate to contact me, clone and create a pull request, and I'd be happy to take a look. I try to keep up to date with the latest OrTools developments.
