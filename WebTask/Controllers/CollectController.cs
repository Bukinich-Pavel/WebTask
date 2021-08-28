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
    public class CollectController : Controller
    {
        private readonly ICollectData collectData;
        private ApplicationContext db;
        static string urlPhoto;
        string emptyPhoto = "https://res.cloudinary.com/boola98/image/upload/v1629834503/no-photo_nclqfy.png";


        public CollectController(ICollectData collectData, ApplicationContext context)
        {
            this.collectData = collectData;
            db = context;
        }

        public IActionResult Personal()
        {
            ViewBag.itemsforCloud = StatMethod.ReturnUniqueTags(db);
            return View(collectData.GetCollect());
        }

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
                collect.Photo = urlPhoto == null ? emptyPhoto : urlPhoto;
                urlPhoto = null;
                collect.NameAuthor = User.Identity.Name;
                collectData.AddCollect(collect);
            }
            return RedirectToAction("Personal", "Collect");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCollect(int? id)
        {
            if (id != null)
            {

                Collect collect = new Collect { Id = id.Value };
                db.Entry(collect).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Personal");
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

    }
}
