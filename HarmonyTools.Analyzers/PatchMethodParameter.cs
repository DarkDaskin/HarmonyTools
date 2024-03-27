using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace HarmonyTools.Analyzers;

internal class PatchMethodParameter(IParameterSymbol parameter, InjectionKind kind, ParameterMatchKind matchKind) : IHasSyntax
{
    private const string InstanceParameterName = "__instance";
    private const string ResultParameterName = "__result";
    private const string ResultRefParameterName = "__resultRef";
    private const string StateParameterName = "__state";
    private const string ArgsParameterName = "__args";
    private const string OriginalMethodParameterName = "__originalMethod";
    private const string RunOriginalParameterName = "__runOriginal";
    private const string ExceptionParameterName = "__exception";
    private const string ParameterByIndexPrefix = "__";
    private const string FieldPrefix = "___";
    
    public IParameterSymbol Parameter { get; } = parameter;
    public InjectionKind Kind { get; } = kind;
    public ParameterMatchKind MatchKind { get; } = matchKind;
    public HarmonyArgument? ArgumentOverride { get; private set; }

    public static ImmutableArray<PatchMethodParameter> GetParameters(PatchMethod patchMethod, WellKnownTypes wellKnownTypes, Compilation compilation)
    {
        var parameters = ImmutableArray.CreateBuilder<PatchMethodParameter>();

        foreach (var parameter in patchMethod.Method.Parameters)
        {
            var patchMethodParameter = GetParameter(patchMethod, parameter, wellKnownTypes, compilation);
            patchMethodParameter.UpdateArgumentOverride(wellKnownTypes);
            parameters.Add(patchMethodParameter);
        }

        return parameters.DrainToImmutable();
    }

    private static PatchMethodParameter GetParameter(PatchMethod patchMethod, IParameterSymbol parameter, WellKnownTypes wellKnownTypes,
        Compilation compilation)
    {
        if (patchMethod.Is(PatchMethodKind.Prepare))
        {
            if (parameter.Type.Is(wellKnownTypes.MethodBase))
                return new PatchMethodParameter(parameter, InjectionKind.OriginalMethod, ParameterMatchKind.ByType);
            if (parameter.Type.Is(wellKnownTypes.HarmonyInstance))
                return new PatchMethodParameter(parameter, InjectionKind.HarmonyInstance, ParameterMatchKind.ByType);
        }
        if (patchMethod.Is(PatchMethodKind.Cleanup))
        {
            if (parameter.Type.Is(wellKnownTypes.MethodBase))
                return new PatchMethodParameter(parameter, InjectionKind.OriginalMethod, ParameterMatchKind.ByType);
            if (parameter.Type.Is(wellKnownTypes.HarmonyInstance))
                return new PatchMethodParameter(parameter, InjectionKind.HarmonyInstance, ParameterMatchKind.ByType);
            if (parameter.Type.Is(wellKnownTypes.Exception))
                return new PatchMethodParameter(parameter, InjectionKind.Exception, ParameterMatchKind.ByType);
        }
        if (patchMethod.Is(PatchMethodKind.TargetMethod) || patchMethod.Is(PatchMethodKind.TargetMethods))
        {
            if (parameter.Type.Is(wellKnownTypes.HarmonyInstance))
                return new PatchMethodParameter(parameter, InjectionKind.HarmonyInstance, ParameterMatchKind.ByType);
        }
        if (patchMethod.Is(PatchMethodKind.Transpiler))
        {
            if (parameter.Type.Is(wellKnownTypes.EnumerableOfCodeInstruction))
                return new PatchMethodParameter(parameter, InjectionKind.Instructions, ParameterMatchKind.ByType);
            if (parameter.Type.Is(wellKnownTypes.ILGenerator))
                return new PatchMethodParameter(parameter, InjectionKind.ILGenerator, ParameterMatchKind.ByType);
            if (parameter.Type.Is(wellKnownTypes.MethodBase))
                return new PatchMethodParameter(parameter, InjectionKind.OriginalMethod, ParameterMatchKind.ByType);
        }

        if (patchMethod.Is(PatchMethodKind.Postfix) && parameter.Ordinal == 0 && parameter.Type.Is(patchMethod.Method.ReturnType, true))
            return new PatchMethodParameter(parameter, InjectionKind.Result, ParameterMatchKind.ByPosition);

        if (patchMethod.Is(PatchMethodKind.Prefix) || patchMethod.Is(PatchMethodKind.Postfix) || patchMethod.Is(PatchMethodKind.Finalizer))
        {
            if (parameter.Name == InstanceParameterName)
                return new PatchMethodParameter(parameter, InjectionKind.Instance, ParameterMatchKind.ByName);
            if (parameter.Name == ResultParameterName)
                return new PatchMethodParameter(parameter, InjectionKind.Result, ParameterMatchKind.ByName);
            if (parameter.Name == ResultRefParameterName && patchMethod.PatchDescription?.HarmonyVersion == 2)
                return new PatchMethodParameter(parameter, InjectionKind.ResultRef, ParameterMatchKind.ByName);
            if (parameter.Name == StateParameterName)
                return new PatchMethodParameter(parameter, InjectionKind.State, ParameterMatchKind.ByName);
            if (parameter.Name == ArgsParameterName)
                return new PatchMethodParameter(parameter, InjectionKind.Args, ParameterMatchKind.ByName);
            if (parameter.Name == OriginalMethodParameterName)
                return new PatchMethodParameter(parameter, InjectionKind.OriginalMethod, ParameterMatchKind.ByName);
            if (parameter.Name == RunOriginalParameterName)
                return new PatchMethodParameter(parameter, InjectionKind.RunOriginal, ParameterMatchKind.ByName);
            if (parameter.Name == ExceptionParameterName && patchMethod.Is(PatchMethodKind.Finalizer))
                return new PatchMethodParameter(parameter, InjectionKind.Exception, ParameterMatchKind.ByName);

            if (parameter.Name.StartsWith(FieldPrefix))
            {
                var fieldNameOrIndex = parameter.Name[FieldPrefix.Length..];

                if (int.TryParse(fieldNameOrIndex, out var index))
                    return new PatchMethodFieldByIndexParameter(parameter, index);

                return new PatchMethodFieldByNameParameter(parameter, fieldNameOrIndex);
            }

            if (parameter.Name.StartsWith(ParameterByIndexPrefix))
            {
                var indexString = parameter.Name[ParameterByIndexPrefix.Length..];
                if (int.TryParse(indexString, out var index))
                    return new PatchMethodParameterByIndexParameter(parameter, index, false);
            }

            var harmonyArgument = parameter.GetAttributes()
                .Concat(patchMethod.Method.GetAttributes())
                .Concat(patchMethod.Method.ContainingType.GetAttributes())
                .Where(attribute => attribute.Is(wellKnownTypes.HarmonyArgument))
                .Select(attribute => HarmonyArgument.Parse(attribute, parameter, wellKnownTypes))
                .FirstOrDefault(argument => argument is not null && (argument.NewName is null || argument.NewName.Value == parameter.Name));
            if (harmonyArgument is not null)
            {
                if (harmonyArgument.Name is not null)
                    return new PatchMethodParameterByNameParameter(parameter, harmonyArgument.Name.Value);
                if (harmonyArgument.Index is not null)
                    return new PatchMethodParameterByIndexParameter(parameter, harmonyArgument.Index.Value, true);
            }

            if (parameter.Type.Is(wellKnownTypes.Delegate) && patchMethod.PatchDescription?.HarmonyVersion == 2)
            {
                var hasHarmonyDelegateAttribute =
                    parameter.Type.GetAttributes().Any(attribute => attribute.Is(wellKnownTypes.HarmonyDelegate));
                var isParameterByName = patchMethod.TargetMethod is not null &&
                                        patchMethod.TargetMethod.Parameters.Any(targetMethodParameter => targetMethodParameter.Name == parameter.Name);
                if (hasHarmonyDelegateAttribute || !isParameterByName)
                    return new PatchMethodParameter(parameter, InjectionKind.Delegate, ParameterMatchKind.ByType);
            }

            return new PatchMethodParameterByNameParameter(parameter, parameter.Name);
        }

        if (patchMethod.Is(PatchMethodKind.ReversePatch))
        {
            // Rather than determining it for sure, check if it can be possibly instance parameter and probably not an ordinary one.
            // This produces better diagnostics.
            if (parameter.Ordinal == 0 && patchMethod.Method.IsStatic && parameter.Type.Is(patchMethod.TargetType, compilation) &&
                (patchMethod.TargetMethod is null || !parameter.Type.Is(patchMethod.TargetMethod.Parameters.FirstOrDefault()?.Type, compilation)))
                return new PatchMethodParameter(parameter, InjectionKind.Instance, ParameterMatchKind.ByPosition);

            return new PatchMethodParameter(parameter, InjectionKind.ParameterByPosition, ParameterMatchKind.ByPosition);
        }

        if (patchMethod.Is(PatchMethodKind.DelegateInvoke))
            return new PatchMethodParameter(parameter, InjectionKind.ParameterByPosition, ParameterMatchKind.ByPosition);

        return new PatchMethodParameter(parameter, InjectionKind.None, ParameterMatchKind.None);
    }

    public void UpdateArgumentOverride(WellKnownTypes wellKnownTypes)
    {
        var attribute = Parameter.GetAttributes().FirstOrDefault(attribute => attribute.Is(wellKnownTypes.HarmonyArgument));
        if (attribute is null)
            return;

        ArgumentOverride = HarmonyArgument.Parse(attribute, Parameter, wellKnownTypes);
    }

    public Location? GetLocation(CancellationToken cancellationToken = default) => 
        Parameter.GetSyntax(cancellationToken: cancellationToken)?.GetIdentifierLocation();

    public Location? GetTypeLocation(CancellationToken cancellationToken = default) => 
        (Parameter.GetSyntax(cancellationToken: cancellationToken) as ParameterSyntax)?.Type?.GetLocation();

    public Location? GetRefLocation(CancellationToken cancellationToken = default)
    {
        if (Parameter.GetSyntax(cancellationToken: cancellationToken) is not ParameterSyntax parameterSyntax)
            return null;
        var refTokens = parameterSyntax.Modifiers
            .Where(token => token.Kind() is SyntaxKind.RefKeyword or SyntaxKind.OutKeyword or SyntaxKind.InKeyword or SyntaxKind.ReadOnlyKeyword)
            .ToArray();
        if (refTokens is [])
            return null;
        return Location.Create(parameterSyntax.SyntaxTree, TextSpan.FromBounds(refTokens.Min(token => token.Span.Start), refTokens.Max(token => token.Span.End)));
    }

    public SyntaxNode? Syntax => Parameter.GetSyntax();

    public override string ToString() => Parameter.ToString();

    public bool IsPassthrough => Kind == InjectionKind.Result && MatchKind == ParameterMatchKind.ByPosition;
}

internal class PatchMethodParameterByNameParameter(IParameterSymbol parameter, string? parameterName)
    : PatchMethodParameter(parameter, InjectionKind.ParameterByName, ParameterMatchKind.ByName)
{
    public string? ParameterName { get; } = parameterName;
}

internal class PatchMethodParameterByIndexParameter(IParameterSymbol parameter, int parameterIndex, bool isByArgumentOverride)
    : PatchMethodParameter(parameter, InjectionKind.ParameterByIndex, ParameterMatchKind.ByName)
{
    public int ParameterIndex { get; } = parameterIndex;
    public bool IsByArgumentOverride { get; } = isByArgumentOverride;
}

internal class PatchMethodFieldByNameParameter(IParameterSymbol parameter, string fieldName)
    : PatchMethodParameter(parameter, InjectionKind.FieldByName, ParameterMatchKind.ByName)
{
    public string FieldName { get; } = fieldName;
}

internal class PatchMethodFieldByIndexParameter(IParameterSymbol parameter, int fieldIndex)
    : PatchMethodParameter(parameter, InjectionKind.FieldByIndex, ParameterMatchKind.ByName)
{
    public int FieldIndex { get; } = fieldIndex;
}