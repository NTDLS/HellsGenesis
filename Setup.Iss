[Setup]
;-- Main Setup Information
 AppName                         = NebulaSiege
 AppVerName                      = NebulaSiege 1.0.0.0
 AppCopyright                    = Copyright © 1995-2023 NetworkDLS.
 DefaultDirName                  = {commonpf}\NetworkDLS\NebulaSiege
 DefaultGroupName                = NetworkDLS\NebulaSiege
 UninstallDisplayIcon            = {app}\HG.exe
 SetupIconFile                   = HG.ico
 PrivilegesRequired              = admin
 Uninstallable                   = Yes
 Compression                     = bZIP/9
 ChangesAssociations             = Yes
 OutputBaseFilename              = NebulaSiege
 ArchitecturesInstallIn64BitMode = x64
 ArchitecturesAllowed            = x64

;-- Windows 2000 & XP (Support Dialog)
 AppPublisher    = NetworkDLS
 AppPublisherURL = http://www.NetworkDLS.com/
 AppUpdatesURL   = http://www.NetworkDLS.com/
 AppVersion      = 1.0.0.2
 
[Components]
 Name: "Game";         Description: "NebulaSiege";             Types: Full Compact Custom; Flags: Fixed;
 Name: "Assets";       Description: "Assets";           Types: Full Compact Custom; Flags: Fixed;
 Name: "ScenarioEdit"; Description: "Scenario Editor";  Types: Full Custom;

[Files]
 Source: "NebulaSiege\bin\Release\net7.0-windows\*.exe";          DestDir: "{app}";                          Components: Game;          Flags: IgnoreVersion; 
 Source: "NebulaSiege\bin\Release\net7.0-windows\*.dll";          DestDir: "{app}";                          Components: Game;          Flags: IgnoreVersion;
 Source: "NebulaSiege\bin\Release\net7.0-windows\*.json";         DestDir: "{app}";                          Components: Game;          Flags: IgnoreVersion;
 Source: "NebulaSiege\Assets\*.*";                              DestDir: "{app}\Assets";                   Components: Assets;        Flags: IgnoreVersion recursesubdirs;
 
 Source: "Fonts\Digital-7 (Mono).ttf"; DestDir: "{fonts}"; FontInstall: "Digital-7 Mono"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Digital-7.ttf"; DestDir: "{fonts}"; FontInstall: "digital-7"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Orbitronio.ttf"; DestDir: "{fonts}"; FontInstall: "Orbitronio"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Roboto-Regular.ttf"; DestDir: "{fonts}"; FontInstall: "Roboto"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Roboto-Thin.ttf"; DestDir: "{fonts}"; FontInstall: "Roboto Th"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\visitor1.ttf"; DestDir: "{fonts}"; FontInstall: "Visitor TT1 BRK"; Flags: onlyifdoesntexist uninsneveruninstall


[Registry]
 Root: HKLM; Subkey: "Software\NetworkDLS\Rogue Quest"; ValueType: STRING;ValueName: "CommonAssetPath"; ValueData:"{app}\Assets";               Flags: CreateValueIfDoesntExist;
 Root: HKLM; Subkey: "Software\NetworkDLS\Rogue Quest"; ValueType: STRING;ValueName: "UserAssetPath";   ValueData:"{userappdata}\NebulaSiege"; Flags: CreateValueIfDoesntExist;

[Icons]
 Name: "{group}\NebulaSiege"; Filename: "{app}\HG.exe"; Components: Game;
 
[Run]
 Filename: "{app}\HG.exe"; Description: "Run NebulaSiege now?"; Flags: postinstall nowait skipifsilent shellexec; Components: Game;
