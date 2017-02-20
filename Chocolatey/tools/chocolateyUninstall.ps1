$instpth = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
Remove-Item $instpth\Mahou.exe
$confirmation = Read-Host "Delete Mahou settings and snippets?[y/n]"
if ($confirmation -eq 'y' -And $confirmation -eq 'Y') {
  Remove-Item $instpth\Mahou.ini -Force
  Remove-Item $instpth\snippets.txt -Force
}