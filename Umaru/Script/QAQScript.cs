using Umaru.Core;
using Umaru.Core.Node;
using Umaru.Core.OCR;

namespace Umaru.Script
{
	public class QAQScript : UmaruScript
	{
		public override void Run()
		{
			base.Run();
			Tools.Toast("脚本启动");
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
			//Tools.CloseApp("indi.sky.fireworks");
			//Tools.CloseApp("com.netease.sky");
		}

		public void ScriptMain()
		{

			var ocr = new PaddleOCR();
			var result = ocr.Recognize(87, 458, 143 - 87, 496 - 458);
			var a = 1;
		}
	}
}
