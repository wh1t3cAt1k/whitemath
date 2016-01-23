using System.Drawing.Imaging;
using System.IO;
using System.Drawing;

namespace whiteMath.Graphers
{
    public static class GrapherHelper
    {
        /// <summary>
        /// Gets a blank image with specified background color.
        /// </summary>
        /// <param name="width">A positive width in pixels.</param>
        /// <param name="height">A positive height in pixels.</param>
        /// <param name="backgroundColor">The background color.</param>
        /// <returns></returns>
        public static Bitmap GetBlankImage(int width, int height, Color backgroundColor)
        {
            Bitmap bmp = new Bitmap(width, height);

            try
            {
                Graphics g = Graphics.FromImage(bmp);
                g.FillRectangle(new SolidBrush(backgroundColor), new Rectangle(0, 0, width, height));
            }
            catch
            {
                bmp.Dispose();
            }

            return bmp;
        }

        /// <summary>
        /// Saves the specified image to a file.
        /// Calls the standard Image.Save method.
        /// </summary>
        /// <param name="img">The image object you want to save.</param>
        /// <param name="FileName">The full file name.</param>
        /// <param name="ImageType">ImageFormat object to specify the image format.</param>
        public static void SaveImageToFile(Image img, string FileName, ImageFormat ImageType)
        {
            FileStream output;
            
            try { output = new FileStream(FileName, FileMode.Create, FileAccess.Write); }
            catch { throw; }

            img.Save(output, ImageType);
            output.Close();
        }
    }
}
