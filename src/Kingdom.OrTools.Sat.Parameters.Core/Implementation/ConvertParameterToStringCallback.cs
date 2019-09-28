namespace Kingdom.OrTools.Sat.Parameters
{
    /// <summary>
    /// Callback used to Convert the <paramref name="value"/> to <see cref="string"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate string ConvertParameterToStringCallback<in T>(T value);
}
