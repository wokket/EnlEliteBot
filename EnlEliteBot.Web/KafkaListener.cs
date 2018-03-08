using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using EnlEliteBot.Web.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EnlEliteBot.Web
{
    public static class KafkaListener
    {
        internal static void Start()
        {


            ThreadPool.QueueUserWorkItem(RunAsync, null);

        }

        private static void RunAsync(object unused)
        {
            var config = new Dictionary<string, object>
            {
                { "group.id", Startup.Configuration["Kafka.ConsumerGroup"] ?? "EnlEliteBot" },
                { "bootstrap.servers", Startup.Configuration["Kafka.Endpoint"] },

                //SSL Settings, see https://github.com/ah-/rdkafka-dotnet/issues/60
              //  { "security.protocol", "SSL" },
              //  { "ssl.ca.location", @"C:\dev\EnlEliteBot\EnlEliteBot.Web\bin\Debug\netcoreapp2.0\server-cert.cer"},
                { "debug", "security,broker" }


            };


            Console.WriteLine($"Connecting to Kafka at {config["bootstrap.servers"]}...");
            using (var consumer = new Consumer<Ignore, string>(config, null, new StringDeserializer(Encoding.UTF8)))
            {
                // just start at the head of the parititon, there's no value to replaying messages here....
                consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset("commanders", 0, Offset.Beginning) });

                consumer.OnError += (_, error)
                    => Console.Error.WriteLine($"Kafka Error: {error}");

                // Raised on deserialization errors or when a consumed message has an error != NoError.
                consumer.OnConsumeError += (_, error)
                    => Console.Error.WriteLine($"Kafka consume error: {error}");

                Console.WriteLine($"    Connected.");
                while (true)
                {
                    Message<Ignore, string> msg;
                    if (consumer.Consume(out msg, TimeSpan.FromSeconds(1)))
                    {
                        Console.WriteLine($"Kafka Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");

                        //delegate to existing handler in the short term
                        EddnListener.HandleResult(null, msg.Value);
                    }
                }
            }
        }
    }
}
