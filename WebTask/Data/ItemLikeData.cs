using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTask.Interfaces;
using WebTask.Models;

namespace WebTask.Data
{
    public class ItemLikeData : IItemLikeData
    {
        private readonly ApplicationContext context;

        public ItemLikeData(ApplicationContext context)
        {
            this.context = context;
        }

        public void AddLike(ItemLike like)
        {
            context.itemsLike.Add(like);
            context.SaveChanges();
        }

        public IEnumerable<ItemLike> GetLike()
        {
            return this.context.itemsLike;
        }
    }
}
