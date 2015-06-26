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

##

I've modeled the famous [Sudoku](http://en.wikipedia.org/wiki/Sudoku) problem and provided a couple of unit tests that demonstrate the problem solver in action. Besides being blindingly fast on my 8-core desktop machine, the puzzles start from a known problem statement, and which results are reported to ``Console.Out`` upon completion. I haven't timed it yet, but I might like to see just how quickly it does solve, perhaps averaging over some volume of solved problems.

## Conclusions

If there are any other extensible features you would like to see added, by all means do not hesitate to contact me, clone and create a pull request, and I'd be happy to take a look. I try to keep up to date with the latest OrTools developments.
