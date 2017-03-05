if ($env:install_vbox_tools -eq $true)
{
  Write-Output "Installing Virtualbox Guest Additions"
  Write-Output "Checking for Certificates in vBox ISO"
  if(test-path E:\ -Filter *.cer)
  {
    Get-ChildItem E:\cert -Filter *.cer | ForEach-Object { certutil -addstore -f "TrustedPublisher" $_.FullName }
  }
  Start-Process -FilePath "E:\VBoxWindowsAdditions.exe" -ArgumentList "/S" -Wait
}
else
{
  Write-Output "Skipping installation of Virtualbox Guest Additions"
}
