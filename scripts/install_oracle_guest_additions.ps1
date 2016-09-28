if(Test-Path "C:\Users\vagrant\VBoxGuestAdditions.iso") {
    Write-Host "Installing Guest Additions"
    certutil -addstore -f "TrustedPublisher" A:\oracle.cer
    cinst 7zip.commandline -y
    Move-Item C:\Users\vagrant\VBoxGuestAdditions.iso C:\Windows\Temp
    ."C:\ProgramData\chocolatey\lib\7zip.commandline\tools\7z.exe" x C:\Windows\Temp\VBoxGuestAdditions.iso -oC:\Windows\Temp\virtualbox

    Start-Process -FilePath "C:\Windows\Temp\virtualbox\VBoxWindowsAdditions.exe" -ArgumentList "/S" -WorkingDirectory "C:\Windows\Temp\virtualbox" -Wait

    Remove-Item C:\Windows\Temp\virtualbox -Recurse -Force
    Remove-Item C:\Windows\Temp\VBoxGuestAdditions.iso -Force
}
