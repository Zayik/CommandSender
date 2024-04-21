
setlocal EnableDelayedExpansion

REM Get the full path of the directory where the batch file is located
set CURRENT_DIR=%~dp0

REM Remove the trailing backslash from the directory path
set CURRENT_DIR=!CURRENT_DIR:~0,-1!

REM USAGE: Install.bat <DEBUG/RELEASE> <UUID>
REM Example: Install.bat RELEASE com.barraider.spotify
REM setlocal
REM cd /d bin/%~dp0
REM cd %1

REM *** MAKE SURE THE FOLLOWING VARIABLES ARE CORRECT ***
REM (Distribution tool be downloaded from: https://developer.elgato.com/documentation/stream-deck/sdk/exporting-your-plugin/ )
REM Set the new variables using the current directory path
set OUTPUT_DIR=!CURRENT_DIR!\Release
set DISTRIBUTION_TOOL=!CURRENT_DIR!\DistributionTool.exe
SET STREAM_DECK_FILE="C:\Program Files\Elgato\StreamDeck\StreamDeck.exe"
SET STREAM_DECK_LOAD_TIMEOUT=7

taskkill /f /im streamdeck.exe
taskkill /f /im %2.exe
timeout /t 2
del %OUTPUT_DIR%\%2.streamDeckPlugin
%DISTRIBUTION_TOOL% -b -i CommandSender/bin/Debug/%2.sdPlugin -o %OUTPUT_DIR%
rmdir %APPDATA%\Elgato\StreamDeck\Plugins\%2.sdPlugin /s /q
start "" %STREAM_DECK_FILE%
timeout /t %STREAM_DECK_LOAD_TIMEOUT%

start %OUTPUT_DIR%\%2.streamDeckPlugin