using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Z3.UIBuilder.Core
{
    public static class Serializer
    {
        private static JsonSerializerSettings Settings => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = new WritablePropertiesOnlyResolver()
        };

        private static JsonSerializerSettings ReadableSettings => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented,
            ContractResolver = new WritablePropertiesOnlyResolver()
        };

        public static string ToJson<T>(T data)
        {
            return JsonConvert.SerializeObject(data, Settings);
        }

        public static string ToReadableJson<T>(T data)
        {
            return JsonConvert.SerializeObject(data, ReadableSettings);
        }

        public static object FromJson(string data) => FromJson<object>(data);

        public static T FromJson<T>(string data)
        {
            using StringReader stringReader = new StringReader(data);
            using CustomReader jsonReader = new CustomReader(stringReader);
            return JsonSerializer.CreateDefault(Settings).Deserialize<T>(jsonReader);
        }

        public static T FromReadableJson<T>(string data)
        {
            using StringReader stringReader = new StringReader(data);
            using CustomReader jsonReader = new CustomReader(stringReader);
            return JsonSerializer.CreateDefault(ReadableSettings).Deserialize<T>(jsonReader);
        }

        /// <summary>
        /// Used to serialize only writable fields
        /// </summary>
        private class WritablePropertiesOnlyResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
                return props.Where(p => p.Writable).ToList();
            }
        }

        /// <summary>
        /// Used to give preference to deserialize numeric objects as Int instead Long
        /// </summary>
        /// <remarks>
        /// Source: https://stackoverflow.com/questions/8297541/how-do-i-change-the-default-type-for-numeric-deserialization
        /// </remarks>
        private class CustomReader : JsonTextReader
        {
            public CustomReader(TextReader reader) : base(reader) { }

            public override bool Read()
            {
                bool ret = base.Read();

                // TODO: Review long and double deserialization
                if (ValueType == typeof(long) && Value is long longValue)
                {
                    if (TokenType == JsonToken.Integer)
                    {
                        bool valueIsInsideBoundaries = longValue <= int.MaxValue && longValue >= int.MinValue;

                        if (valueIsInsideBoundaries)
                        {
                            int newValue = checked((int)longValue);
                            SetToken(TokenType, newValue, false);
                        }
                    }
                }
                else if (ValueType == typeof(double) && Value is double doubleValue)
                {
                    if (TokenType == JsonToken.Float)
                    {
                        bool valueIsInsideBoundaries = doubleValue <= float.MaxValue && doubleValue >= float.MinValue;

                        if (valueIsInsideBoundaries)
                        {
                            float newValue = (float)doubleValue;
                            SetToken(TokenType, newValue, false);
                        }
                    }
                }

                return ret;
            }
        }
    }
}