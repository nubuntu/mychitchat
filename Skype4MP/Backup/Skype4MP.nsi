!include "MUI.nsh"

;--------------------------------
;General
; Copied from NSIS docs

  ;Name and file
  Name "MediaPortal Skype Plugin Installer"
  OutFile "Skype4MPv0114.exe"

  ;Default installation folder
  InstallDir "$PROGRAMFILES\Team MediaPortal\MediaPortal"
  
  ;Get installation folder from registry if available
  ;InstallDirRegKey HKCU "Software\Modern UI Test" ""

  !insertmacro MUI_LANGUAGE "English"

!define MUI_WELCOMEPAGE_TITLE "Installing the Skype Plugin for MediaPortal"
!define MUI_WELCOMEPAGE_TEXT "This installer will set-up the Skype Plugin\r\nfor MediaPortal 0.2.2.0 and greater."
!insertmacro MUI_PAGE_WELCOME

!define MUI_DIRECTORYPAGE_TEXT_TOP "Select the directory in which MediaPortal has been installed."
!insertmacro MUI_PAGE_DIRECTORY

!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_TITLE  "The Skype Plugin has been successfully installed."
!define MUI_FINISHPAGE_TEXT  " Don't forget to enable it in the MediaPortal\r\nconfiguration before starting MediaPortal."
!define MUI_FINISHPAGE_RUN $INSTDIR\Configuration.exe
!define MUI_FINISHPAGE_RUN_TEXT "Run Configuration to enable the plugin?"
!define MUI_FINISHPAGE_NOREBOOTSUPPORT
!define MUI_FINISHPAGE_NOAUTOCLOSE
!insertmacro MUI_PAGE_FINISH

;--------------------------------
;Installer Sections

Section "InstallationSection" SecInstall

  SetOutPath "$INSTDIR\plugins\windows"
  File bin\Debug\Skype4MP.dll
  
  SetOutPath "$INSTDIR"
  File bin\Debug\Interop.SKYPE4COMLib.dll
  File D:\MediaPortalDev\SVNBuild\xbmc\bin\Debug\keymap_skype.xml
  
  SetOutPath "$INSTDIR\skin\BlueTwo"
  File skin\BlueTwo\*.xml
  
  SetOutPath "$INSTDIR\skin\BlueTwo\Media\Skype"
  File skin\Media\Skype\*.png
  
  SetOutPath "$INSTDIR\skin\BlueTwo wide"
  File "skin\BlueTwo wide\*.xml"
  
  SetOutPath "$INSTDIR\skin\BlueTwo wide\Media\Skype"
  File skin\Media\Skype\*.png
  
  CreateDirectory $INSTDIR\plugins\windows\SkypeAvatars
  
  ;Store installation folder
  ;WriteRegStr HKCU "Software\Modern UI Test" "" $INSTDIR
  
  ;Create uninstaller
  WriteUninstaller "$INSTDIR\plugins\windows\UninstallSkypePlugin.exe"

SectionEnd

Section "Uninstall"
  Delete $INSTDIR\UninstallSkypePlugin.exe ; delete self (see explanation below why this works)
  Delete $INSTDIR\Skype4MP.dll ; delete self (see explanation below why this works)
  Delete $INSTDIR\..\..\Interop.SKYPE4COMLib.dll
  Delete $INSTDIR\..\..\skin\BlueTwo\mySkype*.xml
  RMDir /r $INSTDIR\..\..\skin\BlueTwo\Media\Skype
  Delete "$INSTDIR\..\..\skin\BlueTwo wide\mySkype*.xml"
  RMDir /r "$INSTDIR\..\..\skin\BlueTwo wide\Media\Skype"
SectionEnd
