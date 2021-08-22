using Microsoft.AspNetCore.Mvc;
using WebTask.Models;
using WebTask.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace WebTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICollectData collectData;
        private readonly IItemData itemData;
        private readonly IItemLikeData itemLikeData;
        private readonly ICommentData commentData;
        private ApplicationContext db;

        static string urlPhoto;
        static string idCollect;

        protected string MyProperty { get { return "your value"; } }

        public HomeController(ICollectData collectData, IItemData itemData, IItemLikeData itemLikeData, ApplicationContext context, ICommentData commentData)
        {
            this.collectData = collectData;
            this.itemData = itemData;
            this.itemLikeData = itemLikeData;
            this.commentData = commentData;
            db = context;
        }


        /// <summary>
        /// Show All
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewBag.itemsforCloud = ReturnUniqueTags();
            return View(collectData.GetCollect());
        }

        /// <summary>
        /// Show your collections
        /// </summary>
        /// <returns></returns>
        public IActionResult Personal()
        {
            ViewBag.itemsforCloud = ReturnUniqueTags();
            return View(collectData.GetCollect());
        }

        /// <summary>
        /// Show List Items
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Items(int? Id)
        {
            ViewBag.itemsforCloud = ReturnUniqueTags();

            idCollect = Id.ToString();
            ViewBag.Collect = await db.collects.FirstOrDefaultAsync(p => p.Id == Id);

            var items = db.items.Where(p => p.CollectId == Id);

            ViewBag.Items = items;
            return View();

        }

        [HttpGet]
        public IActionResult ItemsAll(string Id)
        {
            ViewBag.var1 = ReturnUniqueTags();
            ViewBag.Tag = Id;
            ViewBag.items = db.items.Where(p => p.Tags.Contains(Id));
            ViewBag.like = db.itemsLike.Where(p => p.NameUser == User.Identity.Name).ToList<ItemLike>();
            return View(itemLikeData.GetLike());
        }


        #region Collects

        [HttpGet]
        public IActionResult AddCollect()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCollect(Collect collect)
        {

            if (ModelState.IsValid && collect.NameCollection != null)
            {
                collect.Photo = urlPhoto;
                urlPhoto = null;
                collect.NameAuthor = User.Identity.Name;
                collectData.AddCollect(collect);
            }

            return RedirectToAction("Personal", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCollect(int? id)
        {
            if (id != null)
            {
                Collect collect = new Collect { Id = id.Value };
                db.Entry(collect).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Collect collect = await db.collects.FirstOrDefaultAsync(p => p.Id == id);
                if (collect != null)
                    return View(collect);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditCollect(Collect collect)
        {
            if (urlPhoto != null)
            {
                collect.Photo = urlPhoto;
                urlPhoto = null;
            }

            db.collects.Update(collect);
            await db.SaveChangesAsync();
            return RedirectToAction("Personal");
        }


        #endregion

        #region Items

        public ActionResult AutocompleteSearch(string term)
        {
            var models = db.items.Where(a => a.Tags.Contains(term))
                            .Select(a => new { value = a.Tags })
                            .Distinct();

            return Json(models);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(int? id)
        {
            if (id != null)
            {
                Collect collect = await db.collects.FirstOrDefaultAsync(p => p.Id == id);
                if (collect != null)
                    return View(collect);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult AddItemInCollect(Item item)
        {
            if (ModelState.IsValid && item.NameItem != null)
            {
                itemData.AddItem(item);
            }
            return RedirectToAction("Personal", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id != null)
            {
                Item item = new Item { Id = id.Value };
                db.Entry(item).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Personal");
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditItemView(int? id)
        {
            if (id != null)
            {
                Item item = await db.items.FirstOrDefaultAsync(p => p.Id == id);


                Collect collect = await db.collects.FirstOrDefaultAsync(p => p.Id == item.CollectId);
                ViewBag.collect = collect;

                if (item != null)
                    return View(item);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditItem(Item item)
        {
            db.items.Update(item);
            await db.SaveChangesAsync();
            //return RedirectToAction("Personal");
            return LocalRedirect($"/Home/Items/{idCollect}");
        }

        public IActionResult LikeItem(string tag, int id)
        {
            var itemsLike = db.itemsLike.Where(p => p.ItemId == id && p.NameUser == User.Identity.Name);
            ItemLike itemLike = null;
            foreach (var item in itemsLike.ToList<ItemLike>())
            {
                itemLike = item;
            }
            if (itemLike == null)
            {
                ItemLike itemL = new ItemLike() { ItemId = id, NameUser = User.Identity.Name, Like = true };
                itemLikeData.AddLike(itemL);
            }
            else
            {
                itemLike.Like = itemLike.Like == true ? false : true;
                db.itemsLike.Update(itemLike);
                db.SaveChanges();

            }

            return RedirectToAction("ItemsAll", "Home", new { Id = tag });
        }

        public IActionResult CommentsView(int itemId)
        {
            var commentsList = db.comments.Where(p => p.ItemId == itemId).ToList<Comment>();
            var ItemComments = db.items.Where(p => p.Id == itemId).ToList<Item>();
            ViewBag.Comments = commentsList;
            
            foreach (var item in ItemComments)
            {
                ViewBag.Item = item;
            }
            
            return View();
        }


        public IActionResult AddComment(Comment comment, string itemName)
        {
            comment.DateTimeComment = DateTime.Now;
            comment.UserName = User.Identity.Name;
            commentData.AddComment(comment);
            return RedirectToAction("CommentsView", "Home", new { itemId = comment.ItemId });
        }


        #endregion


        /// <summary>
        /// Adds photo to Cloudinary and saves url in static field
        /// </summary>
        /// <param name="file"></param>
        public void UploadedPhoto(IFormFile file)
        {
            Account account = new Account("boola98", "914541695534158", "zwamf_cagDU1I0kEGiWtpky4Th8");
            Cloudinary cloudinary = new Cloudinary(account);
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };
            var uploadResult = cloudinary.Upload(uploadParams);
            urlPhoto = uploadResult.Url.ToString();
        }


        #region private method

        private string GetStringTags()
        {
            string str = "";
            var tags = db.items.Select(c => c.Tags);
            foreach (var tag in tags)
            {
                //{ text: "Lorem", weight: 13, link: 'http://github.com/mistic100/jQCloud' },
                str += $"{{ text: \"{tag}\", weight: 10, link: 'http://github.com/mistic100/jQCloud' }}, ";
                //str += $"\"{tag}\", ";
            }
            return str;
        }

        private IEnumerable<Item> ReturnUniqueTags()
        {
            var items = db.items.Select(s => s).ToList();
            var itemsList = items.Distinct(new ItemComparer());

            #region
            int i = 999;
            List<Item> itemsListMax = new List<Item> { };
            List<string> alreadyTags = new() { };

            foreach (var item in itemsList)
            {
                if (!item.Tags.Contains(' '))
                {
                    itemsListMax.Add(item);
                }
                else
                {
                    string[] tagsArray = item.Tags.Split(' ');
                    foreach (var tag in tagsArray)
                    {
                        bool bl = ReturnTrueIfTagRepeated(alreadyTags, tag);

                        if (!bl)
                        {
                            alreadyTags.Add(tag);
                            itemsListMax.Add(new Item { Tags = tag, Id = i++ });
                        }
                    }
                }

            }
            return itemsListMax;

            #endregion

        }

        private static bool ReturnTrueIfTagRepeated(List<string> alreadyTags, string tag)
        {
            bool bl = false;
            foreach (var alTag in alreadyTags)
            {
                if (alTag == tag) bl = true;
            }

            return bl;
        }


        #endregion
    }
}
