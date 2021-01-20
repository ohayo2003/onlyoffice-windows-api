using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WebApi.Utility
{
    public static class BitmapHelper
    {
        //图片转为base64编码的字符串
        public static string ImgToBase64String(Bitmap bmp)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //threeebase64编码的字符串转为图片
        public static Bitmap Base64StringToImage(string strbase64)
        {
            try
            {
                byte[] arr = Convert.FromBase64String(strbase64);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                ms.Close();
                return bmp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 压缩图片并保存，文件名为compress_+原文件名
        /// </summary>
        /// <param name="filePath">原图路径</param>
        /// <returns>缩略图路径</returns>
        public static string GetReducedImage(string filePath)
        {
            //string fileDir = AppDomain.CurrentDomain.BaseDirectory;
            //if (!Directory.Exists(fileDir))
            //{
            //    Directory.CreateDirectory(fileDir);
            //}

            //string compressName = new StringBuilder(fileDir).Append("\\compress_").Append(Path.GetFileName(filePath)).ToString();

            int minSize = 200;

            Graphics draw = null;
            System.Drawing.Image ResourceImage = System.Drawing.Image.FromFile(filePath);
            double percent = 0.4;
            int imageWidth = Convert.ToInt32(ResourceImage.Width);
            int imageHeight = Convert.ToInt32(ResourceImage.Height);
            if (imageWidth > imageHeight)
            {
                if (imageWidth > minSize)
                {
                    percent = Convert.ToDouble(minSize) / imageWidth;
                    imageWidth = minSize;
                    imageHeight = (int)(imageHeight * percent);
                }
            }
            else
            {
                if (imageHeight > minSize)
                {
                    percent = Convert.ToDouble(minSize) / imageHeight;
                    imageHeight = minSize;
                    imageWidth = (int)(imageWidth * percent);
                }
            }

            // 新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(imageWidth, imageHeight);
            System.Drawing.Image bitmap2 = new System.Drawing.Bitmap(imageWidth, imageHeight);
            // 新建一个画板
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            try
            {
                // 设置高质量插值法
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                // 设置高质量,低速度呈现平滑程度
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                // 清空画布并以透明背景色(白色)填充
                g.Clear(System.Drawing.Color.White);
                //g.Clear(System.Drawing.Color.Transparent);
                // 在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(ResourceImage, new System.Drawing.Rectangle(0, 0, imageWidth, imageHeight));

                // 用新建立的image对象拷贝bitmap对象 让g对象可以释放资源
                draw = Graphics.FromImage(bitmap2);
                draw.DrawImage(bitmap, 0, 0);

                // 设置缩略图编码格式
                ImageCodecInfo ici = null;
                if (Path.GetExtension(filePath).Equals(".png") || Path.GetExtension(filePath).Equals(".PNG"))
                {
                    //ici = this.getImageCoderInfo("image/png");
                    ici = getImageCoderInfo("image/jpeg");
                }
                else if (Path.GetExtension(filePath).Equals(".gif") || Path.GetExtension(filePath).Equals(".GIF"))
                {
                    ici = getImageCoderInfo("image/gif");
                }
                else
                {
                    ici = getImageCoderInfo("image/jpeg");
                }

                // 设置压缩率
                long ratio = 20L; // 压缩为原图20%的质量

                System.Drawing.Imaging.Encoder ecd = System.Drawing.Imaging.Encoder.Quality;

                ResourceImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
                draw.Dispose();

                // 保存调整在这里即可
                //using (EncoderParameters eptS = new EncoderParameters(1))
                //{
                //    using (EncoderParameter ept = new EncoderParameter(ecd, ratio))
                //    {
                //        eptS.Param[0] = ept;
                //        bitmap2.Save(compressName, ici, eptS);
                //    }
                //}

                return ImgToBase64String((Bitmap)bitmap2);
            }
            catch (System.Exception e)
            {
                //compressName = string.Empty;
                return "";
            }
            finally
            {
                if (ResourceImage != null)
                {
                    ResourceImage.Dispose();
                }

                if (bitmap != null)
                {
                    bitmap.Dispose();
                }

                if (g != null)
                {
                    g.Dispose();
                }

                if (bitmap2 != null)
                {
                    bitmap2.Dispose();
                }

                if (draw != null)
                {
                    draw.Dispose();
                }
            }

            //return compressName;
        }

        /// <summary>
        /// 获取图片编码
        /// </summary>
        /// <param name="coderType">编码格式：image/png、image/jpeg等</param>
        /// <returns></returns>
        private static ImageCodecInfo getImageCoderInfo(string coderType)
        {
            ImageCodecInfo[] iciS = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo retIci = null;
            foreach (ImageCodecInfo ici in iciS)
            {
                if (ici.MimeType.Equals(coderType))
                {
                    retIci = ici;
                }
            }

            return retIci;
        }
    }

}
