using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Cats.Build.Blacklist
{
    public static class Blacklist
    {
        public static IReadOnlyDictionary<string, IList<Library>> GetBlacklistedLibraries(IEnumerable<Target> projectAssets, IEnumerable<BlacklistItem> packageBlacklist, IEnumerable<BlacklistItem> projectBlacklist)
        {
            var foundItems = new Dictionary<string, IList<Library>>(StringComparer.Ordinal);

            var packageLookup = packageBlacklist.ToLookup(i => i.Name, StringComparer.OrdinalIgnoreCase);
            var projectLookup = projectBlacklist.ToLookup(i => i.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var target in projectAssets)
            {
                var blacklistedInTarget = new List<Library>();

                foreach (var library in target.Libraries)
                {
                    var lookup = string.Equals(library.Type, Library.PackageType, StringComparison.Ordinal) ? packageLookup :
                                 string.Equals(library.Type, Library.ProjectType, StringComparison.Ordinal) ? projectLookup : null;

                    if (lookup?.Contains(library.Name) == true && lookup[library.Name].Any(p => p.Matches(library)))
                    {
                        blacklistedInTarget.Add(library);
                    }
                }

                if (blacklistedInTarget.Count > 0)
                {
                    foundItems.Add(target.Name, blacklistedInTarget.OrderBy(d => d.Name).ToList());
                }
            }

            return foundItems;
        }

        internal static IList<Target> GetProjectAssets(string projectAssetsFilePath)
        {
            var result = new List<Target>();

            using (var fs = File.OpenRead(projectAssetsFilePath))
            {
                using var jsonDocument = JsonDocument.Parse(fs);
                var root = jsonDocument.RootElement;

                if (!root.TryGetProperty("version", out var versionElement) ||
                    (versionElement.GetInt32() is int version && version != 3 && version != 4))
                {
                    throw new NotImplementedException($"Expected \"project.assets.json\" version 3 or 4, got '{versionElement}'");
                }

                if (root.TryGetProperty("targets", out var targets) && root.TryGetProperty("projectFileDependencyGroups", out var projDepGroups))
                {
                    foreach (var target in targets.EnumerateObject())
                    {
                        var targetObj = new Target { Name = target.Name };

                        var projDepGroup = projDepGroups
                            .EnumerateObject()
                            .FirstOrDefault(g => g.Name.Equals(target.Name, StringComparison.OrdinalIgnoreCase));
                        var directDeps = projDepGroup.Value.ValueKind != JsonValueKind.Undefined
                            ? projDepGroup.Value.EnumerateArray()
                                .Select(i => i.GetString())
                                .Where(i => !string.IsNullOrWhiteSpace(i) && i.IndexOf(' ') > 0)
                                .Select(i => i.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0] ?? string.Empty)
                                .ToHashSet(StringComparer.OrdinalIgnoreCase)
                            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                        foreach (var lib in target.Value.EnumerateObject())
                        {
                            var libComponents = lib.Name.Split('/');
                            if (libComponents.Length == 2)
                            {
                                var libObj = new Library
                                {
                                    Name = libComponents[0],
                                    Version = libComponents[1],
                                    Type = lib.Value.TryGetProperty("type", out var type) ? type.GetString() : null,
                                    ReferenceType = directDeps.Contains(libComponents[0]) ? Library.TopLevelReference : Library.TransitiveReference
                                };

                                if (lib.Value.TryGetProperty("dependencies", out var deps))
                                {
                                    foreach (var dep in deps.EnumerateObject())
                                    {
                                        libObj.Dependencies.Add(new Dependency
                                        {
                                            Name = dep.Name,
                                            Version = dep.Value.GetString()
                                        });
                                    }
                                }

                                targetObj.Libraries.Add(libObj);
                            }
                        }
                        result.Add(targetObj);
                    }
                }
            }

            return result;
        }

        internal static IList<BlacklistItem> GetBlacklistItems(string blacklistFilePath)
        {
            var result = new List<BlacklistItem>();

            var lines = File.ReadAllLines(blacklistFilePath ?? string.Empty)
                            .Select(l => l.IndexOf('#') is int idx && idx >= 0
                                         ? l[..idx].Trim()
                                         : l.Trim())
                            .Where(l => !string.IsNullOrEmpty(l));

            result.AddRange(lines.Select(l =>
            {
                var components = l.Split(['/'], StringSplitOptions.RemoveEmptyEntries);
                return components.Length > 0
                    ? new BlacklistItem()
                    {
                        Name = components[0],
                        Range = components.Length == 2 &&
                                VersionRange.TryParse(components[1], out var range)
                                ? range
                                : VersionRange.All
                    }
                    : null;
            }).Where(i => i is not null).OrderBy(i => i.Name));

            return result;
        }
    }
}
