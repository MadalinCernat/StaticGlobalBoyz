using StaticGlobalBoyz.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.ViewModels
{
    public class EditDeleteBeatViewModel
    {
        public Guid BeatId { get; set; }
        public List<BeatModel> Beats { get; set; }
    }
}
