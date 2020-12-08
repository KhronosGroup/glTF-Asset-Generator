using AssetGenerator.Conversion;
using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AssetGenerator
{
    /// <summary>
    /// Custom JsonConverter. Converts a Vector3 into a float array.
    /// Intended for use in adding camera translations to the manifest.
    /// </summary>
    class Vector3ToFloatArrayJsonConverter : JsonConverter<Vector3>
    {
        public override void Write(
            Utf8JsonWriter writer,
            Vector3 vec3,
            JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, vec3.ToArray(), options);

        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            throw new NotImplementedException();

    }
}
