using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umaru.Core.Yolo
{
	public class YoloModel
	{
		public string ModelPath { get; set; }

		public Dictionary<int, string> ClassNames { get; set; }

		public YoloModel(string modelPath, Dictionary<int, string> classNames)
		{
			this.ModelPath = modelPath;
			this.ClassNames = classNames;
		}

		public string GetClassName(int index)
		{
			try
			{
				return this.ClassNames[index];
			}
			catch
			{
				return string.Empty;
			}
		}
	}
}
