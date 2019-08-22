function sendEmail {
    Param([String]$username,
        [String]$password,
        [String]$to,
        [String]$body,
        [String]$attachmentsDirPath)

    $data = @{
        From = $username
        To = $to
        Subject = "AutoScreenshots"
        Body = $body
        BodyAsHtml = $true
        Attachments = Get-ChildItem $attachmentsDirPath | %{ $_.FullName }
    }

    $server = @{
        SmtpServer = "smtp.gmail.com"
        Port = 587
        Credential = New-Object System.Management.Automation.PSCredential($username, (ConvertTo-SecureString $password -AsPlainText -Force))
        UseSsl = $true
    }

    Send-MailMessage @data @server
}

# sendEmail -username 'from@example.com' -password 'password' -to 'to@example.com' -attachmentsDirPath 'C:\tmp' -body 'text'

function htmlEmailBody {
    Param([PSObject]$body)

    $lineBreak = '<br />'
    $tableStyle = @"
<style>
table { border-width: 1px; border-style: solid; border-color: black; border-collapse: collapse; width: 100%; text-align: center; }
th { border-width: 1px; padding: 3px; border-style: solid; border-color: black; background-color: #6495ed; }
tr { border-width: 1px; padding: 3px; border-style: solid; border-color: black }
</style>
"@

    ($body | ConvertTo-Html -Head $tableStyle | Out-String) + $lineBreak
}

function generateHtmlBody {
    Param([PSObject[]]$body)

    $body | ForEach-Object { htmlEmailBody -body $_ }
}
