using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace WpfApplication3.Data.MoneyMarketFund
{
    public static class SecurityDetail
    {
        public static string fundcode = GlobalParameters.fundCode;
        public static string date = GlobalParameters.lastTradingDate;

        public static decimal zbjs_cpamount;        //短融金额
        public static decimal zbjs_cpbl;            //短融比例 keyindex（6，1）
        public static decimal zbjs_floatbndamount;  //浮息债金额
        public static decimal zbjs_floatbndbl;      //浮息债比例 keyindex（7，1）
        public static decimal zbjs_otherbndbl;      //其他利率债比例 keyindex（8，1）
        public static decimal zbjs_bndfy;           //债券总浮盈 keyindex（20，1）
        public static decimal zbjs_bndfk;           //债券总浮亏 keyindex（21，1）
        public static decimal zbjs_zqbl;            //债券比例 keyindex（5，1）bndratio-cdratio 去掉存单

        public static DataTable dt;

        public static decimal assetamount;
        public static decimal assetyield;
        public static decimal weightterm;

        public static DataTable dt_bondTrade;
        public static string[] zq_sellout_out6;
        public static string[] zq_buyin_out4;
        public static string[] zq_dqdf_out7;

        public static void getSecurityDetail()
        {
            SqlHelper sqh = new SqlHelper("con_BondFund");
            //sqh.getConnectString("con_BondFund");
            string sql_str = string.Format(@"
exec bondfund..sp_GetBondDetail_copy '{0}', '{1}';

select *
from Infocenter..fundjszcjhbdata t
where fundcode='{0}' and maturedate>='{2}'
and voucherdate between '{2}' and dateadd(day, 30, getdate ())
and t.zqbz='ZQ'
            ", GlobalParameters.fundCode, GlobalParameters.lastTradingDate,GlobalParameters.Date);
            // 存单明细
            dt = sqh.ExecuteDataSetText(sql_str, null).Tables[0];

            //债券比例 keyindex（5，1）bndratio-cdratio 去掉存单
            zbjs_zqbl = ValuationTable.bndratio - CKCDDetail.zbjs_cdratio;
            //债券总浮盈 keyindex（20，1）
            zbjs_bndfy = dt.Select("总体盈亏>=0").Length == 0 ? 0 : Convert.ToDecimal(dt.Compute("sum(总体盈亏)", "总体盈亏>0").ToString());
            //债券总浮亏 keyindex（21，1）
            zbjs_bndfk = dt.Select("总体盈亏<0").Length == 0 ? 0 : Convert.ToDecimal(dt.Compute("sum(总体盈亏)", "总体盈亏<0").ToString());
            //短融金额
            zbjs_cpamount = dt.Select("债券类型 in ('短融','超短融')").Length == 0 ? 0 : Convert.ToDecimal(dt.Compute("sum(证券市值)", "债券类型 in ('短融','超短融')").ToString());
            //短融比例 keyindex（6，1）
            zbjs_cpbl = dt.Select("债券类型 in ('短融','超短融')").Length == 0 ? 0 : Convert.ToDecimal(dt.Compute("sum(证券市值)", "债券类型 in ('短融','超短融')").ToString())/ValuationTable.zbjs_NAV;
            //浮息债金额
            zbjs_floatbndamount = dt.Select("是否浮息=3").Length == 0 ? 0 : Convert.ToDecimal(dt.Compute("sum(证券市值)", "是否浮息=3").ToString());
            //浮息债比例 keyindex（7，1）
            zbjs_floatbndbl = dt.Select("是否浮息=3").Length == 0 ? 0 : Convert.ToDecimal(dt.Compute("sum(证券市值)", "是否浮息=3").ToString()) / ValuationTable.zbjs_NAV;
            //其他利率债比例 keyindex（8，1）
            zbjs_otherbndbl = dt.Select("债券类型 not in ('短融','超短融')").Length == 0 ? 0 : Convert.ToDecimal(dt.Compute("sum(证券市值)", "债券类型 not in ('短融','超短融')").ToString()) / ValuationTable.zbjs_NAV;

            DataTable dt_copy = dt;
            DataColumn column_tmp_yield = new DataColumn("tmp_yield", typeof(decimal));
            dt_copy.Columns.Add(column_tmp_yield);
            column_tmp_yield.Expression = "证券市值*到期收益率";

            DataColumn column_tmp_term = new DataColumn("tmp_term", typeof(decimal));
            dt_copy.Columns.Add(column_tmp_term);
            column_tmp_term.Expression = "证券市值*剩余期限";

            assetamount = decimal.Round(Convert.ToDecimal(dt_copy.Compute("sum(证券市值)", "true").ToString()), 0);
            assetyield = decimal.Round(Convert.ToDecimal(dt_copy.Compute("sum(tmp_yield)", "true").ToString()) / assetamount, 4);
            weightterm = decimal.Round(Convert.ToDecimal(dt_copy.Compute("sum(tmp_term)", "true").ToString()) / assetamount, 1);

            foreach (DataRow dr in dt.Rows)
            {
                dr["持仓数量"] = decimal.Parse(dr["持仓数量"].ToString()).ToString("0");
                dr["证券市值"] = decimal.Parse(dr["证券市值"].ToString()).ToString("0.00");
                dr["市值比净值"] = decimal.Parse(dr["市值比净值"].ToString()).ToString("0.00");
                dr["总体盈亏"] = decimal.Parse(dr["总体盈亏"].ToString()).ToString("0.00");
                dr["单位盈亏"] = decimal.Parse(dr["单位盈亏"].ToString()).ToString("0.000");
                dr["票面利率"] = decimal.Parse(dr["票面利率"].ToString()).ToString("0.00");
                dr["到期收益率"] = decimal.Parse(dr["票面利率"].ToString()).ToString("0.00");

            }

            //头寸：债券到期兑付
            DataColumn column_tmp_interest = new DataColumn("tmp_interest", typeof(decimal));
            dt_copy.Columns.Add(column_tmp_interest);
            column_tmp_interest.Expression = "持仓数量*(1+票面利率/100)";

            zq_dqdf_out7 = new string[13];

            for (int i = 0; i < 13; i++)
            {
                string zq_dqdf_sqlStr = string.Format(@"到期日='{0}'", GlobalParameters.TradingDateSeries[i].ToString());
                zq_dqdf_out7[i] = dt.Select(zq_dqdf_sqlStr).Length == 0 ? null : decimal.Round(Convert.ToDecimal(dt.Compute("sum(tmp_interest)", zq_dqdf_sqlStr).ToString()), 1).ToString();
            }
            
            //头寸：债券买卖
            string str_bond_buyin_sellout;
            zq_buyin_out4 = new string[13];
            zq_sellout_out6 = new string[13];

            dt_bondTrade = sqh.ExecuteDataSetText(sql_str, null).Tables[1];
            foreach (DataRow dr in dt_bondTrade.Rows)
            {
                if (Convert.ToDateTime(dr["voucherdate"].ToString()) >= Convert.ToDateTime(GlobalParameters.Date))
                {
                    for (int i = 1; i < 13; i++)
                    {
                        str_bond_buyin_sellout = string.Format("voucherdate='{0}' and entrust_bs='{1}'"
                            , GlobalParameters.TradingDateSeries[i], "1");//债券买入
                        zq_buyin_out4[i] = dt_bondTrade.Select(str_bond_buyin_sellout).Length == 0 ? null : decimal.Round(Convert.ToDecimal(dt_bondTrade.Compute("sum(ghbalance)/10000", str_bond_buyin_sellout).ToString()), 1).ToString();

                        str_bond_buyin_sellout = string.Format("voucherdate='{0}' and entrust_bs='{1}'"
                            , GlobalParameters.TradingDateSeries[i], "2");//债券卖出
                        zq_sellout_out6[i] = dt_bondTrade.Select(str_bond_buyin_sellout).Length == 0 ? null : decimal.Round(Convert.ToDecimal(dt_bondTrade.Compute("sum(ghbalance)/10000", str_bond_buyin_sellout).ToString()), 1).ToString();
                    }
                }
            }
        }
    }
}
