using System;
using WebTask.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WebTask.Interfaces
{
    public interface IItemData
    {
        IEnumerable<Item> GetItem();
        void AddItem(Item item);

    }
}
