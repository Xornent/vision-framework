using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Categories {

    public class ListItem {
        public List<ListItem> Children { get; set; } = new List<ListItem>();
        public List<Section> Content { get; set; } = new List<Section>();
        public bool Numbered { get; set; } = false;
        public ListItem Parent { get; set; }

        public bool HasChildren {
            get {
                return (Children.Count > 0);
            }
        }

        public override string ToString() {
            string s = "";
            foreach (var item in Content) {
                s = s + item.Raw;
            }

            if (HasChildren) {
                string lis = "" + s + "\n";
                bool? is_in_number = null;
                foreach (var item in Children) {
                    if(is_in_number == null) {
                        is_in_number = item.Numbered;
                        if(is_in_number == true) {
                            lis = lis + "<ol>";
                            lis = lis + "<li>" + item.ToString() + "</li>";
                        } else {
                            lis = lis + "<ul>";
                            lis = lis + "<li>" + item.ToString() + "</li>";
                        }
                     } else if(is_in_number == true) {
                        if(item.Numbered == true) {
                            lis = lis + "<li>" + item.ToString() + "</li>";
                        } else {
                            lis = lis + "</ol>";
                            is_in_number = false;
                            lis = lis + "<ul>" + "<li>" + item.ToString() + "</li>";
                        }
                    } else {
                        if(item.Numbered == true) {
                            lis = lis + "</ul>";
                            is_in_number = true;
                            lis = lis + "<ol>" + "<li>" + item.ToString() + "</li>";
                        } else lis = lis + "<li>" + item.ToString() + "</li>";
                    }
                }

                if(is_in_number != null) {
                    if (is_in_number == true) {
                        lis = lis + "</ol>";
                    } else lis = lis + "</ul>";
                }

                return lis;
            } else {
                return s;
            }
        }
    }
}
