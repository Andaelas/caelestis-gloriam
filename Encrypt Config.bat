@echo off
color 0A
title Encrypt Config

::All work is done in this directory, update for newer versions
SET masterDirectory="C:\Windows\Microsoft.NET\Framework\v4.0.30319"

:BEGIN
cls
SET /p directory="Enter the full directory where your app.config or web.config is located: "
set /p option1="Are you sure you entered that correctly? y/n: "
if "%option1%"=="y" goto DIRECTORY
if "%option1%"=="n" goto BEGIN

:DIRECTORY
cd %directory%
dir *.config | find "App"
if %errorlevel% == 0 GOTO APPENCRYPT
dir *.config | find "Web"
if %errorlevel% == 0 GOTO WEBNCRYPT
if %errorlevel% == 1 ECHO Config file not found! Press a key to continue.
PAUSE
GOTO BEGIN

:APPENCRYPT
COPY /Y App.config App.config.old
cd %masterDirectory%
ASPNET_REGIIS -pef "connectionStrings" "%directory%"
GOTO END

:WEBENCRYPT
COPY /Y Web.config Web.config.old
cd %masterDirectory%
SPNET_REGIIS -pef "connectionStrings" %directory%
GOTO END


:END
PAUSE
EXIT