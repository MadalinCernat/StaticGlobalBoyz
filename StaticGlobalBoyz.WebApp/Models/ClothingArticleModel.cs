using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Models
{
    public class ClothingArticleModel : IProduct
    {
        [BsonId]
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Available Sizes")]
        public List<SizeModel> AvailableSizes { get; set; } = new List<SizeModel>();
        [Required]
        [Display(Name = "Clothing Type")]
        public ClothingTypeModel ClothingType { get; set; }
        public List<ClothingTypeModel> ClothingTypes { get; set; }
        [Required]
        [Display(Name = "Photos Url")]
        public List<PhotoUrlModel> PhotosUrl { get; set; } = new List<PhotoUrlModel>();
        [Required]
        public decimal Price { get; set; }
        public bool InStock { get; set; } = true;
        public DateTime DateUploaded { get; set; } = DateTime.Now;
        public SizeModel Size { get; set; }
    }
}
