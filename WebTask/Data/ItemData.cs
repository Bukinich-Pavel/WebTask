using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTask.Interfaces;
using WebTask.Models;

namespace WebTask.Data
{
    public class ItemData : IItemData
    {
        private readonly ApplicationContext context;

        public ItemData(ApplicationContext context)
        {
            this.context = context;
        }
        public void AddCollect(Item item)
        {
            context.items.Add(item);
            context.SaveChanges();
        }

        public IEnumerable<Item> GetCollect()
        {
            return this.context.items;
        }
    }
}
