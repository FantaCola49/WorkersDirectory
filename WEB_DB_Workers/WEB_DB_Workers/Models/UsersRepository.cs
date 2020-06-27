using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WEB_DB_Workers.Domain;

namespace WEB_DB_Workers.Models
{
    public class UsersRepository
    {
        public bool AddUser(UserClass user)
        {
            user.UserID = AddUser(Name: user.Loginname, LanguageID: user.Language.LanguageID, Email: user.Email, SupporterTier: user.SupporterTier);
            return user.UserID > 0;
        }

        public int AddUser(string Name, int LanguageID, string Email, Supporter SupporterTier)
        {
            int ID = 0;
            using (MySqlConnection connect = new MySqlConnection(Base.strConnect))
            {
                string sql = "INSERT INTO `Users` (`Loginname`, `LanguageID`, `Email`, `SupporterTier`) VALUES (@Loginname, @LanguageID, @Email, @SupporterTier)";
                using (MySqlCommand cmd = new MySqlCommand(sql, connect))
                {
                    cmd.Parameters.Add("Loginname", MySqlDbType.String).Value = Name;
                    cmd.Parameters.Add("LanguageID", MySqlDbType.Int32).Value = LanguageID;
                    cmd.Parameters.Add("Email", MySqlDbType.String).Value = Email;
                    cmd.Parameters.Add("SupporterTier", MySqlDbType.Int32).Value = SupporterTier;
                    connect.Open();
                    if (cmd.ExecuteNonQuery() >= 0)
                    {
                        sql = "SELECT LAST_INSERT_ID() AS ID";
                        cmd.CommandText = sql;
                        int.TryParse(cmd.ExecuteScalar().ToString(), out ID);
                    }
                }
            }
            return ID;
        }

        public bool ChangeUser(UserClass user)
        {
            return ChangeUser(ID: user.UserID, Name: user.Loginname, LanguageID: user.Language.LanguageID, Email: user.Email, SupporterTier: user.SupporterTier);
        }

        public bool ChangeUser(int ID, string Name, int LanguageID, string Email, Supporter SupporterTier)
        {
            bool result = false;
            if (ID > 0)
            {
                using (MySqlConnection connect = new MySqlConnection(Base.strConnect))
                {
                    string sql = "UPDATE `Users` SET `Loginname`=@Loginname, `LanguageID`=@LanguageID, `Email`=@Email, `SupporterTier`=@SupporterTier WHERE UserID=@UserID";
                    using (MySqlCommand cmd = new MySqlCommand(sql, connect))
                    {
                        cmd.Parameters.Add("UserID", MySqlDbType.Int32).Value = ID;
                        cmd.Parameters.Add("Loginname", MySqlDbType.String).Value = Name;
                        cmd.Parameters.Add("LanguageID", MySqlDbType.Int32).Value = LanguageID;
                        cmd.Parameters.Add("Email", MySqlDbType.String).Value = Email;
                        cmd.Parameters.Add("SupporterTier", MySqlDbType.Int32).Value = SupporterTier;
                        connect.Open();
                        result = cmd.ExecuteNonQuery() >= 0;
                    }
                }
            }
            return result;
        }

        public bool RemoveUser(UserClass user)
        {
            return RemoveUser(user.UserID);
        }

        public bool RemoveUser(int ID)
        {
            using (MySqlConnection connect = new MySqlConnection(Base.strConnect))
            {
                string sql = "DELETE FROM `Users` WHERE `UserID`=@UserID";
                using (MySqlCommand cmd = new MySqlCommand(sql, connect))
                {
                    cmd.Parameters.Add("UserID", MySqlDbType.Int32).Value = ID;
                    connect.Open();
                    return cmd.ExecuteNonQuery() >= 0;
                }
            }
        }

        public UserClass FetchByID(int ID)
        {
            UserClass user = null;
            using (MySqlConnection objConnect = new MySqlConnection(Base.strConnect))
            {
                string strSQL = "SELECT u.`UserID`, u.`Loginname`, l.`LanguageID`, l.`LanguageName`, u.`Email`, u.`LastLoginDate`, CAST(u.`SupporterTier` AS UNSIGNED) as `SupporterTier` FROM `Users` u LEFT JOIN `Languages` l ON l.LanguageID=u.LanguageID WHERE `UserID`=@UserID";
                using (MySqlCommand cmd = new MySqlCommand(strSQL, objConnect))
                {
                    objConnect.Open();
                    int UserID = 0, LanguageID = 0;
                    string Loginname = null, LanguageName = null, Email = String.Empty;
                    Supporter SupporterTier = Supporter.None;
                    DateTime? LastLoginDate = null;
                    cmd.Parameters.Add("UserID", MySqlDbType.Int32).Value = ID;
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            UserID = dr.GetInt32("UserID");
                            Loginname = dr.GetString("Loginname").ToString();
                            LanguageID = dr.GetInt32("LanguageID");
                            LanguageName = dr.GetString("LanguageName").ToString();
                            if (!dr.IsDBNull(dr.GetOrdinal("Email"))) Email = dr.GetString("Email").ToString();
                            if (!dr.IsDBNull(dr.GetOrdinal("LastLoginDate"))) LastLoginDate = dr.GetDateTime("LastLoginDate");
                            if (!dr.IsDBNull(dr.GetOrdinal("SupporterTier"))) SupporterTier = (Supporter)dr.GetInt32("SupporterTier");
                        }
                        LanguageClass language = null;
                        if (LanguageID > 0) language = new LanguageClass(LanguageID: LanguageID, LanguageName: LanguageName);
                        if (UserID > 0 && language != null && language.LanguageID > 0) user = new UserClass(UserID: UserID, Loginname: Loginname, Language: language, Email: Email, LastLoginDate: LastLoginDate, SupporterTier: (Supporter)SupporterTier);
                    }
                }
            }
            return user;
        }

        public IList<UserClass> List(string sortOrder, System.Web.Helpers.SortDirection sortDir, int page, int pagesize, out int count)
        {
            List<UserClass> users = new List<UserClass>();
            using (MySqlConnection objConnect = new MySqlConnection(Base.strConnect))
            {
                // Добавил в запрос сортировку
                string sort = " ORDER BY ";
                /* Это плохо, потому что можно использовать sql injection
                   но, к сожалению, MySQL не дает возможности использовать параметры для сортировки.
                   Поэтому надо экранировать кавычками, но перед этим обеспечить сначала проверку входного значения (чтобы кавычек в нём не было)
                   в моём проекте проверка значения идет в контроллере, перед построением модели
                */
                if (sortOrder != null && sortOrder != String.Empty)
                {
                    sort += "`" + sortOrder + "`";
                    if (sortDir == System.Web.Helpers.SortDirection.Descending) sort += " DESC";
                    sort += ",";
                }
                sort += "`UserID`"; // по умолчанию
                                    // добавляем в запрос отображение только части записей (отображение страницами)
                string limit = "";
                if (pagesize > 0)
                {
                    int start = (page - 1) * pagesize;
                    limit = string.Concat(" LIMIT ", start.ToString(), ", ", pagesize.ToString());
                }
                string strSQL = "SELECT SQL_CALC_FOUND_ROWS u.`UserID`, u.`Loginname`, l.`LanguageID`, l.`LanguageName` as `Language`, u.`Email`, u.`LastLoginDate`, CAST(u.`SupporterTier` AS UNSIGNED) as `SupporterTier` FROM `Users` u LEFT JOIN `Languages` l ON l.LanguageID=u.LanguageID" + sort + limit;
                using (MySqlCommand cmd = new MySqlCommand(strSQL, objConnect))
                {
                    objConnect.Open();
                    cmd.Parameters.Add("page", MySqlDbType.Int32).Value = page;
                    cmd.Parameters.Add("pagesize", MySqlDbType.Int32).Value = pagesize;
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            LanguageClass language = new LanguageClass(LanguageID: dr.GetInt32("LanguageID"), LanguageName: dr.GetString("Language").ToString());

                            users.Add(new UserClass(
                                UserID: dr.GetInt32("UserID"),
                                Loginname: dr.GetString("Loginname"),
                                Language: language,
                                Email: dr.IsDBNull(dr.GetOrdinal("Email")) ? String.Empty : dr.GetString("Email"),
                                LastLoginDate: dr.IsDBNull(dr.GetOrdinal("LastLoginDate")) ? (DateTime?)null : dr.GetDateTime("LastLoginDate"),
                                SupporterTier: dr.IsDBNull(dr.GetOrdinal("SupporterTier")) ? (Supporter)Supporter.None : (Supporter)dr.GetInt32("SupporterTier")));
                        }
                    }
                }
                using (MySqlCommand cmdrows = new MySqlCommand("SELECT FOUND_ROWS()", objConnect))
                {
                    int.TryParse(cmdrows.ExecuteScalar().ToString(), out count);
                }
            }
            return users;
        }
    }
}