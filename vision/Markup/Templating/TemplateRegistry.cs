using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Markup.Templating {

    public class TemplateRegistry {
        public static Dictionary<string, List<Section>> Registry =
            new Dictionary<string, List<Section>>();

        public static Dictionary<string, List<string>> Parameters =
            new Dictionary<string, List<string>>();
    }
}
