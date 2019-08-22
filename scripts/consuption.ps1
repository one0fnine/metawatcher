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
        @{Name="RAM (MB)"; Expression={[Math]::Round(($_.workingSetPrivate / 1mb), 2)}},
        @{Name="Disk (MB)"; Expression={[Math]::Round(($_.IODataOperationPersec / 1mb), 2)}}
    )
    $terminals = Get-WmiObject -Class Win32_PerfFormattedData_perfproc_process | Where-Object { $_.Name -like '*terminal*' }
    
    $terminals | Select-Object $properties
}
