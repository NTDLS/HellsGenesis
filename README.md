# Strikeforce Infinite

Strikeforce Infinite is a 2D space fighter game written in C# (currently .net8) and rendered with DirectX.
The game features infinite space, neural network AI, multiple scenarios, single-player, multi-player, locking/seeking weapons, a "speed-scaled" environment, dozens of weapons and massive fiery explosions...

Check out the alpha build 13 gameplay on YouTube: [https://www.youtube.com/watch?v=Kd-aEshWiNg](https://youtu.be/uxRjQ9BHlek)

**GDI Notes**
The rendering engine was originally written to use GDI+. Before moving to DirectX, the framerate was still 80+FPS, but the resource contention required for measuring strings was causing all kinds of odd race conditions. That said, the GDI+ code contains a lot of "lessons" learned" code so we are providing it here for educational purposes. The last DGI+ (no-DirectX) commit was https://github.com/NTDLS/HellsGenesis/tree/6f593ea1daf21054382b54e98f9d41ab3303ba8a.

