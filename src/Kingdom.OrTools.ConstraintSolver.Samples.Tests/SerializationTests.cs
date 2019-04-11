//using System;

namespace Kingdom.OrTools.ConstraintSolver.Samples
{
    //using Google.OrTools.ConstraintSolver;
    //using Xunit;

    /// <summary>
    /// 
    /// </summary>
    public class SerializationTests /*: TestFixtureBase*/
    {
        //// TODO: TBD: the serialization bits are just not applicable any longer, I think...
        ///// <summary>
        ///// 
        ///// </summary>
        //[Fact]
        //public void Testing()
        //{
        //    const string exportFile = "this is a test";

        //    using (var s = new Solver("Test"))
        //    {
        //        var p1 = s.Parameters();
        //        Assert.NotNull(p1);

        //        Assert.That(p1.ExportFile, Is.EqualTo(string.Empty));
        //        p1.ExportFile = exportFile;
        //        Assert.That(p1.ExportFile, Is.EqualTo(exportFile));

        //        var p2 = s.Parameters();
        //        Assert.NotNull(p2);

        //        // Given the underlying SWIG, I would not expect this to be the case.
        //        // In other words, two different reference proxies are generated.
        //        Assert.That(p2, Is.Not.SameAs(p1));

        //        // However, I **WOULD** expect that this be the case.
        //        Assert.That(p2.ExportFile, Is.EqualTo(exportFile));
        //        Assert.That(p1.ExportFile, Is.EqualTo(p2.ExportFile));

        //        // But given the fact that a **COPY** of parameters_ is being returned,
        //        // what we in fact end up with is no-change, from a language perspective.
        //    }
        //}
    }
}
