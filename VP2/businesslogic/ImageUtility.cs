
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Security.AccessControl;

namespace VP2.businesslogic
{
    public class ImageUtility
    {

        public static Size ImageDimension(string imageFile)
        {
            Size dimensions = new Size(0, 0);

            using (Image srcImage = System.Drawing.Image.FromFile(imageFile))
            {
                dimensions = srcImage.Size;
            }
            return dimensions;
        }

        /// <summary>
        /// Resize an image file and save to the specified location 
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="maxDimension"></param>
        public static void ResizeAndSave(string imageFile, string outputFile, int maxDimension)
        {
            using (var srcImage = System.Drawing.Image.FromFile(imageFile))
            {
                short rotation = 1;

                // If the image was taken with a rotated camera it will be stored in the default orientation
                //  but there will be an extended property that describes the cameras rotation (you need an exif viewer to see this)
                foreach (var prop in srcImage.PropertyItems)
                {
                    if (prop.Id == 0x0112)
                    {
                        rotation = prop.Value[0];
                    }
                }
                // 6 indicates that the camera was rotated 90 deg clockwise, so that's what we do with the image
                if (rotation == 6)
                {
                    srcImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }
                //90 deg anticlockwise
                if (rotation == 8)
                {
                    srcImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                }
                // Upside-down!?
                if (rotation == 3)
                {
                    srcImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                }

                // Calculte the scale factor.
                // First Orientation:
                // 1. Landscape, Height < Width (correct)
                //    - Scale the height to the desired value
                // 2. Portrait, Height > Width (naughty)
                //    - Scale the width to the desired value
                int newWidth;
                int newHeight;

                if (srcImage.Height <= srcImage.Width)
                {
                    if (srcImage.Height > maxDimension)
                    {
                        newHeight = maxDimension;
                        newWidth = (int)(srcImage.Width * ((double)maxDimension / (double)srcImage.Height));
                    }
                    else
                    {
                        newHeight = srcImage.Height;
                        newWidth = srcImage.Width;
                    }
                }
                else
                {
                    if (srcImage.Width > maxDimension)
                    {
                        newWidth = maxDimension;
                        newHeight = (int)(srcImage.Height * ((double)maxDimension / (double)srcImage.Width));
                    }
                    else
                    {
                        newHeight = srcImage.Height;
                        newWidth = srcImage.Width;
                    }
                }


                ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 60L);
                myEncoderParameters.Param[0] = myEncoderParameter;


                using (var newImage = new Bitmap(newWidth, newHeight))
                {
                    using (var graphics = Graphics.FromImage(newImage))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.InterpolationMode = InterpolationMode.Default;
                        graphics.PixelOffsetMode = PixelOffsetMode.Default;
                        graphics.DrawImage(srcImage, new Rectangle(0, 0, newWidth, newHeight));
                        newImage.Save(outputFile, System.Drawing.Imaging.ImageFormat.Jpeg);
                        SetEveryoneAccess(outputFile);
                    }
                }
            }
        }

        public static void SetEveryoneAccess(string filePath)
        {
            FileInfo fInfo = new FileInfo(filePath);
            FileSecurity fSecurity = fInfo.GetAccessControl();
            fSecurity.AddAccessRule(new FileSystemAccessRule("everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            fInfo.SetAccessControl(fSecurity);
            //    return true;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }




        /// <summary>
        /// Returns the raw byte data of an image by saving it to a memory stream and extracting the 
        /// byte data from there
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] data = ms.ToArray();
            ms.Close();
            ms.Dispose();
            return data;
        }

        /// <summary>
        /// Returns the raw byte data of an image by saving it to a memory stream and extracting the 
        /// byte data from there
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(string filePath)
        {
            Image img = Image.FromFile(filePath);
            return ImageToByteArray(img);
        }

        public static string GetUniqueImageFileName()
        {
            Guid g = Guid.NewGuid();
            return g.ToString();
        }

    }
}
