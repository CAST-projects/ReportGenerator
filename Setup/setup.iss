#define MyAppName "CAST Report Generator"
#define MyAppShortName "ReportGenerator"
#define MyAppVersion "1.6.0"
#define MyAppPublisher "CAST"
#define MyAppURL "http://www.castsoftware.com/"
#define MyAppExeName "CastReporting.UI.WPF.exe"
#define MyAppExe "../CastReporting.UI.WPF.V2/bin/Release/"+MyAppExeName
#define MyAppCopyright GetFileCopyright(MyAppExe)

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{B3C47653-5B85-4218-AFC5-EB9F2AAD341B}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppCopyright={#MyAppCopyright}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
VersionInfoCompany={#MyAppPublisher}
VersionInfoVersion={#MyAppVersion}
DefaultDirName={pf32}\CAST\{#MyAppShortName} {#MyAppVersion}
DefaultGroupName={#MyAppName} {#MyAppVersion}
OutputBaseFilename=ReportGeneratorSetup
OutputDir=../Setup
SetupIconFile=../CastReporting.UI.WPF.V2/Resources/Images/cast.ico
Compression=lzma
SolidCompression=yes
AlwaysShowDirOnReadyPage=true
;DirExistsWarning=No
UninstallDisplayIcon={app}\{#MyAppExeName}


[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl" ; LicenseFile: "../Setup/License.rtf"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]

; NOTE:packages/CommonServiceLocator.1.0/lib/NET35
Source: "../packages/CommonServiceLocator.1.0/lib/NET35/Microsoft.Practices.ServiceLocation.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "../packages/CommonServiceLocator.1.0/lib/NET35/Microsoft.Practices.ServiceLocation.XML"; DestDir: "{app}"; Flags: ignoreversion
; NOTE:packages/CommonServiceLocator.1.0/lib/SL30
Source: "../packages/CommonServiceLocator.1.0/lib/SL30/Microsoft.Practices.ServiceLocation.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "../packages/CommonServiceLocator.1.0/lib/SL30/Microsoft.Practices.ServiceLocation.XML"; DestDir: "{app}"; Flags: ignoreversion
; NOTE:packages/WpfAnimatedGif-1.4.4
Source: "../packages/WpfAnimatedGif-1.4.4/WpfAnimatedGif.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE:packages/
Source: "../packages/repositories.config"; DestDir: "{app}"; Flags: ignoreversion
; NOTE:packages\log4net.2.0.0\lib\net40-full
Source: "../packages/log4net.2.0.0/lib/net40-client/log4net.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE:CastReporting.UI.WPF.V2/Images
Source: "../CastReporting.UI.WPF.V2/Resources/Images/*"; DestDir: "{app}\Resources\Images"; Flags: ignoreversion
; NOTE: Value from CastReporting.UI.WPF\bin\Release
source: "../CastReporting.UI.WPF.V2/bin/Release/*.dll";DestDir: "{app}"; Flags: ignoreversion recursesubdirs
source: "../CastReporting.UI.WPF.V2/bin/Release/*.exe";DestDir: "{app}"; Flags: ignoreversion recursesubdirs
source: "../CastReporting.UI.WPF.V2/bin/Release/*.config";DestDir: "{app}"; Flags: ignoreversion recursesubdirs
source: "../CastReporting.Console/bin/Release/CastReporting.Console.exe";DestDir: "{app}"; Flags: ignoreversion
source: "../CastReporting.Console/bin/Release/CastReporting.Console.exe.config";DestDir: "{app}"; Flags: ignoreversion
source: "../CastReporting.Console/bin/Release/Parameters/*.xml";DestDir: "{app}"; Flags: ignoreversion
source: "../CastReporting.DAL/CastReportingSetting.xml"; DestDir: "{code:GetSettingsPath}"; Flags: ignoreversion
Source: "../CastReporting.Reporting\TemplatesFiles/*"; DestDir: "{code:GetTempPath}\Templates"; Flags: ignoreversion; AfterInstall:SaveSettings()
Source: "../CastReporting.Reporting\PortfolioTemplatesFiles/*"; DestDir: "{code:GetTempPath}\Templates\Portfolio"; Flags: ignoreversion; AfterInstall:SavePortfolioSettings()
; NOTE:CastReporting.Reporting/bin/Release
source: "../CastReporting.Reporting/bin/Release/Microsoft.Practices.Prism.dll";DestDir: "{app}"; Flags: ignoreversion
source: "../CastReporting.Reporting/bin/Release/Microsoft.Practices.Prism.Interactivity.dll";DestDir: "{app}"; Flags: ignoreversion
source: "../CastReporting.Reporting/bin/Release/Microsoft.Practices.Prism.Interactivity.xml";DestDir: "{app}"; Flags: ignoreversion
source: "../CastReporting.Reporting/bin/Release/Microsoft.Practices.Prism.MefExtensions.dll";DestDir: "{app}"; Flags: ignoreversion
source: "../CastReporting.Reporting/bin/Release/Microsoft.Practices.Prism.MefExtensions.xml";DestDir: "{app}"; Flags: ignoreversion
source: "../CastReporting.Reporting/bin/Release/Microsoft.Practices.Prism.xml";DestDir: "{app}"; Flags: ignoreversion
; NOTE:License
Source: "../Setup/License.rtf"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppShortName} {#MyAppVersion}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppShortName} {#MyAppVersion}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppShortName} {#MyAppVersion}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppShortName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
// Variables Globales
var
  PageParam: TInputDirWizardPage;

// Creer les Pages Personnalisées
procedure CreateTheWizardPages;
begin
  PageParam := CreateInputDirPage(wpSelectDir,
		'Select Templates Destination location',
		'Where should Template documents be located?',
		'Setup will store Template documents into a sub folder “Templates” of the following folder'#13#10#13#10 +
			'To continue, click Next. If you would like to select a different folder, click Browse.',
		False, 'New Folder');

	// Ajouter un élément (avec une valeur vide)
	PageParam.Add('');
	
	// Initialiser les valeurs par défaut (optional)
	PageParam.Values[0] := ExpandConstant('{localappdata}')+'\CAST\ReportGenerator\' + '{#MyAppVersion}';
	
end;

//Replace substring in a string
procedure FileReplace(SrcFile, sFrom, sTo: String);
var
        FileContent: AnsiString;
        FileContentW: String;
begin
    //Load srcfile to a string
    LoadStringFromFile(SrcFile, FileContent);
    FileContentW := FileContent;
    //Replace Fromstring by toString in file string content
    StringChange (FileContentW, sFrom, sTo);
    //Replace old content srcfile by the new content
    DeleteFile(SrcFile);
    SaveStringToFile(SrcFile,FileContentW, True);
end;

// Fonctions de retour
function GetTempPath(Param: String): String;
begin
    Result := PageParam.Values[0];
end;

function GetSettingsPath(Param: String): String;
begin
    //Settings dir name as used in C# report generator program
    Result := ExpandConstant('{localappdata}') + '\CAST\ReportGenerator\' + '{#MyAppVersion}';
end;

procedure SaveSettings();
var
  S1, S2 : String;
begin
    FileReplace(GetSettingsPath(S1) + '\CastReportingSetting.xml','<TemplatePath></TemplatePath>','<TemplatePath>' + GetTempPath(S2) + '\Templates</TemplatePath>'); 
end;

procedure SavePortfolioSettings();
var
  S1, S2 : String;
begin
    FileReplace(GetSettingsPath(S1) + '\CastReportingSetting.xml','<PortfolioFolderNamePath></PortfolioFolderNamePath>','<PortfolioFolderNamePath>' + GetTempPath(S2) + '\Templates\Portfolio</PortfolioFolderNamePath>'); 
end;

// Update ReadyMemo
function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo,
  MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  S, S1: String;  
begin
  S := '';
  S := S + MemoDirInfo + NewLine;
  S := S + NewLine + MemoGroupInfo + NewLine;
  S := S + NewLine + 'Templates Destination location :' + NewLine;
  S := S + Space + GetTempPath(S1) + NewLine + NewLine;
  S := S + NewLine + MemoTasksInfo + NewLine;
  Result := S;
end;



function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1.4322'     .NET Framework 1.1
//    'v2.0.50727'    .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
// Version 4.6 is needed for TLS 1.2 support
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key, fwkversion: string;
    install, serviceCount : cardinal;
    success: boolean;
begin
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version;
    success := RegQueryDWordValue(HKLM, key, 'Install', install);
    // .NET 4.0 uses value Servicing instead of SP
    // NET 4.6 in version key in registry
    if Pos('v4', version) = 1 then begin
      success := success and RegQueryStringValue(HKLM, key, 'Version', fwkversion);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;
    result := success and (install = 1) and (Pos('4.6', fwkversion) = 1);
end;

function InitializeSetup(): Boolean;
begin
    if not IsDotNetDetected('v4\Client', 0) then begin
        MsgBox('{#MyAppName} requires Microsoft .NET Framework 4.6.'#13#13
            'Please use Windows Update to install this version,'#13
            'and then re-run the {#MyAppName} setup program.', mbInformation, MB_OK);
        result := false;
    end else
        result := true;
end;
{*** INITIALISATION ***}
procedure InitializeWizard;
begin
  InitializeSetup;
  CreateTheWizardPages;
end;

