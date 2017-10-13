using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Wrapper for Image class
    /// </summary>
    internal class Image
    {
        /// <summary>
        /// The location of the image file, or a data uri containing texture data as an encoded string
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// The user-defined name of the image
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The image's mimetype
        /// </summary>
        public glTFLoader.Schema.Image.MimeTypeEnum? MimeType { get; set; }
        /// <summary>
        /// converts the Runtime image to a glTF Image
        /// </summary>
        /// <returns>Returns a gltf Image object</returns>
        public glTFLoader.Schema.Image ConvertToImage()
        {
            glTFLoader.Schema.Image image = new glTFLoader.Schema.Image
            {
                Uri = Uri
            };
            if (MimeType.HasValue)
            {
                image.MimeType = MimeType.Value;
            }
            if (Name != null)
            {
                image.Name = Name;
            }
            return image;
        }
    }
}
