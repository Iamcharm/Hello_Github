using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WpfApplication3
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            FillDataGrid();
        }
        private void FillDataGrid()
        {
            string str = string.Format(@"
                SELECT 
                ROW_NUMBER() OVER (ORDER BY a.assetId desc) AS RowNumber,
                a.assetId, a.name1, convert(varchar,aa.pricingdate,23) as pricingdate, 
                aa.pc05, aa.pccd05, aa.pc10, aa.pccd10,
                aa.pc20, aa.pccd20, aa.pc60, aa.pccd60, 
                aa.pc120, aa.pccd120, aa.pc250, aa.pccd250
                FROM AA.dbo.assetsector ar, AA.dbo.percentchange aa, AA.dbo.asset a
                WHERE aa.pricingdate = '20160712'
                AND ar.modelid = 0 
                AND ar.classid = 1 
                AND (ar.parentlevel = 6 OR ar.parentlevel = 7)
                AND aa.assetid = ar.assetid 
                AND aa.assetid = a.assetid 
                ORDER BY ar.parentlevel, aa.assetid
            "/*, pagesize, pageindex, strWhere*/);
            SqlHelper sqh = new SqlHelper("con_Infocenter");
            //sqh.getConnectString("con_Infocenter");
            DataTable dt = sqh.ExecuteDataSetText(str, null).Tables[0];
            grdEmployee.ItemsSource = dt.DefaultView;
        }
    }
}
