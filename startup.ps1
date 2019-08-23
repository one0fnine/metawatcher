Import-Module "$PSScriptRoot/scripts/system.psm1"

prepare_system_env

$path = @([System.Environment]::GetEnvironmentVariable('BLACKDPATH', 'User') | Split-String -Separator ';' -ErrorAction SilentlyContinue)

foreach($dir in $path) {
  $terminalPath = Join-Path $dir -ChildPath '\terminal.exe'
  if(Test-Path $terminalPath) {
    Start-Process -FilePath $terminalPath -WindowStyle Maximized  
  }
}
