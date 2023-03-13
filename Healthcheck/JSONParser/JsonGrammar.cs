using Sprache;
using System.Globalization;
using System.Text.Json.Nodes;

namespace Healthcheck.JSONParser
{
    public static class JsonGrammar
    {
        private static readonly Parser<JsonNull> JsonNull =
            Parse.String("null").Return(new JsonNull());

        private static readonly Parser<JsonNull> JsonEmptyArray =
            Parse.String("[]").Return(new JsonNull());

        private static readonly Parser<JsonBoolean> TrueJsonBoolean =
            Parse.String("true").Return(new JsonBoolean(true));

        private static readonly Parser<JsonBoolean> FalseJsonBoolean =
            Parse.String("false").Return(new JsonBoolean(false));

        private static readonly Parser<JsonBoolean> JsonBoolean =
            TrueJsonBoolean.Or(FalseJsonBoolean);

        private static readonly Parser<JsonNumber> IntegerJsonNumber =
            from minus in Parse.Char('-').Once().Text().Optional()
            from integer in Parse.Digit.AtLeastOnce().Text()
            let str = minus.GetOrDefault() + integer
            select new JsonNumber(double.Parse(str, CultureInfo.InvariantCulture));

        private static readonly Parser<JsonNumber> FloatJsonNumber =
            from minus in Parse.Char('-').Once().Text().Optional()
            from integer in Parse.Digit.AtLeastOnce().Text()
            from separator in Parse.Char('.').Once().Text()
            from fractional in Parse.Digit.AtLeastOnce().Text()
            let str = minus.GetOrDefault() + integer + separator + fractional
            select new JsonNumber(double.Parse(str, CultureInfo.InvariantCulture));

        private static readonly Parser<JsonNumber> JsonNumber = FloatJsonNumber.Or(IntegerJsonNumber);

        private static readonly Parser<JsonString> JsonString =
            from open in Parse.Char('"')
            from value in Parse.CharExcept('"').Many().Text()
            from close in Parse.Char('"')
            select new JsonString(value);

        private static readonly Parser<JsonArray> JsonArray =
            from open in Parse.Char('[')
            from children in JsonEntity.Token().DelimitedBy(Parse.Char(','))
            from close in Parse.Char(']')
            select new JsonArray(children.ToArray());

        private static readonly Parser<KeyValuePair<string, JsonEntity>> JsonProperty =
            from name in JsonString.Select(s => s.Value)
            from colon in Parse.Char(':').Token()
            from value in JsonEntity
            select new KeyValuePair<string, JsonEntity>(name, value);

        private static readonly Parser<JsonObject> JsonObject =
            from open in Parse.Char('{')
            from properties in JsonProperty.Token().DelimitedBy(Parse.Char(','))
            from close in Parse.Char('}')
            select new JsonObject(properties.ToDictionary(p => p.Key, p => p.Value));

        public static readonly Parser<JsonEntity> JsonEntity =
            JsonObject
                .Or<JsonEntity>(JsonArray)
                .Or(JsonEmptyArray)
                .Or(JsonString)
                .Or(JsonNumber)
                .Or(JsonBoolean)
                .Or(JsonNull);
    }
}
