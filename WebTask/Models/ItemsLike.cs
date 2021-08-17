using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTask.Models
{
    public class ItemsLike
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int IdUser { get; set; }
        public bool Like { get; set; }
    }
}
