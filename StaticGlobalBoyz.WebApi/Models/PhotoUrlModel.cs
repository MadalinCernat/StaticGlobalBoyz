using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApi.Models
{
    public class PhotoUrlModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Url { get; set; }
    }
}
