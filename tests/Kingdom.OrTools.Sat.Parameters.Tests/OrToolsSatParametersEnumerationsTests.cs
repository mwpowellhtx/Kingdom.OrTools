using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kingdom.OrTools.Sat.Parameters
{
    using Xunit;
    using Xunit.Abstractions;
    using static String;

    public class OrToolsSatParametersEnumerationsTests : OrToolsSatParametersTestFixtureBase
    {
        public OrToolsSatParametersEnumerationsTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Returns where <paramref name="candidateType"/> <see cref="Type.IsEnum"/>.
        /// </summary>
        /// <param name="candidateType"></param>
        /// <returns></returns>
        private static bool TypeIsEnum(Type candidateType) => candidateType?.IsEnum == true;

        /// <summary>
        /// Gets the Enumeration Types.
        /// </summary>
        /// <see cref="TypeIsEnum"/>
        private static IEnumerable<Type> EnumerationTypes => ParametersAssembly.GetTypes().Where(TypeIsEnum).AssertNotNull().AssertNotEmpty();

        /// <summary>
        /// Do some verification of the internal bits, without which the
        /// <see cref="TheoryAttribute"/> unit tests will fail to yield test cases in most
        /// instances.
        /// </summary>
        [Fact]
        public void Should_Have_EnumerationTypes() => EnumerationTypes.AssertNotNull();

        private static IEnumerable<object[]> _enumTypeTestCases;

        public static IEnumerable<object[]> EnumTypeTestCases
        {
            get
            {
                IEnumerable<object[]> GetAll()
                {
                    foreach (var enumType in EnumerationTypes)
                    {
                        yield return GetRange<object>(enumType).ToArray();
                    }
                }

                return _enumTypeTestCases ?? (_enumTypeTestCases = GetAll().ToArray());
            }
        }

        /// <summary>
        /// The Code Generated <paramref name="enumType"/> are all Greater Than or Equal
        /// To Zero, there is Exactly One Zero Value, and are All Distinct.
        /// </summary>
        /// <param name="enumType"></param>
        [Theory, MemberData(nameof(EnumTypeTestCases))]
        public void Enum_Has_Values(Type enumType)
        {
            const long zed = 0L;
            var longValues = enumType.GetEnumValues<long>().ToArray();
            longValues.AssertTrue(x => x.All(y => y >= zed));
            longValues.AssertTrue(x => x.Count(y => y == zed) == 1);
            longValues.AssertTrue(x => x.Distinct().Count() == x.Length);
        }

        private static IEnumerable<object[]> _codeGeneratedEnumTestCases;

        public static IEnumerable<object[]> CodeGeneratedEnumTestCases
        {
            get
            {
                IEnumerable<object[]> GetAll()
                {
                    yield return GetRange<object>(EnumerationTypes.ToArray(), 10).ToArray();
                }

                return _codeGeneratedEnumTestCases ?? (_codeGeneratedEnumTestCases = GetAll().ToArray());
            }
        }

        /// <summary>
        /// Verifies whether <paramref name="enumTypes"/> Has the
        /// <paramref name="expectedLength"/> of <see cref="Type"/> instances.
        /// </summary>
        /// <param name="enumTypes"></param>
        /// <param name="expectedLength"></param>
        [Theory, MemberData(nameof(CodeGeneratedEnumTestCases))]
        public void Has_Expected_Number_of_Enums(Type[] enumTypes, int expectedLength) => enumTypes
            .AssertNotNull().AssertNotEmpty()
            .AssertEqual(expectedLength.AssertTrue(x => x > 0), x => x.Length);

        private static IEnumerable<object[]> _memberNameAttributeTestCases;

        public static IEnumerable<object[]> MemberNameAttributeTestCases
        {
            get
            {
                IEnumerable<object[]> GetOne(Type enumType)
                {
                    foreach (var x in enumType.GetEnumValues())
                    {
                        yield return GetRange(enumType, x).ToArray();
                    }
                }

                IEnumerable<object[]> GetAll()
                {
                    foreach (var x in EnumerationTypes.SelectMany(GetOne))
                    {
                        yield return x;
                    }
                }

                return _memberNameAttributeTestCases ?? (_memberNameAttributeTestCases = GetAll().ToArray());
            }
        }

        private static ParameterMemberNameAttribute GetMemberAnnotation(Type enumType, object value)
        {
            // Rule out a couple of obvious potential issues with the parameters first.
            enumType = enumType.AssertNotNull().AssertTrue(x => x.IsEnum);
            value = value.AssertNotNull();

            // Seems strange that Type.GetMember, singular, should return an ARRAY! But, whatever...
            var members = enumType.GetMember($"{value}").AssertNotNull().AssertTrue(x => x.Length == 1);
            var memberInfo = members.SingleOrDefault().AssertNotNull();

            return memberInfo.GetCustomAttribute<ParameterMemberNameAttribute>().AssertNotNull();
        }

        /// <summary>
        /// Verifies that each <paramref name="enumType"/> Member <paramref name="value"/>
        /// is properly decorated with the <see cref="ParameterMemberNameAttribute"/> attribute.
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <see cref="ParameterMemberNameAttribute"/>
        [Theory, MemberData(nameof(MemberNameAttributeTestCases))]
        public void Member_Name_Decoration_Exists(Type enumType, object value)
            => GetMemberAnnotation(enumType, value).MemberName.AssertNotNull().AssertNotEmpty();

        private static IEnumerable<object[]> _memberNamesDistinctTestCases;

        public static IEnumerable<object[]> MemberNamesDistinctTestCases
        {
            get
            {
                IEnumerable<object[]> GetAll()
                {
                    foreach (var x in EnumerationTypes)
                    {
                        yield return GetRange<object>(x).ToArray();
                    }
                }

                return _memberNamesDistinctTestCases ?? (_memberNamesDistinctTestCases = GetAll().ToArray());
            }
        }

        [Theory, MemberData(nameof(MemberNamesDistinctTestCases))]
        public void Member_Name_Decorations_Are_Distinct(Type enumType)
        {
            enumType = enumType.AssertNotNull().AssertTrue(x => x.IsEnum);

            Tuple<object, string> GetEnumMemberAnnotatedTuple(object value) => Tuple.Create(
                value.AssertNotNull()
                , GetMemberAnnotation(enumType, value).MemberName
            );

            IEnumerable<Tuple<object, string>> GetMemberAnnotatedTuples()
                => enumType.GetEnumValues().AssertNotNull().AssertNotEmpty()
                    .OfType<object>().Select(GetEnumMemberAnnotatedTuple)
                    .AssertTrue(x => x.All(y => y.Item2.AssertNotNull().Any()));

            var tuples = GetMemberAnnotatedTuples().ToArray();
            var names = tuples.Select(x => x.Item2).ToArray();

            // If we are here, the Collection should, by definition, be neither Null nor Empty.
            names.Distinct().AssertEqual(names);

            string ReportTuple(Tuple<object, string> tuple) => $"{tuple.Item1} = {tuple.Item2} ({(long) tuple.Item1})";

            OutputHelper.WriteLine(
                $"Enumerated type `{enumType.FullName}´"
                + $" member decorations: {Join(", ", tuples.Select(ReportTuple))}"
            );
        }
    }
}
