using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WEB_DB_Workers.Domain
{
    [DisplayName("Language")]
    public class LanguageClass
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        [Required(ErrorMessage = "Please select a language")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a language")]
        public int LanguageID { get; set; }

        [Display(Name = "Language")]
        public string LanguageName { get; set; }

        public LanguageClass() { }

        public LanguageClass(int LanguageID, string LanguageName)
        {
            this.LanguageID = LanguageID;
            this.LanguageName = LanguageName;
        }
    }
}