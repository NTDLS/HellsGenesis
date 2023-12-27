# Module Descriptions

## Si.Game
This is the "client" application which serves up the visuals and audio rendered by the Si.GameEngine

## Si.GameEngine
The game engine. Rendering, world clock, AI, etc.

## Si.Server
The server is the application that the game clients (Si.Game) connect to for multiplayer play.
It is responsible for receiving configuration, receiving and distributing all sprite updates to
clients. It also houses the information on which lobbies are available.

## Si.Multiplay
Contains the multiplayer client. This is what the game uses to communicate with the Si.Server.
It is split out from the game so that Si.IndependentLobbyServer can use it to host lobbies without
the overhead of a DirectX game.

## Si.IndependentLobbyHost
Uses the Si.Multiplay module to allow for hosting lobbies for human players independent from the Si.Game.

## Si.IndependentLobbyHostWithAI
Uses the Si.Multiplay and Si.GameEngine modules to allow for hosting lobbies a situation server which includes human and AI players.

## Si.Shared
Contains various shared objects that are used between all the other modules.
