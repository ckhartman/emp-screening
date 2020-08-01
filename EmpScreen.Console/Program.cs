using System;
using eClearSdk;
using System.Threading.Tasks;

namespace EmpScreen.Console
{
    class Program
    {
        private static IeClearSdk _eClearBusSdk;
        static async Task Main(string[] args)
        {
            await AsyncMain();
        }
        static async Task AsyncMain()
        {
            System.Console.WriteLine("GetReadySdk - Start");
            const string connectionString = "amqps://coh-dev:c1ae304117c9182723d24e48b94fc7475472@ec-cloud-prod-01.ec.docker:5671/coh-dev";
            _eClearBusSdk = await eClearSdkFactory.GetReadySdk(connectionString, true);
            System.Console.WriteLine("GetReadySdk - Done");
            _eClearBusSdk.Subscribe.FaceDetectedCallback((item =>
            {
                // do something with the callback (face detection)

                System.Console.WriteLine($"Device: {item.Device.DeviceName}, Time: {item.DateTimeUtc}, Temp: {item.Attributes.PersonTemperature}");
            }));
            //}, "MyUniqueQueueName", true, true)); if you want a durable queue that will survive when you are disconnected, and not losing any data.
            // indicate all callbacks are setup, so the bus routing keys can be bound
            _eClearBusSdk.ServerReady();
            System.Console.WriteLine($"Press key to exit");
            System.Console.ReadKey();
            // Cleanup - when you are shutting down, this will clear the connection resources
            _eClearBusSdk.Dispose();
        }
    }
}
    
