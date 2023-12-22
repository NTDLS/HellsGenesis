using NTDLS.Semaphore;
using StrikeforceInfinity.Shared;
using System.Timers;

namespace StrikeforceInfinity.Server.Engine.Objects
{
    internal class Lobby
    {
        private readonly ServerCore _serverCore;
        private readonly PessimisticSemaphore<Dictionary<Guid, LobbyConnection>> _connections = new();
        private readonly System.Timers.Timer _timer = new(1000);

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
        /// The maximum number of players that can register for the lobby.
        /// </summary>
        public int MaxPlayers { get; set; }

        public Lobby(ServerCore serverCore, Guid ownerConnectionId, string name, int maxPlayers)
        {
            _serverCore = serverCore;
            UID = Guid.NewGuid();
            OwnerConnectionId = ownerConnectionId;
            Name = name;
            MaxPlayers = maxPlayers;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        ~Lobby()
        {
            SiUtility.TryAndIgnore(_timer.Stop);
            SiUtility.TryAndIgnore(_timer.Dispose);
        }

        bool _isTimerExecuting = false;
        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (_isTimerExecuting == false)
            {
                try
                {
                    _isTimerExecuting = true;

                    var connectionIds = Connections();

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
                    _isTimerExecuting = false;
                }
            }
        }

        /// <summary>
        /// Registers a connection for this lobby.
        /// </summary>
        /// <param name="connectionId"></param>
        public void Register(Guid connectionId)
        {
            _connections.Use(o =>
            {
                var state = new LobbyConnection()
                {
                    ConnectionId = connectionId
                };

                o.Remove(connectionId);
                o.Add(connectionId, state);
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
            });
        }

        /// <summary>
        /// The lobby is being deleted. Cleanup any resources.
        /// </summary>
        public void Cleanup()
        {
            SiUtility.TryAndIgnore(_timer.Stop);
            SiUtility.TryAndIgnore(_timer.Dispose);
        }

        /// <summary>
        /// Gets the list of all registered connection ids.
        /// </summary>
        /// <param name="connectionId"></param>
        public List<Guid> Connections()
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
        public bool FlagConnectionAsReadyToPlay(Guid connectionId)
        {
            return _connections.Use(o =>
            {
                if (o.TryGetValue(connectionId, out var state))
                {
                    state.IsReadyToPlay = true;
                }

                return o.Values.All(o => o.IsReadyToPlay == true);
            });
        }

        /// <summary>
        /// Tells the server that the connections is waiting on the lobby.
        /// </summary>
        /// <param name="connectionId"></param>
        public void FlagConnectionAsWaitingInLobby(Guid connectionId)
        {
            _connections.Use(o =>
            {
                if (o.TryGetValue(connectionId, out var state))
                {
                    state.IsWaitingInLobby = true;
                }
            });
        }

        /// <summary>
        /// Tells the server that the connections has left the lobby - but is still registered.
        /// </summary>
        /// <param name="connectionId"></param>
        public void FlagConnectionAsLeftInLobby(Guid connectionId)
        {
            _connections.Use(o =>
            {
                if (o.TryGetValue(connectionId, out var state))
                {
                    state.IsWaitingInLobby = false;
                }
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
