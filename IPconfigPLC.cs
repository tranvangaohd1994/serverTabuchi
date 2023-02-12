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
using System.Data.SqlClient;
namespace Server
{
    public partial class IPconfigPLC : Form
    {//sql
        
        // doi tuong chua cac ham xu ly database
        DBmanager db = new DBmanager();

        string[] arrIP = new string[12];
        string allIP="";
        int[] arrIpInt = new int[12];
        public IPconfigPLC()
        {
            InitializeComponent();
        }

        private void IPconfigPLC_Load(object sender, EventArgs e)
        {
            initView();
        }
        void initView() {
            TextBox[] arrTextBox = { ip_plc1, ip_plc2, ip_plc3, ip_plc4, ip_plc5, ip_plc6, ip_plc7, ip_plc8, ip_plc9, ip_plc10 };
           
            for (int i = 0; i < DungChung.SoPLC; ++i)
            {
                if (DungChung.diachiIP[i].Length > 0)
                {
                    arrTextBox[i].Text = DungChung.diachiIP[i] + "-" + DungChung.L_PLC[i].ToString() + " line" ;
                }
            }
            ip_plc_stock1.Text = DungChung.diachiIP[DungChung.SoPLC];
            ip_plc_stock2.Text = DungChung.diachiIP[DungChung.SoPLC+1];
            SumLine.Text = DungChung.soLine.ToString();
        }
        void wirte_registry(string arrIP , int numLine)
        {
            RegistryKey regKey = Registry.CurrentUser;
            RegistryKey regKey2 = regKey.OpenSubKey("Software\\ServerTabuchi\\IpConfig");

            regKey2 = regKey.CreateSubKey("Software\\ServerTabuchi\\IpConfig", RegistryKeyPermissionCheck.ReadWriteSubTree);
            regKey2.SetValue("numberLine", numLine);
            regKey2.SetValue("StringIP", arrIP);
            regKey2.Close();
            regKey.Close();
            
        }
        private void btnSaveConfigIP_Click(object sender, EventArgs e)
        {
            return ;
            int numLine;
            try{
                 numLine = int.Parse(SumLine.Text);
            }
            catch {
                numLine = 0;
            }
            if (numLine > 0 && numLine <= 70 ){
                
            }
            else {
                showMessageWrong("Wrong SumLine.SumLine >0 and <=70");
                return;
            }
            //lay so luong PLC can dung
            int numPLC;
            if ((numLine % 7) != 0) { numPLC = numLine / 7 + 1; }
            else numPLC = numLine / 7;

            TextBox[] arrTextBox = { ip_plc1, ip_plc2, ip_plc3, ip_plc4, ip_plc5, ip_plc6, ip_plc7, ip_plc8, ip_plc9, ip_plc10 };
            for (int i = 0; i < numPLC;++i )
            {
                if (check_text(arrTextBox[i].Text,i))
                {
                    arrIP[i] = arrTextBox[i].Text;
                }
                else {
                    showMessageWrong("wrong ip_plc"+(i+1));
                    return;
                }
            }
            //kiem tra 2 stock
            if (!check_text(ip_plc_stock1.Text, numPLC)) {
                showMessageWrong("wrong ip_plc_stock1");
                return;
            }
            else arrIP[numPLC] = ip_plc_stock1.Text;
            if (!check_text(ip_plc_stock2.Text, numPLC + 1)) {
                showMessageWrong("wrong ip_plc_stock1");
                return;
            }
            else arrIP[numPLC + 1] = ip_plc_stock2.Text;
            allIP = "";
            for (int i = 0; i < numPLC + 2;++i )
            {
                allIP =allIP + arrIP[i]+" ";
            }

            string caption = "Exit message";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            // Displays the MessageBox.
            result = MessageBox.Show("Chuong trinh phai khoi dong lai de hoan tat cai dat ", caption, buttons);
            if (result == DialogResult.Yes)
            {
                //DungChung.soLine = numLine;
                wirte_registry(allIP, numLine);
                db.xulydulieu("Update LineConfig set Value= '" + numLine + "'");

            }
                      
        }
        bool check_text(string input,int indexPLC) {
            string[] output = input.Split('.');
            if (output.Length == 4)
            {
                if (output[0] != "192") return false;
                if (output[1] != "168") return false;
                int a = int.Parse(output[2]);
                if (a <= 0 || a >= 255) return false;
                int b = int.Parse(output[3]);
                if (b <= 0 || b >= 255) return false;
                for (int i = 0; i < indexPLC; ++i) {
                    if (b == arrIpInt[i]) return false;
                }
                arrIpInt[indexPLC] = b;
                
            }
            else return false;
            return true;
        }
        void showMessageWrong(string msg) {
            string caption = "Exit message";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;
            // Displays the MessageBox.
            result = MessageBox.Show(msg, caption, buttons);
        }
    }
}
