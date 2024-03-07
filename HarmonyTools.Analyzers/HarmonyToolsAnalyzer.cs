using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
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

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [
        MethodMustExistRule,
        MethodMustNotBeAmbiguousRule,
        TypeMustExistRule,
        MethodMustBeSpecifiedRule,
        MethodMustNotBeOverspecifiedRule,
        AttributeArgumentsMustBeValidRule,
        ArgumentTypesAndVariationsMustMatchRule,
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

    private static void AnalyzeTypeV1(SymbolAnalysisContext context)
    {
        var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        var patchDescription = HarmonyPatchDescriptionV1.Parse(namedTypeSymbol, context.Compilation);
        if (patchDescription == null)
            return;

        CommonChecks(context, patchDescription);
    }

    private static void AnalyzeTypeV2(SymbolAnalysisContext context)
    {
        var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        var patchDescription = HarmonyPatchDescriptionV2.Parse(namedTypeSymbol, context.Compilation);
        if (patchDescription == null)
            return;

        CommonChecks(context, patchDescription);

        CheckMethodMustExistAndNotAmbiguousV2(context, patchDescription);
        CheckTypeMustExistV2(context, patchDescription);
        CheckAttributeArgumentsMustBeValidV2(context, patchDescription);
    }

    private static void CommonChecks(SymbolAnalysisContext context, HarmonyPatchDescription patchDescription)
    {
        CheckMethodMustExistAndNotAmbiguous(context, patchDescription);
        CheckMethodMustBeSpecified(context, patchDescription);
        CheckMethodMustNotBeOverspecified(context, patchDescription);
        CheckAttributeArgumentsMustBeValid(context, patchDescription);
        CheckArgumentTypesAndVariationsMustMatch(context, patchDescription);
    }

    private static void CheckMethodMustExistAndNotAmbiguous(SymbolAnalysisContext context, HarmonyPatchDescription patchDescription)
    {
        if (patchDescription.TargetTypes is not [{ Value: not null }])
            return;

        var targetType = patchDescription.TargetTypes[0].Value!;
        CheckMethodMustExistAndNotAmbiguous(context, patchDescription, targetType);
    }

    private static void CheckMethodMustExistAndNotAmbiguousV2(SymbolAnalysisContext context, HarmonyPatchDescriptionV2 patchDescription)
    {
        if (patchDescription.TargetTypeNames is not [{ Value: not null }])
            return;

        var targetTypeName = patchDescription.TargetTypeNames[0].Value!;
        var targetType = context.Compilation.GetTypeByMetadataName(targetTypeName);
        if (targetType is not null)
            CheckMethodMustExistAndNotAmbiguous(context, patchDescription, targetType);
    }

    private static void CheckMethodMustExistAndNotAmbiguous(SymbolAnalysisContext context, HarmonyPatchDescription patchDescription, INamedTypeSymbol targetType)
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
                        targetMembers = targetMembers.OfType<IMethodSymbol>();
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

            targetMembers = targetMembers.Where(member => IsMatch(member, argumentTypes, argumentVariations, context.Compilation));
        }

        var count = targetMembers.Count();
        if (count == 0)
            context.ReportDiagnostic(Diagnostic.Create(MethodMustExistRule,
                patchDescription.GetLocation(), patchDescription.GetAdditionalLocations(),
                memberName, targetType.ToDisplayString()));
        else if (count > 1)
            context.ReportDiagnostic(Diagnostic.Create(MethodMustNotBeAmbiguousRule,
                patchDescription.GetLocation(), patchDescription.GetAdditionalLocations(),
                memberName, targetType.ToDisplayString(), count));
    }

    private static void CheckTypeMustExistV2(SymbolAnalysisContext context, HarmonyPatchDescriptionV2 patchDescription)
    {
        if (patchDescription.TargetTypeNames is not [{ Value: not null }])
            return;

        var targetTypeName = patchDescription.TargetTypeNames[0].Value!;
        var targetType = context.Compilation.GetTypeByMetadataName(targetTypeName);
        if (targetType is null)
            context.ReportDiagnostic(Diagnostic.Create(TypeMustExistRule,
                patchDescription.GetLocation(), patchDescription.GetAdditionalLocations(),
                targetTypeName));
    }

    private static void CheckMethodMustBeSpecified(SymbolAnalysisContext context, HarmonyPatchDescription patchDescription)
    {
        if (HasConflictingSpecifications(patchDescription))
            return;

        var methodType = patchDescription.MethodTypes.Length == 0 ? MethodType.Normal : patchDescription.MethodTypes[0].Value;
        var isMemberTypeSpecified = patchDescription.TargetTypes is [_] ||
                                    patchDescription is HarmonyPatchDescriptionV2 { TargetTypeNames: [_] };
        var isMemberNameSpecified = patchDescription.MethodNames is [_];
        if (!isMemberTypeSpecified || methodType is MethodType.Normal && !isMemberNameSpecified)
            context.ReportDiagnostic(Diagnostic.Create(MethodMustBeSpecifiedRule,
                patchDescription.GetLocation(), patchDescription.GetAdditionalLocations()));
    }

    private static void CheckMethodMustNotBeOverspecified(SymbolAnalysisContext context, HarmonyPatchDescription patchDescription)
    {
        if (patchDescription is HarmonyPatchDescriptionV2 patchDescriptionV2)
        {
            var typeDetails = patchDescriptionV2.TargetTypes.Concat<IHasSyntax>(patchDescriptionV2.TargetTypeNames).ToArray();
            if (typeDetails.Length > 1)
                context.ReportDiagnostic(Diagnostic.Create(MethodMustNotBeOverspecifiedRule,
                    typeDetails.GetLocation(), typeDetails.GetAdditionalLocations()));
        }
        else if (patchDescription.TargetTypes.Length > 1)
            context.ReportDiagnostic(Diagnostic.Create(MethodMustNotBeOverspecifiedRule,
                patchDescription.TargetTypes.GetLocation(), patchDescription.TargetTypes.GetAdditionalLocations()));        

        if (patchDescription.MethodNames.Length > 1)
            context.ReportDiagnostic(Diagnostic.Create(MethodMustNotBeOverspecifiedRule,
                patchDescription.MethodNames.GetLocation(), patchDescription.MethodNames.GetAdditionalLocations()));

        if (patchDescription.MethodTypes.Length > 1)
            context.ReportDiagnostic(Diagnostic.Create(MethodMustNotBeOverspecifiedRule,
                patchDescription.MethodTypes.GetLocation(), patchDescription.MethodTypes.GetAdditionalLocations()));

        if (patchDescription.ArgumentTypes.Length > 1)
            context.ReportDiagnostic(Diagnostic.Create(MethodMustNotBeOverspecifiedRule,
                patchDescription.ArgumentTypes.GetLocation(), patchDescription.ArgumentTypes.GetAdditionalLocations()));

        if (patchDescription.ArgumentVariations.Length > 1)
            context.ReportDiagnostic(Diagnostic.Create(MethodMustNotBeOverspecifiedRule,
                patchDescription.ArgumentVariations.GetLocation(), patchDescription.ArgumentVariations.GetAdditionalLocations()));
    }

    private static void CheckAttributeArgumentsMustBeValid(SymbolAnalysisContext context, HarmonyPatchDescription patchDescription)
    {
        foreach (var detail in patchDescription.TargetTypes.Where(type => type.Value is null))
            ReportInvalidAttributeArgument(context, detail);

        foreach (var detail in patchDescription.MethodNames.Where(type => type.Value is null))
            ReportInvalidAttributeArgument(context, detail);

        foreach (var detail in patchDescription.MethodTypes.Where(type => !IsValidEnumValue(type.Value)))
            ReportInvalidAttributeArgument(context, detail);

        if (patchDescription.HarmonyVersion == 1)
            foreach (var detail in patchDescription.MethodTypes.Where(type => type.Value is >= MethodType.Enumerator and <= MethodType.Async))
                ReportInvalidAttributeArgument(context, detail);

        foreach (var detail in patchDescription.ArgumentTypes.Where(type => type.Value.IsDefault))
            ReportInvalidAttributeArgument(context, detail);

        foreach (var detail in patchDescription.ArgumentTypes.Where(type => !type.Value.IsDefault))
            for (var i = 0; i < detail.Value.Length; i++)
                if (detail.Value[i] is null)
                    ReportInvalidAttributeArgument(context, detail, i);

        foreach (var detail in patchDescription.ArgumentVariations.Where(type => type.Value.IsDefault))
            ReportInvalidAttributeArgument(context, detail);

        foreach (var detail in patchDescription.ArgumentVariations.Where(type => !type.Value.IsDefault))
            for (var i = 0; i < detail.Value.Length; i++)
                if (!IsValidEnumValue(detail.Value[i]))
                    ReportInvalidAttributeArgument(context, detail, i);
    }

    private static void CheckArgumentTypesAndVariationsMustMatch(SymbolAnalysisContext context, HarmonyPatchDescription patchDescription)
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
                context.ReportDiagnostic(Diagnostic.Create(ArgumentTypesAndVariationsMustMatchRule, 
                    type.Syntax!.GetLocation(), additionalLocations: [variation.Syntax!.GetLocation()]));
        

        static AttributeSyntax? GetAttributeSyntax(IHasSyntax hasSyntax)
        {
            var attributeArgumentSyntax = hasSyntax.Syntax as AttributeArgumentSyntax;
            var attributeArgumentListSyntax = attributeArgumentSyntax?.Parent as AttributeArgumentListSyntax;
            return attributeArgumentListSyntax?.Parent as AttributeSyntax;
        }
    }

    private static void CheckAttributeArgumentsMustBeValidV2(SymbolAnalysisContext context, HarmonyPatchDescriptionV2 patchDescription)
    {
        foreach (var detail in patchDescription.TargetTypeNames.Where(type => type.Value is null))
            ReportInvalidAttributeArgument(context, detail);
    }

    private static void ReportInvalidAttributeArgument(SymbolAnalysisContext context, IHasSyntax detail) =>
        context.ReportDiagnostic(Diagnostic.Create(AttributeArgumentsMustBeValidRule, detail.Syntax?.GetLocation()));

    private static void ReportInvalidAttributeArgument<T>(SymbolAnalysisContext context, DetailWithSyntax<ImmutableArray<T>> detail, int arrayIndex)
    {
        var attributeArgumentSyntax = detail.Syntax as AttributeArgumentSyntax;
        var arrayCreationExpressionSyntax = attributeArgumentSyntax?.Expression as ArrayCreationExpressionSyntax;
        var itemExpressionSyntax = arrayCreationExpressionSyntax?.Initializer?.Expressions.ElementAtOrDefault(arrayIndex);
        context.ReportDiagnostic(Diagnostic.Create(AttributeArgumentsMustBeValidRule, (itemExpressionSyntax ?? detail.Syntax)?.GetLocation()));
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