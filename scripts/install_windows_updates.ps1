$ErrorActionPreference = 'Stop'

$wuInstallExe = Join-Path "$($env:windir)\SYSTEM32" 'WUInstallAMD64.exe'

if (!(Test-Path -Path $wuInstallExe))
{
    Invoke-WebRequest -UseBasicParsing -Uri 'https://www.dropbox.com/s/bk1dodl4fb7znj3/WUInstallAMD64.exe?dl=1' -OutFile $wuInstallExe
}

C:\WINDOWS\SYSTEM32\WUInstallAMD64.exe /install /autoaccepteula /silent /nomatch "KB4041685"

# /reboot_if_needed_force /rebootcycle 10 /customActionBefore "Powershell.exe Stop-Service -force winrm && Powershell.exe Set-Service -Name winrm -StartupType Disabled" /customActionAfter "Powershell.exe Set-Service -Name winrm -StartupType Automatic && Powershell.exe Start-Service winrm"
