using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace WpfApplication3.Data.BondFund
{
    public static class BondInfo
    {
        public static DataSet bondInfoDataSet_keyIndex;
        public static DataSet bondInfoDataSet_bonddetail;

        public static DataSet getBondInfoKeyIndex(string fundcode,string date)
        {
            SqlHelper sqh = new SqlHelper("con_BondFund");
            string sql_str = string.Format(@"
--0表：债券关键指标？？？
--最新规模（亿元）
select * from (select  '规模（亿元）' as name, round(stock_asset/100000000,3) as value
from infocenter..fundjszgzdata
where fundcode='{0}'
and gzdate='{1}'
and finance_subject = '604资产类合计：'
) as a 
union
--单位净值
select * from (select '单位净值' as name, finance_name as value
from infocenter..fundjszgzdata
where fundcode='{0}'
and gzdate='{1}'
and finance_subject = '902今日单位净值：'
) as a
--成立以来年化收益
union
select * from (select '收益率（%）' as name, case when finance_name ='' then 0 else (cast(finance_name as numeric(38,3))-1)*100 end as value
from infocenter..fundjszgzdata
where fundcode='{0}'
and gzdate='{1}'
and finance_subject like '905累计单位净值：' 
) as a
--组合久期
union
select '组合久期（年）' as name,round(sum(asset_ratio/100*ytm),3) as value 
from bondfund..fundasset_bonddetail( '{0}','{1}') 
group by fundcode,gzdate
--杠杆率
union
select '杠杆率' as name,round(asset_ratio/100,3) as value
from infocenter..fundjszgzdata
where fundcode='{0}'
and gzdate='{1}'
and finance_subject like '604资产类合计：'
;
--1表：大类资产配置(会计科目：来自估值表) finance_subject
select t.gzdate,
case when ( t.finance_subject = '100201') then '活期存款'
     when ( t.finance_subject = '100203') then '定期存款'
	 when ( t.finance_subject = '100360') then '存单'
	 when ( t.finance_subject = '1202') then '回购'
	 when ( t.finance_subject = '1102') then '股票'
	 when ( t.finance_subject = '110') then '可转债'
	 when ( t.finance_subject = '1103') then '债券'
	 when ( left(t.finance_subject,4) = '1105') then '基金'
	 else '其他'
     end as asset_type,asset_ratio
from BondFund.dbo.AssetAllocation_Saa('{0}', '{1}') t

--2表：债券类属资产 bondnature
select t.gzdate,t2.bondtype,t.asset_ratio
from BondFund.dbo.AssetAllocation_Ta_FixedIncomeAsset('{0}', '{1}') t
left join bondfund..bondtypeinfo t2 on t.bondnature=t2.bondnature;

--3表：债券市场配置 secumarket
select t.gzdate,
	case when ( t.secumarket = 89 ) then '银行间'   
		 when ( t.secumarket = 83 ) then '深交所'
		 when ( t.secumarket = 90 ) then '上交所'
		 else '其他'
         end as secumarket_type,t.asset_ratio
from BondFund.dbo.AssetAllocation_BondMarket('{0}', '{1}') t;

--4表：债券利率类型 compoundmethod
select t.gzdate,
	case when ( t.compoundmethod = 1 ) then '固定利率'   
		 when ( t.compoundmethod = 3 ) then '浮动利率'
		 when ( t.compoundmethod = 4 ) then '累进利率'
		 when ( t.compoundmethod = 5 ) then '贴现'
		 when ( t.compoundmethod = 6 ) then '无序利率'
		 else '其他'
         end as compoundmethod_type,t.asset_ratio
from BondFund.dbo.AssetAllocation_BondCompoundMethod('{0}', '{1}') t;

--5表：期限分布（到期）年限
SELECT t.gzdate,
case when ( datediff(day,getdate(),t.enddate) <= 365 ) then '1、1年以下'   
when ( datediff(day,getdate(),t.enddate) > 365   and datediff(day,getdate(),t.enddate) <= 365*3 ) then '2、1年-3年'   
when ( datediff(day,getdate(),t.enddate) > 365*3 and datediff(day,getdate(),t.enddate) <= 365*5 ) then '3、3年-5年'   
when ( datediff(day,getdate(),t.enddate) > 365*5 and datediff(day,getdate(),t.enddate) <= 365*7 ) then '4、5年-7年'   
when ( datediff(day,getdate(),t.enddate) > 365*7 and datediff(day,getdate(),t.enddate) <= 365*10) then '5、7年-10年'   
when ( datediff(day,getdate(),t.enddate) > 365*10) then '6、10年以上' end as 剩余期限,
sum(t.asset_ratio) as asset_ratio
FROM bondfund..FundAsset_BondDetail('{0}', '{1}') t
group by t.gzdate,
case when ( datediff(day,getdate(),t.enddate) <= 365 ) then '1、1年以下'   
when ( datediff(day,getdate(),t.enddate) > 365   and datediff(day,getdate(),t.enddate) <= 365*3 ) then '2、1年-3年'   
when ( datediff(day,getdate(),t.enddate) > 365*3 and datediff(day,getdate(),t.enddate) <= 365*5 ) then '3、3年-5年'   
when ( datediff(day,getdate(),t.enddate) > 365*5 and datediff(day,getdate(),t.enddate) <= 365*7 ) then '4、5年-7年'   
when ( datediff(day,getdate(),t.enddate) > 365*7 and datediff(day,getdate(),t.enddate) <= 365*10) then '5、7年-10年'   
when ( datediff(day,getdate(),t.enddate) > 365*10) then '6、10年以上' end;

--6表：信用债配置期限比例
SELECT t.gzdate,
	case when ( datediff(day,getdate(),t.enddate) <= 365*2 ) then '2年以下'   
		 else '2年以上'
         end as 剩余期限,
	sum(t.asset_ratio) as asset_ratio
FROM bondfund..FundAsset_BondDetail('{0}', '{1}') t
where t.bondnature not in (1,2,4,10,16)
group by t.gzdate,case when ( datediff(day,getdate(),t.enddate) <= 365*2 ) then '2年以下' else '2年以上' end;

--7表：主体信用评级
select t.* from bondfund.dbo.AssetAllocation_BondZtRating('{0}', '{1}') t 
order by t.ztrating desc;

--8表：债项信用评级
select t.* from bondfund.dbo.AssetAllocation_BondZxRating('{0}', '{1}') t
order by t.zxrating desc;

--9表：债券明细（全部）
SELECT t1.*,convert(varchar,t1.enddate,23) as enddate_,
	case when ( t1.secumarket = 89 ) then '银行间'   
		 when ( t1.secumarket = 83 ) then '深交所'
		 when ( t1.secumarket = 90 ) then '上交所'
		 else '其他'
         end as secumarket_type,
	case when ( t1.compoundmethod = 1 ) then '固定利率'   
		 when ( t1.compoundmethod = 3 ) then '浮动利率'
		 when ( t1.compoundmethod = 4 ) then '累进利率'
		 when ( t1.compoundmethod = 5 ) then '贴现'
		 when ( t1.compoundmethod = 6 ) then '无序利率'
		 else '其他'
         end as compoundmethod_type,t2.bondtype
 FROM bondfund..FundAsset_BondDetail('{0}', '{1}') t1
 left join bondfund..bondtypeinfo t2 on t1.bondnature=t2.bondnature;

--10表：可转债
select  m1.gzdate,m1.fundcode,m1.finance_subject,m1.SecuCode,m1.SecuId,m1.MainCode,m1.finance_name,m1.stock_amount/10000 as cap,m1.capvalue/10000 as capvalue,m1.asset_ratio,m1.drift_profit/10000 as earn,
m1.SecuMarket,m1.BondNature,m1.pmll as CoupornRate,m1.zqqx as IssueTerm,m2.BondIssuerCR as Ztrating,
m3.bondtype,
case when ( m1.secumarket = 89 ) then '银行间'   
	 when ( m1.secumarket = 83 ) then '深交所'
	 when ( m1.secumarket = 90 ) then '上交所'
	 else '其他'
        end as secumarket_type
from
	(select t1.*,t2.CompoundMethod,t2.PayInterestEffency,t2.EndDate,t2.CreditRating from
		(select p1.*,p2.Secucode,p2.SecuId,p2.MainCode,p2.SecuMarket,p2.BondNature,p2.pmll,p2.zqqx from
			(select gzdate,fundcode,finance_subject,finance_name,stock_amount,stock_asset as capvalue,asset_ratio,drift_profit
			from infocenter..fundjszgzdata
			where gzdate='{1}' and fundcode='{0}'
			and len(finance_subject)=14 and left(finance_subject,8) in
				(select finance_subject from bondfund..Risk_FinancesubjectType where AssetLevel1=6 and fundcode='{0}')) as p1
			left join BondFund..BondCodelist as p2
			on right(p1.finance_subject,6)=p2.HSDM) as t1
	left join InfoCenter..Bond_BasicInfo t2
	on t1.MainCode=t2.MainCode) as m1
left join BondFund..Bond_CreditRating_New as m2 on m1.MainCode=m2.MainCode
left join bondfund..bondtypeinfo m3 on m1.bondnature=m3.bondnature;

--11表：股票
select  m1.gzdate,m1.fundcode,m1.finance_subject,m1.finance_name,m1.stock_amount/10000 as cap,m1.capvalue/10000 as capvalue,m1.asset_ratio,m1.drift_profit/10000 as earn
from
(select gzdate,fundcode,finance_subject,finance_name,stock_amount,stock_asset as capvalue,asset_ratio,drift_profit
from infocenter..fundjszgzdata
where gzdate='{1}' and fundcode='{0}'
and len(finance_subject)=14 and left(finance_subject,8) in
(select finance_subject from bondfund..Risk_FinancesubjectType where AssetLevel1=4 and fundcode='{0}')
) as m1;
--12表：基金
select  m1.gzdate,m1.fundcode,m1.finance_subject,m1.finance_name,m1.stock_amount/10000 as cap,m1.capvalue/10000 as capvalue,m1.asset_ratio,m1.drift_profit/10000 as earn
from
(select gzdate,fundcode,finance_subject,finance_name,stock_amount,stock_asset as capvalue,asset_ratio,drift_profit
from infocenter..fundjszgzdata
where gzdate='{1}' and fundcode='{0}'
and len(finance_subject)=14 and left(finance_subject,8) in
(select finance_subject from bondfund..Risk_FinancesubjectType where AssetLevel1=7 and fundcode='{0}')
) as m1;
--13表：资产证券化
select  m1.gzdate,m1.fundcode,m1.finance_subject,m1.finance_name,m1.stock_amount/10000 as cap,m1.capvalue/10000 as capvalue,m1.asset_ratio,m1.drift_profit/10000 as earn
from
(select gzdate,fundcode,finance_subject,finance_name,stock_amount,stock_asset as capvalue,asset_ratio,drift_profit
from infocenter..fundjszgzdata
where gzdate='{1}' and fundcode='{0}'
and len(finance_subject)=14 and left(finance_subject,8) in
(select finance_subject from bondfund..Risk_FinancesubjectType where AssetLevel2=13 and fundcode='{0}')
) as m1;
            ", fundcode, date);
            bondInfoDataSet_keyIndex = sqh.ExecuteDataSetText(sql_str, null);
            return bondInfoDataSet_keyIndex;
        }

//        public static DataSet getBondInfoDetail(string fundcode, string date)
//        {
//            SqlHelper sqh = new SqlHelper("con_BondFund");
//            string sql_str = string.Format(@"
//--全部
//SELECT * FROM bondfund..FundAsset_BondDetail('{0}', '{1}');

//--债券
//SELECT * FROM bondfund..FundAsset_BondDetail('{0}', '{1}')
//where bondnature=1;

//            ", fundcode, date);
//            bondInfoDataSet_bonddetail = sqh.ExecuteDataSetText(sql_str, null);
//            return bondInfoDataSet_bonddetail;
//        }
    }
}
