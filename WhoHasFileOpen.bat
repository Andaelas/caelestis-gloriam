@echo off
color 0A
title Who has a file open?

set site1 = "<Site 1>"
set site2 = "<Site 2>"
set site3 = "<Site 3>"
set adminUser = "<Admin User>"


:password
cls
set /p password="Before we begin, what is the Domain Administrator password? "
cls
echo If the password is wrong, you will need to exit and restart.
set /p option1="Are you sure you entered that correctly? y/n: "
if "%option1%"=="y" goto start
if "%option1%"=="n" goto password

:start
cls
echo 1 - %site1%
echo 2 - %site2%
echo 3 - %site3%
echo.
echo 0 - exit
set /p site="Select your Site: "
if "%site%"=="1" goto Site1
if "%site%"=="2" goto Site2
if "%site%"=="3" goto Site3
if "%site%"=="0" goto end
echo Invalid entry: %site%
pause
cls
goto start

:Site1
set host=<IP Host1>
goto openfile

:Site2
set host=<IP Host2>
goto openfile

:Site3
set host=<IP Host3>
goto openfile

:openfile
openfiles.exe /query /s %host% /u %adminUser% /p %password% /v > openfiles.txt 
echo file created,
echo Be sure to copy any information you need!
echo Close File to continue...
openfiles.txt
:select
cls
echo 1 - Select Disconnection type
echo 2 - Select Site again
echo.
echo 0 - Exit
set /p option2="Enter your selection: "
if "%option2%"=="1" goto DisSelect
if "%option2%"=="2" goto start
if "%option2%"=="0" goto end
echo invalid entry: %option%
pause
goto select

:DisSelect
cls
echo How would you like to Disconnect?
echo 1 - User
echo 2 - FileID 
echo 3 - Go Back to site selection
echo.
echo 0 - exit
set /p Select1="Enter your selection: "
if "%Select1%"=="1" goto User
if "%Select1%"=="2" goto FileID
if "%Select1%"=="3" goto start
if "%Select1%"=="0" goto end
echo invalid entry: %Select1%
pause
goto select

:User
cls
echo Warning! Disconencting a User connection will
echo disconnect them from all open files and lose their progress!
echo.
set /p UserName="Okay, who is the User you would like to disconnect? "
openfiles.exe /disconnect /s %host% /u %adminUser% /p %password% /a %UserName%
echo Taking you back to the Selection menu
pause
goto select

:FileID
cls
echo Warning! Disconnecting a file will lose unsaved data!
echo.
set /p File="Okay, what is the File ID#? "
openfiles.exe /disconnect /s %host% /u %adminUser% /p %password% /id %File%
echo Taking you back to the Selection menu
pause
goto select


:end
del openfiles.txt
echo Got rid of that txt file.
echo Goodbye
pause
exit