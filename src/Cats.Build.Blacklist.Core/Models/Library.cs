using System.Collections.Generic;

namespace Cats.Build.Blacklist
{
    public class Library : Dependency
    {
        public const string PackageType = "package";
        public const string ProjectType = "project";

        public string Type { get; set; }
        public List<Dependency> Dependencies { get; set; } = new List<Dependency>();

        public override bool Equals(object obj) =>
                    obj is Dependency dependency &&
                    obj.Equals(dependency);
        public override int GetHashCode() => base.GetHashCode();
    }
}
