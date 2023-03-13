namespace Healthcheck.JSONParser
{
    public class JsonNull : JsonLiteral<object>
    {
        public JsonNull() : base(null)
        {
        }
    }
}