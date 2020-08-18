using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Models {

    public class Category {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }
        public string Pages { get; set; }
        public string Alias { get; set; }

        [NotMapped]
        [BindNever]
        public List<string> PageList { get; set; } = new List<string>();

        [NotMapped]
        [BindNever]
        public List<string> SubCategoryList { get; set; } = new List<string>();

        [NotMapped]
        [BindNever]
        public int CurrentPage { get; set; } = 1;

        [NotMapped]
        [BindNever]
        public int CurrentCategory { get; set; } = 1;

        [NotMapped]
        [BindNever]
        public string AliasBody { get; set; } 

        [NotMapped]
        [BindNever]
        public string AliasHtml { get; set; }

        public void DeletePage(string pageName) {
            this.Pages = this.Pages.Replace("\n@page " + pageName, "").Replace("@page " + pageName + "\n", "").Replace("@page " + pageName, "");
        }

        public void AddPage(string pageName) {
            this.Pages = this.Pages + "@page " + pageName + "\n";
        }

        public void DeleteCategory(string pageName) {
            this.Pages = this.Pages.Replace("\n@cat " + pageName, "").Replace("@cat " + pageName + "\n", "").Replace("@cat " + pageName, "");
        }

        public void AddCategory(string pageName) {
            this.Pages = this.Pages + "@cat " + pageName + "\n";
        }
    }
}
