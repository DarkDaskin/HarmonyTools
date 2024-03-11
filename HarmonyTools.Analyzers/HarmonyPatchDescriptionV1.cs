using HarmonyTools.Analyzers.HarmonyEnums;
using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class HarmonyPatchDescriptionV1(ISymbol symbol) : HarmonyPatchDescription(symbol)
{
    public override int HarmonyVersion => 1;

    public static HarmonyPatchDescriptionSet<HarmonyPatchDescriptionV1> Parse(INamedTypeSymbol type, WellKnownTypes wellKnownTypes) =>
        Parse(type, wellKnownTypes, symbol => new HarmonyPatchDescriptionV1(symbol));

    protected override void ProcessHarmonyPatchAttribute(AttributeData attribute, WellKnownTypes wellKnownTypes)
    {
        base.ProcessHarmonyPatchAttribute(attribute, wellKnownTypes);
      
        if (attribute.IsMatch(wellKnownTypes.String, wellKnownTypes.PropertyMethod!))
        {
            MethodNames = MethodNames.Add(attribute.GetDetailWithSyntax<string?>(0));
            var propertyMethodDetail = attribute.GetDetailWithSyntax<PropertyMethod>(1);
            MethodTypes = MethodTypes.Add(new DetailWithSyntax<MethodType>(Map(propertyMethodDetail.Value), propertyMethodDetail.Syntax));
        }

        static MethodType Map(PropertyMethod value) => value switch
        {
            PropertyMethod.Getter => MethodType.Getter,
            PropertyMethod.Setter => MethodType.Setter,
            _ => (MethodType)int.MaxValue
        };
    }
}