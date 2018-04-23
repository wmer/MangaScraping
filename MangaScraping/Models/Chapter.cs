using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MangaScraping.Models {
    public class Chapter : ModelBase, IComparable<Chapter>, IEquatable<Chapter> {
        public virtual Hq Hq { get; set; }
        public virtual List<Page> Pages { get; set; }
        public bool ToDownload { get; set; }
        public bool IsUpdate { get; set; }
        public DateTime Date { get; set; }

        public int CompareTo(Chapter other) {
            return Pages.Count().CompareTo(other.Pages.Count());
        }

        public override bool Equals(object obj) {
            return Equals(obj as Chapter);
        }

        public bool Equals(Chapter other) {
            return other != null &&
                     Title == other.Title;
        }

        public override int GetHashCode() {
            return 508055234 + EqualityComparer<string>.Default.GetHashCode(Title);
        }
    }
}
