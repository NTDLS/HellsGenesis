# Strikeforce Infinity
As a Hell Fighter tribute, Strikeforce Infinity is a 2D space fighter game engine written in C# (currently .net8) and DirectX.

Infinite space, neural network AI, multiple scenario, event driven, locking/seeking weapons, etc.

For the first first 3 years, this was rendering with GDI+ but I caved and moved to DirectX. I was still getting about 80FPS, but the resource locking required to do basic thigs like "measure strings" with a device context was causing all kinds of odd race conditions. So anyway, if you are here to see the GDI+, the last non-DirectX commit was https://github.com/NTDLS/HellsGenesis/tree/6f593ea1daf21054382b54e98f9d41ab3303ba8a.

Version 0.0.1.0 Gameplay: (I've made progress since this video)
https://www.youtube.com/watch?v=Kd-aEshWiNg

**Welcome Screen**

![image](https://github.com/NTDLS/StrikeforceInfinity/assets/11428567/c57c3611-9f39-4b21-a1ec-60910d563280)

**Ship Class Selection**

![image](https://github.com/NTDLS/StrikeforceInfinity/assets/11428567/de754036-2e2a-48ff-9ef9-a4f8e7364d98)

**Early Gameplay**

![image](https://github.com/NTDLS/StrikeforceInfinity/assets/11428567/89468ceb-6645-41fb-856d-ad6b732b11bc)

