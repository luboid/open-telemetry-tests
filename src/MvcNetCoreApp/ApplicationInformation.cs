using System.Reflection;

namespace MvcNetCoreApp
{
    public static class ApplicationInformation
    {
        static ApplicationInformation()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            Name = assemblyName.Name;
            Version = assemblyName.Version;
        }

        public static string Name { get; }

        public static Version Version { get; }
    }
}
