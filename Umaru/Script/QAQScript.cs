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
