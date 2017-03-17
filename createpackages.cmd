echo *** Building Bot.Builder.ChannelConnector
setlocal
setlocal enabledelayedexpansion
setlocal enableextensions
set errorlevel=0
mkdir .\nugets
erase /s .\nugets\Bot.Builder.ChannelConnector*nupkg
msbuild /property:Configuration=release BotBuilderChannelConnector\BotBuilderChannelConnector.csproj 
msbuild /property:Configuration=release BotBuilderChannelConnectorOwin\BotBuilderChannelConnectorOwin.csproj 
for /f %%v in ('powershell -noprofile "(Get-Command .\BotBuilderChannelConnector\bin\release\Microsoft.Bot.Builder.dll).FileVersionInfo.FileVersion"') do set builder=%%v
.\.paket\paket.exe pack output nugets version %builder%
echo *** Finished building Bot.Builder.ChannelConnector
