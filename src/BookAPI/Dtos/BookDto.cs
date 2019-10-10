using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPI.Dtos
{
    public class BookDto
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public DateTime? DatePublished { get; set; }
    }
}
