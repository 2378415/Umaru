using System.IO;
using System.Linq;
using System.Reflection;
using Emgu.CV;
using Path = System.IO.Path;

namespace Umaru.Core.Store
{
    public static class RawUtils
    {
        public static List<string> Files { get; private set; } = new List<string>();

        public static void WriteLocal()
        {
            var rootPath = FileSystem.AppDataDirectory;
            // 获取当前应用程序域中的所有类型
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IRawFile).IsAssignableFrom(type) && !type.IsAbstract);

            foreach (var type in types)
            {
                // 创建类型的实例并运行脚本
                if (Activator.CreateInstance(type) is IRawFile instance)
                {
                    var items = instance.GetFlies();
                    Files.AddRange(items);
                }
            }

            foreach (var file in Files)
            {
                Write(rootPath, file);
            }
        }

        private static void Write(string rootPath, string resourceName)
        {
            string outputPath = Path.Combine(rootPath, resourceName);

            if (!File.Exists(outputPath))
            {
                using (Stream resourceStream = FileSystem.OpenAppPackageFileAsync(resourceName).Result)
                using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }
        }

        public static async Task WriteLocalAsync()
        {
            var rootPath = FileSystem.AppDataDirectory;
            // 获取当前应用程序域中的所有类型
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IRawFile).IsAssignableFrom(type) && !type.IsAbstract);

            foreach (var type in types)
            {
                // 创建类型的实例并运行脚本
                if (Activator.CreateInstance(type) is IRawFile instance)
                {
                    var items = instance.GetFlies();
                    Files.AddRange(items);
                }
            }

            var writeTasks = Files.Select(file => WriteAsync(rootPath, file));
            await Task.WhenAll(writeTasks);
        }

        private static async Task WriteAsync(string rootPath, string resourceName)
        {
            string outputPath = Path.Combine(rootPath, resourceName);
            if (!File.Exists(outputPath))
            {
                using (Stream resourceStream = await FileSystem.OpenAppPackageFileAsync(resourceName))
                using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
                {
                    await resourceStream.CopyToAsync(fileStream);
                }
            }
        }
    }
}
