@echo off
:menu
cls
echo Choose an option:
echo +----------------------------------------------------------------------------------------------------+
echo 1. DISM Scan health
echo 	Scans the Windows image for corruption or integrity issues
echo 	Useful when troubleshooting system problems or verifying the health of the Windows installation.
echo +----------------------------------------------------------------------------------------------------+
echo 2. DISM Restore health
echo 	Restores the Windows image by repairing any corrupted files found during the scan.
echo 	Helps fix issues related to missing or damaged system files.	
echo +----------------------------------------------------------------------------------------------------+
echo 3. DISM Check health
echo 	Checks the health of the Windows image without attempting repairs.
echo 	Provides a quick assessment of the system’s health.
echo +----------------------------------------------------------------------------------------------------+
echo 4. SFC Scan now
echo 	Scans and repairs protected system files using the System File Checker (SFC) utility.
echo 	Useful when encountering issues related to system file corruption.
echo +----------------------------------------------------------------------------------------------------+
echo 5. WMIC Get hard disks status
echo 	Retrieves information about the status of hard disk drives.
echo 	Helps monitor disk health, check for errors, and identify failing drives.
echo +----------------------------------------------------------------------------------------------------+
echo 6. Exit

set /p choice=Enter the number of your choice: 

if "%choice%"=="1" (
    start cmd /k dism /Online /Cleanup-Image /ScanHealth
    goto menu
) else if "%choice%"=="2" (
    start cmd /k dism /Online /Cleanup-Image /RestoreHealth
    goto menu
) else if "%choice%"=="3" (
    start cmd /k dism /Online /Cleanup-Image /CheckHealth
    goto menu
) else if "%choice%"=="4" (
    start cmd /k sfc /scannow
    goto menu
) else if "%choice%"=="5" (
    start cmd /k wmic diskdrive get status
    goto menu
) else if "%choice%"=="6" (
    exit
) else (
    echo Invalid choice. Please try again.
    pause
    goto menu
)