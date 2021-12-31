@echo off

:: determine product version
FOR /F "tokens=* USEBACKQ" %%F IN (`FINDSTR "AssemblyVersion(" Properties\AssemblyInfo.cs`) DO (SET verLine=%%F)
for /F "tokens=2 delims=()" %%a in ("%verLine%") do (SET quotedVer=%%a)
set ver=%quotedVer:~1,-1%
echo Release version: %ver%
echo:

:: copy release files
echo Creating release dirs...
mkdir ..\Releases\%ver%
mkdir ..\Releases\%ver%\MediaDownloader
mkdir ..\Releases\%ver%\MediaDownloader\bin
xcopy /y /s /exclude:.releaseIgnore bin\Release ..\Releases\%ver%\MediaDownloader\bin
echo:

:: create shortcut to main exe
echo Creating shortcut to MediaDownloader.exe...

set SCRIPT="%TEMP%\%RANDOM%-%RANDOM%-%RANDOM%-%RANDOM%.vbs"
echo Set oWS = WScript.CreateObject("WScript.Shell") >> %SCRIPT%
echo sLinkFile = "..\Releases\%ver%\MediaDownloader\Media Downloader.lnk" >> %SCRIPT%
echo Set oLink = oWS.CreateShortcut(sLinkFile) >> %SCRIPT%
echo oLink.TargetPath = "%cd%\..\Releases\%ver%\MediaDownloader\bin\MediaDownloader.exe" >> %SCRIPT%
echo oLink.Save >> %SCRIPT%

cscript /nologo %SCRIPT%
del %SCRIPT%
echo:

:: create portable release
echo Creating archive for protable release...
"C:\Program Files\7-Zip\7z.exe" a ..\Releases\%ver%\MediaDownloader.zip ..\Releases\%ver%\MediaDownloader\* -r