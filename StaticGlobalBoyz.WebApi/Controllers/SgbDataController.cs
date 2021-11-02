using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StaticGlobalBoyz.DataAccessLibrary;
using StaticGlobalBoyz.DataAccessLibrary.Data;
using StaticGlobalBoyz.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SgbDataController : Controller
    {
        private readonly IConfiguration _config;
        private const string _dbName = "SgbDB";
        private const string _connStringName = "MongoDbConnection";

        public SgbDataController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet("beats/all")]
        public List<BeatModel> GetAllBeats()
        {
            var db = new MongoDbDataAccess(new DbInfo(_dbName, _connStringName), _config);
            return db.LoadRecords<BeatModel>("Beats");
        }
        [HttpGet("tags/all")]
        public List<TagModel> GetAllTags()
        {
            var db = new MongoDbDataAccess(new DbInfo(_dbName, _connStringName), _config);
            return db.LoadRecords<TagModel>("Tags");
        }
        [HttpGet("clothes/all")]
        public List<ClothingArticleModel> GetAllClothes()
        {
            var db = new MongoDbDataAccess(new DbInfo(_dbName, _connStringName), _config);
            return db.LoadRecords<ClothingArticleModel>("Clothes");
        }
        [HttpGet("sizes/all")]
        public List<SizeModel> GetAllSizes()
        {
            var db = new MongoDbDataAccess(new DbInfo(_dbName, _connStringName), _config);
            return db.LoadRecords<SizeModel>("Sizes");
        }
        [HttpGet("clothingtypes/all")]
        public List<ClothingTypeModel> GetAllClothingTypes()
        {
            var db = new MongoDbDataAccess(new DbInfo(_dbName, _connStringName), _config);
            return db.LoadRecords<ClothingTypeModel>("ClothingTypes");
        }
        [HttpGet("orders/all")]
        public List<OrderModel> GetAllOrders()
        {
            var db = new MongoDbDataAccess(new DbInfo(_dbName, _connStringName), _config);
            return db.LoadRecords<OrderModel>("Orders");
        }
        [HttpGet("beatorders/all")]
        public List<BeatOrderModel> GetAllBeatOrders()
        {
            var db = new MongoDbDataAccess(new DbInfo(_dbName, _connStringName), _config);
            return db.LoadRecords<BeatOrderModel>("BeatOrders");
        }
        [HttpGet("beatspurchased/all")]
        public List<BeatsPurchasedModel> GetAllBeatsPurchased()
        {
            var db = new MongoDbDataAccess(new DbInfo(_dbName, _connStringName), _config);
            return db.LoadRecords<BeatsPurchasedModel>("BeatsPurchased");
        }

        [HttpGet("beats/{id}")]
        public BeatModel GetBeat(Guid id)
        {
            var db = new MongoDbDataAccess(new DbInfo(_dbName, _connStringName), _config);
            return db.LoadRecordById<BeatModel>("Beats", id);
        }
        [HttpGet("clothes/{id}")]
        public ClothingArticleModel GetClothing(Guid id)
        {
            var db = new MongoDbDataAccess(new DbInfo(_dbName, _connStringName), _config);
            return db.LoadRecordById<ClothingArticleModel>("Clothes", id);
        }
        [HttpGet("sizes/{id}")]
        public SizeModel GetSize(Guid id)
        {
            var db = new MongoDbDataAccess(new DbInfo(_dbName, _connStringName), _config);
            return db.LoadRecordById<SizeModel>("Sizes", id);
        }
        [HttpGet("beatorders/{id}")]
        public BeatOrderModel GetBeatOrder(Guid id)
        {
            var db = new MongoDbDataAccess(new DbInfo(_dbName, _connStringName), _config);
            return db.LoadRecordById<BeatOrderModel>("BeatOrders", id);
        }
        [HttpGet("orders/{id}")]
        public OrderModel GetOrder(Guid id)
        {
            var db = new MongoDbDataAccess(new DbInfo(_dbName, _connStringName), _config);
            return db.LoadRecordById<OrderModel>("Orders", id);
        }
    }
}
