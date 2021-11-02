using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Models
{
    public class BeatsPurchasedModel
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public List<Guid> PurchasedBeatsIds { get; set; }
    }
}
