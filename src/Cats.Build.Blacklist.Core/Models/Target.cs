using System.Collections.Generic;

namespace Cats.Build.Blacklist
{
    public class Target
    {
        public string Name { get; set; }
        public List<Library> Libraries { get; set; } = new List<Library>();

        public override bool Equals(object obj) =>
                    obj is Target target &&
                    Name == target.Name;
        public override int GetHashCode() => (Name).GetHashCode();

        public override string ToString() => !string.IsNullOrEmpty(Name)
            ? $"{Name}"
            : base.ToString();
    }
}
