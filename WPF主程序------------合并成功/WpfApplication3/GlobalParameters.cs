using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace WpfApplication3
{
    public static class GlobalParameters
    {
        #region 货币参数
        public static string fundCode { get; set; }
        public static string lastTradingDate { get; set; }
        public static string last2TradingDate { get; set; }
        public static DataTable dt_postTradingDate;
        public static List<string> TradingDateSeries;
        public static string Date { get; set; }
        //{
        //    get
        //    {
        //        return DateTime.Now.ToString("yyyy-MM-dd").ToString();
        //    }
        //}

        public static Dictionary<string, string> portfolio_1 = new Dictionary<string, string>()
        {
            {"000330","现金宝"},
            {"000397","全额宝"},
            {"000600","和聚宝"}
        };

        public static Dictionary<string, string> portfolio_2 = new Dictionary<string, string>()
        {
            {"000366","添富通"},
            {"159005","添富快钱(深)"},
            {"519888","添富快线(沪)"},
            {"471007","理财7天"},
            {"470014","理财14天"},
            {"470030","理财30天"},
            {"470060","理财60天"},
            {"519518","添富货币"}
        };

        public static void getTradingdates(string inputDate)
        {
            SqlHelper sqh = new SqlHelper("con_Infocenter");
            string sql_TradingDate = string.Format(@"
--前工作日
    WITH tmpDates AS
    (
		SELECT tradingday, DENSE_RANK() OVER (ORDER BY tradingday desc) AS daysAgo
		FROM infocenter..tradingdate
		WHERE exchange='SS'
		AND istradingday = 1
		AND tradingday <  DATEADD(dd, 0, DATEDIFF(dd, 0, '{0}')) 
		AND tradingday >= DATEADD(dd, 0, DATEDIFF(dd, 0, '{0}')) -60
    )
    SELECT convert(varchar,tradingday,23) as tradingdate
    FROM tmpDates
    WHERE daysAgo between 1 and 2;

--后工作日
    WITH tmpDates AS
    (
		SELECT tradingday, DENSE_RANK() OVER (ORDER BY tradingday) AS daysAgo
		FROM infocenter..tradingdate
		WHERE exchange='SS'
		AND istradingday = 1
		AND tradingday <  DATEADD(dd, 0, DATEDIFF(dd, 0, '{0}')) + 60
		AND tradingday >= DATEADD(dd, 0, DATEDIFF(dd, 0, '{0}')) 
    )
    SELECT convert(varchar,tradingday,23) as tradingdate
    FROM tmpDates
    WHERE daysAgo between 1 and 12
            ", inputDate);
            DataTable dt_preTradingDate  = sqh.ExecuteDataSetText(sql_TradingDate, null).Tables[0];
            dt_postTradingDate = sqh.ExecuteDataSetText(sql_TradingDate, null).Tables[1];
            lastTradingDate  = dt_preTradingDate.Rows[0][0].ToString();
            last2TradingDate = dt_preTradingDate.Rows[1][0].ToString();
            TradingDateSeries = new List<string>();
            TradingDateSeries.Add(lastTradingDate);
            foreach(DataRow dr in dt_postTradingDate.Rows)
            {
                TradingDateSeries.Add(dr[0].ToString());
            }
        }
        #endregion

        #region 债券参数
        public static string fundCode_bond { get; set; }
        public static string Date_bond { get; set; }
        public static Dictionary<string, string> portfolio_bond = new Dictionary<string, string>()
        {
            {"519078","A添富增收"},
            {"470021","A添富优选回报"},
            {"000406","A双利增强"},
            {"000122","A添富实业债"},
            {"000174","A添富高息债"},
            {"000221","A添富年年利"},
            {"000395","A汇添富安心中国债"},
            {"001796","A安鑫智选"},
            {"001801","A添富达欣"},
            {"002419","A盈安保本混合"},
            {"002420","A盈鑫保本混合"},
            {"002487","A稳健添利"},
            {"002959","A盈稳保本"},
            {"164702","A添富季季红"},
            {"164703","A添富互利分级"},
            {"470010","A添富多元收益"},
            {"470018","A双利债券"},
            {"470058","A添富转债"},
            {"470088","A添富6月红"},
            {"Z00104","C工行私行2号"},
            {"Z00117","C平安人寿债券专户1号"},
            {"Z00173","C建行中国人寿固收组合专户"},
            {"Z00174","C宁波添富天安财险专户3号"},
            {"Z00128","C中国人寿混合型组合"},
            {"479019","B工行添富牛2号"},
            {"Z00108","C工行私行3号"},
            {"Z00109","C天安财险添富专户"},
            {"Z00113","C汇添富工行私行5号"},
            {"Z00131","C工行罗莱家纺1号"},
            {"Z00156","C招行华润信托鑫睿1号"},
            {"Z00186","C工行汇添富宝钢1号"},
            {"Z00201","C建行银华资本添富1号"},
            {"Z00153","C上行债添利25号"},
            {"Z00175","C上行债添利添富牛29号"},
            {"Z00187","C招行债添利31号资管计划"},
            {"Z00194","C上海银行债添利32号"},
            {"Z00196","C债添利50号"},
            {"Z00183","C工行平安产险固收1号"},
            {"Z00195","C债添利52号"},
            {"Z00205","C债添利57号"},
            {"Z00202","C建行债添利39号"},
            {"478065","B交行一创金丰债添利1号"},
            {"479010","B光大添富牛2号"},
            {"479068","B上海银行双喜牛3号"},
            {"479071","B工行安中货币1号"},
            {"Z00157","C平安信托1号资管计划"},
            {"Z00135","C国寿集团混合型3号"},
            {"478086","B宁波添添盈3号"},
            {"478091","B兴业添添赢理财5号"},
            {"478093","B兴业添添盈理财6号"},
            {"Z00140","C兴业银行私人银行理财X1号"},
            {"Z00145","C交行货币增强资管计划"},
            {"Z00160","C兴业汇添富兴汇1号"},
            {"Z00170","C兴汇1号2期"},
            {"Z00171","C上行债添利添富牛26号"},
            {"Z00193","C浦发汇添富浦发银行1号"},
            {"Z00199","C招财添富牛78号"},
            {"478030","B宁波债券宝电商1号"},
            {"478044","B民生添添盈理财1号"},
            {"478046","B宁波汇添富添添盈理财2号"},
            {"478095","B招行稳利添富牛79号"},
            {"Z00114","C汇添富森马服饰同业专户1号"},
            {"Z00136","C工行杭州银行1号"},
            {"Z00152","C工行江阴农商行债添利1号"},
            {"Z00165","C广发汇添富广发1号资管计划"},
            {"Z00179","C广发广发2号资管计划"},
            {"Z00184","C广州农商行债添利27号"},
            {"Z00200","C民生债添利23号"},
        };

        #endregion

        #region 投资分析工具参数

        public static Dictionary<string, string> BondYieldTrend_assetType = new Dictionary<string, string>()
        {
            {"154","10年国债"},
            {"157","7年国债"},
            {"159","5年国债"},
            {"161","3年国债"},
            {"163","1年国债"},
            {"170","1年AA+企业债"},
            {"171","3年AA+企业债"},
            {"172","5年AA+企业债"},
            {"173","7年AA+企业债"},
            {"175","1年AA企业债"},
            {"176","3年AA企业债"},
            {"177","5年AA企业债"},
            {"178","7年AA企业债"},
            {"185","1年AAA企业债"},
            {"186","3年AAA企业债"},
            {"187","5年AAA企业债"},
            {"188","7年AAA企业债"},
            {"190","1年AAA城投债"},
            {"191","3年AAA城投债"},
            {"192","5年AAA城投债"},
            {"193","7年AAA城投债"},
            {"195","1年AA+城投债"},
            {"196","3年AA+城投债"},
            {"197","5年AA+城投债"},
            {"198","7年AA+城投债"},
            {"200","1年AA城投债"},
            {"201","3年AA城投债"},
            {"202","5年AA城投债"},
            {"203","7年AA城投债"},
            {"210","1年铁道债"},
            {"211","3年铁道债"},
            {"212","5年铁道债"},
            {"213","7年铁道债"},
            {"214","10年铁道债"},
            {"215","1年AAA中票"},
            {"216","3年AAA中票"},
            {"217","5年AAA中票"},
            {"218","7年AAA中票"},
            {"220","1年AA+中票"},
            {"221","3年AA+中票"},
            {"222","5年AA+中票"},
            {"223","7年AA+中票"},
            {"225","1年AA中票"},
            {"226","3年AA中票"},
            {"227","5年AA中票"},
            {"243","1年国开债"},
            {"244","3年国开债"},
            {"245","5年国开债"},
            {"246","7年国开债"},
            {"247","10年国开债"},
            {"312","1年进出口行债"},
            {"313","3年进出口行债"},
            {"314","5年进出口行债"},
            {"315","7年进出口行债"},
            {"316","10年进出口行债"},
            {"144","德国10年期国债"},
            {"145","美国10年期国债"},
        };

        public static Dictionary<string, string> BondSpread_termSpread_assetType = new Dictionary<string, string>()
        {
            {"1","AAA企业债" },
            {"2","AAA中票短融" },
            {"3","AA+城投债" },
            {"4","AA+企业债" },
            {"5","AA+中票短融" },
            {"6","AA城投债" },
            {"7","AA企业债" },
            {"8","AA中票短融" },
            {"9","国开债" },
            {"10","国债" },
            {"11","口行农发债" },
            {"12","铁道债" },
        };

        public static Dictionary<string, string> BondSpread_curveSpread_assetType = new Dictionary<string, string>()
        {
            {"1","国开-国债" },
            {"2","AA+城投-国开" },
            //{"3","AA+短融-国开" },
            {"4","AA+企业债-国开" },
            {"5","AA+中票-国开" },
            //{"6","AAA短融-国开" },
            {"7","AAA企业债-国开" },
            {"8","AAA中票-国开" },
            {"9","AA城投-国开" },
            //{"10","AA短融-国开" },
            {"11","AA企业债-国开" },
            {"12","AA中票-国开" },
            {"13","非国开-国开" },
            {"14","铁道债-国开" }
        };

        public static Dictionary<string, string> BondSpread_termSpread_term = new Dictionary<string, string>()
        {
            {"1","3Y1Y"},
            {"2","5Y3Y"},
            {"3","7Y5Y"},
            {"4","10Y7Y"},
            {"5","10Y1Y"}
        };

        public static Dictionary<string, string> BondSpread_curveSpread_term = new Dictionary<string, string>()
        {
            {"1","1Y"},
            {"2","3Y"},
            {"3","5Y"},
            {"4","7Y"},
            {"5","10Y"}
        };

        public static string getPrevNBusinessDay(string inputDate, int n)
        {
            SqlHelper sqh = new SqlHelper("con_AA");
            string sql_str = string.Format(@"
select convert(varchar,AA.dbo.fn_prevBusinessDay('{0}',{1}),23)
            ", inputDate, n);
            DataTable dt = sqh.ExecuteDataSetText(sql_str, null).Tables[0];
            string prevNBusinessDay = dt.Rows[0][0].ToString();
            return prevNBusinessDay;
        }
        #endregion
    }
}
