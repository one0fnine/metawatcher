function takeScreenshots {
  Param([int[]]$ids, [string]$path)

  $settings = getGraphicSettings

  foreach($id in $ids) {
    $process = Get-Process -Id $id
    $screenshotPath = getScreenshotPath -id $id -process $process -path $path

    toggleWindow -process $process
    $screenshotSettings = @{
      out = $screenshotPath
      image = $settings.Image
      graphic = $settings.Graphic
      left = $settings.Left
      top = $settings.Top
    }
    takeScreenshot @screenshotSettings 
  }
}

function toggleWindow {
  Param([PSObject]$process, [int]$pause = 3)

  Set-ForegroundWindow ($process).MainWindowHandle
  Start-Sleep -Second $pause
}

function getScreenshotPath {
  Param([int]$id, [PSObject]$process, [string]$path)

  $titleObject = $process | Where-Object { $_.MainWindowTitle } | Select-Object MainWindowTitle
  $first,$last = $titleObject.MainWindowTitle.Split(': ')

  $path + '\' + $first + '.png'
}

function takeScreenshot {
  Param([string]$out,
    [PSObject]$image,
    [PSObject]$graphic,
    [int]$left,
    [int]$top,
    [int]$right = 0,
    [int]$bottom = 0)

  $graphic.CopyFromScreen($left, $top, $right, $bottom, $image.Size)
  $image.Save($out)
}

function getGraphicSettings {
  Add-Type -AssemblyName System.Windows.Forms
  Add-Type -AssemblyName System.Drawing

  $screen = [System.Windows.Forms.SystemInformation]::VirtualScreen
  $width = $screen.Width
  $height = $screen.Height
  $top = $screen.Top
  $left = $screen.Left

  $imageObject = New-Object System.Drawing.Bitmap $width, $height
  $graphicObject = [System.Drawing.Graphics]::FromImage($imageObject)
  
  [PSCustomObject]@{
    Top = $top
    Left = $left
    Right = 0
    Bottom = 0
    Image = $imageObject
    Graphic = $graphicObject  
  }
}

Export-ModuleMember -Function @('takeScreenshots')
