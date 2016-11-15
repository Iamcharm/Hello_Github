using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication3
{
    class Portfolio:AbstractPortfolio
    {

        public new string code { get; set; }
        public new string name { get; set; }
        public new string type { get; set; }


        public Dictionary<string, Security> securities { get; set; }

        public List<AbstractPortfolio> portfolioList = new List<AbstractPortfolio>();

        public Portfolio(string name):base(name)
        {
        }
        public void Add(AbstractPortfolio portfolio)
        {
            portfolioList.Add(portfolio);
        }
        public void Remove(AbstractPortfolio portfolio)
        {
            portfolioList.Remove(portfolio);
        }
        public void Modify(AbstractPortfolio portfolio)
        {
        }
        public override void GetSubPortfolio()
        {
            foreach (AbstractPortfolio p in portfolioList)
            {
                p.GetSubPortfolio();
            }
        }
    }
}
