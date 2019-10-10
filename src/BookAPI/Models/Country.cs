using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPI.Models
{
    public class Country
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50,ErrorMessage ="Mininmum 50 chars required as a country name.")]
        public string Name { get; set; }
        public virtual ICollection<Author> Authors { get; set; }
    }
}
