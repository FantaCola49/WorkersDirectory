using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_DB_Workers.Domain;

namespace WEB_DB_Workers.Models
{
    public class UserModel
    {
        public UserClass User { get; set; }
        private IList<LanguageClass> Languages { get; set; }

        public UserModel() { }
        public UserModel(UserClass user, IList<LanguageClass> languages)
        {
            this.User = user;
            this.Languages = languages;
        }

        public IEnumerable<SelectListItem> SelectLanguages()
        {
            if (Languages != null) { return new SelectList(Languages, "LanguageID", "LanguageName"); }
            return null;
        }
    }
}