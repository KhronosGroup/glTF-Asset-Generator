using System.Numerics;

namespace AssetGenerator.Runtime.Extensions
{
    internal class KHR_texture_transform : Extension
    {
        /// <summary>
        /// The name of the extension
        /// </summary>
        public override string Name
        {
            get
            {
                return "KHR_texture_transform";
            }
        }

        public Vector2? Offset { get; set; }
        public float Rotation { get; set; }
        public Vector2? Scale { get; set; }
        public int? TexCoord { get; set; }
    }
}
