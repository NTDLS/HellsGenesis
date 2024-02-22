#define Version "0.0.18"

[Setup]
;-- Main Setup Information
 AppName                         = Strikeforce Infinity
 AppVerName                      = Strikeforce Infinity {#Version}
 AppCopyright                    = Copyright © 1995-2024 NetworkDLS.
 DefaultDirName                  = {commonpf}\NetworkDLS\Strikeforce Infinity
 DefaultGroupName                = NetworkDLS\Strikeforce Infinity
 UninstallDisplayIcon            = {app}\StrikeforceInfinity.exe
 SetupIconFile                   = ../@Resources\StrikeforceInfinity.ico
 PrivilegesRequired              = admin
 Uninstallable                   = Yes
 Compression                     = ZIP/9
 ChangesAssociations             = Yes
 OutputBaseFilename              = Si-{#Version}
 ArchitecturesInstallIn64BitMode = x64
 ArchitecturesAllowed            = x64

;-- Windows 2000 & XP (Support Dialog)
 AppPublisher    = NetworkDLS
 AppPublisherURL = http://www.NetworkDLS.com/
 AppUpdatesURL   = http://www.NetworkDLS.com/
 AppVersion      = {#Version}
 
[Components]
 Name: "Game";         Description: "Strikeforce Infinity"; Types: Full Compact Custom; Flags: Fixed;
 Name: "Assets";       Description: "Assets";               Types: Full Compact Custom; Flags: Fixed;
 Name: "FontAsset";    Description: "Fonts";                Types: Full Compact Custom; Flags: Fixed;
 
[Files]
 Source: "../Si.Game\bin\Release\net8.0-windows7.0\*.exe";   DestDir: "{app}"; Components: Game;   Flags: IgnoreVersion; 
 Source: "../Si.Game\bin\Release\net8.0-windows7.0\*.dll";   DestDir: "{app}"; Components: Game;   Flags: IgnoreVersion;
 Source: "../Si.Game\bin\Release\net8.0-windows7.0\*.json";  DestDir: "{app}"; Components: Game;   Flags: IgnoreVersion;
 Source: "Si.Assets.rez";                                    DestDir: "{app}"; Components: Assets; Flags: IgnoreVersion recursesubdirs;

 Source: "Fonts\Digital-7 (Mono).ttf"; DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Digital-7 Mono";  Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Digital-7.ttf";        DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "digital-7";       Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Orbitronio.ttf";       DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Orbitronio";      Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Roboto-Regular.ttf";   DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Roboto";          Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Roboto-Thin.ttf";      DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Roboto Th";       Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\visitor1.ttf";         DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Visitor TT1 BRK"; Flags: onlyifdoesntexist uninsneveruninstall

[Registry]
 Root: HKLM; Subkey: "Software\NetworkDLS\Strikeforce Infinity"; ValueType: STRING;ValueName: "CommonAssetPath"; ValueData:"{app}\Assets";                      Flags: CreateValueIfDoesntExist;
 Root: HKLM; Subkey: "Software\NetworkDLS\Strikeforce Infinity"; ValueType: STRING;ValueName: "UserAssetPath";   ValueData:"{userappdata}\StrikeforceInfinity"; Flags: CreateValueIfDoesntExist;

[Icons]
 Name: "{group}\Strikeforce Infinity"; Filename: "{app}\Si.Game.exe"; Components: Game;
 
[Run]
 Filename: "{app}\Si.Game.exe"; Description: "Run Strikeforce Infinity now?"; Flags: postinstall nowait skipifsilent shellexec; Components: Game;
