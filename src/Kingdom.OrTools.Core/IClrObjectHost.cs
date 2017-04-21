using System.Collections.Generic;

namespace Kingdom.OrTools
{
    /// <summary>
    /// Clr object host interface.
    /// </summary>
    public interface IClrObjectHost
    {
        /// <summary>
        /// Gets the ClrCreatedObjects.
        /// </summary>
        IList<object> ClrCreatedObjects { get; }
    }

    /// <summary>
    /// Provides some useful extension methods for purposes of tracking Clr created objects.
    /// </summary>
    public static class ClrCreatedObjectExtensionMethods
    {
        /// <summary>
        /// Tracks the <paramref name="obj"/> given the <paramref name="host"/>. Returns the
        /// <paramref name="obj"/> for use afterwards.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public static T TrackClrObject<T>(this T obj, IClrObjectHost host)
        {
            host.ClrCreatedObjects.Add(obj);
            return obj;
        }
    }
}
