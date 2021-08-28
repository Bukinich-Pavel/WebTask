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
            ViewBag.itemsforCloud = StatMethod.ReturnUniqueTags(db);
            return View(collectData.GetCollect());
        }

        public IActionResult LikeItem(string tag, int id)
        {
            var itemsLike = db.itemsLike.Where(p => p.ItemId == id && p.NameUser == User.Identity.Name).ToList<ItemLike>();
            ItemLike itemLike = null;
            foreach (var item in itemsLike)
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
            
            return RedirectToAction("ItemsAll", "Item", new { Id = tag });
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

        public IActionResult AddComment(Comment comment)
        {
            comment.DateTimeComment = DateTime.Now;
            comment.UserName = User.Identity.Name;
            commentData.AddComment(comment);
            return new NoContentResult();
            //return RedirectToAction("CommentsView", "Home", new { itemId = comment.ItemId });
        }
    }

    static class StatMethod
    {
        public static IEnumerable<Item> ReturnUniqueTags(ApplicationContext db)
        {
            var items = db.items.Select(s => s).ToList();
            var itemsList = items.Distinct(new ItemComparer());

            #region
            int i = 999;
            List<Item> itemsListMax = new List<Item> { };
            List<string> alreadyTags = new() { };

            foreach (var item in itemsList)
            {
                if (item.Tags == null) continue;
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

    }

}
