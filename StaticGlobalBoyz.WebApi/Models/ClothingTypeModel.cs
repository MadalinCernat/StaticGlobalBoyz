using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApi.Models
{
    public class ClothingTypeModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
