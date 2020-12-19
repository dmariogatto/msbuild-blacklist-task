using NuGet.Versioning;

namespace Cats.Build.Blacklist
{
    public class BlacklistItem
    {
        public string Name { get; set; }
        public VersionRange Range { get; set; }
        
        public bool Matches(Dependency dependency)
        {
            var depVersion = !string.IsNullOrWhiteSpace(dependency?.Version)
                ? NuGetVersion.Parse(dependency.Version)
                : null;

            return dependency?.Name == Name &&
                   (depVersion == null ||
                    Range == null ||
                    Range == VersionRange.All ||                    
                    Range.Satisfies(depVersion));
        }

        public override bool Equals(object obj) => 
            obj is BlacklistItem item &&
            Name == item.Name &&
            Range?.Equals(item.Range) == true;

        public override int GetHashCode() => (Name, Range).GetHashCode();

        public override string ToString() => !string.IsNullOrEmpty(Name) && Range != null
            ? $"{Name}/{Range.PrettyPrint()}"
            : base.ToString();
    }
}
