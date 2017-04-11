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
for /f %%v in ('powershell -noprofile "(Get-Command .\BotBuilderChannelConnector\bin\release\Microsoft.Bot.Builder.dll).FileVersionInfo.FileVersion"') do set builder=%%v
rem .\.paket\paket.exe pack output nugets version %builder%
.\.paket\paket.exe pack output nugets version 4.0.1-alpha
echo *** Finished building Bot.Builder.ChannelConnector
