@echo off
:: BatchGotAdmin code adapted from https://stackoverflow.com/a/8385663/14678391
REM Check for permissions
>nul 2>&1 "%SYSTEMROOT%\system32\cacls.exe" "%SYSTEMROOT%\system32\config\system"
REM If error flag set, we do not have admin access
if '%errorlevel%' NEQ '0' (
    echo Requesting administrative privileges...
    goto UACPrompt
) else ( goto gotAdmin )

:UACPrompt
    echo Set UAC = CreateObject^("Shell.Application"^) > "%temp%\getadmin.vbs"
    set params = %*:"="
    echo UAC.ShellExecute "cmd.exe", "/c %~s0 %params%", "", "runas", 1 >> "%temp%\getadmin.vbs"

    "%temp%\getadmin.vbs"
    del "%temp%\getadmin.vbs"
    exit /B


:gotAdmin
    pushd "%CD%"
    CD /D "%~dp0"

:: Create a system restore point with the current date
for /f "tokens=2-4 delims=/ " %%a in ('date /t') do (
    set mydate=%%c%%a%%b
)
wmic.exe /Namespace:\\root\default Path SystemRestore Call CreateRestorePoint "PM%mydate%", 105, 