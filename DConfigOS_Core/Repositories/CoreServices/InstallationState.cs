using NuGet;

namespace DConfigOS_Core.Layer1.CoreServices
{
    internal class InstallationState
    {
        public IPackage Installed { get; set; }

        public IPackage Update { get; set; }
    }
}
