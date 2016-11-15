using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace WpfApplication3.Data.PortfolioRiskBudget
{
    public static class RiskAssetDecompose
    {
        public static DataTable portfolioHodings;
        public static DataTable riskBenchmark;

        public static void getRiskAssetDecompose(string fundcode, string date)
        {
            SqlHelper sqh = new SqlHelper("con_AA");//229
            string sql_str = string.Format(@"
select c.name1,
	case when ( a.assetid in (14,18) ) then '股票'  
		 when ( a.assetid in (307) ) then '可转债'
		 when ( a.assetid in (154,157,159,161,163) ) then '国债'
		 when ( a.assetid in (243,244,245,246,247) ) then '金融债'
		 when ( a.assetid in (308,309,310,311) ) then '短融/超短融'
		 when ( a.assetid in (210,211,212,213,214) ) then '铁道债'
		 when ( a.assetid in (215,216,217,218,219) ) then 'AAA中票'
		 when ( a.assetid in (220,221,222,223,224) ) then '非AAA中票'
		 when ( a.assetid in (185,186,187,188,189) ) then 'AAA公司债'
		 when ( a.assetid in (170,171,172,173,174) ) then '非AAA公司债'
		 else '其他'
         end as assettype,
a.*,b.assetname_level2 
from riskplatform.dbo.riskresults as a 
left join riskplatform.dbo.riskasset as b on a.assetid=b.assetid
left join aa.dbo.asset c on a.assetid=c.assetid
where pricingdate = '2016-08-05' 
order by assetcode_level1, assetcode_level2
            ", date);

            SqlHelper sqh1 = new SqlHelper("con_Infocenter");//114
            string sql_str1 = string.Format(@"
--资产
select t.assetid,sum(t.asset_ratio) as asset_ratio,
case when t.assetid in (14,18,307)  then sum(t.asset_ratio)
	 else sum(t.asset_ratio*t.adjduration) end as ratiowithduration
from(
select a.*,b.finance_name,b.secucode,b.ytm as couponrate,b.adjduration 
from bondfund..fundasset_bonddetail( '000406','2016-08-05') as b 
right join bondfund.dbo.fn_Risk_AssetClassification( '000406','2016-08-05') as a 
on a.finance_subject=b.finance_subject 
where a.assetid Is Not Null and ((b.ytm is not null and b.adjduration is not null) or (assetid=14 or assetid=18 or assetid=307))
)t
group by t.assetid
order by t.assetid;
--基金占组合比（和）
select * from infocenter..fundjszgzdata 
where fundcode='Z00156' and gzdate='2016-08-05' and len(finance_subject)=8 
and left(finance_subject,8) in 
	(select finance_subject 
	 from bondfund.. Risk_FinancesubjectType 
	 where  fundcode='Z00156'  and assetlevel1=7 and assetlevel2=14 );
--每只基金占组合比（分别）
select * from infocenter..fundjszgzdata 
where fundcode='Z00156 ' and gzdate='2016-08-05' and len(finance_subject)=14 
and left(finance_subject,8) in 
	(select finance_subject 
	 from bondfund.. Risk_FinancesubjectType 
	 where  fundcode='Z00156'  and assetlevel1=7 and assetlevel2=14 );
--基金内资产占基金比
exec bondfund..sp_risk_openfundassetid  'Z00156','2016-08-05'
--select * from bondfund..fn_Risk_OpenFundAssetId('Z00156','2016-08-05')


            ", fundcode, date);

            riskBenchmark = sqh.ExecuteDataSetText(sql_str, null).Tables[0];//风险评估基准

            DataSet ds = sqh1.ExecuteDataSetText(sql_str1, null);
            portfolioHodings = ds.Tables[0];

            decimal fundWeightSum = ds.Tables[1] == null ? 0 : Convert.ToDecimal(ds.Tables[1].Rows[0]["asset_ratio"].ToString());
            DataTable fundWeights = ds.Tables[2];
            DataTable fundHoldings = ds.Tables[3];

            //资产占比
            DataColumn column_asset_ratio = new DataColumn("tmp_asset_ratio", typeof(decimal));
            //temp列：分资产std\var
            DataColumn column_asset_stdev20_1y = new DataColumn("tmp_asset_stdev20_1y", typeof(decimal));
            DataColumn column_asset_stdev60_1y = new DataColumn("tmp_asset_stdev60_1y", typeof(decimal));
            DataColumn column_asset_var20_1y   = new DataColumn("tmp_asset_var20_1y"  , typeof(decimal));
            DataColumn column_asset_var60_1y   = new DataColumn("tmp_asset_var60_1y"  , typeof(decimal));
            DataColumn column_asset_stdev20_all= new DataColumn("tmp_asset_stdev20_all", typeof(decimal));
            DataColumn column_asset_stdev60_all= new DataColumn("tmp_asset_stdev60_all", typeof(decimal));
            DataColumn column_asset_var20_all  = new DataColumn("tmp_asset_var20_all", typeof(decimal));
            DataColumn column_asset_var60_all  = new DataColumn("tmp_asset_var60_all", typeof(decimal));

            riskBenchmark.Columns.Add(column_asset_ratio);
            riskBenchmark.Columns.Add(column_asset_stdev20_1y);
            riskBenchmark.Columns.Add(column_asset_stdev60_1y);
            riskBenchmark.Columns.Add(column_asset_var20_1y);
            riskBenchmark.Columns.Add(column_asset_var60_1y);
            riskBenchmark.Columns.Add(column_asset_stdev20_all);
            riskBenchmark.Columns.Add(column_asset_stdev60_all);
            riskBenchmark.Columns.Add(column_asset_var20_all);
            riskBenchmark.Columns.Add(column_asset_var60_all);

            foreach (DataRow dr in riskBenchmark.Rows)
            {
                foreach(DataRow dr1 in portfolioHodings.Rows)
                {
                    if (dr1["assetid"].ToString() == dr["assetid"].ToString() && (dr["assettype"].ToString() == "股票" || dr["assettype"].ToString() == "可转债")) 
                    {
                        dr["tmp_asset_ratio"] = dr["tmp_asset_ratio"] == null ? 0 : dr1["asset_ratio"];
                        dr["tmp_asset_stdev20_1y"] = Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["stdev20_1y"].ToString()) / 10000;
                        dr["tmp_asset_stdev60_1y"] = Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["stdev60_1y"].ToString()) / 10000;
                        dr["tmp_asset_var20_1y"  ] = Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["var20_1y"  ].ToString()) / 10000;
                        dr["tmp_asset_var60_1y"  ] = Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["var60_1y"  ].ToString()) / 10000;
                        dr["tmp_asset_stdev20_all"]= Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["stdev20_all"].ToString()) / 10000;
                        dr["tmp_asset_stdev60_all"]= Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["stdev60_all"].ToString()) / 10000;
                        dr["tmp_asset_var20_all"]  = Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["var20_all"].ToString()) / 10000;
                        dr["tmp_asset_var60_all"]  = Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["var60_all"].ToString()) / 10000;
                    }
                    else if (dr1["assetid"].ToString() == dr["assetid"].ToString() && (dr["assettype"].ToString() != "股票" && dr["assettype"].ToString() != "可转债"))
                    {
                        dr["tmp_asset_ratio"] = dr["tmp_asset_ratio"] == null ? 0 : dr1["asset_ratio"];
                        dr["tmp_asset_stdev20_1y"] = Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["stdev20_1y"].ToString()) / 1000000;
                        dr["tmp_asset_stdev60_1y"] = Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["stdev60_1y"].ToString()) / 1000000;
                        dr["tmp_asset_var20_1y"]   = -Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["var20_1y"].ToString()) / 1000000;
                        dr["tmp_asset_var60_1y"]   = -Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["var60_1y"].ToString()) / 1000000;
                        dr["tmp_asset_stdev20_all"]= Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["stdev20_all"].ToString()) / 1000000;
                        dr["tmp_asset_stdev60_all"]= Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["stdev60_all"].ToString()) / 1000000;
                        dr["tmp_asset_var20_all"]  = -Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["var20_all"].ToString()) / 1000000;
                        dr["tmp_asset_var60_all"]  = -Convert.ToDecimal(dr1["ratiowithduration"].ToString()) * Convert.ToDecimal(dr["var60_all"].ToString()) / 1000000;
                    }
                }
            }

            return;
        }
    }
}
