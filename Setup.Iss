[Setup]
;-- Main Setup Information
 AppName                         = Nebula Siege
 AppVerName                      = Nebula Siege 1.0.0.0
 AppCopyright                    = Copyright © 1995-2023 NetworkDLS.
 DefaultDirName                  = {commonpf}\NetworkDLS\Nebula Siege
 DefaultGroupName                = NetworkDLS\Nebula Siege
 UninstallDisplayIcon            = {app}\NebulaSiege.exe
 SetupIconFile                   = HG.ico
 PrivilegesRequired              = admin
 Uninstallable                   = Yes
 Compression                     = bZIP/9
 ChangesAssociations             = Yes
 OutputBaseFilename              = Nebula Siege
 ArchitecturesInstallIn64BitMode = x64
 ArchitecturesAllowed            = x64

;-- Windows 2000 & XP (Support Dialog)
 AppPublisher    = NetworkDLS
 AppPublisherURL = http://www.NetworkDLS.com/
 AppUpdatesURL   = http://www.NetworkDLS.com/
 AppVersion      = 1.0.0.0
 
[Components]
 Name: "Game";         Description: "Nebula Siege";     Types: Full Compact Custom; Flags: Fixed;
 Name: "Assets";       Description: "Assets";           Types: Full Compact Custom; Flags: Fixed;
 Name: "FontAsset";    Description: "Fonts";            Types: Full Compact Custom; Flags: Fixed;
 
[Files]
 Source: "NebulaSiege\bin\Release\net7.0-windows\*.exe";   DestDir: "{app}";         Components: Game;   Flags: IgnoreVersion; 
 Source: "NebulaSiege\bin\Release\net7.0-windows\*.dll";   DestDir: "{app}";         Components: Game;   Flags: IgnoreVersion;
 Source: "NebulaSiege\bin\Release\net7.0-windows\*.json";  DestDir: "{app}";         Components: Game;   Flags: IgnoreVersion;
 Source: "NebulaSiege\Assets\*.*";                         DestDir: "{app}\Assets";  Components: Assets; Flags: IgnoreVersion recursesubdirs;

 Source: "Fonts\Digital-7 (Mono).ttf"; DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Digital-7 Mono"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Digital-7.ttf";        DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "digital-7"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Orbitronio.ttf";       DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Orbitronio"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Roboto-Regular.ttf";   DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Roboto"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Roboto-Thin.ttf";      DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Roboto Th"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\visitor1.ttf";         DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Visitor TT1 BRK"; Flags: onlyifdoesntexist uninsneveruninstall

[Registry]
 Root: HKLM; Subkey: "Software\NetworkDLS\Nebula Siege"; ValueType: STRING;ValueName: "CommonAssetPath"; ValueData:"{app}\Assets";              Flags: CreateValueIfDoesntExist;
 Root: HKLM; Subkey: "Software\NetworkDLS\Nebula Siege"; ValueType: STRING;ValueName: "UserAssetPath";   ValueData:"{userappdata}\NebulaSiege"; Flags: CreateValueIfDoesntExist;

[Icons]
 Name: "{group}\NebulaSiege"; Filename: "{app}\NebulaSiege.exe"; Components: Game;
 
[Run]
 Filename: "{app}\HG.exe"; Description: "Run Nebula Siege now?"; Flags: postinstall nowait skipifsilent shellexec; Components: Game;
