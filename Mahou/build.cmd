@ECHO OFF>nul
SET configuration=%1
SET platform=%2
IF [%1] == [clean] GOTO CLEAN
IF [%1] == [] SET configuration=Release
IF [%2] == [x86_x64] SET platform="Any CPU"
IF [%2] == [] SET platform="Any CPU"
IF [%1] == [-h] GOTO HELP
IF [%1] == [--help] GOTO HELP
IF [%1] == [/?] GOTO HELP
@%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe .\Mahou.sln /nologo /m /t:Build /p:Configuration=%configuration% /p:Platform=%platform%
GOTO EOF
:CLEAN
clean.cmd
:HELP
ECHO Usage:
ECHO   build.cmd [configuration] [platform]
ECHO By default configuration is "release" and platform is "Any CPU".
ECHO Exapmles:
ECHO   build.cmd debug x86         -^> Builds Mahou debug x86.
ECHO   build.cmd release "Any CPU" -^> Builds Mahou release any cpu.
ECHO   build.cmd debug x86_x64     -^> Builds Mahou debug any cpu.
ECHO   build.cmd clean             -^> calls clean.cmd.
:EOF
