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
        private ApplicationContext db;

        static string urlPhoto;

        protected string MyProperty { get { return "your value"; } }

        public HomeController(ICollectData collectData, IItemData itemData, ApplicationContext context)
        {
            this.collectData = collectData;
            this.itemData = itemData;
            db = context;
        }


        /// <summary>
        /// Show All
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {

            ViewBag.var1 = db.items.Select(c => c.Tags);

            return View(collectData.GetCollect());
        }

        /// <summary>
        /// Show your collections
        /// </summary>
        /// <returns></returns>
        public IActionResult Personal()
        {

            return View(collectData.GetCollect());

        }

        /// <summary>
        /// Show List Items
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Items(int? Id)
        {
            ViewBag.Collect = await db.collects.FirstOrDefaultAsync(p => p.Id == Id);

            var items = db.items.Where(p => p.CollectId == Id);

            ViewBag.Items = items;
            return View();

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
                itemData.AddCollect(item);
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
            return RedirectToAction("Personal");
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


        #endregion
    }
}
