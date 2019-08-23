Import-Module "$PSScriptRoot\scripts\index.psm1"

$screenshotsDirPath="$TMP\screenshots\"

prepare_system_env
prepare_screenshots_directory -path $screenshotsDirPath

$body = @((totalConsuption),(metatraderConsuption))
$ids = Get-Process -Name terminal | Select-Object -expand id
$emailParams = @{
  username = [environment]::GetEnvironmentVariable('EMAILUSERNAME', 'User')
  password = [environment]::GetEnvironmentVariable('EMAILPASSWORD', 'User')
  to = [environment]::GetEnvironmentVariable('EMAILRECIPIENT', 'User')
  attachmentsDirPath = $screenshotsDirPath
  body = generateHtmlBody -body $body
}

takeScreenshots -ids $ids -path $screenshotsDirPath
sendEmail @emailParams
