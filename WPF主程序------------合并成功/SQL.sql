--��¼Ȩ��
SELECT t1.[FundCode],t2.managercode,t2.managername
  FROM [BondFund].[dbo].[OperatorAuthority] t1
  left join [BondFund].[dbo].[UserLoginInfo] t2
  on t1.managercode = t2.managercode
----------------------------------------------------------
FundAsset_BondDetail(@fundcode,@gzdate��
select distinct t.finance_subject,finance_name from Infocenter..fundjszgzdata t
--0��:
SELECT * FROM bondfund..FundAsset_BondDetail('000366', '2016-10-21')
exec bondfund..sp_GetBondDetail_copy '519078', '2016-10-20'
select * from Infocenter..fundjszgzdata t where fundcode in ('519078') and gzdate='2016-10-20' order by finance_subject

--�ʲ����
select * from Bondfund.. Risk_AssetGzCodeclassification;
select * from BondFund.. Risk_FinancesubjectType order by assetlevel1,assetlevel2,finance_subject,fundcode;
--��ƿ�Ŀ��Ӧ�ʲ����
select a.fundcode,a.finance_subject,l1.assetname as assetlevel1,l2.assetname as assetlevel2 
from BondFund.. Risk_FinancesubjectType a 
left join (select * from Bondfund.. Risk_AssetGzCodeclassification where assetlevel=1) l1 on a.assetlevel1=l1.assetcode
left join (select * from Bondfund.. Risk_AssetGzCodeclassification where assetlevel=2) l2 on a.assetlevel2=l2.assetcode

--1�������ʲ�����(��ƿ�Ŀ�����Թ�ֵ��) finance_subject
select t.gzdate,
case when ( t.finance_subject = '100201') then '���ڴ��'
     when ( t.finance_subject = '100203') then '���ڴ��'
	 when ( t.finance_subject = '100360') then '�浥'
	 when ( t.finance_subject = '1202') then '�ع�'
	 when ( t.finance_subject = '1102') then '��Ʊ'
	 when ( t.finance_subject = '110') then '��תծ'
	 when ( t.finance_subject = '1103') then 'ծȯ'
	 when ( left(t.finance_subject,4) = '1105') then '����'
	 else '����'
     end as asset_type,asset_ratio
from BondFund.dbo.AssetAllocation_Saa('470088','2016-08-22') t
select * from Infocenter..fundjszgzdata t
where t.finance_subject='1202'
and t.gzdate between '2016-09-01' and '2016-09-30'

select t1.*,t2.* from (select * from BondFund.dbo.AssetAllocation_Saa('519888','2016-08-22')) t1
left join
(select a.fundcode,a.finance_subject,l1.assetname as assetlevel1,l2.assetname as assetlevel2 
from BondFund.. Risk_FinancesubjectType a 
left join (select * from Bondfund.. Risk_AssetGzCodeclassification where assetlevel=1) l1 on a.assetlevel1=l1.assetcode
left join (select * from Bondfund.. Risk_AssetGzCodeclassification where assetlevel=2) l2 on a.assetlevel2=l2.assetcode
) t2 
on t1.finance_subject=t2.finance_subject

--2��ծȯ�����ʲ� bondnature
select * from BondFund.dbo.AssetAllocation_Ta_FixedIncomeAsset('519888','2016-08-22')
select t.gzdate,
	case when ( t.bondnature = 1 ) then '��ҵծ'   
		 when ( t.bondnature = 2 ) then '����ծ'
		 when ( t.bondnature = 3 ) then '���ڴμ�ծ'
	     when ( t.bondnature = 4 ) then '��ծ'   
	     when ( t.bondnature = 5 ) then '��Ʊ'   
	     when ( t.bondnature = 6 ) then '����'   
	     when ( t.bondnature = 7 ) then 'MBS(����֧��)'   
	     when ( t.bondnature = 8 ) then 'ABS(����֧��)'   
	     when ( t.bondnature = 9 ) then '����ʱ�ծȯ'   
	     when ( t.bondnature = 10) then '�����ֽ����'   
	     when ( t.bondnature = 13) then '��˾ծ'   
	     when ( t.bondnature = 14) then '��Ʊ'   
	     when ( t.bondnature = 16) then '�ط�����ծȯ'   
	     when ( t.bondnature = 19) then '������'   
		 else '����'
         end as bond_type,t.asset_ratio
from BondFund.dbo.AssetAllocation_Ta_FixedIncomeAsset('519888','2016-08-22') t

1-��ҵծȯ��2-����ծȯ��3-���ڴμ�ծ��4-��ծ�ֻ���5-����Ʊ�ݣ�6-��������ȯ��7-MBS(����֧��)
8-ABS(����֧��)��9-����ʱ�ծȯ��11-�����ֽ����13-��˾ծȯ��14-����Ʊ�ݣ�16-�ط�����ծȯ(����ծ)
17-��С��ҵ����Ʊ�ݣ�18-����ծȯ
	--case when ( t.bondnature in (2,16)) then '����ծ'
	--     when ( t.bondnature=6) then '����'   
	--     when ( t.bondnature=19) then '������'   
	--     when ( t.bondnature=4) then '��ծ'   
	--     when ( t.bondnature=1) then '��ҵծ'   
	--     when ( t.bondnature=5) then '��Ʊ'   
	--     when ( t.bondnature=13) then '��˾ծ'   
	--     when ( t.bondnature=14) then '��Ʊ'   
	--	 else '����'
 --        end as 'ծȯ����',

--3��ծȯ�г����� secumarket
select * from BondFund.dbo.AssetAllocation_BondMarket('519888','2016-08-22')
select t.gzdate,
	case when ( t.secumarket = 89 ) then '���м�'   
		 when ( t.secumarket = 83 ) then '���'
		 when ( t.secumarket = 90 ) then '�Ͻ���'
		 else '����'
         end as secumarket_type,t.asset_ratio
from BondFund.dbo.AssetAllocation_BondMarket('519888','2016-08-22') t


--4��ծȯ�������� compoundmethod
1-�̶����ʣ�3-�������ʣ�4-�۽����ʣ�5-���֣�6-��������
select * from BondFund.dbo.AssetAllocation_BondCompoundMethod('519888','2016-08-22')
select t.gzdate,
	case when ( t.compoundmethod = 1 ) then '�̶�����'   
		 when ( t.compoundmethod = 3 ) then '��������'
		 when ( t.compoundmethod = 4 ) then '�۽�����'
		 when ( t.compoundmethod = 5 ) then '����'
		 when ( t.compoundmethod = 6 ) then '��������'
		 else '����'
         end as compoundmethod_type,t.asset_ratio
from BondFund.dbo.AssetAllocation_BondCompoundMethod('519888','2016-08-22') t

--5�����޷ֲ������ڣ�����
SELECT t.gzdate,
case when ( datediff(day,getdate(),t.enddate) <= 365 ) then '1������'   
when ( datediff(day,getdate(),t.enddate) > 365   and datediff(day,getdate(),t.enddate) <= 365*3 ) then '1��-3��'   
when ( datediff(day,getdate(),t.enddate) > 365*3 and datediff(day,getdate(),t.enddate) <= 365*5 ) then '3��-5��'   
when ( datediff(day,getdate(),t.enddate) > 365*5 and datediff(day,getdate(),t.enddate) <= 365*7 ) then '5��-7��'   
when ( datediff(day,getdate(),t.enddate) > 365*7 and datediff(day,getdate(),t.enddate) <= 365*10) then '7��-10��'   
when ( datediff(day,getdate(),t.enddate) > 365*10) then '10������' end as ʣ������,
sum(t.asset_ratio) as asset_ratio
FROM bondfund..FundAsset_BondDetail('519078', '2016-10-20') t
group by t.gzdate,
case when ( datediff(day,getdate(),t.enddate) <= 365 ) then '1������'   
when ( datediff(day,getdate(),t.enddate) > 365   and datediff(day,getdate(),t.enddate) <= 365*3 ) then '1��-3��'   
when ( datediff(day,getdate(),t.enddate) > 365*3 and datediff(day,getdate(),t.enddate) <= 365*5 ) then '3��-5��'   
when ( datediff(day,getdate(),t.enddate) > 365*5 and datediff(day,getdate(),t.enddate) <= 365*7 ) then '5��-7��'   
when ( datediff(day,getdate(),t.enddate) > 365*7 and datediff(day,getdate(),t.enddate) <= 365*10) then '7��-10��'   
when ( datediff(day,getdate(),t.enddate) > 365*10) then '10������' end;

--6������ծ�������ޱ���
SELECT t.gzdate,
	case when ( datediff(day,getdate(),t.enddate) <= 365*2 ) then '2������'   
		 else '2������'
         end as ʣ������,
	sum(t.asset_ratio) as asset_ratio
FROM bondfund..FundAsset_BondDetail('519078', '2016-10-20');

--7��������������
select t.* from bondfund.dbo.AssetAllocation_BondZtRating('{0}', '{1}') t 
order by t.ztrating desc;

--8��ծ����������
select t.* from bondfund.dbo.AssetAllocation_BondZxRating('{0}', '{1}') t
order by t.zxrating desc;

--ծȯ��ϸ
exec bondfund..sp_GetBondDetail_copy '519888', '2016-09-12'

exec bondfund..sp_GetBondDetail '519888', '2016-09-12'

--�ǵ���������
select t1.assetid,t1.name1,
t1.Stdev05,  t1.Stdevcd05,  t1.Stdev10,  t1.Stdevcd10,
t1.Stdev20,  t1.Stdevcd20,  t1.Stdev60,  t1.Stdevcd60,
t2.YTM,t2.YTM_CD
from 
(SELECT 
a.assetId, a.name1, convert(varchar,aa.pricingdate,23) as pricingdate, ar.parentlevel,
aa.Stdev05, 
aa.Stdevcd05, cast(Convert(decimal(18,2),aa.Stdevcd05*100) as varchar )+'%' as Stdevcd05,
aa.Stdev10, 
aa.Stdevcd10,
aa.Stdev20, 
aa.Stdevcd20, 
aa.Stdev60, 
aa.Stdevcd60, 
aa.Stdev120, 
aa.Stdevcd120, 
aa.Stdev250, 
aa.Stdevcd250
FROM AA.dbo.assetsector ar, AA.dbo.Volatility aa, AA.dbo.asset a
WHERE  aa.pricingdate='2016-09-30'
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
WHERE aa.pricingdate='2016-09-30'
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
---
SELECT  convert(varchar,aa.pricingdate,23) as pricingdate, aa.measurevalue as YTM
FROM AA.dbo.assetsector ar, AA.dbo.asset a , AA.dbo.AssetAnalytic aa
WHERE aa.pricingdate between '2016-09-01' and '2016-09-30'
AND aa.AssetId='154' 
AND (ar.parentlevel = 8 OR ar.parentlevel = 9)
AND aa.assetid = ar.assetid 
AND aa.assetid = a.assetid 
AND ar.modelid = 0 
AND ar.classid = 1 
AND aa.measureId = 2 
ORDER BY ar.parentlevel, aa.assetid


-------------------------------------------------------------------------------------
select * from Infocenter..fundjszgzdata 
where fundcode='159005' and gzdate='2016-09-27' order by finance_subject
            

SELECT Name FROM infocenter..SysObjects 
Where XType='U' 
and name like '%date%'
ORDER BY Name

select * from infocenter..tradingdate
where tradingday between '2015-01-01' and '2015-12-31'
and exchange='SS'
order by tradingday desc

    WITH tmpDates AS
    (
		SELECT tradingday, DENSE_RANK() OVER (ORDER BY tradingday desc) AS daysAgo
		FROM infocenter..tradingdate
		WHERE exchange='SS'
		AND istradingday = 1
		AND tradingday <  DATEADD(dd, 0, DATEDIFF(dd, 0, '2016-09-22')) 
		AND tradingday >= DATEADD(dd, 0, DATEDIFF(dd, 0, '2016-09-22')) -60
    )
    SELECT convert(varchar,tradingday,23) as tradingdate--'2016-09-22' = tradingday
    FROM tmpDates
    WHERE daysAgo between 1 and 2




--
select t.������,sum(t.���*(1+t.����*t.����/360)) as ����
from(
select ckJe/10000 as ���, jxlv/100 as ����,pzdate as �����,zqdate as ������,
datediff(day,pzdate,zqdate) as ����,
datediff(day,getdate(),zqdate) as ʣ������
from Infocenter..FundJSZyhjckywData
where fundcode='000330' and zqdate>'2016-09-26' 
and ywlb='���ڴ��'
--order by zqdate
) t
group by t.������

--�浥��ϸ
select t.������,sum(t.���*(1+t.Ʊ����Ϣ*t.��Ϣ����/365)) as �浥����
from(
select gzdate as ����,
finance_name as �浥����,
zqamount as ���,
ghdate as ������,
expr1  as Ʊ����Ϣ,
jxts as ��Ϣ����,
ghdate as ʣ������
from bondfund..fundcdinfo_vw
where fundcode='000330' and gzdate='2016-09-26' 
) t
group by t.������





--���
select ckJe/10000 as ���, jxlv/100 as ����,pzdate as �����,zqdate as ������,
datediff(day,pzdate,zqdate) as ����,
datediff(day,getdate(),zqdate) as ʣ������
from Infocenter..FundJSZyhjckywData
where fundcode='000330' and zqdate>'2016-08-23' 
and ywlb='���ڴ��'
order by zqdate

select t.zqdate as ������,convert(decimal(18,0),sum(zqamount)) as ���,
--(sum(zqamount*jxlv)/sum(zqamount)*100) as varchar+'%'
cast(Convert(decimal(18,2),sum(interest)/sum(zqamount)/100) as varchar )+'%' as ��Ϣ
from (
select zqdate,ckje/10000 as zqamount,jxlv/100 as jxlv,ckje*jxlv/100 as interest from infocenter..FundJSZyhjckywData
where fundcode='000330' and zqdate>'2016-09-16' and ywlb='���ڴ��'
) t
group by t.zqdate order by t.zqdate--��������

select t.ghdate as ������,convert(decimal(18,0),sum(zqamount)) as ���,
--(sum(zqamount*jxlv)/sum(zqamount)*100) as varchar+'%'
cast(Convert(decimal(18,2),sum(interest)/sum(zqamount)*100) as varchar )+'%' as ��Ϣ
from (
select ghdate,zqamount,expr1,zqamount*expr1 as interest from Bondfund..FundCdInfo_vw
where fundcode='000330' and gzdate='2016-09-16' and ghdate>'2016-09-16'
) t
group by t.ghdate order by t.ghdate--�浥��������

select t.zqdate as ������,convert(decimal(18,0),sum(zqamount)) as ���,
--(sum(zqamount*jxlv)/sum(zqamount)*100) as varchar+'%'
cast(Convert(decimal(18,2),sum(interest)/sum(zqamount)/100) as varchar )+'%' as ��Ϣ
from (
select zqdate,ckje/10000 as zqamount,jxlv/100 as jxlv,ckje*jxlv/100 as interest from infocenter..FundJSZyhjckywData
where fundcode='000330' and zqdate>'2016-09-16' and ywlb='���ڴ��'
union all
select ghdate,zqamount,expr1,zqamount*expr1 as interest from Bondfund..FundCdInfo_vw
where fundcode='000330' and gzdate='2016-09-16' and ghdate>'2016-09-16'
) t
group by t.zqdate order by t.zqdate--��������




with t as
(
select sum(ckje/10000) as total_ckje 
from infocenter..FundJSZyhjckywData 
where fundcode='000330' and zqdate>'2016-09-16'  
)
Select yhzh,sum(ckje/10000),convert(decimal(18,2),sum(ckje/100)/t.total_ckje) as ckratio 
from infocenter..FundJSZyhjckywData,t 
where fundcode='000330' and zqdate>'2016-09-16' and ywlb='���ڴ��' 
group by yhzh,t.total_ckje--������

Select yhzh,sum(ckje/10000) 
from infocenter..FundJSZyhjckywData 
where fundcode='000330' and zqdate>'2016-09-16' and ywlb='���ڴ��' 
group by yhzh
----------------------------------------------
exec bondfund..sp_CKandCDbyDate '000330','2016-09-16',2

----�����ʲ�����ͳ�ƣ�3��1������3��2������3��3�������ڴ�����+�浥
----------------------------------------------
select sum(t.amount),sum(t.amount*t.yield)/sum(t.amount),sum(t.amount*t.term)/sum(t.amount)
from (
select ckJe/10000 as amount, jxlv/100 as yield,
--pzdate,zqdate,datediff(day,pzdate,zqdate) as ckqx,
datediff(day,getdate(),zqdate) as term
from Infocenter..FundJSZyhjckywData
where fundcode='000330' and zqdate>'2016-09-20' and ywlb='���ڴ��' --order by zqdate
union all --����ȥ��
select zqmktvalue as amount,expr1 as yield,datediff(day,getdate(),ghdate) as term  
from bondfund..fundcdinfo_vw
where fundcode='000330' and gzdate='2016-09-20'-- order by ghdate
) t
where t.term>0
-----------------------------------------------

select zqmktvalue as amount,expr1 as yield,datediff(day,getdate(),ghdate) as cdqx  
from bondfund..fundcdinfo_vw
where fundcode='000330' and gzdate='2016-08-15' order by ghdate



--�浥���£�
select a.fundcode,a.gzdate,a.finance_subject,a.finance_name,a.stock_amount,a.asset_ratio,b.secuid,b.pmll,b.jzrq
from infocenter..fundjszgzdata a
left join bondfund..bondcodelist b on right(a.finance_subject,6)=b.hsdm
where a.fundcode='000330' and a.gzdate='2016-08-15' and
len(a.finance_subject)=14 and left(a.finance_subject,8) in 
(select finance_subject from bondfund..Risk_FinancesubjectType 
where fundcode='000330' and assetlevel1=3)

--�浥
select t.*,datediff(day,getdate(),t.ghdate)  from bondfund..fundcdinfo_vw t
where t.fundcode='000330' and t.gzdate='2016-10-20' order by t.ghdate

select * from bondfund..fundcdinfo_vw where fundcode='000330' and gzdate='2016-10-14' order by ghdate
--��ֵ��
select * from Infocenter..fundjszgzdata t
where fundcode in ('000366') and 
--t.finance_subject like ('902%000366%') and 
gzdate='2016-08-21' order by finance_subject

select t.fundcode,t.finance_subject from Infocenter..fundjszgzdata t
where fundcode in ('000330','000366','000397','000600','159005','470014','470030','470060','471007','519518','519888')
and t.finance_subject like ('902%')
and gzdate='2016-08-21' --order by finance_subject
order by t.fundcode
--�ع����ݡ���ȯ���ݺ�������������
select * from Infocenter..fundjszcjhbdata 
where fundcode='519888' and maturedate>='2016-07-08'
order by zqbz
--�ӹ�ֵ���ȡ��ȥ6�������������
select top 6 gzdate,finance_name from Infocenter..fundjszgzdata 
where finance_subject='902��������ÿ���519889�������棺' and fundcode='519888'
order by gzdate desc
--ծȯ��ϸ
select distinct t.fundcode 
from(
exec bondfund..sp_GetBondDetail_copy '000330', '2016-09-12'
)  t
exec BondFund..sp_GetBondDetail '000330', '2016-09-12'
--�ع�
select *
from Infocenter..fundjszcjhbdata t
where fundcode='000330' and maturedate>='2016-10-20'
and t.zqbz='HG'and voucherdate<'2016-10-20' and maturedate>'2016/8/18'
order by maturedate desc

select * from Infocenter..fundjszcjhbdata where fundcode='000330' and maturedate>='2016/8/18'
and zqbz='HG'

select * from Infocenter..fundjszcjhbdata where fundcode='000330' and maturedate>='2016-08-18'
and voucherdate<='2016-08-18' and maturedate>'2016-08-18'           


select (ghbalance + hggain)/10000 as '���', 
case when voucherdate!=maturedate then ((ghbalance + hggain) / real_balance - 1) / datediff(day,voucherdate,maturedate) * 356 
	 else 0
	 end as '����',
datediff(day,voucherdate,maturedate) as '����',
case when exchange_type=5 and entrust_bs=1 then '��ȯ'
	 when exchange_type=5 and entrust_bs=2 then '����'
	 when exchange_type=1 and entrust_bs=2 then '��ȯ'
	 when exchange_type=1 and entrust_bs=1 then '����'
	 end as '���׷���',
dealdate as '������',
case when dealdate=voucherdate then 'T+0'
	 else 'T+1'
	 end as '����Ч��',
case when exchange_type=5 then '���м�'
	 else '������'
	 end as '�г�',
voucherdate as '������',maturedate as '�ع�������',datediff(day,getdate(),maturedate) as 'ʣ������'
from Infocenter..fundjszcjhbdata
where fundcode='000330'-- and maturedate='2016-08-18'
and zqbz='HG'
and voucherdate<='2016-10-16' and maturedate>='2016-10-16'
--��ȯ���׺�����

select *
from Infocenter..fundjszcjhbdata t
where fundcode='000330' and maturedate>='2016-10-24'
and voucherdate between '2016-10-24' and dateadd(day, 30, getdate ())
and t.zqbz='ZQ'
order by 


exec bondfund..sp_GetBondDetail_copy '000366', '2016-10-21';




















--------------------------------------------------------------------------------
select top 10 * from infocenter..fundjszcjhbdata where fundcode='519888'

select * from jydb..[Bond_CBYieldCurve] c
where c.curvecode=195 and c.yearstomaturity=10
order by c.enddate desc

(select zqdate,ckje/10000 as zqamount,jxlv/100 as jxlv from infocenter..FundJSZyhjckywData
where fundcode='000330' and zqdate>'2016-07-20' and ywlb='���ڴ��')
union all
(select ghdate,zqamount,expr1 from Bondfund..FundCdInfo_vw
where fundcode='000330' and gzdate='2016-07-20' and ghdate>'2016-07-20')
order by zqdate


select t.*,datediff(day,getdate(),t.ghdate)  from bondfund..fundcdinfo_vw t
where t.fundcode='000330' and t.gzdate='2016-07-27' order by t.ghdate


--���ĳһ������һ�����Ƿ����
select t1.gzdate,t1.fundcode,t1.finance_subject,
right(t1.finance_subject,6) as secucode, t1.finance_name,
--case when t2.secuabbr is null then '0'
--     else '1'
--	 end as ifHS300
case when (select count(SecuCode) from bondfund..hs300element where right(t1.finance_subject,6)=secucode)>0
then 1
else 0
end as ifHS300
from infocenter..FundJSZGzData t1
--left join bondfund..hs300element t2 on right(t1.finance_subject,6)=t2.secucode
where t1.gzdate='2016-07-28' and t1.fundcode='470010'
and LEN(t1.finance_subject) = 14
and (t1.finance_subject LIKE '11020101%' or  t1.finance_subject LIKE '11023101%')

select t1.gzdate,t1.fundcode,t1.finance_subject,
right(t1.finance_subject,6) as secucode, t1.finance_name,
case when (select count(SecuCode) from bondfund..hs300element where right(t1.finance_subject,6)=secucode)>0
then 1
else 0
end as ifHS300
from infocenter..FundJSZGzData t1
where t1.gzdate='2016-07-28' and t1.fundcode='470010'
and LEN(t1.finance_subject) = 14
and (t1.finance_subject LIKE '11020101%' or  t1.finance_subject LIKE '11023101%')


select distinct t.curvetype,t.curvecode,t.yearstomaturity 
from bondfund..bond_cbyieldcurve t
where t.curvetype like ('%��ծ%')
order by t.curvecode,t.yearstomaturity

select t.curvetype,t.curvecode,t.yearstomaturity,t.yield
from bondfund..bond_cbyieldcurve t
where t.curvecode=10
and t.enddate='2016-11-10'
order by t.curvecode,t.yearstomaturity

