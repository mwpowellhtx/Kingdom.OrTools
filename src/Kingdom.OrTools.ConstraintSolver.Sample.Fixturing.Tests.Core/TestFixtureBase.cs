using System;
using System.Linq;

// ReSharper disable once IdentifierTypo
namespace Kingdom.Constraints.Sample.Fixturing.Tests
{
    using Xunit.Abstractions;

    public abstract class TestFixtureBase : IDisposable
    {
        private ITestOutputHelper OutputHelper { get; }

        protected void WriteLine(string message, params object[] args)
        {
            if (!args.Any())
            {
                OutputHelper.WriteLine(message);
            }
            else
            {
                OutputHelper.WriteLine(message, args);
            }
        }

        protected TestFixtureBase(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        protected bool IsDisposed { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            IsDisposed = true;
        }
    }
}
