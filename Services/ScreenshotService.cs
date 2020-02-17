using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace metawatcher.Services
{
  public class ScreenshotService
  {
    private int width = 0;
    private int height = 0;
    private Size size;
    private int left = 0;
    private int top = 0;

    public int Width
    { set => width = value; }

    public int Height
    { set => height = value; }

    public Size Size
    { set => size = value; }

    public int Left
    { set => left = value; }

    public int Top
    { set => top = value; }

    public MemoryStream ScreenshotStream()
    {
      using(var image = new Bitmap(width, height, PixelFormat.Format32bppArgb))
      {
        using(var graphics = Graphics.FromImage(image))
        {
          var memoryStream = new MemoryStream();
          graphics.CopyFromScreen(left, top, 0, 0, size, CopyPixelOperation.SourceCopy);
          image.Save(memoryStream, ImageFormat.Png);
          
          return memoryStream;
        }
      }
    }
  }
}