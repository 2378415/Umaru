using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umaru.Core.Services;

namespace Umaru.Core
{
    public interface IUmaruScript
    {
        public void Run();

        public void Stop();
    }

    public abstract class UmaruScript : IUmaruScript
    {
        public virtual void Run()
        {
            FloatingService.IsRun = true;
        }
        public virtual void Stop()
        {
            FloatingService.IsRun = false;
        }
    }


    public class ScriptUtils
    {

        public static bool IsCanRun()
        {
            if (BarrierService.IsTestiness) return true;
            Tools.Toast("无障碍服务异常-开启服务/重启手机");
            return false;
        }


        public static void RunScript()
        {
            // 获取当前应用程序域中的所有类型
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IUmaruScript).IsAssignableFrom(type) && !type.IsAbstract);

            foreach (var type in types)
            {
                // 创建类型的实例并运行脚本
                if (Activator.CreateInstance(type) is IUmaruScript scriptInstance)
                {
                    scriptInstance.Run();
                }
            }

        }


        public static void StopScript()
        {
            // 获取当前应用程序域中的所有类型
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IUmaruScript).IsAssignableFrom(type) && !type.IsAbstract);

            foreach (var type in types)
            {
                // 创建类型的实例并运行脚本
                if (Activator.CreateInstance(type) is IUmaruScript scriptInstance)
                {
                    scriptInstance.Stop();
                }
            }

        }

    }
}
