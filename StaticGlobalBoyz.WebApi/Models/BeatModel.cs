using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApi.Models
{
    public class BeatModel
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public List<TagModel> Tags { get; set; } = new List<TagModel>();
        public string SampleUrl { get; set; }
        public decimal Mp3Price { get; set; }
        public decimal WavPrice { get; set; }
        public decimal ZipPrice { get; set; }
        public decimal ExclusivePrice { get; set; }
        public decimal Price { get; set; }
        public int Bpm { get; set; }
        public string Key { get; set; }
        public string Length { get; set; }
        public string ProducerName { get; set; }
        public DateTime DateUploaded { get; set; } = DateTime.Now;
        public string CoverUrl { get; set; }
        public string Format
        {
            get
            {
                if (Price == Mp3Price)
                {
                    return "Mp3";
                }
                else if (Price == WavPrice)
                {
                    return "Wav";
                }
                else if (Price == ZipPrice)
                {
                    return "Zip";
                }
                else if (Price == ExclusivePrice)
                {
                    return "Exclusive";
                }
                else
                {
                    return "Unknown";
                }
            }
        }
        public bool BoughtExclusively { get; set; } = false;
    }
}
