namespace AssetGenerator
{
    static class FloatMath
    {
        public const float Pi = 3.14159265f;
        public const float NinetyDegreesInRadians = Pi / 2;

        public static float ConvertDegreesToRadians(float degrees)
        {
            return degrees * Pi / 180.0f;
        }
    }
}
