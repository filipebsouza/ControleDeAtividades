using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Model.ViewModel
{
    public class GridVM
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<object> Lista { get; set; }
        public int TotalPages { get; set; }
        public Type Tipo { get; set; }
        public int PageLimit { get; set; }
    }
}
