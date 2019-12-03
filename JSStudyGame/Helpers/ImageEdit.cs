using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace JSStudyGame.Helpers
{
    public static class ImageEdit
    {
        /// <summary>
        /// Create Bitmap from image with saving proportion
        /// </summary>
        /// <param name="originalPic"> Bitmap of original image </param>
        /// <param name="maxWidth"> User's wanted width </param>
        /// <param name="maxHeight"> User's wanted height </param>
        /// <returns></returns>
        public static Bitmap CreateBitmapUserSize(this Bitmap originalPic, int maxWidth, int maxHeight)
        {
            try
            {
                int width = originalPic.Width;
                int height = originalPic.Height;
                int widthDiff = width - maxWidth;
                int heightDiff = height - maxHeight;
                bool doWidthResize = (maxWidth > 0 && width > maxWidth && widthDiff > heightDiff);
                bool doHeightResize = (maxHeight > 0 && height > maxHeight && heightDiff > widthDiff);
                if (doWidthResize || doHeightResize || (width.Equals(height) && widthDiff.Equals(heightDiff)))
                {
                    int iStart;
                    Decimal divider;
                    if (doWidthResize)
                    {
                        iStart = width;
                        divider = Math.Abs((Decimal)iStart / maxWidth);
                        width = maxWidth;
                        height = (int)Math.Round((height / divider));
                    }
                    else
                    {
                        iStart = height;
                        divider = Math.Abs((Decimal)iStart / maxHeight);
                        height = maxHeight;
                        width = (int)Math.Round(width / divider);
                    }
                }
                using (Bitmap outBmp = new Bitmap(width, height, PixelFormat.Format24bppRgb))
                {
                    using (Graphics oGraphics = Graphics.FromImage(outBmp))
                    {
                        oGraphics.DrawImage(originalPic, 0, 0, width, height);
                        return new Bitmap(outBmp);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Convert Bitmap to BitmapImage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage BitmapToImageSource(this Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Jpeg);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        /// <summary>
        /// Create BitmapImage from image with saving proportion
        /// </summary>
        /// <param name="fileName"> Path to Image</param>
        /// <param name="maxWidth"> User's wanted width </param>
        /// <param name="maxHeight"> User's wanted height </param>
        /// <returns></returns>
        public static BitmapImage CreateBitmapImageUserSize(string fileName, int maxWidth, int maxHeight)
        {
            try
            {
                var originalPic = new BitmapImage(new Uri(fileName)); 
                double width = originalPic.Width;
                double height = originalPic.Height;
                double widthDiff = width - maxWidth;
                double heightDiff = height - maxHeight;
                bool doWidthResize = (maxWidth > 0 && width > maxWidth && widthDiff > heightDiff);
                bool doHeightResize = (maxHeight > 0 && height > maxHeight && heightDiff > widthDiff);
                if (doWidthResize || doHeightResize || (width.Equals(height) && widthDiff.Equals(heightDiff)))
                {
                    double iStart;
                    double divider;
                    if (doWidthResize)
                    {
                        iStart = width;
                        divider = Math.Abs(iStart / maxWidth);
                        width = maxWidth;
                        height = Math.Round(height / divider);
                    }
                    else
                    {
                        iStart = height;
                        divider = Math.Abs(iStart / maxHeight);
                        height = maxHeight;
                        width = Math.Round(width / divider);
                    }
                }

                Uri uri = new Uri(fileName);
                BitmapImage source = new BitmapImage();
                source.BeginInit();
                source.UriSource = uri;
                source.DecodePixelHeight = (int)height;
                source.DecodePixelWidth = (int)width;
                source.EndInit();
                return source;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Convert BitmapImage To Base64 string
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns> retutn Base64 String </returns>
        public static string ToBase64String(this BitmapImage bitmapImage)
        {
            string imageInBase64 = string.Empty;
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            try
            {

                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    data = ms.ToArray();
                }
                imageInBase64 = Convert.ToBase64String(data);
            }
            catch (Exception)
            {
                imageInBase64 = string.Empty; ;
            }
            return imageInBase64;
        }

        /// <summary>
        /// Saving BitmapImage in jpeg format 
        /// </summary>
        /// <param name="bitmapImage">image in BitmapImage format</param>
        /// <param name="fileName">Path to file and name of </param>
        public static void SaveToJpegFile(this BitmapImage bitmapImage, string fileName)
        {
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

            using (var fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        /// <summary>
        /// Create BitmapImage
        /// </summary>
        /// <param name="fileName"> Path and name file </param>
        /// <returns></returns>
        public static BitmapImage CreateBitmapImage(string fileName)
        {
            return new BitmapImage(new Uri(fileName, UriKind.RelativeOrAbsolute));
        }
    }
}
