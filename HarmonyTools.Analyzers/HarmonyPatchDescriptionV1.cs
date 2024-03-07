using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class HarmonyPatchDescriptionV1 : HarmonyPatchDescription
{
    public override int HarmonyVersion => 1;

    public static HarmonyPatchDescriptionV1? Parse(INamedTypeSymbol type, Compilation compilation) => 
        Parse<HarmonyPatchDescriptionV1>(type, compilation, "Harmony");

    protected override void ProcessHarmonyPatchAttribute(AttributeData attribute, WellKnownTypes wellKnownTypes)
    {
        base.ProcessHarmonyPatchAttribute(attribute, wellKnownTypes);
      
        if (IsMatch(attribute.AttributeConstructor, wellKnownTypes.String, wellKnownTypes.PropertyMethod!))
        {
            MethodNames = MethodNames.Add(GetDetailWithSyntax<string?>(attribute, 0));
            var propertyMethodDetail = GetDetailWithSyntax<PropertyMethod>(attribute, 1);
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