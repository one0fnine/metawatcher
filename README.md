
# metawatcher

powershell scripts to look after mt4 terminals.

If we can't use `Import-Module` and get an error that this isn't recognized. You have to install Windows Management 5.1 and higher.
<https://www.microsoft.com/en-us/download/details.aspx?id=54616>

To work with `Set-Foreground` cmdlet it's need installing **Pscx** module.
Following this command:

```powershell
    Install-Module -Name Pscx -Force -AllowClobber
```
