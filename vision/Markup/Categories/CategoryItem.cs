using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Categories {

    public class CategoryItem {
        public List<CategoryItem> Children { get; set; } = new List<CategoryItem>();
        public List<Section> Content { get; set; } = new List<Section>();
        public string Location { get; set; } = "";

        public bool HasChildren {
            get {
                return (Children.Count > 0);
            }
        }

        public string ToString(string elementName) {
            string s = "(" + elementName + ": (content: ";
            foreach (var item in Content) {
                s = s + "<a href='#title-"+Location+"'>"+item.Raw+"</a>";
            }
            foreach (var item in Children) {
                s = s + item.ToString(elementName);
            }
            return (s + " ))");
        }
    }
}
