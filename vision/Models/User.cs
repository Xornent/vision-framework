using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Models {

    public class User {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Level { get; set; }

        [MaxLength(50)]
        public string Display { get; set; }

        public string Contact { get; set; }
        public string Edit { get; set; }
        public int Evaluation { get; set; }
        public int Banned { get; set; }
    }
}
