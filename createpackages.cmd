@echo off
echo *** Building Bot.Builder.ChannelConnector
setlocal
setlocal enabledelayedexpansion
setlocal enableextensions
set errorlevel=0
mkdir .\nugets
erase /s .\nugets\Bot.Builder.ChannelConnector*nupkg
msbuild /property:Configuration=release BotBuilderChannelConnector\BotBuilderChannelConnector.csproj 
msbuild /property:Configuration=release BotBuilderChannelConnectorOwin\BotBuilderChannelConnectorOwin.csproj 
for /f %%v in ('powershell -noprofile "(Get-Command .\BotBuilderChannelConnector\bin\release\Bot.Builder.ChannelConnector.dll).FileVersionInfo.ProductVersion"') do set productVersion=%%v
echo Building version %productVersion% for ChannelConnector
.\.paket\paket.exe pack output nugets templatefile ./BotBuilderChannelConnector/paket.template version %productVersion% lock-dependencies
.\.paket\paket.exe push url "https://www.nuget.org" file ./nugets/Bot.Builder.ChannelConnector.%ProductVersion%.nupkg 

for /f %%v in ('powershell -noprofile "(Get-Command .\BotBuilderChannelConnectorOwin\bin\release\Bot.Builder.ChannelConnector.Owin.dll).FileVersionInfo.ProductVersion"') do set productVersion=%%v
echo Building version %productVersion% for ChannelConnector.Owin
.\.paket\paket.exe pack output nugets templatefile ./BotBuilderChannelConnectorOwin/paket.template version %productVersion% lock-dependencies
.\.paket\paket.exe push url "https://www.nuget.org" file ./nugets/Bot.Builder.ChannelConnector.Owin.%ProductVersion%.nupkg 

echo *** Finished building Bot.Builder.ChannelConnector
