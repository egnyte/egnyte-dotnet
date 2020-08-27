using System;
using System.Collections.Generic;
using Egnyte.Api.Permissions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Egnyte.Api.Common
{
    public class GroupOrUserPermissionsResponseV2Converter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary<string, string>).IsAssignableFrom(objectType) && objectType != typeof(Dictionary<string, string>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var dictionary = existingValue as IDictionary<string, string> ?? new Dictionary<string, string>();

            var token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                foreach (var item in token)
                    using (var subReader = item.CreateReader())
                    {
                        serializer.Populate(subReader, dictionary);
                    }
            }
            else if (token.Type == JTokenType.Object)
            {
                using (var subReader = token.CreateReader())
                {
                    serializer.Populate(subReader, dictionary);
                }
            }

            var permissions = new List<GroupOrUserPermissionsResponse>();

            foreach (var kvp in dictionary)
            {
                permissions.Add(new GroupOrUserPermissionsResponse
                {
                    Subject = kvp.Key,
                    Permission = kvp.Value
                });
            }

            return permissions;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {;
            var permissions = value as List<GroupOrUserPermissionsResponse> ?? new List<GroupOrUserPermissionsResponse>();

            var dictionary = new Dictionary<string, string>();
            foreach (var p in permissions)
            {
                dictionary.Add(p.Subject, p.Permission);
            }

            serializer.Serialize(writer, dictionary);
        }
    }
}
