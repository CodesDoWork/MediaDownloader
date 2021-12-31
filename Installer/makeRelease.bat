@echo off

:: determine product version
FOR /F "tokens=* USEBACKQ" %%F IN (`FINDSTR "AssemblyVersion(" ..\MediaDownloader\Properties\AssemblyInfo.cs`) DO (SET verLine=%%F)
for /F "tokens=2 delims=()" %%a in ("%verLine%") do (SET quotedVer=%%a)
set ver=%quotedVer:~1,-1%
echo Release version: %ver%
echo:

:: copy release files
echo Copying installer
xcopy /y /s bin\Release\MediaDownloader.msi ..\Releases\%ver%\