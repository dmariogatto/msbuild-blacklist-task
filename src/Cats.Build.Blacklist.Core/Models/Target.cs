using System;
using System.Collections.Generic;
using System.Linq;

namespace Cats.Build.Blacklist
{
    public class Target
    {
        public string Name { get; set; }
        public List<Library> Libraries { get; set; } = new List<Library>();

        public override bool Equals(object obj)
            => Equals(obj as Target);

        private bool Equals(Target other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is null)
                return false;

            if (!string.Equals(Name, other.Name, StringComparison.Ordinal))
                return false;

            if (Libraries is null && other.Libraries is null)
                return true;

            if (Libraries is null || other.Libraries is null)
                return false;

            return Libraries.SequenceEqual(other.Libraries);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            hash.Add(Name, StringComparer.Ordinal);

            if (Libraries is not null)
            {
                foreach (var lib in Libraries)
                {
                    hash.Add(lib);
                }
            }

            return hash.ToHashCode();
        }

        public override string ToString() => !string.IsNullOrEmpty(Name)
            ? $"{Name}"
            : base.ToString();
    }
}
