using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters
{
    internal class IndividualParameterTestCases : ParameterTestCasesBase
    {
        /// <summary>
        /// Returns the <paramref name="descriptors"/> that are exclusively
        /// <see cref="TestCaseDescriptor{T}"/> and not
        /// <see cref="RepeatedTestCaseDescriptor{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="descriptors"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        private static IEnumerable<object[]> GetDescriptorTestCases<T>(IEnumerable<TestCaseDescriptor> descriptors, int? precision = null)
            where T : IComparable
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var descriptor in descriptors.OfType<TestCaseDescriptor<T>>())
            {
                TestCaseDescriptor d = descriptor;
                yield return GetRange(d.Instance, d.ValueType, d.Value, d.Rendered, d.Instance.Ordinal, precision).ToArray();
            }
        }

        /// <summary>
        /// Returns the <paramref name="descriptors"/> that are exclusively
        /// <see cref="RepeatedTestCaseDescriptor{T}"/> and not
        /// <see cref="TestCaseDescriptor{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="descriptors"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        private static IEnumerable<object[]> GetRepeatedDescriptorTestCases<T>(IEnumerable<TestCaseDescriptor> descriptors, int? precision = null)
            where T : IComparable
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var descriptor in descriptors.OfType<RepeatedTestCaseDescriptor<T>>())
            {
                var d = descriptor;
                yield return GetRange(d.Instance, d.ItemType, (object)d.Value, d.Rendered, d.Instance.Ordinal, precision).ToArray();
            }
        }

        // ReSharper disable PossibleMultipleEnumeration
        /// <summary>
        /// Returns all those <paramref name="descriptors"/> that are exclusively
        /// <see cref="TestCaseDescriptor{T}"/> of the form <typeparamref name="T1"/>,
        /// <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>,
        /// and <typeparamref name="T5"/>.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="descriptors"></param>
        /// <returns></returns>
        /// <see cref="GetDescriptorTestCases{T}"/>
        private static IEnumerable<object[]> GetAllDescriptorTestCases<T1, T2, T3, T4, T5>(IEnumerable<TestCaseDescriptor> descriptors)
            where T1 : IComparable
            where T2 : IComparable
            where T3 : IComparable
            where T4 : IComparable
            where T5 : IComparable
        {
            foreach (var tc in GetDescriptorTestCases<T1>(descriptors))
            {
                yield return tc;
            }

            foreach (var tc in GetDescriptorTestCases<T2>(descriptors))
            {
                yield return tc;
            }

            foreach (var tc in GetDescriptorTestCases<T3>(descriptors))
            {
                yield return tc;
            }

            foreach (var tc in GetDescriptorTestCases<T4>(descriptors))
            {
                yield return tc;
            }

            foreach (var tc in GetDescriptorTestCases<T5>(descriptors))
            {
                yield return tc;
            }
        }
        // ReSharper restore PossibleMultipleEnumeration

        // ReSharper disable PossibleMultipleEnumeration

        /// <summary>
        /// Returns all those <paramref name="descriptors"/> that are exclusively
        /// <see cref="RepeatedTestCaseDescriptor{T}"/> of the form <typeparamref name="T1"/>,
        /// <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>,
        /// and <typeparamref name="T5"/>.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="descriptors"></param>
        /// <returns></returns>
        /// <see cref="GetRepeatedDescriptorTestCases{T}"/>
        private static IEnumerable<object[]> GetAllRepeatedDescriptorTestCases<T1, T2, T3, T4, T5>(IEnumerable<TestCaseDescriptor> descriptors)
            where T1 : IComparable
            where T2 : IComparable
            where T3 : IComparable
            where T4 : IComparable
            where T5 : IComparable
        {
            foreach (var tc in GetRepeatedDescriptorTestCases<T1>(descriptors))
            {
                yield return tc;
            }

            foreach (var tc in GetRepeatedDescriptorTestCases<T2>(descriptors))
            {
                yield return tc;
            }

            foreach (var tc in GetRepeatedDescriptorTestCases<T3>(descriptors))
            {
                yield return tc;
            }

            foreach (var tc in GetRepeatedDescriptorTestCases<T4>(descriptors))
            {
                yield return tc;
            }

            foreach (var tc in GetRepeatedDescriptorTestCases<T5>(descriptors))
            {
                yield return tc;
            }
        }
        // ReSharper restore PossibleMultipleEnumeration

        private static IEnumerable<object[]> _privateCases;

        protected override IEnumerable<object[]> Cases
        {
            get
            {
                const int defaultPrecision = 3;

                // ReSharper disable PossibleMultipleEnumeration
                IEnumerable<object[]> GetAll(IEnumerable<TestCaseDescriptor> descriptors)
                {
                    // Does not really matter the order of the generic arguments but for consistency sake throughout.
                    foreach (var tc in GetAllDescriptorTestCases<bool, int, long, Month, AnnotatedWeekday>(descriptors))
                    {
                        yield return tc;
                    }

                    // With Double generic arguments being the odd man out due to Precision requirements.
                    foreach (var tc in GetDescriptorTestCases<double>(descriptors, defaultPrecision))
                    {
                        yield return tc;
                    }

                    // Ditto Singular Descriptor Test Cases, rinse and repeat for Repeated, pardon the pun.
                    foreach (var tc in GetAllRepeatedDescriptorTestCases<bool, int, long, Month, AnnotatedWeekday>(descriptors))
                    {
                        yield return tc;
                    }

                    foreach (var tc in GetRepeatedDescriptorTestCases<double>(descriptors, defaultPrecision))
                    {
                        yield return tc;
                    }
                }
                // ReSharper restore PossibleMultipleEnumeration

                // We think this is a bit cleaner now that we depend upon a statically defined Descriptors in the base class.
                return _privateCases ?? (_privateCases = GetAll(Descriptors).ToArray());
            }
        }
    }
}
