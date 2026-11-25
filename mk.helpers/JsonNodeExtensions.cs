using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace mk.helpers
{
    /// <summary>
    /// Helpers for JsonNode and JsonObject.
    /// </summary>
    public static class JsonNodeExtensions
    {
        private static readonly JsonNodeOptions _nodeOptions = new JsonNodeOptions { PropertyNameCaseInsensitive = true };
        private static readonly JsonDocumentOptions _documentOptions = new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };

        /// <summary>
        /// Reads a JSON object, allowing comments and trailing commas. Empty text gives an empty object.
        /// </summary>
        public static JsonObject ParseJsonObject(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new JsonObject(_nodeOptions);

            if (JsonNode.Parse(text, _nodeOptions, _documentOptions) is JsonObject result)
                return result;

            throw new JsonException("The JSON is not an object");
        }

        /// <summary>
        /// True when the JSON has // or /* */ comments.
        /// </summary>
        public static bool HasJsonComments(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(text), new JsonReaderOptions { CommentHandling = JsonCommentHandling.Allow, AllowTrailingCommas = true });
            try
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.Comment)
                        return true;
                }
            }
            catch (JsonException)
            {
                // broken json
            }

            return false;
        }

        /// <summary>
        /// Returns the child object under key, creating it if it's missing or not an object.
        /// </summary>
        public static JsonObject GetOrCreateObject(this JsonObject parent, string key)
        {
            if (parent[key] is JsonObject existing)
                return existing;

            var created = new JsonObject(_nodeOptions);
            parent[key] = created;
            return created;
        }

        /// <summary>
        /// The node's string value, or null when it's missing or not a string.
        /// </summary>
        public static string ReadString(this JsonNode node)
        {
            return node is JsonValue value && value.TryGetValue<string>(out var text) ? text : null;
        }

        /// <summary>
        /// The node's bool value, or null when it's missing or not a bool.
        /// </summary>
        public static bool? ReadBool(this JsonNode node)
        {
            return node is JsonValue value && value.TryGetValue<bool>(out var flag) ? flag : (bool?)null;
        }

        /// <summary>
        /// The node's int value, or null when it's missing or not a whole number.
        /// </summary>
        public static int? ReadInt(this JsonNode node)
        {
            return node is JsonValue value && value.TryGetValue<int>(out var number) ? number : (int?)null;
        }

        /// <summary>
        /// Returns a copy of the base object with the overrides laid on top. Child objects merge.
        /// </summary>
        public static JsonObject DeepMerge(this JsonObject baseObject, JsonObject overrides)
        {
            var result = (JsonObject)baseObject.DeepClone();
            foreach (var x in overrides)
            {
                if (x.Value is JsonObject overChild && result[x.Key] is JsonObject baseChild)
                    result[x.Key] = baseChild.DeepMerge(overChild);
                else
                    result[x.Key] = x.Value?.DeepClone();
            }

            return result;
        }

        /// <summary>
        /// Turns an object into a case-insensitive string dictionary. Non-string values become their JSON.
        /// </summary>
        public static Dictionary<string, string> ToStringDictionary(this JsonNode node)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!(node is JsonObject obj))
                return result;

            foreach (var x in obj)
            {
                if (x.Value == null)
                    result[x.Key] = "";
                else
                    result[x.Key] = x.Value.ReadString() ?? x.Value.ToJsonString();
            }

            return result;
        }

        /// <summary>
        /// Turns string pairs into a JSON object of strings.
        /// </summary>
        public static JsonObject ToJsonObject(this IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var result = new JsonObject(_nodeOptions);
            foreach (var x in pairs)
                result[x.Key] = x.Value;

            return result;
        }
    }
}
