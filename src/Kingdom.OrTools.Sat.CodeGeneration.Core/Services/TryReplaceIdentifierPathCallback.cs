namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Protobuf;

    /// <summary>
    /// Callback for use during Code Generation. Provides an opportunity to substitute
    /// the <paramref name="inputPath"/> with the result <paramref name="outputPath"/>.
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <returns></returns>
    public delegate bool TryReplaceIdentifierPathCallback(IdentifierPath inputPath
        , out IdentifierPath outputPath);
}
