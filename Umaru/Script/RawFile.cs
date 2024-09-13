using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umaru.Core.Store;

namespace Umaru.Script
{
    public class RawFile : IRawFile
    {
        public string[] GetFlies()
        {
            return ["test_avatar.jpg", "afind.png", "yolov5s_best.onnx", "ggvb.jpg", "20240429195311.jpg"];
        }
    }
}
