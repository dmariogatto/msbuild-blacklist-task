using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace Cats.Build.Blacklist.Tests
{
    [TestClass]
    public class ProjectAssetsTests
    {
        public const string ProjectAssetsFilePath = @"Files\project.assets.json";

        [TestMethod]
        public void FileDoesNotExist()
        {
            Assert.ThrowsException<FileNotFoundException>(() => Blacklist.GetProjectAssets("FAIL"));
        }

        [TestMethod]
        public void ParsesNoError()
        {
            var projectAssests = Blacklist.GetProjectAssets(ProjectAssetsFilePath);
            Assert.IsTrue(projectAssests.Any());
        }

        [TestMethod]
        public void ParsesLibraries()
        {
            var projectAssests = Blacklist.GetProjectAssets(ProjectAssetsFilePath);

            var direct = new[]
            {
                "Acr.UserDialogs",
                "Humanizer",
                "LiteDB",
                "Microsoft.AppCenter.Analytics",
                "Microsoft.AppCenter.Crashes",
                "Microsoft.Extensions.Caching.Memory",
                "Newtonsoft.Json",
                "NuGet.Versioning",
                "Plugin.StoreReview",
                "Polly",
                "Refractored.MvvmHelpers",
                "Shiny.Core",
                "Shiny.Notifications",
                "SimpleInjector",
                "Xamarin.Essentials.Interfaces",
            };

            var transitive = new[]
            {
                "Humanizer.Core",
                "Humanizer.Core.af",
                "Humanizer.Core.ar",
                "Humanizer.Core.az",
                "Humanizer.Core.bg",
                "Humanizer.Core.bn-BD",
                "Humanizer.Core.cs",
                "Humanizer.Core.da",
                "Humanizer.Core.de",
                "Humanizer.Core.el",
                "Humanizer.Core.es",
                "Humanizer.Core.fa",
                "Humanizer.Core.fi-FI",
                "Humanizer.Core.fr",
                "Humanizer.Core.fr-BE",
                "Humanizer.Core.he",
                "Humanizer.Core.hr",
                "Humanizer.Core.hu",
                "Humanizer.Core.hy",
                "Humanizer.Core.id",
                "Humanizer.Core.it",
                "Humanizer.Core.ja",
                "Humanizer.Core.lv",
                "Humanizer.Core.ms-MY",
                "Humanizer.Core.mt",
                "Humanizer.Core.nb",
                "Humanizer.Core.nb-NO",
                "Humanizer.Core.nl",
                "Humanizer.Core.pl",
                "Humanizer.Core.pt",
                "Humanizer.Core.ro",
                "Humanizer.Core.ru",
                "Humanizer.Core.sk",
                "Humanizer.Core.sl",
                "Humanizer.Core.sr",
                "Humanizer.Core.sr-Latn",
                "Humanizer.Core.sv",
                "Humanizer.Core.tr",
                "Humanizer.Core.uk",
                "Humanizer.Core.uz-Cyrl-UZ",
                "Humanizer.Core.uz-Latn-UZ",
                "Humanizer.Core.vi",
                "Humanizer.Core.zh-CN",
                "Humanizer.Core.zh-Hans",
                "Humanizer.Core.zh-Hant",
                "Microsoft.AppCenter",
                "Microsoft.Extensions.Caching.Abstractions",
                "Microsoft.Extensions.DependencyInjection",
                "Microsoft.Extensions.DependencyInjection.Abstractions",
                "Microsoft.Extensions.Logging.Abstractions",
                "Microsoft.Extensions.Options",
                "Microsoft.Extensions.Primitives",
                "Microsoft.NETCore.Platforms",
                "Microsoft.NETCore.Targets",
                "MuGet.Localisation",
                "System.Buffers",
                "System.ComponentModel",
                "System.Memory",
                "System.Numerics.Vectors",
                "System.Reactive",
                "System.Runtime",
                "System.Runtime.CompilerServices.Unsafe",
                "System.Runtime.InteropServices.WindowsRuntime",
                "System.Threading.Tasks.Extensions",
                "System.ValueTuple",
                "Xamarin.Essentials",
            };

            var target = projectAssests.First();
            var libNames = target.Libraries.Select(l => l.Name);

            Assert.IsTrue(libNames.Distinct().Count() == target.Libraries.Count, "Duplicate libraries");
            Assert.IsTrue(libNames.Except(direct.Concat(transitive)).Count() == 0);
        }
    }
}
