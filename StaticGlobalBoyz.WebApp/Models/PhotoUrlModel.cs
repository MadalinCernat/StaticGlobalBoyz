using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Models
{
    public class PhotoUrlModel
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Url { get; set; }
    }
}
