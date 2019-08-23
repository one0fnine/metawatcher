$moduleFunctions = @(Get-ChildItem -Path "$PSScriptRoot/*" -Exclude index.* -Include *.psm1 -ErrorAction SilentlyContinue)

foreach ($module in $moduleFunctions) {
  Import-Module $module.FullName
}
