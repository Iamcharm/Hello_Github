using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication3.Data.PortfolioRiskBudget;


namespace WpfApplication3
{
    /// <summary>
    /// Tab_PortfolioRiskBudget.xaml 的交互逻辑
    /// </summary>
    public partial class Tab_PortfolioRiskBudget : UserControl
    {
        public Tab_PortfolioRiskBudget()
        {
            InitializeComponent();
            selectDate_riskBudget.Text = DateTime.Now.ToString("yyyy-MM-dd").ToString();
            BindCombobox_riskBudget();
            RiskAssetDecompose.getRiskAssetDecompose(portfolioBox_riskBudget.SelectedValue.ToString(), GlobalParameters.lastTradingDate);
            var query = from t in RiskAssetDecompose.riskBenchmark.AsEnumerable()
                               group t by new { t1 = t.Field<string>("assettype")/*, t2 = t.Field<string>("债券类型")*/ } into m
                               //orderby m.Key.t1
                               select new
                               {
                                   资产类别 = m.Key.t1,
                                   //债券类型 = m.Key.t2,
                                   净值占比 = m.Sum(n => n.Field<decimal?>("tmp_asset_ratio")),
                                   波动率20日 = m.Sum(n => n.Field<decimal?>("tmp_asset_stdev20_all")),
                                   波动率60日 = m.Sum(n => n.Field<decimal?>("tmp_asset_stdev60_all")),
                                   Var20日 = m.Sum(n => n.Field<decimal?>("tmp_asset_var20_all")),
                                   Var60日 = m.Sum(n => n.Field<decimal?>("tmp_asset_var60_all")),
                               };
            risk.ItemsSource = query.ToList();
            bar_ratio.DataSource = query.ToList();
            line_stdvar.DataSource = query.ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("情景分析");
            dt.Columns.Add("20天波动区间");
            dt.Columns.Add("60天波动区间");
            dt.Columns.Add("20天最大回撤");
            dt.Columns.Add("60天最大回撤");
            dt.Rows.Add("上行区间", "-", "-", "-", "-");
            dt.Rows.Add("上行区间", "0.46%", "-0.07%", "-0.63%", "-0.41%");
            situationAnalysis.ItemsSource = dt.DefaultView;
        }
        private void BindCombobox_riskBudget()
        {
            portfolioBox_riskBudget.ItemsSource = GlobalParameters.portfolio_bond;
            portfolioBox_riskBudget.SelectedValuePath = "Key";
            portfolioBox_riskBudget.DisplayMemberPath = "Value";
            portfolioBox_riskBudget.SelectedIndex = 0;
        }
    }
}
