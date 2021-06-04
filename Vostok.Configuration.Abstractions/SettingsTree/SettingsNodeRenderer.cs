using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.SettingsTree
{
    internal static class SettingsNodeRenderer
    {
        private const string NullRepresentation = "null";
        private const string OpeningSquareBracket = "[";
        private const string OpeningCurlyBracket = "{";
        private const string ClosingSquareBracket = "]";
        private const string ClosingCurlyBracket = "}";

        private const char Quote = '"';
        private const char Comma = ',';
        private const char Colon = ':';
        private const char Space = ' ';

        public static string Render([CanBeNull] ISettingsNode node)
        {
            var builder = new StringBuilder();

            RenderInternal(node, builder, 0);

            return builder.ToString();
        }

        private static void RenderInternal([CanBeNull] ISettingsNode node, [NotNull] StringBuilder builder, int depth)
        {
            switch (node)
            {
                case ValueNode valueNode when valueNode.Value == null:
                    builder
                        .Indent(depth)
                        .Append(NullRepresentation);
                    break;
                    
                case ValueNode valueNode:
                    builder
                        .Indent(depth)
                        .Append(Quote)
                        .Append(valueNode.Value)
                        .Append(Quote);
                    break;

                case ArrayNode arrayNode:
                    if (arrayNode.ChildrenCount > 0)
                    {

                        builder
                            .Indent(depth)
                            .AppendLine(OpeningSquareBracket);

                        var counter = 0;

                        foreach (var child in arrayNode.Children)
                        {
                            RenderInternal(child, builder, depth + 1);

                            if (++counter < arrayNode.ChildrenCount)
                                builder.Append(Comma);

                            builder.AppendLine();
                        }

                        builder
                            .Indent(depth)
                            .Append(ClosingSquareBracket);
                    }
                    else builder
                        .Indent(depth)
                        .Append(OpeningSquareBracket)
                        .Append(ClosingSquareBracket);

                    break;

                case ObjectNode objectNode:
                    if (objectNode.ChildrenCount > 0)
                    {
                        builder
                            .Indent(depth)
                            .AppendLine(OpeningCurlyBracket);

                        var counter = 0;

                        foreach (var child in objectNode.Children)
                        {
                            builder
                                .Indent(depth + 1)
                                .Append(Quote)
                                .Append(child.Name)
                                .Append(Quote)
                                .Append(Colon)
                                .Append(Space);

                            if (child.Children.Any())
                            {
                                builder.AppendLine();
                                RenderInternal(child, builder, depth + 1);
                            }
                            else
                            {
                                RenderInternal(child, builder, 0);
                            }

                            if (++counter < objectNode.ChildrenCount)
                                builder.Append(Comma);

                            builder.AppendLine();
                        }

                        builder
                            .Indent(depth)
                            .Append(ClosingCurlyBracket);
                    }
                    else builder
                        .Indent(depth)
                        .Append(OpeningCurlyBracket)
                        .Append(ClosingCurlyBracket);

                    break;

                default:
                    builder
                        .Indent(depth)
                        .Append(node?.ToString() ?? NullRepresentation);

                    break;
            }
        }

        private static StringBuilder Indent([NotNull] this StringBuilder builder, int depth)
            => builder.Append(Space, depth * 3);
    }
}
