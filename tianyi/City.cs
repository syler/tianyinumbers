using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tianyi
{
    public class City
    {
        public City(string name , string code)
        {
            this.Name = name;
            this.Code = code;
        }
        public string Name
        {
            get;
            private set;
        }

        public string Code
        {
            get;
            private set;
        }
    }
}
