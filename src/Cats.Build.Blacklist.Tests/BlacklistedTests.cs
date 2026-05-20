using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Cats.Build.Blacklist.Tests
{
    [TestClass]
    public class BlacklistedTests
    {
        [TestMethod]
        public void SimpleBlacklistedItem()
        {
            var projectAssets = Blacklist.GetProjectAssets(ProjectAssetsTests.ProjectAssetsV3FilePath);
            var packageBlacklist = new[]
            {
                new BlacklistItem
                {
                    Name = "Xamarin.Essentials",
                }
            };

            var blacklistedItems = Blacklist.GetBlacklistedLibraries(projectAssets, packageBlacklist, Array.Empty<BlacklistItem>());
            var blacklistedNames = blacklistedItems
                .SelectMany(kv => kv.Value.Select(i => i.Name).ToHashSet());

            Assert.HasCount(1, blacklistedNames);
            Assert.IsTrue(blacklistedNames.Contains("Xamarin.Essentials"));
        }

        [TestMethod]
        public void SimpleBlacklistedItemMixedCase()
        {
            var projectAssets = Blacklist.GetProjectAssets(ProjectAssetsTests.ProjectAssetsV3FilePath);
            var packageBlacklist = new[]
            {
                new BlacklistItem
                {
                    Name = "xamArIn.EssenTIALS",
                }
            };

            var blacklistedItems = Blacklist.GetBlacklistedLibraries(projectAssets, packageBlacklist, Array.Empty<BlacklistItem>());
            var blacklistedNames = blacklistedItems
                .SelectMany(kv => kv.Value.Select(i => i.Name).ToHashSet());

            Assert.HasCount(1, blacklistedNames);
            Assert.IsTrue(blacklistedNames.Contains("Xamarin.Essentials"));
        }
    }
}