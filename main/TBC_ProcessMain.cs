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
        #region khai bao chung
        // doi tuong load tam co so du lieu
        DataTable dt = new DataTable();
        DataTable color_table = new DataTable();
        // doi tuong chua cac ham xu ly database
        DBmanager db = new DBmanager();
        //dia chi cac PLC, duoc load tu database
        /*TabuchiFix*/
        //thêm 1 PLC mới
        string[] diachi = DungChung.diachiIP;
        string[] diachitmp = DungChung.diachiIP;
        private string timeStart = "00:00";
        private string timeFinish = "24:00";
        private string timeStockStart = "00:00";
        private string timeStockStop = "24:00";
        int stock_H_start = 0;
        int stock_M_start = 0;
        int stock_H_stop = 0;
        int stock_M_stop = 0;
        //thong bao ca sang hay ca toi
        bool DoN = true; //true -> ngay; false -> toi

        private DateTime streamingTimeS, streamingTimeF;
        private TimeSpan timeSR;
        private TimeSpan timeRF;
        bool[] plcStatus = { false, false, false, false, false, false, false, false, false, false, false, false };

        //bool c = regKey.
        //thoi gian bat dau ca sang 
        public static int auto_startD_h = 7;
        public static int auto_startD_m = 0;
        //thoi gian bat dau ca toi
        public static int auto_startN_h = 19;
        public static int auto_startN_m = 0;
        //thoi gian  xuat excel ca sang 
        public static int auto_expD_h = 18;
        public static int auto_expD_m = 35;
        //thoi gian xuat excel ca toi
        public static int auto_expN_h = 6;
        public static int auto_expN_m = 35;
        //gia tri ID cua database
        int _PlanId = 0;
        //co` thong bao tien trinh timer1 hoan tat
        bool Main_thread_done = true;
        //co` thong bao tien trinh manager Thread hoan tat
        bool Manager_thread_done = true;
        //Bien thong bao da autoset cho ca lam viec moi
        private static bool Auto_set_new_plan = true;
        //Bien thong bao cho main_thread sleep
        bool mainthr_sleep = false;
        //bien thong bao main_thread da sleep
        bool mainthr_sleeped = false;
        //dau hieu cho biet timer chay lan dau hoac de in mot so thong tin trong timer
        private static bool mainThr_1st = true;
        //bien cho biet da khoi tao log moi cua ngay hay chua (cho che do tu dong)
        //bool log_creat = false;
        //excel creat
        bool excel_creat = false;
        //bien ten cua logfile (ngay_thang_nam)
        //string log_name;
        //bien ten cua duong dan toi log file(log file tinh theo ngay)
        //doi tuong log
        //FileInfo filelog;
        //luong ghi log
        //StreamWriter logstream;
        #endregion

        Line_pr Line_base_on_data ;
        //trang thai cac thread
        bool Main_thread_stt = false;
        bool Manager_thread_stt = false;
        bool Ping_thread_stt = false;
        bool Reset_thread_stt = false;
        bool Stop_thread_stt = false;
        bool Init_thread_stt = false;
        bool First_connect_stt = true;// rieng bien nay se duoc set lai = false sau khi chay xong first_connect khi khoi tao form     
        bool SundayIsTrue = false;
        int targetReal;
        int resetDataAt7h = 0;

        public void Set_stattus(string status)
        {
            if (lbStatus.InvokeRequired)
            {
                lbStatus.Invoke(new set_status(Set_stattus), new object[] { status });
            }
            else
            {
                lbStatus.Text = "Status: " + status;
            }
        }
        private int ConvertInt(String str)
        {
            int i;
            if (!int.TryParse(str, out i))
            {
                i = 0;
            }
            return i;
        }
        private uint ConvertInt2_x1000(String str)
        {
            float i;
            if (!float.TryParse(str, out i))
            {
                i = 0;
            }
            return (uint)((i + 0.005) * 1000);
        }
        public string setColorCell(double rate)
        {
            Color color = new Color();
            if (rate >= 99)
            {
                color = Color.GreenYellow;
            }
            else if (rate < 99 && rate > 80)
            {
                color = Color.Yellow;
            }
            else
            {
                color = Color.Red;
            }
            return color.Name;
        }
        public double rateReal(double a, double b){
            if (b != 0)
                return a / b * 100;
            else return 0;
        }
        public DateTime changeHour(int hour){
            DateTime dtime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, 0, 0);
            return dtime;
        }

        
        private string ConvertTextTime(int hour){
            if (hour < 10)
            {
                return "0" + hour;
            }
            else
                return hour.ToString();
        }

        //Ham uy quyen set time tu form Time_auto:
        public delegate void dlgsettime(int dh, int dm, int nh, int nm);
        static public void settime_auto(int dh, int dm, int nh, int nm){
            //thoi gian bat dau ca sang 
            auto_startD_h = dh;
            auto_startD_m = dm;
            //thoi gian bat dau ca toi
            auto_startN_h = nh;
            auto_startN_m = nm;
            RegistryKey regKey = Registry.CurrentUser;
            RegistryKey regKey2 = regKey.CreateSubKey("Software\\ServerTabuchi\\Autostart time", RegistryKeyPermissionCheck.ReadWriteSubTree);
            regKey.Close();
            regKey2.SetValue("Day start h", dh);
            regKey2.SetValue("Day start m", dm);
            regKey2.SetValue("Night start h", nh);
            regKey2.SetValue("Night start m", nm);
            regKey2.Close();
        }
        static public void settime_autoEx(int dh, int dm, int nh, int nm){
            //thoi gian bat dau ca sang 
            auto_expD_h = dh;
            auto_expD_m = dm;
            //thoi gian bat dau ca toi
            auto_expN_h = nh;
            auto_expN_m = nm;
            RegistryKey regKey = Registry.CurrentUser;
            RegistryKey regKey2 = regKey.CreateSubKey("Software\\ServerTabuchi\\Auto Export time", RegistryKeyPermissionCheck.ReadWriteSubTree);
            regKey2.SetValue("Day Ex h", dh);
            regKey2.SetValue("Day Ex m", dm);
            regKey2.SetValue("Night Ex h", nh);
            regKey2.SetValue("Night Ex m", nm);
            regKey2.Close();
            regKey.Close();

        }
        dlgsettime settime = new dlgsettime(settime_auto);
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////CAC HAM XU LY CHINH
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Main_thread_v2()
        {
            //set giao dien
            Set_stattus("Run");
            Start_enable(false);
            Stop_enable(false);
            Reset_enable(false);
            Init_enable(false);

            while (First_connect_stt == true)
            {
                Thread.Sleep(100);
            }
            int countloop = 1;
            Main_thread_stt = true;
            mainThr_1st = true;
            bool Ping_one_in_minute = false;//bien thong bao de ping 1 lan
            while (Main_thread_stt)
            {
                countloop += 1;
                Main_thread_done = false;
                Display_listbox_stt("********************************--Main_thread_stt--****************************************");
                DateTime t = DateTime.Now;
                //kiem tra khoi tao log
                Creat_new_log(t);

                int PlanId = db.findIdByDateTime(t);
                _PlanId = PlanId;

                //cap nhat bang du lieu tu database
                try
                {
                    //bat loi chu nhat hoac ngay chua tao plan
                    var a = getTablefromDB(t, PlanId).Rows[0][0];
                    SundayIsTrue = false;
                }
                catch (Exception e)
                {
                    //neu la chu nhat thi khong lam gi ca
                    Display_listbox_stt("Error "+e.ToString());
                    SundayIsTrue = true;
                    Thread.Sleep(5000);
                }
                //Khoi tao ngay moi

                Auto_Reset_Init_v2(t, PlanId);
                #region ping
                if ((int)t.Minute % 2 == 0 && Ping_one_in_minute == true)
                {
                    Ping_one_in_minute = false;
                    Stop_enable(false);
                    countloop = 0;
                    plc_Ping();
                }
                if ((int)t.Minute % 2 != 0)
                {
                    Ping_one_in_minute = true;
                }
                #endregion

                #region ket noi kiem tra
                Stop_enable(true);

                if (countloop % 5 == 0)
                {
                    countloop = 0;
                    /*TabuchiFix*/
                    int ln_index = -1;
                    for (int i = 0; i < MB.Length; i++)//i < DungChung.SoPLC + 2
                    {
                        if (MB[i] != null)
                        {
                            if (MB[i].M_connected == false)
                            {
                                MB[i].Dispose();
                                MB[i] = null;
                            }
                        }
                        else
                        {
                            MB[i] = new ModbusTCP();
                            if (plcStatus[i] == true)
                            {
                                try
                                {
                                    Connect_MB(ref MB[i], diachi[i]);
                                    Thread.Sleep(500);
                                }
                                catch (Exception ex)
                                {
                                    if (ex is SocketException)
                                    {
                                        MB[i].M_connected = false;
                                    }
                                    MB[i].connect_done = true;
                                    Display_listbox_stt(DateTime.Now + " Error SocketException: " + ex.Message);
                                }
                            }
                            if (MB[i].M_connected == true)
                            {
                                //cho status = stop
                                if (i == DungChung.SoPLC)
                                {
                                    Display_listbox_stt(DateTime.Now + ":  Connect EDB Stock 2");
                                }
                                else if (i == DungChung.SoPLC+1)
                                {
                                    Display_listbox_stt(DateTime.Now + ":  Connect EDB Stock 1");
                                }
                                else if (i < DungChung.SoPLC)
                                {
                                    if (i * 7 + 7 > DungChung.soLine)
                                        Display_listbox_stt(DateTime.Now + ":  Connect EDB " + (i * 7 + 1) + " - " + DungChung.soLine);
                                    else Display_listbox_stt(DateTime.Now + ":  Connect EDB " + (i * 7 + 1) + " - " + (i * 7 + 7));
                                    //cho status = stop
                                    int[] status = new int[7] { 3, 3, 3, 3, 3, 3, 3 };
                                    MB_Write_Status(ref MB[i], status);
                                    Thread.Sleep(50);

                                    /*TabuchiFix*/
                                    #region//kiem tra lai reset va init < 5 o day neu khong duoc thi reset lai
                                    if (DungChung.reseted[i] == false || DungChung.initted[i] == false)
                                    {
                                        //gui lenh reset lai
                                        try
                                        {
                                            //code reset lai plc
                                            int[] tmp_reset = new int[] { 1, 1, 1, 1, 1, 1, 1 };
                                            MB_Write_Status(ref MB[i], tmp_reset);
                                            Thread.Sleep(50);
                                            Display_listbox_stt(t.ToString() + ":  Line " + (i * 7 + 1) + " to " + (i * 7 + 7) + " have been reseted again");
                                            // reset duoc thi gan bang true
                                            DungChung.reseted[i] = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            Display_listbox_stt(t.ToString() + " Error Reset Line: " + ex.Message);
                                        }

                                        #region cho stop truoc khi init
                                        int[] tmp_stop = new int[] { 3, 3, 3, 3, 3, 3, 3 };
                                        MB_Write_Status(ref MB[i], tmp_stop);//mang status cho 7 line trong 1 PLC
                                        Thread.Sleep(50);
                                        #endregion
                                        int[] tmp_plan = new int[7];//mang plan cho 7 line trong 1 PLC
                                        uint[] tmp_tact_time = new uint[7];
                                        #region cap nhat thong so tu database
                                        for (int j = 0; j < DungChung.L_PLC[i]; j++)
                                        {
                                            
                                            ln_index++;                                           
                                            //doc tact_time tu data base
                                            Line_base_on_data.Tact_time[ln_index] = ConvertInt2_x1000(dt.Rows[ln_index]["Tact_Time"].ToString());
                                            //uint.Parse(dt.Rows[ln_index]["Tact_Time"].ToString()) * 1000;
                                            tmp_tact_time[j] = Line_base_on_data.Tact_time[ln_index];
                                            //doc plan tu data_base
                                            Line_base_on_data.Plan[ln_index] = ConvertInt(dt.Rows[ln_index]["Plan"].ToString());
                                            tmp_plan[j] = Line_base_on_data.Plan[ln_index];
                                            Display_listbox_stt("Line: " + (ln_index + 1) + " Task Time: " + Line_base_on_data.Tact_time[ln_index] + ", Plan: " + Line_base_on_data.Plan[ln_index]);
                                        }
                                        #endregion
                                        //Ghi Plan va tact time vao PLC
                                        #region ghi vao 7 line lien tiep
                                        MB_Write_Plan(ref MB[i], tmp_plan);
                                        Thread.Sleep(50);
                                        MB_Write_Tact_time(ref MB[i], tmp_tact_time);
                                        Thread.Sleep(50);
                                        //fix-N
                                        if (i * 7 + 7 > DungChung.soLine)
                                            Display_listbox_stt(t.ToString() + ": Line " + (i * 7 + 1) + " - " + DungChung.soLine + " have been initilized");
                                        else Display_listbox_stt(t.ToString() + ":  Line " + (i * 7 + 1) + " to " + (i * 7 + 7) + " have been initilized");
                                        
                                        DungChung.initted[i] = true;
                                        #endregion

                                        //cho status = stop
                                        MB_Write_Status(ref MB[i], status);
                                        Thread.Sleep(50);
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                MB[i].Dispose(); MB[i] = null;
                            }
                        }
                    }
                }
                #endregion
                set_auto();////ghi thoi gian hoat dong hien hanh vao regisry neu ko ghi se tu dong reset lai chuong trinh
                // n LINE
                if (SundayIsTrue == false)
                {
                    
                    //Kiem tra intime
                    check_line_intime(t);
                    //Cap nhat cac gia tri tu database: plan, tact time, change actual?
                    //Neu co thay doi thi thuc hien thay doi cho EDB
                    Excute_if_change_param(t, PlanId);
                    //cap nhat lai target
                    Update_target(DateTime.Now);
                    //Cap nhat gia tri: status, plan, actual, target, diff, tacttime tu EDB
                    Update_from_EDB();
                    //Thong bao toi EDB trang thai, start, stop, idel
                    Update_work_status(t);
                    //luu vao database
                    Update_to_database(t, PlanId);
                    //update mau
                    update_clolor_all_hour(t, PlanId);
                    //set null cho o nao khong co mau
                    Set_null_for_white(t, PlanId);


                    //******************************************************************************************
                    // 2 STOCK
                    //check stock intime
                    check_stock_intime(t);
                    //update data from stock EDB
                    Update_stock_infor(t);
                    //thay doi plan va tactime
                    Excute_if_change_param_stock(t);
                    //Cap nhat trang thai hoat dong
                    Update_work_status_stock(t);
                    //sao chep thong so sang stocj 2
                    Update_to_Stock2();
                    //cap nhat data moi vao database
                    Update_to_database_stock(t, PlanId);
                    //quan ly thoi gian vong lap, che do sleep cua Main_thread
                    #region quan ly thoi gian vong lap, che do sleep cua Main_thread
                    mainThr_1st = false;
                    // dieu kien dung chuong trinh
                    #region thoat khoi main thread neu co yeu cau
                    Thread.Sleep(100);
                    if (Main_thread_stt == false)
                    {
                        Main_thread_done = true;
                        break;
                    }
                    #endregion
                    //cho phep main_thread ngu? cho den khi mainthr_sleep == false
                    #region cho phep sleep neu co yeu cau
                    while (mainthr_sleep == true)
                    {
                        mainthr_sleeped = true;
                        Thread.Sleep(35);
                    }
                    mainthr_sleeped = false;
                    #endregion
                    //doi cho du 5 s, neu nhieu hon 5 s thi bat dau vong lap moi
                    #region kiem soat thoi gian
                    TimeSpan Thr_number_seconds = DateTime.Now.Subtract(t);
                    while (Thr_number_seconds.TotalSeconds < 5.0)
                    {
                        if (Thr_number_seconds.TotalSeconds < 0)
                        {
                            Display_listbox_stt("FALSE TIME");
                            break;
                        }
                        if (Main_thread_stt == false)
                        {
                            Main_thread_done = true;
                            break;
                        }
                        Thread.Sleep(100);
                        Thr_number_seconds = DateTime.Now.Subtract(t);
                    }
                    if (Main_thread_stt == false)
                    {
                        Main_thread_done = true;
                        break;
                    }
                    //}
                    #endregion
                    Main_thread_done = true;
                    #endregion quan ly thoi gian vong lap, che do sleep cua Main_thread
                }
            }
        }
        //Kiem tra intime

        ///////////////////////////////////////////////////////       
        // ham thuc thi cua ping thread /*TabuchiFix*/
        private void Ping_thread()
        {
            Ping_thread_stt = true;
            Ping p = new Ping();
            for (int i = 0; i < DungChung.SoPLC+2; i++)
            {
                try
                {
                    //diachi[i] = diachitmp[i];
                    var builder = new StringBuilder();
                    var buffer = new byte[32];
                    var reply = p.Send(diachi[i], 1000, buffer, new PingOptions(600, false));
                    var error = reply.Status != IPStatus.Success || reply.RoundtripTime > 3000;
                    //ping thanh cong khi trang thai IP success va ...

                    if (ping_click == true)
                    {
                        if(DungChung.Enabel_PLC[i]==true)
                        Display_listbox_stt("Ping to : " + diachi[i] + "  " + reply.Status.ToString() + ",  " + reply.RoundtripTime + " ms");
                    }
                    else
                    {
                        if (reply.Status == IPStatus.Success)
                        {
                            plcStatus[i] = true;

                            if (DungChung.Enabel_PLC[i] == true)
                            Display_listbox_stt(DateTime.Now.ToString() + " Ping to : " + diachi[i] + "  " + reply.Status.ToString() + ",  " + reply.RoundtripTime + " ms");
                        }
                        else
                        {
                            plcStatus[i] = false;
                            if (DungChung.Enabel_PLC[i] == true)
                            Display_listbox_stt(DateTime.Now.ToString() + " Ping to : " + diachi[i] + "  " + reply.Status.ToString() + ",  " + reply.RoundtripTime + " ms");
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (DungChung.Enabel_PLC[i] == true)
                    Display_listbox_stt(DateTime.Now.ToString() + " Ping error: " + ex.Message);
                }
            }
            ping_click = false;
            Ping_thread_stt = false;
        }

        //ham thuc thi cua reset_thread - /*TabuchiFix*/
        private void reset_thread()
        {
            Set_stattus("Reset");
            Start_enable(false);
            Stop_enable(false);
            Reset_enable(false);
            Init_enable(false);
            //thong bao cho main thread nghi neu dang hoat dong
            Main_thread_stt = false;
            Reset_thread_stt = true;
            // doi cho den khi no da hoan thanh
            while (Main_thread_done == false || First_connect_stt == true || Ping_thread_stt == true || Stop_thread_stt == true || Init_thread_stt == true)
            {
                Thread.Sleep(50);
            }
            plc_Ping();
            DateTime t = DateTime.Now;
            int PlanId = db.findIdByDateTime(t);
            try
            {
                #region reset co so du lieu
                if (t.Hour < 19 && t.Hour > 6)
                {
                    db.xulydulieu("Update LineInputDay SET [Status]='-', [Archived_Rate]= 0, [Diff] = null, [Target] = null, [H07] = null, [H08] = null, [H09] = null, [H10] = null, [H11] = null, [H12] = null, [H13] = null, [H14] = null,[H15] = null, [H16] = null, [H17] = null, [H18] = null,[Actual]= null" +
                    " WHERE PlanId = '" + PlanId + "'");
                    db.xulydulieu("Update LineColorDay SET [Count07H] = 0,[Count08H] = 0,[Count09H] = 0,[Count10H] = 0,[Count11H] = 0,[Count12H] = 0,[Count13H] = 0,[Count14H] = 0,[Count15H] = 0,[Count16H] = 0,[Count17H] = 0,[Count18H] = 0" +
                    " WHERE PlanId = '" + PlanId + "'");
                    db.xulydulieu("Update LineColorDay SET [Color07H] = null,[Color08H] = null,[Color09H] = null,[Color10H] = null,[Color11H] = null,[Color12H] = null,[Color13H] = null,[Color14H] = null,[Color15H] = null,[Color16H] = null,[Color17H] = null,[Color18H] = null" +
                    " WHERE PlanId = '" + PlanId + "'");
                    //db.xulydulieu("DELETE FROM LineColorDay" +
                    //" WHERE PlanId = '" + PlanId + "'");
                    //db.xulydulieu("INSERT INTO LineColorDay(LineId, LineName, PlanId) SELECT LineId, LineName, PlanId FROM LineInputDay" +
                    //" WHERE PlanId = '" + PlanId + "';");
                    Display_listbox_stt(t.ToString() + ":  Database for Day session has been reseted");
                }
                else
                {
                    db.xulydulieu("Update LineInputNight SET [Status]='-', [Archived_Rate]= 0, [Diff] = null, [Target] = null,[H19] = null, [H20] = null, [H21] = null, [H22] = null, [H23] = null, [H00] = null, [H01] = null, [H02] = null,[H03] = null, [H04] = null, [H05] = null, [H06] = null,[Actual]= null" +
                    " WHERE PlanId = '" + PlanId + "'");
                    db.xulydulieu("Update LineColorNight SET [Count19H] = 0,[Count20H] = 0,[Count21H] = 0,[Count22H] = 0,[Count23H] = 0,[Count00H] = 0,[Count01H] = 0,[Count02H] = 0,[Count03H] = 0,[Count04H] = 0,[Count05H] = 0,[Count06H] = 0," +
                    " WHERE PlanId = '" + PlanId + "'");
                    db.xulydulieu("Update LineColorNight SET [Color19H] = NULL,[Color20H] = NULL,[Color21H] = NULL,[Color22H] = NULL,[Color23H] = NULL,[Color00H] = NULL,[Color01H] = NULL,[Color02H] = NULL,[Color03H] = NULL,[Color04H] = NULL,[Color05H] = NULL,[Color06H] = NULL" +
                    " WHERE PlanId = '" + PlanId + "'");
                    //db.xulydulieu("DELETE FROM LineColorNight" +
                    //" WHERE PlanId = '" + PlanId + "'");
                    // db.xulydulieu("INSERT INTO LineColorNight(LineId, LineName, PlanId) SELECT LineId, LineName, PlanId FROM LineInputNight" +
                    //" WHERE PlanId = '" + PlanId + "';");
                    Display_listbox_stt(t.ToString() + ":  Database for Night session has been reseted");
                }
                #endregion
            }
            catch (Exception ex)
            {
                Display_listbox_stt(t.ToString() + ", Error Reset database: " + ex.Message);
            }
            /*TabuchiFix*/
            //bat dau reset den PLC
            for (int coutplc = 0; coutplc < DungChung.SoPLC; coutplc++)
            {
                
                #region reset 7 EDB 1 lan
                if ((plcStatus[coutplc] == true) && (MB[coutplc] != null))
                {
                    if (MB[coutplc].connected == true)
                    {
                        try
                        {
                            //code reset lai plc
                            int[] tmp_reset = new int[] { 1, 1, 1, 1, 1, 1, 1 };
                            MB_Write_Status(ref MB[coutplc], tmp_reset);
                            Thread.Sleep(50);
                            Display_listbox_stt(t.ToString() + ": PLC " + (coutplc +1) + " have been reseted");                           
                            // reset duoc thi gan bang true
                            DungChung.reseted[coutplc] = true;
                        }
                        catch (Exception ex)
                        {
                            Display_listbox_stt(t.ToString() + " Error Reset Line: " + ex.Message);
                        }
                    }
                }
                #endregion
            }
            //reset stock neu duoc phep
            #region reset EDB stock 1
            if ((plcStatus[DungChung.SoPLC] == true) && (MB[DungChung.SoPLC] != null))
            {
                if (MB[DungChung.SoPLC].connected == true)
                {
                    try
                    {
                        MB_Write_stock_Status(ref MB[DungChung.SoPLC], 1);
                        Thread.Sleep(50);
                        Display_listbox_stt_insert(t.ToString() + ":  Stocking 1 has been reseted");
                    }
                    catch (Exception ex)
                    {
                        Display_listbox_stt(t.ToString() + ":  Error Reset Stock 1: " + ex.Message);
                    }
                }
            }
            #endregion

            Display_listbox_stt(t.ToString() + ":  Reset all done. ");
            mainThr_1st = true;
            Reset_thread_stt = false;
            Start_enable(false);
            Stop_enable(false);
            Reset_enable(true);
            Init_enable(true);
        }

        //ham thuc thi cua stop_thread- không sửa
        private void stop_thread()
        {
            Set_stattus("Stop");
            Start_enable(false);
            Stop_enable(false);
            Reset_enable(false);
            Init_enable(false);
            //thong bao cho main thread nghi neu dang hoat dong
            Main_thread_stt = false;
            Stop_thread_stt = true;
            // doi cho den khi no da hoan thanh
            while (Main_thread_done == false || Ping_thread_stt == true || Reset_thread_stt == true || Init_thread_stt == true)
            {
                Thread.Sleep(50);
            }
            plc_Ping();
            DateTime t = DateTime.Now;

            for (int coutplc = 0; coutplc < DungChung.SoPLC; coutplc++)
            {
                
                if ((plcStatus[coutplc] == true) && (MB[coutplc] != null))
                {
                    if (MB[coutplc].connected == true)
                    {
                        try
                        {
                            int[] tmp_reset = new int[] { 3, 3, 3, 3, 3, 3, 3 };
                            MB_Write_Status(ref MB[coutplc], tmp_reset);
                            Thread.Sleep(50);                      
                            Display_listbox_stt(t.ToString() + ":  PLC " + (coutplc +1) + " have been stoped");                      
                        }
                        catch (Exception ex)
                        {
                            Display_listbox_stt(t.ToString() + ":  Error Stop Line: " + ex.Message);
                        }
                    }
                }
            }

            #region stop stock 1
            if ((plcStatus[DungChung.SoPLC] == true) && (MB[DungChung.SoPLC] != null))
            {
                try
                {
                    MB_Write_stock_Status(ref MB[DungChung.SoPLC], 3);
                    Thread.Sleep(50);
                    Display_listbox_stt_insert(t.ToString() + ":  Stocking 1 has been stoped");
                }
                catch (Exception ex)
                {
                    Display_listbox_stt(t.ToString() + ":  Error stoped Stock 1: " + ex.Message);
                }
            }
            #endregion
            #region stop stock 2
            if ((plcStatus[DungChung.SoPLC + 1] == true) && (MB[DungChung.SoPLC + 1].connected == true))
            {
                try
                {
                    MB_Write_stock_Status(ref MB[DungChung.SoPLC + 1], 3);
                    Thread.Sleep(50);
                    Display_listbox_stt_insert(t.ToString() + ":  Stocking 2 has been stoped");
                }
                catch (Exception ex)
                {
                    Display_listbox_stt(t.ToString() + ":  Error stoped Stock 2: " + ex.Message);
                }
            }
            #endregion

            Display_listbox_stt(DateTime.Now.ToString() + ":  All stop ......................");
            //set co` bao trang thai nut bam
            Stop_thread_stt = false;
            mainThr_1st = false;

            Start_enable(true);
            Stop_enable(true);
            Reset_enable(true);
            Init_enable(true);
        }

        //ham thuc thi cua init_thread- /*TabuchiFix*/
        private void init_thread()
        {
            Set_stattus("Initialize");
            Start_enable(false);
            Stop_enable(false);
            Reset_enable(false);
            Init_enable(false);
            ///thong bao cho main thread nghi neu dang hoat dong
            Main_thread_stt = false;
            // doi cho den khi no da hoan thanh
            Init_thread_stt = true;
            // doi cho den khi no da hoan thanh
            while (Main_thread_done == false || Ping_thread_stt == true || Reset_thread_stt == true || Stop_thread_stt == true)
            {
                Thread.Sleep(50);
            }
            DateTime t = DateTime.Now;
            //Ping truoc khi init
            plc_Ping();
            //truoc khi lay du lieu bang database, thuc hien xoa du lieu
            int PlanId = db.findIdByDateTime(t);
            getTablefromDB(t, PlanId);//toa moi dt tu databse
            bool No_PLC = true;
            Display_listbox_stt(t.ToString() + ": Initilize .......................");

            /*TabuchiFix*/
            //lay plan va tact time cho  line
            #region init n line
            int ln_index = -1;
            for (int i = 0; i < DungChung.SoPLC; i++)
            {
              try
                {
                    if ((plcStatus[i] == true) && (MB[i] != null)) //Lam gi voi PLC thi cung cang PING OK
                    {
                        if (MB[i].M_connected == true)
                        {
                            No_PLC = false;
                            #region cho stop truoc khi init
                            int[] tmp_stop = new int[] { 3, 3, 3, 3, 3, 3, 3 };
                            MB_Write_Status(ref MB[i], tmp_stop);//mang status cho 7 line trong 1 PLC
                            Thread.Sleep(50);
                            #endregion
                            int[] tmp_plan = new int[7];//mang plan cho 7 line trong 1 PLC
                            uint[] tmp_tact_time = new uint[7];
                            #region cap nhat thong so tu database
                            for (int j = 0; j < DungChung.L_PLC[i]; j++)
                            {

                                ln_index += 1;
                                //doc tact_time tu data base
                                Line_base_on_data.Tact_time[ln_index] = ConvertInt2_x1000(dt.Rows[ln_index]["Tact_Time"].ToString());
                                tmp_tact_time[j] = Line_base_on_data.Tact_time[ln_index];
                                //doc plan tu data_base
                                Line_base_on_data.Plan[ln_index] = ConvertInt(dt.Rows[ln_index]["Plan"].ToString());
                                tmp_plan[j] = Line_base_on_data.Plan[ln_index];
                                Display_listbox_stt("Line: " + (ln_index + 1) + " Task Time: " + Line_base_on_data.Tact_time[ln_index] + ", Plan: " + Line_base_on_data.Plan[ln_index]);
                            }
                            #endregion
                            //Ghi Plan va tact time vao PLC
                            #region ghi vao 7 line lien tiep
                            MB_Write_Plan(ref MB[i], tmp_plan);
                            Thread.Sleep(50);
                            MB_Write_Tact_time(ref MB[i], tmp_tact_time);
                            Thread.Sleep(50);
                            Display_listbox_stt(t.ToString() + ":  PLC " + (i + 1) + " have been initilized");
                            DungChung.initted[i] = true;
                            #endregion
                        }
                        else
                        {
                            ln_index += DungChung.L_PLC[i];
                        }
                    }
                    else {
                        ln_index += DungChung.L_PLC[i];
                    }
                }
                #region exception
                catch (Exception ex)
                {
                    ln_index += DungChung.L_PLC[i];
                    Display_listbox_stt(t.ToString() + ":  Init Lin err:   " + ex.Message);
                }
                #endregion
            }
            #endregion

            //Init va stop stock 1 neu ping OK-/*TabuchiFix*/
            #region init stock 1
            if ((plcStatus[DungChung.SoPLC] == true) && (MB[DungChung.SoPLC].connected == true))
            {
                try
                {
                    No_PLC = false;
                    #region cho stock stop
                    MB_Write_stock_Status(ref MB[DungChung.SoPLC], 3);
                    Thread.Sleep(50);
                    #endregion
                    #region cap nhat thong tin tu database cua stock
                    Line_base_on_data.Tact_time_Stock[1] = (uint)ConvertInt2_x1000(dt.Rows[DungChung.soLine]["Tact_Time"].ToString());
                    Line_base_on_data.Plan_Stock[1] = (uint)ConvertInt(dt.Rows[DungChung.soLine]["Plan"].ToString());
                    #endregion
                    //Ghi pl va tt cho stock
                    #region Init cho stock 1
                    MB_Write_stock_Plan(ref MB[DungChung.SoPLC], Line_base_on_data.Plan_Stock[1]);
                    Thread.Sleep(50);
                    MB_Write_stock_Tact_time(ref MB[DungChung.SoPLC], Line_base_on_data.Tact_time_Stock[1]);
                    Thread.Sleep(50);
                    Display_listbox_stt(t.ToString() + ":  Stock 1 has been initiliazed");
                    #endregion
                }
                #region Exception
                catch (Exception ex)
                {
                    Display_listbox_stt(t.ToString() + ":  Init stock1 err:   " + ex.Message);
                }
                #endregion
            }
            #endregion
            //Init va stop stock 2 neu ping OK-không thay đổi gì
            #region init stock 2
            if ((plcStatus[DungChung.SoPLC + 1] == true) && (MB[DungChung.SoPLC+1].connected == true))
            {
                try
                {
                    #region cho stock 2 stop
                    MB_Write_stock_Status(ref MB[DungChung.SoPLC+1], 3);
                    Thread.Sleep(50);
                    #endregion
                    #region thong so cho stock 2 (giong stock 1)
                    Line_base_on_data.Tact_time_Stock[0] = Line_base_on_data.Tact_time_Stock[1];
                    Line_base_on_data.Plan_Stock[0] = Line_base_on_data.Plan_Stock[1];
                    #endregion
                    #region init thong so cho stock 2
                    MB_Write_stock_Plan(ref MB[DungChung.SoPLC+1], Line_base_on_data.Plan_Stock[0]);
                    Thread.Sleep(50);
                    MB_Write_stock_Tact_time(ref MB[DungChung.SoPLC+1], Line_base_on_data.Tact_time_Stock[0]);
                    Thread.Sleep(50);
                    Display_listbox_stt(t.ToString() + " Sotck 2 has been initialized");
                    #endregion

                }
                #region Exception
                catch (Exception ex)
                {
                    Display_listbox_stt(t.ToString() + ":  Init stock2 err:   " + ex.Message);
                }
                #endregion
            }
            #endregion

            #region neu khong co line nao duoc init
            if (No_PLC == true)
            {
                Display_listbox_stt(t.ToString() + ":  There are 0 line has been initalized.");
            }
            #endregion

            Display_listbox_stt(t.ToString() + ":  Initializing has been done");
            mainThr_1st = true;// neu click start thi no se cho in ra trang thai
            Start_enable(true);
            Stop_enable(false);
            Reset_enable(true);
            Init_enable(true);
            Init_thread_stt = false;
        }

        //ham thuc thi  Threadcheck trang thai tat ca cac line--/*TabuchiFix*/
        private void First_connect()
        {
            First_connect_stt = true;
            //ping truoc
            plc_Ping();
            //lay plan va tact time cho  line
            Display_listbox_stt(DateTime.Now.ToString() + " Try connect to PLC");
            for (int i = 0; i < DungChung.SoPLC; i++)
            {
                #region ket noi cac PLC neu chua co ket noi
                if (plcStatus[i] == true)
                {
                    MB[i] = new ModbusTCP();
                    if (MB[i].M_connected == false)
                    {
                        try { Connect_MB(ref MB[i], diachi[i]); }// HAM KET NOI
                        catch (Exception ex)
                        {
                            if (ex is SocketException)
                            {
                                MB[i].M_connected = false;
                            }
                            MB[i].connect_done = true;
                            Display_listbox_stt(DateTime.Now + " " + ex.Message);
                        }

                        //xoa doi tuong ket noi neu ko ket noi duoc
                        if (MB[i].M_connected == false)
                        {
                            MB[i].Dispose();
                            MB[i] = null;
                        }
                        else
                        {
                            Thread.Sleep(50);
                            if (i < DungChung.SoPLC)
                            {
                                Display_listbox_stt(DateTime.Now + ":  Connect PLC "+i);
                            }
                            if (i == (DungChung.SoPLC +1))
                            {
                                Display_listbox_stt(DateTime.Now + ":  Connect PLC"+i+" Stock 2");
                            }
                            if (i == DungChung.SoPLC)
                            {
                                Display_listbox_stt(DateTime.Now + ":  Connect PLC"+i+" Stock 1");
                            }
                        }
                    }
                #endregion
                    try
                    {
                        #region cap nhat truc tiep thong tin tu tren cac EDB
                        if ((plcStatus[i] == true) && (MB[i] != null)) //Lam gi voi PLC thi cung can PING OK
                        {                  
                                MB_Read_Status(ref MB[i]);
                                Thread.Sleep(50);
                                MB_Read_Plan(ref MB[i]);
                                Thread.Sleep(50);
                                MB_Read_Target(ref MB[i]);
                                Thread.Sleep(50);
                                MB_Read_Actual(ref MB[i]);
                                Thread.Sleep(50);
                                MB_Read_Tact_time(ref MB[i]);
                                Thread.Sleep(50);

                                for (int k = 0; k < DungChung.L_PLC[i]; k++)
                                {
                                    MB[i].Diff[k] = MB[i].Actual[k] = MB[i].Target[k];
                                }
                        }
                        #endregion
                    }
                    #region exception
                    catch (Exception ex)
                    {
                        Display_listbox_stt(DateTime.Now.ToString() + ":  Check Line err:   " + ex.Message);
                    }
                    #endregion
                }
            }

            //doc gia tri truc tiep tu stock 2 neu ping OK- không sửa
            #region doc gia tri truc tiep tu stock 2
            if ((plcStatus[DungChung.SoPLC+1] == true) && (MB[DungChung.SoPLC+1] != null))
            {
                try
                {
                    MB_Read_stock_Status(ref MB[DungChung.SoPLC + 1]);
                    Thread.Sleep(50);
                    MB_Read_stock_Plan(ref MB[DungChung.SoPLC + 1]);
                    Thread.Sleep(50);
                    MB_Read_stock_Tact_time(ref MB[DungChung.SoPLC + 1]);
                    Thread.Sleep(50);
                    MB_Read_stock_Target(ref MB[DungChung.SoPLC + 1]);
                    Thread.Sleep(50);
                    MB_Read_stock_Actual(ref MB[DungChung.SoPLC + 1]);
                    Thread.Sleep(50);
                    MB[DungChung.SoPLC + 1].Diff_Stock[0] = (int)MB[DungChung.SoPLC + 1].Actual_Stock[0] - (int)MB[DungChung.SoPLC + 1].Target_Stock[0];
                }
                catch (Exception ex)
                {
                    Display_listbox_stt(DateTime.Now.ToString() + ":  Check Plan stock 2 err:   " + ex.Message);
                }
            }
            #endregion

            //Init va stop stock 1 neu ping OK- không sửa
            #region doc gia tri truc tiep tu stock 1
            if ((plcStatus[DungChung.SoPLC] == true) && (MB[DungChung.SoPLC] != null))
            {
                try
                {
                    MB_Read_stock_Status(ref MB[DungChung.SoPLC]);
                    Thread.Sleep(50);
                    MB_Read_stock_Plan(ref MB[DungChung.SoPLC]);
                    Thread.Sleep(50);
                    MB_Read_stock_Tact_time(ref MB[DungChung.SoPLC]);
                    Thread.Sleep(50);
                    MB_Read_stock_Target(ref MB[DungChung.SoPLC]);
                    Thread.Sleep(50);
                    MB_Read_stock_Actual(ref MB[DungChung.SoPLC]);
                    Thread.Sleep(50);
                    MB[DungChung.SoPLC].Diff_Stock[0] = (int)MB[DungChung.SoPLC].Actual_Stock[0] - (int)MB[DungChung.SoPLC].Target_Stock[0];
                }
                catch (Exception ex)
                {
                    Display_listbox_stt(DateTime.Now.ToString() + ":  Check plan stock 1 err:   " + ex.Message);
                }
            }
            #endregion
            First_connect_stt = false;
        }

        //Ham Ping
        public void plc_Ping()
        {
            Ping_thread();
        }

        //tự động khởi tạo ca làm việc-không sửa
        public void Auto_Reset_Init_v2(DateTime t, int PlanId)
        {
            //xoa khi PLC van con >2000 actual
            if (t.Hour == 7 && resetDataAt7h == 1)
            {
                Auto_set_new_plan = false;
                Display_listbox_stt(t.ToString() + ":   It's Day session....");
                /*TabuchiFix_20160607*/
                for (int i = 0; i < 5; i++)
                {
                    Display_listbox_stt("Resetinit Day lan thu " + (i + 1).ToString());//reset va init lai 3 lan cho chac :))
                    Reset_CSDL_DauCa(t, PlanId);
                    Reset_init_PLC(t, PlanId);
                    Thread.Sleep(5000);
                }
                resetDataAt7h = 2;//da reset xong
            }
            if (t.Hour != 7)
            {
                resetDataAt7h = 0;
            }

            //tu dong khoi tao cho ca sang
            #region tu dong tao ca sang
            if (t.Hour == auto_startD_h && t.Minute == auto_startD_m && Auto_set_new_plan == true)
            {
                Auto_set_new_plan = false;
                Display_listbox_stt(t.ToString() + ":   It's Day session....");
                /*TabuchiFix_20160607*/
                for (int i = 0; i < 5; i++)
                {
                    Display_listbox_stt("Resetinit Day lan thu "+ (i+1).ToString() );//reset va init lai 3 lan cho chac :))
                    Reset_CSDL_DauCa(t, PlanId);
                    Reset_init_PLC(t, PlanId);
                    Thread.Sleep(5000);
                }
            }
            #endregion
            //tu dong khoi tao cho ca toi
            #region tu khoi tao ca toi
            if (t.Hour == auto_startN_h && t.Minute == auto_startN_m && Auto_set_new_plan == true)
            {
                Auto_set_new_plan = false;
                Display_listbox_stt(t.ToString() + ":   It's Night session....");

                /*TabuchiFix_20160607*/
                for (int i = 0; i < 5; i++)
                {
                    Display_listbox_stt("Resetinit Day lan thu " + (i + 1).ToString());
                    Reset_init_PLC(t, PlanId);
                    Thread.Sleep(5000);
                }
            }
            #endregion
            //tu dong xoa khi ket thuc ca toi
            #region tu dong xoa khi ket thuc ca toi
            if (t.Hour == 6 && t.Minute == 45 && Auto_set_new_plan == true)
            {
                Auto_set_new_plan = false;
                Display_listbox_stt(t.ToString() + ":   Reset Cuoi ca Dem");
                /*TabuchiFix_20160607*/
                for (int i = 0; i < 5; i++)
                {
                    Display_listbox_stt("Resetinit Day lan thu " + (i + 1).ToString());//reset va init lai 3 lan cho chac :))
                    Reset_init_PLC(t, PlanId);
                    Thread.Sleep(5000);
                }
            }
            #endregion
            // tu dong xoa khi ket thuc ca sang
            #region tu dong xoa khi ket thuc ca sang
            if (t.Hour == 18 && t.Minute == 45 && Auto_set_new_plan == true)
            {
                Auto_set_new_plan = false;
                Display_listbox_stt(t.ToString() + ":   Reset Cuoi ca Sang");
                /*TabuchiFix_20160607*/
                for (int i = 0; i < 5; i++)
                {
                    Display_listbox_stt("Resetinit Day lan thu " + (i + 1).ToString());//reset va init lai 3 lan cho chac :))
                    Reset_init_PLC(t, PlanId);
                    Thread.Sleep(5000);
                }
            }
            #endregion

            if (t.Hour != auto_startD_h && t.Minute != auto_startD_m || t.Hour != auto_startN_h && t.Minute != auto_startN_m || t.Hour != 6 && t.Minute != 45 || t.Hour != 18 && t.Minute != 45)
            {
                Auto_set_new_plan = true;
            }

        }

        //moi them vao de reset co so du lieu dau ca
        public void Reset_CSDL_DauCa(DateTime t, int PlanId)
        {
            try
            {//không sửa
                #region reset co so du lieu
                if (t.Hour < 19 && t.Hour > 6)
                {
                    db.xulydulieu("Update LineInputDay SET [Status]='-', [Archived_Rate]= 0, [Diff] = null, [Target] = null, [H07] = null, [H08] = null, [H09] = null, [H10] = null, [H11] = null, [H12] = null, [H13] = null, [H14] = null,[H15] = null, [H16] = null, [H17] = null, [H18] = null,[Actual]= null" +
                    " WHERE PlanId = '" + PlanId + "'");
                    db.xulydulieu("Update LineColorDay SET [Count07H] = 0,[Count08H] = 0,[Count09H] = 0,[Count10H] = 0,[Count11H] = 0,[Count12H] = 0,[Count13H] = 0,[Count14H] = 0,[Count15H] = 0,[Count16H] = 0,[Count17H] = 0,[Count18H] = 0" +
                    " WHERE PlanId = '" + PlanId + "'");
                    db.xulydulieu("Update LineColorDay SET [Color07H] = null,[Color08H] = null,[Color09H] = null,[Color10H] = null,[Color11H] = null,[Color12H] = null,[Color13H] = null,[Color14H] = null,[Color15H] = null,[Color16H] = null,[Color17H] = null,[Color18H] = null" +
                    " WHERE PlanId = '" + PlanId + "'");
                    //db.xulydulieu("DELETE FROM LineColorDay" +
                    //" WHERE PlanId = '" + PlanId + "'");
                    //db.xulydulieu("INSERT INTO LineColorDay(LineId, LineName, PlanId) SELECT LineId, LineName, PlanId FROM LineInputDay" +
                    //" WHERE PlanId = '" + PlanId + "';");
                    Display_listbox_stt(t.ToString() + ":  Database for Day session has been reseted");
                }
                else
                {
                    db.xulydulieu("Update LineInputNight SET [Status]='-', [Archived_Rate]= 0, [Diff] = null, [Target] = null,[H19] = null, [H20] = null, [H21] = null, [H22] = null, [H23] = null, [H00] = null, [H01] = null, [H02] = null,[H03] = null, [H04] = null, [H05] = null, [H06] = null,[Actual]= null" +
                    " WHERE PlanId = '" + PlanId + "'");
                    db.xulydulieu("Update LineColorNight SET [Count19H] = 0,[Count20H] = 0,[Count21H] = 0,[Count22H] = 0,[Count23H] = 0,[Count00H] = 0,[Count01H] = 0,[Count02H] = 0,[Count03H] = 0,[Count04H] = 0,[Count05H] = 0,[Count06H] = 0" +
                    " WHERE PlanId = '" + PlanId + "'");
                    db.xulydulieu("Update LineColorNight SET [Color19H] = null,[Color20H] = null,[Color21H] = null,[Color22H] = null,[Color23H] = null,[Color00H] = null,[Color01H] = null,[Color02H] = null,[Color03H] = null,[Color04H] = null,[Color05H] = null,[Color06H] = null" +
                    " WHERE PlanId = '" + PlanId + "'");
                    //db.xulydulieu("DELETE FROM LineColorNight" +
                    //" WHERE PlanId = '" + PlanId + "'");
                    //db.xulydulieu("INSERT INTO LineColorNight(LineId, LineName, PlanId) SELECT LineId, LineName, PlanId FROM LineInputNight" +
                    //" WHERE PlanId = '" + PlanId + "';");
                    Display_listbox_stt(t.ToString() + ":  Database for Night session has been reseted");
                }
                #endregion
            }
            catch (Exception ex)
            {
                Display_listbox_stt(t.ToString() + ", Error Reset database: " + ex.Message);
            }

        }
        public void Reset_init_PLC(DateTime t, int PlanId)//reset va init tren PLC
        {
            plc_Ping();
            for (int i = 0; i < DungChung.SoPLC+2; i++) {
                DungChung.initted[i] = false;
                DungChung.reseted[i] = false;
            }
            //RESET
            ////////////////////////////////////////////////////
            #region reset
            Display_listbox_stt(t.ToString() + ": Reset .......................");
            //reset các line 
            for (int coutplc = 0; coutplc < DungChung.SoPLC; coutplc++)
            {
                #region reset 7 EDB 1 lan
                
                if ((plcStatus[coutplc] == true) && (MB[coutplc] != null))
                {
                    if (MB[coutplc].connected == true)
                    {
                        try
                        {
                            int[] tmp_reset = new int[] { 1, 1, 1, 1, 1, 1, 1 };
                            MB_Write_Status(ref MB[coutplc], tmp_reset);
                            Thread.Sleep(150);
                            
                            Display_listbox_stt(t.ToString() + ":  PLC " + (coutplc + 1)  + " have been reseted");
                          
                            DungChung.reseted[coutplc] = true;
                        }
                        catch (Exception ex)
                        {
                            Display_listbox_stt(t.ToString() + " Error Reset PLC: "+(coutplc+1)+"  " + ex.Message);
                        }
                    }
                }
                #endregion
            }

            //reset stock neu duoc phep

            #region reset EDB stock 1
            if ((plcStatus[DungChung.SoPLC] == true) && (MB[DungChung.SoPLC] != null))
            {
                if (MB[DungChung.SoPLC].connected == true)
                {
                    try
                    {
                        MB_Write_stock_Status(ref MB[DungChung.SoPLC], 1);
                        Thread.Sleep(150);
                        DungChung.reseted[DungChung.SoPLC] = true;
                        Display_listbox_stt_insert(t.ToString() + ":  Stocking 1 has been reseted");
                    }
                    catch (Exception ex)
                    {
                        Display_listbox_stt(t.ToString() + ":  Error Reset Stock 1: " + ex.Message);
                    }
                }
            }
            #endregion
            #region reset EDB stock 2
            if ((plcStatus[DungChung.SoPLC + 1] == true) && (MB[DungChung.SoPLC + 1] != null))
            {
                if (MB[DungChung.SoPLC + 1].connected == true)
                {
                    try
                    {
                        MB_Write_stock_Status(ref MB[DungChung.SoPLC + 1], 1);
                        Thread.Sleep(150);
                        DungChung.reseted[DungChung.SoPLC + 1] = true;
                        Display_listbox_stt_insert(t.ToString() + ":  Stocking 2 has been reseted");
                    }
                    catch (Exception ex)
                    {
                        Display_listbox_stt(t.ToString() + ":  Error Reset Stock 2: " + ex.Message);
                    }
                }
            }
            #endregion

            Display_listbox_stt(t.ToString() + ":  Reset all done. ");
            #endregion
            //INIT
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            #region init
            bool No_PLC = true;
            Display_listbox_stt(t.ToString() + ": Initilize .......................");

            //lay plan va tact time cho n line - /*TabuchiFix*/
            #region init soline
            int ln_index = -1;
            for (int i = 0; i < DungChung.SoPLC; i++)
            {
                try
                {
                    if ((plcStatus[i] == true) && (MB[i] != null)) //Lam gi voi PLC thi cung cang PING OK
                    {
                        if (MB[i].M_connected == true)
                        {
                            No_PLC = false;
                            #region cho stop truoc khi init
                            int[] tmp_stop = new int[] { 3, 3, 3, 3, 3, 3, 3 };
                            MB_Write_Status(ref MB[i], tmp_stop);//mang status cho 7 line trong 1 PLC
                            Thread.Sleep(50);
                            #endregion
                            int[] tmp_plan = new int[7];//mang plan cho 7 line trong 1 PLC
                            uint[] tmp_tact_time = new uint[7];
                            #region cap nhat thong so tu database
                            for (int j = 0; j < DungChung.L_PLC[i]; j++)
                            {

                                ln_index++;
                                //doc tact_time tu data base
                                Line_base_on_data.Tact_time[ln_index] = ConvertInt2_x1000(dt.Rows[ln_index]["Tact_Time"].ToString());
                                //uint.Parse(dt.Rows[ln_index]["Tact_Time"].ToString()) * 1000;
                                tmp_tact_time[j] = Line_base_on_data.Tact_time[ln_index];
                                //doc plan tu data_base
                                Line_base_on_data.Plan[ln_index] = ConvertInt(dt.Rows[ln_index]["Plan"].ToString());
                                tmp_plan[j] = Line_base_on_data.Plan[ln_index];
                                Display_listbox_stt("Line: " + (ln_index + 1) + " Task Time: " + Line_base_on_data.Tact_time[ln_index] + ", Plan: " + Line_base_on_data.Plan[ln_index]);
                            }
                            #endregion
                            //Ghi Plan va tact time vao PLC
                            #region ghi vao 7 line lien tiep
                            MB_Write_Plan(ref MB[i], tmp_plan);
                            Thread.Sleep(50);
                            MB_Write_Tact_time(ref MB[i], tmp_tact_time);
                            Thread.Sleep(50);
                            if (i * i + 7 < DungChung.soLine) //eo quan trong lam nen  bo qua mac du hoi sai
                                Display_listbox_stt(t.ToString() + ":  Line " + (i * 7 + 1) + " to " + (i * 7 + 7) + " have been initilized");
                            else Display_listbox_stt(t.ToString() + ":  Line " + (i * 7 + 1) + " to " + DungChung.soLine + " have been initilized");

                            DungChung.initted[i] = true;
                            #endregion
                        }
                        else
                        {
                            ln_index += DungChung.L_PLC[i];
                        }
                    }
                    else {
                        ln_index += DungChung.L_PLC[i];
                    }
                }
                #region exception
                catch (Exception ex)
                {
                    ln_index += DungChung.L_PLC[i];
                    Display_listbox_stt(t.ToString() + ":  Init Line err:   " + ex.Message);
                }
                #endregion
            }
            #endregion

            //Init va stop stock 1 neu ping OK- /*TabuchiFix*/
            #region init stock 1
            if ((plcStatus[DungChung.SoPLC] == true) && (MB[DungChung.SoPLC].connected == true))
            {
                try
                {
                    No_PLC = false;
                    #region cho stock stop
                    MB_Write_stock_Status(ref MB[DungChung.SoPLC], 3);
                    Thread.Sleep(50);
                    #endregion
                    #region cap nhat thong tin tu database cua stock
                    //line cuoi DungChung.soLine la line danh cho stock
                    Line_base_on_data.Tact_time_Stock[1] = (uint)ConvertInt2_x1000(dt.Rows[DungChung.soLine]["Tact_Time"].ToString());
                    Line_base_on_data.Plan_Stock[1] = (uint)ConvertInt(dt.Rows[DungChung.soLine]["Plan"].ToString());
                    #endregion
                    //Ghi pl va tt cho stock
                    #region Init cho stock 1
                    MB_Write_stock_Plan(ref MB[DungChung.SoPLC], Line_base_on_data.Plan_Stock[1]);
                    Thread.Sleep(50);
                    MB_Write_stock_Tact_time(ref MB[DungChung.SoPLC], Line_base_on_data.Tact_time_Stock[1]);
                    Thread.Sleep(50);
                    DungChung.initted[DungChung.SoPLC] = true;
                    Display_listbox_stt(t.ToString() + ":  Stock 1 has been initiliazed");
                    #endregion
                }
                #region Exception
                catch (Exception ex)
                {
                    Display_listbox_stt(t.ToString() + ":  Init stock1 err:   " + ex.Message);
                }
                #endregion
            }
            #endregion

            //Init va stop stock 2 neu ping OK
            #region init stock 2
            if ((plcStatus[DungChung.SoPLC + 1] == true) && (MB[DungChung.SoPLC+1].connected == true))
            {
                try
                {
                    #region cho stock 2 stop
                    MB_Write_stock_Status(ref MB[DungChung.SoPLC + 1], 3);
                    Thread.Sleep(50);
                    #endregion
                    #region thong so cho stock 2 (giong stock 1)
                    Line_base_on_data.Tact_time_Stock[0] = Line_base_on_data.Tact_time_Stock[1];
                    Line_base_on_data.Plan_Stock[0] = Line_base_on_data.Plan_Stock[1];
                    #endregion
                    #region init thong so cho stock 2
                    MB_Write_stock_Plan(ref MB[DungChung.SoPLC + 1], Line_base_on_data.Plan_Stock[0]);
                    Thread.Sleep(50);
                    MB_Write_stock_Tact_time(ref MB[DungChung.SoPLC + 1], Line_base_on_data.Tact_time_Stock[0]);
                    Thread.Sleep(50);
                    DungChung.initted[DungChung.SoPLC + 1] = true;
                    Display_listbox_stt(t.ToString() + " Sotck 2 has been initialized");
                    #endregion

                }
                #region Exception
                catch (Exception ex)
                {
                    Display_listbox_stt(t.ToString() + ":  Init stock2 err:   " + ex.Message);
                }
                #endregion
            }
            #endregion

            #region neu khong co line nao duoc init
            if (No_PLC == true)
            {
                //listStatus.Items.Add("There are 0 line has been initalized.");
                Display_listbox_stt(t.ToString() + ":  There are 0 line has been initalized.");
            }
            #endregion
            Display_listbox_stt(t.ToString() + ":  Initializing has been done");
            
            #endregion
        }

        //ham cap nhat lai 
        //ham thuc hien tao log- không sửa
        public void Creat_new_log(DateTime t)
        {
            /*
            #region khoi tao log cho ngay moi
            if (t.Hour == 0 && t.Minute == 0)
            {
                if (log_creat == false)
                {
                    log_creat = true;
                    var path = System.Reflection.Assembly.GetExecutingAssembly().Location;// lay duong dan +  chuong trinh
                    //var name = System.IO.Path.GetFileName(path); //lay ten chuong trinh
                    string path_folder = Path.GetDirectoryName(path);// lay duong dan
                    //File.Delete(path);
                    path_folder += "\\log";
                    //kiem tra folder log co ton tai hay ko
                    DirectoryInfo directoryLog = new DirectoryInfo(path_folder);
                    if (!directoryLog.Exists)
                    {
                        //neu ko ton tai thi tao folder log
                        directoryLog.Create();
                    }
                    //khoi tao log file moi -> ten log file = ngay_thang_nam
                    //
                    if (t.Day < 10)
                    {
                        log_name = "0" + t.Day.ToString() + "_";
                    }
                    else { log_name = t.Day.ToString() + "_"; }
                    if (t.Month < 10)
                    {
                        log_name += "0" + t.Month.ToString() + "_";
                    }
                    else { log_name += t.Month.ToString() + "_"; }
                    log_name += t.Year.ToString();

                    string log_path = path_folder + "\\" + log_name.ToString() + ".log";
                    
                    filelog = new FileInfo(log_path);
                    //khoi tao neu ko ton tai
                    if (!filelog.Exists)
                    {
                        //ghi ngay thang nam, danh dau h
                        logstream = new StreamWriter(filelog.FullName, true, Encoding.Unicode);//append -> nhay den cuoi dong va mo file hoac tao file
                        logstream.WriteLine("Log file has been created at:  " + t.ToString());
                        logstream.Close();
                    }
                    mainthr_sleep = false;// thong bao cho main_thread tiep tuc chay
                }
            }
            #endregion
            //cho phep khoi tao log cho ngay moi neu vao H23 58'
            if (t.Hour != 0 && t.Minute != 0)
            {
                log_creat = false;
            }
             */
        }
        //ham thuc hien xuat excel- không sửa
        public void Excel_export()
        {
            
            if ((DateTime.Now.Hour == auto_expD_h && DateTime.Now.Minute == auto_expD_m) || (DateTime.Now.Hour == auto_expN_h && DateTime.Now.Minute == auto_expN_m))
            {
                if (Main_thread_stt == false)
                {
                    _PlanId = db.findIdByDateTime(DateTime.Now);
                }
                if (excel_creat == true)
                {

                    excel_creat = false;
                    FormAllDetai _FormAllDetail = new FormAllDetai(_PlanId);
                    _FormAllDetail.Show();
                }
            }
            else { excel_creat = true; }
            
        }
        //ham quan ly chung- không sửa
        public void Manager_thread()
        {
            Manager_thread_stt = true;
            while (Manager_thread_stt)
            {
                Manager_thread_done = false;
                DateTime t = DateTime.Now;
                //hien thi thoi gian
                Display_time();
                TimeSpan delT = DateTime.Now.Subtract(t);
                double DeltaT = delT.TotalMilliseconds;
                if (Manager_thread_stt == false)
                {
                    Manager_thread_done = true;
                    break;
                }
                while (DeltaT < 200.0)
                {
                    if (DeltaT < 0)
                    {
                        break;
                    }
                    Thread.Sleep(20);
                    delT = DateTime.Now.Subtract(t);
                    DeltaT = delT.TotalMilliseconds;
                }
                Manager_thread_done = true;
            }
        }
    }
}