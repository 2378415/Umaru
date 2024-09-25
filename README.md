# Umaru
Umaru 是一个脚本框架，基于.Net Maui开发，目前仅支持Android平台
## 开发进度
- [x] 基础框架搭建
- [x] 脚本执行器
- [x] 文件系统-释放资源文件
- [x] 本地存储-SQLite
- [x] Yolo支持-YoloV5
- [x] 节点视图-NodeQuery
- [x] 找图功能-FindPic
- [x] OCR功能-PaddleOCR-v4
- [ ] 找色功能
## 使用说明
1. 下载项目源码，解压
2. 使用VS2022打开Umaru.sln，编译项目
3. 运行Umaru.App项目
4. 个人脚本文件放在Umaru.App/Scripts目录下
5. 脚本执行类继承UmaruScript并重载Run，Stop函数
6. 资源文件放在Umaru.App/Resources/Raw目录下并创建文件说明类继承IRawFile接口并实现GetFlies函数
7. Core文件夹属于框架核心部分，不建议修改
## 脚本示例
```csharp
using Umaru.Core;
using Umaru.Core.Node;

namespace Umaru.Script
{
	public class QAQScript : UmaruScript
	{
		public override void Run()
		{
			base.Run();



			ScriptMain();

		}

		public override void Stop()
		{
			base.Stop();
			//CloseAll();
			Tools.Toast("脚本终止");
		}

		public void CloseAll()
		{
			Tools.CloseApp("indi.sky.fireworks");
			Tools.CloseApp("com.netease.sky");
		}

		public void ScriptMain()
		{

			var nodes = NodeQuery.Pkg("indi.sky.fireworks").Id("indi.sky.fireworks:id/hot_frame").ToList();
			if (nodes.Count > 0)
			{
				Thread.Sleep(500);
				nodes.FirstOrDefault()?.Tap();
				Tools.Toast("找到啦");
			}


			var a = 1;
		}
	}
}
```

## 文件示例
```csharp
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
```

## OCR示例
```csharp
	//参数1 模型 参数2 模型对应的字典
    var model = new YoloModel("xxx.onnx",new Dictionary<int, string> { });
    var yolo = new SuperYolo(model);
```

## OCR示例
```csharp
	var ocr = new PaddleOCR();
	var result = ocr.Recognize(87, 458, 143 - 87, 496 - 458);
```