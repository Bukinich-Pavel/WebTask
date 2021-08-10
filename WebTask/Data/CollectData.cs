using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTask.Interfaces;
using WebTask.Models;

namespace WebTask.Data
{
    public class CollectData : ICollectData
    {

        private readonly ApplicationContext context;

        public CollectData(ApplicationContext Context)
        {
            this.context = Context;
        }

        public void AddCollect(Collect collect)
        {
            context.collects.Add(collect);
            context.SaveChanges();
        }


        public IEnumerable<Collect> GetCollect()
        {
            return this.context.collects;
        }


    }
}
