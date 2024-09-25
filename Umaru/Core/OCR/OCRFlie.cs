using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umaru.Core.Store;

namespace Umaru.Core.OCR
{
	public class OCRFlie : IRawFile
	{
		public string[] GetFlies()
		{
			return ["ch_ppocr_mobile_v2.0_cls_train.onnx", "ch_PP-OCRv4_det_infer.onnx", "ch_PP-OCRv4_rec_infer.onnx", "rec_word_dict.txt"];
		}
	}
}
