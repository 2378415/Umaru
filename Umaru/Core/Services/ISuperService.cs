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

        void Toast(string message);
    }
}
