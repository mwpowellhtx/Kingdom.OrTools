using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Protobuf;
    using static Characters;
    using static String;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

    internal abstract class DescriptorCodeGenerationStrategyBase
    {
        protected PackageStatement PackageStatement { get; }

        private static Lazy<IEnumerable<long>> LazyNormalFieldStatementOrdinals { get; set; }

        protected class OrdinalRange
        {
            internal long Minimum { get; }

            internal long Maximum { get; private set; }

            internal static OrdinalRange Create(long minimum, long maximum) => new OrdinalRange(minimum, maximum);

            internal static OrdinalRange From(long minimum) => new OrdinalRange(minimum, minimum);

            internal OrdinalRange To(long maximum)
            {
                Maximum = maximum;
                return this;
            }

            private OrdinalRange(long minimum, long maximum)
            {
                Minimum = minimum;
                Maximum = maximum;
            }

            internal void Deconstruct(out long minimum, out long maximum)
            {
                minimum = Minimum;
                maximum = Maximum;
            }

            public override string ToString()
                => Minimum == Maximum
                    ? $"{Minimum}"
                    : $"{Minimum}-{Maximum}";
        }

        protected IEnumerable<long> NormalFieldStatementOrdinals => LazyNormalFieldStatementOrdinals.Value;

        private IEnumerable<OrdinalRange> _normalFieldStatementOrdinalRanges;

        protected IEnumerable<OrdinalRange> NormalFieldStatementOrdinalRanges
        {
            get
            {
                IEnumerable<OrdinalRange> GetAll()
                {
                    var ordinals = NormalFieldStatementOrdinals.ToArray();

                    while (ordinals.Any())
                    {
                        var delta = 0;
                        var minimum = ordinals.First();
                        var range = ordinals.TakeWhile(x => x - minimum == delta++).ToArray();
                        yield return OrdinalRange.From(range.First()).To(range.Last());
                        ordinals = ordinals.Skip(range.Length).ToArray();
                    }
                }

                return _normalFieldStatementOrdinalRanges ?? (_normalFieldStatementOrdinalRanges = GetAll().ToArray());
            }
        }

        protected int NormalFieldStatementOrdinalCount => NormalFieldStatementOrdinals.Count();

        protected long NormalFieldStatementOrdinalsMax => NormalFieldStatementOrdinals.Max();

        protected IEnumerable<string> GetNormalFieldStatementOrdinalMetricsTriviaStrings<THasNumber>(THasNumber havingNumber)
            where THasNumber : IHasNumber
        {
            var renderedRanges = Join(", ", NormalFieldStatementOrdinalRanges.Select(x => $"{x}"));

            // TODO: TBD: could get fancier here...
            yield return $@"// Ordinal: {havingNumber.Number} (of {NormalFieldStatementOrdinalsMax})";
            yield return $@"// Ordinals: {renderedRanges} (Count: {NormalFieldStatementOrdinalCount})";
        }

        /// <summary>
        /// Gets an <see cref="IdentifierPath"/> representing
        /// &quot;Kingdom.OrTools.Sat.Parameters&quot;.
        /// </summary>
        protected static IdentifierPath SatParametersNameSpacePath { get; }
            = (Identifier) "Kingdom" / "OrTools" / "Sat" / "Parameters";

        protected static IEnumerable<T> GetRange<T>(params T[] values)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var value in values)
            {
                yield return value;
            }
        }

        protected DescriptorCodeGenerationStrategyBase(PackageStatement packageStatement)
        {
            PackageStatement = packageStatement ?? throw new ArgumentNullException($"{nameof(packageStatement)}");


            if (packageStatement.Parent is ProtoDescriptor protoDescriptor)
            {
                LazyNormalFieldStatementOrdinals = new Lazy<IEnumerable<long>>(
                    () =>
                    {
                        var satParametersMessageStatement = protoDescriptor.Items.OfType<MessageStatement>()
                            .SingleOrDefault(x => x.Name.Equals("SatParameters"));

                        return satParametersMessageStatement?.Items.OfType<NormalFieldStatement>()
                            .Select(x => x.Number).OrderBy(x => x).ToArray();
                    }
                );
            }
            else
            {
                LazyNormalFieldStatementOrdinals = new Lazy<IEnumerable<long>>(() => GetRange(0L));

                throw new InvalidOperationException($"Unable to locate the '{typeof(ProtoDescriptor).FullName}' parent.");
            }
        }
    }

    internal abstract class DescriptorCodeGenerationStrategyBase<TDescriptor, TName>
        : DescriptorCodeGenerationStrategyBase
        where TName : class
        where TDescriptor : DescriptorBase<TName>
    {
        protected virtual TDescriptor Descriptor { get; set; }

        private IdentifierPath _replacedPackagePath;

        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
        private IList<TryReplaceIdentifierPathCallback> TryReplacePackageIdentifierPathCallbacks { get; }
            = new List<TryReplaceIdentifierPathCallback> { };

        private void Add(TryReplaceIdentifierPathCallback callback
            , params TryReplaceIdentifierPathCallback[] callbacks)
            => GetRange(callback).Concat(callbacks).ForEach(
                TryReplacePackageIdentifierPathCallbacks.Add);

        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
        private IList<TryReplaceMemberIdentifierCallback> TryReplaceMemberIdentifierCallbacks { get; }
            = new List<TryReplaceMemberIdentifierCallback> { };

        private void Add(TryReplaceMemberIdentifierCallback callback
            , params TryReplaceMemberIdentifierCallback[] callbacks)
            => GetRange(callback).Concat(callbacks).ForEach(
                TryReplaceMemberIdentifierCallbacks.Add);

        protected IdentifierPath ReplacedPackagePath
        {
            get
            {
                IdentifierPath GetReplacedIdentifierPath(PackageStatement statement)
                {
                    foreach (var tryReplace in TryReplacePackageIdentifierPathCallbacks)
                    {
                        if (tryReplace(statement.PackagePath, out var replacedPath))
                        {
                            return replacedPath;
                        }
                    }

                    return new IdentifierPath(statement.PackagePath);
                }

                return _replacedPackagePath ?? (_replacedPackagePath = GetReplacedIdentifierPath(PackageStatement));
            }
        }

        protected DescriptorCodeGenerationStrategyBase(PackageStatement packageStatement, TDescriptor descriptor)
            : base(packageStatement)
            => Initialize(descriptor);

        private void Initialize(TDescriptor descriptor)
        {
            Descriptor = descriptor;

            string ToCamelCase(string s) => s.Aggregate(Empty
                , (g, x) => g + (g.Any() ? $"{x}".ToLower() : $"{x}".ToUpper())
            );

            Add((IdentifierPath a, out IdentifierPath b) => !ReferenceEquals(
                b = a.Equals((Identifier) "operations_research" / "sat")
                    ? SatParametersNameSpacePath
                    : a, a));

            Add((Identifier a, out Identifier b) => !a.Equals(b = Join(Empty
                , a.Name.Split(UnderScore).Select(ToCamelCase))));
        }

        protected virtual IEnumerable<UsingDirectiveSyntax> GetInnerUsingDirectives()
        {
            yield break;
        }

        protected MemberDeclarationSyntax GenerateNameSpace(Func<IEnumerable<MemberDeclarationSyntax>> generateMembers)
        {
            // Invoking SyntaxFactory, things are basically named $"{nameof(MethodName)}Syntax".
            var nameSpaceDecl = NamespaceDeclaration(GetQualifiedIdentifierPathName(ReplacedPackagePath));

            // ReSharper disable PossibleMultipleEnumeration
            var innerUsingDirectives = GetInnerUsingDirectives();
            if (innerUsingDirectives.Any())
            {
                nameSpaceDecl = nameSpaceDecl.AddUsings(innerUsingDirectives.ToArray());
            }
            // ReSharper restore PossibleMultipleEnumeration

            // ReSharper disable PossibleMultipleEnumeration
            var members = generateMembers();
            return members.Any() ? nameSpaceDecl.WithMembers(List(members)) : nameSpaceDecl;
            // ReSharper restore PossibleMultipleEnumeration
        }

        protected static NameSyntax GetQualifiedIdentifierPathName(IdentifierPath path)
        {
            NameSyntax result = null;

            for (var i = 0; i < path.Count; ++i)
            {
                switch (i)
                {
                    case 0:
                        // A single one is simply the Identifier Name.
                        result = IdentifierName(path[i].Name);
                        break;
                    default:
                        // Beyond that is a Qualified Identifier Name, folding in the prior path elements.
                        result = QualifiedName(result, IdentifierName(path[i].Name));
                        break;
                }
            }

            // TODO: TBD: we may assert and/or throw for nullness here...
            return result;
        }

        protected Identifier GetReplacedIdentifier(Identifier identifier)
        {
            foreach (var tryReplace in TryReplaceMemberIdentifierCallbacks)
            {
                if (tryReplace(identifier, out var replaced))
                {
                    return replaced;
                }
            }

            return identifier;
        }

        protected Identifier GetReplacedIdentifier(Func<Identifier> getIdentifier)
            => GetReplacedIdentifier(getIdentifier());

        protected IdentifierPath GetReplacedIdentifierPath(IdentifierPath path)
        {
            IdentifierPath result = null;
            foreach (var identifier in path)
            {
                var replaced = GetReplacedIdentifier(() => identifier);

                switch (result)
                {
                    case null:
                        result = new IdentifierPath(GetRange(replaced));
                        break;
                    default:
                        result /= replaced;
                        break;
                }
            }

            return result;
        }

        protected IdentifierPath GetReplacedIdentifierPath(Func<IdentifierPath> getPath)
            => GetReplacedIdentifierPath(getPath());

        /// <summary>
        /// Returns a Generated <see cref="CompilationUnitSyntax"/>.
        /// </summary>
        /// <returns></returns>
        protected abstract CompilationUnitSyntax GenerateCompilationUnit();
    }
}
