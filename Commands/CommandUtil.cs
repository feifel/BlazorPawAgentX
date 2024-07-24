using System.Text.Json.Nodes;
using System.Text.Json;

namespace BlazorPawAgentX.Commands
{
    public class CommandUtil
    {
        public static Object? Deserialize(string json)
        {
            var typeName = JsonSerializer.Deserialize<JsonNode>(json)?["TypeName"]?.GetValue<string>();
            if (typeName != null)
            {
                var type = Type.GetType(typeName);
                if (type != null) return JsonSerializer.Deserialize(json, type);
            }
            throw new Exception($"Could not deserialize command: {json}");
        }
    }
}
