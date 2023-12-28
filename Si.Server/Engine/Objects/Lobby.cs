using NTDLS.Semaphore;
using Si.Shared;
using Si.Shared.Payload;
using System.Timers;

namespace Si.Server.Engine.Objects
{
    internal class Lobby
    {
        private readonly ServerCore _serverCore;
        private readonly PessimisticSemaphore<Dictionary<Guid, LobbyConnection>> _connections = new();
        private readonly System.Timers.Timer _pingTimer = new(1000);
        private readonly System.Timers.Timer _lobbyStartTimer = new(1000);
        private DateTime? _countdownStartTime;

        /// <summary>
        /// The date and time that the lobby was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The sessionid of the connection that created the lobby.
        /// </summary>
        public Guid OwnerConnectionId { get; set; }

        /// <summary>
        /// The unique id of the  lobby. This is server generated.
        /// </summary>
        public Guid UID { get; set; }

        /// <summary>
        /// User suppled name of the lobby.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The minimum number of players needed to start the game.
        /// </summary>
        public int MinPlayers { get; set; }

        /// <summary>
        /// The maximum number of players that can register for the lobby.
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// The maximum amount of time to wait after reaching the MinPlayers that the server will wait before auto-starting the game.
        /// </summary>
        public int AutoStartSeconds { get; set; }

        /// <summary>
        /// Whether the server is hosted by a person or an independent lobby host.
        /// </summary>
        public bool IsHeadless { get; set; }

        public Lobby(ServerCore serverCore, Guid ownerConnectionId, SiLobbyConfiguration configuration)
        {
            _serverCore = serverCore;
            UID = Guid.NewGuid();
            OwnerConnectionId = ownerConnectionId;
            Name = configuration.Name;
            MinPlayers = configuration.MinPlayers;
            MaxPlayers = configuration.MaxPlayers;
            AutoStartSeconds = configuration.AutoStartSeconds;
            IsHeadless = configuration.IsHeadless;

            _pingTimer.Elapsed += PingTimer_Elapsed;
            _pingTimer.Start();

            _lobbyStartTimer.Elapsed += LobbyStartTimer_Elapsed;
            _lobbyStartTimer.Start();
        }

        ~Lobby()
        {
            SiUtility.TryAndIgnore(_pingTimer.Stop);
            SiUtility.TryAndIgnore(_pingTimer.Dispose);
        }

        public SiLobbyInfo GetLobbyInfo()
        {
            var lobbyInfo = new SiLobbyInfo()
            {
                Name = Name,
                WaitingCount = 0,
            };

            foreach (var connection in GetConenctions())
            {
                lobbyInfo.Connections.Add(new SiLobbyConnection
                {
                    Name = "The <player name> is not implemented.",
                    IsWaitingInLobby = connection.IsWaitingInLobby,
                    LatencyMs = connection.LatencyMs,
                });
            }

            return lobbyInfo;
        }

        bool _isPingTimerExecuting = false;
        private void PingTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (_isPingTimerExecuting == false)
            {
                _isPingTimerExecuting = true;
                try
                {
                    var connectionIds = GetConnectionIDs();

                    foreach (var connectionId in connectionIds)
                    {
                        try
                        {
                            //Ping from the server to the client and record the result.
                            _serverCore.PingConnection(connectionId).ContinueWith(x =>
                            {
                                if (x.Result != null)
                                {
                                    var LatencyMs = (DateTime.UtcNow - x.Result.Timestamp).TotalMilliseconds;

                                    _connections.Use(o =>
                                    {
                                        if (o.TryGetValue(connectionId, out var connection))
                                        {
                                            connection.LatencyMs = LatencyMs;
                                        }
                                    });
                                }
                            });
                        }
                        catch { };
                    }
                }
                catch { }
                finally
                {
                    _isPingTimerExecuting = false;
                }
            }
        }

        bool _isLobbyStartTimerExecuting = false;
        private void LobbyStartTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (_isLobbyStartTimerExecuting == false)
            {
                _isLobbyStartTimerExecuting = true;
                try
                {
                    //if(RemainingSecondsUntilAutoStart <= 0)
                    {
                    }
                }
                catch { }
                finally
                {
                    _isLobbyStartTimerExecuting = false;
                }
            }
        }

        public int? RemainingSecondsUntilAutoStart
        {
            get
            {
                try
                {
                    if (_countdownStartTime != null)
                    {
                        var elapsedSeconds = (DateTime.UtcNow - (DateTime)_countdownStartTime).TotalSeconds;
                        return (int)(AutoStartSeconds - elapsedSeconds);
                    }
                }
                catch { }
                return null;
            }
        }

        /// <summary>
        /// Registers a connection for this lobby.
        /// </summary>
        /// <param name="connectionId"></param>
        public void Register(Guid connectionId, string playerName)
        {
            _connections.Use(o =>
            {
                var state = new LobbyConnection()
                {
                    PlayerName = playerName,
                    ConnectionId = connectionId
                };

                o.Remove(connectionId);
                o.Add(connectionId, state);
                StartLobbyCountdownIfApplicable();
            });
        }

        /// <summary>
        /// Removes a connection from a lobby.
        /// </summary>
        /// <param name="connectionId"></param>
        public void Deregister(Guid connectionId)
        {
            _connections.Use(o =>
            {
                o.Remove(connectionId);
                StartLobbyCountdownIfApplicable();
            });
        }

        /// <summary>
        /// The lobby is being deleted. Cleanup any resources.
        /// </summary>
        public void Cleanup()
        {
            SiUtility.TryAndIgnore(_pingTimer.Stop);
            SiUtility.TryAndIgnore(_pingTimer.Dispose);

            SiUtility.TryAndIgnore(_lobbyStartTimer.Stop);
            SiUtility.TryAndIgnore(_lobbyStartTimer.Dispose);
        }

        /// <summary>
        /// Gets the list of all registered connections.
        /// </summary>
        /// <param name="connectionId"></param>
        public List<LobbyConnection> GetConenctions()
        {
            return _connections.Use(o =>
            {
                return o.Select(o => o.Value).ToList();
            });
        }

        /// <summary>
        /// Gets the list of all registered connection ids.
        /// </summary>
        /// <param name="connectionId"></param>
        public List<Guid> GetConnectionIDs()
        {
            return _connections.Use(o =>
            {
                return o.Select(o => o.Key).ToList();
            });
        }

        /// <summary>
        /// Tells the server that the connections is ready to start the game.
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns>Returns true if all registered connections are ready to start.</returns>
        public bool SetConnectionAsReadyToPlay(Guid connectionId)
        {
            return _connections.Use(o =>
            {
                if (o.TryGetValue(connectionId, out var state))
                {
                    state.IsReadyToPlay = true;
                }

                StartLobbyCountdownIfApplicable();

                return o.Values.All(o => o.IsReadyToPlay == true);
            });
        }

        /// <summary>
        /// Tells the server that the connections is waiting on the lobby.
        /// </summary>
        /// <param name="connectionId"></param>
        public void SetConnectionAsWaitingInLobby(Guid connectionId, Type selectedPlayerClass)
        {
            _connections.Use(o =>
            {
                if (o.TryGetValue(connectionId, out var state))
                {
                    state.IsWaitingInLobby = true;
                    state.SelectedPlayerClass = selectedPlayerClass;
                }
                StartLobbyCountdownIfApplicable();
            });
        }

        /// <summary>
        /// Tells the server that the connections has left the lobby - but is still registered.
        /// </summary>
        /// <param name="connectionId"></param>
        public void SetConnectionAsLeftInLobby(Guid connectionId)
        {
            _connections.Use(o =>
            {
                if (o.TryGetValue(connectionId, out var state))
                {
                    state.IsWaitingInLobby = false;
                }
                StartLobbyCountdownIfApplicable();
            });
        }

        private void StartLobbyCountdownIfApplicable()
        {
            _connections.Use(o =>
            {
                //Is the lobby owner ready (or headless)?
                if (IsHeadless || o.Values.Where(o => o.ConnectionId == OwnerConnectionId && o.IsWaitingInLobby == true).Any())
                {
                    //How many other players are ready?
                    int readyToPlayCount = o.Values.Where(o => o.IsWaitingInLobby == true).Count();

                    if (readyToPlayCount >= MinPlayers)
                    {
                        //We have enough players to start the game, so start the countdown.
                        _countdownStartTime = DateTime.UtcNow;
                        return;
                    }
                }

                _countdownStartTime = null;
            });
        }

        public int ConnectionsWaitingInLobbyCount()
        {
            return _connections.Use(o =>
            {
                return o.Values.Where(o => o.IsWaitingInLobby).Count();
            });
        }

        /*
        /// <summary>
        /// Returns true if all registered connections are ready to play.
        /// </summary>
        /// <param name="connectionId"></param>
        public bool AreAllConnectionsReadyToPlay()
        {
            return _connections.Use(o =>
            {
                return o.Values.All(o => o.IsReadyToPlay == true);
            });
        }

        public int ConnectionCount()
        {
            return _connections.Use(o =>
            {
                return o.Values.Count();
            });
        }

        public int ReadyConnectionCount()
        {
            return _connections.Use(o =>
            {
                return o.Values.Where(o => o.IsReadyToPlay).Count();
            });
        }
        */
    }
}
