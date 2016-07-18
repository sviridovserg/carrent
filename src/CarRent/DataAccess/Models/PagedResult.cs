using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRent.DataAccess.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Result { get; set; }
        
        public int TotalCount { get; set; }
    }
}
