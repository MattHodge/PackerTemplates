if ((Get-WmiObject -Class Win32_OperatingSystem).Caption -like '*Windows 10*')
{
  Get-NetAdapter | Set-NetConnectionProfile -NetworkCategory Private
  Enable-PSRemoting -Force -SkipNetworkProfileCheck
}
else
{
  Enable-PSRemoting -Force
}
winrm set winrm/config/client/auth '@{Basic="true"}'
winrm set winrm/config/service/auth '@{Basic="true"}'
winrm set winrm/config/service '@{AllowUnencrypted="true"}'
Restart-Service -Name WinRM
netsh advfirewall firewall add rule name="WinRM-HTTP" dir=in localport=5985 protocol=TCP action=allow
