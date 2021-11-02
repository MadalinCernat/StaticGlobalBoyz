using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;

namespace StaticGlobalBoyz.WebApp.Models
{
    public class BeatModel
    {
        [BsonId]
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        public List<TagModel> Tags { get; set; } = new List<TagModel>();
        [Required]
        [Display(Name = "Sample Url")]
        public string SampleUrl { get; set; }
        [Display(Name = "Mp3 Price")]
        public decimal Mp3Price { get; set; }
        [Display(Name = "Wav Price")]
        public decimal WavPrice { get; set; }
        [Display(Name = "Zip Price")]
        public decimal ZipPrice { get; set; }
        [Display(Name = "Exclusive Price")]
        public decimal ExclusivePrice { get; set; }
        public decimal Price { get; set; }
        [Required]
        public int Bpm { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public string Length { get; set; }
        [Required]
        [Display(Name = "Prod by")]
        public string ProducerName { get; set; }
        public DateTime DateUploaded { get; set; } = DateTime.Now;
        [Required]
        [Display(Name = "Cover Url")]
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
