namespace AssetGenerator
{
    public class Attribute
    {
        public AttributeName name { get; }
        public dynamic value; // Could be a float, array of floats, string, or enum
        public AttributeName prerequisite = AttributeName.Undefined;
        public int attributeGroup;

        public Attribute(AttributeName attributeName, dynamic attributeValue, AttributeName ParentAttribute = AttributeName.Undefined, int group = 0)
        {
            name = attributeName;
            value = attributeValue;
            prerequisite = ParentAttribute;
            attributeGroup = group;
        }
    }
}
