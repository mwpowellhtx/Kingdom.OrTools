namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Protobuf;

    /// <summary>
    /// Callback for use during Code Generation. Provides an opportunity to substitute the
    /// <paramref name="inputName"/> with a more suitable <paramref name="outputName"/>. The
    /// chosen strategy (or strategies) for doing so are entirely up to the caller to configure.
    /// </summary>
    /// <param name="inputName"></param>
    /// <param name="outputName"></param>
    /// <returns></returns>
    public delegate bool TryReplaceMemberIdentifierCallback(Identifier inputName, out Identifier outputName);
}
