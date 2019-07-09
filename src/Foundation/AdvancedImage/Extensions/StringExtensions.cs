namespace AdvancedImage.Extensions
{
    public static class StringExtensions
    {
        public static bool Empty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool ParseOrDefault(this string value, bool defaultValue)
        {
            if (value.Empty())
            {
                return defaultValue;
            }

            return bool.TryParse(value, out var parsedValue)
                ? parsedValue
                : defaultValue;
        }
    }
}