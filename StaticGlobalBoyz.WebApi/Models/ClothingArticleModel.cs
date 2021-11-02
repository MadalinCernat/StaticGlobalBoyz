using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApi.Models
{
    public class ClothingArticleModel
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public List<SizeModel> AvailableSizes { get; set; } = new List<SizeModel>();
        public ClothingTypeModel ClothingType { get; set; }
        public List<ClothingTypeModel> ClothingTypes { get; set; }
        public List<PhotoUrlModel> PhotosUrl { get; set; } = new List<PhotoUrlModel>();
        public decimal Price { get; set; }
        public bool InStock { get; set; } = true;
        public DateTime DateUploaded { get; set; } = DateTime.Now;
        public SizeModel Size { get; set; }
    }
}
