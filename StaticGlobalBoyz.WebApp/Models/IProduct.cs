using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Models
{
    public interface IProduct
    {
        Guid Id { get; set; }
        string Title { get; set; }
        decimal Price { get; set; }
    }
}
