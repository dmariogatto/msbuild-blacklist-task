using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Versioning;
using System.IO;
using System.Linq;

namespace Cats.Build.Blacklist.Tests
{
    [TestClass]
    public class BlacklistTests
    {
        internal static readonly string EmptyBlacklistFilePath = $"Files{Path.DirectorySeparatorChar}empty.blacklist";
        internal static readonly string SimpleBlacklistFilePath = $"Files{Path.DirectorySeparatorChar}simple.blacklist";
        internal static readonly string SimpleMixedCaseBlacklistFilePath = $"Files{Path.DirectorySeparatorChar}simple.mixedcase.blacklist";
        internal static readonly string CommentsBlacklistFilePath = $"Files{Path.DirectorySeparatorChar}comments.blacklist";
        internal static readonly string PackageBlacklistFilePath = $"Files{Path.DirectorySeparatorChar}package.blacklist";

        [TestMethod]
        public void FileDoesNotExist()
        {
            Assert.Throws<FileNotFoundException>(() => Blacklist.GetBlacklistItems("FAIL"));
        }

        [TestMethod]
        public void EmptyBlackList()
        {
            var items = Blacklist.GetBlacklistItems(EmptyBlacklistFilePath);
            Assert.IsTrue(!items.Any());
        }

        [TestMethod]
        public void SimpleBlackList()
        {
            var items = Blacklist.GetBlacklistItems(SimpleBlacklistFilePath);
            Assert.IsTrue(items.Count == 1);

            var item = items.First();

            Assert.IsTrue(item.Name == "Xamarin.Forms");
            Assert.IsTrue(item.Range == VersionRange.All);
        }

        [TestMethod]
        public void SimpleBlackListMixedCase()
        {
            var items = Blacklist.GetBlacklistItems(SimpleMixedCaseBlacklistFilePath);
            Assert.IsTrue(items.Count == 1);

            var item = items.First();

            Assert.IsTrue(item.Name == "xamArin.forMS");
            Assert.IsTrue(item.Range == VersionRange.All);
        }

        [TestMethod]
        public void CommentsBlackList()
        {
            var items = Blacklist.GetBlacklistItems(CommentsBlacklistFilePath);
            Assert.IsTrue(items.Count == 2);

            var i1 = items.First(i => i.Name == "Xamarin.Forms");
            var i2 = items.First(i => i.Name == "iTextSharp");

            Assert.IsTrue(i1.Range == VersionRange.All);
            Assert.IsTrue(i2.Range == VersionRange.All);
        }

        [TestMethod]
        public void BlackList()
        {
            var items = Blacklist.GetBlacklistItems(PackageBlacklistFilePath);

            var expected = new[]
            {
                new BlacklistItem()
                {
                    Name = "Xamarin.Forms",
                    Range = VersionRange.All
                },
                new BlacklistItem()
                {
                    Name = "Newtonsoft.Json",
                    Range = VersionRange.Parse("[7.0.0,9.0.0]")
                },
                new BlacklistItem()
                {
                    Name = "Google.Protobuf",
                    Range = VersionRange.Parse("[3.10.0]")
                },
                new BlacklistItem()
                {
                    Name = "Portable.BouncyCastle",
                    Range = VersionRange.All
                },
            }.OrderBy(i => i.Name);

            Assert.IsTrue(items.SequenceEqual(expected));
        }
    }
}