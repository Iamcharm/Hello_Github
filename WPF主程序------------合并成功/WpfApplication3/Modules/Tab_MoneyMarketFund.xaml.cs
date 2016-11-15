using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Linq;
using System.Drawing;
using WpfApplication3.Data.MoneyMarketFund;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using C1.WPF;
using System.Windows.Data;
using System.Collections;
using System.Reflection;

namespace WpfApplication3
{
    /// <summary>
    /// Tab_MoneyMarketFund.xaml 的交互逻辑
    /// </summary>
    public partial class Tab_MoneyMarketFund : UserControl
    {
        public static ObservableCollection<KeyIndex> keyIndexTests;
        public static ObservableCollection<KeyIndex> KeyIndexList;
        public static ObservableCollection<KeyIndex> currentYieldStats;
        public static List<Task> lists = new List<Task>();
        public static Task dateSeries = new Task();

        public Tab_MoneyMarketFund()
        {
            InitializeComponent();
            selectDate.Text = DateTime.Now.ToString("yyyy-MM-dd").ToString();
            BindCombobox();
            GlobalParameters.Date = DateTime.Now.ToString("yyyy-MM-dd").ToString();
            GlobalParameters.getTradingdates(GlobalParameters.Date);
            string portfolio;
            if ( null != portfolioBox.SelectedValue)
            {
                GlobalParameters.fundCode = portfolioBox.SelectedValue.ToString();  //选择基金代码
                portfolio = portfolioBox.SelectedValue.ToString();
            }

            //数据初始化
            ValuationTable.getValuationTable();                                 //获取估值表
            CKCDDetail.getCDDetail();
            CKCDDetail.getCKCDStats();
            SecurityDetail.getSecurityDetail();
            RepoDetail.getRepoDetail();

            //数据展示
            showIndexes();
            FirstKeyIndex();
            IndexTest();
            yieldStats();
            showCashFlow();
        }

        private void BindCombobox()
        {
            //Dictionary<string, string> portfolioes = new Dictionary<string, string>();
            //portfolioes = GlobalParameters.portfolio_1.Concat(GlobalParameters.portfolio_2).ToDictionary(k => k.Key, v => v.Value);
            portfolioBox.Text = "请选择";
            portfolioBox.ItemsSource = GlobalParameters.portfolio_1;
            portfolioBox.SelectedValuePath = "Key";
            portfolioBox.DisplayMemberPath = "Value";
            portfolioBox.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GlobalParameters.Date = selectDate.Text;
            TimeSpan diff;
            string portfolio = portfolioBox.SelectedValue.ToString();
            if (selectDate.Text == "" )
            {
                selectDate.Text = DateTime.Now.ToString("yyyy-MM-dd").ToString();
                GlobalParameters.Date = DateTime.Now.ToString("yyyy-MM-dd").ToString();
            }
            else
            {
                diff = Convert.ToDateTime(selectDate.Text) - DateTime.Now;
                if(diff.Days>0)
                {
                    MessageBox.Show("请输入的日期小于等于今日");
                    GlobalParameters.Date = DateTime.Now.ToString("yyyy-MM-dd").ToString();
                }
                else
                {
                    GlobalParameters.Date = Convert.ToDateTime(selectDate.Text).ToString("yyyy-MM-dd").ToString();
                }
            }
            //System.Windows.MessageBox.Show(date + "," + portfolio);
            GlobalParameters.getTradingdates(GlobalParameters.Date);
            GlobalParameters.fundCode = portfolioBox.SelectedValue.ToString();  //选择基金代码

            //数据初始化
            ValuationTable.getValuationTable();                                 //获取估值表
            CKCDDetail.getCDDetail();
            CKCDDetail.getCKCDStats();
            SecurityDetail.getSecurityDetail();
            RepoDetail.getRepoDetail();

            //数据展示
            showIndexes();
            FirstKeyIndex();
            IndexTest();
            yieldStats();
            showCashFlow();
            dgList_Loaded(sender, e);
            yieldStatsList_Loaded(sender, e);
            keyIndexGrid_Loaded(sender, e);
        }

        private void showIndexes()
        {
            bondDetail.ItemsSource = SecurityDetail.dt.DefaultView;
            var query_credit = from t in SecurityDetail.dt.AsEnumerable()
                        group t by new { t1 = t.Field<string>("主体评级")/*, t2 = t.Field<string>("债券类型")*/ } into m
                               orderby m.Key.t1 descending
                               select new
                        {
                            主体评级 = m.Key.t1,
                            //债券类型 = m.Key.t2,
                            持仓数量 = m.Sum(n => n.Field<decimal>("持仓数量")),
                            占净值比 = m.Sum(n => n.Field<decimal>("市值比净值")),
                            占债券比 = m.Sum(n => n.Field<decimal>("市值占比"))
                        };

            bondCreditDistribution.ItemsSource = query_credit.ToList();//按评级分布

            var query_enddate = from t in SecurityDetail.dt.AsEnumerable()
                        group t by new { t1 = t.Field<string>("期限分布") } into m
                                orderby m.Key.t1
                                select new
                        {
                            期限分布 = m.Key.t1,
                            持仓数量 = m.Sum(n => n.Field<decimal>("持仓数量")),
                            占净值比 = m.Sum(n => n.Field<decimal>("市值比净值")),
                            占债券比 = m.Sum(n => n.Field<decimal>("市值占比"))
                        }
                        ;

            bondEndDateDistribution.ItemsSource = query_enddate.ToList();//按到期日分布
            securityDetail_Chart.DataSource= query_enddate.ToList();

            depositReceiptDetail.ItemsSource = CKCDDetail.getCDDetail().DefaultView;
            dgList.Columns[0].IsReadOnly = Convert.ToBoolean("True");
            dgList.Columns[1].IsReadOnly = Convert.ToBoolean("False");
            if(CKCD_Button.IsChecked==true)
            {
                byMaturityDate.ItemsSource = CKCDDetail.getCDCKbyDate().Tables[2].DefaultView;
                byBank.ItemsSource = CKCDDetail.getCDCKbyBank().Tables[2].DefaultView;
                bar_amt_byDate.DataSource = CKCDDetail.getCDCKbyDate().Tables[2].DefaultView;
                line_int_byDate.DataSource = CKCDDetail.getCDCKbyDate().Tables[2].DefaultView;
                bar_amt_byBank.DataSource = CKCDDetail.getCDCKbyBank().Tables[2].DefaultView;
                line_ratio_byBank.DataSource = CKCDDetail.getCDCKbyBank().Tables[2].DefaultView;
            }
            else if (CK_Button.IsChecked == true)
            {
                byMaturityDate.ItemsSource = CKCDDetail.getCDCKbyDate().Tables[0].DefaultView;
                byBank.ItemsSource = CKCDDetail.getCDCKbyBank().Tables[0].DefaultView;
                bar_amt_byDate.DataSource = CKCDDetail.getCDCKbyDate().Tables[0].DefaultView;
                line_int_byDate.DataSource = CKCDDetail.getCDCKbyDate().Tables[0].DefaultView;
                bar_amt_byBank.DataSource = CKCDDetail.getCDCKbyBank().Tables[0].DefaultView;
                line_ratio_byBank.DataSource = CKCDDetail.getCDCKbyBank().Tables[0].DefaultView;
            }
            else if (CD_Button.IsChecked == true)
            {
                byMaturityDate.ItemsSource = CKCDDetail.getCDCKbyDate().Tables[1].DefaultView;
                byBank.ItemsSource = CKCDDetail.getCDCKbyBank().Tables[1].DefaultView;
                bar_amt_byDate.DataSource = CKCDDetail.getCDCKbyDate().Tables[1].DefaultView;
                line_int_byDate.DataSource = CKCDDetail.getCDCKbyDate().Tables[1].DefaultView;
                bar_amt_byBank.DataSource = CKCDDetail.getCDCKbyBank().Tables[1].DefaultView;
                line_ratio_byBank.DataSource = CKCDDetail.getCDCKbyBank().Tables[1].DefaultView;
            }
        }
        //////////////////////关键指标////////////////////////////////////
        private void FirstKeyIndex()
        {
            KeyIndexList = new ObservableCollection<KeyIndex>()
            {
                new KeyIndex("估值表日期", GlobalParameters.lastTradingDate),
                new KeyIndex("基金资产净值", string.Format("{0:F1}",ValuationTable.zbjs_NAV)),
                new KeyIndex("净申赎", string.Format("{0:F1}",ValuationTable.zbjs_yssgk - ValuationTable.zbjs_ysshk)),
                new KeyIndex("定存比例", string.Format("{0:F1}%",ValuationTable.zbjs_dcbl)),
                new KeyIndex("存单比例", string.Format("{0:F1}%",CKCDDetail.zbjs_cdratio)),
                new KeyIndex("债券比例", string.Format("{0:F1}%",SecurityDetail.zbjs_zqbl)),
                new KeyIndex("    短融比例", string.Format("{0:P1}",SecurityDetail.zbjs_cpbl)),
                new KeyIndex("    浮息债比例", string.Format("{0:P1}",SecurityDetail.zbjs_floatbndbl)),
                new KeyIndex("    其他利率债比例", string.Format("{0:P1}",SecurityDetail.zbjs_otherbndbl)),
                new KeyIndex("融券回购比例", string.Format("{0:F1}%",ValuationTable.zbjs_rqbl)),//有问题 100倍？
                new KeyIndex("现金头寸资产比例", string.Format("{0:F1}%",ValuationTable.zbjs_xjlzcbl)),
                new KeyIndex("融资回购比例(杠杆率)", string.Format("{0:F1}%",ValuationTable.zbjs_yhjrzhgbl)),
                new KeyIndex("银行间回购比例", string.Format("{0:F1}%",ValuationTable.zbjs_yhjhgbl)),
                new KeyIndex("剩余天期", ValuationTable.zbjs_sytq),
                new KeyIndex("万份收益A", ValuationTable.zbjs_wfsyA),
                new KeyIndex("万份收益B", ""),
                new KeyIndex("7日年化A", string.Format("{0}%",ValuationTable.zbjs_7dnhA)),
                new KeyIndex("7日年化B", ""),
                new KeyIndex("偏离度", string.Format("{0:F3}%",ValuationTable.zbjs_pld)),
                new KeyIndex("盈亏总额", string.Format("{0:F1}",ValuationTable.zbjs_plje)),
                new KeyIndex("    债券浮盈金额", string.Format("{0:F1}",SecurityDetail.zbjs_bndfy)),
                new KeyIndex("    债券浮亏金额", string.Format("{0:F1}",SecurityDetail.zbjs_bndfk)),
                new KeyIndex("    存单浮盈金额", string.Format("{0:F1}",CKCDDetail.zbjs_cdfy)),
                new KeyIndex("    存单浮亏金额", string.Format("{0:F1}",CKCDDetail.zbjs_cdfk)),
                new KeyIndex("万元损益影响万份收益", string.Format("{0:F5}",ValuationTable.zbjs_wysyyxwfsy)),
                new KeyIndex("万元损益影响7日年化", string.Format("{0:P3}",ValuationTable.zbjs_wysyyx7dnh)),
                new KeyIndex("1bp偏离度调整对应金额", string.Format("{0:F1}",ValuationTable.zbjs_1bppldtzdyje))
            };
            keyIndexGrid.ItemsSource = KeyIndexList;
        }
        //////////////////////指标试算////////////////////////////////////
        private void IndexTest()
        {
            string gzdate = GlobalParameters.lastTradingDate;                      //估值表时间 indextest（0，1）
            string tradingtestday = DateTime.Now.ToShortDateString().ToString();   //试算时间（当天） indextest（1，1）
            decimal lastNAV = ValuationTable.zbjs_NAV;                              //上日基金净值 indextest（2，1）
            decimal estimateSSamount = 0;                                           //当日基金预计申赎金额 indextest（3，1）input
            decimal netCPchg = 0;                                                   //当日短融净买卖金额 indextest（4，1）input
            decimal netFLOATbndchg = 0;                                             //当日浮动利率债净买卖金额 indextest（5，1）input
            decimal netREPOintbank = 0;                                             //当日银行间回购操作量 indextest（6，1）input
            decimal netRZrepo = 0;                                                  //融资回购净变动金额 indextest（7，1）input
            decimal ykchg = 0;                                                      //当日兑现盈亏金额 indextest（8，1）input
            decimal fee_A = 0;                                                      //A类份额费率 indextest（21，1）
            decimal fee_B = 0;                                                      //B类份额费率 indextest（22，1）

            decimal fundNAV = estimateSSamount + lastNAV;                           //当日预计基金净值 indextest（9，1）20，1
            decimal test_cpbl = (SecurityDetail.zbjs_cpamount + netCPchg) / fundNAV;                      //短融比例 indextest（10，1）
            decimal test_floatbndbl = (SecurityDetail.zbjs_floatbndamount + netFLOATbndchg) / fundNAV;    //浮息债比例 indextest（11，1）
            decimal test_ggl = (netRZrepo+RepoDetail.RZrepo_forzbss) / fundNAV;               //杠杆率 indextest（12，1）
            decimal test_yhjREPObl = (netREPOintbank+RepoDetail.YHJrepo_forzbss) / fundNAV;    //银行间回购比例 indextest（13，1）
            decimal test_sytq = ValuationTable.zbjs_sytq * lastNAV / fundNAV;//剩余天期 indextest（14，1）
            decimal test_xjlzc = ValuationTable.zbjs_xjlzcbl * lastNAV / fundNAV;//现金类资产比例 indextest（16，1）
            decimal test_wfsyA = ValuationTable.zbjs_wfsyA;                         //万份收益A indextest（17，1）
            decimal test_wfsyB = 0;                                                 //万份收益B indextest（18，1）
            string  test_7dnhA = string.Format("{0}%",ValuationTable.zbjs_7dnhA);                         //7日年化A indextest（19，1）
            decimal test_7dnhB = 0;                                                 //7日年化B indextest（20，1）

            decimal test_pld = (ValuationTable.zbjs_pld * lastNAV - ykchg) / fundNAV;//偏离度 indextest（15，1）=7日年化收益A：keyindex（17，1）*nav/fundNAV
            keyIndexTests = new ObservableCollection<KeyIndex>()
            {
                new KeyIndex("估值表日期", GlobalParameters.lastTradingDate),
                new KeyIndex("试算日期", GlobalParameters.Date),
                new KeyIndex("上日基金资产净值", string.Format("{0:F1}",ValuationTable.zbjs_NAV)),
                new KeyIndex("当日基金预计申赎金额",0),
                new KeyIndex("当日短融净买卖金额", 0),
                new KeyIndex("当日浮动利率债净买卖金额", 0),
                new KeyIndex("当日银行间回购操作量", 0),
                new KeyIndex("融资回购净变动金额", 0),
                new KeyIndex("当日兑现盈亏金额", 0),
                new KeyIndex("上日预计基金资产净值", string.Format("{0:F1}",estimateSSamount + lastNAV)),
                new KeyIndex("短融比例", string.Format("{0:P1}",test_cpbl)),
                new KeyIndex("浮息债比例", string.Format("{0:P1}",test_floatbndbl)),
                new KeyIndex("杠杆率", string.Format("{0:P1}",test_ggl)),
                new KeyIndex("银行间回购比例", string.Format("{0:P1}",test_yhjREPObl)),
                new KeyIndex("剩余天期", test_sytq),
                new KeyIndex("偏离度", string.Format("{0:F3}%",test_pld)),
                new KeyIndex("现金类资产比例", string.Format("{0:F1}%",test_xjlzc)),
                new KeyIndex("万份收益A", test_wfsyA),
                new KeyIndex("万份收益B", ""),
                new KeyIndex("7日年化A", test_7dnhA),
                new KeyIndex("7日年化B", ""),
                new KeyIndex("A类份额费率", fee_A),
                new KeyIndex("B类份额费率", fee_B),
            };
            dgList.ItemsSource = keyIndexTests;
        }
        //////////////////////当日资产收益统计/////////////////////////////
        private void yieldStats()
        {

            currentYieldStats = new ObservableCollection<KeyIndex>()
            {
                new KeyIndex("活期存款","0","0.72%",""),
                new KeyIndex("备付金",ValuationTable.zbjs_jsbfj.ToString(),"1.62%",""),
                new KeyIndex("定期存款",CKCDDetail.yieldStats_ck[0].ToString(),CKCDDetail.yieldStats_ck[1].ToString(),CKCDDetail.yieldStats_ck[2].ToString().ToString()),
                new KeyIndex("融券回购",RepoDetail.assetamount_rq.ToString(),decimal.Round(RepoDetail.assetyield_rq*100,2).ToString()+"%",RepoDetail.weightterm_rq.ToString()),
                new KeyIndex("融资回购",RepoDetail.assetamount_rz.ToString(),decimal.Round(RepoDetail.assetyield_rz*100,2).ToString()+"%",""),
                new KeyIndex("债券(上日余额)",SecurityDetail.assetamount.ToString(),decimal.Round(SecurityDetail.assetyield,2).ToString()+"%",SecurityDetail.weightterm.ToString()),
                new KeyIndex("存款(当日存出)","0","0","0"),
                new KeyIndex("债券(当日T+0买入)","0","0","0"),
                new KeyIndex("回购(当日T+0)","0","0","0"),
                new KeyIndex("总资产合计","0","0","0"),
                new KeyIndex("净资产合计","0","0","0")
            };
            for (int i = 0; i <= 8; i++)
            {
                if (i != 4)
                    currentYieldStats[9].IndexValue = (Convert.ToDecimal(currentYieldStats[9].IndexValue) + Convert.ToDecimal(currentYieldStats[i].IndexValue)).ToString();
            }
            yieldStatsList.ItemsSource = currentYieldStats;
        }

        #region 获取DataGrid某一行
        /// <summary>
        /// 获取DataGrid某一行
        /// </summary>
        /// <param name="datagrid"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        //public static DataGridRow GetDataGridRow(DataGrid datagrid, int rowIndex)
        //{
        //    DataGridRow row = (DataGridRow)datagrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);

        //    return row;
        //}
        #endregion

        #region 获取DataGrid某一单元格
        /// <summary>
        /// 得到DataGrid的一个单元格
        /// </summary>
        /// <param name="rowIndex">行索引</param>
        /// <param name="cellIndex">列索引</param>
        /// <param name="cellIndex">单元格颜色</param>
        /// <returns></returns>
        private DataGridCell GetDataGridCell(DataGrid dglistname, int rowIndex, int cellIndex, System.Windows.Media.Color colorType)
        {
            DataGridRow row = (DataGridRow)dglistname.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            dglistname.UpdateLayout();
            row = (DataGridRow)dglistname.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            if (row == null)
            {
                return null;
            }
            DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
            DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(cellIndex);
            dglistname.ScrollIntoView(row, dglistname.Columns[cellIndex]);
            cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(cellIndex);
            //cell.IsEnabled = false;
            cell.Background = new SolidColorBrush(colorType);
            return cell;
        }
        private DataGridCell GetDataGridCell(DataGrid dglistname, int rowIndex, int cellIndex)
        {
            DataGridRow row = (DataGridRow)dglistname.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            dglistname.UpdateLayout();
            row = (DataGridRow)dglistname.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            if (row == null)
            {
                return null;
            }
            DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
            DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(cellIndex);
            dglistname.ScrollIntoView(row, dglistname.Columns[cellIndex]);
            cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(cellIndex);
            //cell.IsEnabled = false;
            return cell;
        }


        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T childContent = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                childContent = v as T;
                if (childContent == null)
                {
                    childContent = GetVisualChild<T>(v);
                }
                if (childContent != null)
                {
                    break;
                }
            }
            return childContent;
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                                                               //
        //                                                                                                                               //
        //                                                                                                                               //
        //                                                                                                                               //
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void dgList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //keyIndexTests[e.Row.GetIndex()].IndexValue = ((e.EditingElement as TextBox).Text == "" || keyIndexTests[e.Row.GetIndex()].IndexValue=="") ? "0" : (e.EditingElement as TextBox).Text;
            if ((e.EditingElement as TextBox).Text == "")
            {
                (e.EditingElement as TextBox).Text = "0";
            }
            keyIndexTests[e.Row.GetIndex()].IndexValue = (e.EditingElement as TextBox).Text;
            //判断输入是否为数字
            if (Regex.IsMatch(keyIndexTests[e.Row.GetIndex()].IndexValue, @"^[+-]?\d*[.]?\d*$"))
            {
                //keyIndexTests[e.Row.GetIndex()].IndexValue = newValue;
                //当日预计基金净值
                keyIndexTests[9].IndexValue = (Convert.ToDecimal(keyIndexTests[2].IndexValue) + Convert.ToDecimal(keyIndexTests[3].IndexValue)).ToString();
                //短融比例
                keyIndexTests[10].IndexValue = decimal.Round(((SecurityDetail.zbjs_cpamount + Convert.ToDecimal(keyIndexTests[4].IndexValue)) / Convert.ToDecimal(keyIndexTests[9].IndexValue)) * 100, 1).ToString() + "%";
                //浮息债比例
                keyIndexTests[11].IndexValue = decimal.Round(((SecurityDetail.zbjs_floatbndamount + Convert.ToDecimal(keyIndexTests[5].IndexValue)) / Convert.ToDecimal(keyIndexTests[9].IndexValue)) * 100, 1).ToString() + "%";
                //杠杆率                     
                keyIndexTests[12].IndexValue = decimal.Round((((Convert.ToDecimal(keyIndexTests[7].IndexValue)) + RepoDetail.RZrepo_forzbss) / Convert.ToDecimal(keyIndexTests[9].IndexValue)) * 100, 1).ToString() + "%";
                //银行间回购比例              
                keyIndexTests[13].IndexValue = decimal.Round((((Convert.ToDecimal(keyIndexTests[6].IndexValue)) + RepoDetail.YHJrepo_forzbss) / Convert.ToDecimal(keyIndexTests[9].IndexValue)) * 100, 1).ToString() + "%";
                //剩余天期                    
                keyIndexTests[14].IndexValue = decimal.Round((ValuationTable.zbjs_sytq * Convert.ToDecimal(keyIndexTests[2].IndexValue) / Convert.ToDecimal(keyIndexTests[9].IndexValue)), 1).ToString();
                //偏离度                     
                keyIndexTests[15].IndexValue = decimal.Round(((ValuationTable.zbjs_plje - Convert.ToDecimal(keyIndexTests[8].IndexValue)) / Convert.ToDecimal(keyIndexTests[9].IndexValue) * 100), 3).ToString() + "%";
                //现金类资产                  
                keyIndexTests[16].IndexValue = decimal.Round((ValuationTable.zbjs_xjlzcbl * Convert.ToDecimal(keyIndexTests[2].IndexValue) / Convert.ToDecimal(keyIndexTests[9].IndexValue) ), 1).ToString() + "%";
                ////万份收益A                   
                //keyIndexTests[17].IndexValue =
                ////7日年化收益率A              
                //keyIndexTests[19].IndexValue =
                //KeyIndex.KeyIndexTest[4].IndexValue = KeyIndex.KeyIndexTest[3].IndexValue + KeyIndex.KeyIndexTest[2].IndexValue;
            }
            else
            {
                (e.EditingElement as TextBox).Text = "0";
                MessageBox.Show("请输入数字");
            }
        }


        #region 
        /// <summary>
        /// 判断DataGrid某一单元格是否满足条件并染色
        /// </summary>
        /// <param name="rowIndex">行索引</param>
        /// <param name="judgeCondition">大于这个判断条件</param>
        /// <param name="colorType">单元格颜色</param>
        /// <returns></returns>
        private bool setGridCellColor(DataGrid dglistname, int rowIndex, string judgeCondition, System.Windows.Media.Color colorType)
        {
            int i = rowIndex;
            DataRowView drv = dglistname.Items[i] as DataRowView;
            DataGridRow row = (DataGridRow)dglistname.ItemContainerGenerator.ContainerFromIndex(i);
            if (row == null)
            {
                return false;
            }
            KeyIndex valueList = dglistname.Items[i] as KeyIndex;
            if (valueList != null)
            {
                string value;
                value = valueList.IndexValue;
                float f_value = float.Parse(value.Substring(0, value.Length - 1));
                float f_judge = float.Parse(judgeCondition.Substring(0, judgeCondition.Length - 1));
                if (f_value > f_judge )
                {
                    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                    DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(1);
                    cell.Background = new SolidColorBrush(Colors.DarkGray);
                }
            }
            return true;
        }
        #endregion


        private void dgList_Loaded(object sender, RoutedEventArgs e)
        {
            GetDataGridCell(dgList, 0, 1, Colors.Red);

            for (int i = 0; i < dgList.Items.Count; i++)
            {
                DataRowView drv = dgList.Items[i] as DataRowView;
                DataGridRow row = (DataGridRow)dgList.ItemContainerGenerator.ContainerFromIndex(i);
                if (row == null)
                {
                    return;
                }
                if ((i >= 3 && i <= 8) || ((i >= 21 && i <= 22)))
                {
                    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                    DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(1);
                    cell.Background = new SolidColorBrush(Colors.Yellow);
                    //cell.IsEnabled = false;
                }
                else
                {
                    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                    DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(1);
                    //cell.Background = new SolidColorBrush(Colors.Yellow);
                    cell.IsEnabled = false;
                }
            }
        }


        private void yieldStatsList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            if ((e.EditingElement as TextBox).Text == "")
            {
                (e.EditingElement as TextBox).Text = "0";
            }
            currentYieldStats[e.Row.GetIndex()].IndexValue = (e.EditingElement as TextBox).Text;
            //判断输入是否为数字
            if (Regex.IsMatch(currentYieldStats[e.Row.GetIndex()].IndexValue, @"^[+-]?\d*[.]?\d*$"))
            {

            }
            else
            {
                (e.EditingElement as TextBox).Text = "0";
                MessageBox.Show("请输入数字");
            }
        }

        private void yieldStatsList_Loaded(object sender, RoutedEventArgs e)
        {
            GetDataGridCell(yieldStatsList, 0, 1, Colors.Yellow);
            for (int i = 0; i <= 10; i++)
            {
                for (int j = 1; j <= 3; j++)
                {
                    if ((i == 0 && j == 1) || (i >= 6 && i <= 8))
                        GetDataGridCell(yieldStatsList, i, j, Colors.Yellow);
                    else
                    {
                        DataGridCell cell = GetDataGridCell(yieldStatsList, i, j);
                        if (cell != null)
                            cell.IsEnabled = false;
                    }
                }
            }
            yieldStatsList.Columns[0].IsReadOnly = Convert.ToBoolean("True");
        }

        private void CKCD_Button_Click(object sender, RoutedEventArgs e)
        {
            byMaturityDate.ItemsSource = CKCDDetail.getCDCKbyDate().Tables[2].DefaultView;
            byBank.ItemsSource = CKCDDetail.getCDCKbyBank().Tables[2].DefaultView;
            bar_amt_byDate.DataSource = CKCDDetail.getCDCKbyDate().Tables[2].DefaultView;
            line_int_byDate.DataSource = CKCDDetail.getCDCKbyDate().Tables[2].DefaultView;
            bar_amt_byBank.DataSource = CKCDDetail.getCDCKbyBank().Tables[2].DefaultView;
            line_ratio_byBank.DataSource = CKCDDetail.getCDCKbyBank().Tables[2].DefaultView;
        }

        private void CK_Button_Click(object sender, RoutedEventArgs e)
        {
            byMaturityDate.ItemsSource = CKCDDetail.getCDCKbyDate().Tables[0].DefaultView;
            byBank.ItemsSource = CKCDDetail.getCDCKbyBank().Tables[0].DefaultView;
            bar_amt_byDate.DataSource = CKCDDetail.getCDCKbyDate().Tables[0].DefaultView;
            line_int_byDate.DataSource = CKCDDetail.getCDCKbyDate().Tables[0].DefaultView;
            bar_amt_byBank.DataSource = CKCDDetail.getCDCKbyBank().Tables[0].DefaultView;
            line_ratio_byBank.DataSource = CKCDDetail.getCDCKbyBank().Tables[0].DefaultView;
        }

        private void CD_Button_Click(object sender, RoutedEventArgs e)
        {
            byMaturityDate.ItemsSource = CKCDDetail.getCDCKbyDate().Tables[1].DefaultView;
            byBank.ItemsSource = CKCDDetail.getCDCKbyBank().Tables[1].DefaultView;
            bar_amt_byDate.DataSource = CKCDDetail.getCDCKbyDate().Tables[1].DefaultView;
            line_int_byDate.DataSource = CKCDDetail.getCDCKbyDate().Tables[1].DefaultView;
            bar_amt_byBank.DataSource = CKCDDetail.getCDCKbyBank().Tables[1].DefaultView;
            line_ratio_byBank.DataSource = CKCDDetail.getCDCKbyBank().Tables[1].DefaultView;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                                                               //
        //                                                                                                                               //
        //                                                                                                                               //
        //                                                                                                                               //
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //////////////////////头寸/////////////////////////////
        #region 头寸数据
        private void showCashFlow()
        {
            #region ** Create tasks
            //var dateSeries = new Task() { WBS = "", Name = "" };
            var task0 = new Task() { WBS = "", Name = "" };
            var task1 = new Task() { WBS = "1", Name = "日初在账现金" };
            var task2 = new Task() { WBS = "2", Name = "净申购赎回款" };
            var task3 = new Task() { WBS = "3", Name = "流入合计" };
            var task3_0 = new Task() { WBS = "3.0", Name = "存款到期", ParentTask = task3 };
            var task3_1 = new Task() { WBS = "3.1", Name = "存单到期", ParentTask = task3 };
            var task3_2 = new Task() { WBS = "3.2", Name = "交易所回购到期流入", ParentTask = task3 };
            var task3_3 = new Task() { WBS = "3.3", Name = "交易所回购融资流入", ParentTask = task3 };
            var task3_4 = new Task() { WBS = "3.4", Name = "银行间回购到期流入", ParentTask = task3 };
            var task3_5 = new Task() { WBS = "3.5", Name = "银行间回购融资流入", ParentTask = task3 };
            var task3_6 = new Task() { WBS = "3.6", Name = "银行间现券卖出", ParentTask = task3 };
            var task3_7 = new Task() { WBS = "3.7", Name = "债券到期兑付", ParentTask = task3 };
            var task3_8 = new Task() { WBS = "3.8", Name = "其他当日应收", ParentTask = task3 };
            task3.SubTasks = new ObservableCollection<Task> { task3_0, task3_1, task3_2, task3_3, task3_4, task3_5, task3_6, task3_7, task3_8 };
            var task4 = new Task() { WBS = "4", Name = "流出合计" };
            var task4_0 = new Task() { WBS = "4.0", Name = "交易所回购融券流出", ParentTask = task4 };
            var task4_1 = new Task() { WBS = "4.1", Name = "交易所回购到期流出", ParentTask = task4 };
            var task4_2 = new Task() { WBS = "4.2", Name = "银行间回购融券流出", ParentTask = task4 };
            var task4_3 = new Task() { WBS = "4.3", Name = "银行间回购到期流出", ParentTask = task4 };
            var task4_4 = new Task() { WBS = "4.4", Name = "银行间现券买入", ParentTask = task4 };
            var task4_5 = new Task() { WBS = "4.5", Name = "其他当日应付", ParentTask = task4 };
            task4.SubTasks = new ObservableCollection<Task> { task4_0, task4_1, task4_2, task4_3, task4_4, task4_5 };
            var task5 = new Task() { WBS = "5", Name = "系统冻结总额" };
            var task5_0 = new Task() { WBS = "5.0", Name = "T+1日到期银行间回购", ParentTask = task5 };
            var task5_1 = new Task() { WBS = "5.1", Name = "T+1日到期交易所回购", ParentTask = task5 };
            var task5_2 = new Task() { WBS = "5.2", Name = "其他当日待付预冻结项目", ParentTask = task5 };
            var task5_3 = new Task() { WBS = "5.3", Name = "当日待付项目", ParentTask = task5 };
            task5.SubTasks = new ObservableCollection<Task> { task5_0, task5_1, task5_2, task5_3 };
            grid.CanUserEditRows = true;
            var task6 = new Task() { WBS = "6", Name = "日初T+0可用头寸" };
            var task7 = new Task() { WBS = "7", Name = "日初T+1可用头寸" };
            var task8 = new Task() { WBS = "8", Name = "日初场外在途资金" };
            var task9 = new Task() { WBS = "9", Name = "日初当日场外待付资金" };
            var task10 = new Task() { WBS = "10", Name = "日间缺口" };
            var task10_0 = new Task() { WBS = "10.0", Name = "12:00前缺口", ParentTask = task10 };
            var task10_1 = new Task() { WBS = "10.1", Name = "17:00前缺口", ParentTask = task10 };
            task10.SubTasks = new ObservableCollection<Task> { task10_0, task10_1 };
            var task11 = new Task() { WBS = "11", Name = "当日操作" };
            var task11_0 = new Task() { WBS = "11.0", Name = "存款：存款存出", ParentTask = task11 };
            var task11_1 = new Task() { WBS = "11.1", Name = "回购：交易所回购净额", ParentTask = task11 };
            var task11_2 = new Task() { WBS = "11.2", Name = "回购：银行间T+0回购净额", ParentTask = task11 };
            var task11_3 = new Task() { WBS = "11.3", Name = "回购：银行间T+1回购净额", ParentTask = task11 };
            var task11_4 = new Task() { WBS = "11.4", Name = "现券：交易所现券买卖净额", ParentTask = task11 };
            var task11_5 = new Task() { WBS = "11.5", Name = "现券：银行间T+0现券买卖净额", ParentTask = task11 };
            var task11_6 = new Task() { WBS = "11.6", Name = "现券：银行间T+1现券买卖净额", ParentTask = task11 };
            var task11_7 = new Task() { WBS = "11.7", Name = "当日交易资金结算净额", ParentTask = task11 };
            task11.SubTasks = new ObservableCollection<Task> { task11_0, task11_1, task11_2, task11_3, task11_4, task11_5, task11_6, task11_7 };
            var task12 = new Task() { WBS = "12", Name = "调整项" };
            var task12_0 = new Task() { WBS = "12.0", Name = "存款到期", ParentTask = task12 };
            var task12_1 = new Task() { WBS = "12.1", Name = "回购到期流入", ParentTask = task12 };
            var task12_2 = new Task() { WBS = "12.2", Name = "回购偿还流出", ParentTask = task12 };
            var task12_3 = new Task() { WBS = "12.3", Name = "其他调整", ParentTask = task12 };
            task12.SubTasks = new ObservableCollection<Task> { task12_0, task12_1, task12_2, task12_3 };
            var task13 = new Task() { WBS = "13", Name = "日终净头寸（含当日计划交易）" };
            var task14 = new Task() { WBS = "14", Name = "日终净头寸（不含当日计划交易）" };
            var task15 = new Task() { WBS = "14", Name = "两天合计" };
            var tasks = new ObservableCollection<Task> { task0, task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12, task13, task14, task15 };
            #endregion

            #region ** 定义表头：日期序列
            for (int i = 0; i < 13; i++)
                dateSeries.day[i] = GlobalParameters.TradingDateSeries[i];
            dateSeries.setDayValue();
            #endregion

            #region ** task13：日终净头寸（含当日计划交易）<---上日估值表在账活期存款

            task13.day[0] = ValuationTable.zbjs_hqck;
            task13.setDayValue();

            #endregion

            #region ** task0：星期
            var weekdays = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            for (int i = 0; i < 13; i++)
                task0.day[i] = weekdays[(int)DateTime.Parse(GlobalParameters.TradingDateSeries[i]).DayOfWeek];
            task0.setDayValue();
            #endregion

            #region ** task1：日初在账金额 //计算

            task1.day[1] = task13.day[0];
            task1.setDayValue();

            #endregion

            #region ** task2：净申购赎回款
            task2.day[1] = ValuationTable.zbjs_netss_before1;
            task2.day[2] = ValuationTable.zbjs_netss;
            task2.setDayValue();
            #endregion

            #region ** task3：流入合计 //计算

            #endregion

            #region ** task3.0：存款到期

            grid_LoadRowValue(task3_0, CKCDDetail.ckdq);

            #endregion

            #region ** task3.1：存单到期

            grid_LoadRowValue(task3_1, CKCDDetail.cddq);

            #endregion

            #region ** task3.2：交易所回购到期流入

            grid_LoadRowValue(task3_2, RepoDetail.jys_HG_dq_in2);

            #endregion

            #region ** task3.3：交易所回购融资流入

            grid_LoadRowValue(task3_3, RepoDetail.jys_HG_rz_in3);

            #endregion

            #region ** task3.4：银行间回购到期流入

            grid_LoadRowValue(task3_4, RepoDetail.yhj_HG_dq_in4);

            #endregion

            #region ** task3.5：银行间回购融资流入

            grid_LoadRowValue(task3_5, RepoDetail.yhj_HG_rz_in5);
            //T+1
            //for (int i = 0; i < 13; i++)
            //    task3_5.day[i] = RepoDetail.yhj_HG_rz_in5[i];
            //task3_5.setDayValue();

            #endregion

            #region ** task3.6：银行间现券卖出

            grid_LoadRowValue(task3_6, SecurityDetail.zq_sellout_out6);

            #endregion

            #region ** task3.7：债券到期兑付（测试，待写入，*休息日到期后移*）

            grid_LoadRowValue(task3_7, SecurityDetail.zq_dqdf_out7);
            
            //task3_7.day[6] = 5173.0;
            //task3_7.day[7] = 3115.5;
            //task3_7.day[8] = 10343.0;
            //task3_7.day[11] = 3102.3;
            //task3_7.setDayValue();

            #endregion

            #region ** task4：流出合计（见后文） //计算

            #endregion

            #region ** task4.0：交易所回购融券流出

            grid_LoadRowValue(task4_0, RepoDetail.jys_HG_rq_out0);

            #endregion

            #region ** task4.1：交易所回购到期流出

            grid_LoadRowValue(task4_1, RepoDetail.jys_HG_dq_out1);

            #endregion

            #region ** task4.2：银行间回购融券流出

            grid_LoadRowValue(task4_2, RepoDetail.yhj_HG_rq_out2);

            #endregion

            #region ** task4.3：银行间回购到期流出

            grid_LoadRowValue(task4_3, RepoDetail.yhj_HG_dq_out3);

            #endregion

            #region ** task4.4：银行间现券买入

            grid_LoadRowValue(task4_4, SecurityDetail.zq_buyin_out4);

            #endregion

            #region 其他头寸指标计算
            //计算
            //流入合计
            for (int i = 1; i <= 12; i++)
            {
                task1.day[i] = task13.day[i-1];

                task3.day[i] = (
                                  (task1.day[i] == null ? 0 : Convert.ToDecimal(task1.day[i].ToString()))
                                + (task2.day[i] == null ? 0 : Convert.ToDecimal(task2.day[i].ToString()))
                                + (task3_0.day[i] == null ? 0 : Convert.ToDecimal(task3_0.day[i].ToString()))
                                + (task3_1.day[i] == null ? 0 : Convert.ToDecimal(task3_1.day[i].ToString()))
                                + (task3_2.day[i] == null ? 0 : Convert.ToDecimal(task3_2.day[i].ToString()))
                                + (task3_3.day[i] == null ? 0 : Convert.ToDecimal(task3_3.day[i].ToString()))
                                + (task3_4.day[i] == null ? 0 : Convert.ToDecimal(task3_4.day[i].ToString()))
                                + (task3_5.day[i] == null ? 0 : Convert.ToDecimal(task3_5.day[i].ToString()))
                                + (task3_6.day[i] == null ? 0 : Convert.ToDecimal(task3_6.day[i].ToString()))
                                + (task3_7.day[i] == null ? 0 : Convert.ToDecimal(task3_7.day[i].ToString()))
                                + (task3_8.day[i] == null ? 0 : Convert.ToDecimal(task3_8.day[i].ToString()))
                                ).ToString();
                //流出合计
                task4.day[i] = (
                                (task4_0.day[i] == null ? 0 : Convert.ToDecimal(task4_0.day[i].ToString()))
                                + (task4_1.day[i] == null ? 0 : Convert.ToDecimal(task4_1.day[i].ToString()))
                                + (task4_2.day[i] == null ? 0 : Convert.ToDecimal(task4_2.day[i].ToString()))
                                + (task4_3.day[i] == null ? 0 : Convert.ToDecimal(task4_3.day[i].ToString()))
                                + (task4_4.day[i] == null ? 0 : Convert.ToDecimal(task4_4.day[i].ToString()))
                                + (task4_5.day[i] == null ? 0 : Convert.ToDecimal(task4_5.day[i].ToString()))
                                ).ToString();

                //日初O32预冻结
                task5_0.day[i] = (
                                (task4_3.day[i+1] == null ? 0 : Convert.ToDecimal(task4_3.day[i+1].ToString()))
                                ).ToString();

                task5_1.day[i] = (
                                (task4_1.day[i+1] == null ? 0 : Convert.ToDecimal(task4_1.day[i+1].ToString()))
                                ).ToString();

                task5_3.day[i] = (
                                  (task4_0.day[i] == null ? 0 : Convert.ToDecimal(task4_0.day[i].ToString()))
                                + (task4_1.day[i] == null ? 0 : Convert.ToDecimal(task4_1.day[i].ToString()))
                                + (task4_2.day[i] == null ? 0 : Convert.ToDecimal(task4_2.day[i].ToString()))
                                + (task4_3.day[i] == null ? 0 : Convert.ToDecimal(task4_3.day[i].ToString()))
                                + (task4_4.day[i] == null ? 0 : Convert.ToDecimal(task4_4.day[i].ToString()))
                                ).ToString();

                task5.day[i] = (
                                  (task5_0.day[i] == null ? 0 : Convert.ToDecimal(task5_0.day[i].ToString()))
                                + (task5_1.day[i] == null ? 0 : Convert.ToDecimal(task5_1.day[i].ToString()))
                                + (task5_2.day[i] == null ? 0 : Convert.ToDecimal(task5_2.day[i].ToString()))
                                + (task5_3.day[i] == null ? 0 : Convert.ToDecimal(task5_3.day[i].ToString()))
                                ).ToString();

                //日初T+0可用头寸
                task6.day[i] = (
                                  (task1.day[i] == null ? 0 : Convert.ToDecimal(task1.day[i].ToString()))
                                + (task2.day[i] == null ? 0 : Convert.ToDecimal(task2.day[i].ToString()))
                                + (task3_2.day[i] == null ? 0 : Convert.ToDecimal(task3_2.day[i].ToString()))
                                + (task3_3.day[i] == null ? 0 : Convert.ToDecimal(task3_3.day[i].ToString()))
                                - (task5.day[i] == null ? 0 : Convert.ToDecimal(task5.day[i].ToString()))
                                ).ToString();

                //日初T+1可用头寸
                task7.day[i] = (
                                  (task3_2.day[i+1] == null ? 0 : Convert.ToDecimal(task3_2.day[i+1].ToString()))
                                + (task3_3.day[i+1] == null ? 0 : Convert.ToDecimal(task3_3.day[i+1].ToString()))
                                + (task5_1.day[i] == null ? 0 : Convert.ToDecimal(task5_1.day[i].ToString()))
                                ).ToString();

                //日初场外在途资金
                task8.day[i] = (
                                +(task3_0.day[i] == null ? 0 : Convert.ToDecimal(task3_0.day[i].ToString()))
                                + (task3_4.day[i] == null ? 0 : Convert.ToDecimal(task3_4.day[i].ToString()))
                                + (task3_5.day[i] == null ? 0 : Convert.ToDecimal(task3_5.day[i].ToString()))
                                + (task3_6.day[i] == null ? 0 : Convert.ToDecimal(task3_6.day[i].ToString()))
                                + (task3_7.day[i] == null ? 0 : Convert.ToDecimal(task3_7.day[i].ToString()))
                                + (task3_8.day[i] == null ? 0 : Convert.ToDecimal(task3_8.day[i].ToString()))
                                ).ToString();

                //日初当日场外待付资金
                task9.day[i] = (
                                +(task4_2.day[i] == null ? 0 : Convert.ToDecimal(task4_2.day[i].ToString()))
                                + (task4_3.day[i] == null ? 0 : Convert.ToDecimal(task4_3.day[i].ToString()))
                                + (task4_4.day[i] == null ? 0 : Convert.ToDecimal(task4_4.day[i].ToString()))
                                + (task4_5.day[i] == null ? 0 : Convert.ToDecimal(task4_5.day[i].ToString()))
                                ).ToString();

                //12:00前缺口
                task10_0.day[i] = (
                                +(task1.day[i] == null ? 0 : Convert.ToDecimal(task1.day[i].ToString()))
                                + (task2.day[i] == null ? 0 : Convert.ToDecimal(task2.day[i].ToString()))
                                + (task3_2.day[i] == null ? 0 : Convert.ToDecimal(task3_2.day[i].ToString()))
                                + (task3_3.day[i] == null ? 0 : Convert.ToDecimal(task3_3.day[i].ToString()))
                                - (task4_0.day[i] == null ? 0 : Convert.ToDecimal(task4_0.day[i].ToString()))
                                - (task4_1.day[i] == null ? 0 : Convert.ToDecimal(task4_1.day[i].ToString()))
                                ).ToString();

                //17:00前缺口
                task10_1.day[i] = (
                                +(task3.day[i] == null ? 0 : Convert.ToDecimal(task3.day[i].ToString()))
                                - (task4.day[i] == null ? 0 : Convert.ToDecimal(task4.day[i].ToString()))
                                ).ToString();

                //当日交易资金结算净额
                task11_7.day[i] = (
                                +(task11_2.day[i] == null ? 0 : Convert.ToDecimal(task11_2.day[i].ToString()))
                                + (task11_5.day[i] == null ? 0 : Convert.ToDecimal(task11_5.day[i].ToString()))
                                + (task11_1.day[i-1] == null ? 0 : Convert.ToDecimal(task11_1.day[i-1].ToString()))
                                + (task11_3.day[i-1] == null ? 0 : Convert.ToDecimal(task11_3.day[i-1].ToString()))
                                + (task11_4.day[i-1] == null ? 0 : Convert.ToDecimal(task11_4.day[i-1].ToString()))
                                + (task11_6.day[i-1] == null ? 0 : Convert.ToDecimal(task11_6.day[i-1].ToString()))
                                - (task11_0.day[i] == null ? 0 : Convert.ToDecimal(task11_0.day[i].ToString()))
                                ).ToString();

                //日终净头寸（含当日计划交易）
                task13.day[i] = (
                                +(task10_1.day[i] == null ? 0 : Convert.ToDecimal(task10_1.day[i].ToString()))
                                + (task11_7.day[i] == null ? 0 : Convert.ToDecimal(task11_7.day[i].ToString()))
                                + (task12_0.day[i] == null ? 0 : Convert.ToDecimal(task12_0.day[i].ToString()))
                                + (task12_1.day[i] == null ? 0 : Convert.ToDecimal(task12_1.day[i].ToString()))
                                + (task12_2.day[i] == null ? 0 : Convert.ToDecimal(task12_2.day[i].ToString()))
                                + (task12_3.day[i] == null ? 0 : Convert.ToDecimal(task12_3.day[i].ToString()))
                                ).ToString();

            }
            //日终净头寸（不含当日计划交易）
            task14.day[1] = task13.day[1];
            //最后单独算
            for (int i = 2; i <= 12; i++)
            {
                task14.day[i] = (
                                +(task10_1.day[i] == null ? 0 : Convert.ToDecimal(task10_1.day[i].ToString()))
                                - (task1.day[i] == null ? 0 : Convert.ToDecimal(task1.day[i].ToString()))
                                + (task2.day[i] == null ? 0 : Convert.ToDecimal(task2.day[i].ToString()))
                                + (task12_0.day[i] == null ? 0 : Convert.ToDecimal(task12_0.day[i].ToString()))
                                + (task12_1.day[i] == null ? 0 : Convert.ToDecimal(task12_1.day[i].ToString()))
                                + (task12_2.day[i] == null ? 0 : Convert.ToDecimal(task12_2.day[i].ToString()))
                                + (task12_3.day[i] == null ? 0 : Convert.ToDecimal(task12_3.day[i].ToString()))
                                ).ToString();

                //两天合计
                //最后单独算
                task15.day[i] = (
                                +(task14.day[i-1] == null ? 0 : Convert.ToDecimal(task14.day[i-1].ToString()))
                                + (task14.day[i] == null ? 0 : Convert.ToDecimal(task14.day[i].ToString()))
                                ).ToString();
            }
            #endregion 其他头寸指标计算

            IEnumerable<Task> mytasks = Task.GetAllTasks(tasks);
            lists = new List<Task>();
            foreach (var item in mytasks)
            {
                item.IsExpanded = false;
                lists.Add(item);
                item.setDayValue();
            }
            grid.ItemsSource = lists;
        }

        private void grid_LoadRowValue(Task cashflowItem, string[] dataSourse)
        {
            for (int i = 0; i < 13; i++)
                cashflowItem.day[i] = dataSourse[i];
            cashflowItem.setDayValue();
        }

        #endregion 头寸数据
        ///////////////////////////////////////////////////
        #region 头寸响应函数
        private void grid_LoadingRow(object sender, C1.WPF.DataGrid.DataGridRowEventArgs e)
        {
            var task = e.Row.DataItem as Task;
            task.PropertyChanged += (s, a) =>
            {
                if (a.PropertyName == "IsVisible")
                {
                    var subTask = s as Task;
                    var row = grid.Rows[subTask];
                    if (row != null && subTask != null)
                    {
                        UpdateRowVisibility(row, subTask);
                    }
                }
            };
            UpdateRowVisibility(e.Row, task);
        }

        private static void UpdateRowVisibility(C1.WPF.DataGrid.DataGridRow row, Task task)
        {
            row.Visibility = task.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void grid_LoadedRowPresenter(object sender, C1.WPF.DataGrid.DataGridRowEventArgs e)
        {
            var task = e.Row.DataItem as Task;
            //e.Row.Presenter.FontWeight = task.SubTasks.Count > 0 ? FontWeights.Bold : FontWeights.Normal;
            e.Row.Presenter.FontWeight = task.Level == 0 ? FontWeights.Bold : FontWeights.Normal;
        }

        private void grid_LoadedRowHeaderPresenter(object sender, C1.WPF.DataGrid.DataGridRowEventArgs e)
        {
            var task = e.Row.DataItem as Task;
            var rowHeaderPresenter = e.Row.HeaderPresenter;
            if (task.SubTasks.Count > 0)
            {
                var toggleButton = new ToggleButton();
                toggleButton.Style = Resources["TButtonStyle"] as Style;
                toggleButton.SetBinding(ToggleButton.IsCheckedProperty, new Binding("IsExpanded")
                {
                    Source = task,
                    Mode = BindingMode.TwoWay
                });
                toggleButton.IsThreeState = false;
                toggleButton.Tag = task;
                rowHeaderPresenter.Content = toggleButton;
            }
            else
            {
                rowHeaderPresenter.Content = null;
            }
        }

        private void grid_AutoGeneratingColumn(object sender, C1.WPF.DataGrid.DataGridAutoGeneratingColumnEventArgs e)
        {

            if (e.Property.Name == "WBS")
            {
                e.Column.Width = C1.WPF.DataGrid.DataGridLength.Auto;
            }
            if (e.Column is C1.WPF.DataGrid.DataGridDateTimeColumn)
            {
                ((C1.WPF.DataGrid.DataGridDateTimeColumn)e.Column).Format = "ddd d/M/yyyy";
            }
            if (e.Property.Name == "Duration" && e.Column is C1.WPF.DataGrid.DataGridBoundColumn)
            {
                ((C1.WPF.DataGrid.DataGridBoundColumn)e.Column).Binding.Converter = CustomConverter.Create((value, type, parameter, culture) =>
                {
                    var valueTS = (TimeSpan)value;
                    return string.Format("{0:N1} days?", valueTS.TotalDays);
                });
            }
            if (new string[] { "Level", "ParentTask", "SubTasks", "IsExpanded", "IsVisible", "Duration", "Start", "Finish" }.Contains(e.Property.Name))
            {
                e.Cancel = true;
            }
            if (new string[] { "day0", "day1", "day2", "day3", "day4", "day5", "day6", "day7", "day8", "day9", "day10", "day11", "day12" }.Contains(e.Property.Name))
            {
                e.Column.Width = C1.WPF.DataGrid.DataGridLength.Auto;
                string s = e.Property.Name;
                object propertyValue = ContainProperty(dateSeries, s);
                e.Column.Header = propertyValue.ToString();
            }
        }

        public object ContainProperty(object instance, string propertyName)
        {
            if (instance != null && !string.IsNullOrEmpty(propertyName))
            {
                PropertyInfo _findedPropertyInfo = instance.GetType().GetProperty(propertyName);
                if (_findedPropertyInfo != null)
                    return _findedPropertyInfo.GetValue(instance, null);
            }
            return null;
        }

        private void grid_LoadedCellPresenter(object sender, C1.WPF.DataGrid.DataGridCellEventArgs e)
        {
            if (e.Cell.Column.Name == "Name")
            {
                var task = e.Cell.Row.DataItem as Task;
                e.Cell.Presenter.Padding = new Thickness(task.Level * 16, 0, 0, 0);
            }
            else if (e.Cell.Column.Name == "WBS")
            {
                var task = e.Cell.Row.DataItem as Task;
                e.Cell.Presenter.Padding = new Thickness(task.Level * 4, 0, 0, 0);
            }
            else
            {
                e.Cell.Presenter.Padding = new Thickness();
            }
            //if (e.Cell.Row.Index == 1 && e.Cell.Column.Index == 5)
            //    e.Cell.Presenter.Background = new SolidColorBrush(Colors.Yellow);
            //e.Cell. = false;

        }

        private void grid_CommittedEdit(object sender, C1.WPF.DataGrid.DataGridCellEventArgs e)
        {
            string day = string.Format("day{0}", e.Cell.Column.Index);
            //tasks[e.Cell.Row.Index].day1 = Convert.ToDecimal(e.Cell.Value);

            MessageBox.Show("行：" + e.Cell.Row.Index + " 列：day" + (e.Cell.Column.Index - 2) + " = " + lists[e.Cell.Row.Index].day[e.Cell.Column.Index - 2]);
        }

        #endregion

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            BindCombobox();
        }

        private void keyIndexGrid_Loaded(object sender, RoutedEventArgs e)
        {
            setGridCellColor(keyIndexGrid,6,"5%", Colors.DarkBlue); 
        }
        
        private void grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            //DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(i);
            //DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(1);
            //cell.Background = new SolidColorBrush(Colors.DarkGray); 
            //C1.WPF.DataGrid.DataGridRow row = grid.Rows[0];
            
        }

        private void grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            grid[2, 1].Presenter.Background = new SolidColorBrush(Colors.Green);
            grid.Rows[3].Presenter.Background = new SolidColorBrush(Colors.Gold);
        }
    }

    #region 数据结构:关键指标
    public class KeyIndex : INotifyPropertyChanged
    {
        public string Name { set; get; }
        //第二列
        private string indexValue;
        public string IndexValue
        {
            get
            {
                return indexValue;
            }
            set
            {
                if (indexValue != value)
                {
                    indexValue = value;
                    OnPrpertyChanged("IndexValue");
                }
            }
        }
        //第三列
        private string indexValue1;
        public string IndexValue1
        {
            get
            {
                return indexValue1;
            }
            set
            {
                if (indexValue1 != value)
                {
                    indexValue1 = value;
                    OnPrpertyChanged("IndexValue1");
                }
            }
        }
        //第四列
        private string indexValue2;
        public string IndexValue2
        {
            get
            {
                return indexValue2;
            }
            set
            {
                if (indexValue2 != value)
                {
                    indexValue2 = value;
                    OnPrpertyChanged("IndexValue2");
                }
            }
        }
        //两个参数的构造函数
        public KeyIndex(string name, decimal indexValue)
        {
            Name = name;
            IndexValue = indexValue.ToString();
        }
        public KeyIndex(string name, string indexValue)
        {
            Name = name;
            IndexValue = indexValue;
        }
        //四个参数的构造函数
        public KeyIndex(string name, decimal indexValue, decimal indexValue1, decimal indexValue2)
        {
            Name = name;
            IndexValue = indexValue.ToString();
            IndexValue1 = indexValue1.ToString();
            IndexValue2 = indexValue2.ToString();
        }
        public KeyIndex(string name, string indexValue, string indexValue1, string indexValue2)
        {
            Name = name;
            IndexValue = indexValue;
            IndexValue1 = indexValue1;
            IndexValue2 = indexValue2;
        }

        private void OnPrpertyChanged(string p)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
    #endregion

    #region 数据结构:头寸
    public class Task : INotifyPropertyChanged
    {
        private bool _isExpanded = true;
        public Task()
        {
            SubTasks = new ObservableCollection<Task>();
            IsExpanded = true;
        }

        public string WBS { get; set; }

        //[Display(Name = "Task Name")]
        public string Name { get; set; }

        /// <summary>
        /// day1-day13
        /// </summary>

        //public string day1  { get; set; }
        //public string day2  { get; set; }
        //public string day3  { get; set; }
        //public string day4  { get; set; }
        //public string day5  { get; set; }
        //public string day6  { get; set; }
        //public string day7  { get; set; }
        //public string day8  { get; set; }
        //public string day9  { get; set; }
        //public string day10 { get; set; }
        //public string day11 { get; set; }
        //public string day12 { get; set; }
        //public string day13 { get; set; }

        public Hashtable day = new Hashtable();
        private string _day0;
        private string _day1;
        private string _day2;
        private string _day3;
        private string _day4;
        private string _day5;
        private string _day6;
        private string _day7;
        private string _day8;
        private string _day9;
        private string _day10;
        private string _day11;
        private string _day12;
        //1：通过key存取Values
        public string this[int index]
        {
            get { return day[index].ToString(); }
            set { day.Add(index, value); /*_day1 = day[0].ToString();*/ }
        }

        public string day0 { get { return _day0; } set { day[0] = value; _day0 = value; } }
        public string day1 { get { return _day1; } set { day[1] = value; _day1 = value; } }
        public string day2 { get { return _day2; } set { day[2] = value; _day2 = value; } }
        public string day3 { get { return _day3; } set { day[3] = value; _day3 = value; } }
        public string day4 { get { return _day4; } set { day[4] = value; _day4 = value; } }
        public string day5 { get { return _day5; } set { day[5] = value; _day5 = value; } }
        public string day6 { get { return _day6; } set { day[6] = value; _day6 = value; } }
        public string day7 { get { return _day7; } set { day[7] = value; _day7 = value; } }
        public string day8 { get { return _day8; } set { day[8] = value; _day8 = value; } }
        public string day9 { get { return _day9; } set { day[9] = value; _day9 = value; } }
        public string day10 { get { return _day10; } set { day[10] = value; _day10 = value; } }
        public string day11 { get { return _day11; } set { day[11] = value; _day11 = value; } }
        public string day12 { get { return _day12; } set { day[12] = value; _day12 = value; } }

        public void setDayValue()
        {
            if (day[0] != null)
                _day0 = day[0].ToString();
            if (day[1] != null)
                _day1 = day[1].ToString();
            if (day[2] != null)
                _day2 = day[2].ToString();
            if (day[3] != null)
                _day3 = day[3].ToString();
            if (day[4] != null)
                _day4 = day[4].ToString();
            if (day[5] != null)
                _day5 = day[5].ToString();
            if (day[6] != null)
                _day6 = day[6].ToString();
            if (day[7] != null)
                _day7 = day[7].ToString();
            if (day[8] != null)
                _day8 = day[8].ToString();
            if (day[9] != null)
                _day9 = day[9].ToString();
            if (day[10] != null)
                _day10 = day[10].ToString();
            if (day[11] != null)
                _day11 = day[11].ToString();
            if (day[12] != null)
                _day12 = day[12].ToString();
        }


        //[Display(AutoGenerateField = false)]
        public Task ParentTask { get; set; }

        //[Display(AutoGenerateField = false)]
        public ObservableCollection<Task> SubTasks { get; set; }

        //[Display(AutoGenerateField = false)]
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                OnPropertyChanged("IsExpanded");
                OnIsVisibleChanged();
            }
        }

        private void OnIsVisibleChanged()
        {
            OnPropertyChanged("IsVisible");
            foreach (var subTask in SubTasks)
            {
                subTask.OnIsVisibleChanged();
            }
        }

        //[Display(AutoGenerateField = false)]
        public bool IsVisible
        {
            get
            {
                if (ParentTask == null)
                {
                    return true;
                }
                else if (!ParentTask.IsExpanded)
                {
                    return false;
                }
                else
                {
                    return ParentTask.IsVisible;
                }
            }
        }

        //[Display(AutoGenerateField = false)]
        public int Level
        {
            get
            {
                if (ParentTask == null)
                {
                    return 0;
                }
                else
                {
                    return ParentTask.Level + 1;
                }
            }
        }

        public static IEnumerable<Task> GetAllTasks(ObservableCollection<Task> tasks)
        {

            foreach (var task in tasks)
            {
                yield return task;
                foreach (var subTask in Task.GetAllTasks(task.SubTasks))
                {
                    yield return subTask;
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
    #endregion
}
