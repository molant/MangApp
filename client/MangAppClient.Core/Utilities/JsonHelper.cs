namespace MangAppClient.Core.Utilities
{
    using Newtonsoft.Json.Linq;

    public static class JsonHelper
    {
        // Working
        public static string ParseString(JToken stringToken)
        {
            if (stringToken == null)
            {
                return null;
            }

            return stringToken.Value<string>();
        }

        // Working
        public static int? ParseInt(JToken intToken)
        {
            if (intToken == null)
            {
                return null;
            }

            try
            {
                return intToken.Value<int?>();
            }
            catch
            {
                return null;
            }
        }
    }
}
