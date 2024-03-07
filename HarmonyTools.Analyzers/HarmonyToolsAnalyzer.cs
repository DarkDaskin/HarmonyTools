using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using HarmonyTools.Analyzers.HarmonyEnums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace HarmonyTools.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class HarmonyToolsAnalyzer : DiagnosticAnalyzer
{
    private const string TargetCategory = "Target";

#pragma warning disable RS2008
    private static readonly DiagnosticDescriptor MethodMustExistRule = 
        CreateRule(DiagnosticIds.MethodMustExist, nameof(Resources.MethodMustExistTitle), nameof(Resources.MethodMustExistMessageFormat), TargetCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor MethodMustNotBeAmbiguousRule = 
        CreateRule(DiagnosticIds.MethodMustNotBeAmbiguous, nameof(Resources.MethodMustNotBeAmbiguousTitle), nameof(Resources.MethodMustNotBeAmbiguousMessageFormat), TargetCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor TypeMustExistRule = 
        CreateRule(DiagnosticIds.TypeMustExist, nameof(Resources.TypeMustExistTitle), nameof(Resources.TypeMustExistMessageFormat), TargetCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor MethodMustBeSpecifiedRule = 
        CreateRule(DiagnosticIds.MethodMustBeSpecified, nameof(Resources.MethodMustBeSpecifiedTitle), nameof(Resources.MethodMustBeSpecifiedMessageFormat), TargetCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor MethodMustNotBeOverspecifiedRule = 
        CreateRule(DiagnosticIds.MethodMustNotBeOverspecified, nameof(Resources.MethodMustNotBeOverspecifiedTitle), nameof(Resources.MethodMustNotBeOverspecifiedMessageFormat), TargetCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor AttributeArgumentsMustBeValidRule = 
        CreateRule(DiagnosticIds.AttributeArgumentsMustBeValid, nameof(Resources.AttributeArgumentsMustBeValidTitle), nameof(Resources.AttributeArgumentsMustBeValidMessageFormat), TargetCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor ArgumentTypesAndVariationsMustMatchRule = 
        CreateRule(DiagnosticIds.ArgumentTypesAndVariationsMustMatch, nameof(Resources.ArgumentTypesAndVariationsMustMatchTitle), nameof(Resources.ArgumentTypesAndVariationsMustMatchMessageFormat), TargetCategory, DiagnosticSeverity.Warning);
    private static readonly DiagnosticDescriptor HarmonyPatchAttributeMustBeOnTypeRule = 
        CreateRule(DiagnosticIds.HarmonyPatchAttributeMustBeOnType, nameof(Resources.HarmonyPatchAttributeMustBeOnTypeTitle), nameof(Resources.HarmonyPatchAttributeMustBeOnTypeMessageFormat), TargetCategory, DiagnosticSeverity.Warning);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [
        MethodMustExistRule,
        MethodMustNotBeAmbiguousRule,
        TypeMustExistRule,
        MethodMustBeSpecifiedRule,
        MethodMustNotBeOverspecifiedRule,
        AttributeArgumentsMustBeValidRule,
        ArgumentTypesAndVariationsMustMatchRule,
        HarmonyPatchAttributeMustBeOnTypeRule,
    ];

    private static DiagnosticDescriptor CreateRule(string id, string titleResource, string messageFormatResource,
        string category, DiagnosticSeverity severity, bool isEnabledByDefault = true, string? descriptionResource = null) =>
        new (id,
            new LocalizableResourceString(titleResource, Resources.ResourceManager, typeof(Resources)),
            new LocalizableResourceString(messageFormatResource, Resources.ResourceManager, typeof(Resources)),
            category, severity, isEnabledByDefault,
            descriptionResource == null ? null : new LocalizableResourceString(descriptionResource, Resources.ResourceManager, typeof(Resources)));

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(RegisterActionsWithFullMetadata);
        context.RegisterCompilationAction(RunAnalyzerWithFullMetadata);
    }

    private static void RegisterActionsWithFullMetadata(CompilationStartAnalysisContext context)
    {
        if (context.Compilation.Options.MetadataImportOptions == MetadataImportOptions.All)
        {
            context.RegisterSymbolAction(AnalyzeTypeV1, SymbolKind.NamedType);
            context.RegisterSymbolAction(AnalyzeTypeV2, SymbolKind.NamedType);
        }
    }

    private void RunAnalyzerWithFullMetadata(CompilationAnalysisContext context)
    {
        if (context.Compilation.Options.MetadataImportOptions == MetadataImportOptions.All)
            return;

        var compilation = context.Compilation.WithOptions(context.Compilation.Options.WithMetadataImportOptions(MetadataImportOptions.All));
        var compilationWithAnalyzer = new CompilationWithAnalyzers(compilation, [this], context.Options);
        var disgnostics = compilationWithAnalyzer.GetAnalyzerDiagnosticsAsync(context.CancellationToken).Result;
        foreach (var disgnostic in disgnostics)
            context.ReportDiagnostic(disgnostic);
    }

    private static void AnalyzeTypeV1(SymbolAnalysisContext context) => new VerifierV1(context).Verify();

    private static void AnalyzeTypeV2(SymbolAnalysisContext context) => new VerifierV2(context).Verify();


    private abstract class Verifier<TPatchDescription>(SymbolAnalysisContext context)
        where TPatchDescription : HarmonyPatchDescription
    {
        protected readonly SymbolAnalysisContext Context = context;

        public abstract void Verify();

        protected void VerifyCore(HarmonyPatchDescriptionSet<TPatchDescription> set)
        {
            if (set.TypePatchDescription is not null)
                PreMergeChecks(set.TypePatchDescription);

            var hasPatchMethodDescription = false;
            foreach (var patchMethod in set.PatchMethods)
            {
                if (patchMethod.PatchDescription is null) 
                    continue;

                hasPatchMethodDescription = true;

                PreMergeChecks(patchMethod.PatchDescription);

                if (set.TypePatchDescription is not null)
                    patchMethod.PatchDescription.Merge(set.TypePatchDescription);

                PostMergeChecks(patchMethod.PatchDescription);
            }

            if (!set.PatchMethods.IsEmpty && !hasPatchMethodDescription && set.TypePatchDescription is not null)
                PostMergeChecks(set.TypePatchDescription);

            CheckHarmonyPatchAttributeMustBeOnType(set);
        }

        protected virtual void PreMergeChecks(TPatchDescription patchDescription)
        {
            CheckAttributeArgumentsMustBeValid(patchDescription);
            CheckArgumentTypesAndVariationsMustMatch(patchDescription);
        }

        protected virtual void PostMergeChecks(TPatchDescription patchDescription)
        {
            CheckMethodMustExistAndNotAmbiguous(patchDescription);
            CheckMethodMustBeSpecified(patchDescription);
            CheckMethodMustNotBeOverspecified(patchDescription);
        }

        private void CheckAttributeArgumentsMustBeValid(HarmonyPatchDescription patchDescription)
        {
            foreach (var detail in patchDescription.TargetTypes.Where(type => type.Value is null))
                ReportInvalidAttributeArgument(detail);

            foreach (var detail in patchDescription.MethodNames.Where(type => type.Value is null))
                ReportInvalidAttributeArgument(detail);

            foreach (var detail in patchDescription.MethodTypes.Where(type => !IsValidEnumValue(type.Value)))
                ReportInvalidAttributeArgument(detail);

            if (patchDescription.HarmonyVersion == 1)
                foreach (var detail in patchDescription.MethodTypes.Where(type => type.Value is >= MethodType.Enumerator and <= MethodType.Async))
                    ReportInvalidAttributeArgument(detail);

            foreach (var detail in patchDescription.ArgumentTypes.Where(type => type.Value.IsDefault))
                ReportInvalidAttributeArgument(detail);

            foreach (var detail in patchDescription.ArgumentTypes.Where(type => !type.Value.IsDefault))
                for (var i = 0; i < detail.Value.Length; i++)
                    if (detail.Value[i] is null)
                        ReportInvalidAttributeArgument(detail, i);

            foreach (var detail in patchDescription.ArgumentVariations.Where(type => type.Value.IsDefault))
                ReportInvalidAttributeArgument(detail);

            foreach (var detail in patchDescription.ArgumentVariations.Where(type => !type.Value.IsDefault))
                for (var i = 0; i < detail.Value.Length; i++)
                    if (!IsValidEnumValue(detail.Value[i]))
                        ReportInvalidAttributeArgument(detail, i);
        }

        private void CheckArgumentTypesAndVariationsMustMatch(HarmonyPatchDescription patchDescription)
        {
            // Match argument variations with argument types from the same attribute.
            var argumentTypesAndVariationsByAttribute =
                from variation in patchDescription.ArgumentVariations
                let attributeSyntax = GetAttributeSyntax(variation)
                where attributeSyntax != null
                let type = patchDescription.ArgumentTypes.Single(type => GetAttributeSyntax(type) == attributeSyntax)
                where !type.Value.IsDefault && !variation.Value.IsDefault
                select (type, variation);
            foreach (var (type, variation) in argumentTypesAndVariationsByAttribute)
                if (type.Value.Length != variation.Value.Length)
                    Context.ReportDiagnostic(Diagnostic.Create(ArgumentTypesAndVariationsMustMatchRule,
                        type.Syntax!.GetLocation(), additionalLocations: [variation.Syntax!.GetLocation()]));


            static AttributeSyntax? GetAttributeSyntax(IHasSyntax hasSyntax)
            {
                var attributeArgumentSyntax = hasSyntax.Syntax as AttributeArgumentSyntax;
                var attributeArgumentListSyntax = attributeArgumentSyntax?.Parent as AttributeArgumentListSyntax;
                return attributeArgumentListSyntax?.Parent as AttributeSyntax;
            }
        }

        private void CheckMethodMustExistAndNotAmbiguous(HarmonyPatchDescription patchDescription)
        {
            if (patchDescription.TargetTypes is not [{ Value: not null }])
                return;

            var targetType = patchDescription.TargetTypes[0].Value!;
            CheckMethodMustExistAndNotAmbiguous(patchDescription, targetType);
        }

        protected void CheckMethodMustExistAndNotAmbiguous(HarmonyPatchDescription patchDescription, INamedTypeSymbol targetType)
        {
            if (HasConflictingSpecifications(patchDescription))
                return;

            var methodType = patchDescription.MethodTypes.Length == 0 ? MethodType.Normal : patchDescription.MethodTypes[0].Value;
            var isMemberNameSpecified = patchDescription.MethodNames is [{ Value: not null }];

            IEnumerable<ISymbol> targetMembers;
            string memberName;
            switch (methodType)
            {
                case MethodType.Constructor when !isMemberNameSpecified:
                    targetMembers = targetType.InstanceConstructors;
                    memberName = ".ctor";
                    break;
                case MethodType.StaticConstructor when !isMemberNameSpecified:
                    targetMembers = targetType.StaticConstructors;
                    memberName = ".cctor";
                    break;
                // TODO: check that Enumerator and Async methods are actually as such
                case MethodType.Normal or MethodType.Getter or MethodType.Setter or MethodType.Enumerator or MethodType.Async
                    when isMemberNameSpecified:
                    memberName = patchDescription.MethodNames[0].Value!;
                    targetMembers = targetType.GetMembers(memberName);
                    switch (methodType)
                    {
                        case MethodType.Getter:
                            targetMembers = targetMembers.OfType<IPropertySymbol>().Where(property => property.GetMethod is not null);
                            memberName = $"get_{memberName}";
                            break;
                        case MethodType.Setter:
                            targetMembers = targetMembers.OfType<IPropertySymbol>().Where(property => property.SetMethod is not null);
                            memberName = $"set_{memberName}";
                            break;
                        default:
                            targetMembers = targetMembers.OfType<IMethodSymbol>().Where(method => method.MethodKind == MethodKind.Ordinary);
                            break;
                    }
                    break;
                case MethodType.Getter or MethodType.Setter when !isMemberNameSpecified:
                    memberName = "this[]";
                    targetMembers = targetType.GetMembers(memberName);
                    switch (methodType)
                    {
                        case MethodType.Getter:
                            targetMembers = targetMembers.OfType<IPropertySymbol>().Where(property => property.GetMethod is not null);
                            memberName = "get_Item";
                            break;
                        case MethodType.Setter:
                            targetMembers = targetMembers.OfType<IPropertySymbol>().Where(property => property.SetMethod is not null);
                            memberName = "set_Item";
                            break;
                    }
                    break;
                default:
                    return;
            }

            if (patchDescription.ArgumentTypes.Length == 1)
            {
                var argumentTypes = patchDescription.ArgumentTypes[0].Value;
                var argumentVariations = patchDescription.ArgumentVariations.Length == 0 ?
                    [..argumentTypes.Select(_ => ArgumentType.Normal)] :
                    patchDescription.ArgumentVariations[0].Value;

                if (argumentTypes.Length != argumentVariations.Length)
                    return;

                if (argumentTypes.Any(type => type is null) || argumentVariations.Any(variation => !IsValidEnumValue(variation)))
                    return;

                if (methodType == MethodType.StaticConstructor && argumentTypes.Length > 0)
                    return;

                if (methodType is MethodType.Getter or MethodType.Setter && isMemberNameSpecified && argumentTypes.Length > 0)
                    return;

                targetMembers = targetMembers.Where(member => IsMatch(member, argumentTypes, argumentVariations, Context.Compilation));
            }

            var count = targetMembers.Count();
            if (count == 0)
                Context.ReportDiagnostic(Diagnostic.Create(MethodMustExistRule,
                    patchDescription.GetLocation(), patchDescription.GetAdditionalLocations(),
                    memberName, targetType.ToDisplayString()));
            else if (count > 1)
                Context.ReportDiagnostic(Diagnostic.Create(MethodMustNotBeAmbiguousRule,
                    patchDescription.GetLocation(), patchDescription.GetAdditionalLocations(),
                    memberName, targetType.ToDisplayString(), count));
        }

        private void CheckMethodMustBeSpecified(HarmonyPatchDescription patchDescription)
        {
            if (HasConflictingSpecifications(patchDescription))
                return;

            var methodType = patchDescription.MethodTypes.Length == 0 ? MethodType.Normal : patchDescription.MethodTypes[0].Value;
            var isMemberTypeSpecified = patchDescription.TargetTypes is [_] ||
                                        patchDescription is HarmonyPatchDescriptionV2 { TargetTypeNames: [_] };
            var isMemberNameSpecified = patchDescription.MethodNames is [_];
            if (!isMemberTypeSpecified || methodType is MethodType.Normal && !isMemberNameSpecified)
                Context.ReportDiagnostic(Diagnostic.Create(MethodMustBeSpecifiedRule,
                    patchDescription.GetLocation(), patchDescription.GetAdditionalLocations()));
        }

        private void CheckMethodMustNotBeOverspecified(HarmonyPatchDescription patchDescription)
        {
            if (patchDescription is HarmonyPatchDescriptionV2 patchDescriptionV2)
            {
                var typeDetails = patchDescriptionV2.TargetTypes.Concat<IHasSyntax>(patchDescriptionV2.TargetTypeNames).ToArray();
                if (typeDetails.Length > 1)
                    Context.ReportDiagnostic(Diagnostic.Create(MethodMustNotBeOverspecifiedRule,
                        typeDetails.GetLocation(), typeDetails.GetAdditionalLocations()));
            }
            else if (patchDescription.TargetTypes.Length > 1)
                Context.ReportDiagnostic(Diagnostic.Create(MethodMustNotBeOverspecifiedRule,
                    patchDescription.TargetTypes.GetLocation(), patchDescription.TargetTypes.GetAdditionalLocations()));

            if (patchDescription.MethodNames.Length > 1)
                Context.ReportDiagnostic(Diagnostic.Create(MethodMustNotBeOverspecifiedRule,
                    patchDescription.MethodNames.GetLocation(), patchDescription.MethodNames.GetAdditionalLocations()));

            if (patchDescription.MethodTypes.Length > 1)
                Context.ReportDiagnostic(Diagnostic.Create(MethodMustNotBeOverspecifiedRule,
                    patchDescription.MethodTypes.GetLocation(), patchDescription.MethodTypes.GetAdditionalLocations()));

            if (patchDescription.ArgumentTypes.Length > 1)
                Context.ReportDiagnostic(Diagnostic.Create(MethodMustNotBeOverspecifiedRule,
                    patchDescription.ArgumentTypes.GetLocation(), patchDescription.ArgumentTypes.GetAdditionalLocations()));

            if (patchDescription.ArgumentVariations.Length > 1)
                Context.ReportDiagnostic(Diagnostic.Create(MethodMustNotBeOverspecifiedRule,
                    patchDescription.ArgumentVariations.GetLocation(), patchDescription.ArgumentVariations.GetAdditionalLocations()));
        }

        private void CheckHarmonyPatchAttributeMustBeOnType(HarmonyPatchDescriptionSet<TPatchDescription> set)
        {
            if (set.TypePatchDescription is not null)
                return;

            // Find any patch method having any Harmony attribute. Ignore convention-based methods, because in type not marked with HarmonyPatch
            // method names do not have special meaning.
            var patchMethodWithAttributeSyntax = (
                from patchMethod in set.PatchMethods
                let methodKindAttributeSyntax = patchMethod.MethodKinds.FirstOrDefault(detail => detail.Syntax is AttributeSyntax)?.Syntax
                let patchDescriptionAttributeSyntax =
                    patchMethod.PatchDescription?.AttrubuteSyntaxes.OfType<AttributeSyntax>().FirstOrDefault()
                let syntax = patchDescriptionAttributeSyntax ?? methodKindAttributeSyntax
                where syntax is not null
                select (patchMethod, syntax)).FirstOrDefault();
            if (patchMethodWithAttributeSyntax.patchMethod is not null)
                Context.ReportDiagnostic(Diagnostic.Create(HarmonyPatchAttributeMustBeOnTypeRule,
                    patchMethodWithAttributeSyntax.patchMethod.Method.ContainingType
                        .GetSyntax(patchMethodWithAttributeSyntax.syntax)?.GetIdentifierLocation()));
        }

        protected void ReportInvalidAttributeArgument(IHasSyntax detail) =>
            Context.ReportDiagnostic(Diagnostic.Create(AttributeArgumentsMustBeValidRule, detail.Syntax?.GetLocation()));

        private void ReportInvalidAttributeArgument<T>(DetailWithSyntax<ImmutableArray<T>> detail, int arrayIndex)
        {
            var attributeArgumentSyntax = detail.Syntax as AttributeArgumentSyntax;
            var arrayCreationExpressionSyntax = attributeArgumentSyntax?.Expression as ArrayCreationExpressionSyntax;
            var itemExpressionSyntax = arrayCreationExpressionSyntax?.Initializer?.Expressions.ElementAtOrDefault(arrayIndex);
            Context.ReportDiagnostic(Diagnostic.Create(AttributeArgumentsMustBeValidRule, (itemExpressionSyntax ?? detail.Syntax)?.GetLocation()));
        }

        private static bool HasConflictingSpecifications(HarmonyPatchDescription patchDescription)
        {
            if (patchDescription.TargetTypes.Length > 1 || patchDescription.MethodNames.Length > 1 || patchDescription.MethodTypes.Length > 1 ||
                patchDescription.ArgumentTypes.Length > 1 || patchDescription.ArgumentVariations.Length > 1)
                return true;

            if (patchDescription is HarmonyPatchDescriptionV2 { TargetTypeNames.Length: > 1 })
                return true;

            if (patchDescription is HarmonyPatchDescriptionV2 { TargetTypes: [_], TargetTypeNames: [_] })
                return true;

            return false;
        }

        private static bool IsMatch(ISymbol member, ImmutableArray<ITypeSymbol?> types, ImmutableArray<ArgumentType> variations,
            Compilation compilation) =>
            member switch
            {
                IMethodSymbol method => IsMatch(method, types, variations, compilation),
                IPropertySymbol { GetMethod: not null } property => IsMatch(property.GetMethod, types, variations, compilation),
                IPropertySymbol { SetMethod: not null } property => IsMatch(property.SetMethod, types, variations, compilation, true),
                _ => false
            };

        private static bool IsMatch(IMethodSymbol method, ImmutableArray<ITypeSymbol?> types, ImmutableArray<ArgumentType> variations,
            Compilation compilation, bool isSetter = false)
        {
            var parameters = method.Parameters;
            if (isSetter)
                parameters = parameters[..^1];

            if (parameters.Length != types.Length)
                return false;

            Debug.Assert(types.Length == variations.Length);
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];

                if (type is null)
                    return false;

                if (variations[i] == ArgumentType.Pointer)
                    type = compilation.CreatePointerTypeSymbol(type);

                if (!parameters[i].Type.Equals(type, SymbolEqualityComparer.Default))
                    return false;

                if (variations[i] == ArgumentType.Ref && parameters[i].RefKind != RefKind.Ref)
                    return false;
                if (variations[i] == ArgumentType.Out && parameters[i].RefKind != RefKind.Out)
                    return false;
            }

            return true;
        }

        private static bool IsValidEnumValue<TEnum>(TEnum value) where TEnum : struct => Enum.IsDefined(typeof(TEnum), value);
    }

    private class VerifierV1(SymbolAnalysisContext context) : Verifier<HarmonyPatchDescriptionV1>(context)
    {
        public override void Verify() => VerifyCore(HarmonyPatchDescriptionV1.Parse((INamedTypeSymbol)Context.Symbol, Context.Compilation));
    }

    private class VerifierV2(SymbolAnalysisContext context) : Verifier<HarmonyPatchDescriptionV2>(context)
    {
        public override void Verify() => VerifyCore(HarmonyPatchDescriptionV2.Parse((INamedTypeSymbol)Context.Symbol, Context.Compilation));

        protected override void PreMergeChecks(HarmonyPatchDescriptionV2 patchDescription)
        {
            base.PreMergeChecks(patchDescription);

            CheckAttributeArgumentsMustBeValidV2(patchDescription);
        }

        protected override void PostMergeChecks(HarmonyPatchDescriptionV2 patchDescription)
        {
            base.PostMergeChecks(patchDescription);

            CheckMethodMustExistAndNotAmbiguousV2(patchDescription);
            CheckTypeMustExistV2(patchDescription);
        }

        private void CheckAttributeArgumentsMustBeValidV2(HarmonyPatchDescriptionV2 patchDescription)
        {
            foreach (var detail in patchDescription.TargetTypeNames.Where(type => type.Value is null))
                ReportInvalidAttributeArgument(detail);
        }

        private void CheckMethodMustExistAndNotAmbiguousV2(HarmonyPatchDescriptionV2 patchDescription)
        {
            if (patchDescription.TargetTypeNames is not [{ Value: not null }])
                return;

            var targetTypeName = patchDescription.TargetTypeNames[0].Value!;
            var targetType = Context.Compilation.GetTypeByMetadataName(targetTypeName);
            if (targetType is not null)
                CheckMethodMustExistAndNotAmbiguous(patchDescription, targetType);
        }

        private void CheckTypeMustExistV2(HarmonyPatchDescriptionV2 patchDescription)
        {
            if (patchDescription.TargetTypeNames is not [{ Value: not null }])
                return;

            var targetTypeName = patchDescription.TargetTypeNames[0].Value!;
            var targetType = Context.Compilation.GetTypeByMetadataName(targetTypeName);
            if (targetType is null)
                Context.ReportDiagnostic(Diagnostic.Create(TypeMustExistRule,
                    patchDescription.GetLocation(), patchDescription.GetAdditionalLocations(),
                    targetTypeName));
        }
    }
}