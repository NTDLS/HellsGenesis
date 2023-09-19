# Nebula Siege
Hells Genesis is a 2D space fighter (Hell Fighter Tribute) game engine written in C# (currently .net7) and DirectX.

Infinite space, neural network AI, multiple scenario, event driven, locking/seeking weapons, etc.

For the first first 3 years, this was rendering with GDI+ which was for the challenge but I caved and moved to DirectX. I was still getting about 80FPS, but the locking required to do basic thigs like "measure strings" with a decive context was causing all kinds of odd race conditions. So anyway, if you are here to see the GDI+, the last non-DirectX commit was https://github.com/NTDLS/HellsGenesis/tree/6f593ea1daf21054382b54e98f9d41ab3303ba8a.

Version 0.0.1.0 Gameplay: (I've made progress since this video)
https://www.youtube.com/watch?v=Kd-aEshWiNg

![image](https://user-images.githubusercontent.com/11428567/118584633-604a6400-b765-11eb-8d98-222c02e796d3.png)
![image](https://user-images.githubusercontent.com/11428567/118584646-680a0880-b765-11eb-822c-6fe38f498ebc.png)
![image](https://user-images.githubusercontent.com/11428567/118584654-6c362600-b765-11eb-9939-c2b131e7fe4d.png)
