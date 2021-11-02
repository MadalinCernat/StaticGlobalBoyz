using StaticGlobalBoyz.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.ViewModels
{
    public class YourBeatsViewModel
    {
        public List<BeatItemModel> Beats { get; set; } = new List<BeatItemModel>();
    }
}
