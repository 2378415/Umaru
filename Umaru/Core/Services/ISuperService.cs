using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umaru.Core.Services
{
    public interface ISuperService
    {
        void LaunchApp(string packageName);

        void CloseApp(string packageName);

        void Tap(int x, int y);

		void Swipe(int x1, int y1, int x2, int y2, int duration = 500);

		void Roll(int index, int count);

	    void KeyEvent(string @event);

		void Toast(string message);

		void ToHomePage();

	}
}
