using System.IO;
using System.Linq;
using MSBuildTask = Microsoft.Build.Utilities.Task;

namespace Cats.Build.Blacklist
{
    public class CheckLibrariesTask : MSBuildTask
    {
        public string ProjectAssetsFilePath { get; set; } = string.Empty;
        public string PackageBlacklistFilePath { get; set; } = string.Empty;
        public string ProjectBlacklistFilePath { get; set; } = string.Empty;

        public override bool Execute()
        {
            if (!File.Exists(ProjectAssetsFilePath))
            {
                Log.LogError($"\"{ProjectAssetsFilePath}\" does not exist");
                return false;
            }

            Log.LogMessage($"Project assets path: \"{ProjectAssetsFilePath}\"");

            var projectAssets = Blacklist.GetProjectAssets(ProjectAssetsFilePath);

            Log.LogMessage($"Package blacklist path: \"{PackageBlacklistFilePath}\"");
            Log.LogMessage($"Project blacklist path: \"{ProjectBlacklistFilePath}\"");

            var packageBlacklist = File.Exists(PackageBlacklistFilePath)
                ? Blacklist.GetBlacklistItems(PackageBlacklistFilePath)
                : Enumerable.Empty<BlacklistItem>();
            var projectBlacklist = File.Exists(ProjectBlacklistFilePath)
                ? Blacklist.GetBlacklistItems(ProjectBlacklistFilePath)
                : Enumerable.Empty<BlacklistItem>();

            if (!packageBlacklist.Any())
                Log.LogMessage($"No packages found on the blacklist \"{PackageBlacklistFilePath}\", assuming all are valid");
            if (!projectBlacklist.Any())
                Log.LogMessage($"No projects found on the blacklist \"{ProjectBlacklistFilePath}\", assuming all are valid");

            var blacklistedItems = Blacklist.GetBlacklistedLibraries(projectAssets, packageBlacklist, projectBlacklist);

            foreach (var kv in blacklistedItems)
            {
                foreach (var p in kv.Value)
                {
                    Log.LogError($"'{p}; {kv.Key}' has been blacklisted! Please remove reference to continue 🔥");
                }
            }

            return !blacklistedItems.Any();
        }
    }
}
