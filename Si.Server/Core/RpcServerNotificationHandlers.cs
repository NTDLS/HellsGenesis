using NTDLS.ReliableMessaging;
using Si.Library.Messages.Notify;
using Si.Library.Messages.Query;

namespace Si.Server.Core
{
    internal class RpcServerNotificationHandlers : IRmMessageHandler
    {
        private readonly ServerEngineCore _serverCore;

        public RpcServerNotificationHandlers(ServerEngineCore serverCore)
        {
            _serverCore = serverCore;
        }

        public void OnSiHostIsStartingGame(RmContext context, SiHostIsStartingGame param)
        {
            if (!_serverCore.Sessions.TryGetByConnectionId(context.ConnectionId, out var session))
            {
                _serverCore.Log.Exception($"The session was not found '{context.ConnectionId}'.");
                return;
            }

            _serverCore.Log.Verbose($"ConnectionId: '{context.ConnectionId}' is starting game for lobby: '{param.LobbyUID}'.");

            if (!_serverCore.Lobbies.TryGetByLobbyUID(param.LobbyUID, out var lobby))
            {
                _serverCore.Log.Exception($"The lobby was not found '{param.LobbyUID}'.");
                return;
            }

            //Let all the non-lobby-owner connections know that the host is starting the game.
            _serverCore.BroadcastNotificationToAll(lobby, context.ConnectionId, param);
        }

        public void OnSiDeleteLobby(RmContext context, SiDeleteLobby param)
        {
            if (!_serverCore.Sessions.TryGetByConnectionId(context.ConnectionId, out var session))
            {
                _serverCore.Log.Exception($"The session was not found '{context.ConnectionId}'.");
                return;
            }

            _serverCore.Log.Verbose($"ConnectionId: '{context.ConnectionId}' is deleting the lobby: '{param.LobbyUID}'");

            if (!_serverCore.Lobbies.TryGetByLobbyUID(param.LobbyUID, out var lobby))
            {
                _serverCore.Log.Exception($"The lobby was not found '{param.LobbyUID}'.");
                return;
            }

            _serverCore.BroadcastNotificationToAll(lobby, context.ConnectionId, new SiLobbyDeleted(param.LobbyUID));

            _serverCore.Lobbies.Delete(param.LobbyUID);
        }

        public void OnSiDeregisterToLobby(RmContext context, SiDeregisterToLobby param)
        {
            if (!_serverCore.Sessions.TryGetByConnectionId(context.ConnectionId, out var session))
            {
                _serverCore.Log.Exception($"The session was not found '{context.ConnectionId}'.");
                return;
            }

            _serverCore.Log.Verbose($"ConnectionId: '{context.ConnectionId}' deregistered for lobby: '{param.LobbyUID}'");

            if (!_serverCore.Lobbies.TryGetByLobbyUID(param.LobbyUID, out var lobby))
            {
                _serverCore.Log.Exception($"The lobby was not found '{param.LobbyUID}'.");
                return;
            }

            lobby.Deregister(context.ConnectionId);
            session.ClearCurrentLobby();
        }

        public void OnSiRegisterToLobby(RmContext context, SiRegisterToLobby param)
        {
            if (!_serverCore.Sessions.TryGetByConnectionId(context.ConnectionId, out var session))
            {
                _serverCore.Log.Exception($"The session was not found '{context.ConnectionId}'.");
                return;
            }

            _serverCore.Log.Verbose($"ConnectionId: '{context.ConnectionId}' registered for lobby: '{param.LobbyUID}'");

            if (!_serverCore.Lobbies.TryGetByLobbyUID(param.LobbyUID, out var lobby))
            {
                _serverCore.Log.Exception($"The lobby was not found '{param.LobbyUID}'.");
                return;
            }

            lobby.Register(context.ConnectionId, param.PlayerName);
            session.SetCurrentLobby(param.LobbyUID);
        }

        public void OnSiWaitingInLobby(RmContext context, SiWaitingInLobby param)
        {
            if (!_serverCore.Sessions.TryGetByConnectionId(context.ConnectionId, out var session))
            {
                _serverCore.Log.Exception($"The session was not found '{context.ConnectionId}'.");
                return;
            }

            _serverCore.Log.Verbose($"ConnectionId: '{context.ConnectionId}' is waiting in the lobby.");

            if (!_serverCore.Lobbies.TryGetByLobbyUID(session.LobbyUID, out var lobby))
            {
                _serverCore.Log.Exception($"The lobby was not found '{session.LobbyUID}'.");
                return;
            }

            lobby.SetConnectionAsWaitingInLobby(context.ConnectionId);
        }

        public void OnSiLeftLobby(RmContext context, SiLeftLobby param)
        {
            if (!_serverCore.Sessions.TryGetByConnectionId(context.ConnectionId, out var session))
            {
                _serverCore.Log.Exception($"The session was not found '{context.ConnectionId}'.");
                return;
            }

            _serverCore.Log.Verbose($"ConnectionId: '{context.ConnectionId}' has left the lobby.");

            if (!_serverCore.Lobbies.TryGetByLobbyUID(session.LobbyUID, out var lobby))
            {
                _serverCore.Log.Exception($"The lobby was not found '{session.LobbyUID}'.");
                return;
            }

            lobby.SetConnectionAsLeftInLobby(context.ConnectionId);
        }

        public void OnSiSituationLayout(RmContext context, SiSituationLayout param)
        {
            if (!_serverCore.Sessions.TryGetByConnectionId(context.ConnectionId, out var session))
            {
                _serverCore.Log.Exception($"The session was not found '{context.ConnectionId}'.");
                return;
            }

            _serverCore.Log.Verbose($"ConnectionId: '{context.ConnectionId}' issued a new layout.");

            if (!_serverCore.Lobbies.TryGetByLobbyUID(session.LobbyUID, out var lobby))
            {
                _serverCore.Log.Exception($"The lobby was not found '{session.LobbyUID}'.");
                return;
            }

            //The owner of the lobby sent a new layout, send it to all of the connections except for the one that sent it to us.
            _serverCore.BroadcastNotificationToAll(lobby, context.ConnectionId, param);
        }

        public void OnSiPlayerSpriteCreated(RmContext context, SiPlayerSpriteCreated param)
        {
            if (!_serverCore.Sessions.TryGetByConnectionId(context.ConnectionId, out var session))
            {
                _serverCore.Log.Exception($"The session was not found '{context.ConnectionId}'.");
                return;
            }

            _serverCore.Log.Verbose($"ConnectionId: '{context.ConnectionId}' has created its sprite.");

            if (!_serverCore.Lobbies.TryGetByLobbyUID(session.LobbyUID, out var lobby))
            {
                _serverCore.Log.Exception($"The lobby was not found '{session.LobbyUID}'.");
                return;
            }

            _serverCore.BroadcastNotificationToAll(lobby, context.ConnectionId, param);
        }

        public void OnSiHostStartedLevel(RmContext context, SiHostStartedLevel param)
        {
            if (!_serverCore.Sessions.TryGetByConnectionId(context.ConnectionId, out var session))
            {
                _serverCore.Log.Exception($"The session was not found '{context.ConnectionId}'.");
                return;
            }

            _serverCore.Log.Verbose($"ConnectionId: '{context.ConnectionId}' is started level.");

            if (!_serverCore.Lobbies.TryGetByLobbyUID(session.LobbyUID, out var lobby))
            {
                _serverCore.Log.Exception($"The lobby was not found '{session.LobbyUID}'.");
                return;
            }

            _serverCore.BroadcastNotificationToAll(lobby, context.ConnectionId, param);
        }

        public void OnSiSpriteCreated(RmContext context, SiSpriteCreated param)
        {
            if (!_serverCore.Sessions.TryGetByConnectionId(context.ConnectionId, out var session))
            {
                _serverCore.Log.Exception($"The session was not found '{context.ConnectionId}'.");
                return;
            }

            _serverCore.Log.Verbose($"ConnectionId: '{context.ConnectionId}' created a sprite of type {param.Layout?.FullTypeName ?? string.Empty}.");

            if (!_serverCore.Lobbies.TryGetByLobbyUID(session.LobbyUID, out var lobby))
            {
                _serverCore.Log.Exception($"The lobby was not found '{session.LobbyUID}'.");
                return;
            }

            _serverCore.BroadcastNotificationToAll(lobby, context.ConnectionId, param);
        }
    }
}
