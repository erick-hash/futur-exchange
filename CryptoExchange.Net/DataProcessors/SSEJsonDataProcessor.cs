using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoExchange.Net.DataProcessors
{
    /// <summary>
    /// Server side events with json data IDataConverter implementation. 
    /// This converter only works for deserialization.
    /// </summary>
    public class SSEJsonDataConverter : JsonDataConverter
    {
        private MethodInfo _deserializeTokenMethod;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="log"></param>
        /// <param name="serializer"></param>
        public SSEJsonDataConverter(Log log, JsonSerializer serializer) : base(log, serializer)
        {
             _deserializeTokenMethod = 
                typeof(JsonDataConverter).GetMethod("DeserializeToken", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetBaseDefinition();
        }

        /// <inheritdoc />
        public override CallResult<T> Deserialize<T>(int id, string dataString, CancellationToken ct)
        {
            return ParseData<T>(id, dataString);
        }

        /// <inheritdoc />
        public override async Task<CallResult<T>> DeserializeAsync<T>(int id, Stream dataStream, CancellationToken ct)
        {
            using var reader = new StreamReader(dataStream);
            return await DeserializeAsync<T>(id, await reader.ReadToEndAsync().ConfigureAwait(false), ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override Task<CallResult<T>> DeserializeAsync<T>(int id, string dataString, CancellationToken ct)
        {
            return Task.FromResult(ParseData<T>(id, dataString));
        }

        /// <summary>
        /// Parse the received data string into a model. This expects data to come in in the following format:
        /// 
        /// event: start
        /// data: start
        /// 
        /// event: data1
        /// data: { \"jsonData\": 1 }
        /// 
        /// event: data2
        /// data: { \"jsonData\": 2 }
        /// 
        /// event: end
        /// data: end
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="dataString"></param>
        /// <returns></returns>
        protected virtual CallResult<T> ParseData<T>(int id, string dataString)
        {
            var result = Activator.CreateInstance(typeof(T));
            var lines = dataString.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < lines.Length; i++)
            {
                var currentLine = lines[i];
                if (string.IsNullOrEmpty(currentLine))
                    continue;

                if (currentLine.StartsWith("event:"))
                {
                    var eventName = currentLine.Split(':')[1].Trim(' ');
                    if (eventName == "start" || eventName == "end")
                        continue;

                    var property = typeof(T).GetProperty(eventName.Substring(0, 1).ToUpper() + eventName.Substring(1));

                    var data = lines[i + 1].Substring(6).Trim(' ');
                    var token = ValidateJson(data);
                    if (!token.Success)
                        return new CallResult<T>(token.Error!);
                
                    var fooRef = _deserializeTokenMethod.MakeGenericMethod(property.PropertyType);
                    var desResult = (dynamic)fooRef.Invoke(this, new object[] { id, token.Data });

                    property.SetValue(result, desResult.Data);
                }
            }

            return new CallResult<T>((T)result);
        }
    }
}
