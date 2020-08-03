using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Categories {

    public class CategoryItem {
        public List<CategoryItem> Children { get; set; } = new List<CategoryItem>();
        public int Level { get; set; }

        public bool HasChildren {
            get {
                return (Children.Count > 0);
            }
        }
    }
}
