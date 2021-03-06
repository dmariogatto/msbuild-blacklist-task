﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cats.Build.Blacklist
{
    public static class Blacklist
    {
        public static IDictionary<string, IList<Library>> GetBlacklistedLibraries(IEnumerable<Target> projectAssets, IEnumerable<BlacklistItem> packageBlacklist, IEnumerable<BlacklistItem> projectBlacklist)
        {
            var foundItems = new Dictionary<string, IList<Library>>();

            var packageDictionary = packageBlacklist
                .GroupBy(i => i.Name)
                .ToDictionary(g => g.Key, g => g.Select(i => i));
            var projectDictionary = projectBlacklist
                .GroupBy(i => i.Name)
                .ToDictionary(g => g.Key, g => g.Select(i => i));

            foreach (var t in projectAssets)
            {
                var packageMatches = t.Libraries
                    .Where(i => i.Type == Library.PackageType &&
                                packageDictionary.ContainsKey(i.Name) &&
                                packageDictionary[i.Name].Any(p => p.Matches(i)));
                var projectMatches = t.Libraries
                    .Where(i => i.Type == Library.ProjectType &&
                                projectDictionary.ContainsKey(i.Name) &&
                                projectDictionary[i.Name].Any(p => p.Matches(i)));

                if (packageMatches.Any() || projectMatches.Any())
                {
                    foundItems.Add(t.Name, packageMatches.Concat(projectMatches).OrderBy(d => d.Name).ToList());
                }
            }

            return foundItems;
        }

        internal static IList<Target> GetProjectAssets(string projectAssetsFilePath)
        {
            var result = new List<Target>();

            var jsonDocument = default(JObject);

            using (var fs = File.OpenRead(projectAssetsFilePath))
            using (var sReader = new StreamReader(fs))
            using (var jReader = new JsonTextReader(sReader))
                jsonDocument = new JsonSerializer().Deserialize<JObject>(jReader);

            var version = jsonDocument.GetValue("version").ToObject<int>();
            if (version != 3)
                throw new NotImplementedException($"Expected \"project.assets.json\" version 3, got '{version}'");

            var targets = jsonDocument.GetValue("targets").ToObject<JObject>();
            foreach (var target in targets.Properties())
            {
                var targetObj = new Target()
                {
                    Name = target.Name
                };

                foreach (var lib in target.Value.ToObject<JObject>().Properties())
                {
                    var libComponents = lib.Name.Split('/');
                    if (libComponents.Length == 2)
                    {
                        var libObj = new Library()
                        {
                            Name = libComponents[0],
                            Version = libComponents[1],
                            Type = lib.Value.Value<string>("type")
                        };

                        if (lib.Value.Value<JObject>("dependencies") is JObject deps)
                        {
                            foreach (var dep in deps.Properties())
                            {
                                libObj.Dependencies.Add(new Dependency()
                                {
                                    Name = dep.Name,
                                    Version = dep.Value.ToObject<string>()
                                });
                            }
                        }

                        targetObj.Libraries.Add(libObj);
                    }
                }

                result.Add(targetObj);
            }

            return result;
        }

        internal static IList<BlacklistItem> GetBlacklistItems(string blacklistFilePath)
        {
            var result = new List<BlacklistItem>();

            var lines = File.ReadAllLines(blacklistFilePath ?? string.Empty)
                            .Select(l => l.IndexOf('#') is int idx && idx >= 0
                                         ? l.Substring(0, idx).Trim()
                                         : l.Trim())
                            .Where(l => !string.IsNullOrEmpty(l));

            result.AddRange(lines.Select(l =>
            {
                var components = l.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
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
            }).Where(i => i != null).OrderBy(i => i.Name));

            return result;
        }
    }
}
