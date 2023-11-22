using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Build.Construction;
using System.Runtime.Versioning;
using NuGet;
using NuGet.Common;
using FubuCore;
using IFileSystem = NuGet.IFileSystem;

namespace DConfigOS_Core.Layer1.CoreServices
{
    public class CoreManager
    {
        private readonly IProjectManager _coreManager;
        private readonly MSBuildProjectSystem _projectSystem;

        // Methods
        public CoreManager(string remoteSource, string siteRoot)
        {
            string webRepositoryDirectory = GetWebRepositoryDirectory(siteRoot);
            var sourceRepository = PackageRepositoryFactory.Default.CreateRepository(remoteSource);
            var packagesFolderFileSystem = new PhysicalFileSystem(siteRoot + "Packages");
            var pathResolver = new DefaultPackagePathResolver(siteRoot + "Packages");
            _projectSystem = new MSBuildProjectSystem(siteRoot + "WebApplication2.csproj") { Logger = new ErrorLogger() };
            
            var localRepository = new LocalPackageRepository(pathResolver, packagesFolderFileSystem);
            _coreManager = new ProjectManager(sourceRepository, pathResolver, _projectSystem, localRepository);

            _coreManager.PackageReferenceAdded += (sender, args) => args.Package.GetLibFiles().Each(file => SaveAssemblyFile(args.InstallPath, file));
        }

        private void SaveAssemblyFile(string installPath, IPackageFile file)
        {
            var targetPath = installPath.AppendPath(file.Path);
            Directory.CreateDirectory(targetPath.ParentDirectory());
            using (Stream outputStream = File.Create(targetPath))
            {
                file.GetStream().CopyTo(outputStream);
            }

        }

        public IQueryable<IPackage> GetInstalledPackages(string searchTerms)
        {
            return GetPackages(LocalRepository, searchTerms);
        }

        private static IEnumerable<IPackage> GetPackageDependencies(IPackage package, IPackageRepository localRepository, IPackageRepository sourceRepository)
        {
            var walker = new InstallWalker(
                localRepository,
                sourceRepository,
                new FrameworkName(".NET Framework, Version=4.5"),
                NullLogger.Instance,
                ignoreDependencies: false,
                allowPrereleaseVersions: true, dependencyVersion: DependencyVersion.Highest);
            return (from operation in walker.ResolveOperations(package)
                    where operation.Action == PackageAction.Install
                    select operation.Package);
        }

        internal static IQueryable<IPackage> GetPackages(IPackageRepository repository, string searchTerm)
        {
            return GetPackages(repository.GetPackages(), searchTerm);
        }

        internal static IQueryable<IPackage> GetPackages(IQueryable<IPackage> packages, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                packages = packages.Find(searchTerm);
            }
            return packages;
        }

        internal IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance(IPackage package)
        {
            IPackageRepository localRepository = LocalRepository;
            IPackageRepository sourceRepository = SourceRepository;
            return GetPackagesRequiringLicenseAcceptance(package, localRepository, sourceRepository);
        }

        internal static IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance(IPackage package, IPackageRepository localRepository, IPackageRepository sourceRepository)
        {
            return (from p in GetPackageDependencies(package, localRepository, sourceRepository)
                    where p.RequireLicenseAcceptance
                    select p);
        }

        public IQueryable<IPackage> GetPackagesWithUpdates(string searchTerms)
        {
            return GetPackages(LocalRepository.GetUpdates(
                SourceRepository.GetPackages(),
                includePrerelease: true, includeAllVersions:true)
            .AsQueryable(), searchTerms);
        }

        public IQueryable<IPackage> GetRemotePackages(string searchTerms)
        {
            return GetPackages(SourceRepository, searchTerms);
        }

        public IPackage GetUpdate(IPackage package)
        {
            return SourceRepository.GetUpdates(
                LocalRepository.GetPackages(),
                includePrerelease: true, includeAllVersions:true)
            .FirstOrDefault(p => (package.Id == p.Id));
        }

        internal static string GetWebRepositoryDirectory(string siteRoot)
        {
            return Path.Combine(siteRoot, "App_Data", "packages");
        }

        public IEnumerable<string> InstallPackage(IPackage package)
        {
            return PerformLoggedAction(
                () => _coreManager.AddPackageReference(
                    package,
                    ignoreDependencies: false,
                    allowPrereleaseVersions: true));
        }

        public bool IsPackageInstalled(IPackage package)
        {
            return LocalRepository.Exists(package);
        }

        private IEnumerable<string> PerformLoggedAction(Action action)
        {
            var logger = new ErrorLogger();
            _coreManager.Logger = logger;
            try
            {
                action();
                _projectSystem.Save();
            }
            finally
            {
                _coreManager.Logger = null;
            }
            return logger.Errors;
        }

        public IEnumerable<string> UninstallPackage(IPackage package, bool removeDependencies)
        {
            return PerformLoggedAction(
                () =>
                    _coreManager.RemovePackageReference(package,
                        forceRemove: false,
                        removeDependencies: removeDependencies));
        }

        public IEnumerable<string> UpdatePackage(IPackage package)
        {
            return PerformLoggedAction(
                () =>
                    _coreManager.UpdatePackageReference(
                        package.Id,
                        package.Version,
                        updateDependencies: true,
                        allowPrereleaseVersions: true));
            
        }

        // Properties
        public IPackageRepository LocalRepository
        {
            get
            {
                return _coreManager.LocalRepository;
            }
        }

        public IPackageRepository SourceRepository
        {
            get
            {
                return _coreManager.SourceRepository;
            }
        }

        // Nested Types
        private class ErrorLogger : ILogger
        {
            // Fields
            private readonly IList<string> _errors = new List<string>();

            // Methods
            public void Log(MessageLevel level, string message, params object[] args)
            {
                if (level == MessageLevel.Warning)
                {
                    _errors.Add(string.Format(CultureInfo.CurrentCulture, message, args));
                }
            }

            public FileConflictResolution ResolveFileConflict(string message)
            {
                throw new NotImplementedException();
            }

            // Properties
            public IEnumerable<string> Errors
            {
                get
                {
                    return _errors;
                }
            }

        }
    }

    public class BetterThanMSBuildProjectSystem : NuGet.Common.MSBuildProjectSystem
    {

        public override void AddFile(string path, System.IO.Stream stream)
        {
            base.AddFile(path, stream);
            var rootElement = ProjectRootElement.Open(ProjectPath);
            rootElement.AddItem("Content", path);
        }

        public string ProjectPath { get; private set; }

        public BetterThanMSBuildProjectSystem(string projectFile)
            : base(projectFile)
        {
            ProjectPath = projectFile;
        }
    }

    // The problem with the default LocalPackageRepository class is that it returns true for its Exists method whenever a package physically exists in a folder
    // So, if a package exists on disk, but isn't referenced by the project, ProjectManager skips it and doesn't add references to the project 
    // Hence I subclassed LocalPackageRepository.
    // Using this class makes sure that we skip a package only if it exists on disk, and all its assemblies are referenced by our project (this doesn't cover content-only packages, like jQuery).
    public class BetterThanLocalPackageRepository : LocalPackageRepository
    {
        private readonly MSBuildProjectSystem _projectSystem;

        public BetterThanLocalPackageRepository(IPackagePathResolver pathResolver, IFileSystem fileSystem, MSBuildProjectSystem projectSystem) : base(pathResolver, fileSystem)
        {
            _projectSystem = projectSystem;
        }


        public override bool Exists(string packageId, SemanticVersion version)
        {
            //if no package file exists, return false
            if (!base.Exists(packageId, version))
                return false;
            //find the package and check whether all its assemblies are referenced
            var package = this.FindPackage(packageId, version);
            return package.AssemblyReferences.All(reference => _projectSystem.ReferenceExists(reference.Name));
        }
    }
}
