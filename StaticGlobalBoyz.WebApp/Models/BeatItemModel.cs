using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Models
{
    public class BeatItemModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public BeatModel Product { get; set; }
        public int Quantity { get; set; }
    }
}
