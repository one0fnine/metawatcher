$defaultExecuteArgs = "-WindowStyle Hidden -NoProfile -NoLogo -NonInteractive -ExecutionPolicy Bypass"

# will start terminals if a host was restarted
$action = New-ScheduledTaskAction -Execute 'powershell.exe' -Argument "$defaultExecuteArgs -File $PSScriptRoot\startup.ps1"
$trigger = New-ScheduledTaskTrigger -AtLogOn
$settings = New-ScheduledTaskSettingsSet -RunOnlyIfNetworkAvailable -ExecutionTimeLimit (New-TimeSpan -Minutes 5) -Compatibility Win7
Register-ScheduledTask -TaskName 'Start Terminals at LogOn' -Action $action -Trigger $trigger -Settings $settings -Force

# will send about terminals information if a host was restarted (task delay for 5 min)
$trigger.Delay = 'PT5M'
$action = New-ScheduledTaskAction -Execute 'powershell.exe' -Argument "$defaultExecuteArgs -File $PSScriptRoot\send_meta_bundle.ps1"
Register-ScheduledTask -TaskName 'Send Terminals information at LogOn' -Action $action -Trigger $trigger -Settings $settings -Force 

# will send about terminals information every 4 hours
$trigger = New-ScheduledTaskTrigger -Once -At 2am -RepetitionInterval (New-TimeSpan -Hours 4)
Register-ScheduledTask -TaskName 'Send Terminals information every 4 hours' -Action $action -Trigger $trigger -Settings $settings -Force 
