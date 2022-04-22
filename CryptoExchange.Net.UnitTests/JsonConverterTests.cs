using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoExchange.Net.DataProcessors;
using CryptoExchange.Net.Logging;
using NUnit.Framework;

namespace CryptoExchange.Net.UnitTests
{
    [TestFixture()]
    public class JsonConverterTests
    {
        [TestCase]
        public void DeserializingValidJson_Should_GiveSuccessfulResult()
        {
            // arrange
            Log log = new Log("test");
            var topic = new JsonDataConverter(log, new Newtonsoft.Json.JsonSerializer());

            // act
            var result = topic.Deserialize<object>(0, "{\"testProperty\": 123}", default);

            // assert
            Assert.IsTrue(result.Success);
        }

        [TestCase]
        public void DeserializingInvalidJson_Should_GiveErrorResult()
        {
            // arrange
            Log log = new Log("test");
            var topic = new JsonDataConverter(log, new Newtonsoft.Json.JsonSerializer());

            // act
            var result = topic.Deserialize<object>(0, "{\"testProperty\": 123", default);

            // assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Error != null);
        }
    }
}
