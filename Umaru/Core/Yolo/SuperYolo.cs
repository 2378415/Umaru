using Android.Graphics;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Drawing;
using Umaru.Core.OpenCV;
using Bitmap = Android.Graphics.Bitmap;
using Path = System.IO.Path;

namespace Umaru.Core.Yolo
{
	public class SuperYolo
	{
		private InferenceSession _session;

		private YoloModel _model;

		public SuperYolo(YoloModel model)
		{
			_model = model;
			var onnxPath = Path.Combine(FileSystem.AppDataDirectory, _model.ModelPath);

			// 加载 YOLOv5 ONNX 模型
			_session = new InferenceSession(onnxPath);
		}

		/// <summary>
		/// 只能加载Raw释放的资源
		/// </summary>
		/// <param name="imagePath"></param>
		/// <returns></returns>
		Bitmap? LoadImage(string imagePath)
		{
			imagePath = Path.Combine(FileSystem.AppDataDirectory, imagePath);
			return BitmapFactory.DecodeFile(imagePath);
		}

		public List<Detection> Detect(int x, int y, int w, int h)
		{
			var image = SuperImage.Capture(x, y, w, h);
			if (image == null) return new List<Detection>();
			return Detect(image);
		}

		public List<Detection> Detect(string imagePath)
		{
			var image = LoadImage(imagePath);
			if (image == null) return new List<Detection>();
			return Detect(image);
		}

		public List<Detection> Detect(Bitmap image)
		{
			//var image = LoadImage(imagePath);
			var originalWidth = image.Width;
			var originalHeight = image.Height;
			// 将图片数据转换为模型输入格式
			var (inputTensor, scale, padX, padY) = PreprocessImage(image);

			// 获取模型输入名称
			var inputName = _session.InputMetadata.Keys.First();
			// 创建输入容器
			var inputs = new List<NamedOnnxValue>
			{
				NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
			};

			// 运行推理
			using var results = _session.Run(inputs);

			// 处理输出
			var outputName = _session.OutputMetadata.Keys.First();
			var outputTensor = results.First(v => v.Name == outputName).AsTensor<float>();

			// 解析输出
			var detections = ParseOutput(outputTensor, scale, padX, padY);
			return detections;
		}


		/// <summary>
		/// 处理图片数据，转换为模型输入格式
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		static (DenseTensor<float>, float, float, float) PreprocessImage(Bitmap image)
		{
			int targetWidth = 640;
			int targetHeight = 640;
			float scale = Math.Min((float)targetWidth / image.Width, (float)targetHeight / image.Height);
			int newWidth = (int)(image.Width * scale);
			int newHeight = (int)(image.Height * scale);
			int padX = (targetWidth - newWidth) / 2;
			int padY = (targetHeight - newHeight) / 2;

			var tensor = new DenseTensor<float>(new[] { 1, 3, targetHeight, targetWidth });

			using (var resized = Bitmap.CreateScaledBitmap(image, newWidth, newHeight, false))
			{
				int[] pixels = new int[newWidth * newHeight];
				resized.GetPixels(pixels, 0, newWidth, 0, 0, newWidth, newHeight);

				for (int y = 0; y < newHeight; y++)
				{
					for (int x = 0; x < newWidth; x++)
					{
						int pixel = pixels[y * newWidth + x];
						float red = ((pixel >> 16) & 0xFF) / 255.0f;
						float green = ((pixel >> 8) & 0xFF) / 255.0f;
						float blue = (pixel & 0xFF) / 255.0f;

						tensor[0, 0, y + padY, x + padX] = red;
						tensor[0, 1, y + padY, x + padX] = green;
						tensor[0, 2, y + padY, x + padX] = blue;
					}
				}
			}


			return (tensor, scale, padX, padY);
		}


		List<Detection> ParseOutput(Tensor<float> outputTensor, float scale, float padX, float padY, float iouThreshold = 0.45f)
		{
			var detections = new List<Detection>();

			int numDetections = outputTensor.Dimensions[1];
			int numClasses = outputTensor.Dimensions[2] - 5;

			for (int i = 0; i < numDetections; i++)
			{
				float confidence = outputTensor[0, i, 4];
				if (confidence > 0.5)
				{
					float x = (outputTensor[0, i, 0] - padX) / scale;
					float y = (outputTensor[0, i, 1] - padY) / scale;
					float w = outputTensor[0, i, 2] / scale;
					float h = outputTensor[0, i, 3] / scale;

					x = x - (w / 2);
					y = y - (h / 2);

					float maxClassScore = 0;
					int maxClassIndex = -1;
					for (int j = 0; j < numClasses; j++)
					{
						float classScore = outputTensor[0, i, 5 + j];
						if (classScore > maxClassScore)
						{
							maxClassScore = classScore;
							maxClassIndex = j;
						}
					}

					detections.Add(new Detection
					{
						Class = maxClassIndex,
						ClassName = _model.GetClassName(maxClassIndex),
						Sim = confidence,
						Box = new RectangleF(x, y, w, h)
					});
				}
			}

			return NonMaxSuppression(detections, iouThreshold);
		}

		List<Detection> NonMaxSuppression(List<Detection> detections, float iouThreshold)
		{
			var result = new List<Detection>();

			var sortedDetections = detections.OrderByDescending(d => d.Sim).ToList();

			while (sortedDetections.Count > 0)
			{
				var bestDetection = sortedDetections[0];
				result.Add(bestDetection);
				sortedDetections.RemoveAt(0);

				sortedDetections = sortedDetections
					.Where(d => IoU(bestDetection.Box, d.Box) < iouThreshold)
					.ToList();
			}

			return result;
		}

		float IoU(RectangleF box1, RectangleF box2)
		{
			float intersectionArea = RectangleF.Intersect(box1, box2).Area();
			float unionArea = box1.Area() + box2.Area() - intersectionArea;
			return intersectionArea / unionArea;
		}
	}

	static class RectangleFExtensions
	{
		public static float Area(this RectangleF rect)
		{
			return rect.Width * rect.Height;
		}
	}

	public class Detection
	{
		public int Class { get; set; }

		public string? ClassName { get; set; }

		public float Sim { get; set; }

		public RectangleF Box { get; set; }
	}
}
