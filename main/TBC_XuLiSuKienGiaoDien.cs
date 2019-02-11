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
        //trang thai cac nut
        bool ping_click = false;
        //delegate cho hien thi 
        private delegate void dlg_Display(string s);
        //delegate cho insert
        private delegate void dlg_Display_insert(string s);

        //không sửa
        private void Display_listbox_stt(string s)
        {
            if (this.listStatus.InvokeRequired)
            {
                this.Invoke(new dlg_Display(Display_listbox_stt), new object[] { s });
            }
            else
            {
                string time = DateTime.Now.ToString() +"  "+ s;
                this.listStatus.Items.Add(time);
                int i = this.listStatus.Items.Count;
                if (i > 200) { this.listStatus.Items.RemoveAt(0); }
                this.autoScroll();

                //log--không cần ghi log 30-8
                /*
                this.logstream = new StreamWriter(filelog.FullName, true, Encoding.Unicode);
                this.logstream.WriteLine(time);
                this.logstream.Close();
                 */
            }
        }
        private delegate void dlg_Display_time();
        private void Display_time()
        {
            if (this.lblTimer.InvokeRequired)
            {
                this.Invoke(new dlg_Display_time(Display_time));
            }
            else
            {
                lblTimer.Text = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "   " + DateTime.Now.ToShortTimeString();
            }
        }
        //insert
        private void Display_listbox_stt_insert(string s)
        {
            if (this.listStatus.InvokeRequired)
            {
                this.listStatus.Invoke(new dlg_Display_insert(Display_listbox_stt_insert), new object[] { s });
            }
            else
            {
                int i = this.listStatus.Items.Count;
                if (i > 1) { this.listStatus.Items.RemoveAt(i - 1); }
                this.listStatus.Items.Add(s);
                this.autoScroll();
                //log--không cần ghi logfile nữa 
                /*
                this.logstream = new StreamWriter(filelog.FullName, true, Encoding.Unicode);
                this.logstream.WriteLine(s);
                this.logstream.Close();
                 * */
            }
        }
        //Ham set trang thai cac nut tren giao dien
        public void bt_statusSystem(bool status)
        {
            btnReset.Enabled = !status;
            btnStart.Enabled = !status;
            btnIntinalize.Enabled = !status;
            btnStop.Enabled = status;
        }

        //Ham lay du lieu dua vao thoi gian (ca sang hay ca dem)
        public DataTable getTablefromDB(DateTime t, int PlanId)
        {
            if (dt != null) dt.Dispose();

            dt = new DataTable();
            //Neu buoi sang (6h den H19) -> thi lay du lieu cho buoi sang
            if (PlanId == -1)
            {
                Display_listbox_stt(t.ToString() + "    Data for this day is invalid");
            }
            else
            {
                if (t.Hour < 19 && t.Hour > 6)
                {
                    dt = db.laybang("Select * FROM LineInputDay Where PlanId = '" + PlanId + "'" + "  order by LineName");
                }
                else//Neu buoi sang (6h den H19) -> thi lay du lieu cho buoi sang
                    dt = db.laybang("Select * FROM LineInputNight Where [PlanId] = '" + PlanId + "'" + "  order by LineName");
            }
            return dt;
        }
        public void get_color_table(DateTime t, int PlanId)
        {
            if (color_table != null)
                color_table.Dispose();

            color_table = new DataTable();
            //Neu buoi sang (6h den H19) -> thi lay du lieu cho buoi sang
            if (PlanId == -1)
            {
                Display_listbox_stt(t.ToString() + "    Color table for this day is invalid");
            }
            else
            {
                if (t.Hour < 19 && t.Hour > 6)
                {
                    color_table = db.laybang("Select * FROM LineColorDay Where PlanId = '" + PlanId + "'" +"  order by LineName");
                }
                else//Neu buoi sang (6h den H19) -> thi lay du lieu cho buoi sang
                    color_table = db.laybang("Select * FROM LineColorNight Where [PlanId] = '" + PlanId + "'" + "  order by LineName");
            }
        }
        //ham cho phep thanh cuon tren giao dien luon o dong cuoi
        public delegate void dlg_autoscrll();
        public void autoScroll()
        {
            if (listStatus.InvokeRequired)
            {
                listStatus.Invoke(new dlg_autoscrll(autoScroll), new object[] { });
            }
            else
            {
                int visible = listStatus.ClientSize.Height / listStatus.ItemHeight;
                listStatus.TopIndex = Math.Max(listStatus.Items.Count - visible + 1, 0);
            }
        }

        private int countByHourBefore = 0;
        private int countByHourReal = 0;
        //Ham uy quyen set enable cac nut
        public delegate void Set_buttton(bool state);
        //START
        public void Start_enable(bool state)
        {
            if (btnStart.InvokeRequired)
            {
                btnStart.Invoke(new Set_buttton(Start_enable), new object[] { state });
            }
            else
            {
                if (state)
                {
                    btnStart.Enabled = true;
                }
                else { btnStart.Enabled = false; }
            }
        }
        //STOP
        public void Stop_enable(bool state)
        {
            if (btnStop.InvokeRequired)
            {
                btnStop.Invoke(new Set_buttton(Stop_enable), new object[] { state });
            }
            else
            {
                if (state)
                {
                    btnStop.Enabled = true;
                }
                else { btnStop.Enabled = false; }
            }
        }
        //Init
        public void Init_enable(bool state)
        {
            if (btnIntinalize.InvokeRequired)
            {
                btnIntinalize.Invoke(new Set_buttton(Init_enable), new object[] { state });
            }
            else
            {
                if (state)
                {
                    btnIntinalize.Enabled = true;
                }
                else { btnIntinalize.Enabled = false; }
            }
        }
        //RESET
        public void Reset_enable(bool state)
        {
            if (btnReset.InvokeRequired)
            {
                btnReset.Invoke(new Set_buttton(Reset_enable), new object[] { state });
            }
            else
            {
                if (state)
                {
                    btnReset.Enabled = true;
                }
                else { btnReset.Enabled = false; }
            }
        }

        //Uy quyen set status
        public delegate void set_status(string status);

        public void Exc_btnStart()
        {
            /*
            logstream = new StreamWriter(filelog.FullName, true, Encoding.Unicode);
            logstream.WriteLine(DateTime.Now.ToLongTimeString() + "   " + "click Start button");
            logstream.Close();
            */
            if (Main_thread_stt != true)
            {
                while (Main_thread_done != true)
                {
                    Thread.Sleep(100);
                }
                Thread Main_thread = new Thread(new ThreadStart(Main_thread_v2));
                Main_thread.IsBackground = true;
                Main_thread.Start();
            }
        } //xu li su kien an nut Start
        public void Exc_btnStop()
        {
            //thong bao cho main thread nghi neu dang hoat dong
            Main_thread_stt = false;
            // doi cho den khi no da hoan thanh
            while (Main_thread_done == false)
            {
                Console.WriteLine("Main_thread_done" + Main_thread_done.ToString());
                Main_thread_stt = false;
                Thread.Sleep(10);
            }
            //log
            /*
            logstream = new StreamWriter(filelog.FullName, true, Encoding.Unicode);
            logstream.WriteLine(DateTime.Now.ToLongTimeString() + "   " + "click Stop button");
            logstream.Close();
            */
            Thread stopThread = new Thread(new ThreadStart(stop_thread));
            stopThread.IsBackground = true;
            stopThread.Start();
        }// xu li su kien an nut Stop

        public void Exc_btnIntinalize() 
        {
            //thong bao cho main thread nghi neu dang hoat dong
            Main_thread_stt = false;
            // doi cho den khi no da hoan thanh
            while (Main_thread_done == false)
            {
                Thread.Sleep(10);
            }
            //ghi log
            /*
            logstream = new StreamWriter(filelog.FullName, true, Encoding.Unicode);
            logstream.WriteLine(DateTime.Now.ToLongTimeString() + "   " + "click Intinalize button");
            logstream.Close();
            */
            Thread initThread = new Thread(new ThreadStart(init_thread));
            initThread.IsBackground = true;
            initThread.Start();
        }//xu li su kien an nut Intinalize

        public void Exc_btnReset()
        {
            //thong bao cho main thread nghi neu dang hoat dong
            Main_thread_stt = false;
            // doi cho den khi no da hoan thanh
            while (Main_thread_done == false)
            {
                Thread.Sleep(50);
            }
            /*
            logstream = new StreamWriter(filelog.FullName, true, Encoding.Unicode);
            logstream.WriteLine(DateTime.Now.ToLongTimeString() + "   " + "click Reset button");
            logstream.Close();
            */
            DialogResult result = MessageBox.Show("Be careful, all data in today will be deleted", "Alart", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Display_listbox_stt(DateTime.Now.ToString() + ": Reset ......................");
                Thread resetThread = new Thread(new ThreadStart(reset_thread));
                resetThread.IsBackground = true;
                resetThread.Start();
            }

            mainThr_1st = true;
        }
        public void Exc_btnPing()
        {
            /*
            logstream = new StreamWriter(filelog.FullName, true, Encoding.Unicode);
            logstream.WriteLine(DateTime.Now.ToLongTimeString() + "   " + "click Ping button");
            logstream.Close();
            */
            ping_click = true;
            Display_listbox_stt(DateTime.Now.ToString() + ": Ping All ......................");
            Thread pingThread = new Thread(new ThreadStart(Ping_thread));
            pingThread.IsBackground = true;
            pingThread.Start();
        }
        public void Exc_allDetail()
        {
            // doi cho den khi no da hoan thanh
            if (Main_thread_stt == false)
            {
                _PlanId = db.findIdByDateTime(DateTime.Now);
            }
            FormAllDetai _FormAllDetail = new FormAllDetai(_PlanId);
            _FormAllDetail.Show();
        }
        public void Exc_auto_time_Click()
        {
            Time_auto.settime_auto(auto_startD_h, auto_startD_m, auto_startN_h, auto_startN_m);
            Time_auto.settime_autoEx(auto_expD_h, auto_expD_m, auto_expN_h, auto_expN_m);
            Time_auto frm_auto_time = new Time_auto();
            frm_auto_time.Show();
        }
        public void Exc_logoutToolStripMenuItem()
        {
            // Login frmLogin = new Login();
            //frmLogin.Show();
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
                for (int i = 0; i < 8; i++)
                {
                    if (MB[i] != null)
                    {
                        MB[i].Dispose();
                        MB[i] = null;
                    }
                }
                //ghi log
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
            this.Dispose();
        }
    }
}