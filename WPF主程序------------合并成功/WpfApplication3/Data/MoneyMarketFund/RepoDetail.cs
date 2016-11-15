using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace WpfApplication3.Data.MoneyMarketFund
{
    public static class RepoDetail
    {
        public static string fundcode = GlobalParameters.fundCode;
        public static string date = GlobalParameters.Date;
        public static DataTable dt;

        public static DataRow[] dt_HG;//购回金额（本金+利息）

        //指标试算
        public static decimal RZrepo_forzbss;
        public static decimal YHJrepo_forzbss;

        //当日资产收益统计
        public static decimal assetamount_rz;
        public static decimal assetyield_rz;
        public static decimal weightterm_rz;

        public static decimal assetamount_rq;
        public static decimal assetyield_rq;
        public static decimal weightterm_rq;
        //头寸
        public static string[] jys_HG_dq_in2;
        public static string[] jys_HG_rz_in3;
        public static string[] yhj_HG_dq_in4;
        public static string[] yhj_HG_rz_in5;

        public static string[] jys_HG_rq_out0;
        public static string[] jys_HG_dq_out1;
        public static string[] yhj_HG_rq_out2;
        public static string[] yhj_HG_dq_out3;

        public static DataTable getRepoDetail()
        {
            SqlHelper sqh = new SqlHelper("con_BondFund");
            //sqh.getConnectString("con_BondFund");
            string sql_str = string.Format(@"
select (ghbalance + hggain)/10000 as '金额', 
case when voucherdate!=maturedate then ((ghbalance + hggain) / real_balance - 1) / datediff(day,voucherdate,maturedate) * 356 
	 else 0
	 end as '利率',
datediff(day,voucherdate,maturedate) as '期限',
dealdate as '交易日',
case when dealdate=voucherdate then 'T+0'
	 else 'T+1'
	 end as '结算效率',
case when exchange_type=5 then '银行间'
	 else '交易所'
	 end as '市场',
case when exchange_type=5 and entrust_bs=1 then '融券'
	 when exchange_type=5 and entrust_bs=2 then '融资'
	 when exchange_type=1 and entrust_bs=2 then '融券'
	 when exchange_type=1 and entrust_bs=1 then '融资'
	 end as '交易方向',
voucherdate as '结算日',maturedate as '回购交割日',datediff(day,'{1}',maturedate) as '剩余天期'
from Infocenter..fundjszcjhbdata t
where fundcode='{0}' and maturedate>='{1}'
and zqbz='HG'
--and voucherdate<'{1}' and maturedate>='{1}'
            ", GlobalParameters.fundCode, GlobalParameters.Date);
            //回购明细
            dt = sqh.ExecuteDataSetText(sql_str, null).Tables[0];

            //指标试算
            RZrepo_forzbss = dt.Select(string.Format("回购交割日>'{0}' and 交易方向='融资'", GlobalParameters.Date)).Length == 0 ? 0 : Convert.ToDecimal(dt.Compute("sum(金额)", string.Format("回购交割日>'{0}' and 交易方向='融资'", GlobalParameters.Date)).ToString());
            YHJrepo_forzbss = dt.Select(string.Format("回购交割日>'{0}' and 市场='银行间'", GlobalParameters.Date)).Length == 0 ? 0 : Convert.ToDecimal(dt.Compute("sum(金额)", string.Format("回购交割日>'{0}' and 市场='银行间'", GlobalParameters.Date)).ToString());

            DataColumn column_tmp_yield = new DataColumn("tmp_yield", typeof(decimal));
            dt.Columns.Add(column_tmp_yield);
            column_tmp_yield.Expression = "金额*利率";

            DataColumn column_tmp_term = new DataColumn("tmp_term", typeof(decimal));
            dt.Columns.Add(column_tmp_term);
            column_tmp_term.Expression = "金额*剩余天期";
            //当日资产收益统计
            assetamount_rz = dt.Select("交易方向 = '融资'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Compute("sum(金额)", "交易方向='融资'").ToString()), 0);
            assetyield_rz = dt.Select("交易方向 = '融资'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Compute("sum(tmp_yield)", "交易方向='融资'").ToString()) / assetamount_rz, 4);
            weightterm_rz = dt.Select("交易方向 = '融资'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Compute("sum(tmp_term)", "交易方向='融资'").ToString()) / assetamount_rz, 2);

            assetamount_rq = dt.Select("交易方向 = '融券'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Compute("sum(金额)", "交易方向='融券'").ToString()), 0);
            assetyield_rq = dt.Select("交易方向 = '融券'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Compute("sum(tmp_yield)", "交易方向='融券'").ToString()) / assetamount_rq, 4);
            weightterm_rq = dt.Select("交易方向 = '融券'").Length == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Compute("sum(tmp_term)", "交易方向='融券'").ToString()) / assetamount_rq, 2);

            //头寸
            string str_in_out;
            jys_HG_dq_in2 = new string[13];
            jys_HG_rz_in3 = new string[13];
            yhj_HG_dq_in4 = new string[13];
            yhj_HG_rz_in5 = new string[13];

            jys_HG_rq_out0 = new string[13];
            jys_HG_dq_out1 = new string[13];
            yhj_HG_rq_out2 = new string[13];
            yhj_HG_dq_out3 = new string[13];

            int i;
            foreach(DataRow dr in dt.Rows)
            {
                if (Convert.ToDateTime(dr["结算日"].ToString()) >= Convert.ToDateTime(GlobalParameters.Date))
                {
                    for (i = 0; i < 13; i++)
                    {
                        str_in_out = string.Format("结算日='{0}' and 市场='{1}' and 交易方向='{2}'"
                            , GlobalParameters.TradingDateSeries[i],"交易所","融券");
                        jys_HG_rq_out0[i] = dt.Select(str_in_out).Length==0?null:decimal.Round(Convert.ToDecimal(dt.Compute("sum(金额)", str_in_out).ToString()), 1).ToString();

                        str_in_out = string.Format("结算日='{0}' and 市场='{1}' and 交易方向='{2}'"
                            , GlobalParameters.TradingDateSeries[i], "交易所", "融资");
                        jys_HG_rz_in3[i] = dt.Select(str_in_out).Length == 0 ? null : decimal.Round(Convert.ToDecimal(dt.Compute("sum(金额)", str_in_out).ToString()), 1).ToString();

                        str_in_out = string.Format("结算日='{0}' and 市场='{1}' and 交易方向='{2}'"
                            , GlobalParameters.TradingDateSeries[i], "银行间", "融券");
                        yhj_HG_rq_out2[i] = dt.Select(str_in_out).Length == 0 ? null : decimal.Round(Convert.ToDecimal(dt.Compute("sum(金额)", str_in_out).ToString()), 1).ToString();

                        str_in_out = string.Format("结算日='{0}' and 市场='{1}' and 交易方向='{2}'"
                            , GlobalParameters.TradingDateSeries[i], "银行间", "融资");
                        yhj_HG_rz_in5[i] = dt.Select(str_in_out).Length == 0 ? null : decimal.Round(Convert.ToDecimal(dt.Compute("sum(金额)", str_in_out).ToString()), 1).ToString();
                    }
                }
                if (Convert.ToDateTime(dr["回购交割日"].ToString()) <= Convert.ToDateTime(GlobalParameters.TradingDateSeries[12]))
                {
                    for (i = 0; i < 13; i++)
                    {
                        str_in_out = string.Format("回购交割日='{0}' and 市场='{1}' and 交易方向='{2}'"
                            , GlobalParameters.TradingDateSeries[i], "交易所", "融券");
                        jys_HG_dq_in2[i] = dt.Select(str_in_out).Length == 0 ? null : decimal.Round(Convert.ToDecimal(dt.Compute("sum(金额)", str_in_out).ToString()), 1).ToString();

                        str_in_out = string.Format("回购交割日='{0}' and 市场='{1}' and 交易方向='{2}'"
                            , GlobalParameters.TradingDateSeries[i], "交易所", "融资");
                        jys_HG_dq_out1[i] = dt.Select(str_in_out).Length == 0 ? null : decimal.Round(Convert.ToDecimal(dt.Compute("sum(金额)", str_in_out).ToString()), 1).ToString();

                        str_in_out = string.Format("回购交割日='{0}' and 市场='{1}' and 交易方向='{2}'"
                            , GlobalParameters.TradingDateSeries[i], "银行间", "融券");
                        yhj_HG_dq_in4[i] = dt.Select(str_in_out).Length == 0 ? null : decimal.Round(Convert.ToDecimal(dt.Compute("sum(金额)", str_in_out).ToString()), 1).ToString();

                        str_in_out = string.Format("回购交割日='{0}' and 市场='{1}' and 交易方向='{2}'"
                            , GlobalParameters.TradingDateSeries[i], "银行间", "融资");
                        yhj_HG_dq_out3[i] = dt.Select(str_in_out).Length == 0 ? null : decimal.Round(Convert.ToDecimal(dt.Compute("sum(金额)", str_in_out).ToString()), 1).ToString();
                    }
                }
            }

            return dt;
        }
    }
}