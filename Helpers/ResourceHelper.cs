using System.Drawing;
using System.IO;

namespace STTNote.Helpers
{
    public static class ResourceHelper
    {
        public static Image? GetImage(string imageName)
        {
            try
            {
                var imagePath = $"{Directory.GetCurrentDirectory()}/Resources/{imageName}";
                if (File.Exists(imagePath))
                {
                    return Image.FromFile(imagePath);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}