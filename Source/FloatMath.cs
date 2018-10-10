namespace AssetGenerator
{
    static class FloatMath
    {
        public const float Pi = 3.14159265f;

        public static float ConvertDegreesToRadians(float degrees)
        {
            return degrees * Pi / 180;
        }
    }
}
