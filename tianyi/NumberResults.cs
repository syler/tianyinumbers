/*
 * this class is created from http://json2csharp.com/
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tianyi
{
    public class Obj
    {
        public string baiscPrice { get; set; }
        public string cardId { get; set; }
        public string cardPrice { get; set; }
        public string cardSizeType { get; set; }
        public string cityCode { get; set; }
        public int currentPage { get; set; }
        public string hbid { get; set; }
        public string hyid { get; set; }
        public string isCard { get; set; }
        public string isEnable { get; set; }
        public bool isPretty { get; set; }
        public string name { get; set; }
        public string nowMin { get; set; }
        public string number { get; set; }
        public string numberCode { get; set; }
        public string numberlevel { get; set; }
        public string offerCombName { get; set; }
        public string oldNumber { get; set; }
        public string prettyName { get; set; }
        public string price { get; set; }
        public string terminalPrice { get; set; }
        public int totalPage { get; set; }
        public string totalPrice { get; set; }
    }

    public class RootObject
    {
        public int count { get; set; }
        public string isOk { get; set; }
        public string message { get; set; }
        public int now { get; set; }
        public List<Obj> obj { get; set; }
    }
}
