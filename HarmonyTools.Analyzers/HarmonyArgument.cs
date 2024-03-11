using Microsoft.CodeAnalysis;

namespace HarmonyTools.Analyzers;

internal class HarmonyArgument : IHasSyntax
{
    public AttributeData Attribute { get; }
    public DetailWithSyntax<string?>? Name { get; private set; }
    public DetailWithSyntax<int>? Index { get; private set; }
    public DetailWithSyntax<string?>? NewName { get; private set; }

    private HarmonyArgument(AttributeData attribute)
    {
        Attribute = attribute;
    }

    public static HarmonyArgument? Parse(AttributeData attribute, WellKnownTypes wellKnownTypes)
    {
        if (attribute.IsMatch(wellKnownTypes.String))
            return new HarmonyArgument(attribute)
            {
                Name = attribute.GetDetailWithSyntax<string?>(0),
            };
        if (attribute.IsMatch(wellKnownTypes.Int32))
            return new HarmonyArgument(attribute)
            {
                Index = attribute.GetDetailWithSyntax<int>(0),
            };
        if (attribute.IsMatch(wellKnownTypes.String, wellKnownTypes.String))
            return new HarmonyArgument(attribute)
            {
                Name = attribute.GetDetailWithSyntax<string?>(0),
                NewName = attribute.GetDetailWithSyntax<string?>(1),
            };
        if (attribute.IsMatch(wellKnownTypes.Int32, wellKnownTypes.String))
            return new HarmonyArgument(attribute)
            {
                Index = attribute.GetDetailWithSyntax<int>(0),
                NewName = attribute.GetDetailWithSyntax<string?>(1),
            };

        return null;
    }

    public SyntaxNode? Syntax => Attribute.GetSyntax();
}