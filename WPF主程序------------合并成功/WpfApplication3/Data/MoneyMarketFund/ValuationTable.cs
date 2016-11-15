using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace WpfApplication3.Data.MoneyMarketFund
{
    public static class ValuationTable
    {

        public static string fundcode = GlobalParameters.fundCode;
        public static string gzdate = GlobalParameters.lastTradingDate;

        public static decimal zbjs_hqck;//活期存款
        public static decimal zbjs_dqck;//定期存款
        public static decimal zbjs_dcbl;//定存比例 keyindex（3，1）
        public static decimal bndratio; //债券投资比例（包含存单）
        public static decimal zbjs_rqbl;//融券回购：融券投资比例 keyindex（9，1）
        public static decimal zbjs_yhjrqhg;//银行间融券回购金额
        public static decimal zbjs_yhjrqhgbl;//银行间融券回购比例
        public static decimal zbjs_yssgk;//应收申购款
        public static decimal zbjs_rzbl;//融资回购：融资投资比例 keyindex（11，1）
        public static decimal zbjs_yhjrzhg;//银行间融资回购金额
        public static decimal zbjs_yhjrzhgbl;//银行间融资回购比例 keyindex（11，1）???
        public static decimal zbjs_ysshk;//应收赎回款
        public static decimal zbjs_NAV;//基金资产净值（上一日） keyindex（1，1）
        public static decimal zbjs_plje;//偏离金额(盈亏总额) keyindex（19，1）
        public static string  str_zbjs_pld;//偏离金额 (带%字符串转换成数字)
        public static decimal zbjs_pld;//偏离金额 keyindex（18，1）
        public static decimal zbjs_sytq;//剩余天期 keyindex（13，1）
        public static string  str_zbjs_xjlzcbl;//现金类资产比例 (带%字符串转换成数字)
        public static decimal zbjs_xjlzcbl;//现金类资产比例 keyindex（10，1）
        public static decimal zbjs_wfsyA;//万份收益 keyindex（14，1） keyindex（15，1）
        public static string  str_zbjs_7dnhA;//七日年化 (带%字符串转换成数字)
        public static decimal zbjs_7dnhA;//七日年化 keyindex（16，1） keyindex（17，1）
        public static decimal zbjs_jss;//净申赎 keyindex（10，1）
        public static decimal zbjs_yhjhgbl;//银行间回购比例 keyindex（12，1）
        public static decimal zbjs_wysyyxwfsy;//万元损益影响万份收益 keyindex（24，1）
        public static decimal zbjs_wysyyx7dnh;//万元损益影响7日年化 keyindex（25，1）
        public static decimal zbjs_1bppldtzdyje;//1bp偏离度调整对应金额 keyindex（26，1）

        public static decimal zbjs_netss;//净申赎 keyindex（2，1）
        public static decimal zbjs_netss_before1;//上日净申赎 begin_cashflow（3，1）

        public static decimal zbjs_jsbfj;//结算备付金
        public static decimal ysglf;//应收管理费
        public static decimal ystgf;//应收托管费
        public static decimal ysxsfwf;//应收销售服务费
        public static decimal ysjybfj;//应收交易备付金

        public static void getValuationTable()
        {
            SqlHelper sqh = new SqlHelper("con_Infocenter");
            //sqh.getConnectString("con_Infocenter");
            string sql_str = string.Format(@"
select * from Infocenter..fundjszgzdata 
where fundcode='{0}' and gzdate='{1}' order by finance_subject
            ", GlobalParameters.fundCode, GlobalParameters.lastTradingDate);
            //估值表
            DataTable dt = sqh.ExecuteDataSetText(sql_str, null).Tables[0];

            int l = dt.Select("finance_subject='1202'").Length;
            //活期存款
            zbjs_hqck = dt.Select("finance_subject='100201'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Select("finance_subject='100201'")[0]["stock_asset"].ToString()) / 10000, 1);
            //定期存款
            zbjs_dqck = dt.Select("finance_subject='100203'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Select("finance_subject='100203'")[0]["stock_asset"].ToString()) / 10000, 2);
            //定存比例 未除100
            zbjs_dcbl = dt.Select("finance_subject='100203'").Length == 0 ? 0 : Convert.ToDecimal(dt.Select("finance_subject='100203'")[0]["asset_ratio"].ToString());
            //债券投资比例（包含存单）
            bndratio = dt.Select("finance_subject='1103'").Length == 0 ? 0 : Convert.ToDecimal(dt.Select("finance_subject='1103'")[0]["asset_ratio"].ToString());
            //融券回购：融券投资比例
            zbjs_rqbl = dt.Select("finance_subject='1202'").Length == 0 ? 0 : Convert.ToDecimal(dt.Select("finance_subject='1202'")[0]["asset_ratio"].ToString());
            //银行间融券回购金额
            zbjs_yhjrqhg = dt.Select("finance_subject='120203'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Select("finance_subject='120203'")[0]["stock_asset"].ToString()) / 10000, 2);
            //银行间融券回购比例
            zbjs_yhjrqhgbl = dt.Select("finance_subject='120203'").Length == 0 ? 0 : Convert.ToDecimal(dt.Select("finance_subject='120203'")[0]["asset_ratio"].ToString());
            //应收申购款
            zbjs_yssgk = dt.Select("finance_subject='1207'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Select("finance_subject='1207'")[0]["stock_asset"].ToString()) / 10000,2);
            //融资回购：融资投资比例
            zbjs_rzbl = dt.Select("finance_subject='2202'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Select("finance_subject='2202'")[0]["asset_ratio"].ToString()), 2);
            //银行间融券回购金额
            zbjs_yhjrzhg = dt.Select("finance_subject='220203'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Select("finance_subject='220203'")[0]["stock_asset"].ToString()) / 10000, 2);
            //银行间融资回购比例
            zbjs_yhjrzhgbl = dt.Select("finance_subject='220203'").Length == 0 ? 0 : Convert.ToDecimal(dt.Select("finance_subject='220203'")[0]["asset_ratio"].ToString());
            //应收赎回款
            zbjs_ysshk = dt.Select("finance_subject='2203'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Select("finance_subject='2203'")[0]["stock_asset"].ToString()) / 10000,2);
            //基金资产净值（上一日）
            zbjs_NAV = dt.Select("finance_subject='701基金资产净值：'").Length == 0 ? 0 : Convert.ToDecimal(dt.Select("finance_subject='701基金资产净值：'")[0]["stock_asset"].ToString()) / 10000;
            //偏离金额(盈亏总额)
            zbjs_plje = dt.Select("finance_subject='803偏离金额：'").Length == 0 ? 0 : Convert.ToDecimal(dt.Select("finance_subject='803偏离金额：'")[0]["finance_name"].ToString()) / 10000;
            //偏离度
            str_zbjs_pld = dt.Select("finance_subject='804偏离度：'").Length == 0 ? "0" : dt.Select("finance_subject='804偏离度：'")[0]["finance_name"].ToString();
            zbjs_pld = Convert.ToDecimal(str_zbjs_pld.Substring(0, str_zbjs_pld.Length - 1));
            //剩余天期 keyindex（13，1）
            zbjs_sytq = dt.Select("finance_subject='908投资组合平均到期日：'").Length == 0 ? 0 : Convert.ToDecimal(dt.Select("finance_subject='908投资组合平均到期日：'")[0]["finance_name"].ToString());
            //现金类资产比例 keyindex（10，1）
            str_zbjs_xjlzcbl = dt.Select("finance_subject='920现金类占净值比：'")[0]["finance_name"].ToString();
            zbjs_xjlzcbl = Convert.ToDecimal(str_zbjs_xjlzcbl.Substring(0, str_zbjs_xjlzcbl.Length - 1));
            //万份收益与七日年化

            if (GlobalParameters.portfolio_1.ContainsKey(GlobalParameters.fundCode))
            {
                string expr_wfsyA = string.Format(@"finance_subject like ('902每万份基金收益：')", GlobalParameters.fundCode);
                string expr_7dnhA = string.Format(@"finance_subject like ('907基金七日收益率：')", GlobalParameters.fundCode);
                zbjs_wfsyA = Convert.ToDecimal(dt.Select(expr_wfsyA)[0]["finance_name"].ToString());
                str_zbjs_7dnhA = dt.Select(expr_7dnhA)[0]["finance_name"].ToString();
                zbjs_7dnhA = Convert.ToDecimal(str_zbjs_7dnhA.Substring(0, str_zbjs_7dnhA.Length - 1));
            }
            else if (GlobalParameters.portfolio_2.ContainsKey(GlobalParameters.fundCode))
            {
                string expr_wfsyA = string.Format(@"finance_subject like ('902——其中每万份{0}基金收益：')", GlobalParameters.fundCode);
                string expr_7dnhA = string.Format(@"finance_subject like ('907——其中{0}基金七日收益率：')", GlobalParameters.fundCode);
                zbjs_wfsyA = Convert.ToDecimal(dt.Select(expr_wfsyA)[0]["finance_name"].ToString());
                str_zbjs_7dnhA = dt.Select(expr_7dnhA)[0]["finance_name"].ToString();
                zbjs_7dnhA = Convert.ToDecimal(str_zbjs_7dnhA.Substring(0, str_zbjs_7dnhA.Length - 1));
            }

            //净申赎
            zbjs_jss = zbjs_yssgk - zbjs_yssgk;
            //银行间回购比例 keyindex（12，1）
            zbjs_yhjhgbl = zbjs_yhjrzhgbl + zbjs_yhjrqhgbl;
            //万元损益影响万份收益 keyindex（24，1）
            zbjs_wysyyxwfsy = 10000 / zbjs_NAV;
            //万元损益影响7日年化 keyindex（25，1）
            zbjs_wysyyx7dnh = zbjs_wysyyxwfsy * 360 / 10000;
            //1bp偏离度调整对应金额 keyindex（26，1）
            zbjs_1bppldtzdyje = zbjs_NAV / 10000;

            //净申赎 keyindex（2，1）
            zbjs_netss = GlobalParameters.fundCode == "159005" ? getZbjs_netss(GlobalParameters.lastTradingDate, ValuationTable.fundcode) : (zbjs_yssgk - zbjs_ysshk);

            //上日净申赎 begin_cashflow（3，1）
            zbjs_netss_before1 = GlobalParameters.fundCode == "159005" ? getZbjs_netss(GlobalParameters.lastTradingDate,ValuationTable.fundcode) : 0;


            //结算备付金
            zbjs_jsbfj = dt.Select("finance_subject='1021'").Length==0?0:decimal.Round(Convert.ToDecimal(dt.Select("finance_subject='1021'")[0]["stock_asset"].ToString()) / 10000, 0);
            //应收管理费
            ysglf = dt.Select("finance_subject='2206'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Select("finance_subject='2206'")[0]["stock_asset"].ToString()) / 10000, 2);
            //应收托管费
            ystgf = dt.Select("finance_subject='2207'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Select("finance_subject='2207'")[0]["stock_asset"].ToString()) / 10000, 2);
            //应收销售服务费
            ysxsfwf = dt.Select("finance_subject='2208'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Select("finance_subject='2208'")[0]["stock_asset"].ToString()) / 10000, 2);
            //应收交易备付金
            ysjybfj = dt.Select("finance_subject='950预算最低备付金：'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Select("finance_subject='950预算最低备付金：'")[0]["finance_name"].ToString()) / 10000, 2);



        }
        public static decimal getZbjs_netss(string fundCode, string tradingDate)
        {
            decimal netss=0;
            SqlHelper sqh = new SqlHelper("con_Infocenter");
            //sqh.getConnectString("con_Infocenter");
            string sql_str = string.Format(@"
select sum(a.HBCJSL * 100.0 * case a.HBYWLB when 'KS' then 1.0 when 'KB' then -1.0 else 0 end)/10000
from (select * from user2009..szetfreturn where HBCJRQ = '{0}' and HBZQDM = '{1}' and HBYWLB in ('KB','KS')
union select * from user2009..szetfreturn_his where HBCJRQ ='{0}'and HBZQDM ='{1}'
and HBYWLB in ('KB','KS')) a group by a.HBCJRQ, a.HBZQDM
            ", fundCode, tradingDate);

            DataTable dt = sqh.ExecuteDataSetText(sql_str, null).Tables[0];
            netss = dt.Rows.Count == 0 ? 0:decimal.Round(Convert.ToDecimal(dt.Rows[0][0].ToString()) , 2);
            return netss;
        }
    }
}
