using System;

namespace Cats.Build.Blacklist
{
    public class Dependency
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            return obj is Dependency dependency &&
            Name == dependency.Name &&
            Version == dependency.Version;
        }

        public override int GetHashCode() => HashCode.Combine(Name, Version);

        public override string ToString() => !string.IsNullOrEmpty(Name)
            ? $"{Name}/{Version ?? string.Empty}"
            : base.ToString();
    }
}
