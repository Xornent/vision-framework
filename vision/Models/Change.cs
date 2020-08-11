using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Models {

    public class Change {
        public int Type { get; set; } = (int)ChangeTag.Edit;
        public int Version { get; set; }
        public int Checked { get; set; } = 0;
        public DateTime Post { get; set; }
        public string User { get; set; }
        public string Summary { get; set; }
    }

    public enum ChangeTag {
        Create,
        Edit,
        Delete,
        Republish
    }
}
