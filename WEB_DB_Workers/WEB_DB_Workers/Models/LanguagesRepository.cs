using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WEB_DB_Workers.Domain;

namespace WEB_DB_Workers.Models
{
    public class LanguagesRepository
    {
        public IList<LanguageClass> List()
        {
            List<LanguageClass> languages = new List<LanguageClass>();
            using (MySqlConnection objConnect = new MySqlConnection(Base.strConnect))
            {
                string strSQL = "SELECT `LanguageID`, `LanguageName` as `Language` FROM `Languages` ORDER BY `LanguageName`";
                using (MySqlCommand cmd = new MySqlCommand(strSQL, objConnect))
                {
                    objConnect.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            LanguageClass language = new LanguageClass(LanguageID: dr.GetInt32("LanguageID"), LanguageName: dr.GetString("Language").ToString());
                            languages.Add(language);
                        }
                    }
                }
            }
            return languages;
        }
    }
}