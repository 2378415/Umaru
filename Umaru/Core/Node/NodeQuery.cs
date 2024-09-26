using Android.Views.Accessibility;
using Umaru.Core.Services;

namespace Umaru.Core.Node
{
    public static class NodeQuery
    {
        public static AccessibilityNodeInfo[] Pkg(string pkg)
        {
            var nodes = BarrierService.GetNodes();
            return nodes.Where(t => t.PackageName == pkg).ToArray();
        }

        public static AccessibilityNodeInfo[] Pkg(this AccessibilityNodeInfo[] nodes, string pkg)
        {
            return nodes.Where(t => t.PackageName == pkg).ToArray();
        }

        public static AccessibilityNodeInfo[] Id(string id)
        {
            var nodes = BarrierService.GetNodes();
            return nodes.Where(t => t.ViewIdResourceName == id).ToArray();
        }

        public static AccessibilityNodeInfo[] Id(this AccessibilityNodeInfo[] nodes, string id)
        {
            return nodes.Where(t => t.ViewIdResourceName == id).ToArray();
        }

        public static AccessibilityNodeInfo[] Text(string text)
        {
            var nodes = BarrierService.GetNodes();
            return nodes.Where(t => t.Text == text).ToArray();
        }

        public static AccessibilityNodeInfo[] Text(this AccessibilityNodeInfo[] nodes, string text)
        {
            return nodes.Where(t => t.Text == text).ToArray();
        }

		public static AccessibilityNodeInfo[] ClassName(string @class)
		{
			var nodes = BarrierService.GetNodes();
			return nodes.Where(t => t.ClassName == @class).ToArray();
		}

		public static AccessibilityNodeInfo[] ClassName(this AccessibilityNodeInfo[] nodes, string @class)
		{
			return nodes.Where(t => t.ClassName == @class).ToArray();
		}

		public static AccessibilityNodeInfo[] ContentDescription(string description)
		{
			var nodes = BarrierService.GetNodes();
			return nodes.Where(t => t.ContentDescription == description).ToArray();
		}

		public static AccessibilityNodeInfo[] ContentDescription(this AccessibilityNodeInfo[] nodes, string description)
		{
			return nodes.Where(t => t.ContentDescription == description).ToArray();
		}

		public static AccessibilityNodeInfo[] DrawingOrder(int order)
		{
			var nodes = BarrierService.GetNodes();
			return nodes.Where(t => t.DrawingOrder == order).ToArray();
		}

		public static AccessibilityNodeInfo[] DrawingOrder(this AccessibilityNodeInfo[] nodes, int order)
		{
			return nodes.Where(t => t.DrawingOrder == order).ToArray();
		}


		public static bool Tap(this AccessibilityNodeInfo node)
        {
            return node.PerformAction(Android.Views.Accessibility.Action.Click);
        }
    }
}
