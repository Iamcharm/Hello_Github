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
using WpfApplication3.Data.InvestmentAnalysisTools;

namespace WpfApplication3
{
    /// <summary>
    /// Tab_InvestmentAnalysisTools.xaml 的交互逻辑
    /// </summary>
    public partial class Tab_InvestmentAnalysisTools : UserControl
    {
        public Tab_InvestmentAnalysisTools()
        {
            InitializeComponent();
            // 右对齐风格  (全部右对齐，部分右对齐在XAML中设置)
            //Style styleRight = new Style(typeof(TextBlock));
            //Setter setRight = new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right); styleRight.Setters.Add(setRight);
            //foreach (DataGridColumn c in volatility_yield_grid.Columns)
            //{
            //    DataGridTextColumn tc = c as DataGridTextColumn;
            //    if (tc != null)
            //    {
            //        tc.ElementStyle = styleRight;
            //    }
            //}

            //收益率初始化
            selectDate_BondYieldTrend_yieldGrid.Text = GlobalParameters.lastTradingDate;
            selectDate_BondYieldTrend_yieldChart_start.Text = GlobalParameters.getPrevNBusinessDay(DateTime.Now.ToString("yyyy-MM-dd"),30);
            selectDate_BondYieldTrend_yieldChart_end.Text = GlobalParameters.lastTradingDate;
            BondYieldTrend_yieldGrid.ItemsSource = AnalysisData.getBondYieldTrend_yieldGridData(GlobalParameters.lastTradingDate).Tables[0].DefaultView;
            BindCombobox_selectAssetName_BondYieldTrend();
            ShowYieldChart(selectDate_BondYieldTrend_yieldChart_start.Text, selectDate_BondYieldTrend_yieldChart_end.Text, selectAssetName_BondYieldTrend.SelectedValue.ToString());
            //曲线形态和利差初始化(radio button 1)
            selectDate_BondSpread_Grid.Text = GlobalParameters.lastTradingDate;
            selectDate_BondSpread_Chart_start.Text = GlobalParameters.getPrevNBusinessDay(DateTime.Now.ToString("yyyy-MM-dd"), 30);
            selectDate_BondSpread_Chart_end.Text = GlobalParameters.lastTradingDate;
            BondSpread_termGrid.ItemsSource = AnalysisData.getBondSpread_GridData(GlobalParameters.lastTradingDate).Tables[0].DefaultView;
            BindCombobox_selectAssetName_termSpread_BondSpread();
            BondSpread_curveGrid.ItemsSource = AnalysisData.getBondSpread_GridData(GlobalParameters.lastTradingDate).Tables[1].DefaultView;
            ShowSpreadChart(1,selectDate_BondSpread_Chart_start.Text, selectDate_BondSpread_Chart_end.Text, selectAssetName_BondSpread.SelectedValue.ToString(), selectTerm_BondSpread.SelectedValue.ToString());
        }
        private void ShowYieldChart(string dataStart,string dataEnd,string assetName)
        {
            List<string> statList = AnalysisData.getBondYieldTrend_yieldChartData(dataStart, dataEnd, assetName);
            //画图函数
            BondYieldTrend_yieldChart.DisplayName = GlobalParameters.BondYieldTrend_assetType[selectAssetName_BondYieldTrend.SelectedValue.ToString()] + " 收益率走势";
            BondYieldTrend_yieldChart.DataSource = AnalysisData.invAnaToolsDataSet_BondYieldTrend_yieldChart.Tables[0];
            //BondYieldTrend_yieldChart.CrosshairLabelPattern = "";
            //样本统计
            yield_stat_count.Text = statList[0];
            yield_stat_max.Text = string.Format("{0:P2}", Convert.ToDecimal(statList[1]));
            yield_stat_min.Text = string.Format("{0:P2}", Convert.ToDecimal(statList[2]));
            yield_stat_avg.Text = string.Format("{0:P2}", Convert.ToDecimal(statList[3]));
            yield_stat_last.Text = string.Format("{0:P2}", Convert.ToDecimal(statList[4]));
            ChartRange.MaxValue = statList[1];
            ChartRange.MinValue = statList[2];
        }
        private void ShowSpreadChart(int analytictypecode,string dataStart, string dataEnd, string assetName, string term)
        {
            List<string> statList = AnalysisData.getBondSpread_ChartData(analytictypecode,dataStart, dataEnd, assetName, term);
            //画图函数
            BondSpread_curveChart.DisplayName = assetName + " " + term + " 利差走势";
            BondSpread_curveChart.DataSource = AnalysisData.invAnaToolsDataSet_BondSpread_Chart.Tables[0];
            //样本统计
            spread_stat_count.Text = statList[0].ToString();
            spread_stat_max.Text = statList[1].ToString();
            spread_stat_min.Text = statList[2].ToString();
            spread_stat_avg.Text = statList[3].ToString();
            spread_stat_last.Text = statList[4].ToString();
        }
        private void BindCombobox_selectAssetName_BondYieldTrend()
        {
            selectAssetName_BondYieldTrend.ItemsSource = GlobalParameters.BondYieldTrend_assetType;
            selectAssetName_BondYieldTrend.SelectedValuePath = "Key";
            selectAssetName_BondYieldTrend.DisplayMemberPath = "Value";
            selectAssetName_BondYieldTrend.SelectedIndex = 0;
        }

        private void BindCombobox_selectAssetName_termSpread_BondSpread()
        {
            selectAssetName_BondSpread.ItemsSource = GlobalParameters.BondSpread_termSpread_assetType;
            selectAssetName_BondSpread.SelectedValuePath = "Value";
            selectAssetName_BondSpread.DisplayMemberPath = "Value";
            selectAssetName_BondSpread.SelectedIndex = 0;

            selectTerm_BondSpread.ItemsSource = GlobalParameters.BondSpread_termSpread_term;
            selectTerm_BondSpread.SelectedValuePath = "Value";
            selectTerm_BondSpread.DisplayMemberPath = "Value";
            selectTerm_BondSpread.SelectedIndex = 0;
        }

        private void BindCombobox_selectAssetName_curveSpread_BondSpread()
        {
            selectAssetName_BondSpread.ItemsSource = GlobalParameters.BondSpread_curveSpread_assetType;
            selectAssetName_BondSpread.SelectedValuePath = "Value";
            selectAssetName_BondSpread.DisplayMemberPath = "Value";
            selectAssetName_BondSpread.SelectedIndex = 0;

            selectTerm_BondSpread.ItemsSource = GlobalParameters.BondSpread_curveSpread_term;
            selectTerm_BondSpread.SelectedValuePath = "Value";
            selectTerm_BondSpread.DisplayMemberPath = "Value";
            selectTerm_BondSpread.SelectedIndex = 0;
        }

        private void Button_Click_BondYieldTrend_yieldGrid(object sender, RoutedEventArgs e)
        {
            TimeSpan diff;
            if (selectDate_BondYieldTrend_yieldGrid.Text == "" )
            {
                selectDate_BondYieldTrend_yieldGrid.Text = GlobalParameters.lastTradingDate;
                BondYieldTrend_yieldGrid.ItemsSource = AnalysisData.getBondYieldTrend_yieldGridData(selectDate_BondYieldTrend_yieldGrid.Text).Tables[0].DefaultView;
            }
            else
            {
                diff = Convert.ToDateTime(selectDate_BondYieldTrend_yieldGrid.Text) - DateTime.Now;
                if (diff.Days > 0)
                {
                    MessageBox.Show("请输入的日期小于等于今日");
                    return;
                }
                else
                {
                    BondYieldTrend_yieldGrid.ItemsSource = AnalysisData.getBondYieldTrend_yieldGridData(selectDate_BondYieldTrend_yieldGrid.Text).Tables[0].DefaultView;
                }
            }

            //volatility_yield_grid.ItemsSource = AnalysisData.getVolYieldData(selectDate_BondVolYield.Text).Tables[0].DefaultView;
        }

        private void Button_Click_BondYieldTrend_yieldChart(object sender, RoutedEventArgs e)
        {
            TimeSpan diff;
            if (selectDate_BondYieldTrend_yieldChart_start.Text == "" || selectDate_BondYieldTrend_yieldChart_end.Text == "")
            {
                MessageBox.Show("请输入起止时间");
            }
            else
            {
                diff = Convert.ToDateTime(selectDate_BondYieldTrend_yieldChart_start.Text) - Convert.ToDateTime(selectDate_BondYieldTrend_yieldChart_end.Text);
                if (diff.Days > 0)
                {
                    MessageBox.Show("起始时间必须大于截止时间");
                    return;
                }
                else
                {
                    ShowYieldChart(selectDate_BondYieldTrend_yieldChart_start.Text, selectDate_BondYieldTrend_yieldChart_end.Text, selectAssetName_BondYieldTrend.SelectedValue.ToString());
                }
            }
        }

        private void Button_Click_BondSpread_Grid(object sender, RoutedEventArgs e)
        {
            TimeSpan diff;
            if (selectDate_BondSpread_Grid.Text == "")
            {
                selectDate_BondSpread_Grid.Text = GlobalParameters.lastTradingDate;
                BondSpread_termGrid.ItemsSource = AnalysisData.getBondSpread_GridData(selectDate_BondSpread_Grid.Text).Tables[0].DefaultView;
                BondSpread_curveGrid.ItemsSource = AnalysisData.getBondSpread_GridData(selectDate_BondSpread_Grid.Text).Tables[1].DefaultView;
            }
            else
            {
                diff = Convert.ToDateTime(selectDate_BondSpread_Grid.Text) - DateTime.Now;
                if (diff.Days > 0)
                {
                    MessageBox.Show("请输入的日期小于等于今日");
                    return;
                }
                else
                {
                    BondSpread_termGrid.ItemsSource = AnalysisData.getBondSpread_GridData(selectDate_BondSpread_Grid.Text).Tables[0].DefaultView;
                    BondSpread_curveGrid.ItemsSource = AnalysisData.getBondSpread_GridData(selectDate_BondSpread_Grid.Text).Tables[1].DefaultView;
                }
            }

        }
        private void RadioButton_termChart_Click(object sender, RoutedEventArgs e)
        {
            BindCombobox_selectAssetName_termSpread_BondSpread();
        }

        private void RadioButton_curveChart_Click(object sender, RoutedEventArgs e)
        {
            BindCombobox_selectAssetName_curveSpread_BondSpread();
        }

        private void Button_Click_BondSpread_Chart(object sender, RoutedEventArgs e)
        {
            TimeSpan diff;
            if (selectDate_BondSpread_Chart_start.Text == "" || selectDate_BondSpread_Chart_end.Text == "")
            {
                MessageBox.Show("请输入起止时间");
            }
            else
            {
                diff = Convert.ToDateTime(selectDate_BondSpread_Chart_start.Text) - Convert.ToDateTime(selectDate_BondSpread_Chart_end.Text);
                if (diff.Days > 0)
                {
                    MessageBox.Show("起始时间必须大于截止时间");
                    return;
                }
                else
                {
                    if(RadioButton_termChart.IsChecked==true)
                    {
                        ShowSpreadChart(1, selectDate_BondSpread_Chart_start.Text, selectDate_BondSpread_Chart_end.Text, selectAssetName_BondSpread.SelectedValue.ToString(), selectTerm_BondSpread.SelectedValue.ToString());
                    }
                    else if(RadioButton_curveChart.IsChecked == true)
                    {
                        ShowSpreadChart(2, selectDate_BondSpread_Chart_start.Text, selectDate_BondSpread_Chart_end.Text, selectAssetName_BondSpread.SelectedValue.ToString(), selectTerm_BondSpread.SelectedValue.ToString());
                    }
                }
            }
        }

        private void holdingReturn_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable("yieldcaculate");
            DateTime sdate = DateTime.Now.Date;
            double holdterm = 0.25;  //定义持有期限，按年表示，0.25为3个月，0.5为半年，以此类推，一般不超过1年；


            dt.Columns.Add("term", typeof(double));
            dt.Columns.Add("Yield", typeof(double));
            dt.Columns.Add("AdjYield(Par)", typeof(double));
            dt.Columns.Add("Duration", typeof(double));
            dt.Columns.Add("YTM_AfterHold", typeof(double));
            dt.Columns.Add("RollDown", typeof(double));
            dt.Columns.Add("Hold_Yield", typeof(double));
            dt.Columns.Add("+30Bp", typeof(double));
            dt.Columns.Add("+20Bp", typeof(double));
            dt.Columns.Add("+10Bp", typeof(double));
            dt.Columns.Add("-10Bp", typeof(double));
            dt.Columns.Add("-20Bp", typeof(double));
            dt.Columns.Add("-30Bp", typeof(double));

            //收益率曲线赋值
            YieldCurve yc = new YieldCurve();
            yc.AddPoint(0.5, 0.018);
            yc.AddPoint(1, 0.0235);
            yc.AddPoint(2, 0.024);
            yc.AddPoint(3, 0.0253);
            yc.AddPoint(4, 0.0259);
            yc.AddPoint(5, 0.0264);
            yc.AddPoint(6, 0.0285);
            yc.AddPoint(7, 0.0305);
            yc.AddPoint(8, 0.0312);
            yc.AddPoint(9, 0.0318);
            yc.AddPoint(10, 0.032);

            //yc.FlatChange(5);

            Array ycout = yc.GetCurveValue();   //GetCurveValue方法，将收益率曲线的数据输出到数组
            Bond mybond = new Bond();

            for (int i = 0; i < ycout.Length / 2; i++)
            {
                double ytm_afterhold = 0;
                double rolldown = 0;
                double return_static = 0, return_30 = 0, return_20 = 0, return_10 = 0, return_n30 = 0, return_n20 = 0, return_n10 = 0;
                double term = (double)(ycout.GetValue(i, 0));
                double yield = (double)ycout.GetValue(i, 1);
                mybond.CouponRate = yield;
                mybond.ParValue = 100;
                mybond.PayFrequency = 1;
                mybond.MatureDate = sdate.AddYears((int)Math.Round(term)).Date.ToString();
                double bnddur = mybond.GetBondDuration(sdate, yield);
                if (i > 0)
                {
                    double newterm = term - holdterm;
                    ytm_afterhold = yc.YiledOnCurve(newterm);
                }
                rolldown = (yield - ytm_afterhold) * bnddur * 100;    //骑乘收益
                return_static = rolldown + yield * 100 * holdterm;     //总持有期静态收益
                return_30 = return_static - 0.3 * bnddur;   //收益率上升30bp，以下类推
                return_20 = return_static - 0.2 * bnddur;
                return_10 = return_static - 0.1 * bnddur;
                return_n30 = return_static + 0.3 * bnddur;   //收益率下降30bp，以下类推
                return_n20 = return_static + 0.2 * bnddur;   //收益率下降30bp，以下类推
                return_n10 = return_static + 0.1 * bnddur;   //收益率下降30bp，以下类推

                DataRow dr = dt.NewRow();
                dr[0] = term;
                dr[1] = Math.Round(yield * 100, 3);
                dr[3] = Math.Round(bnddur, 2);
                dr[4] = Math.Round(ytm_afterhold * 100, 3);
                dr[5] = Math.Round(rolldown, 2);
                dr[6] = Math.Round(return_static, 2);
                dr[7] = Math.Round(return_30, 2);
                dr[8] = Math.Round(return_20, 2);
                dr[9] = Math.Round(return_10, 2);
                dr[10] = Math.Round(return_n10, 2);
                dr[11] = Math.Round(return_n20, 2);
                dr[12] = Math.Round(return_n30, 2);
                dt.Rows.Add(dr);
            }
            HoldingReturn_Grid.ItemsSource = dt.DefaultView;
        }
    }
}
