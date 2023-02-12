using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using System.Net.NetworkInformation;
using System.IO;
using System.IO.Ports;
using Microsoft.Win32;
using System.Net;
using System.Net.Sockets;
namespace Server
{
    public partial class Main_form : Form
    {
        
        /// Khoi tao FORM chinh
        /// ////////////////////////////////////////////////
        public Main_form()
        {
            #region kiem tra registry
            RegistryKey currentUser = Registry.CurrentUser;
            RegistryKey registryKey = currentUser.OpenSubKey("Software\\ServerTabuchi\\Autostart time");
            RegistryKey registryKey2 = currentUser.OpenSubKey("Software\\ServerTabuchi\\Auto Export time");
            RegistryKey registryKey3 = currentUser.OpenSubKey("Software\\ServerTabuchi\\IpConfig");
            if (registryKey == null)
            {
                registryKey = currentUser.CreateSubKey("Software\\ServerTabuchi\\Autostart time", RegistryKeyPermissionCheck.ReadWriteSubTree);
                currentUser.Close();
                registryKey.SetValue("Day start h", 7);
                registryKey.SetValue("Day start m", 0);
                registryKey.SetValue("Night start h", 19);
                registryKey.SetValue("Night start m", 0);
            }
            else
            {
                auto_startD_h = (int)registryKey.GetValue("Day start h");
                auto_startD_m = (int)registryKey.GetValue("Day start m");
                auto_startN_h = (int)registryKey.GetValue("Night start h");
                auto_startN_m = (int)registryKey.GetValue("Night start m");
            }
            if (registryKey2 == null)
            {
                registryKey2 = currentUser.CreateSubKey("Software\\ServerTabuchi\\Auto Export time", RegistryKeyPermissionCheck.ReadWriteSubTree);
                registryKey2.SetValue("Day Ex h", 18);
                registryKey2.SetValue("Day Ex m", 35);
                registryKey2.SetValue("Night Ex h", 6);
                registryKey2.SetValue("Night Ex m", 35);
            }
            else
            {
                auto_expD_h = (int)registryKey2.GetValue("Day Ex h");
                auto_expD_m = (int)registryKey2.GetValue("Day Ex m");
                auto_expN_h = (int)registryKey2.GetValue("Night Ex h");
                auto_expN_m = (int)registryKey2.GetValue("Night Ex m");
            }
            //registryKey3.Close();
            registryKey.Close();
            registryKey2.Close();
            currentUser.Close();
            #endregion
            MB = new ModbusTCP[DungChung.SoPLC + 2];//1 mảng Modbus 8 phần từ 6 PLC ở line và 2 PLC ở Stocking
            Line_base_on_data = new Line_pr();
            #region khoi tao log--khong ghi log nua
            /*
            //Kiem tra co ton tai ngay_thang_nam chua, neu da co thi ghi tiep len file tren
            //
            var path = System.Reflection.Assembly.GetExecutingAssembly().Location;// lay duong dan +  chuong trinh
            string path_folder = Path.GetDirectoryName(path);// lay duong dan
            path_folder += "\\log";
            DirectoryInfo directoryLog = new DirectoryInfo(path_folder);
            if (!directoryLog.Exists)
            {
                //neu ko ton tai thi tao folder log
                directoryLog.Create();

            }
            if (DateTime.Now.Day < 10)
            {
                log_name = "0" + DateTime.Now.Day.ToString() + "_";
            }
            else { log_name = DateTime.Now.Day.ToString() + "_"; }
            if (DateTime.Now.Month < 10)
            {
                log_name += "0" + DateTime.Now.Month.ToString() + "_";
            }
            else {  log_name += DateTime.Now.Month.ToString() + "_"; }
            log_name += DateTime.Now.Year.ToString();

            string log_path = path_folder + "\\" + log_name.ToString() + ".log";
            filelog = new FileInfo(log_path);
            //khoi tao neu ko ton tai
            if (!filelog.Exists)
            {
                //ghi ngay thang nam, danh dau h
                logstream = new StreamWriter(filelog.FullName, true, Encoding.Unicode);//append -> nhay den cuoi dong va mo file hoac tao file
                logstream.WriteLine("Log file has been created at:  " + DateTime.Now.ToString());
                logstream.Close();
            }
            else
            {
                logstream = new StreamWriter(filelog.FullName, true, Encoding.Unicode);//append -> nhay den cuoi dong va mo file hoac tao file
                //logstream.Write(DateTime.Now.ToString());
                logstream.WriteLine("**********Open Program**********");
                logstream.WriteLine("********************************");
                logstream.Close();
                //logstream.
            }
            */
            #endregion
            
            //khoi tao giao dien
            InitializeComponent();
            

            //kiem tra che do hoat dong la  auto hay manual
            //if (menu_Auto.Checked == true)
            
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            btnIntinalize.Enabled = false;
            btnReset.Enabled = false;
            toolStripMenuItem1.Checked = true;
            
            #region Ping va ket noi
            Thread checklinestt = new Thread(new ThreadStart(First_connect));
            checklinestt.IsBackground = true;
            checklinestt.Start();
            #endregion
            #region chay main_thread
            mainThr_1st = true;
            Thread main_thread = new Thread(new ThreadStart(Main_thread_v2));
            //main_thread.IsBackground = true;
            main_thread.Start();
            lbStatus.Text = "Status: Run";
            #endregion
            //khoi dong luong quan ly chinh
            Thread ManagerThread = new Thread(new ThreadStart(Manager_thread));
            ManagerThread.IsBackground = true;
            ManagerThread.Start();
        }
        public void set_auto()//ghi thoi gian hoat dong hien hanh vao regisry 
        {
            //thoi gian hien hanh
            int gio = DateTime.Now.Hour;
            int phut = DateTime.Now.Minute;
            RegistryKey regKey = Registry.CurrentUser;
            RegistryKey regKey2 = regKey.CreateSubKey("Software\\ServerTabuchi\\Auto Export time", RegistryKeyPermissionCheck.ReadWriteSubTree);          
            regKey2.SetValue("Gio", gio);
            regKey2.SetValue("Phut", phut);
            regKey2.Close();
            regKey.Close();
           
        }

        /// CAC SU KIEN CLICK NUT/////////////////////////
        private void allDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exc_allDetail();
        }
        private void menuIT_auto_time_Click(object sender, EventArgs e)
        {
            Exc_auto_time_Click();
        }
        private void timer1_Tick(object sender, EventArgs e){
            //set_auto();
            //Excel_export();          
        }          
        private void btnIntinalize_Click(object sender, EventArgs e){
            Exc_btnIntinalize();
        }
        private void btnStart_Click(object sender, EventArgs e){
            Exc_btnStart();
        }
        private void btnStop_Click(object sender, EventArgs e){
            Exc_btnStop();
        }
        private void btnReset_Click(object sender, EventArgs e){
            Exc_btnReset();
        }
        private void logoutToolStripMenuItem_Click(object sender, EventArgs e){
            Exc_logoutToolStripMenuItem();
        }
        // khi an Ping
        private void btnPing_Click(object sender, EventArgs e){
            Exc_btnPing();
        }
        private void btnClear_Click(object sender, EventArgs e){
            listStatus.Items.Clear();
        }
        private void btnSave_Click(object sender, EventArgs e){
            DialogResult dr = MessageBox.Show("Do you want to save this listbox?", "Alart", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                string path = @"C:\" + DateTime.Now.ToString() + ".txt";
                System.IO.StreamWriter saveFile = new System.IO.StreamWriter(path);
                saveFile.WriteLine(listStatus.Items.ToString());
                saveFile.ToString();
                saveFile.Close();
                listStatus.Items.Add(DateTime.Now.ToString() + ": Saved");
            }
        }
        //thu ve icon stray khi thu giao dien
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }
        //kick dup de hien thi giao dien tu icon
        private void iconServer_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.Focus();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();// da co su kien hoi khi close: "Form1_FormClosing"
        }
        //hien thi lai giao dien
        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.Focus();
        }

        //xu ly su kien khi tat form
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string message = "Do you want to exit?";
            string caption = "Exit message";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            // Displays the MessageBox.
            result = MessageBox.Show(message, caption, buttons);
            if (result == System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = true;
            }
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    //stop main thread
                    //thong bao cho main thread nghi neu dang hoat dong
                    // doi cho den khi no da hoan thanh
                    timer1.Dispose();
                    Main_thread_stt = false;
                    Manager_thread_stt = false;
                    while (Main_thread_done == false || Manager_thread_done == false)
                    {
                        Thread.Sleep(50);
                    }
                    /*TabuchiFix*/
                    for (int i = 0; i < DungChung.SoPLC + 2; i++)
                    {
                        if (MB[i] != null)
                        {
                            MB[i].Dispose();
                            MB[i] = null;
                        }
                    }
                    //ghi log--không cần ghi log nữa 30-8
                    /*
                    logstream = new StreamWriter(filelog.FullName, true, Encoding.Unicode);
                    logstream.WriteLine("*******************************************************");
                    logstream.WriteLine(DateTime.Now.ToLongTimeString() + "   " + "CLOSE PROGRAM");
                    logstream.WriteLine("*******************************************************");
                    logstream.Dispose();
                     * */
                }
                catch (Exception ex)
                {
                    Display_listbox_stt(DateTime.Now.ToString() + "  :Close err: " + ex.Message);
                }
            }
        }
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e) { }
        private void configipToolStripMenuItem_Click_1(object sender, EventArgs e) {
            IPconfigPLC _formIPconfig = new IPconfigPLC();
            _formIPconfig.Show();
        }
        private void helpToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void configipToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void toolStripTextBox1_Click(object sender, EventArgs e) { }
        private void toolStripMenuItem1_Click(object sender, EventArgs e) { }
        private void helpToolStripMenuItem1_Click(object sender, EventArgs e) { }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) { }
    }
}