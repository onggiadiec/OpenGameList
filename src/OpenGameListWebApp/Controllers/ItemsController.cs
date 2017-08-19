using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nelibur.ObjectMapper;
using OpenGameListWebApp.ViewModels;
using Newtonsoft.Json;
using OpenGameListWebApp.Data;
using OpenGameListWebApp.Data.Items;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OpenGameListWebApp.Controllers
{    
    [Route("api/[controller]")]
    public class ItemsController : Controller
    {
        #region Private Fields

        private ApplicationDbContext DbContext;
        #endregion Private Fields

        #region Constructor

        public ItemsController(ApplicationDbContext context)
        {
            // Dependency Injection
            DbContext = context;
        }
        #endregion Constructor

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
            var item = DbContext.Items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                return new JsonResult(TinyMapper.Map<ItemViewModel>(item), DefaultJsonSettings);
            }
            else
            {
                return NotFound(new
                {
                    Error = $"Item ID {id} has not been found"
                });
            }
        }
        /// <summary>
        /// POST: api/itmes
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns>Creates a new Item and reuturn it accordingly.</returns>
        [HttpPost()]
        public IActionResult Add([FromBody] ItemViewModel ivm)
        {
            if (ivm != null)
            {
                // create a new Item with the client-sent json data
                var item = TinyMapper.Map<Item>(ivm);
                //override any property that could be wise to set from server-side only
                item.CreatedDate = item.LastModifiedDate = DateTime.Now;
                // replace the following with the current user's id when authentication will be available
                item.UserId = DbContext.Users.FirstOrDefault(u => u.UserName == "Admin").Id;
                // add the new item
                DbContext.Items.Add(item);
                // persist the changes into the database
                DbContext.SaveChanges();
                // return the newly-created Item to the client.
                return new JsonResult(TinyMapper.Map<ItemViewModel>(item), DefaultJsonSettings);
            }
            // return a generic HTTP Status 500 (Not Found) if the client payload is invalid.
            return new StatusCodeResult(500);
        }
        /// <summary>
        /// PUT: api/items/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ivm"></param>
        /// <returns></returns>
        [HttpPut ("{id}")]
        public IActionResult Update(int id, [FromBody] ItemViewModel ivm)
        {
            if (ivm != null)
            {
                var item = DbContext.Items.FirstOrDefault(i => i.Id == id);
                if (item != null)
                {
                    // handle the update (on per-property basic)
                    item.UserId = ivm.UserId;
                    item.Description = ivm.Description;
                    item.Flags = ivm.Flags;
                    item.Notes = ivm.Notes;
                    item.Text = ivm.Text;
                    item.Title = ivm.Title;
                    item.Type = ivm.Type;
                    // override any property that could be wise to set from server-side only
                    item.LastModifiedDate = DateTime.Now;
                    DbContext.SaveChanges();

                    return new JsonResult(TinyMapper.Map<ItemViewModel>(item), DefaultJsonSettings);
                }
            }
            return NotFound(new
            {
                Error = $"Item ID {id} has not been found"
            });
        }
        /// <summary>
        /// DELETE: api/items/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes an Item, returning a HTTP status 200</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = DbContext.Items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                // remove the item to delete from the DbContext
                DbContext.Items.Remove(item);

                DbContext.SaveChanges();

                return new OkResult();
            }
            return NotFound(new {Error = $"Item ID {id} has not been found"});
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
            var items = DbContext.Items.OrderByDescending(i => i.CreatedDate).Take(n).ToArray();
            return new JsonResult(ToItemViewModelList(items), DefaultJsonSettings);
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
            var items = DbContext.Items.OrderByDescending(i => i.ViewCount).Take(n).ToArray();
            return new JsonResult(ToItemViewModelList(items), DefaultJsonSettings);
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
            var items = DbContext.Items.OrderBy(i => Guid.NewGuid()).Take(n).ToArray();
            return new JsonResult(ToItemViewModelList(items), DefaultJsonSettings);
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
                    Title = $"Item {id} Title",
                    Description = $"This is a sample description for item {id}: Lorem ipsum dolor sit amet.",
                    CreatedDate = date.AddDays(id),
                    ViewCount = num - id
                });
            }
            return lst;
        }
        /// <summary>
        /// Maps a collection of Item entities into a list of Itemviewmodel objects
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private List<ItemViewModel> ToItemViewModelList(IEnumerable<Item> items)
        {
            var lst = new List<ItemViewModel>();
            foreach (var i in items)
            {
                lst.Add(TinyMapper.Map<ItemViewModel>(i));
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
