function prepare_system_env {
  $envFromFile = Get-Content -raw "$PSScriptRoot\..\.env" | ConvertFrom-StringData

  foreach($name in $envFromFile.keys) {
    [Environment]::SetEnvironmentVariable($name, $envFromFile[$name], 'User')      
  }
}

function prepare_screenshots_directory {
    Param([string]$path = "$PSScriptRoot\..\screenshots")
    
    $path = if ([System.Environment]::screenshotsDirPath) {
      [System.Environment]::screenshotsDirPath
    } else {
      $path  
    }

    if ((Test-Path $path) -eq $false) {
      [void](New-Item -ItemType Directory -Path $path)
    } 
}

Export-ModuleMember -Function @('prepare_system_env', 'prepare_screenshots_directory')
