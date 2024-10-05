﻿using Automation.Plugins.Shared;
using Automation.Shared.Base;
using Automation.Shared.Data;
using NuGet;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Data;
using System.Reflection;
using System.Runtime.Versioning;


namespace Automation.Api.Shared
{
    public class LocalPackageManagement : IPackageManagement
    {
        private const string PACKAGE_EXTENSION = ".nupkg";
        private readonly string _folder;
        private readonly SourceRepository _repository;
        private readonly SourceCacheContext _cacheContext;
        private readonly ILogger _logger;

        public LocalPackageManagement(string folder)
        {
            _folder = folder;
            var packageSource = new PackageSource(folder);
            _repository = Repository.Factory.GetCoreV3(packageSource);
            _cacheContext = new SourceCacheContext();
            _logger = NullLogger.Instance;
        }

        public async Task<ListPageWrapper<PackageInfos>> SearchAsync(string name = "", int page = 1, int pageSize = 50)
        {
            var searchResource = await _repository.GetResourceAsync<PackageSearchResource>();
            var searchFilter = new SearchFilter(includePrerelease: true);

            var packages = await searchResource.SearchAsync(
                name,
                searchFilter,
                skip: page * pageSize,
                take: pageSize,
                _logger,
                CancellationToken.None);

            var searchResult = new ListPageWrapper<PackageInfos>()
            {
                Page = page,
                PageSize = pageSize,
                Total = packages.Count()
            };

            foreach (var package in packages)
            {
                searchResult.Data.Add(package.ToPackageInfos());
            }

            return searchResult;
        }

        public async Task<PackageInfos> GetInfosAsync(string packageId, Version? version)
        {
            var findPackageByIdResource = await _repository.GetResourceAsync<FindPackageByIdResource>();

            if (version == null)
            {
                var versions = await findPackageByIdResource.GetAllVersionsAsync(
                    packageId,
                    _cacheContext,
                    _logger,
                    CancellationToken.None);

                var latestVersion = versions.Max();
                using var packageStream = new MemoryStream();
                await findPackageByIdResource.CopyNupkgToStreamAsync(
                    packageId,
                    latestVersion,
                    packageStream,
                    _cacheContext,
                    _logger,
                    CancellationToken.None);

                packageStream.Position = 0;
                using var packageReader = new PackageArchiveReader(packageStream);
                return packageReader.ToPackageInfos();
            }
            else
            {
                var nugetVersion = new NuGetVersion(version);
                using var packageStream = new MemoryStream();
                await findPackageByIdResource.CopyNupkgToStreamAsync(
                    packageId,
                    nugetVersion,
                    packageStream,
                    _cacheContext,
                    _logger,
                    CancellationToken.None);

                packageStream.Position = 0;
                using var packageReader = new PackageArchiveReader(packageStream);
                return packageReader.ToPackageInfos();
            }
        }

        public PackageInfos GetInfosFromStream(Stream stream)
        {
            using var packageReader = new PackageArchiveReader(stream);
            return packageReader.ToPackageInfos();
        }

        public async Task<PackageInfos> CreateFromStreamAsync(Stream stream)
        {
            var packageReader = new PackageArchiveReader(stream);
            var identity = await packageReader.GetIdentityAsync(CancellationToken.None);

            string packagePath = Path.Combine(_folder, $"{identity.Id}.{identity.Version}.nupkg");
            using (var fileStream = File.Create(packagePath))
            {
                stream.Position = 0;
                await stream.CopyToAsync(fileStream);
            }

            return packageReader.ToPackageInfos();
        }

        public async Task RemoveAsync(string id, Version version)
        {
            var nugetVersion = new NuGetVersion(version);
            string packagePath = Path.Combine(_folder, $"{id}.{nugetVersion}.nupkg");

            if (File.Exists(packagePath))
            {
                File.Delete(packagePath);
            }
        }

        // XXX : should be cached since we have to load the package and dll and check types dynamically
        public async Task<IEnumerable<TaskClass>> GetTaskClassesAsync(string id, Version version)
        {
            var findPackageByIdResource = await _repository.GetResourceAsync<FindPackageByIdResource>();
            var nugetVersion = new NuGetVersion(version);

            using var packageStream = new MemoryStream();
            await findPackageByIdResource.CopyNupkgToStreamAsync(
                id,
                nugetVersion,
                packageStream,
                _cacheContext,
                _logger,
                CancellationToken.None);

            packageStream.Position = 0;
            using var packageReader = new PackageArchiveReader(packageStream);

            // Get the nearest framework
            var nearestFramework = GetNearestFramework(packageReader);

            // Get only the dll files from the nearest framework folder
            var dllFiles = packageReader.GetFiles()
                .Where(f => f.StartsWith($"lib/{nearestFramework}/") && f.EndsWith(".dll"));

            // Load all the assemblies and get all the types that implement the ITask interface
            var taskClasses = new List<TaskClass>();
            foreach (var dllFile in dllFiles)
            {
                using var dllStream = packageReader.GetStream(dllFile);
                using var memoryStream = new MemoryStream();
                dllStream.CopyTo(memoryStream);
                var assembly = Assembly.Load(memoryStream.ToArray());

                var taskTypes = assembly.GetTypes()
                    .Where(t => typeof(ITask).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
                taskClasses.AddRange(taskTypes.Select(t => new TaskClass(dllFile, t.FullName)));
            }

            return taskClasses;
        }

        /// <summary>
        /// Get the nearest framework to the current assembly
        /// </summary>
        /// <param name="packageReader"></param>
        /// <returns></returns>
        private string GetNearestFramework(PackageArchiveReader packageReader)
        {
            var currentFramework = NuGetFramework.Parse(
                Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName ??
                    throw new Exception("Cannot determine the current version of .net"));
            var frameworks = packageReader.GetLibItems()
                .Select(group => group.TargetFramework)
                .Where(f => f != null)
                .ToList();

            var nearestFramework = NuGetFrameworkUtility.GetNearest(frameworks, currentFramework, f => f) ?? throw new Exception("Can't find the nearest framework.");

            return nearestFramework.GetShortFolderName();
        }
    }

    // Extension method to convert to PackageInfos (you'll need to implement this based on your PackageInfos class)
    public static class PackageExtensions
    {
        public static PackageInfos ToPackageInfos(this IPackageSearchMetadata package)
        {
            // Implement conversion logic here
            throw new NotImplementedException();
        }

        public static PackageInfos ToPackageInfos(this PackageArchiveReader package)
        {
            // Implement conversion logic here
            throw new NotImplementedException();
        }
    }
}
