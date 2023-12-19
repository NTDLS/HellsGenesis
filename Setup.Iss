[Setup]
;-- Main Setup Information
 AppName                         = Strikeforce Infinity
 AppVerName                      = Strikeforce Infinity 1.0.0.0
 AppCopyright                    = Copyright © 1995-2023 NetworkDLS.
 DefaultDirName                  = {commonpf}\NetworkDLS\Strikeforce Infinity
 DefaultGroupName                = NetworkDLS\Strikeforce Infinity
 UninstallDisplayIcon            = {app}\StrikeforceInfinity.exe
 SetupIconFile                   = StrikeforceInfinity\Resources\StrikeforceInfinity.ico
 PrivilegesRequired              = admin
 Uninstallable                   = Yes
 Compression                     = bZIP/9
 ChangesAssociations             = Yes
 OutputBaseFilename              = Strikeforce Infinity
 ArchitecturesInstallIn64BitMode = x64
 ArchitecturesAllowed            = x64

;-- Windows 2000 & XP (Support Dialog)
 AppPublisher    = NetworkDLS
 AppPublisherURL = http://www.NetworkDLS.com/
 AppUpdatesURL   = http://www.NetworkDLS.com/
 AppVersion      = 1.0.0.0
 
[Components]
 Name: "Game";         Description: "Strikeforce Infinity"; Types: Full Compact Custom; Flags: Fixed;
 Name: "Assets";       Description: "Assets";               Types: Full Compact Custom; Flags: Fixed;
 Name: "FontAsset";    Description: "Fonts";                Types: Full Compact Custom; Flags: Fixed;
 
[Files]
 Source: "StrikeforceInfinity\bin\Release\net7.0-windows\*.exe";   DestDir: "{app}";         Components: Game;   Flags: IgnoreVersion; 
 Source: "StrikeforceInfinity\bin\Release\net7.0-windows\*.dll";   DestDir: "{app}";         Components: Game;   Flags: IgnoreVersion;
 Source: "StrikeforceInfinity\bin\Release\net7.0-windows\*.json";  DestDir: "{app}";         Components: Game;   Flags: IgnoreVersion;
 Source: "StrikeforceInfinity\Assets\*.*";                         DestDir: "{app}\Assets";  Components: Assets; Flags: IgnoreVersion recursesubdirs;

 Source: "Fonts\Digital-7 (Mono).ttf"; DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Digital-7 Mono"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Digital-7.ttf";        DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "digital-7"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Orbitronio.ttf";       DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Orbitronio"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Roboto-Regular.ttf";   DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Roboto"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Roboto-Thin.ttf";      DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Roboto Th"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\visitor1.ttf";         DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Visitor TT1 BRK"; Flags: onlyifdoesntexist uninsneveruninstall

[Registry]
 Root: HKLM; Subkey: "Software\NetworkDLS\Strikeforce Infinity"; ValueType: STRING;ValueName: "CommonAssetPath"; ValueData:"{app}\Assets";              Flags: CreateValueIfDoesntExist;
 Root: HKLM; Subkey: "Software\NetworkDLS\Strikeforce Infinity"; ValueType: STRING;ValueName: "UserAssetPath";   ValueData:"{userappdata}\StrikeforceInfinity"; Flags: CreateValueIfDoesntExist;

[Icons]
 Name: "{group}\Strikeforce Infinity"; Filename: "{app}\StrikeforceInfinity.exe"; Components: Game;
 
[Run]
 Filename: "{app}\StrikeforceInfinity.exe"; Description: "Run Strikeforce Infinity now?"; Flags: postinstall nowait skipifsilent shellexec; Components: Game;
