using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTask.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string NameItem { get; set; }
        public string Tags { get; set; }
        public int integer1 { get; set; }
        public int integer2 { get; set; }
        public int integer3 { get; set; }
        public string string1 { get; set; }
        public string string2 { get; set; }
        public string string3 { get; set; }
        public string text1 { get; set; }
        public string text2 { get; set; }
        public string text3 { get; set; }
        public DateTime data1 { get; set; }
        public DateTime data2 { get; set; }
        public DateTime data3 { get; set; }
        public bool checkbox1 { get; set; }
        public bool checkbox2 { get; set; }
        public bool checkbox3 { get; set; }

        public int? CollectId { get; set; }
        public Collect сollect { get; set; }



    }
}
