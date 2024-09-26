using Android.AccessibilityServices;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Views.Accessibility;

namespace Umaru.Core.Services
{
	[Service(Label = "小埋-无障碍", Permission = "android.permission.BIND_ACCESSIBILITY_SERVICE", Exported = true)]
	[IntentFilter(["android.accessibilityservice.AccessibilityService"])]
	[MetaData("android.accessibilityservice", Resource = "@xml/barrierservice")]
	public class BarrierService : AccessibilityService
	{
		private static List<AccessibilityNodeInfo>? _nodes = null;

		public static BarrierService? Instance = null;

		public static bool IsTestiness { get; set; } = false;

		public override void OnAccessibilityEvent(AccessibilityEvent? e)
		{
			//GetAllNodes();
			//Tools.Toast("收集节点数据");
		}

		public override void OnCreate()
		{
			base.OnCreate();
			IsTestiness = true;
			Instance = this;
			Tools.Toast("小埋无障碍服务已启动");
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			IsTestiness = false;
			Instance = null;
		}


		public static List<AccessibilityNodeInfo> GetNodes()
		{
			Instance?.GetAllNodes();
			// 返回 _nodes 的克隆副本
			return _nodes != null ? new List<AccessibilityNodeInfo>(_nodes) : new List<AccessibilityNodeInfo>();
		}

		private void GetAllNodes()
		{
			var nodes = new List<AccessibilityNodeInfo>();
			var windows = Windows;
			if (windows == null) return;

			foreach (var window in windows)
			{
				var rootNode = window.Root;
				if (rootNode != null)
				{
					var items = new List<AccessibilityNodeInfo>();
					TraverseNode(rootNode, items);
					nodes.AddRange(items);
				}
			}

			_nodes = nodes;
		}

		private static void TraverseNode(AccessibilityNodeInfo node, List<AccessibilityNodeInfo> allNodes)
		{
			if (node == null) return;
			allNodes.Add(node);

			for (int i = 0; i < node.ChildCount; i++)
			{
				var child = node.GetChild(i);
				if (child != null) TraverseNode(child, allNodes);
			}
		}



		public override void OnInterrupt()
		{
			// Handle interrupt
		}

		public void Tap(int x, int y)
		{
			var path = new Android.Graphics.Path();
			path.MoveTo(x, y);
			path.LineTo(x, y); // 添加一条线到同一个点，模拟点击
			var gestureBuilder = new GestureDescription.Builder();
			gestureBuilder.AddStroke(new GestureDescription.StrokeDescription(path, 0, 100));
			var gesture = gestureBuilder.Build();
			if (gesture != null) DispatchGesture(gesture, null, null);
		}

	}
}
