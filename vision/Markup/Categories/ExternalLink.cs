using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Categories {

    public class ExternalLink {
        public int Id { get; set; }
        public List<Section> Alias { get; set; }
        public string Href { get; set; }
    }
}
