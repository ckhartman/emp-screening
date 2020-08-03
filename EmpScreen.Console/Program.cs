using eClearSdk;
using System.Threading.Tasks;
using System.Configuration;


namespace EmpScreen.Console
{
    class Program
    {
        private static IeClearSdk _eClearBusSdk;

        private static SalesForceClient CreateClient()
        {
            return new SalesForceClient
            {
                Username = ConfigurationManager.AppSettings["username"],
                Password = ConfigurationManager.AppSettings["password"],
                ClientId = ConfigurationManager.AppSettings["clientId"],
                ClientSecret = ConfigurationManager.AppSettings["clientSecret"],
                SecurityToken = ConfigurationManager.AppSettings["security_token"]
            };
        }

        static async Task Main(string[] args)
        {
            //var client = CreateClient();

            //if (args.Length > 0)
            //{
            //    client.Login();
            //    System.Console.WriteLine(client.Query(args[0]));
            //}
            //System.Console.ReadLine();

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
                var client = CreateClient();
                client.Login();
            var stringContent = "{ \"DeviceName__c\":" + item.Device.DeviceName + ", \"PersonTemperature__c\":" +  item.Attributes.PersonTemperature + " }";
                    client.Query(stringContent);
                    //System.Console.WriteLine(SalesForceClient.AuthToken);

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

