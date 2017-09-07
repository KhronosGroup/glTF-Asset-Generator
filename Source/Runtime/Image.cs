using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Wrapper for glTF loader's Image
    /// </summary>
    public class Image
    {
        /// <summary>
        /// The location of the image file, or a data uri containing texture data as an encoded string
        /// </summary>
        public string uri;

        /// <summary>
        /// The user-defined name of the image
        /// </summary>
        public string name;

        /// <summary>
        /// The image's mimetype
        /// </summary>
        public glTFLoader.Schema.Image.MimeTypeEnum? mimeType;
        /// <summary>
        /// converts the GLTFImage to a glTF Image
        /// </summary>
        /// <returns>Returns an Image object</returns>
        public glTFLoader.Schema.Image convertToImage()
        {
            glTFLoader.Schema.Image image = new glTFLoader.Schema.Image
            {
                Uri = uri
            };
            if (mimeType.HasValue)
            {
                image.MimeType = mimeType.Value;
            }
            if (name != null)
            {
                image.Name = name;
            }
            return image;
        }
    }
}
