using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace WpfApplication3.Data.InvestmentAnalysisTools
{
    public static class AnalysisData
    {
        //收益率
        public static DataSet invAnaToolsDataSet_BondYieldTrend_yieldGrid;
        public static DataSet invAnaToolsDataSet_BondYieldTrend_yieldChart;

        //利差
        public static DataSet invAnaToolsDataSet_BondSpread_Grid;
        public static DataSet invAnaToolsDataSet_BondSpread_Chart;

        public static DataSet getBondYieldTrend_yieldGridData(string date)
        {
            SqlHelper sqh = new SqlHelper("con_AA");
            string sql_str = string.Format(@"
--涨跌幅统计
select t1.name1,
--Convert(decimal(18,1),t1.pc05*10000) as pc05,  
--cast(Convert(decimal(18,1),t1.pccd05*100) as varchar )+'%' as pccd05,  
--Convert(decimal(18,1),t1.pc10*10000) as pc10,
--cast(Convert(decimal(18,1),t1.pccd10*100) as varchar )+'%' as pccd10,
--Convert(decimal(18,1),t1.pc20*10000) as pc20,
--cast(Convert(decimal(18,1),t1.pccd20*100) as varchar )+'%' as pccd20,  
--Convert(decimal(18,1),t1.pc60*10000) as pc60,
--cast(Convert(decimal(18,1),t1.pccd60*100) as varchar )+'%' as pccd60,
--cast(Convert(decimal(18,1),t2.YTM*100) as varchar )+'%' as YTM,
--cast(Convert(decimal(18,1),t2.YTM_CD*100) as varchar )+'%' as YTM_CD
t1.pc05*10000 as pc05, t1.pccd05, t1.pc10*10000 as pc10, t1.pccd10,
t1.pc20*10000 as pc20, t1.pccd20, t1.pc60*10000 as pc60, t1.pccd60, 
t2.YTM,t2.YTM_CD
from 
(SELECT 
a.assetId, a.name1, convert(varchar,aa.pricingdate,23) as pricingdate, ar.parentlevel,
aa.pc05, aa.pccd05, aa.pc10, aa.pccd10,
aa.pc20, aa.pccd20, aa.pc60, aa.pccd60, 
aa.pc120, aa.pccd120, aa.pc250, aa.pccd250
FROM AA.dbo.assetsector ar, AA.dbo.percentchange aa, AA.dbo.asset a
WHERE  aa.pricingdate='{0}'
AND ar.modelid = 0 
AND ar.classid = 1 
AND (ar.parentlevel = 8 OR ar.parentlevel = 9)
AND aa.assetid = ar.assetid 
AND aa.assetid = a.assetid 
AND aa.assetid not in (256,257,258,259,260)
--ORDER BY ar.parentlevel, aa.assetid
)t1
left join
(SELECT a.assetId, a.name1, convert(varchar,aa.pricingdate,23) as pricingdate, 
aa.measurevalue as YTM,aa_cd.measurevalue as YTM_CD
FROM AA.dbo.assetsector ar, AA.dbo.asset a , AA.dbo.AssetAnalytic aa, AA.dbo.AssetAnalytic aa_cd
WHERE aa.pricingdate='{0}'
AND (ar.parentlevel = 8 OR ar.parentlevel = 9)
AND aa.assetid = ar.assetid 
AND aa.assetid = a.assetid 
AND aa.assetid = aa_cd.assetid 
AND ar.modelid = 0 
AND ar.classid = 1 
AND aa.measureId = 2 
AND aa_cd.measureId = 35
AND aa.pricingdate = aa_cd.pricingdate 
--ORDER BY ar.parentlevel, aa.assetid
)t2
on t1.assetId=t2.assetId
ORDER BY t1.parentlevel, t1.assetid;
            ",  date);
            invAnaToolsDataSet_BondYieldTrend_yieldGrid = sqh.ExecuteDataSetText(sql_str, null);
            return invAnaToolsDataSet_BondYieldTrend_yieldGrid;
        }

        public static DataSet getBondSpread_GridData(string date)
        {
            SqlHelper sqh = new SqlHelper("con_AA");
            string sql_str = string.Format(@"
SELECT t1.name,
t1.[3Y1Y]*10000 as '3Y1Y',
t2.[3Y1Y_cd],
t1.[5Y3Y]*10000 as '5Y3Y',
t2.[5Y3Y_cd],
t1.[7Y5Y]*10000 as '7Y5Y',
t2.[7Y5Y_cd],
t1.[10Y7Y]*10000 as '10Y7Y',
t2.[10Y7Y_cd],
t1.[10Y1Y]*10000 as '10Y1Y',
t2.[10Y1Y_cd]
FROM
(
    SELECT cd1.name,cd1.CurveAnalyticId,ca1.PricingDate, ca1.value AS '3Y1Y', ca2.value AS '5Y3Y', ca3.value AS '7Y5Y', ca4.value AS '10Y7Y', ca5.value AS '10Y1Y'
    FROM AA.dbo.curveanalyticdefinition cd1, AA.dbo.curveanalyticdefinition cd2, AA.dbo.curveanalyticdefinition cd3,
    AA.dbo.curveanalyticdefinition cd4, AA.dbo.curveanalyticdefinition cd5,
    AA.dbo.curveanalytic ca1, AA.dbo.curveanalytic ca2, AA.dbo.curveanalytic ca3,
    AA.dbo.curveanalytic ca4, AA.dbo.curveanalytic ca5
    WHERE cd1.analytictypecode = 1 AND cd1.analytictypecode = cd2.analytictypecode AND cd2.analytictypecode = cd3.analytictypecode
    AND cd3.analytictypecode = cd4.analytictypecode AND cd4.analytictypecode = cd5.analytictypecode
    AND cd1.source = 'JY' AND cd1.source = cd2.source AND cd2.source = cd3.source AND cd3.source = cd4.source AND cd4.source = cd5.source
    AND cd1.name = cd2.name AND cd2.name = cd3.name AND cd3.name = cd4.name AND cd4.name = cd5.name
    AND cd1.term = '3Y1Y' AND cd2.term = '5Y3Y' AND cd3.term = '7Y5Y' AND cd4.term = '10Y7Y' AND cd5.term = '10Y1Y'
    AND cd1.curveanalyticid = ca1.curveanalyticid AND cd2.curveanalyticid = ca2.curveanalyticid AND cd3.curveanalyticid = ca3.curveanalyticid
    AND cd4.curveanalyticid = ca4.curveanalyticid AND cd5.curveanalyticid = ca5.curveanalyticid
    AND ca1.pricingdate = ca2.pricingdate AND ca2.pricingdate = ca3.pricingdate
    AND ca3.pricingdate = ca4.pricingdate AND ca4.pricingdate = ca5.pricingdate
    AND cd1.name in ('AAA企业债','AA+城投债','AA+企业债','AA城投债','AA企业债','国开债','国债','口行农发债','铁道债')
    UNION
    SELECT cd1.name, cd1.CurveAnalyticId, ca1.PricingDate, ca1.value AS '3Y1Y', ca2.value AS '5Y3Y', NULL, NULL, NULL
    FROM AA.dbo.curveanalyticdefinition cd1, AA.dbo.curveanalyticdefinition cd2, AA.dbo.curveanalytic ca1, AA.dbo.curveanalytic ca2
    WHERE cd1.analytictypecode = 1 AND cd1.analytictypecode = cd2.analytictypecode AND cd1.source = 'JY' AND cd1.source = cd2.source
    AND cd1.name = cd2.name AND cd1.term = '3Y1Y' AND cd2.term = '5Y3Y'
    AND cd1.curveanalyticid = ca1.curveanalyticid AND cd2.curveanalyticid = ca2.curveanalyticid
    AND ca1.pricingdate = ca2.pricingdate AND cd1.name in ('AAA中票短融','AA+中票短融','AA中票短融')
) AS t1
LEFT JOIN
(
    SELECT cd1.name,cd1.CurveAnalyticId, ca1.PricingDate ,ca1.value AS '3Y1Y_cd', ca2.value AS '5Y3Y_cd', ca3.value AS '7Y5Y_cd', ca4.value AS '10Y7Y_cd', ca5.value AS '10Y1Y_cd'
    FROM AA.dbo.curveanalyticdefinition cd1, AA.dbo.curveanalyticdefinition cd2, AA.dbo.curveanalyticdefinition cd3,
    AA.dbo.curveanalyticdefinition cd4, AA.dbo.curveanalyticdefinition cd5,
    AA.dbo.curveanalytic ca1, AA.dbo.curveanalytic ca2, AA.dbo.curveanalytic ca3,
    AA.dbo.curveanalytic ca4, AA.dbo.curveanalytic ca5
    WHERE cd1.analytictypecode = 3 AND cd1.analytictypecode = cd2.analytictypecode AND cd2.analytictypecode = cd3.analytictypecode
    AND cd3.analytictypecode = cd4.analytictypecode AND cd4.analytictypecode = cd5.analytictypecode
    AND cd1.source = 'JY' AND cd1.source = cd2.source AND cd2.source = cd3.source AND cd3.source = cd4.source AND cd4.source = cd5.source
    AND cd1.name = cd2.name AND cd2.name = cd3.name AND cd3.name = cd4.name AND cd4.name = cd5.name
    AND cd1.term = '3Y1Y' AND cd2.term = '5Y3Y' AND cd3.term = '7Y5Y' AND cd4.term = '10Y7Y' AND cd5.term = '10Y1Y'
    AND cd1.curveanalyticid = ca1.curveanalyticid AND cd2.curveanalyticid = ca2.curveanalyticid AND cd3.curveanalyticid = ca3.curveanalyticid
    AND cd4.curveanalyticid = ca4.curveanalyticid AND cd5.curveanalyticid = ca5.curveanalyticid
    AND ca1.pricingdate = ca2.pricingdate AND ca2.pricingdate = ca3.pricingdate
    AND ca3.pricingdate = ca4.pricingdate AND ca4.pricingdate = ca5.pricingdate
    AND cd1.name in ('AAA企业债_利差分位数','AA+城投债_利差分位数','AA+企业债_利差分位数','AA城投债_利差分位数','AA企业债_利差分位数','国开债_利差分位数','国债_利差分位数','口行农发债_利差分位数','铁道债_利差分位数')
    UNION
    SELECT cd1.name, cd1.CurveAnalyticId, ca1.PricingDate, ca1.value AS '3Y1Y_cd', ca2.value AS '5Y3Y_cd', NULL, NULL, NULL
    FROM AA.dbo.curveanalyticdefinition cd1, AA.dbo.curveanalyticdefinition cd2, AA.dbo.curveanalytic ca1, AA.dbo.curveanalytic ca2
    WHERE cd1.analytictypecode = 3 AND cd1.analytictypecode = cd2.analytictypecode
    AND cd1.source = 'JY' AND cd1.source = cd2.source AND cd1.name = cd2.name AND cd1.term = '3Y1Y' AND cd2.term = '5Y3Y'
    AND cd1.curveanalyticid = ca1.curveanalyticid AND cd2.curveanalyticid = ca2.curveanalyticid AND ca1.pricingdate = ca2.pricingdate
    AND cd1.name in ('AAA中票短融_利差分位数','AA+中票短融_利差分位数','AA中票短融_利差分位数')
) AS t2 ON t1.PricingDate = t2.PricingDate AND t1.CurveAnalyticId = t2.CurveAnalyticId - 99
where t1.pricingdate = '{0}';


--债券利差:品种利差数值、分位数
SELECT t1.name,
t1.[1Y] * 10000 as '1Y',
t2.[1Y_cd],        
t1.[3Y] * 10000 as '3Y',
t2.[3Y_cd],        
t1.[5Y] * 10000 as '5Y',
t2.[5Y_cd],        
t1.[7Y] * 10000 as '7Y',
t2.[7Y_cd],        
t1.[10Y]* 10000 as '10Y',
t2.[10Y_cd]
FROM
(
    SELECT cd1.name,cd1.CurveAnalyticId, ca1.PricingDate, ca1.value AS '1Y', ca2.value AS '3Y', ca3.value AS '5Y', ca4.value AS '7Y', ca5.value AS '10Y'
    FROM AA.dbo.curveanalyticdefinition cd1, AA.dbo.curveanalyticdefinition cd2, AA.dbo.curveanalyticdefinition cd3,
    AA.dbo.curveanalyticdefinition cd4, AA.dbo.curveanalyticdefinition cd5,
    AA.dbo.curveanalytic ca1, AA.dbo.curveanalytic ca2, AA.dbo.curveanalytic ca3,
    AA.dbo.curveanalytic ca4, AA.dbo.curveanalytic ca5
    WHERE cd1.analytictypecode = 2 AND cd1.analytictypecode = cd2.analytictypecode AND cd2.analytictypecode = cd3.analytictypecode AND cd3.analytictypecode = cd4.analytictypecode
    AND cd4.analytictypecode = cd5.analytictypecode AND cd1.source = 'JY' AND cd1.source = cd2.source AND cd2.source = cd3.source AND cd3.source = cd4.source
    AND cd4.source = cd5.source AND cd1.name = cd2.name AND cd2.name = cd3.name AND cd3.name = cd4.name AND cd4.name = cd5.name
    AND cd1.term = '1Y' AND cd2.term = '3Y' AND cd3.term = '5Y' AND cd4.term = '7Y' AND cd5.term = '10Y'
    AND cd1.curveanalyticid = ca1.curveanalyticid AND cd2.curveanalyticid = ca2.curveanalyticid AND cd3.curveanalyticid = ca3.curveanalyticid
    AND cd4.curveanalyticid = ca4.curveanalyticid AND cd5.curveanalyticid = ca5.curveanalyticid
    AND ca1.pricingdate = ca2.pricingdate AND ca2.pricingdate = ca3.pricingdate AND ca3.pricingdate = ca4.pricingdate AND ca4.pricingdate = ca5.pricingdate
    AND cd1.name in ('国开-国债','AA+短融-国开','AA+企业债-国开','AAA短融-国开','AAA企业债-国开','AA短融-国开','AA企业债-国开','非国开-国开','铁道债-国开')
    UNION
    SELECT cd1.name,cd1.CurveAnalyticId, ca1.PricingDate, ca1.value AS '1Y', ca2.value AS '3Y', ca3.value AS '5Y', ca4.value AS '7Y', NULL
    FROM AA.dbo.curveanalyticdefinition cd1, AA.dbo.curveanalyticdefinition cd2, AA.dbo.curveanalyticdefinition cd3, AA.dbo.curveanalyticdefinition cd4,
    AA.dbo.curveanalytic ca1, AA.dbo.curveanalytic ca2, AA.dbo.curveanalytic ca3, AA.dbo.curveanalytic ca4
    WHERE cd1.analytictypecode = 2 AND cd1.analytictypecode = cd2.analytictypecode AND cd2.analytictypecode = cd3.analytictypecode AND cd3.analytictypecode = cd4.analytictypecode
    AND cd1.source = 'JY' AND cd1.source = cd2.source AND cd2.source = cd3.source AND cd3.source = cd4.source
    AND cd1.name = cd2.name AND cd2.name = cd3.name AND cd3.name = cd4.name AND cd1.term = '1Y' AND cd2.term = '3Y' AND cd3.term = '5Y' AND cd4.term = '7Y'
    AND cd1.curveanalyticid = ca1.curveanalyticid AND cd2.curveanalyticid = ca2.curveanalyticid AND cd3.curveanalyticid = ca3.curveanalyticid
    AND cd4.curveanalyticid = ca4.curveanalyticid AND ca1.pricingdate = ca2.pricingdate AND ca2.pricingdate = ca3.pricingdate AND ca3.pricingdate = ca4.pricingdate
    AND cd1.name in ('AA+城投-国开','AA城投-国开')
    UNION
    SELECT cd2.name, cd2.CurveAnalyticId, ca2.PricingDate, NULL, ca2.value AS '3Y', ca3.value AS '5Y', NULL, NULL
    FROM AA.dbo.curveanalyticdefinition cd2, AA.dbo.curveanalyticdefinition cd3, AA.dbo.curveanalytic ca2, AA.dbo.curveanalytic ca3
    WHERE cd2.analytictypecode = 2 AND cd2.analytictypecode = cd3.analytictypecode
    AND cd2.source = 'JY' AND cd2.source = cd3.source AND cd2.name = cd3.name AND cd2.term = '3Y' AND cd3.term = '5Y'
    AND cd2.curveanalyticid = ca2.curveanalyticid AND cd3.curveanalyticid = ca3.curveanalyticid AND ca2.pricingdate = ca3.pricingdate
    AND cd2.name in ('AA+中票-国开','AAA中票-国开','AA中票-国开')
) AS t1
LEFT JOIN
(
    SELECT cd1.name,cd1.CurveAnalyticId, ca1.PricingDate, ca1.value AS '1Y_cd', ca2.value AS '3Y_cd', ca3.value AS '5Y_cd', ca4.value AS '7Y_cd', ca5.value AS '10Y_cd'
    FROM AA.dbo.curveanalyticdefinition cd1, AA.dbo.curveanalyticdefinition cd2, AA.dbo.curveanalyticdefinition cd3,
    AA.dbo.curveanalyticdefinition cd4, AA.dbo.curveanalyticdefinition cd5,
    AA.dbo.curveanalytic ca1, AA.dbo.curveanalytic ca2, AA.dbo.curveanalytic ca3,
    AA.dbo.curveanalytic ca4, AA.dbo.curveanalytic ca5
    WHERE cd1.analytictypecode = 4 AND cd1.analytictypecode = cd2.analytictypecode AND cd2.analytictypecode = cd3.analytictypecode AND cd3.analytictypecode = cd4.analytictypecode
    AND cd4.analytictypecode = cd5.analytictypecode AND cd1.source = 'JY' AND cd1.source = cd2.source AND cd2.source = cd3.source AND cd3.source = cd4.source
    AND cd4.source = cd5.source AND cd1.name = cd2.name AND cd2.name = cd3.name AND cd3.name = cd4.name AND cd4.name = cd5.name
    AND cd1.term = '1Y' AND cd2.term = '3Y' AND cd3.term = '5Y' AND cd4.term = '7Y' AND cd5.term = '10Y'
    AND cd1.curveanalyticid = ca1.curveanalyticid AND cd2.curveanalyticid = ca2.curveanalyticid AND cd3.curveanalyticid = ca3.curveanalyticid
    AND cd4.curveanalyticid = ca4.curveanalyticid AND cd5.curveanalyticid = ca5.curveanalyticid
    AND ca1.pricingdate = ca2.pricingdate AND ca2.pricingdate = ca3.pricingdate AND ca3.pricingdate = ca4.pricingdate AND ca4.pricingdate = ca5.pricingdate
    AND cd1.name in ('国开-国债_利差分位数','AA+短融-国开_利差分位数','AA+企业债-国开_利差分位数','AAA短融-国开_利差分位数','AAA企业债-国开_利差分位数','AA短融-国开_利差分位数','AA企业债-国开_利差分位数','非国开-国开_利差分位数','铁道债-国开_利差分位数')
    UNION
    SELECT cd1.name,cd1.CurveAnalyticId, ca1.PricingDate, ca1.value AS '1Y_cd', ca2.value AS '3Y_cd', ca3.value AS '5Y_cd', ca4.value AS '7Y_cd', NULL
    FROM AA.dbo.curveanalyticdefinition cd1, AA.dbo.curveanalyticdefinition cd2, AA.dbo.curveanalyticdefinition cd3, AA.dbo.curveanalyticdefinition cd4,
    AA.dbo.curveanalytic ca1, AA.dbo.curveanalytic ca2, AA.dbo.curveanalytic ca3, AA.dbo.curveanalytic ca4
    WHERE cd1.analytictypecode = 4 AND cd1.analytictypecode = cd2.analytictypecode AND cd2.analytictypecode = cd3.analytictypecode AND cd3.analytictypecode = cd4.analytictypecode
    AND cd1.source = 'JY' AND cd1.source = cd2.source AND cd2.source = cd3.source AND cd3.source = cd4.source
    AND cd1.name = cd2.name AND cd2.name = cd3.name AND cd3.name = cd4.name AND cd1.term = '1Y' AND cd2.term = '3Y' AND cd3.term = '5Y' AND cd4.term = '7Y'
    AND cd1.curveanalyticid = ca1.curveanalyticid AND cd2.curveanalyticid = ca2.curveanalyticid AND cd3.curveanalyticid = ca3.curveanalyticid AND cd4.curveanalyticid = ca4.curveanalyticid
    AND ca1.pricingdate = ca2.pricingdate AND ca2.pricingdate = ca3.pricingdate AND ca3.pricingdate = ca4.pricingdate
    AND cd1.name in ('AA+城投-国开_利差分位数','AA城投-国开_利差分位数')
    UNION
    SELECT cd2.name, cd2.CurveAnalyticId, ca2.PricingDate, NULL, ca2.value AS '3Y_cd', ca3.value AS '5Y_cd', NULL, NULL
    FROM AA.dbo.curveanalyticdefinition cd2, AA.dbo.curveanalyticdefinition cd3,
    AA.dbo.curveanalytic ca2, AA.dbo.curveanalytic ca3
    WHERE cd2.analytictypecode = 4 AND cd2.analytictypecode = cd3.analytictypecode
    AND cd2.source = 'JY' AND cd2.source = cd3.source AND cd2.name = cd3.name AND cd2.term = '3Y' AND cd3.term = '5Y'
    AND cd2.curveanalyticid = ca2.curveanalyticid AND cd3.curveanalyticid = ca3.curveanalyticid
    AND ca2.pricingdate = ca3.pricingdate AND cd2.name in ('AA+中票-国开_利差分位数','AAA中票-国开_利差分位数','AA中票-国开_利差分位数')
) AS t2 ON t1.PricingDate = t2.PricingDate AND t1.CurveAnalyticId = t2.CurveAnalyticId - 99
where t1.pricingdate = '{0}'
            ", date);
            invAnaToolsDataSet_BondSpread_Grid = sqh.ExecuteDataSetText(sql_str, null);
            return invAnaToolsDataSet_BondSpread_Grid;
        }

        public static List<string> getBondYieldTrend_yieldChartData(string startDate,string endDate,string assetid)
        {
            SqlHelper sqh = new SqlHelper("con_AA");
            string sql_str = string.Format(@"
--收益率走势
SELECT  aa.pricingdate, aa.measurevalue as YTM
FROM AA.dbo.assetsector ar, AA.dbo.asset a , AA.dbo.AssetAnalytic aa
WHERE aa.pricingdate between '{0}' and '{1}'
AND aa.AssetId='{2}' 
AND (ar.parentlevel = 8 OR ar.parentlevel = 9)
AND aa.assetid = ar.assetid 
AND aa.assetid = a.assetid 
AND ar.modelid = 0 
AND ar.classid = 1 
AND aa.measureId = 2 
ORDER BY ar.parentlevel, aa.assetid
            ", startDate, endDate, assetid);
            invAnaToolsDataSet_BondYieldTrend_yieldChart = sqh.ExecuteDataSetText(sql_str, null);
            DataTable dt = invAnaToolsDataSet_BondYieldTrend_yieldChart.Tables[0];
            //计算最大值最小值
            List<string> xTime = new List<string>();
            List<decimal> yValue = new List<decimal>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    xTime.Add(dt.Rows[i][0].ToString().Substring(2));
                    yValue.Add(Convert.ToDecimal(dt.Rows[i][1]));
                }
            }

            string count = xTime.Count.ToString();
            string max = xTime.Count == 0 ? "" : string.Format("{0:F8}", yValue.Max());
            string min = xTime.Count == 0 ? "" : string.Format("{0:F8}", yValue.Min());
            string avg = xTime.Count == 0 ? "" : string.Format("{0:F8}", yValue.Average());
            string last = xTime.Count == 0 ? "" : string.Format("{0:F8}", yValue.Last());

            List<string> stat_yieldCurve = new List<string>();
            stat_yieldCurve.Add(count);
            stat_yieldCurve.Add(max);
            stat_yieldCurve.Add(min);
            stat_yieldCurve.Add(avg);
            stat_yieldCurve.Add(last);

            return stat_yieldCurve;
        }

        public static List<string> getBondSpread_ChartData(int analytictypecode ,string startDate, string endDate, string assetName, string term)
        {
            SqlHelper sqh = new SqlHelper("con_AA");
            string sql_str = string.Format(@"
--债券利差：（sql相同）
select ca.pricingdate, ca.value*10000 as value 
from AA.dbo.curveanalyticdefinition cd, AA.dbo.curveanalytic ca
where cd.analytictypecode = {4} and cd.source = 'JY' 
and cd.name = '{2}'
and cd.term = '{3}' and cd.curveanalyticid = ca.curveanalyticid 
and ca.pricingdate between '{0}' and '{1}'
and ca.value is not null 
order by ca.pricingdate
            ", startDate, endDate, assetName,term, analytictypecode);
            invAnaToolsDataSet_BondSpread_Chart = sqh.ExecuteDataSetText(sql_str, null);
            DataTable dt = invAnaToolsDataSet_BondSpread_Chart.Tables[0];
            //计算最大值最小值
            List<string> xTime = new List<string>();
            List<decimal> yValue = new List<decimal>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    xTime.Add(dt.Rows[i][0].ToString().Substring(2));
                    yValue.Add(Convert.ToDecimal(dt.Rows[i][1]));
                }
            }

            string count = xTime.Count.ToString();
            string max = xTime.Count == 0 ? "":string.Format("{0:f2}", yValue.Max());
            string min = xTime.Count == 0 ? "" : string.Format("{0:f2}", yValue.Min());
            string avg = xTime.Count == 0 ? "" : string.Format("{0:f2}", yValue.Average());
            string last = xTime.Count == 0 ? "" : string.Format("{0:f2}", yValue.Last());

            List<string> stat_yieldCurve = new List<string>();
            stat_yieldCurve.Add(count);
            stat_yieldCurve.Add(max);
            stat_yieldCurve.Add(min);
            stat_yieldCurve.Add(avg);
            stat_yieldCurve.Add(last);

            return stat_yieldCurve;
        }


    }
}
