using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfApplication3
{
    class User : Window
    {
        //private string _loginName;
        //private string _cnName;
        //private string _pwd;
        //private int _authorityType;

        public string loginName { get; set; }
        public string cnName { get; set; }
        public string pwd { get; set; }
        public int authorityType { get; set; }

        public List<string> portfolioList;

        User()
        {
            InitPortfolioList();
            InitTabList();
        }

        /// 
        /// 初始化组合列表
        ///
        private void InitPortfolioList()
        {
            //查询PortFolioManager数据库表
        }
        /// 
        /// 初始化权限菜单范围
        ///
        private void InitTabList()
        {
            switch (authorityType)
            {
                case 1:
                    break;
                case 2:
                    //tab1.Visibility = Visibility.Hidden;
                    //tab2.Visibility = Visibility.Hidden;
                    //tab3.Visibility = Visibility.Hidden;
                    break;
            }
        }
    }
}
