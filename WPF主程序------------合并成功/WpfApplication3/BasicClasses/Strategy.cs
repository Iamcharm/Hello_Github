using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication3
{
    class Strategy
    {
        private Measures _measure;
        /// <summary>
        /// 策略执行函数
        /// </summary>
        /// <param name="portfolio"></param>
        public Measures ComputeMeasure1(AbstractPortfolio portfolio)
        {
            return _measure;
        }
        public Measures ComputeMeasure2(AbstractPortfolio portfolio1, AbstractPortfolio portfolio2)
        {
            return _measure;
        }
    }
}
