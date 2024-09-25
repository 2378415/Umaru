using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umaru.Core.OpenCV;

namespace Umaru.Core.OCR
{
	/// <summary>
	/// 使用PaddleOCR识别图片中的文字
	/// </summary>
	public class PaddleOCR : IDisposable
	{
		RapidOcr _ocrEngin;
		public PaddleOCR()
		{
			_ocrEngin = new RapidOcr();
			_ocrEngin.InitModels();
		}

		/// <summary>
		/// 指定图片路径识别文字
		/// </summary>
		/// <param name="targetImg"></param>
		/// <returns></returns>
		public string Recognize(string targetImg)
		{
			try
			{
				var path = Path.Combine(FileSystem.AppDataDirectory, targetImg);
				using (SKBitmap originSrc = SKBitmap.Decode(path))
				{
					OcrResult ocrResult = _ocrEngin.Detect(originSrc, RapidOcrOptions.Default);
					var result = ocrResult.StrRes.Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "");
					return result;
				}
			}
			catch (Exception ex)
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// 指定屏幕范围识别文字
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="w"></param>
		/// <param name="h"></param>
		/// <returns></returns>
		public string Recognize(int x, int y, int w, int h)
		{
			try
			{
				var image = SuperImage.Capture(x, y, w, h);
				var buffer = SuperImage.BitmapToByteArray(image);
				using (SKBitmap originSrc = SKBitmap.Decode(buffer))
				{
					OcrResult ocrResult = _ocrEngin.Detect(originSrc, RapidOcrOptions.Default);
					var result = ocrResult.StrRes.Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "");
					return result;
				}
			}
			catch (Exception ex)
			{
				return string.Empty;
			}
		}

		public void Dispose()
		{
			if (_ocrEngin != null)
			{
				_ocrEngin.Dispose();
				_ocrEngin = null;
			}
		}
	}
}
