using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umaru.Core;
using Umaru.Core.Node;
using Umaru.Core.OpenCV;
using System.Reflection;
using Umaru.Core.Http;
using Umaru.Core.Yolo;
using Umaru.Core.Store;

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
			//         CloseAll();
			//Thread.Sleep(500);
			//Tools.Toast("脚本启动");
			//         Tools.LaunchApp("indi.sky.fireworks");
			//Thread.Sleep(200);
			//Tools.LaunchApp("indi.sky.fireworks");
			//Thread.Sleep(500);
			//Tools.LaunchApp("com.netease.sky");
			//Thread.Sleep(200);
			//Tools.LaunchApp("com.netease.sky");
			//Thread.Sleep(500);
			//Tools.Toast("脚本结束");

			//var nodes = NodeQuery.Pkg("indi.sky.fireworks").Id("indi.sky.fireworks:id/hot_frame").ToList();
			//if (nodes.Count > 0)
			//{
			//	Thread.Sleep(500);
			//             nodes.FirstOrDefault()?.Tap();
			//	Tools.Toast("找到啦");
			//}
			//var a = 1;

			//var data = SuperHttp.Post("https://www.baidu.com/");

			//var model = new YoloModel("yolov5s_best.onnx", new Dictionary<int, string>()
			//{
			//	{ 0, "self" },
			//	{ 1, "target" },
			//	{ 2, "foe" }
			//});
			//var yolo = new SuperYolo(model);
			//var result = yolo.Detect(0, 0, 700, 1200);

			//Tools.Swipe(345, 999, 345, 323, 1000);
			//var a = 1;


			//var inst = SuperSqlite.Instance;
			//inst.SaveItem(new SqliteModel() { Key = "User", Value = "Admin" });

			//var data = inst.GetItem("User");
			var a = 1;
		}
	}
}
