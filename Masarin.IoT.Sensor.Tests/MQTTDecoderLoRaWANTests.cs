using System;
using System.Text;
using Xunit;
using Masarin.IoT.Sensor;
using Moq;
using Fiware;

namespace Masarin.IoT.Sensor.Tests
{
    public class MQTTDecoderLoRaWANTests
    {
        [Fact]
        public void TestThatDeviceMessageIsPostedWithCorrectId()
        {
            var contextBroker = new Mock<IContextBrokerProxy>();
            var decoder = new MQTTDecoderLoRaWAN(contextBroker.Object);
            var payload = "{\"deviceName\":\"sn-elt-livboj-02\",\"devEUI\":\"a81758fffe04d854\",\"data\":\"Bw4yDQA=\",\"object\":{\"present\":true}}";

            decoder.Decode("2020-08-26T07:11:31Z", "iothub", "out", Encoding.UTF8.GetBytes(payload));

            contextBroker.Verify(foo => foo.PostMessage(It.Is<DeviceMessage>(mo => mo.Id == "urn:ngsi-ld:Device:se:servanet:lora:sn-elt-livboj-02")));
        }

        [Fact]
        public void TestThatDeviceMessageIsPostedWithCorrectValue()
        {
            var contextBroker = new Mock<IContextBrokerProxy>();
            var decoder = new MQTTDecoderLoRaWAN(contextBroker.Object);
            var payload = "{\"deviceName\":\"sk-elt-temp-20\",\"devEUI\":\"a81758fffe04d834\",\"data\":\"Bw45DABu\",\"object\":{\"externalTemperature\":11,\"vdd\":3641},\"tags\":{\"Location\":\"Flasian_south\"}}";

            decoder.Decode("2020-08-26T07:11:31Z", "iothub", "out", Encoding.UTF8.GetBytes(payload));

            contextBroker.Verify(foo => foo.PostMessage(It.Is<DeviceMessage>(mo => mo.Value.Value == "t%3D11")));
        }

        [Fact]
        public void TestThatDecoderHandlesNullObjectProperly()
        {
            var contextBroker = new Mock<IContextBrokerProxy>();
            var decoder = new MQTTDecoderLoRaWAN(contextBroker.Object);
            var payload = "{\"deviceName\":\"sk-elt-temp-01\",\"devEUI\":\"xxxxxxxxxxxxxxx\",\"data\":null,\"object\":{},\"tags\":{\"Location\":\"Sidsjobacken\"}}";

            decoder.Decode("2020-10-07T15:46:45Z", "iothub", "out", Encoding.UTF8.GetBytes(payload));

            contextBroker.Verify(foo => foo.PostMessage(It.IsAny<DeviceMessage>()), Times.Never());
        }

        [Fact]
        public void TestThatErsCo2CanFetchCO2()
        {
            var contextBroker = new Mock<IContextBrokerProxy>();
            var decoder = new MQTTDecoderLoRaWAN(contextBroker.Object);
            var payload = "{\"deviceName\":\"mcg-ers-co2-01\",\"devEUI\":\"xxxxxxxxxxxxxxx\",\"data\":\"AQDlAiUEAO8FAAYBwQcOBQ==\",\"object\":{\"co2\":449,\"humidity\":37,\"light\":239,\"motion\":0,\"temperature\":22.9,\"vdd\":3589}}";
            decoder.Decode("2020-10-07T15:46:45Z", "iothub", "out", Encoding.UTF8.GetBytes(payload));

            contextBroker.Verify(foo => foo.PostMessage(It.Is<DeviceMessage>(mo => mo.Value.Value == "co2%3D449")));
        }
    }    


}
