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
        public void loadData_stock(int[] actual, int total_act, int diff, int target, DateTime t, int PlanId)
        {
            #region cap nhat actual tung khung H cho ca sang
            if (t.Hour < 19 && t.Hour > 6)
            {
                for (int i = 7; i <= t.Hour; i++)
                {
                    string H = "";
                    if (i < 10)
                    {
                        H = "[H0" + i.ToString() + "]";
                    }
                    else { H = "[H" + i.ToString() + "]"; }
                    db.xulydulieu("Update LineInputDay set " + H + " = '" + actual[i - 7] + "' where LineName='" + (DungChung.soLine+1) + "' and PlanId = '" + PlanId + "'");
                }
            }
            #endregion
            else
            {
                #region cap nhat actual cho tung khung H cho ca dem truong hop ca dem qua 0 H
                if (t.Hour < 7)
                {
                    for (int i = 19; i <= 23; i++)
                    {
                        string H = "[H" + i.ToString() + "]";
                        db.xulydulieu("Update LineInputNight set " + H + " = '" + actual[i - 19] + "' where LineName='" + (DungChung.soLine + 1) + "' and PlanId = '" + PlanId + "'");
                    }
                    for (int i = 0; i <= t.Hour; i++)
                    {
                        if (i <= t.Hour)
                        {
                            string H = "[H0" + i.ToString() + "]";
                            db.xulydulieu("Update LineInputNight set " + H + " = '" + actual[i + 5] + "' where LineName='" + (DungChung.soLine + 1) + "' and PlanId = '" + PlanId + "'");
                        }
                        else
                        {
                            string H = "[H0" + i.ToString() + "]";
                            db.xulydulieu("Update LineInputNight set " + H + " = null where LineName='" + (DungChung.soLine + 1) + "' and PlanId = '" + PlanId + "'");
                        }

                    }
                }
                #endregion
                else
                {
                    #region cap nhat actual cho tung khung H trong ca dem neu khung H <= 23H
                    for (int i = 19; i <= 23; i++)
                    {
                        if (i <= t.Hour)
                        {
                            string H = "[H" + i.ToString() + "]";
                            db.xulydulieu("Update LineInputNight set " + H + " = '" + actual[i - 19] + "' where LineName='" + (DungChung.soLine + 1) + "' and PlanId = '" + PlanId + "'");
                        }
                        else
                        {
                            string H = "[H" + i.ToString() + "]";
                            db.xulydulieu("Update LineInputNight set " + H + " = null where LineName='" + (DungChung.soLine + 1) + "' and PlanId = '" + PlanId + "'");
                        }
                    }
                    for (int i = 0; i <= 7; i++)
                    {
                        string H = "[H0" + i.ToString() + "]";
                        db.xulydulieu("Update LineInputNight set " + H + " = null where LineName='" + (DungChung.soLine + 1) + "' and PlanId = '" + PlanId + "'");
                    }
                    #endregion
                }
            }

            //cap nhat RAte
            #region cap nhat Rate
            double rate = 0;

            if (target != 0)
            {
                rate = total_act * 100.0 / (target * 1.0);
                if (t.Hour > 6 && t.Hour < 19)
                {
                    db.xulydulieu("Update LineInputDay set Archived_Rate= '" + rate + "' where LineName ='" + (DungChung.soLine + 1) + "' and PlanId = '" + PlanId + "'");
                }
                else
                {
                    db.xulydulieu("Update LineInputNight set Archived_Rate= '" + rate + "' where LineName ='" + (DungChung.soLine + 1) + "' and PlanId = '" + PlanId + "'");
                }
            }
            #endregion
            //cap nhat actual tong 
            #region cap nhat actual tong
            if (t.Hour > 6 && t.Hour < 19)
            {
                db.xulydulieu("Update LineInputDay set Actual= '" + total_act + "' where LineName ='" + (DungChung.soLine + 1) + "' and PlanId = '" + PlanId + "'");
            }
            else
            {
                db.xulydulieu("Update LineInputNight set Actual = '" + total_act + "' where LineName ='" + (DungChung.soLine + 1) + "' and PlanId = '" + PlanId + "'");
            }
            #endregion
            //cap nhat Diff
            #region cap nhat diff
            if (t.Hour > 6 && t.Hour < 19)
            {
                db.xulydulieu("Update LineInputDay set Diff= '" + diff + "' where LineName ='" + (DungChung.soLine + 1) + "' and PlanId = '" + PlanId + "'");
            }
            else
            {
                db.xulydulieu("Update LineInputNight set Diff = '" + diff + "' where LineName ='" + (DungChung.soLine + 1) + "' and PlanId = '" + PlanId + "'");
            }
            #endregion
            //cap nhat target
            #region cap nhat target
            if (target == 0) return;
            if (t.Hour > 6 && t.Hour < 19)
            {
                db.xulydulieu("Update LineInputDay set Target = '" + target + "' where LineName ='" + (DungChung.soLine + 1) + "' and PlanId = '" + PlanId + "'");
            }
            else
            {
                db.xulydulieu("Update LineInputNight set Target = '" + target + "' where LineName ='" + (DungChung.soLine + 1) + "' and PlanId = '" + PlanId + "'");
            }
            #endregion
        }

        //////////////////////////////////////////////////STOCK
        //Kiem tra stock co trong thoi gian lam viec ko
        public void check_stock_intime(DateTime t)
        {
            int time_minute2 = (t.Hour * 60) + t.Minute;
            //dt tu dong load ca lam viec sang hay toi nen thoi gian bat dau tu dong lay sang hoac toi
            timeStockStart = dt.Rows[(DungChung.soLine)]["From1"].ToString();
            timeStockStop = dt.Rows[(DungChung.soLine)]["To1"].ToString();
            // timeStockStart.to
            for (int i = 2; i < 6; i++)
            {
                string Colum = "To" + i.ToString();
                string time_tmp = timeStockStop;
                time_tmp = dt.Rows[DungChung.soLine][Colum].ToString();
                if (time_tmp.Equals(""))
                {
                    //timeStockStop
                    break;
                }
                timeStockStop = time_tmp;
            }
            stock_H_start = ConvertInt(timeStockStart.Substring(0, 2));
            stock_M_start = ConvertInt(timeStockStart.Substring(3, 2));
            stock_H_stop = ConvertInt(timeStockStop.Substring(0, 2));
            stock_M_stop = ConvertInt(timeStockStop.Substring(3, 2));
            // stock_H_start = int.Parse(timeStockStart.Substring(0, 2));
            if ((stock_H_start * 60 + stock_M_start) < (stock_H_stop * 60 + stock_M_stop))
            {
                DoN = true;// ca ngay
            }
            else
            {
                DoN = false;//ca dem 
            }
            // neu la ca ngay
            if (DoN == true)
            {
                //kiem tra stock_intime
                if ((time_minute2 >= (stock_H_start * 60 + stock_M_start)) && (time_minute2 <= (stock_H_stop * 60 + stock_M_stop)))
                {
                    //Line.In_time_Stock[1] = true;
                    Line_base_on_data.In_time_Stock[1] = true;
                }
            }

            else //neu la ca dem
            {
                if ((time_minute2 >= (stock_H_start * 60 + stock_M_start)) || (time_minute2 <= (stock_H_stop * 60 + stock_M_stop)))
                {
                    Line_base_on_data.In_time_Stock[1] = true;
                }
                else
                {
                    //Line.In_time_Stock[1] = false;
                    Line_base_on_data.In_time_Stock[1] = false;
                }
            }
        }
        //cap nhat Target, actual,... cho stock (tu PLC hoac EDB)
        public void Update_stock_infor(DateTime t)
        {
            if (toolStripMenuItem1.Checked == false)// neu khong su dung che do doc stock tu database thi thuc hien doc stock tu PLC
            {
                if (plcStatus[DungChung.SoPLC] == true && (MB[DungChung.SoPLC] != null))
                {
                    if (MB[DungChung.SoPLC].M_connected == true)
                    {
                        //doc cac thong so tu PLC stock
                        MB_Read_stock_Status(ref MB[DungChung.SoPLC]);
                        Thread.Sleep(25);
                        MB_Read_stock_Plan(ref MB[DungChung.SoPLC]);
                        Thread.Sleep(25);
                        MB_Read_stock_Target(ref MB[DungChung.SoPLC]);
                        Thread.Sleep(25);
                        MB_Read_stock_Actual(ref MB[DungChung.SoPLC]);
                        Thread.Sleep(25);
                        MB_Read_stock_Tact_time(ref MB[DungChung.SoPLC]);
                        Thread.Sleep(25);
                        MB[DungChung.SoPLC].Diff_Stock[0] = (int)MB[DungChung.SoPLC].Actual_Stock[0] - (int)MB[DungChung.SoPLC].Target_Stock[0];

                        Line_base_on_data.Status_Stock[1] = MB[DungChung.SoPLC].Status_Stock[0];
                        Line_base_on_data.Plan_Stock[1] = MB[DungChung.SoPLC].Plan_Stock[0];
                        Line_base_on_data.Target_Stock[1] = MB[DungChung.SoPLC].Target_Stock[0];
                        Line_base_on_data.Actual_Stock[1] = MB[DungChung.SoPLC].Actual_Stock[0];
                        Line_base_on_data.Tact_time_Stock[1] = MB[DungChung.SoPLC].Tact_time_Stock[0];
                        Line_base_on_data.Diff_Stock[1] = MB[DungChung.SoPLC].Diff_Stock[0];
                    }
                    else { Display_listbox_stt("Connection is lost: " + diachi[DungChung.SoPLC]); }
                }
                else
                {
                    Display_listbox_stt("It does not connect to PLC: " + diachi[DungChung.SoPLC]);
                }
            }
            else// Neu su dung viec doc actual tu databse trung gian thi thuc hien cac tac vu ben duoi
            {
                //cap nhat cac thong so ve plan, actual, tacttime, target, diff..
                update_target_actual_diff_stock(t);

                if (MB[DungChung.SoPLC] != null)
                {
                    if (MB[DungChung.SoPLC].M_connected == true)
                    {
                        // Cho bang dien tu vao trang thai stop
                        //moi thong tin deu duoc tinh toan cap nhat tu server
                        Line_base_on_data.Status_Stock[1] = 2;
                        MB_Write_stock_Status(ref MB[DungChung.SoPLC], Line_base_on_data.Status_Stock[1]);
                        Thread.Sleep(25);
                        MB_Write_stock_Plan(ref MB[DungChung.SoPLC], (uint)Line_base_on_data.Plan_Stock[1]);//cap nhat Plan
                        Thread.Sleep(25);
                        MB_Write_stock_Target(ref MB[DungChung.SoPLC], (uint)Line_base_on_data.Target_Stock[1]);//cap nhat target
                        Thread.Sleep(25);
                        MB_Write_stock_Actual(ref MB[DungChung.SoPLC], (uint)Line_base_on_data.Actual_Stock[1]);//cap nhat actual
                        Thread.Sleep(25);
                        MB_Write_stock_Tact_time(ref MB[DungChung.SoPLC], (uint)Line_base_on_data.Tact_time_Stock[1]);//cap nhat tact time
                        Thread.Sleep(25);
                        MB_Write_stock_Diff(ref MB[DungChung.SoPLC], (uint)Line_base_on_data.Diff_Stock[1]);//cap nhat diff 
                        Thread.Sleep(25);

                        MB[DungChung.SoPLC].Status_Stock[0] = Line_base_on_data.Status_Stock[1];
                        MB[DungChung.SoPLC].Plan_Stock[0] = Line_base_on_data.Plan_Stock[1];
                        MB[DungChung.SoPLC].Target_Stock[0] = Line_base_on_data.Target_Stock[1];
                        MB[DungChung.SoPLC].Actual_Stock[0] = Line_base_on_data.Actual_Stock[1];
                        MB[DungChung.SoPLC].Tact_time_Stock[0] = Line_base_on_data.Tact_time_Stock[1];
                        MB[DungChung.SoPLC].Diff_Stock[0] = Line_base_on_data.Diff_Stock[1];
                    }
                }
            }
        }
        //thuc hien xu ly neu co thay doi plan va tact time cho stock
        public void Excute_if_change_param_stock(DateTime t)
        {
            if (toolStripMenuItem1.Checked == false)
            {
                #region cap nhat tu database
                Line_base_on_data.Plan_Stock[1] = (uint)ConvertInt(dt.Rows[DungChung.soLine]["Plan"].ToString());
                Line_base_on_data.Tact_time_Stock[1] = ConvertInt2_x1000(dt.Rows[DungChung.soLine]["Tact_Time"].ToString());
                #endregion
                if (MB[DungChung.SoPLC] != null)
                {
                    if ((Line_base_on_data.Plan_Stock[1] != MB[DungChung.SoPLC].Plan_Stock[0]) || (Line_base_on_data.Tact_time_Stock[1] != MB[DungChung.SoPLC].Tact_time_Stock[0]))
                    {
                        if (plcStatus[DungChung.SoPLC] == true && MB[DungChung.SoPLC].M_connected == true)
                        {
                            //Ghi lai plan va tact time cho EDB
                            MB_Write_stock_Plan(ref MB[DungChung.SoPLC], Line_base_on_data.Plan_Stock[1]);
                            Thread.Sleep(25);
                            MB_Write_stock_Tact_time(ref MB[DungChung.SoPLC], Line_base_on_data.Tact_time_Stock[1]);
                            Thread.Sleep(25);
                            Display_listbox_stt(t.ToString() + " Plan or Tasktime of Stock 1 has been changed");
                            //sao chep plan va tact time sang stock 2
                            if (MB[DungChung.SoPLC+1] != null)
                            {
                                if ((plcStatus[DungChung.SoPLC + 1] == true) && (MB[DungChung.SoPLC+1].M_connected == true))
                                {
                                    MB_Write_stock_Plan(ref MB[DungChung.SoPLC + 1], Line_base_on_data.Plan_Stock[1]);
                                    Thread.Sleep(25);
                                    MB_Write_stock_Tact_time(ref MB[DungChung.SoPLC + 1], Line_base_on_data.Tact_time_Stock[1]);
                                    Thread.Sleep(25);
                                    Display_listbox_stt(t.ToString() + " Plan or Tasktime of Stock 2 has been changed");
                                }
                                else { Display_listbox_stt(t.ToString() + " Can not change plan or tact time for stock 2, connection may be lost"); }
                            }
                        }
                        else { Display_listbox_stt(t.ToString() + " Can not change plan or tact time for stock 1, connection may be lost"); }
                    }
                }
            }
        }
        public void Update_work_status_stock(DateTime t)
        {
            if (toolStripMenuItem1.Checked == false)
            {
                if (MB[DungChung.SoPLC] != null)
                {
                    if (Line_base_on_data.In_time_Stock[1] == true)
                    {
                        if ((MB[DungChung.SoPLC].Status_Stock[0] != 2) || (mainThr_1st == true))
                        {
                            MB_Write_stock_Status(ref MB[DungChung.SoPLC], 2);
                            Thread.Sleep(25);
                            Display_listbox_stt(t.ToString() + ": Stock 1 has been started");
                            // Cho EDB stock 2 hoat dong
                            if (MB[DungChung.SoPLC+1] != null)
                            {
                                if ((plcStatus[DungChung.SoPLC + 1] == true) && MB[DungChung.SoPLC+1].M_connected != true)
                                {
                                    MB_Write_stock_Status(ref MB[DungChung.SoPLC+1], 2);
                                    Thread.Sleep(25);
                                    Display_listbox_stt(t.ToString() + ": Stock 2 has been started");
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((MB[DungChung.SoPLC].Status_Stock[0] != 3) || (mainThr_1st == true))
                        {
                            MB_Write_stock_Status(ref MB[DungChung.SoPLC], 3);
                            Thread.Sleep(25);
                            Display_listbox_stt(t.ToString() + ": Stock 1 has been stoped");
                            // Cho EDB stock 2 hoat dong
                            if (MB[DungChung.SoPLC+1] != null)
                            {
                                if ((plcStatus[DungChung.SoPLC + 1] == true) && MB[DungChung.SoPLC+1].M_connected != true)
                                {
                                    MB_Write_stock_Status(ref MB[DungChung.SoPLC+1], 3);
                                    Thread.Sleep(25);
                                    Display_listbox_stt(t.ToString() + ": Stock 2 has been stoped");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (MB[DungChung.SoPLC] != null && plcStatus[DungChung.SoPLC] == true)
                {
                    if (MB[DungChung.SoPLC].M_connected == true)
                    {
                        if (mainThr_1st == true)
                        {
                            //cho EDB vao trang thai stop neu he thong cap nhat stock tu database
                            MB_Write_stock_Status(ref MB[DungChung.SoPLC], 3);
                            Thread.Sleep(25);
                            MB_Write_stock_Status(ref MB[DungChung.SoPLC+1], 3);
                            Thread.Sleep(25);
                        }
                    }
                }
            }
        }
        //cap nhat cac thong so cho bang dien tu stock so 2
        public void Update_to_Stock2()
        {
            if (MB[DungChung.SoPLC + 1] != null && (plcStatus[DungChung.SoPLC+1] == true))
            {
                MB_Write_stock_Status(ref MB[DungChung.SoPLC + 1], Line_base_on_data.Status_Stock[1]);
                Thread.Sleep(25);
                MB_Write_stock_Plan(ref MB[DungChung.SoPLC + 1], Line_base_on_data.Plan_Stock[1]);
                Thread.Sleep(25);
                MB_Write_stock_Actual(ref MB[DungChung.SoPLC + 1], Line_base_on_data.Actual_Stock[1]);
                Thread.Sleep(25);
                MB_Write_stock_Target(ref MB[DungChung.SoPLC + 1], Line_base_on_data.Target_Stock[1]);
                Thread.Sleep(25);
                MB_Write_stock_Tact_time(ref MB[DungChung.SoPLC + 1], Line_base_on_data.Tact_time_Stock[1]);
                Thread.Sleep(25);
                MB_Write_stock_Diff(ref MB[DungChung.SoPLC + 1], (uint)Line_base_on_data.Diff_Stock[1]);
                Thread.Sleep(25);
            }
        }
        //cap nhat cac thong so vao database tong(InputDay hoac InputNight)
        public void Update_to_database_stock(DateTime t, int PlanId)
        {
            if (toolStripMenuItem1.Checked == false)
            {
                #region su dung phuong phap cap nhat tu PLC
                if (MB[DungChung.SoPLC] != null)
                {
                    if (Line_base_on_data.In_time_Stock[1] == true)
                    {

                        if (t.Hour < 19 && t.Hour > 6)
                        {
                            db.xulydulieu("UPDATE LineInputDay SET Actual='" + (MB[DungChung.SoPLC].Actual_Stock[0]).ToString() + "',Diff='" + (MB[DungChung.SoPLC].Diff_Stock[0]).ToString() + "', Target='" + MB[DungChung.SoPLC].Target_Stock[0] + "', [Status]='Run' WHERE LineName = '" + (DungChung.soLine+1) + "' and PlanId = '" + PlanId + "'");
                            loadData((DungChung.soLine + 1), (int)MB[DungChung.SoPLC].Actual_Stock[0], (int)MB[DungChung.SoPLC].Diff_Stock[0], (int)MB[DungChung.SoPLC].Target_Stock[0], dt, t, PlanId);
                            loadColorByHour(((DungChung.soLine + 1)), (int)MB[DungChung.SoPLC].Actual_Stock[0], dt.Rows[DungChung.soLine]["Model"].ToString(), t.Hour, PlanId, t);
                        }
                        else
                        {
                            db.xulydulieu("UPDATE LineInputNight SET Actual='" + (MB[DungChung.SoPLC].Actual_Stock[0]).ToString() + "',Diff='" + (MB[DungChung.SoPLC].Diff_Stock[0]).ToString() + "', Target='" + MB[DungChung.SoPLC].Target_Stock[0] + "', [Status]='Run' WHERE LineName = '" + (DungChung.soLine + 1) + "'and PlanId = '" + PlanId + "'");
                            loadData((DungChung.soLine + 1), (int)MB[(DungChung.soLine + 1)].Actual_Stock[0], (int)MB[(DungChung.soLine + 1)].Diff_Stock[0], (int)MB[(DungChung.soLine + 1)].Target_Stock[0], dt, t, PlanId);
                            loadColorByHour((DungChung.soLine + 1), (int)MB[(DungChung.soLine + 1)].Actual_Stock[0], dt.Rows[DungChung.soLine]["Model"].ToString(), t.Hour, PlanId, t);
                        }
                    }
                    else
                    {
                        if (t.Hour < 19 && t.Hour > 6)
                        {
                            db.xulydulieu("Update LineInputDay SET [Status]='Stop' WHERE LineName = '" + (DungChung.soLine+1) + "'and PlanId = '" + PlanId + "'");
                        }
                        else
                        {
                            db.xulydulieu("Update LineInputNight SET [Status]='Stop' WHERE LineName = '" + (DungChung.soLine + 1) + "'and PlanId = '" + PlanId + "'");
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region su dung phuong phap cap nhat  tu database trung gian
                if (Line_base_on_data.In_time_Stock[1] == true) //neu stock In time
                {
                    int[] tmp_H_actual_stock = new int[12];
                    for (int i = 0; i < 12; i++)
                    {
                        tmp_H_actual_stock[i] = (int)Line_base_on_data.H_Actual_Stock[1, i];
                    }
                    //cap nhat actual cho tung khung H
                    loadData_stock(tmp_H_actual_stock, (int)Line_base_on_data.Actual_Stock[1], (int)Line_base_on_data.Diff_Stock[1], (int)Line_base_on_data.Target_Stock[1], t, PlanId);
                    //Cap nhat mau cho tung khung H
                    update_color_by_hour_for_stock(t, PlanId);
                #endregion
                }
                else
                //neu stock ko in time thi in thong bao no dang stop
                #region thong bao stop
                {
                    if (t.Hour < 19 && t.Hour > 6)
                    {
                        db.xulydulieu("Update LineInputDay SET [Status]='Stop' WHERE LineName = '" + (DungChung.soLine + 1) + "'and PlanId = '" + PlanId + "'");
                    }
                    else
                    {
                        db.xulydulieu("Update LineInputNight SET [Status]='Stop' WHERE LineName = '" + (DungChung.soLine + 1) + "'and PlanId = '" + PlanId + "'");
                    }
                }
                #endregion
            }
        }
        //ham tinh lai target, actual, diff cho stock
        public void update_target_actual_diff_stock(DateTime Now)//tact time tinh theo 1000 ms -> 
        {
            //tinh tong thoi gian
            #region tinh tong thoi gian
            timeStockStart = dt.Rows[DungChung.soLine]["From1"].ToString();
            timeStockStop = dt.Rows[DungChung.soLine]["To1"].ToString();
            // timeStockStart.to
            for (int i = 2; i < 6; i++)
            {
                string Colum = "To" + i.ToString();
                string time_tmp = timeStockStop;
                time_tmp = dt.Rows[DungChung.soLine][Colum].ToString();
                if (time_tmp.Equals(""))
                {
                    //timeStockStop
                    break;
                }
                timeStockStop = time_tmp;
            }
            DateTime time_start = DateTime.ParseExact(timeStockStart, "HH:mm", System.Globalization.CultureInfo.CurrentCulture);
            DateTime time_stop = DateTime.ParseExact(timeStockStop, "HH:mm", System.Globalization.CultureInfo.CurrentCulture);

            int total_minute = 0;
            int target_stock = 0;
            if (Now.Hour <= 23)
            {
                if (time_stop.Hour < 7)
                { time_stop = time_stop.AddDays(1); }
            }
            if (Now.Hour < 7)
            {
                if (time_start.Hour > 7 && time_stop.Hour < 7)
                { time_start = time_start.AddDays(-1); }

                total_minute = (int)time_stop.Subtract(time_start).TotalMinutes;
            }
            else { total_minute = (int)time_stop.Subtract(time_start).TotalMinutes; }
            #endregion
            //tinh lai tact_time
            Line_base_on_data.Plan_Stock[1] = (uint)(ConvertInt(dt.Rows[DungChung.soLine]["Plan"].ToString())); //lay ra plan cua stock trong database
            int tact_time_stock = (int)(total_minute * 60000.0 / (Line_base_on_data.Plan_Stock[1] * 1.0) + 0.0005);
            Line_base_on_data.Tact_time_Stock[1] = (uint)tact_time_stock;
            //cap nhat actual tu database (cap nhat tu bang stock)
            //Line_base_on_data.Actual_Stock[1] = (uint)db.Get_stocking_byHour(Now);
            int[] tmpactual_stock = new int[12];
            db.Get_stocking_all_H(Now, ref tmpactual_stock);
            Line_base_on_data.Actual_Stock[1] = 0;
            for (int i = 0; i < 12; i++)
            {
                Line_base_on_data.H_Actual_Stock[1, i] = (uint)tmpactual_stock[i];
                Line_base_on_data.Actual_Stock[1] += Line_base_on_data.H_Actual_Stock[1, i];
            }
            //tinh lai target
            int sec_to_now = (int)Now.Subtract(time_start).TotalSeconds;
            target_stock = (int)(sec_to_now * 1000.0 / (tact_time_stock * 1.0) + 0.5);
            Line_base_on_data.Target_Stock[1] = (uint)target_stock;
            if (Line_base_on_data.Target_Stock[1] > Line_base_on_data.Plan_Stock[1])//neu target > plan - > target = plan
            {
                Line_base_on_data.Target_Stock[1] = Line_base_on_data.Plan_Stock[1];
            }
            //tinh difference
            Line_base_on_data.Diff_Stock[1] = (int)(Line_base_on_data.Actual_Stock[1] - Line_base_on_data.Target_Stock[1]);
        }
        //ham cap nhat color cho stock (cap nhat mau cho tat ca khung H den thoi diem hien tai)
        public void update_color_by_hour_for_stock(DateTime t, int PlanId)
        {
            //tinh thoi gian stock da hoat dong
            #region tinh thoi gian da hoat dong
            int Work_sec = 0;
            string str_time_start = "";
            DateTime time_start;
            for (int i = 1; i < 6; i++)
            {
                string tmp_index = "From" + i.ToString();
                if (!tmp_index.Equals(""))
                {
                    str_time_start = dt.Rows[i - 1][tmp_index].ToString();
                    break;
                }
            }
            time_start = DateTime.ParseExact(str_time_start, "HH:mm", System.Globalization.CultureInfo.CurrentCulture);
            if (t.Hour >= 7)
            {
                Work_sec = (int)t.Subtract(time_start).TotalSeconds;//tinh thoi gian lam viec den thoi diem hien tai
            }
            else
            {
                if (!(time_start.Hour <= 7)) // lui 1 ngay cho time start neu no lon hon 6h
                {
                    time_start = time_start.AddDays(-1);
                    Work_sec = (int)t.Subtract(time_start).TotalSeconds;//tinh thoi gian lam viec den thoi diem hien tai
                }
            }
            #endregion

            #region Update mau cho ca ngay
            if (t.Hour < 19 && t.Hour > 6)
            {
                for (int i = 7; i < 19; i++)
                {
                    if (i <= t.Hour)
                    {
                        int actual_inH = (int)Line_base_on_data.H_Actual_Stock[1, (i - 7)];
                        string hourstr = this.ConvertTextTime(i);
                        int _work_sec = 0;
                        if (i < t.Hour)
                        {
                            _work_sec = 3600;
                        }
                        else
                        {
                            _work_sec = (int)(t.Minute * 60.0 + t.Second);
                        }
                        int target_real = Convert.ToInt32(_work_sec * 1000.0 / Line_base_on_data.Tact_time_Stock[1] + 0.5);
                        string color = this.setColorCell(rateReal(actual_inH, target_real));
                        db.setColor("LineColorDay", (DungChung.soLine+1).ToString(), "[Color" + hourstr + "H]", color, PlanId);
                    }
                    else
                    {
                        db.setColor("LineColorDay", (DungChung.soLine + 1).ToString(), "[Color" + i.ToString() + "H]", "NULL", PlanId);
                    }
                }
            }
            #endregion
            else
            #region Update mau cho ca dem
            {
                #region neu ca dem chua qua 0 H
                if (!(t.Hour < 7))
                {
                    for (int i = 19; i <= 23; i++)
                    {
                        if (i <= t.Hour)
                        {
                            int actual_inH = (int)Line_base_on_data.H_Actual_Stock[1, (i - 19)];
                            string hourstr = this.ConvertTextTime(i);
                            int _work_sec = 0;
                            if (i < t.Hour)
                            {
                                _work_sec = 3600;
                            }
                            else
                            {
                                _work_sec = (int)(t.Minute * 60.0 + t.Second);
                            }
                            int target_real = Convert.ToInt32(_work_sec * 1000.0 / Line_base_on_data.Tact_time_Stock[1] + 0.5);
                            string color = this.setColorCell(rateReal(actual_inH, target_real));
                            db.setColor("LineColorNight", (DungChung.soLine + 1).ToString(), "[Color" + hourstr + "H]", color, PlanId);
                        }
                        else
                        {
                            db.setColor("LineColorNight", (DungChung.soLine + 1).ToString(), "[Color" + i.ToString() + "H]", "NULL", PlanId);
                        }
                    }

                }
                #endregion
                else
                #region neu ca dem qua 0 H
                {
                    for (int i = 19; i <= 23; i++)
                    {
                        int actual_inH = (int)Line_base_on_data.H_Actual_Stock[1, (i - 19)];
                        string hourstr = this.ConvertTextTime(i);
                        int _work_sec = 0;

                        _work_sec = 3600;

                        int target_real = Convert.ToInt32(_work_sec * 1000.0 / Line_base_on_data.Tact_time_Stock[1] + 0.5);
                        string color = this.setColorCell(rateReal(actual_inH, target_real));
                        db.setColor("LineColorNight", (DungChung.soLine + 1).ToString(), "[Color" + hourstr + "H]", color, PlanId);
                    }
                    for (int i = 0; i < 7; i++)
                    {
                        if (i <= t.Hour)
                        {
                            if (i < t.Hour)
                            {
                                int actual_inH = (int)Line_base_on_data.H_Actual_Stock[1, (i + 5)];
                                string hourstr = this.ConvertTextTime(i);
                                int _work_sec = 0;
                                _work_sec = 3600;
                                int target_real = Convert.ToInt32(_work_sec * 1000.0 / Line_base_on_data.Tact_time_Stock[1] + 0.5);
                                string color = this.setColorCell(rateReal(actual_inH, target_real));
                                db.setColor("LineColorNight", (DungChung.soLine + 1).ToString(), "[Color" + hourstr + "H]", color, PlanId);
                            }
                            else
                            {
                                int actual_inH = (int)Line_base_on_data.H_Actual_Stock[1, (i + 5)];
                                string hourstr = this.ConvertTextTime(i);
                                int _work_sec = 0;
                                _work_sec = (int)(t.Minute * 60.0 + t.Second);
                                int target_real = Convert.ToInt32(_work_sec * 1000.0 / Line_base_on_data.Tact_time_Stock[1] + 0.5);
                                string color = this.setColorCell(rateReal(actual_inH, target_real));
                                db.setColor("LineColorNight", (DungChung.soLine + 1).ToString(), "[Color" + hourstr + "H]", color, PlanId);
                            }
                        }
                        else
                        {
                            string hourstr = this.ConvertTextTime(i);
                            db.setColor("LineColorNight", (DungChung.soLine + 1).ToString(), "[Color" + hourstr + "H]", "NULL", PlanId);
                        }

                    }
                }
                #endregion
            }
            #endregion
        }
    }
}