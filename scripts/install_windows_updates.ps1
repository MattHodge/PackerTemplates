$wuInstallExe = Join-Path "$($env:windir)\SYSTEM32" 'WUInstallAMD64.exe'

if (!(Test-Path -Path $wuInstallExe))
{
    Invoke-WebRequest -UseBasicParsing -Uri 'https://dl.dropboxusercontent.com/u/727435/Tools/WUInstallAMD64.exe' -OutFile $wuInstallExe
}

C:\WINDOWS\SYSTEM32\WUInstallAMD64.exe /install /autoaccepteula /silent
