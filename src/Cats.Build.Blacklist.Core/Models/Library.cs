using System;
using System.Collections.Generic;
using System.Linq;

namespace Cats.Build.Blacklist
{
    public class Library : Dependency
    {
        public const string PackageType = "package";
        public const string ProjectType = "project";

        public const string TopLevelReference = "top-level";
        public const string TransitiveReference = "transitive";

        public string Type { get; set; }
        public string ReferenceType { get; set; }
        public List<Dependency> Dependencies { get; set; } = new List<Dependency>();

        public override bool Equals(object obj)
            => Equals(obj as Library);

        private bool Equals(Library other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is null)
                return false;

            if (!string.Equals(Type, other.Type, StringComparison.Ordinal))
                return false;

            if (!string.Equals(ReferenceType, other.ReferenceType, StringComparison.Ordinal))
                return false;

            if (Dependencies is null && other.Dependencies is null)
                return true;

            if (Dependencies is null || other.Dependencies is null)
                return false;

            return Dependencies.SequenceEqual(other.Dependencies);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            hash.Add(Type, StringComparer.Ordinal);
            hash.Add(ReferenceType, StringComparer.Ordinal);

            if (Dependencies is not null)
            {
                foreach (var dep in Dependencies)
                {
                    hash.Add(dep);
                }
            }

            return hash.ToHashCode();
        }
    }
}
