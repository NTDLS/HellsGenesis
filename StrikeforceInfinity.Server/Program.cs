using Microsoft.Extensions.Configuration;
using StrikeforceInfinity.Server.Engine;
using StrikeforceInfinity.Shared;
using Topshelf;

namespace StrikeforceInfinity.Server
{
    public class Program
    {
        private static ServerCore? _core = null;
        public static ServerCore Core
        {
            get
            {
                _core ??= new ServerCore(Configuration);
                return _core;
            }
        }

        private static StrikeforceInfinitySettings? _settings = null;
        public static StrikeforceInfinitySettings Configuration
        {
            get
            {
                if (_settings == null)
                {
                    IConfiguration config = new ConfigurationBuilder()
                                 .AddJsonFile("appsettings.json")
                                 .Build();

                    _settings = config.GetRequiredSection("Settings").Get<StrikeforceInfinitySettings>()
                        ?? throw new Exception("Failed to load settings");
                }

                return _settings;
            }
        }

        public class NsService
        {
            private readonly SemaphoreSlim _semaphoreToRequestStop;
            private readonly Thread _thread;

            public NsService()
            {
                _semaphoreToRequestStop = new SemaphoreSlim(0);
                _thread = new Thread(DoWork);
            }

            public void Start()
            {
                _thread.Start();
            }

            public void Stop()
            {
                _semaphoreToRequestStop.Release();
                _thread.Join();
            }

            private void DoWork()
            {
                Core.Start();

                Core.Log.Write($"Listening on {Configuration.DataPort}.");

                while (true)
                {
                    if (_semaphoreToRequestStop.Wait(500))
                    {
                        Core.Log.Write($"Stopping...");
                        Core.Stop();
                        break;
                    }
                }
            }
        }

        public static void Main()
        {
            HostFactory.Run(x =>
            {
                x.StartAutomatically(); // Start the service automatically

                x.EnableServiceRecovery(rc =>
                {
                    rc.RestartService(1); // restart the service after 1 minute
                });

                x.Service<NsService>(s =>
                {
                    s.ConstructUsing(hostSettings => new NsService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("StrikeforceInfinity game server service.");
                x.SetDisplayName("StrikeforceInfinity Service");
                x.SetServiceName("StrikeforceInfinity");
            });
        }
    }
}
