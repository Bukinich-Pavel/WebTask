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
    public class ItemController : Controller
    {
        private ApplicationContext db;
        private readonly IItemData itemData;
        private readonly IItemLikeData itemLikeData;


        public ItemController(IItemData itemData, IItemLikeData itemLikeData, ApplicationContext context)
        {
            this.itemData = itemData;
            this.itemLikeData = itemLikeData;
            db = context;
        }

        public async Task<IActionResult> Items(int Id)
        {
            ViewBag.itemsforCloud = StatMethod.ReturnUniqueTags(db);

            ViewBag.Collect = await db.collects.FirstOrDefaultAsync(p => p.Id == Id);

            var items = db.items.Where(p => p.CollectId == Id);

            ViewBag.Items = items;
            return View();
        }

        public IActionResult ItemsAll(string Id)
        {
            ViewBag.var1 = StatMethod.ReturnUniqueTags(db);
            ViewBag.Tag = Id;
            ViewBag.items = db.items.Where(p => p.Tags.Contains(Id));
            ViewBag.like = db.itemsLike.Where(p => p.NameUser == User.Identity.Name).ToList<ItemLike>();
            return View(itemLikeData.GetLike());
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

        public ActionResult AutocompleteSearch(string term)
        {
            var models = db.items.Where(a => a.Tags.Contains(term))
                            .Select(a => new { value = a.Tags })
                            .Distinct();

            return Json(models);
        }

        [HttpPost]
        public IActionResult AddItemInCollect(Item item)
        {
            if (ModelState.IsValid && item.NameItem != null && item.Tags != null)
            {
                itemData.AddItem(item);
            }
            return RedirectToAction("Personal", "Collect");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id != null)
            {
                var items = db.items.Where(p => p.Id == id);
                Item item = new Item();
                foreach (var i in items)
                {
                    item = i;
                }
                //Item item = new Item { Id = id.Value };
                db.Entry(item).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                //return LocalRedirect($"/Item/Items/{item.CollectId.ToString()}");
                return RedirectToAction("Items", "Item", new { Id = item.CollectId });

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
            if (item.Tags == null)
            {
                return LocalRedirect($"/Item/Items/{item.CollectId.ToString()}");
            }
            db.items.Update(item);
            await db.SaveChangesAsync();

            return LocalRedirect($"/Item/Items/{item.CollectId.ToString()}");
        }

    }

}
