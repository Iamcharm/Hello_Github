using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication3.Data.InvestmentAnalysisTools
{
    public class YieldCurve
    {
        private SortedList<double, double> CurvePoints = new SortedList<double, double>();
        public void AddPoint(double bondterm, double bondyield)
        {
            CurvePoints.Add(bondterm, bondyield);
        }
        public double YiledOnCurve(double bondterm)
        {
            double bondyield;
            int i = 0;
            List<double> Lbndterm = this.CurvePoints.Keys.ToList();
            double minterm = Lbndterm[0];
            double maxterm = Lbndterm[CurvePoints.Count - 1];
            if (CurvePoints.Count <= 1)   //如果曲线上的点不足两个，返回-100
            {
                bondyield = -100;
            }
            else if (bondterm > maxterm || bondterm < minterm)    //如果需要计算的期限不在收益率曲线的有效范围内，返回-100
            {
                bondyield = -100;
            }
            else    //用线性插值计算收益率
            {
                do
                {
                    i++;
                } while (Lbndterm[i] <= bondterm);
                double term1 = Lbndterm[i - 1];
                double term2 = Lbndterm[i];
                double yield1, yield2;
                CurvePoints.TryGetValue(term1, out yield1);
                CurvePoints.TryGetValue(term2, out yield2);
                bondyield = yield1 + (bondterm - term1) / (term2 - term1) * (yield2 - yield1);
            }
            return bondyield;
        }

        public void SteepChange(int ChangeBp)   //曲线增陡，从收益率曲线第一个点开始计算，每个期限加上一定的价差
        {
            if (this.CurvePoints.Count >= 1)
            {
                for (int i = 1; i < this.CurvePoints.Count; i++)

                {
                    List<double> Lbndterm = this.CurvePoints.Keys.ToList();
                    double term = Lbndterm[i];
                    double yield;
                    CurvePoints.TryGetValue(term, out yield);
                    if (Math.Round(yield) == 0)  //判断收益率是用百分比表示还是绝对值表示，如0.05（5%）或5
                    {
                        yield += (double)(ChangeBp * i) / 10000;
                    }
                    else
                    {
                        yield += (double)(ChangeBp * i) / 100;
                    }
                    CurvePoints[term] = yield;
                }
            }

        }

        public void FlatChange(int ChangeBp)   //曲线变平，从收益率曲线第一个点开始计算，每个期限加上一定的价差
        {
            if (this.CurvePoints.Count >= 1)
            {
                for (int i = 1; i < this.CurvePoints.Count; i++)
                {
                    List<double> Lbndterm = this.CurvePoints.Keys.ToList();
                    double term = Lbndterm[i];
                    double yield;
                    CurvePoints.TryGetValue(term, out yield);
                    if (Math.Round(yield) == 0)
                    {
                        yield -= (double)(ChangeBp * i) / 10000;
                    }
                    else
                    {
                        yield -= (double)(ChangeBp * i) / 100;
                    }
                    CurvePoints[term] = yield;
                }
            }
        }
        public void ParallelChange(int ChangeBp)   //曲线平移，正值表示向上平移，负值表示向下平移
        {
            for (int i = 0; i < this.CurvePoints.Count; i++)
            {
                List<double> Lbndterm = this.CurvePoints.Keys.ToList();
                double term = Lbndterm[i];
                double yield;
                CurvePoints.TryGetValue(term, out yield);
                if (Math.Round(yield) == 0)
                {
                    yield += (double)ChangeBp / 10000;
                }
                else
                {
                    yield += (double)ChangeBp / 100;
                }
                CurvePoints[term] = yield;
            }
        }

        public Array GetCurveValue()
        {
            List<double> term = this.CurvePoints.Keys.ToList();
            List<double> yield = this.CurvePoints.Values.ToList();
            int i = this.CurvePoints.Count;
            double[,] cv = new double[i, 2];
            for (i = 0; i < this.CurvePoints.Count; i++)
            {
                cv[i, 0] = term[i];
                cv[i, 1] = yield[i];
            }
            return cv;
            //List<double> yield = this.CurvePoints.Values;
        }
    }

    public class Bond
    {
        public string BndCode { get; set; }
        public string BndName { get; set; }
        public double CouponRate { get; set; }
        public int PayFrequency { get; set; }
        public double ParValue { get; set; }
        public double NetPrice { get; set; }
        public string MatureDate { get; set; }

        public double YieldtoMaturity { get; set; }

        public double GetYtm(double netprice, DateTime sdate)
        {
            double r_down = 0.001;
            double r_up = 0.8;
            double yield = (r_down + r_up) / 2;
            DateTime mdate = Convert.ToDateTime(this.MatureDate);
            double bndprice = GetBondPrice(this.CouponRate, sdate.Date, mdate, yield, this.ParValue, this.PayFrequency);
            int i = 0;
            while (i <= 1000)
            {
                if (Math.Abs(bndprice - netprice) >= 0.001)
                {
                    if (bndprice < netprice) { r_up = yield; }
                    else { r_down = yield; }
                }
                else { break; }
                yield = (r_down + r_up) / 2;
                bndprice = GetBondPrice(this.CouponRate, sdate.Date, mdate, yield, this.ParValue, this.PayFrequency);
                i++;
            }
            return yield;
        }

        public double GetBondPrice(double coupon, DateTime SettleDate, DateTime mdate, double ytm, double par, int num)
        {
            TimeSpan ts = mdate - SettleDate;
            if (ts.Days <= 0) { return -100; }
            else
            {
                double ttm = (double)ts.Days / 365;
                int i = 1;
                double bndprice = 0;
                if (Math.Round(coupon) == 0) { coupon = coupon * 100; }
                switch (num)
                {
                    case 0:
                    case 1:
                        bndprice = coupon * (ttm - Math.Round(ttm)) / Math.Pow((1 + ytm), (ttm - Math.Round(ttm)));
                        while (i - Math.Round(ttm) <= 0)
                        {
                            bndprice = bndprice + coupon / Math.Pow((1 + ytm), (i + ttm - Math.Round(ttm)));
                            i++;
                        }
                        break;
                    case 2:
                        if (ttm - Math.Round(ttm) >= 0.5)
                        {
                            bndprice = coupon / 2 * (ttm - Math.Round(ttm)) / Math.Pow((1 + ytm), (ttm - Math.Round(ttm) - 0.5));
                        }
                        while (i - Math.Round(ttm) * 2 <= 0)
                        {
                            bndprice = bndprice + coupon / 2 / Math.Pow((1 + ytm), (i / 2 + ttm - Math.Round(ttm) - 0.5));
                            i++;
                        }
                        break;
                    case 4:
                        if (ttm - Math.Round(ttm) >= 0.75)
                        {
                            bndprice = coupon / 4 * (ttm - Math.Round(ttm)) / Math.Pow((1 + ytm), (ttm - Math.Round(ttm) - 0.75));
                        }
                        while (i - Math.Round(ttm) * 4 <= 0)
                        {
                            bndprice = bndprice + coupon / 4 / Math.Pow((1 + ytm), (i / 4 + ttm - Math.Round(ttm) - 0.75));
                            i++;
                        }
                        break;
                }
                bndprice = bndprice + par / Math.Pow((1 + ytm), ttm);
                //coupon = coupon / 100;
                return bndprice;
            }
        }


        public double GetBondDuration(DateTime sdate, double ytm)
        {
            DateTime mdate = Convert.ToDateTime(this.MatureDate);
            double p0, p1, p2, delta_r;
            delta_r = 0.0001;
            p0 = GetBondPrice(this.CouponRate, sdate.Date, mdate, ytm, this.ParValue, this.PayFrequency);
            p1 = GetBondPrice(this.CouponRate, sdate.Date, mdate, ytm + delta_r, this.ParValue, this.PayFrequency);
            p2 = GetBondPrice(this.CouponRate, sdate.Date, mdate, ytm - delta_r, this.ParValue, this.PayFrequency);
            double bnddur = (p2 - p1) / (2 * p0 * delta_r);
            return bnddur;
        }

    }

}
