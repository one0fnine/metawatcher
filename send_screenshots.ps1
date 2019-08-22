Import-Module '.\scripts\consuption.ps1'
Import-Module '.\scripts\send_email.ps1'
Import-Module '.\scripts\take_screenshot.ps1'

$username = 'username'
$password = 'password'
$sendTo = 'email'
$screenshotsDirPath = "C:\tmp"
$body = @((totalConsuption),(metatraderConsuption))
$ids = Get-Process -Name terminal | select -expand id
$emailParams = @{
  username = $username
  password = $password
  to = $sendTo
  attachmentsDirPath = $screenshotsDirPath
  body = generateHtmlBody -body $body
}

takeScreenshots -ids $ids -path $screenshotsDirPath
sendEmail @emailParams
