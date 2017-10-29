$wuInstallExe = Join-Path "$($env:windir)\SYSTEM32" 'WUInstallAMD64.exe'

if (!(Test-Path -Path $wuInstallExe))
{
    Invoke-WebRequest -UseBasicParsing -Uri 'https://www.dropbox.com/s/u0cf68kjw1i00n5/WUInstallAMD64.exe?dl=1' -OutFile $wuInstallExe
}

C:\WINDOWS\SYSTEM32\WUInstallAMD64.exe /install /autoaccepteula /silent /nomatch "KB4041685"
