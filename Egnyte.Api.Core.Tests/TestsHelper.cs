namespace Egnyte.Api.Tests
{
    public static class TestsHelper
    {
        public static string RemoveWhitespaces(string text)
        {
            return text.Replace(" ", "")
                .Replace("\n", "")
                .Replace("\t", "")
                .Replace("\r", "");
        }
    }
}
