using Newtonsoft.Json;
using System;
using System.Numerics;

namespace AssetGenerator
{
    class Vector3ToFloatArrayJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Vector3 vec3 = (Vector3)value;
            float[] floatArray = new float[3];
            vec3.CopyTo(floatArray);
            serializer.Serialize(writer, floatArray);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(Vector3));
        }

        public override bool CanRead
        {
            get { return false; }
        }
    }
}
