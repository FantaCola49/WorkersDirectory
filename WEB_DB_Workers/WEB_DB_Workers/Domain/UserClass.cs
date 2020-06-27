using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WEB_DB_Workers.Domain
{
    [DisplayName("Use")]
    public class UserClass
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Please enter a login name")]
        [Display(Name = "Login")]
        public string Loginname { get; set; }

        public virtual LanguageClass Language { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; }

        [UIHint("Enum")]
        [EnumDataType(typeof(Supporter))]
        [Required(ErrorMessage = "Please select supporter tier")]
        [Display(Name = "Supporter")]
        public Supporter SupporterTier { get; set; }

        [HiddenInput(DisplayValue = true)]
        [ScaffoldColumn(false)]
        [Display(Name = "Last login")]
        public DateTime? LastLoginDate { get; set; }

        [HiddenInput(DisplayValue = false)]
        public bool IsLastLogin
        {
            get { return LastLoginDate != null && LastLoginDate > DateTime.MinValue; }
        }

        public UserClass() { }

        public UserClass(int UserID, string Loginname, LanguageClass Language, string Email, DateTime? LastLoginDate, Supporter SupporterTier)
        {
            this.UserID = UserID;
            this.Loginname = Loginname;
            this.Language = Language;
            this.Email = Email;
            this.LastLoginDate = LastLoginDate;
            this.SupporterTier = SupporterTier;
        }
    }

    public enum Supporter
    {
        [Display(Name = "")]
        None = 1,
        Silver = 2,
        Gold = 3,
        Platinum = 4

    }
}