; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "BimIshou"
#define MyAppVersion "1.0"
#define MyAppPublisher "Manh Em"
#define MyAppURL "Manh Em"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{15773E79-E8F9-438E-BD40-29E2B27A7DB5}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}
DefaultGroupName={#MyAppName}
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputDir=C:\Users\Admin\Desktop\New folder
OutputBaseFilename=BimIshou
SetupIconFile=C:\Users\Admin\Desktop\New folder\setup.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\BimIshou.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\BimIshou.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\BimIshou.pdb"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\CommunityToolkit.Mvvm.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\ControlzEx.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\JetBrains.Annotations.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\MahApps.Metro.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\Microsoft.Bcl.AsyncInterfaces.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\Microsoft.Xaml.Behaviors.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\Nice3point.Revit.Extensions.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\Nice3point.Revit.Toolkit.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\ricaun.DI.1.0.0.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\ricaun.Revit.DI.1.0.0.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\System.Buffers.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\System.ComponentModel.Annotations.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\System.Memory.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\System.Numerics.Vectors.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\System.Runtime.CompilerServices.Unsafe.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
Source: "C:\Users\Admin\Desktop\New folder\BimIshou-Manh\bin\Debug R21\System.Threading.Tasks.Extensions.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021\{#MyAppName}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

