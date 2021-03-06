using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace GraphQL
{
    public class ExecutionResultJsonConverter : JsonConverter
    {
        public const string ErrorTypeSystem = "System";
        public const string ErrorTypeApplication = "Application";

        public List<string> ApplicationExceptionExclusions = new List<string>
        {
            "DataAccessException"
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is ExecutionResult)
            {
                writer.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
             
                var result = (ExecutionResult) value;

                writer.WriteStartObject();

                writeData(result, writer, serializer);
                if (!string.IsNullOrEmpty(result.Crc))
                {

                    writeCrc(result, writer, serializer);
                }
                writeErrors(result.Errors, writer, serializer);

                writer.WriteEndObject();
            }
        }

        private void writeData(ExecutionResult result, JsonWriter writer, JsonSerializer serializer)
        {
            var data = result.Data;

            //if (result.Errors?.Count > 0 && data == null)
            //{
            //    return;
            //}

            //if (result.Errors?.Count > 0)
            //{
            //    data = null;
            //}

            writer.WritePropertyName("data");
            serializer.Serialize(writer, data);
        }

        private void writeCrc(ExecutionResult result, JsonWriter writer, JsonSerializer serializer)
        {
            var data = result.Data;

            writer.WritePropertyName("crc");
            serializer.Serialize(writer, result.Crc);
        }


        private void writeErrors(ExecutionErrors errors, JsonWriter writer, JsonSerializer serializer)
        {
            var errorType = ErrorTypeSystem;

            if (errors == null || errors.Count == 0)
            {
                return;
            }

            writer.WritePropertyName("errors");

            writer.WriteStartArray();

            errors.Apply(error =>
            {
                writer.WriteStartObject();

                writer.WritePropertyName("message");
                var exceptionMsg = error.InnerException?.Message;
                if (string.IsNullOrEmpty(exceptionMsg) || error.Message.Contains(exceptionMsg))
                {
                    serializer.Serialize(writer, $"{error.Message}");
                }
                else
                {
                    serializer.Serialize(writer, $"{error.Message} {exceptionMsg}");
                }

                var exceptionType = error.InnerException?.GetType();
                var applicationExceptionType = Type.GetType("System.ApplicationException");

                if (exceptionType != null && !ApplicationExceptionExclusions.Contains(exceptionType.Name) && applicationExceptionType.IsAssignableFrom(exceptionType) )
                    errorType = ErrorTypeApplication;

                writer.WritePropertyName("extensions");
                writer.WriteStartObject();
                writer.WritePropertyName("type");
                writer.WriteValue(errorType);

                if (error.AdditionalErrorProperties != null)
                {
                    foreach (var additionalErrorProperty in error.AdditionalErrorProperties)
                    {
                        writer.WritePropertyName(additionalErrorProperty.Key);
                        writer.WriteValue(additionalErrorProperty.Value);
                    }
                }

                writer.WritePropertyName("userMessage");
                writer.WriteValue(error.InnerException?.Message);
                writer.WriteEnd();

                writer.WriteEndObject();
            });

            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ExecutionResult);
        }
    }
}
