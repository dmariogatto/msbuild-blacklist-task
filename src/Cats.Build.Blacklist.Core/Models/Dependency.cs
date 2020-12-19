using System;

namespace Cats.Build.Blacklist
{
    public class Dependency
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public override bool Equals(object obj) =>
                    obj is Dependency dependency &&
                    Name == dependency.Name &&
                    Version == dependency.Version;
        public override int GetHashCode() => (Name, Version).GetHashCode();

        public override string ToString() => !string.IsNullOrEmpty(Name)
            ? $"{Name}/{Version ?? string.Empty}"
            : base.ToString();
    }
}
