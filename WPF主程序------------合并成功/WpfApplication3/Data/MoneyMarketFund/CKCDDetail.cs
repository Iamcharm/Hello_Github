using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace WpfApplication3.Data.MoneyMarketFund
{
    public static class CKCDDetail
    {
        public static string fundcode = GlobalParameters.fundCode;
        public static string date = GlobalParameters.lastTradingDate;

        public static decimal zbjs_cdfy;//存单总浮盈 keyindex（22，1）
        public static decimal zbjs_cdfk;//存单总浮亏 keyindex（23，1）
        public static decimal zbjs_cdratio;//存单比例 keyindex（4，1）

        public static decimal cdamount;//存单市值

        public static DataRow yieldStats_ck;

        public static string[] ckdq;
        public static string[] cddq;
        public static DataTable getCDDetail()
        {
            SqlHelper sqh = new SqlHelper("con_BondFund");
            //sqh.getConnectString("con_BondFund");
            string sql_str = string.Format(@"
select t.gzdate as 日期,
t.finance_name as 存单名称,
t.yhzh as 银行总行,
t.zqamount as 面额,
t.zqmktvalue as 市值,
t.asset_ratio as 市值比净值,
t.ghdate as 到期日,
t.expr1 as 票面利息,
t.jxts as 计息天数,
t.price as 收盘价,
t.unityk as 单位盈亏,
t.totalyk as 总体盈亏,
datediff(day,getdate(),t.ghdate) as 剩余天期
from bondfund..fundcdinfo_vw t
where t.fundcode='{0}' and t.gzdate='{1}' order by t.ghdate
;
--头寸
select t.到期日,sum(t.面额*(1+t.票面利息*t.计息天数/365)) as 存单到期
from(
select gzdate as 日期,
finance_name as 存单名称,
zqamount as 面额,
ghdate as 到期日,
expr1  as 票面利息,
jxts as 计息天数,
ghdate as 剩余天期
from bondfund..fundcdinfo_vw
where fundcode='{0}' and gzdate='{1}'
) t
group by t.到期日

            ", GlobalParameters.fundCode, GlobalParameters.lastTradingDate);
            // 存单明细表（show）
            DataTable dt = sqh.ExecuteDataSetText(sql_str, null).Tables[0];

            //存单比例
            zbjs_cdratio = dt.Rows.Count==0?0:Convert.ToDecimal(dt.Compute("sum(市值比净值)", "true").ToString());
            //存单市值
            cdamount = dt.Rows.Count == 0 ? 0 : decimal.Round(Convert.ToDecimal(dt.Compute("sum(市值)", "true").ToString()), 2);

            //存单总浮盈 keyindex（22，1）
            zbjs_cdfy = dt.Select("总体盈亏>0").Length == 0 ? 0 : Convert.ToDecimal(dt.Compute("sum(总体盈亏)", "总体盈亏>0").ToString());
            //存单总浮亏 keyindex（23，1）
            zbjs_cdfk = dt.Select("总体盈亏<0").Length == 0 ? 0 : Convert.ToDecimal(dt.Compute("sum(总体盈亏)", "总体盈亏<0").ToString());

            //头寸：存单到期
            cddq = new string[13];
            DataTable cddq_dt = sqh.ExecuteDataSetText(sql_str, null).Tables[1];
            int i;
            for (i = 0; i <= 12; i++)
            {
                string ckdq_sqlStr = string.Format(@"到期日='{0}'", GlobalParameters.TradingDateSeries[i].ToString());
                cddq[i] = cddq_dt.Select(ckdq_sqlStr).Length == 0 ? null : decimal.Round(Convert.ToDecimal(cddq_dt.Select(ckdq_sqlStr)[0][1].ToString()),1).ToString();
            }


            return dt;
        }
        /// <summary>
        /// 算当日资产收益统计 和 头寸 用的
        /// </summary>
        /// <returns></returns>
        public static void getCKCDStats()
        {
            SqlHelper sqh = new SqlHelper("con_BondFund");
            //sqh.getConnectString("con_BondFund");
            string sql_str = string.Format(@"
select 
Convert(decimal(18,0),sum(t.amount)),
cast(Convert(decimal(18,2),sum(t.amount*t.yield)/sum(t.amount)*100) as varchar)+'%',
Convert(decimal(18,1),sum(t.amount*t.term)/sum(t.amount))
from (
select ckJe/10000 as amount, jxlv/100 as yield,
--pzdate,zqdate,datediff(day,pzdate,zqdate) as ckqx,
datediff(day,getdate(),zqdate) as term
from Infocenter..FundJSZyhjckywData
where fundcode='{0}' and zqdate>'{1}' and ywlb='定期存款' --order by zqdate
union all --避免去重
select zqmktvalue as amount,expr1 as yield,datediff(day,getdate(),ghdate) as term  
from bondfund..fundcdinfo_vw
where fundcode='{0}' and gzdate='{1}'-- order by ghdate
) t
where t.term>0
;
select t.到期日,sum(t.金额*(1+t.利率*t.期限/360)) as 存款到期
from(
select ckJe/10000 as 金额, jxlv/100 as 利率,pzdate as 存出日,zqdate as 到期日,
datediff(day,pzdate,zqdate) as 期限,
datediff(day,getdate(),zqdate) as 剩余天期
from Infocenter..FundJSZyhjckywData
where fundcode='{0}' and zqdate>'{1}' 
and ywlb='定期存款'
) t
group by t.到期日

            ", GlobalParameters.fundCode, GlobalParameters.lastTradingDate);
            //当日资产收益统计（3，1），（3，2），（3，3）：定期存款：定存 + 存单
            yieldStats_ck = sqh.ExecuteDataSetText(sql_str, null).Tables[0].Rows[0];

            //头寸：存款到期
            ckdq = new string[13];
            DataTable ckdq_dt = sqh.ExecuteDataSetText(sql_str, null).Tables[1];
            int i ;
            for (i = 0; i <= 12; i++)
            {
                string ckdq_sqlStr = string.Format(@"到期日='{0}'", GlobalParameters.TradingDateSeries[i].ToString());
                ckdq[i] = ckdq_dt.Select(ckdq_sqlStr).Length == 0 ? null : decimal.Round(Convert.ToDecimal(ckdq_dt.Select(ckdq_sqlStr)[0][1].ToString()), 1).ToString();
            }
        }
        /// <summary>
        /// 按到期日
        /// </summary>
        /// <returns></returns>
        public static DataSet getCDCKbyDate()
        {
            SqlHelper sqh = new SqlHelper("con_BondFund");
            string sql_str = string.Format(@"
--存款按到期日
select t.zqdate as 到期日,
convert(decimal(18,0),sum(zqamount)) as 金额,
sum(interest)/sum(zqamount)/100 as 利率
from (
select zqdate,ckje/10000 as zqamount,jxlv/100 as jxlv,ckje*jxlv/100 as interest from infocenter..FundJSZyhjckywData
where fundcode='{0}' and zqdate>'{1}' and ywlb='定期存款'
) t
group by t.zqdate order by t.zqdate
;
--存单按到期日
select t.ghdate as 到期日,
convert(decimal(18,0),sum(zqamount)) as 金额,
sum(interest)/sum(zqamount)*100 as 利率
from (
select ghdate,zqamount,expr1,zqamount*expr1 as interest from Bondfund..FundCdInfo_vw
where fundcode='{0}' and gzdate='{1}' and ghdate>'{1}'
) t
group by t.ghdate order by t.ghdate
;
--存款存单按到期日
select t.zqdate as 到期日,convert(decimal(18,0),sum(zqamount)) as 金额,
sum(interest)/sum(zqamount)/100 as 利率
from (
select zqdate,ckje/10000 as zqamount,jxlv/100 as jxlv,ckje*jxlv/100 as interest from infocenter..FundJSZyhjckywData
where fundcode='{0}' and zqdate>'{1}' and ywlb='定期存款'
union all
select ghdate,zqamount,expr1,zqamount*expr1*10000 as interest from Bondfund..FundCdInfo_vw
where fundcode='{0}' and gzdate='{1}' and ghdate>'{1}'
) t
group by t.zqdate order by t.zqdate

            ", GlobalParameters.fundCode, GlobalParameters.lastTradingDate, ValuationTable.zbjs_NAV);
            DataSet ds = sqh.ExecuteDataSetText(sql_str, null);
            return ds;
        }
        /// <summary>
        /// 按银行
        /// </summary>
        /// <returns></returns>
        public static DataSet getCDCKbyBank()
        {
            SqlHelper sqh = new SqlHelper("con_BondFund");
            string sql_str = string.Format(@"
--存款按银行
Select yhzh as 银行,convert(decimal(18,0),sum(ckje/10000)) as 金额,
sum(ckje/100)/{2} as 占净值比 
from infocenter..FundJSZyhjckywData
where fundcode='{0}' and zqdate>'{1}' and ywlb='定期存款' 
group by yhzh
;
--存单按银行
Select yhzh as 银行,convert(decimal(18,0),sum(ckje/10000)) as 金额,
sum(ckje/100)/{2} as 占净值比 
from infocenter..FundJSZyhjckywData
where fundcode='{0}' and zqdate>'{1}' and ywlb='同业存单' 
group by yhzh
;
--存款存单按银行
Select yhzh as 银行,convert(decimal(18,0),sum(ckje/10000)) as 金额,
sum(ckje/100)/{2} as 占净值比 
from infocenter..FundJSZyhjckywData
where fundcode='{0}' and zqdate>'{1}'
group by yhzh
            ", GlobalParameters.fundCode, GlobalParameters.lastTradingDate, ValuationTable.zbjs_NAV);
            DataSet ds = sqh.ExecuteDataSetText(sql_str, null);
            return ds;
        }
    }
}