using NuGet.Versioning;
using System;

namespace Cats.Build.Blacklist
{
    public class BlacklistItem
    {
        public string Name { get; init; }
        public VersionRange Range { get; init; }

        public bool Matches(Dependency dependency)
        {
            var depVersion = !string.IsNullOrWhiteSpace(dependency?.Version)
                ? NuGetVersion.Parse(dependency.Version)
                : null;

            return string.Equals(dependency?.Name, Name, StringComparison.OrdinalIgnoreCase) &&
                   (depVersion is null ||
                    Range is null ||
                    Range == VersionRange.All ||
                    Range.Satisfies(depVersion));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            return obj is BlacklistItem item &&
                Name == item.Name &&
                Range?.Equals(item.Range) == true;
        }

        public override int GetHashCode() => HashCode.Combine(Name, Range);

        public override string ToString() => !string.IsNullOrEmpty(Name) && Range is not null
            ? $"{Name}/{Range.PrettyPrint()}"
            : base.ToString();
    }
}
