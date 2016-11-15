using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication3
{
    class Security : AbstractPortfolio
    {
        public new string code { get; set; }
        public new string name { get; set; }
        public new string type { get; set; }

        public Measures measure { get; set; }

        public Security(string name):base(name)
        {
        }

        public override void GetSubPortfolio()
        {
            Console.WriteLine(name);
        }
    }
}
