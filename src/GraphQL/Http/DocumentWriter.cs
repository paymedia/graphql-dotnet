using Newtonsoft.Json;

namespace GraphQL.Http
{
    public interface IDocumentWriter
    {
        string Write(object value);
    }

    public class DocumentWriter : IDocumentWriter
    {
        private readonly Formatting _formatting;
        private readonly JsonSerializerSettings _settings;

        public DocumentWriter()
            : this(indent: false)
        {
        }

        public DocumentWriter(bool indent)
            : this(
                indent ? Formatting.Indented : Formatting.None,
                new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
                })
        {
        }

        public DocumentWriter(Formatting formatting, JsonSerializerSettings settings)
        {
            _formatting = formatting;
            _settings = settings;
        }

        public string Write(object value)
        {
            return JsonConvert.SerializeObject(value, _formatting, _settings);
        }
    }
}
