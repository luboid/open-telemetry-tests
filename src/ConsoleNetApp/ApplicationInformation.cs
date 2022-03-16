using System.Reflection;

namespace ConsoleNetApp
{
    public static class ApplicationInformation
    {
        static ApplicationInformation()
        {
            var assemblyName = Assembly.GetEntryAssembly().GetName();
            Name = assemblyName.Name;
            Version = assemblyName.Version;
        }

        public static string Name { get; }

        public static Version Version { get; }
    }
}
