function scriptsDirPath {
  if ($psISE) {
    Split-Path -Path $psISE.CurrentFile.FullPath
  } else {
    $PSScriptRoot
  }
}

$moduleFunctions = @(Get-ChildItem -Path (scriptsDirPath) -Exclude index.ps1 -ErrorAction SilentlyContinue)
$toExport = $moduleFunctions | Select-Object -ExpandProperty BaseName

foreach ($module in $moduleFunctions) {
  . $module.FullName
}

