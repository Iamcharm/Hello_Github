using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SimpleLoginSystem
{
    public class LoginViewModel : ViewModelBase
    {
        #region 构造函数及登陆用户名及密码

        public LoginViewModel()
        {
            dicUser = new Dictionary<string, string>();
            dicUser.Add("yeshen","1015068292");
            dicUser.Add("tianshi","123456");
            dicUser.Add("you","123456");
            dicUser.Add("me","123456");

            //显示用户名及密码
            var mainStackPanel=new System.Windows.Controls.StackPanel();
            double width = 160; double hight = 40;


            foreach (var item in dicUser)
            {
                var tStackPanel = new System.Windows.Controls.StackPanel() { 
                    Orientation = System.Windows.Controls.Orientation.Horizontal, 
                };
                var txtName = new System.Windows.Controls.TextBlock() {
                    Width = width,
                    Height = hight,
                  Text=item.Key,
                  Background=new System.Windows.Media.SolidColorBrush(Colors.LightBlue),
                
                };
                tStackPanel.Children.Add(txtName);
                var txtPassWord = new System.Windows.Controls.TextBlock() {
                    Width = width,
                    Height = hight,
                    Text = item.Value,
                    Background = new System.Windows.Media.SolidColorBrush(Colors.LightCyan),
                };
                tStackPanel.Children.Add(txtPassWord);
                var border = new System.Windows.Controls.Border()
                {
                    BorderThickness = new System.Windows.Thickness(2),
                    BorderBrush = new System.Windows.Media.SolidColorBrush(Colors.Black),
                };
                border.Child = tStackPanel;
                mainStackPanel.Children.Add(border);
            }
            System.Windows.Window window = new System.Windows.Window() { 
                Title="测试程序的用户名及密码",
                Content=mainStackPanel
            };
            //window.Show();

        }

        #endregion

        #region 局部变量
        /// <summary>
        /// 测试程序用户名及密码
        /// </summary>
        Dictionary<string, string> dicUser = null;
        #endregion


        #region 成员
        private string userName;
        private string userPassword;
        ///// <summary>
        ///// 自动登录
        ///// </summary>
        //private bool automaticLogon;
        ///// <summary>
        ///// 是否记住密码
        ///// </summary>
        //private bool rememberPwd;
        #endregion

        #region 属性
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get { return userName; } set{this.SetProperty(ref this.userName,value); } }
        /// <summary>
        /// 密码
        /// </summary>
        public string UserPassword { get { return userPassword; } set { this.SetProperty(ref this.userPassword, value); } }
        ///// <summary>
        ///// 自动登录
        ///// </summary>
        //public bool AutomaticLogon { get { return automaticLogon; } set { this.SetProperty(ref this.automaticLogon,value);} }
        ///// <summary>
        ///// 是否记住密码
        ///// </summary>
        //public bool RememberPwd { get { return rememberPwd; } set { this.SetProperty(ref this.rememberPwd, value); } }
       
        #endregion

        #region 工作方法
        /// <summary>
        /// 用于验证登陆信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>
        /// 返回值代表 
        /// -6：密码不正确  -5：用户名不正确
        /// -4：密码含有特殊字符 -3：密码不能为空
        /// -2：用户名不能为空
        /// -1：数据库未连接 0：登陆失败
        ///  1：登陆成功  2：
        /// 
        /// </returns>
        public int Verification(ref int userID)
        {
            int flag = 0;
            try
            {
              

              if (string.IsNullOrEmpty(this.userName))
              {
                  flag = -2;
                  return flag;
              }
             if (string.IsNullOrEmpty(this.userPassword))
             {
                 flag = -3;
                 return flag;
             }
                //正则表达式
            //var r = new System.Text.RegularExpressions.Regex(@"\-");
            //if (r.IsMatch(this.userPassword)||r.IsMatch(this.userName))
            //{
            //    flag = -4;
            //    return flag;
            //}

           //验证登录
             if (null == dicUser.Keys.FirstOrDefault(u=>u==this.userName))
             {
                 flag = -5;
             }
             else
             {
                 var userInfo = dicUser.FirstOrDefault(u => u.Key == this.userName && u.Value == this.userPassword);
                 if (userInfo.Key != null && userInfo.Value!=null)
                 {
                     userID = 100;
                     //int id = userInfo.ID;
                     flag = 1;
                     //休眠1.5秒
                     System.Threading.Thread.Sleep(1500);
                 }
                 else
                 {
                     flag = -6;
                 }
             }


            }catch (Exception)
            {
                flag= -1;
                //出现错误无视
            }
            return flag;
        }

        #endregion


    }
}
