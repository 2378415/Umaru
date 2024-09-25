using Android.Graphics;
using Android.Hardware.Lights;
using Android.Media.Projection;
using Android.Media;
using Android.Util;
using Android.Views;
using Microsoft.Maui.Media;
using System.IO;
using System.Threading.Tasks;
using Umaru.Core.OpenCV;
using Umaru.Core.Services;
using static Android.Icu.Text.ListFormatter;
using Color = Android.Graphics.Color;
using Window = Android.Views.Window;
using Android.Content;
using Android.App;
using Stream = System.IO.Stream;
using System.Diagnostics;
using Google.Android.Material.Shape;
using static Android.AccessibilityServices.AccessibilityService;
using Application = Android.App.Application;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV;
using Point = System.Drawing.Point;
using System.Drawing;
using Size = System.Drawing.Size;
using Bitmap = Android.Graphics.Bitmap;

[assembly: Dependency(typeof(SuperImage))]
namespace Umaru.Core.OpenCV
{
    public class SuperImage 
    {
        public static Point PointEmpty = new Point(-1, -1);

        public static Bitmap ConvertToPng(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                // 将 Bitmap 压缩为 PNG 格式并写入内存流
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
                stream.Seek(0, SeekOrigin.Begin);

                // 从内存流中读取 PNG 格式的 Bitmap
                return BitmapFactory.DecodeStream(stream);
            }
        }

        public static void SaveToFile(Bitmap bitmap, string filePath)
        {
            var tempPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, filePath);
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            }
        }

        private static Bitmap ReadFileByRoot(string path)
        {
            try
            {
                var tempPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, path);
                // 读取文件内容的命令
                string readCommand = $"cat {tempPath}";

                // 使用 su 命令执行读取文件内容
                Process process = new Process();
                process.StartInfo.FileName = "su";
                process.StartInfo.Arguments = "-c \"" + readCommand + "\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                // 获取文件内容
                using (var memoryStream = new MemoryStream())
                {
                    process.StandardOutput.BaseStream.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return BitmapFactory.DecodeStream(memoryStream);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }

		public static byte[] BitmapToByteArray(Bitmap bitmap)
		{
			using (var stream = new MemoryStream())
			{
				// 将 Bitmap 压缩为 PNG 格式并写入内存流
				bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
				return stream.ToArray();
			}
		}

		public static Bitmap Capture(int x, int y, int w, int h)
        {
            var tempPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, $"Capture_{Guid.NewGuid()}.png");
            RootUtils.Screencap(tempPath);

            if (File.Exists(tempPath))
            {    // Open the source file
                var img = ReadFileByRoot(tempPath);

                var croppedImg = Bitmap.CreateBitmap(img, x, y, w, h);
				File.Delete(tempPath);
				return croppedImg;
            }

            return null;
        }

        public static void Capture(string path)
        {
            try
            {
                // 截图命令
                string command = $"/system/bin/screencap -p {System.IO.Path.Combine(FileSystem.AppDataDirectory, path)}";

                // 使用 su 命令执行截图
                Process process = new Process();
                process.StartInfo.FileName = "su";
                process.StartInfo.Arguments = "-c \"" + command + "\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                // 等待命令执行完成
                process.WaitForExit();

                // 检查命令执行结果
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.WriteLine("Exception: " + error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
        }

        /// <summary>
        /// 使用CcorrNormed 查找图片，低于0.9都不能信
        /// </summary>
        /// <param name="x">左上角X</param>
        /// <param name="y">左上角Y</param>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        /// <param name="pic_name">图片路径</param>
        /// <param name="sim">相似度0-1</param>
        /// <returns></returns>
        public static Point FindPic(int x, int y, int w, int h, string pic_name, float sim)
        {
            var screen = Capture(x, y, w, h);
            var pic = ReadFileByRoot(pic_name);

            // 将 Bitmap 转换为 Emgu.CV 的 Image<Bgr, Byte>
            Image<Bgr, byte> screenImage = screen.ToImage<Bgr, byte>();
            Image<Bgr, byte> templateImage = pic.ToImage<Bgr, byte>();


            // 创建掩码图像，过滤掉 FF00FF 颜色
            Image<Gray, byte> mask = CreateMask(templateImage.Clone(), new Bgr(255, 0, 255));

            // 进行模板匹配
            using (var result = new Mat())
            {
                //CcoeffNormed 对黑色背景不兼容
                CvInvoke.MatchTemplate(screenImage, templateImage, result, TemplateMatchingType.CcorrNormed, mask);

                double minVal = 0.0, maxVal = 0.0;
                Point minLoc = new Point(-1, -1), maxLoc = new Point(-1, -1);
                CvInvoke.MinMaxLoc(result, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

                // 检查最大相似度是否大于等于指定的相似度
                if (maxVal >= sim && maxVal <= 1)
                {
                    // 在匹配到的位置绘制矩形框
                    Rectangle matchRect = new Rectangle(maxLoc, templateImage.Size);
                    screenImage.Draw(matchRect, new Bgr(0, 150, 136), 2);

                    // 将结果图像保存到本地
                    SaveToFile(screenImage.ToBitmap(), "MatchResult.png");
                    return new Point(maxLoc.X + x, maxLoc.Y + y);
                }
            }

            return PointEmpty;
        }

		private static Image<Gray, byte> CreateMask(Image<Bgr, byte> image, Bgr transparentColor)
        {
            Image<Gray, byte> mask = new Image<Gray, byte>(image.Width, image.Height, new Gray(1));
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Bgr color = image[y, x];
                    if (color.Equals(transparentColor))
                    {
                        mask[y, x] = new Gray(0); // 透明色位置标记为0
                    }
                }
            }
            return mask;
        }
    }
}
