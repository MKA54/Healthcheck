namespace Healthcheck.JSONParser
{
    public class JsonArray : JsonEntity
    {
        public IReadOnlyList<JsonEntity> Children { get; }

        public JsonArray(IReadOnlyList<JsonEntity> children)
        {
            Children = children;
        }

        public override JsonEntity this[int index] => Children.ElementAtOrDefault(index);
    }
}
