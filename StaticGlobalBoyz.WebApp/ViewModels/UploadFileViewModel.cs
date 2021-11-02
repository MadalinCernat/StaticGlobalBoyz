using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.ViewModels
{
    public class UploadFileViewModel
    {
        public IFormFile File { get; set; }
    }
}
