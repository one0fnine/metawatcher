function totalConsuption {
  $resultObject = New-Object PSCustomObject
  $cpu = (Get-WmiObject -Class win32_processor | Measure-Object -Property LoadPercentage -Average | Select-Object Average).Average
  $hostMemroy = Get-WmiObject -Class win32_operatingsystem
  $memory = ((($hostMemroy.TotalVisibleMemorySize - $hostMemroy.FreePhysicalMemory)*100)/$hostMemroy.TotalVisibleMemorySize)
  $roundMemory = [math]::Round($memory, 2)

  $resultObject | Add-Member -MemberType NoteProperty -Name 'Total CPU (%)' -Value $cpu
  $resultObject | Add-Member -MemberType NoteProperty -Name 'Total RAM (%)' -Value $roundMemory
    
  $resultObject
}


function metatraderConsuption {
  $properties = @(
    @{Name="Name"; Expression={ $_.name }},
    @{Name="PID"; Expression={ $_.IDProcess }},
    @{Name="CPU (%)"; Expression={ $_.PercentProcessorTime }},
    @{Name="RAM (MB)"; Expression={ roundRAM -memory $_.workingSetPrivate }},
    @{Name='IsOverload'; Expression={ isOverLoad -terminal $_ }}
  )
  $terminals = Get-WmiObject -Class Win32_PerfFormattedData_perfproc_process | Where-Object { $_.Name -like '*terminal*' }
    
  $terminals | Select-Object $properties
}

function isOverLoad {
  Param(
    [int]$maxLoadCPU = [System.Environment]::GetEnvironmentVariable('MAXLOADCPU', 'User'),
    [int]$maxLoadRAM = [System.Environment]::GetEnvironmentVariable('MAXLOADRAM', 'User'),
    [psobject]$terminal
  )

  if (($maxLoadCPU -eq $null) -and ($maxLoadRAM -eq $null)) { return $false }

  $isCPUOverload = $terminal.PercentProcessorTime -gt $maxLoadCPU
  $isRAMOverload = (roundRAM -memory $terminal.WorkingSetPrivate) -gt $maxLoadRAM

  $isCPUOverload -or $isRAMOverload
}

function roundRAM {
  Param([int]$memory)

  [Math]::Round(($memory / 1mb), 2)
}

Export-ModuleMember -Function @('totalConsuption', 'metatraderConsuption', 'isOverLoad')
