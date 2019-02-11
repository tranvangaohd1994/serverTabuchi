using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Win32;

namespace Server
{
    //time start:

    public partial class Time_auto : Form
    {
        //uy quyen thiet lap ben form main

        public delegate void dlgsettime(int dh, int dm, int nh, int nm);
        static public void settime_auto(int dh, int dm, int nh, int nm)
        {

            //thoi gian bat dau ca sang 
            auto_startD_h = dh;
            auto_startD_m = dm;
            //thoi gian bat dau ca toi
            auto_startN_h = nh;
            auto_startN_m = nm;
        }

        static public void settime_autoEx(int dh, int dm, int nh, int nm)
        {
            //thoi gian bat dau ca sang 
            auto_expD_h = dh;
            auto_expD_m = dm;
            //thoi gian bat dau ca toi
            auto_expN_h = nh;
            auto_expN_m = nm;
        }
        dlgsettime settime = new dlgsettime(settime_auto);

        public Time_auto()
        {
            InitializeComponent();

            txt_dh.Text = auto_startD_h.ToString();
            txt_dm.Text = auto_startD_m.ToString();
            txt_nh.Text = auto_startN_h.ToString();
            txt_nm.Text = auto_startN_m.ToString();

            txt_Edh.Text = auto_expD_h.ToString();
            txt_Edm.Text = auto_expD_m.ToString();
            txt_Enh.Text = auto_expN_h.ToString();
            txt_Enm.Text = auto_expN_m.ToString();
        }

        //time start Day:
        static public int auto_startD_h = 7;
        static public int auto_startD_m = 0;
        //time start Night:
        static public int auto_startN_h = 19;
        static public int auto_startN_m = 0;

        //time export for day session
        static public int auto_expD_h = 7;
        static public int auto_expD_m = 0;
        //time export for night session
        static public int auto_expN_h = 19;
        static public int auto_expN_m = 0;

        private void bt_default_Click(object sender, EventArgs e)
        {
            //thiet lap mac dinh
            txt_dh.Text = "7";
            txt_dm.Text = "0";
            txt_nh.Text = "19";
            txt_nm.Text = "0";
        }

        private void bt_Save_Click(object sender, EventArgs e)
        {
            bool D_h = false;
            bool D_m = false;
            bool N_h = false;
            bool N_m = false;
            bool ED_h = false;
            bool ED_m = false;
            bool EN_h = false;
            bool EN_m = false;
            string message = " There are some invalid value!";
            string caption = "Message";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;
            #region lay gia tri tu edit text va kiem tra xem co phai kieu so ko
            int n;
            bool isNumeric = int.TryParse(txt_dh.Text, out n);
            if (isNumeric)
            {
                auto_startD_h = Int16.Parse(txt_dh.Text);
            }
            else
            {
                if (txt_dh.Text == "")
                {
                    auto_startD_h = 0;
                    isNumeric = true;
                }

            }

            bool isNumeric2 = int.TryParse(txt_dm.Text, out n);
            if (isNumeric2)
            {
                auto_startD_m = Int16.Parse(txt_dm.Text);
            }
            else
            {
                if (txt_dm.Text == "")
                {
                    auto_startD_m = 0;
                    isNumeric2 = true;
                }
            }

            bool isNumeric3 = int.TryParse(txt_nh.Text, out n);
            if (isNumeric3)
            {
                auto_startN_h = Int16.Parse(txt_nh.Text);
            }
            else
            {
                if (txt_nh.Text == "")
                {
                    auto_startN_h = 0;
                    isNumeric3 = true;
                }
            }

            bool isNumeric4 = int.TryParse(txt_nm.Text, out n);
            if (isNumeric4)
            {
                auto_startN_m = Int16.Parse(txt_nm.Text);
            }
            else
            {
                if (txt_nm.Text == "")
                {
                    auto_startN_m = 0;
                    isNumeric4 = true;
                }
            }
            #endregion
            bool isNumeric5 = int.TryParse(txt_Edh.Text, out n);
            if (isNumeric5)
            {
                auto_expD_h = Int16.Parse(txt_Edh.Text);
            }
            else
            {
                if (txt_Edh.Text.Equals(""))
                {
                    auto_expD_h = 0;
                    isNumeric5 = true;
                }
            }

            bool isNumeric6 = int.TryParse(txt_Edm.Text, out n);
            if (isNumeric6)
            {
                auto_expD_m = Int16.Parse(txt_Edm.Text);
            }
            else
            {
                if (txt_Edm.Text.Equals(""))
                {
                    auto_expD_m = 0;
                    isNumeric6 = true;
                }
            }

            bool isNumeric7 = int.TryParse(txt_Enh.Text, out n);
            if (isNumeric7)
            {
                auto_expN_h = Int16.Parse(txt_Enh.Text);
            }
            else
            {
                if (txt_Enh.Text.Equals(""))
                {
                    auto_expN_h = 0;
                    isNumeric7 = true;
                }
            }

            bool isNumeric8 = int.TryParse(txt_Enm.Text, out n);
            if (isNumeric8)
            {
                auto_expN_m = Int16.Parse(txt_Enm.Text);
            }
            else
            {
                if (txt_Enm.Text.Equals(""))
                {
                    auto_expN_m = 0;
                    isNumeric8 = true;
                }
            }

            #region kiem soat gia tri hop le
            
            //bool DbigerN = false;
            if (isNumeric == true)
            {
                if (auto_startD_h > 23 || auto_startD_h < 0)
                {
                    D_h = true;
                }
            }
            if (isNumeric2 == true)
            {
                if (auto_startD_m > 59 || auto_startD_m < 0)
                {
                    D_m = true;
                }
            }
            if (isNumeric3 == true)
            {
                if (auto_startN_h > 23 || auto_startN_h < 0)
                {
                    N_h = true;
                }
            }
            if (isNumeric4 == true)
            {
                if (auto_startN_m > 59 || auto_startN_m < 0)
                {
                    N_m = true;
                }
            }
            if (isNumeric5 == true)
            {
                if (auto_expD_h > 23 || auto_expD_h < 0)
                {
                    ED_h = true;
                }
            }
            if (isNumeric6 == true)
            {
                if (auto_expD_m > 59 || auto_expD_m < 0)
                {
                    ED_m = true;
                }
            }
            if (isNumeric7 == true)
            {
                if (auto_expN_h > 23 || auto_expN_h < 0)
                {
                    EN_h = true;
                }
            }
            if (isNumeric8 == true)
            {
                if (auto_expN_m > 59 || auto_expN_m < 0)
                {
                    EN_m = true;
                }
            }
            // neu co loi xay ra thi valid 1  se == true
            bool valid1 = D_h || D_m || N_h || N_m || (!isNumeric) || (!isNumeric2) || (!isNumeric3) || (!isNumeric4);
            bool valid2 =ED_h || ED_m || EN_h || EN_m || (!isNumeric5) || (!isNumeric6) || (!isNumeric7) || (!isNumeric8);
            if (valid1)
            {
                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
            }
            #endregion

            // neu khong vi pham loi nao thi thay doi thoi gian auto start

            if (!valid1 && (!valid2))
            {
                Main_form.settime_auto(auto_startD_h, auto_startD_m, auto_startN_h, auto_startN_m);
                Main_form.settime_autoEx(auto_expD_h, auto_expD_m, auto_expN_h, auto_expN_m);
                message = " Time for auto mode has been changed ";
                result = MessageBox.Show(message, caption, buttons);
                if (result == DialogResult.OK)
                {
                    this.Close();
                }
            }

        }

        private void bt_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bt_dfEx_Click(object sender, EventArgs e)
        {
            txt_Edh.Text = "18";
            txt_Edm.Text = "35";
            txt_Enh.Text = "6";
            txt_Enm.Text = "35";
        }


    }
}
