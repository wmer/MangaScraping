using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MangaScraping.Models {
    public class Hq : ModelBase, IComparable<Hq>, IEquatable<Hq> {
        public string CoverSource { get; set; }
        public string Author { get; set; }
        public string Synopsis { get; set; }
        public bool IsDetailedInformation { get; set; }
        public bool IsFinalized { get; set; }
        public virtual List<Chapter> Chapters { get; set; }

        public int CompareTo(Hq other) {
            return String.Compare(Title, other.Title, StringComparison.Ordinal);
        }

        public override bool Equals(object obj) {
            return Equals(obj as Hq);
        }

        public bool Equals(Hq other) {
            return other != null &&
                   Author == other.Author &&
                   Title == other.Title;
        }

        public override int GetHashCode() {
            var hashCode = 925511053;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Author);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
            return hashCode;
        }

        public override string ToString() {
            return $"Titulo: {Title} {Environment.NewLine}" +
                $"Autor: {Author} {Environment.NewLine}" +
                $"Sinopse: {Synopsis} {Environment.NewLine}" +
                $"Capitulos: {Chapters?.Count()} {Environment.NewLine}";
        }
    }
}
