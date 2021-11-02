using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Models
{
    public class TagModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}
