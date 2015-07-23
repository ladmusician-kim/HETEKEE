using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace haetack.Model.DTO
{
    public class ItemsDTO<T>
    {
        public List<T> Items { get; set; }
        public int PerPage { get; set; }
        public int RowCount { get; set; }
        public int Page { get; set; }
    }
}