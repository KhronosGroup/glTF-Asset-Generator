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
    }
}
