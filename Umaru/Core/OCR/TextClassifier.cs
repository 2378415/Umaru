﻿// Apache-2.0 license
// Adapted from RapidAI / RapidOCR
// https://github.com/RapidAI/RapidOCR/blob/92aec2c1234597fa9c3c270efd2600c83feecd8d/dotnet/RapidOcrOnnxCs/OcrLib/AngleNet.cs

using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SkiaSharp;

namespace Umaru.Core.OCR
{
    public sealed class TextClassifier : IDisposable
    {
        private const int AngleDstWidth = 192;
        private const int AngleDstHeight = 48;
        private const int AngleCols = 2;

        private readonly float[] _meanValues = [127.5F, 127.5F, 127.5F];
        private readonly float[] _normValues = [1.0F / 127.5F, 1.0F / 127.5F, 1.0F / 127.5F];

        private InferenceSession? _angleNet;
        private string _inputName = string.Empty;

        public void InitModel(string path, int numThread)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Classifier model file does not exist: '{path}'.");
            }

            var op = new SessionOptions
            {
                GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_EXTENDED,
                InterOpNumThreads = numThread,
                IntraOpNumThreads = numThread
            };
            _angleNet = new InferenceSession(path, op);
            _inputName = _angleNet.InputMetadata.Keys.First();
        }

        public Angle[] GetAngles(SKBitmap[] partImgs, bool doAngle, bool mostAngle)
        {
            var angles = new Angle[partImgs.Length];
            if (doAngle)
            {
                for (int i = 0; i < partImgs.Length; i++)
                {
                    angles[i] = GetAngle(partImgs[i]);
                }

                // Most Possible AngleIndex
                if (mostAngle)
                {
                    double sum = angles.Sum(x => x.Index);
                    double halfPercent = angles.Length / 2.0f;

                    int mostAngleIndex = sum < halfPercent ? 0 : 1; // All angles set to 0 or 1
                    System.Diagnostics.Debug.WriteLine($"Set All Angle to mostAngleIndex({mostAngleIndex})");
                    foreach (var angle in angles)
                    {
                        angle.Index = mostAngleIndex;
                    }
                }
            }
            else
            {
                for (int i = 0; i < partImgs.Length; i++)
                {
                    angles[i] = new Angle
                    {
                        Index = -1,
                        Score = 0F
                    };
                }
            }

            return angles;
        }

        public Angle GetAngle(SKBitmap src)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            Tensor<float> inputTensors;
            using (var angleImg = src.Resize(new SKSizeI(AngleDstWidth, AngleDstHeight), SKFilterQuality.High))
            {
                inputTensors = OcrUtils.SubtractMeanNormalize(angleImg, _meanValues, _normValues);
            }

            IReadOnlyCollection<NamedOnnxValue> inputs = new NamedOnnxValue[]
            {
                NamedOnnxValue.CreateFromTensor(_inputName, inputTensors)
            };

            try
            {
                if (_angleNet != null)
                {
                    using (IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = _angleNet.Run(inputs))
                    {
                        ReadOnlySpan<float> outputData = results[0].AsEnumerable<float>().ToArray();
                        var angle = ScoreToAngle(outputData, AngleCols);
                        angle.Time = sw.ElapsedMilliseconds;
                        return angle;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + ex.StackTrace);
                //throw;
            }

            return new Angle() { Time = sw.ElapsedMilliseconds };
        }

        private static Angle ScoreToAngle(ReadOnlySpan<float> srcData, int angleColumns)
        {
            int angleIndex = 0;
            float maxValue = srcData[0];

            for (int i = 1; i < angleColumns; ++i)
            {
                float current = srcData[i];
                if (current > maxValue)
                {
                    angleIndex = i;
                    maxValue = current;
                }
            }

            return new Angle
            {
                Index = angleIndex,
                Score = maxValue
            };
        }

        public void Dispose()
        {
            _angleNet?.Dispose();
        }
    }
}
