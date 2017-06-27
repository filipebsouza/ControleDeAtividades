using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Model.ViewModel
{
    public class PaginacaoVM
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageLimit { get; set; }
    }
}
