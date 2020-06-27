using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WEB_DB_Workers.Domain;

namespace WEB_DB_Workers.Models
{
    public class UsersGrid
    {
        public IEnumerable<UserClass> Users { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public SortingInfo SortingInfo { get; set; }
    }
}