using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication3
{
    abstract class AbstractPortfolio
    {
        public string code { get; set; }
        public string name { get; set; }
        public string type { get; set; }

        public AbstractPortfolio(string name)
        {
            this.name = name;
        }
        public abstract void GetSubPortfolio();
    }
}
