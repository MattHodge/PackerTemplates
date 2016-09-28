[cmdletbinding()]
param(
    [bool]$Core = $false
)

if ($Core)
{
  & packer.exe build -var 'core=Core' .\01-win2012r2-standard-base.json
  & packer.exe build -var 'source_path=.\output-win2012r2core-base\win2012r2core-base.ovf' -var 'core=Core' .\02-win2012r2-standard-win_updates-wmf5.json
  & packer.exe build -var 'source_path=.\output-win2012r2Core-updates_wmf5\win2012r2Core-updates_wmf5.ovf' -var 'core=Core' .\03-win2012r2-standard-virtualbox-client.json
  & packer.exe build -var 'source_path=.\output-win2012r2Core-vbox-client\win2012r2Core-vbox-client.ovf' -var 'core=Core' .\04-win2012r2-standard-cleanup.json
  & packer.exe build -var 'source_path=.\output-win2012r2Core-cleanup\win2012r2Core-cleanup.ovf' -var 'version=0.0.2' -var 'core=Core' .\05-win2012r2-standard-final.json
}
else
{
  & packer.exe build .\01-win2012r2-standard-base.json
  & packer.exe build -var 'source_path=.\output-win2012r2-base\win2012r2-base.ovf' .\02-win2012r2-standard-win_updates-wmf5.json
  & packer.exe build -var 'source_path=.\output-win2012r2-updates_wmf5\win2012r2-updates_wmf5.ovf' .\03-win2012r2-standard-virtualbox-client.json
  & packer.exe build -var 'source_path=.\output-win2012r2-vbox-client\win2012r2-vbox-client.ovf' .\04-win2012r2-standard-cleanup.json
  & packer.exe build -var 'source_path=.\output-win2012r2-cleanup\win2012r2-cleanup.ovf' -var 'version=0.0.2' .\05-win2012r2-standard-final.json
}
