using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPI.Dtos
{
    public class ReviewDto
    {
        public string HeadLine { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
    }
}
