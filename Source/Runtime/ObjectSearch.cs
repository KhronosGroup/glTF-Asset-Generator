using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Utility functor for finding objects that equal each other within a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectSearch<T>
    {
        public ObjectSearch(T obj)
        {
            this.obj = obj;
        }
        public T obj { get; set; }
        public bool Equals(T obj)
        {
            if ((obj as glTFLoader.Schema.Sampler) != null)
            {
                return SamplersEqual(obj as glTFLoader.Schema.Sampler, this.obj as glTFLoader.Schema.Sampler);
            }
            else if ((obj as glTFLoader.Schema.Texture) != null)
            {
                return TexturesEqual(obj as glTFLoader.Schema.Texture, this.obj as glTFLoader.Schema.Texture);
            }
            else if ((obj as glTFLoader.Schema.Image) != null)
            {
                return ImagesEqual(obj as glTFLoader.Schema.Image, this.obj as glTFLoader.Schema.Image);
            }
            else
                return this.obj.Equals(obj);

        }
        /// <summary>
        /// Function which determines if two Sampler objects have equal values
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool SamplersEqual(glTFLoader.Schema.Sampler s1, glTFLoader.Schema.Sampler s2)
        {
            return ((s1.MagFilter == s2.MagFilter) && (s1.MinFilter == s2.MinFilter) && (s1.Name == s2.Name) && (s1.WrapS == s2.WrapS) && (s1.WrapT == s2.WrapT));

        }
        /// <summary>
        /// Function which determines if two Textures objects have equal values
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static bool TexturesEqual(glTFLoader.Schema.Texture t1, glTFLoader.Schema.Texture t2)
        {
            return ((t1.Name == t2.Name) && (t1.Source == t2.Source) && (t1.Sampler == t2.Sampler));
        }
        /// <summary>
        /// Function which determines if two Image objects have equal values
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool ImagesEqual(glTFLoader.Schema.Image i1, glTFLoader.Schema.Image i2)
        {
            return ((i1.Name == i2.Name) && (i1.Uri == i2.Uri) && i1.MimeType == i2.MimeType);
        }
    }
}
