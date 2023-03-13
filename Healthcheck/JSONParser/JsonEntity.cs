using Sprache;

namespace Healthcheck.JSONParser
{
    public abstract class JsonEntity
    {
        public virtual JsonEntity this[string name] =>
            throw new InvalidOperationException($"{GetType().Name} doesn't support this operation.");

        public virtual JsonEntity this[int index] =>
            throw new InvalidOperationException($"{GetType().Name} doesn't support this operation.");

        public virtual T GetValue<T>() =>
            throw new InvalidOperationException($"{GetType().Name} doesn't support this operation.");

        public static JsonEntity Parse(string json) => JsonGrammar.JsonEntity.Parse(json);
    }
}