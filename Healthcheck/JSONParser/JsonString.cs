namespace Healthcheck.JSONParser
{
    public class JsonString : JsonLiteral<string>
    {
        public JsonString(string value) : base(value)
        {
        }
    }
}