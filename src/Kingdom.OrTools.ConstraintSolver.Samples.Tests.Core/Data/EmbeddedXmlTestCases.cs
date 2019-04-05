using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Kingdom.OrTools.ConstraintSolver.Samples
{
    using Xunit;

    /// <summary>
    /// Recalls some baked in Test Cases from Xml based embedded resources.
    /// </summary>
    /// <inheritdoc />
    public sealed class EmbeddedXmlTestCases : TestCasesBase
    {
        private static Type ThisType { get; } = typeof(EmbeddedXmlTestCases);

        private static IEnumerable<object[]> _privateCases;

        private static IEnumerable<object[]> PrivateCases
        {
            get
            {
                IEnumerable<object[]> GetAll()
                {
                    var assembly = ThisType.Assembly;

                    // ReSharper disable once IdentifierTypo, StringLiteralTypo
                    const string sudokusXml = @"Sudokus.xml";

                    string GetAttributeValue(XElement element, string attributeName)
                    {
                        Assert.NotNull(element);
                        var attribute = element.Attribute(attributeName);
                        Assert.NotNull(attribute);
                        return attribute.Value;
                    }

                    using (var rs = assembly.GetManifestResourceStream(ThisType, sudokusXml))
                    {
                        Assert.NotNull(rs);

                        using (var reader = new StreamReader(rs, Encoding.UTF8))
                        {
                            var loaded = XElement.Load(reader);

                            foreach (var d in loaded.Descendants(@"Sudoku"))
                            {
                                yield return GetRange<object>(
                                    GetAttributeValue(d, "Description")
                                    , GetAttributeValue(d, "Puzzle")).ToArray();
                            }
                        }
                    }
                }

                return _privateCases ?? (_privateCases = GetAll().ToArray());
            }
        }

        protected override IEnumerable<object[]> Cases => PrivateCases;
    }
}
