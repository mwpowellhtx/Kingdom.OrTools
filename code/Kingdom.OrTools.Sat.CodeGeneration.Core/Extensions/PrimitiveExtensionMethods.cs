namespace Kingdom.OrTools.Sat.CodeGeneration
{
    internal static class PrimitiveExtensionMethods
    {
        public static double ReinterpretFloat(this object value) => float.Parse($"{value}");

        public static double ReinterpretDouble(this object value) => double.Parse($"{value}");

        public static int ReinterpretInteger(this object value) => int.Parse($"{value}");

        public static uint ReinterpretUnsignedInteger(this object value) => (uint) long.Parse($"{value}");

        public static long ReinterpretLong(this object value) => long.Parse($"{value}");

        /// <summary>
        /// Reinterprets the <see cref="object"/> <paramref name="value"/> using a scenic route.
        /// We have to do it this way because there is no clear alternative, no primitive large
        /// enough to handle the intermediate result otherwise.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ulong ReinterpretUnsignedLong(this object value)
        {
            var result = 0UL;
            var s = $"{value}";
            for (var i = s.Length - 1; i >= 0; --i)
            {
                result += (ulong) $"{s[i]}".ReinterpretInteger();
            }

            return result;
        }

        public static string ReinterpretString(this object value) => $"{value}";
    }
}
