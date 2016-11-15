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
using WpfApplication3.Data.BondFund;

namespace WpfApplication3
{
    /// <summary>
    /// Tab_BondFund.xaml 的交互逻辑
    /// </summary>
    public partial class Tab_BondFund : UserControl
    {
        public Tab_BondFund()
        {
            InitializeComponent();
            selectDate_bond.Text = DateTime.Now.ToString("yyyy-MM-dd").ToString();
            BindCombobox_bond();
            GlobalParameters.fundCode_bond = portfolioBox_bond.SelectedValue.ToString();  //选择基金代码
            showBondInfoIndex(GlobalParameters.fundCode_bond, GlobalParameters.lastTradingDate);
            //bond_bonddetail.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[9].DefaultView;
            //bond_bonddetail.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[9].DefaultView;
        }
        private void BindCombobox_bond()
        {
            portfolioBox_bond.ItemsSource = GlobalParameters.portfolio_bond;
            portfolioBox_bond.SelectedValuePath = "Key";
            portfolioBox_bond.DisplayMemberPath = "Value";
            portfolioBox_bond.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GlobalParameters.Date_bond = selectDate_bond.Text;
            TimeSpan diff;
            string portfolio = portfolioBox_bond.SelectedValue.ToString();
            if (selectDate_bond.Text == "")
            {
                selectDate_bond.Text= DateTime.Now.ToString("yyyy-MM-dd").ToString();
                GlobalParameters.Date_bond = DateTime.Now.ToString("yyyy-MM-dd").ToString();
            }
            else
            {
                diff = Convert.ToDateTime(selectDate_bond.Text) - DateTime.Now;
                if (diff.Days > 0)
                {
                    MessageBox.Show("请输入的日期小于等于今日");
                    GlobalParameters.Date_bond = DateTime.Now.ToString("yyyy-MM-dd").ToString();
                }
                else
                {
                    GlobalParameters.Date_bond = Convert.ToDateTime(selectDate_bond.Text).ToString("yyyy-MM-dd").ToString();
                }
            }
            GlobalParameters.fundCode_bond = portfolioBox_bond.SelectedValue.ToString();  //选择基金代码
            MessageBox.Show("代码="+ GlobalParameters.fundCode_bond+",日期="+ GlobalParameters.Date_bond);
            showBondInfoIndex(GlobalParameters.fundCode_bond, GlobalParameters.Date_bond);
            //bond_bonddetail.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[9].DefaultView;
        }

        private void showBondInfoIndex(string fundCode_input, string date_input)
        {
            BondInfo.getBondInfoKeyIndex(fundCode_input , date_input);
            bf_keyIndexGrid.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[0].DefaultView;
            bf_aa.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[1].DefaultView;
            bf_bondnature.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[2].DefaultView;
            bf_secumarket.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[3].DefaultView;
            bf_compoundmethod.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[4].DefaultView;
            bf_enddate.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[5].DefaultView;
            bf_creditenddate.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[6].DefaultView;
            bf_ztrating.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[7].DefaultView;
            bf_zxrating.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[8].DefaultView;

            bf_bonddetail.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[9].DefaultView;
            bf_kzzdetail.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[10].DefaultView;
            bf_stockdetail.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[11].DefaultView;
            bf_funddetail.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[12].DefaultView;
            bf_absdetail.ItemsSource = BondInfo.bondInfoDataSet_keyIndex.Tables[13].DefaultView;
        }

        private DataTable showBondIndexDetail(DataTable BondDetailTable,int i)
        {
            DataTable dt;
            if(BondDetailTable==null || BondDetailTable.Select("1=1").Length == 0)
            {
                return null;
            }
            else
            {
                if (i == 0 )
                {
                    dt = BondDetailTable.Select("1=1").CopyToDataTable();
                    return BondDetailTable;
                }
                else
                {
                    string sql_str = string.Format("BondNature={0}", i);
                    dt = BondDetailTable.Select(sql_str).CopyToDataTable<DataRow>();
                    return dt;
                }
            }
        }        
    }
}
