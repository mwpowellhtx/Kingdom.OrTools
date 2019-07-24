namespace Kingdom.OrTools.Sat.Parameters
{
    /// <summary>
    /// Provides a nominal set of useful Characters for use throughout the API.
    /// </summary>
    internal static class Characters
    {
        /// <summary>
        /// &apos;:&apos;
        /// </summary>
        public const char Colon = ':';

        /// <summary>
        /// &apos; &apos;
        /// </summary>
        public const char Space = ' ';

        /// <summary>
        /// &apos;;&apos;
        /// </summary>
        public const char SemiColon = ';';

        /// <summary>
        /// &apos;,&apos;
        /// </summary>
        public const char Comma = ',';

        /// <summary>
        /// &apos;=&apos;
        /// </summary>
        public const char Equal = '=';

        /// <summary>
        /// &quot;[]&quot;
        /// </summary>
        internal const string SquareBrackets = "[]";

        /// <summary>
        /// &apos;[&apos;
        /// </summary>
        public static char LeftSquareBracket => SquareBrackets[0];

        /// <summary>
        /// &apos;]&apos;
        /// </summary>
        public static char RightSquareBracket => SquareBrackets[1];
    }
}
