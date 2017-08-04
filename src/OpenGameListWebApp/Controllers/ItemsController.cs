using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenGameListWebApp.ViewModels;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OpenGameListWebApp.Controllers
{    
    [Route("api/[controller]")]
    public class ItemsController : Controller
    {
        #region RESTful Convenstions
        /// <summary>
        /// GET: api/items
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public IActionResult Get()
        {
            return NotFound(new
            {
                Error = "not found"
            });
        }
        /// <summary>
        /// GET: api/items/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet ("{id}")]
        public IActionResult Get(int id)
        {
            return new JsonResult(GetSampleItems().Where(i => i.Id == id)
                .FirstOrDefault(),
                DefaultJsonSettings);
        }
        #endregion RESTful Conventions

        #region Attribute-based Routing
        [HttpGet ("GetLatest")]
        public IActionResult GetLatest()
        {
            return GetLatest(DefaultNumberOfItems);
        }
        /// <summary>
        /// GET: api/items/GetLatest/{n}
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        [HttpGet ("GetLatest/{n}")]
        public IActionResult GetLatest(int n)
        {
            if (n > MaxNumberOfItems)
                n = MaxNumberOfItems;
            var items = GetSampleItems().OrderByDescending(i => i.CreatedDate).Take(n);
            return new JsonResult(items, DefaultJsonSettings);
        }
        /// <summary>
        /// GET: api/items/GetMostViewed
        /// </summary>
        /// <returns></returns>
        [HttpGet ("GetMostViewed")]
        public IActionResult GetMostViewed()
        {
            return GetMostViewed(DefaultNumberOfItems);
        }

        /// <summary>
        /// GET: api/items/GetMostViewed/{n}
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        [HttpGet ("GetMostViewed/{n}")]
        public IActionResult GetMostViewed(int n)
        {
            if (n > MaxNumberOfItems)
                n = MaxNumberOfItems;
            var items = GetSampleItems().OrderByDescending(i => i.ViewCount).Take(n);
            return new JsonResult(items, DefaultJsonSettings);
        }
        /// <summary>
        /// GET: api/items/GetRandom
        /// </summary>
        /// <returns></returns>
        [HttpGet ("GetRandom")]
        public IActionResult GetRandom()
        {
            return GetRandom(DefaultNumberOfItems);
        }

        /// <summary>
        /// GET: api/items/GetRandom/{n}
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        [HttpGet ("GetRandom/{n}")]
        public IActionResult GetRandom(int n)
        {
            if (n > MaxNumberOfItems)
                n = MaxNumberOfItems;
            var items = GetSampleItems().OrderBy(i => Guid.NewGuid()).Take(n);
            return new JsonResult(items, DefaultJsonSettings);
        }
        #endregion Attribute-based Routing       

        #region Private Members
        private List<ItemViewModel> GetSampleItems(int num = 999)
        {
            List<ItemViewModel> lst = new List<ItemViewModel>();
            DateTime date = new DateTime(2015, 12, 31).AddDays(-num);

            for (int id = 1; id <= num; id++)
            {
                lst.Add(new ItemViewModel()
                {
                    Id = id,
                    Title = string.Format("Item {0} Title", id),
                    Description = string.Format("This is a sample description for item {0}: Lorem ipsum dolor sit amet.", id),
                    CreatedDate = date.AddDays(id),
                    ViewCount = num - id
                });
            }
            return lst;
        }

        private JsonSerializerSettings DefaultJsonSettings
        {
            get
            {
                return new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                };
            }
        }

        private int DefaultNumberOfItems
        {
            get
            {
                return 5;
            }
        }

        private int MaxNumberOfItems
        {
            get
            {
                return 100;
            }
        }
        #endregion Private Members
    }
}
