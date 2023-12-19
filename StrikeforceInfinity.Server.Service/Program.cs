using StrikeforceInfinity.Server.Engine;
using StrikeforceInfinity.Shared;
using Topshelf;

namespace StrikeforceInfinity.Server.Service
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
                                 .AddEnvironmentVariables()
                                 .Build();


                    // Get values from the config given their key and their target type.
                    _settings = config.GetRequiredSection("Settings").Get<StrikeforceInfinitySettings>();
                    if (_settings == null)
                    {
                        throw new Exception("Failed to load settings");
                    }
                }

                return _settings;
            }
        }

        public class NsService
        {
            private SemaphoreSlim _semaphoreToRequestStop;
            private Thread _thread;

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

                // Add services to the container.
                var builder = WebApplication.CreateBuilder();
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddControllers(options =>
                {
                    options.InputFormatters.Add(new TextPlainInputFormatter());
                });

                builder.Logging.ClearProviders();
                builder.Logging.SetMinimumLevel(LogLevel.Warning);

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseAuthorization();
                app.MapControllers();
                app.RunAsync(Configuration.BaseManagementAddress);

                if (app.Environment.IsDevelopment())
                {
                    //System.Diagnostics.Process.Start("explorer", $"{Configuration.BaseManagementAddress}swagger/index.html");
                }

                Core.Log.Write($"Listening on {Configuration.BaseManagementAddress}.");

                while (true)
                {
                    if (_semaphoreToRequestStop.Wait(500))
                    {
                        Core.Log.Write($"Stopping...");
                        app.StopAsync();
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
