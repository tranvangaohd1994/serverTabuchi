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
        //dia chi thanh ghi cua PLC
        public const ushort start_address_Status_Line = 4;
        public const ushort start_address_Actual_Line = 27;
        public const ushort start_address_PLan_Line = 34;
        public const ushort start_address_Tact_time_Line = 41;
        public const ushort start_address_Target_Line = 55;
        public const ushort start_address_Diff_Line = 62;

        public const ushort start_address_Status_Stock = 4;
        public const ushort start_address_Actual_Stock = 24;
        public const ushort start_address_PLan_Stock = 31;
        public const ushort start_address_Tact_time_Stock = 38;
        public const ushort start_address_Target_Stock = 52;
        public const ushort start_address_Diff_Stock = 56;

        ModbusTCP[] MB;// = new ModbusTCP[DungChung.SoPLC + 2];//1 mảng Modbus 8 phần từ 6 PLC ở line và 2 PLC ở Stocking

        //ket noi tong quat //ko sua gi ca
        private void Connect_MB(ref ModbusTCP _MB, string PLC_IP)
        {
            _MB = new ModbusTCP(PLC_IP, 502);
            _MB.OnResponseData += new ModbusTCP.ResponseData(MBmaster_OnResponseData);
            _MB.OnException += new ModbusTCP.ExceptionData(MBmaster_OnException);
        }
        private void MBmaster_OnException(ushort id, byte unit, byte function, byte exception, string IP_address)
        {
            string exc = "Modbus says error: ";
            switch (exception)
            {
                case ModbusTCP.excIllegalFunction: exc += "Illegal function!" + IP_address.ToString(); break;
                case ModbusTCP.excIllegalDataAdr: exc += "Illegal data adress! " + IP_address.ToString(); break;
                case ModbusTCP.excIllegalDataVal: exc += "Illegal data value!" + IP_address.ToString(); break;
                case ModbusTCP.excSlaveDeviceFailure: exc += "Slave device failure! " + IP_address.ToString(); break;
                case ModbusTCP.excAck: exc += "Acknoledge! " + IP_address.ToString(); break;
                case ModbusTCP.excGatePathUnavailable: exc += "Gateway path unavailbale! " + IP_address.ToString(); break;
                case ModbusTCP.excExceptionTimeout: exc += "Slave timed out! " + IP_address.ToString(); break;
                case ModbusTCP.excExceptionConnectionLost: exc += "Connection is lost! " + IP_address.ToString(); break;
                case ModbusTCP.excExceptionNotConnected: exc += "Not connected! " + IP_address.ToString(); break;
            }

            //MessageBox.Show(exc, "Modbus slave exception");
            Display_listbox_stt(exc);
        }
        //Lenh gan 1 mang 2 byte vao 1 so int trong 1 mang int[]
        public void GetArr1Value2b(byte[] B_arr, ref int[] I_arr, int I_arr_index)
        {

            I_arr[I_arr_index - 1] = (int)(B_arr[0] * 256 + B_arr[1]);


        }
        //Lenh gan 1 mang byte 14 phan tu sang 1 mang int 7 phan tu
        public void GetArr7Value2b(byte[] B_arr, ref int[] I_arr, ushort taskID, int lineNumeber, String PLCIP)
        {
            //byte[] tmp = new byte[2];
            try
            {
                for (int i = 0; i < 7; i++)
                {
                    I_arr[i] = (int)(B_arr[i * 2] * 256 + B_arr[i * 2 + 1]);
                }
            }
            catch (Exception ex)
            {
                Display_listbox_stt("GetArr7Value2b-Error at recieve this Task ID:  " + ex.ToString() + taskID.ToString() + " " + B_arr.Length.ToString() + " " + lineNumeber.ToString() + "  " + PLCIP);
            }

        }
        //Lenh gan 1 mang 4 byte vao 1 so int
        public void GetArr1Value4b(byte[] B_arr, ref uint[] I_arr, int I_arr_index)
        {
            I_arr[I_arr_index - 1] = (uint)(B_arr[0] * 256 * 256 * 256 + B_arr[1] * 256 * 256 + B_arr[2] * 256 + B_arr[3]);
            //I_arr[I_arr_index - 1] = (uint)BitConverter.ToInt32(B_arr, 0);
        }
        public void GetArr1Value4b(byte[] B_arr, ref int[] I_arr, int I_arr_index)
        {
            I_arr[I_arr_index - 1] = (int)(B_arr[0] * 256 * 256 * 256 + B_arr[1] * 256 * 256 + B_arr[2] * 256 + B_arr[3]);
            //I_arr[I_arr_index - 1] = (uint)BitConverter.ToInt32(B_arr, 0);
        }
        //Lenh gan 1 mang 28 byte vao 1 mang so uint
        public void GetArr7Value4b(byte[] B_arr, ref uint[] I_arr)
        {
            if (B_arr.Length != 28)
            {
                return;

            }
            for (int i = 0; i < 7; i++)
            {
                I_arr[i] = (uint)(B_arr[i * 4] * 256 * 256 * 256 + B_arr[i * 4 + 1] * 256 * 256 + B_arr[i * 4 + 2] * 256 + B_arr[i * 4 + 3]);
            }
        }

        //doc cac thanh ghi(n*2 byte 1 lan doc, n la _Length)
        private void MB_Read_Hold_Reg(ushort _StartAddress, byte _Length, ref ModbusTCP _MB, ushort Taks_ID)
        {
            byte unit = 1;
            ushort StartAddress = _StartAddress;
            byte Length = _Length;
            _MB.ReadHoldingRegister(Taks_ID, unit, StartAddress, Length);
        }
        //Ghi  2 byte
        private void MB_WriteSingleReg(ushort _StartAddress, ref ModbusTCP _MB, byte[] _data, ushort Taks_ID)
        {
            byte[] _dataw = new byte[2];
            _dataw[0] = _data[0];
            _dataw[1] = _data[1];
            byte unit = 1;
            ushort StartAddress = _StartAddress;
            _MB.WriteSingleRegister(Taks_ID, unit, StartAddress, _dataw);
        }
        //Ghi (n*2 byte)
        private void MB_WriteMultipleReg(ushort _StartAddress, ref ModbusTCP _MB, byte[] _data, ushort Taks_ID)
        {
            byte unit = 1;
            ushort StartAddress = _StartAddress;
            _MB.WriteMultipleRegister(Taks_ID, unit, StartAddress, _data);
        }
        ////////////////////////////////////////////////////////////////////////////////////
        //PHAN SAU KHONG LIEN QUAN DEN MB CHO PLC CO DINH NAO, DUNG PLC NAO THI KHAI BAO NHU TREN
        ////////////////////////////////////////////////////////////////////////////////////
        
        //Ham xu ly du lieu nhan ve--cac ham phai sua trong modbus---TabuchiFix
        private void MBmaster_OnResponseData(ushort ID, byte unit, byte function, byte[] values)
        {
            // ------------------------------------------------------------------
            // Seperate calling threads
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ModbusTCP.ResponseData(MBmaster_OnResponseData), new object[] { ID, unit, function, values });
                return;
            }
            int count_MB = 0;
            int tmp_ID = ID;
            //Neu la tac vu doc cac thong so cua line thi task ID se co gia tri tu 0 -> 288
            if (ID < 480) //480 = 10*8*6      6: so luong plc; 8: 7 line + All line; 6: so data
            {
                if (tmp_ID > 47)
                {
                    tmp_ID = ID % 48;
                    /*TabuchiFix_20160607*/
                    count_MB = (int)((ID) / 48);
                    

                }
                Excute_Rdata(ref MB[count_MB], values, (ushort)tmp_ID);
            }
            else if (ID < 492)//read stock
            {// neu taks ID lon hon 239 thi do la cac tac vu doc stock hoac cac tac vu ghi
                if (ID < 486)// neu task ID nho hon 294 thi do la cac tac vu cho stock 1
                {
                    count_MB = DungChung.SoPLC;
                }
                else
                {
                    count_MB = DungChung.SoPLC+1;
                }
                Excute_Rdata_stock(ref MB[count_MB], values, ID);
            }
        }
        //nhan biet day la du lieu cua PLC nao, loai gi
        private void Excute_Rdata(ref ModbusTCP _MB, byte[] value_recive, ushort Taks_ID)
        {
            #region excute data for n line
            if (Taks_ID < 48 && _MB != null)//nho hon 48 tuc la cua PLC1 ko sua gi ca
            {
                switch (Taks_ID)
                {
                    //READ STATUS
                    case (ushort)MB_Task_enum.r1Ast:
                        GetArr7Value2b(value_recive, ref _MB.Status, Taks_ID , -1 ,_MB.IP_address);                
                        Display_listbox_stt("Status:        " + _MB.Status[0] + "   " + _MB.Status[1] + "   " + _MB.Status[2] + "   " + _MB.Status[3] + "   " + _MB.Status[4] + "   " + _MB.Status[5] + "   " + _MB.Status[6]);
                        break;
                    case (ushort)MB_Task_enum.r1st:
                        GetArr1Value2b(value_recive, ref _MB.Status, 1);
                        break;
                    case (ushort)MB_Task_enum.r2st:
                        GetArr1Value2b(value_recive, ref _MB.Status, 2);
                        break;
                    case (ushort)MB_Task_enum.r3st:
                        GetArr1Value2b(value_recive, ref _MB.Status, 3);
                        break;
                    case (ushort)MB_Task_enum.r4st:
                        GetArr1Value2b(value_recive, ref _MB.Status, 4);
                        break;
                    case (ushort)MB_Task_enum.r5st:
                        GetArr1Value2b(value_recive, ref _MB.Status, 5);
                        break;
                    case (ushort)MB_Task_enum.r6st:
                        GetArr1Value2b(value_recive, ref _MB.Status, 6);
                        break;
                    case (ushort)MB_Task_enum.r7st:
                        GetArr1Value2b(value_recive, ref _MB.Status, 7);
                        break;
                    //READ PLAN
                    case (ushort)MB_Task_enum.r1Apl:
                        GetArr7Value2b(value_recive, ref _MB.Plan, Taks_ID, -1, _MB.IP_address);
                        Display_listbox_stt("Plan " + _MB.Plan[0] + " " + _MB.Plan[1] + " " + _MB.Plan[2] + " " + _MB.Plan[3] + " " + _MB.Plan[4] + " " + _MB.Plan[5] + " " + _MB.Plan[6]);
                        break;
                    case (ushort)MB_Task_enum.r1pl:
                        GetArr1Value2b(value_recive, ref _MB.Plan, 1);
                        break;
                    case (ushort)MB_Task_enum.r2pl:
                        GetArr1Value2b(value_recive, ref _MB.Plan, 2);
                        break;
                    case (ushort)MB_Task_enum.r3pl:
                        GetArr1Value2b(value_recive, ref _MB.Plan, 3);
                        break;
                    case (ushort)MB_Task_enum.r4pl:
                        GetArr1Value2b(value_recive, ref _MB.Plan, 4);
                        break;
                    case (ushort)MB_Task_enum.r5pl:
                        GetArr1Value2b(value_recive, ref _MB.Plan, 5);
                        break;
                    case (ushort)MB_Task_enum.r6pl:
                        GetArr1Value2b(value_recive, ref _MB.Plan, 6);
                        break;
                    case (ushort)MB_Task_enum.r7pl:
                        GetArr1Value2b(value_recive, ref _MB.Plan, 7);
                        break;
                    //READ ACTUAL
                    case (ushort)MB_Task_enum.r1Aat:
                        GetArr7Value2b(value_recive, ref _MB.Actual, Taks_ID, -1, _MB.IP_address);
                        Display_listbox_stt("Actual " + _MB.Actual[0] + " " + _MB.Actual[1] + " " + _MB.Actual[2] + " " + _MB.Actual[3] + " " + _MB.Actual[4] + " " + _MB.Actual[5] + " " + _MB.Actual[6]);
                        break;
                    case (ushort)MB_Task_enum.r1at:
                        GetArr1Value2b(value_recive, ref _MB.Actual, 1);
                        break;
                    case (ushort)MB_Task_enum.r2at:
                        GetArr1Value2b(value_recive, ref _MB.Actual, 2);
                        break;
                    case (ushort)MB_Task_enum.r3at:
                        GetArr1Value2b(value_recive, ref _MB.Actual, 3);
                        break;
                    case (ushort)MB_Task_enum.r4at:
                        GetArr1Value2b(value_recive, ref _MB.Actual, 4);
                        break;
                    case (ushort)MB_Task_enum.r5at:
                        GetArr1Value2b(value_recive, ref _MB.Actual, 5);
                        break;
                    case (ushort)MB_Task_enum.r6at:
                        GetArr1Value2b(value_recive, ref _MB.Actual, 6);
                        break;
                    case (ushort)MB_Task_enum.r7at:
                        GetArr1Value2b(value_recive, ref _MB.Actual, 7);
                        break;
                    //READ TARGET
                    case (int)MB_Task_enum.r1Atg:
                        GetArr7Value2b(value_recive, ref _MB.Target, Taks_ID, -1, _MB.IP_address);
                        Display_listbox_stt("Target:        " + _MB.Target[0] + "   " + _MB.Target[1] + "   " + _MB.Target[2] + "   " + _MB.Target[3] + "   " + _MB.Target[4] + "   " + _MB.Target[5] + "   " + _MB.Target[6]);
                        break;
                    case (int)MB_Task_enum.r1tg:
                        GetArr1Value2b(value_recive, ref _MB.Target, 1);
                        break;
                    case (int)MB_Task_enum.r2tg:
                        GetArr1Value2b(value_recive, ref _MB.Target, 2);
                        break;
                    case (int)MB_Task_enum.r3tg:
                        GetArr1Value2b(value_recive, ref _MB.Target, 3);
                        break;
                    case (int)MB_Task_enum.r4tg:
                        GetArr1Value2b(value_recive, ref _MB.Target, 4);
                        break;
                    case (int)MB_Task_enum.r5tg:
                        GetArr1Value2b(value_recive, ref _MB.Target, 5);
                        break;
                    case (int)MB_Task_enum.r6tg:
                        GetArr1Value2b(value_recive, ref _MB.Target, 6);
                        break;
                    case (int)MB_Task_enum.r7tg:
                        GetArr1Value2b(value_recive, ref _MB.Target, 7);
                        break;
                    //READ DIFF
                    case (ushort)MB_Task_enum.r1Adf:
                        GetArr7Value2b(value_recive, ref _MB.Diff, Taks_ID, -1, _MB.IP_address);
                        Display_listbox_stt("Diff:        " + _MB.Diff[0] + "   " + _MB.Diff[1] + "   " + _MB.Target[2] + "   " + _MB.Diff[3] + "   " + _MB.Diff[4] + "   " + _MB.Diff[5] + "   " + _MB.Diff[6]);
                        break;
                    case (ushort)MB_Task_enum.r1df:
                        GetArr1Value2b(value_recive, ref _MB.Diff, 1);
                        break;
                    case (ushort)MB_Task_enum.r2df:
                        GetArr1Value2b(value_recive, ref _MB.Diff, 2);
                        break;
                    case (ushort)MB_Task_enum.r3df:
                        GetArr1Value2b(value_recive, ref _MB.Diff, 3);
                        break;
                    case (ushort)MB_Task_enum.r4df:
                        GetArr1Value2b(value_recive, ref _MB.Diff, 4);
                        break;
                    case (ushort)MB_Task_enum.r5df:
                        GetArr1Value2b(value_recive, ref _MB.Diff, 5);
                        break;
                    case (ushort)MB_Task_enum.r6df:
                        GetArr1Value2b(value_recive, ref _MB.Diff, 6);
                        break;
                    case (ushort)MB_Task_enum.r7df:
                        GetArr1Value2b(value_recive, ref _MB.Diff, 7);
                        break;

                    //READ TACTIME
                    case (ushort)MB_Task_enum.r1Att:
                        try
                        {
                            GetArr7Value4b(value_recive, ref _MB.Tact_time);
                            Display_listbox_stt("Tacks Time:  "+_MB.IP_address + " " + (_MB.Tact_time[0] / 1000) + "   " + (_MB.Tact_time[1] / 1000) + "   " + (_MB.Tact_time[2] / 1000) + "   " + (_MB.Tact_time[3] / 1000) + "   " + (_MB.Tact_time[4] / 1000) + "   " + (_MB.Tact_time[5] / 1000) + "   " + (_MB.Tact_time[6] / 1000));
                        }
                        catch(Exception e) {
                            Display_listbox_stt("Error Read TactTime : " + e.ToString());
                        }
                            break;
                    case (ushort)MB_Task_enum.r1tt:
                        GetArr1Value4b(value_recive, ref _MB.Tact_time, 1);
                        break;
                    case (ushort)MB_Task_enum.r2tt:
                        GetArr1Value4b(value_recive, ref _MB.Tact_time, 2);
                        break;
                    case (ushort)MB_Task_enum.r3tt:
                        GetArr1Value4b(value_recive, ref _MB.Tact_time, 3);
                        break;
                    case (ushort)MB_Task_enum.r4tt:
                        GetArr1Value4b(value_recive, ref _MB.Tact_time, 4);
                        break;
                    case (ushort)MB_Task_enum.r5tt:
                        GetArr1Value4b(value_recive, ref _MB.Tact_time, 5);
                        break;
                    case (ushort)MB_Task_enum.r6tt:
                        GetArr1Value4b(value_recive, ref _MB.Tact_time, 6);
                        break;
                    case (ushort)MB_Task_enum.r7tt:
                        GetArr1Value4b(value_recive, ref _MB.Tact_time, 7);
                        break;
                }
            }
            else {
                Display_listbox_stt("Modbus Obj is Null or task ID is over scope --> lines  " + Taks_ID.ToString() );
            }
            #endregion
        }
        private void Excute_Rdata_stock(ref ModbusTCP _MB, byte[] value_recive, ushort Taks_ID)
        {
            #region Excute data for stock
            //Cac tac vu doc stock co task ID tu 240 -> 251
            if(_MB ==null){
                Display_listbox_stt("Null:    ...Taks_ID" + Taks_ID.ToString());
            }
            if (Taks_ID < 492 && _MB !=null)
            {
                if (Taks_ID >= 486)
                {
                    int tmp = (int)Taks_ID - 6;
                    Taks_ID = (ushort)tmp;
                }
                switch (Taks_ID)
                {//st pl at tg df tt
                    case (ushort)MB_Task_enum.r11Ast:
                        GetArr1Value2b(value_recive, ref _MB.Status_Stock, 1);
                        Display_listbox_stt("Status stock:   " + MB[DungChung.SoPLC].Status_Stock[0]);
                        break;
                    case (ushort)MB_Task_enum.r11Apl:
                        GetArr1Value4b(value_recive, ref _MB.Plan_Stock, 1);
                        Display_listbox_stt("Plan stock:   " + MB[DungChung.SoPLC].Plan_Stock[0]);
                        break;
                    case (ushort)MB_Task_enum.r11Aat:
                        GetArr1Value4b(value_recive, ref _MB.Actual_Stock, 1);
                        Display_listbox_stt("Actual stock:   " + MB[DungChung.SoPLC].Actual_Stock[0]);
                        break;
                    case (ushort)MB_Task_enum.r11Atg:
                        GetArr1Value4b(value_recive, ref _MB.Target_Stock, 1);
                        Display_listbox_stt("Target stock:   " + MB[DungChung.SoPLC].Target_Stock[0]);
                        break;
                    case (ushort)MB_Task_enum.r11Adf:
                        GetArr1Value4b(value_recive, ref _MB.Diff_Stock, 1);
                        break;
                    case (ushort)MB_Task_enum.r11Att:
                        GetArr1Value4b(value_recive, ref _MB.Tact_time_Stock, 1);
                        Display_listbox_stt("Tact time stock:   " + MB[DungChung.SoPLC].Tact_time_Stock[0]);
                        break;
                }
            }//else{//cac tac vu ghi}
            else
            {
              //  Display_listbox_stt("Modbus(Stock) Obj is Null or task ID is over scope  ...Taks_ID" + Taks_ID.ToString());
            }
            #endregion
        }


        //Enum cho biet PLC dang thuc hien tac vu gi
        //Enum nay giup ta biet duoc du lieu cua tac vu gi dang tra ve
        //cho biet dang thao tac doc hay ghi thanh ghi nao
        public enum MB_Task_enum 
        {
            //PLC1 READ
          r1Ast=0, r1st, r2st, r3st, r4st, r5st, r6st, r7st,//st status     0-7
            r1Apl, r1pl, r2pl, r3pl, r4pl, r5pl, r6pl, r7pl,//pl plan       8-15
            r1Aat, r1at, r2at, r3at, r4at, r5at, r6at, r7at,//at actual     16-23
            r1Atg, r1tg, r2tg, r3tg, r4tg, r5tg, r6tg, r7tg,//tg target     24-31
            r1Adf, r1df, r2df, r3df, r4df, r5df, r6df, r7df,//df diff       32-39
            r1Att, r1tt, r2tt, r3tt, r4tt, r5tt, r6tt, r7tt,//tt total 47   40-47
           
            //PLC2 READ
            r2Ast, r8st, r9st, r10st, r11st, r12st, r13st, r14st,//st       48-55
            r2Apl, r8pl, r9pl, r10pl, r11pl, r12pl, r13pl, r14pl,//pl       56-63
            r2Aat, r8at, r9at, r10at, r11at, r12at, r13at, r14at,//at       64-71
            r2Atg, r8tg, r9tg, r10tg, r11tg, r12tg, r13tg, r14tg,//tg       72-->79
            r2Adf, r8df, r9df, r10df, r11df, r12df, r13df, r14df,//df       80-->87
            r2Att, r8tt, r9tt, r10tt, r11tt, r12tt, r13tt, r14tt,//tt 95    88-->95
            //PLC3 READ
            r3Ast, r15st, r16st, r17st, r18st, r19st, r20st, r21st,//st     96-->103    
            r3Apl, r15pl, r16pl, r17pl, r18pl, r19pl, r20pl, r21pl,//pl     104-->111
            r3Aat, r15at, r16at, r17at, r18at, r19at, r20at, r21at,//at     112-->119
            r3Atg, r15tg, r16tg, r17tg, r18tg, r19tg, r20tg, r21tg,//tg     120-->127
            r3Adf, r15df, r16df, r17df, r18df, r19df, r20df, r21df,//df     128-->135
            r3Att, r15tt, r16tt, r17tt, r18tt, r19tt, r20tt, r21tt,//tt     136-->143
            //PLC4 READ
            r4Ast, r22st, r23st, r24st, r25st, r26st, r27st, r28st,//st     144-->151
            r4Apl, r22pl, r23pl, r24pl, r25pl, r26pl, r27pl, r28pl,//pl     152-->159
            r4Aat, r22at, r23at, r24at, r25at, r26at, r27at, r28at,//at     160-->167
            r4Atg, r22tg, r23tg, r24tg, r25tg, r26tg, r27tg, r28tg,//tg     168-->175
            r4Adf, r22df, r23df, r24df, r25df, r26df, r27df, r28df,//df     176-->183
            r4Att, r22tt, r23tt, r24tt, r25tt, r26tt, r27tt, r28tt,//tt     184-->191
            //PLC5 READ
            r5Ast, r29st, r30st, r31st, r32st, r33st, r34st, r35st,//st     192-->199
            r5Apl, r29pl, r30pl, r31pl, r32pl, r33pl, r34pl, r35pl,//pl     200-->207
            r5Aat, r29at, r30at, r31at, r32at, r33at, r34at, r35at,//at     208-->215
            r5Atg, r29tg, r30tg, r31tg, r32tg, r33tg, r34tg, r35tg,//tg     216-->223
            r5Adf, r29df, r30df, r31df, r32df, r33df, r34df, r35df,//df     224-->231
            r5Att, r29tt, r30tt, r31tt, r32tt, r33tt, r34tt, r35tt,//tt     232-->239
            //PLC6 READ /*TabuchiFix*/
            r6Ast, r36st, r37st, r38st, r39st, r40st, r41st, r42st,//st     240-->247
            r6Apl, r36pl, r37pl, r38pl, r39pl, r40pl, r41pl, r42pl,//pl     248-->255
            r6Aat, r36at, r37at, r38at, r39at, r40at, r41at, r42at,//at     256-->263
            r6Atg, r36tg, r37tg, r38tg, r39tg, r40tg, r41tg, r42tg,//tg     264-->271
            r6Adf, r36df, r37df, r38df, r39df, r40df, r41df, r42df,//df     272-->279
            r6Att, r36tt, r37tt, r38tt, r39tt, r40tt, r41tt, r42tt,//tt     280-->287
            //PLC7 READ /*TabuchiFix*/
            r7Ast, r43st, r44st, r45st, r46st, r47st, r48st, r49st,//st     288-->295
            r7Apl, r43pl, r44pl, r45pl, r46pl, r47pl, r48pl, r49pl,//pl     296-->303
            r7Aat, r43at, r44at, r45at, r46at, r47at, r48at, r49at,//at     304-->311
            r7Atg, r43tg, r44tg, r45tg, r46tg, r47tg, r48tg, r49tg,//tg     312-->319
            r7Adf, r43df, r44df, r45df, r46df, r47df, r48df, r49df,//df     320-->327
            r7Att, r43tt, r44tt, r45tt, r46tt, r47tt, r48tt, r49tt,//tt     328-->335
            //PLC8 READ /*TabuchiFix*/
            r8Ast, r50st, r51st, r52st, r53st, r54st, r55st, r56st,//st     336-->343
            r8Apl, r50pl, r51pl, r52pl, r53pl, r54pl, r55pl, r56pl,//pl     344-->351
            r8Aat, r50at, r51at, r52at, r53at, r54at, r55at, r56at,//at     352-->359
            r8Atg, r50tg, r51tg, r52tg, r53tg, r54tg, r55tg, r56tg,//tg     360-->367
            r8Adf, r50df, r51df, r52df, r53df, r54df, r55df, r56df,//df     368-->375
            r8Att, r50tt, r51tt, r52tt, r53tt, r54tt, r55tt, r56tt,//tt     376-->383
            //PLC9 READ /*TabuchiFix*/
            r9Ast, r57st, r58st, r59st, r60st, r61st, r62st, r63st,//st     384-->391
            r9Apl, r57pl, r58pl, r59pl, r60pl, r61pl, r62pl, r63pl,//pl     392-->399
            r9Aat, r57at, r58at, r59at, r60at, r61at, r62at, r63at,//at     400-->407
            r9Atg, r57tg, r58tg, r59tg, r60tg, r61tg, r62tg, r63tg,//tg     408-->415
            r9Adf, r57df, r58df, r59df, r60df, r61df, r62df, r63df,//df     416-->423
            r9Att, r57tt, r58tt, r59tt, r60tt, r61tt, r62tt, r63tt,//tt     424-->431
            //PLC10 READ /*TabuchiFix*/
            r10Ast, r64st, r65st, r66st, r67st, r68st, r69st, r70st,//st    432-->439
            r10Apl, r64pl, r65pl, r66pl, r67pl, r68pl, r69pl, r70pl,//pl    440-->447
            r10Aat, r64at, r65at, r66at, r67at, r68at, r69at, r70at,//at    448-->455
            r10Atg, r64tg, r65tg, r66tg, r67tg, r68tg, r69tg, r70tg,//tg    456-->463
            r10Adf, r64df, r65df, r66df, r67df, r68df, r69df, r70df,//df    464-->471
            r10Att, r64tt, r65tt, r66tt, r67tt, r68tt, r69tt, r70tt,//tt    472-->479

            //PLC11 READ stock 1
            r11Ast,//st     480
            r11Apl,//pl     481
            r11Aat,//at     482
            r11Atg,//tg     483
            r11Adf,//df     484
            r11Att,//tt     485
            //PLC12 READ stock 2
            r12Ast,//st     486
            r12Apl,//pl     487 
            r12Aat,//at     488
            r12Atg,//tg     489
            r12Adf,//df     490
            r12Att,//tt     491
            w1Ast, w1st, w2st, w3st, w4st, w5st, w6st, w7st,//st status     492-->499
            w1Apl, w1pl, w2pl, w3pl, w4pl, w5pl, w6pl, w7pl,//pl plan       500-->507
            w1Aat, w1at, w2at, w3at, w4at, w5at, w6at, w7at,//at actual     508-->515
            w1Atg, w1tg, w2tg, w3tg, w4tg, w5tg, w6tg, w7tg,//tg tawget     516-->523
            w1Adf, w1df, w2df, w3df, w4df, w5df, w6df, w7df,//df diff       524-->531
            w1Att, w1tt, w2tt, w3tt, w4tt, w5tt, w6tt, w7tt,//tt total      532-->539
            //PLC2 wEAD
            w2Ast, w8st, w9st, w10st, w11st, w12st, w13st, w14st,//st       540-->547
            w2Apl, w8pl, w9pl, w10pl, w11pl, w12pl, w13pl, w14pl,//pl       548-->555
            w2Aat, w8at, w9at, w10at, w11at, w12at, w13at, w14at,//at       556-->563
            w2Atg, w8tg, w9tg, w10tg, w11tg, w12tg, w13tg, w14tg,//tg       564-->571
            w2Adf, w8df, w9df, w10df, w11df, w12df, w13df, w14df,//df       572-->579
            w2Att, w8tt, w9tt, w10tt, w11tt, w12tt, w13tt, w14tt,//tt       580-->587
            //PLC3 wEAD
            w3Ast, w15st, w16st, w17st, w18st, w19st, w20st, w21st,//st     588-->595
            w3Apl, w15pl, w16pl, w17pl, w18pl, w19pl, w20pl, w21pl,//pl     596-->603
            w3Aat, w15at, w16at, w17at, w18at, w19at, w20at, w21at,//at     604-->611
            w3Atg, w15tg, w16tg, w17tg, w18tg, w19tg, w20tg, w21tg,//tg     612-->619
            w3Adf, w15df, w16df, w17df, w18df, w19df, w20df, w21df,//df     620-->627
            w3Att, w15tt, w16tt, w17tt, w18tt, w19tt, w20tt, w21tt,//tt     628-->635
            //PLC4 wEAD
            w4Ast, w22st, w23st, w24st, w25st, w26st, w27st, w28st,//st     636-->643
            w4Apl, w22pl, w23pl, w24pl, w25pl, w26pl, w27pl, w28pl,//pl     644-->651
            w4Aat, w22at, w23at, w24at, w25at, w26at, w27at, w28at,//at     652-->659
            w4Atg, w22tg, w23tg, w24tg, w25tg, w26tg, w27tg, w28tg,//tg     660-->667
            w4Adf, w22df, w23df, w24df, w25df, w26df, w27df, w28df,//df     668-->675
            w4Att, w22tt, w23tt, w24tt, w25tt, w26tt, w27tt, w28tt,//tt     676-->683
            //PLC5 wEAD
            w5Ast, w29st, w30st, w31st, w32st, w33st, w34st, w35st,//st     684-->691
            w5Apl, w29pl, w30pl, w31pl, w32pl, w33pl, w34pl, w35pl,//pl     692-->699
            w5Aat, w29at, w30at, w31at, w32at, w33at, w34at, w35at,//at     700-->707
            w5Atg, w29tg, w30tg, w31tg, w32tg, w33tg, w34tg, w35tg,//tg     708-->715
            w5Adf, w29df, w30df, w31df, w32df, w33df, w34df, w35df,//df     716-->723
            w5Att, w29tt, w30tt, w31tt, w32tt, w33tt, w34tt, w35tt,//tt     724-->731
            //PLC6 wEAD /*TabuchiFix*/
            w6Ast, w36st, w37st, w38st, w39st, w40st, w41st, w42st,//st     732-->739
            w6Apl, w36pl, w37pl, w38pl, w39pl, w40pl, w41pl, w42pl,//pl     740-->747
            w6Aat, w36at, w37at, w38at, w39at, w40at, w41at, w42at,//at     748-->755
            w6Atg, w36tg, w37tg, w38tg, w39tg, w40tg, w41tg, w42tg,//tg     756-->763
            w6Adf, w36df, w37df, w38df, w39df, w40df, w41df, w42df,//df     764-->771
            w6Att, w36tt, w37tt, w38tt, w39tt, w40tt, w41tt, w42tt,//tt     772-->779
            //PLC7 wEAD /*TabuchiFix*/
            w7Ast, w43st, w44st, w45st, w46st, w47st, w48st, w49st,//st     780-->787
            w7Apl, w43pl, w44pl, w45pl, w46pl, w47pl, w48pl, w49pl,//pl     788-->795
            w7Aat, w43at, w44at, w45at, w46at, w47at, w48at, w49at,//at     796-->803
            w7Atg, w43tg, w44tg, w45tg, w46tg, w47tg, w48tg, w49tg,//tg     804-->811
            w7Adf, w43df, w44df, w45df, w46df, w47df, w48df, w49df,//df     812-->819
            w7Att, w43tt, w44tt, w45tt, w46tt, w47tt, w48tt, w49tt,//tt     820-->827
            //PLC8 wEAD /*TabuchiFix*/
            w8Ast, w50st, w51st, w52st, w53st, w54st, w55st, w56st,//st     828-->835
            w8Apl, w50pl, w51pl, w52pl, w53pl, w54pl, w55pl, w56pl,//pl     836-->843
            w8Aat, w50at, w51at, w52at, w53at, w54at, w55at, w56at,//at     844-->851
            w8Atg, w50tg, w51tg, w52tg, w53tg, w54tg, w55tg, w56tg,//tg     852-->859
            w8Adf, w50df, w51df, w52df, w53df, w54df, w55df, w56df,//df     860-->867
            w8Att, w50tt, w51tt, w52tt, w53tt, w54tt, w55tt, w56tt,//tt     868-->875
            //PLC9 wEAD /*TabuchiFix*/
            w9Ast, w57st, w58st, w59st, w60st, w61st, w62st, w63st,//st     876-->883
            w9Apl, w57pl, w58pl, w59pl, w60pl, w61pl, w62pl, w63pl,//pl     884-->891
            w9Aat, w57at, w58at, w59at, w60at, w61at, w62at, w63at,//at     892-->899
            w9Atg, w57tg, w58tg, w59tg, w60tg, w61tg, w62tg, w63tg,//tg     900-->907
            w9Adf, w57df, w58df, w59df, w60df, w61df, w62df, w63df,//df     908-->915
            w9Att, w57tt, w58tt, w59tt, w60tt, w61tt, w62tt, w63tt,//tt     916-->923  
            //PLC10 wEAD /*TabuchiFix*/
            w10Ast, w64st, w65st, w66st, w67st, w68st, w69st, w70st,//st    924-->931    
            w10Apl, w64pl, w65pl, w66pl, w67pl, w68pl, w69pl, w70pl,//pl    932-->939
            w10Aat, w64at, w65at, w66at, w67at, w68at, w69at, w70at,//at    940-->947
            w10Atg, w64tg, w65tg, w66tg, w67tg, w68tg, w69tg, w70tg,//tg    948-->955
            w10Adf, w64df, w65df, w66df, w67df, w68df, w69df, w70df,//df    956-->963
            w10Att, w64tt, w65tt, w66tt, w67tt, w68tt, w69tt, w70tt,//tt    964-->971

            //PLC11 wEAD stock 1
            w11Ast,//st     972
            w11Apl,//pl     973
            w11Aat,//at     974
            w11Atg,//tg     975
            w11Adf,//df     976
            w11Att,//tt     977
            //PLC12 wEAD stock 2
            w12Ast,//st     978
            w12Apl,//pl     979
            w12Aat,//at     980
            w12Atg,//tg     981
            w12Adf,//df     982
            w12Att,//tt     983
        } //st pl at tg df tt


        //////////////////////////////////////////////////////////////////STATUS
        //Ghi STATUS cho 7 line lien tiep--TabuchiFix
        private void MB_Write_Status(ref ModbusTCP _MB, int[] _line_status)
        {
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)MB_Task_enum.w1Ast;
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)MB_Task_enum.w2Ast;
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)MB_Task_enum.w3Ast;
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)MB_Task_enum.w4Ast;
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)MB_Task_enum.w5Ast;
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)MB_Task_enum.w6Ast;
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)MB_Task_enum.w7Ast;
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)MB_Task_enum.w8Ast;
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)MB_Task_enum.w9Ast;
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)MB_Task_enum.w10Ast;
           
            byte[] line_status = new byte[14];
            for (int i = 0; i < 7; i++)
            {
                line_status[i * 2] = (byte)(_line_status[i] >> 8);
                line_status[i * 2 + 1] = (byte)_line_status[i];
            }
            ushort StartAddress = start_address_Status_Line;
            try
            {
                MB_WriteMultipleReg(StartAddress, ref _MB, line_status, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write status err: " + ex.Message);
            }
        }
        //Ghhi STATUS cho tung line--TabuchiFix
        private void MB_Write_Status(ref ModbusTCP _MB, int _line_status, int line_number)
        {
            if (line_number > 7)
            {
                line_number = line_number % 7;
                if (line_number == 0)
                {
                    line_number = 7;
                }
            }
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)(MB_Task_enum.w1Ast + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)(MB_Task_enum.w2Ast + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)(MB_Task_enum.w3Ast + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)(MB_Task_enum.w4Ast + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)(MB_Task_enum.w5Ast + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)(MB_Task_enum.w6Ast + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)(MB_Task_enum.w7Ast + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)(MB_Task_enum.w8Ast + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)(MB_Task_enum.w9Ast + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)(MB_Task_enum.w10Ast + (ushort)line_number);

            byte[] line_status = new byte[2];
            line_status[0] = (byte)(_line_status >> 8);
            line_status[1] = (byte)_line_status;
            //8 10 12 14 16
            ushort StartAddress = (ushort)(start_address_Status_Line + (line_number - 1));
            try
            {
                MB_WriteSingleReg(StartAddress, ref _MB, line_status, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write status err: " + ex.Message);
            }
        }
        //doc STATUS cua 7 line lien tiep
        private void MB_Read_Status(ref ModbusTCP _MB)
        {
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)MB_Task_enum.r1Ast;
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)MB_Task_enum.r2Ast;
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)MB_Task_enum.r3Ast;
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)MB_Task_enum.r4Ast;
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)MB_Task_enum.r5Ast;
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)MB_Task_enum.r6Ast;
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)MB_Task_enum.r7Ast;
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)MB_Task_enum.r8Ast;
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)MB_Task_enum.r9Ast;
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)MB_Task_enum.r10Ast;
         
            try
            {
                MB_Read_Hold_Reg(start_address_Status_Line, 7, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read status err: " + ex.Message);
            }
        }
        //doc STATUS cua 1 line bat ki
        private void MB_Read_Status(ref ModbusTCP _MB, int line_number, ushort Taks_ID)
        {
            if (line_number > 7)
            {
                line_number = line_number % 7;
                if (line_number == 0)
                {
                    line_number = 7;
                }
            }
            ushort StartAddress = (ushort)(start_address_Status_Line + (line_number - 1));
            try
            {
                MB_Read_Hold_Reg(StartAddress, 1, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read status err: " + ex.Message);
            }
        }
        //////////////////////////////////////////////////////////////////PLAN
        
        //Ghi PLAN ch 1 line bat ki
        private void MB_Write_Plan(ref ModbusTCP _MB, int _line_plan, int line_number)
        {
            if (line_number > 7)
            {
                line_number = line_number % 7;
                if (line_number == 0)
                {
                    line_number = 7;
                }
            }
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)(MB_Task_enum.w1Apl + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)(MB_Task_enum.w2Apl + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)(MB_Task_enum.w3Apl + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)(MB_Task_enum.w4Apl + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)(MB_Task_enum.w5Apl + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)(MB_Task_enum.w6Apl + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)(MB_Task_enum.w7Apl + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)(MB_Task_enum.w8Apl + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)(MB_Task_enum.w9Apl + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)(MB_Task_enum.w10Apl + (ushort)line_number);
            
            byte[] line_plan = new byte[2];
            line_plan[0] = (byte)(_line_plan >> 8);
            line_plan[1] = (byte)_line_plan;
            //8 10 12 14 16
            ushort StartAddress = (ushort)(start_address_PLan_Line + (line_number - 1));
            try
            {
                MB_WriteSingleReg(StartAddress, ref _MB, line_plan, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write plan err: " + ex.Message);
            }
        }
        //Ghi PLAN cho 7 line lien tiep
        private void MB_Write_Plan(ref ModbusTCP _MB, int[] _line_plan)
        {
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)MB_Task_enum.w1Apl;
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)MB_Task_enum.w2Apl;
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)MB_Task_enum.w3Apl;
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)MB_Task_enum.w4Apl;
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)MB_Task_enum.w5Apl;
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)MB_Task_enum.w6Apl;
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)MB_Task_enum.w7Apl;
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)MB_Task_enum.w8Apl;
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)MB_Task_enum.w9Apl;
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)MB_Task_enum.w10Apl;
            byte[] line_plan = new byte[14];
            for (int i = 0; i < 7; i++)
            {
                line_plan[i * 2] = (byte)(_line_plan[i] >> 8);
                line_plan[i * 2 + 1] = (byte)_line_plan[i];
            }
            ushort StartAddress = start_address_PLan_Line;//
            try
            {
                MB_WriteMultipleReg(StartAddress, ref _MB, line_plan, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write plan err: " + ex.Message);
            }
        }
        //doc PLAN cua 7 line lien tiep
        private void MB_Read_Plan(ref ModbusTCP _MB)
        {
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)MB_Task_enum.r1Apl;
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)MB_Task_enum.r2Apl;
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)MB_Task_enum.r3Apl;
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)MB_Task_enum.r4Apl;
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)MB_Task_enum.r5Apl;
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)MB_Task_enum.r6Apl;
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)MB_Task_enum.r7Apl;
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)MB_Task_enum.r8Apl;
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)MB_Task_enum.r9Apl;
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)MB_Task_enum.r10Apl;
           
            try
            {
                MB_Read_Hold_Reg(start_address_PLan_Line, 7, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read plan err: " + ex.Message);
            }
        }
        //doc PLAN cua 1 line bat ki
        private void MB_Read_Plan(ref ModbusTCP _MB, int line_number, ushort Taks_ID)
        {
            if (line_number > 7)
            {
                line_number = line_number % 7;
                if (line_number == 0)
                {
                    line_number = 7;
                }
            }
            ushort StartAddress = (ushort)(start_address_PLan_Line + (line_number - 1));
            try
            {
                MB_Read_Hold_Reg(StartAddress, 1, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read plan err: " + ex.Message);
            }
        }

        //////////////////////////////////////////////////////////////////ACTUAL
        //Ghi ACTUAL ch 1 line bat ki
        private void MB_Write_Actual(ref ModbusTCP _MB, int _line_actual, int line_number)
        {
            if (line_number > 7)
            {
                line_number = line_number % 7;
                if (line_number == 0)
                {
                    line_number = 7;
                }
            }
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)(MB_Task_enum.w1Aat + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)(MB_Task_enum.w2Aat + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)(MB_Task_enum.w3Aat + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)(MB_Task_enum.w4Aat + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)(MB_Task_enum.w5Aat + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)(MB_Task_enum.w6Aat + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)(MB_Task_enum.w7Aat + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)(MB_Task_enum.w8Aat + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)(MB_Task_enum.w9Aat + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)(MB_Task_enum.w10Aat + (ushort)line_number);
           

            byte[] line_actual = new byte[2];
            line_actual[0] = (byte)(_line_actual >> 8);
            line_actual[1] = (byte)_line_actual;
            //8 10 12 14 16
            ushort StartAddress = (ushort)(start_address_Actual_Line + (line_number - 1));
            try
            {
                MB_WriteSingleReg(StartAddress, ref _MB, line_actual, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write actual err: " + ex.Message);
            }
        }
        //Ghi ACTUAL cho 7 line lien tiep
        private void MB_Write_Actual(ref ModbusTCP _MB, int[] _line_actual)
        {
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)MB_Task_enum.w1Aat;
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)MB_Task_enum.w2Aat;
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)MB_Task_enum.w3Aat;
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)MB_Task_enum.w4Aat;
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)MB_Task_enum.w5Aat;
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)MB_Task_enum.w6Aat;
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)MB_Task_enum.w7Aat;
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)MB_Task_enum.w8Aat;
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)MB_Task_enum.w9Aat;
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)MB_Task_enum.w10Aat;
      
            byte[] line_actual = new byte[14];
            for (int i = 0; i < 7; i++)
            {
                line_actual[i * 2] = (byte)(_line_actual[i] >> 8);
                line_actual[i * 2 + 1] = (byte)_line_actual[i];
            }
            ushort StartAddress = start_address_Actual_Line;//
            try
            {
                MB_WriteMultipleReg(StartAddress, ref _MB, line_actual, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write actual err: " + ex.Message);
            }
        }
        //doc ACTUAL cua 7 line lien tiep
        private void MB_Read_Actual(ref ModbusTCP _MB)
        {
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)MB_Task_enum.r1Aat;
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)MB_Task_enum.r2Aat;
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)MB_Task_enum.r3Aat;
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)MB_Task_enum.r4Aat;
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)MB_Task_enum.r5Aat;
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)MB_Task_enum.r6Aat;
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)MB_Task_enum.r7Aat;
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)MB_Task_enum.r8Aat;
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)MB_Task_enum.r9Aat;
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)MB_Task_enum.r10Aat;
           
            try
            {
                MB_Read_Hold_Reg(start_address_Actual_Line, 7, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read Read err: " + ex.Message);
            }
        }
        //doc ACTUAL cua 1 line bat ki
        private void MB_Read_Actual(ref ModbusTCP _MB, int line_number, ushort Taks_ID)
        {
            if (line_number > 7)
            {
                line_number = line_number % 7;
                if (line_number == 0)
                {
                    line_number = 7;
                }
            }
            ushort StartAddress = (ushort)(start_address_Actual_Line + (line_number - 1));
            try
            {
                MB_Read_Hold_Reg(StartAddress, 1, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read plan err: " + ex.Message);
            }
        }

        //////////////////////////////////////////////////////////////////TARGET
        //Ghi TARGET ch 1 line bat ki
        private void MB_Write_Target(ref ModbusTCP _MB, int _line_target, int line_number)
        {
            if (line_number > 7)
            {
                line_number = line_number % 7;
                if (line_number == 0)
                {
                    line_number = 7;
                }
            }
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)(MB_Task_enum.w1Atg + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)(MB_Task_enum.w2Atg + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)(MB_Task_enum.w3Atg + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)(MB_Task_enum.w4Atg + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)(MB_Task_enum.w5Atg + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)(MB_Task_enum.w6Atg + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)(MB_Task_enum.w7Atg + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)(MB_Task_enum.w8Atg + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)(MB_Task_enum.w9Atg + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)(MB_Task_enum.w10Atg + (ushort)line_number);
            
            byte[] line_target = new byte[2];
            line_target[0] = (byte)(_line_target >> 8);
            line_target[1] = (byte)_line_target;
            //8 10 12 14 16
            ushort StartAddress = (ushort)(start_address_Target_Line + (line_number - 1));
            try
            {
                MB_WriteSingleReg(StartAddress, ref _MB, line_target, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write target err: " + ex.Message);
            }
        }
        //Ghi TARGET cho 7 line lien tiep
        private void MB_Write_Target(ref ModbusTCP _MB, int[] _line_target)
        {
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)MB_Task_enum.w1Atg;
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)MB_Task_enum.w2Atg;
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)MB_Task_enum.w3Atg;
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)MB_Task_enum.w4Atg;
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)MB_Task_enum.w5Atg;
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)MB_Task_enum.w6Atg;
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)MB_Task_enum.w7Atg;
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)MB_Task_enum.w8Atg;
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)MB_Task_enum.w9Atg;
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)MB_Task_enum.w10Atg;
            
            byte[] line_target = new byte[14];
            for (int i = 0; i < 7; i++)
            {
                line_target[i * 2] = (byte)(_line_target[i] >> 8);
                line_target[i * 2 + 1] = (byte)_line_target[i];
            }
            ushort StartAddress = start_address_Target_Line;//VW62
            try
            {
                MB_WriteMultipleReg(StartAddress, ref _MB, line_target, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write target err: " + ex.Message);
            }
        }
        //doc TARGET cua 7 line lien tiep
        private void MB_Read_Target(ref ModbusTCP _MB)
        {
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)MB_Task_enum.r1Atg;
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)MB_Task_enum.r2Atg;
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)MB_Task_enum.r3Atg;
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)MB_Task_enum.r4Atg;
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)MB_Task_enum.r5Atg;
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)MB_Task_enum.r6Atg;
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)MB_Task_enum.r7Atg;
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)MB_Task_enum.r8Atg;
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)MB_Task_enum.r9Atg;
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)MB_Task_enum.r10Atg;
            
            try
            {
                MB_Read_Hold_Reg(start_address_Target_Line, 7, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read target err: " + ex.Message);
            }
        }
        //doc TARGET cua 1 line bat ki
        private void MB_Read_Target(ref ModbusTCP _MB, int line_number, ushort Taks_ID)
        {
            if (line_number > 7)
            {
                line_number = line_number % 7;
                if (line_number == 0)
                {
                    line_number = 7;
                }
            }
            ushort StartAddress = (ushort)(start_address_Target_Line + (line_number - 1));
            try
            {
                MB_Read_Hold_Reg(StartAddress, 1, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read target err: " + ex.Message);
            }
        }

        //////////////////////////////////////////////////////////////////DIFF
        //Ghi DIFF ch 1 line bat ki
        private void MB_Write_Diff(ref ModbusTCP _MB, int _line_diff, int line_number)
        {
            if (line_number > 7)
            {
                line_number = line_number % 7;
                if (line_number == 0)
                {
                    line_number = 7;
                }
            }
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)(MB_Task_enum.w1Adf + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)(MB_Task_enum.w2Adf + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)(MB_Task_enum.w3Adf + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)(MB_Task_enum.w4Adf + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)(MB_Task_enum.w5Adf + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)(MB_Task_enum.w6Adf + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)(MB_Task_enum.w7Adf + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)(MB_Task_enum.w8Adf + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)(MB_Task_enum.w9Adf + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)(MB_Task_enum.w10Adf + (ushort)line_number);
            
            byte[] line_diff = new byte[2];
            line_diff[0] = (byte)(_line_diff >> 8);
            line_diff[1] = (byte)_line_diff;
            //8 10 12 14 16
            ushort StartAddress = (ushort)(start_address_Diff_Line + (line_number - 1));
            try
            {
                MB_WriteSingleReg(StartAddress, ref _MB, line_diff, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write diff err: " + ex.Message);
            }
        }
        //Ghi DIFF cho 7 line lien tiep
        private void MB_Write_Diff(ref ModbusTCP _MB, int[] _line_diff)
        {
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)MB_Task_enum.w1Adf;
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)MB_Task_enum.w2Adf;
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)MB_Task_enum.w3Adf;
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)MB_Task_enum.w4Adf;
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)MB_Task_enum.w5Adf;
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)MB_Task_enum.w6Adf;
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)MB_Task_enum.w7Adf;
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)MB_Task_enum.w8Adf;
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)MB_Task_enum.w9Adf;
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)MB_Task_enum.w10Adf;
           
            byte[] line_diff = new byte[14];
            for (int i = 0; i < 7; i++)
            {
                line_diff[i * 2] = (byte)_line_diff[i];
                line_diff[i * 2 + 1] = (byte)(_line_diff[i] >> 8);
            }
            ushort StartAddress = start_address_Diff_Line;
            try
            {
                MB_WriteMultipleReg(StartAddress, ref _MB, line_diff, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write diff err: " + ex.Message);
            }
        }
        //doc DIFF cua 7 line lien tiep
        private void MB_Read_Diff(ref ModbusTCP _MB)
        {
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)MB_Task_enum.r1Adf;
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)MB_Task_enum.r2Adf;
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)MB_Task_enum.r3Adf;
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)MB_Task_enum.r4Adf;
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)MB_Task_enum.r5Adf;
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)MB_Task_enum.r6Adf;
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)MB_Task_enum.r7Adf;
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)MB_Task_enum.r8Adf;
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)MB_Task_enum.r9Adf;
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)MB_Task_enum.r10Adf;
            
            try
            {
                MB_Read_Hold_Reg(start_address_Diff_Line, 7, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read diff err: " + ex.Message);
            }
        }
        //doc DIFF cua 1 line bat ki
        private void MB_Read_Diff(ref ModbusTCP _MB, int line_number, ushort Taks_ID)
        {
            if (line_number > 7)
            {
                line_number = line_number % 7;
                if (line_number == 0)
                {
                    line_number = 7;
                }
            }
            ushort StartAddress = (ushort)(start_address_Diff_Line + (line_number - 1));
            try
            {
                MB_Read_Hold_Reg(StartAddress, 1, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read diff err: " + ex.Message);
            }
        }

        //////////////////////////////////////////////////////////////////TACT_TIME
        //Ghi TACT_TIME ch 1 line bat ki
        private void MB_Write_Tact_time(ref ModbusTCP _MB, uint _line_tact_time, int line_number)
        {
            if (line_number > 7)
            {
                line_number = line_number % 7;
                if (line_number == 0)
                {
                    line_number = 7;
                }
            }
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)(MB_Task_enum.w1Att + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)(MB_Task_enum.w2Att + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)(MB_Task_enum.w3Att + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)(MB_Task_enum.w4Att + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)(MB_Task_enum.w5Att + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)(MB_Task_enum.w6Att + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)(MB_Task_enum.w7Att + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)(MB_Task_enum.w8Att + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)(MB_Task_enum.w9Att + (ushort)line_number);
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)(MB_Task_enum.w10Att + (ushort)line_number);
          
            byte[] line_tact_time = new byte[4];
            line_tact_time[0] = (byte)(_line_tact_time >> 24);
            line_tact_time[1] = (byte)(_line_tact_time >> 16);
            line_tact_time[2] = (byte)(_line_tact_time >> 8);
            line_tact_time[3] = (byte)_line_tact_time;
            //8 10 12 14 16
            ushort StartAddress = (ushort)(start_address_Tact_time_Line + (line_number - 1) * 2);
            try
            {
                MB_WriteMultipleReg(StartAddress, ref _MB, line_tact_time, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write tact time err: " + ex.Message);
            }
        }
        //Ghi TACT_TIME cho 7 line lien tiep
        private void MB_Write_Tact_time(ref ModbusTCP _MB, uint[] _line_tact_time)
        {
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)MB_Task_enum.w1Att;
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)MB_Task_enum.w2Att;
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)MB_Task_enum.w3Att;
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)MB_Task_enum.w4Att;
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)MB_Task_enum.w5Att;
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)MB_Task_enum.w6Att;
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)MB_Task_enum.w7Att;
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)MB_Task_enum.w8Att;
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)MB_Task_enum.w9Att;
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)MB_Task_enum.w10Att;
            
            byte[] line_tact_time = new byte[28];
            for (int i = 0; i < 7; i++)
            {
                line_tact_time[i * 4] = (byte)(_line_tact_time[i] >> 24);
                line_tact_time[i * 4 + 1] = (byte)(_line_tact_time[i] >> 16);
                line_tact_time[i * 4 + 2] = (byte)(_line_tact_time[i] >> 8);
                line_tact_time[i * 4 + 3] = (byte)_line_tact_time[i];
            }
            ushort StartAddress = start_address_Tact_time_Line;
            try
            {
                MB_WriteMultipleReg(StartAddress, ref _MB, line_tact_time, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write tact time err: " + ex.Message);
            }
        }
        //doc TACT_TIME cua 7 line lien tiep   ----
        private void MB_Read_Tact_time(ref ModbusTCP _MB)
        {
            ushort Taks_ID = 9999;
            if (_MB.IP_address == DungChung.diachiIP[0]) Taks_ID = (ushort)MB_Task_enum.r1Att;
            else if (_MB.IP_address == DungChung.diachiIP[1]) Taks_ID = (ushort)MB_Task_enum.r2Att;
            else if (_MB.IP_address == DungChung.diachiIP[2]) Taks_ID = (ushort)MB_Task_enum.r3Att;
            else if (_MB.IP_address == DungChung.diachiIP[3]) Taks_ID = (ushort)MB_Task_enum.r4Att;
            else if (_MB.IP_address == DungChung.diachiIP[4]) Taks_ID = (ushort)MB_Task_enum.r5Att;
            else if (_MB.IP_address == DungChung.diachiIP[5]) Taks_ID = (ushort)MB_Task_enum.r6Att;
            else if (_MB.IP_address == DungChung.diachiIP[6]) Taks_ID = (ushort)MB_Task_enum.r7Att;
            else if (_MB.IP_address == DungChung.diachiIP[7]) Taks_ID = (ushort)MB_Task_enum.r8Att;
            else if (_MB.IP_address == DungChung.diachiIP[8]) Taks_ID = (ushort)MB_Task_enum.r9Att;
            else if (_MB.IP_address == DungChung.diachiIP[9]) Taks_ID = (ushort)MB_Task_enum.r10Att;
            
            try
            {
                MB_Read_Hold_Reg(start_address_Tact_time_Line, 14, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read tact time err: " + ex.Message);
            }
        }
        //doc TACT_TIME cua 1 line bat ki
        private void MB_Read_Tact_time(ref ModbusTCP _MB, int line_number, ushort Taks_ID)
        {
            if (line_number > 7)
            {
                line_number = line_number % 7;
                if (line_number == 0)
                {
                    line_number = 7;
                }
            }
            ushort StartAddress = (ushort)(start_address_Tact_time_Line + (line_number - 1) * 2);
            try
            {
                MB_Read_Hold_Reg(StartAddress, 2, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read tact time err: " + ex.Message);
            }
        }

        //////////////////////////////////////////////////////////////////DOC GHI CHO STOCK
        //Ghi STATUS cho stock
        private void MB_Write_stock_Status(ref ModbusTCP _MB, int _stock_status)
        {
            ushort Taks_ID = 9999;
            if (_MB == MB[DungChung.SoPLC])
            {
                Taks_ID = (ushort)(MB_Task_enum.w11Ast);
            }
            if (_MB == MB[DungChung.SoPLC+1])
            {
                Taks_ID = (ushort)(MB_Task_enum.w12Ast);
            }

            byte[] stock_status = new byte[2];
            stock_status[0] = (byte)(_stock_status >> 8);
            stock_status[1] = (byte)_stock_status;
            try
            {
                MB_WriteSingleReg(start_address_Status_Stock, ref _MB, stock_status, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write stock status err: "+ex.ToString());// + ex.Message);
            }

        }
        //doc STATUS cho Stock
        private void MB_Read_stock_Status(ref ModbusTCP _MB)
        {
            ushort Taks_ID = 9999;
            if (_MB == MB[DungChung.SoPLC])
            {
                Taks_ID = (ushort)(MB_Task_enum.r11Ast);
            }
            if (_MB == MB[DungChung.SoPLC+1]) { Taks_ID = (ushort)(MB_Task_enum.r12Ast); }
            try
            {
                MB_Read_Hold_Reg(start_address_Status_Stock, 1, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read stock status err: " + ex.Message);
            }
        }

        //Ghi PLAN cho stock
        private void MB_Write_stock_Plan(ref ModbusTCP _MB, uint _stock_plan)
        {
            ushort Taks_ID = 9999;
            if (_MB == MB[DungChung.SoPLC])
            {
                Taks_ID = (ushort)(MB_Task_enum.w11Apl);
            }
            else { Taks_ID = (ushort)(MB_Task_enum.w12Apl); }

            byte[] stock_plan = new byte[4];
            stock_plan[0] = (byte)(_stock_plan >> 24);
            stock_plan[1] = (byte)(_stock_plan >> 16);
            stock_plan[2] = (byte)(_stock_plan >> 8);
            stock_plan[3] = (byte)_stock_plan;

            ushort StartAddress = start_address_PLan_Stock;
            try
            {
                MB_WriteMultipleReg(StartAddress, ref _MB, stock_plan, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write stock plan err: " + ex.Message);
            }
        }
        //doc PLAN cho Stock
        private void MB_Read_stock_Plan(ref ModbusTCP _MB)
        {
            ushort Taks_ID = 9999;
            if (_MB == MB[DungChung.SoPLC])
            {
                Taks_ID = (ushort)(MB_Task_enum.r11Apl);
            }
            if (_MB == MB[DungChung.SoPLC+1])
            {
                Taks_ID = (ushort)(MB_Task_enum.r12Apl);
            }
            try
            {
                MB_Read_Hold_Reg(start_address_PLan_Stock, 2, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read stock plan err: " + ex.Message);
            }
        }

        //Ghi ACTUAL cho stock
        private void MB_Write_stock_Actual(ref ModbusTCP _MB, uint _stock_actual)
        {
            ushort Taks_ID = 9999;
            if (_MB == MB[DungChung.SoPLC])
            {
                Taks_ID = (ushort)(MB_Task_enum.w11Aat);
            }
            if (_MB == MB[DungChung.SoPLC+1])
            { Taks_ID = (ushort)(MB_Task_enum.w12Aat); }
            byte[] stock_actual = new byte[4];
            stock_actual[0] = (byte)(_stock_actual >> 24);
            stock_actual[1] = (byte)(_stock_actual >> 16);
            stock_actual[2] = (byte)(_stock_actual >> 8);
            stock_actual[3] = (byte)_stock_actual;
            //8 10 12 14 16
            ushort StartAddress = start_address_Actual_Stock;
            try
            {
                MB_WriteMultipleReg(StartAddress, ref _MB, stock_actual, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write stock actual err: " + ex.Message);
            }
        }
        //doc ACTUAL cho Stock
        private void MB_Read_stock_Actual(ref ModbusTCP _MB)
        {
            ushort Taks_ID = 9999;
            if (_MB == MB[DungChung.SoPLC])
            {
                Taks_ID = (ushort)(MB_Task_enum.r11Aat);
            }
            if (_MB == MB[DungChung.SoPLC+1])
            {
                Taks_ID = (ushort)(MB_Task_enum.r12Aat);
            }
            try
            {
                MB_Read_Hold_Reg(start_address_Actual_Stock, 2, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read stock actual err: " + ex.Message);
            }
        }

        //Ghi TARGET cho stock
        private void MB_Write_stock_Target(ref ModbusTCP _MB, uint _stock_target)
        {
            ushort Taks_ID = 9999;
            if (_MB == MB[DungChung.SoPLC])
            {
                Taks_ID = (ushort)(MB_Task_enum.w11Atg);
            }
            if (_MB == MB[DungChung.SoPLC+1])
            { Taks_ID = (ushort)(MB_Task_enum.w12Atg); }
            byte[] stock_target = new byte[4];
            stock_target[0] = (byte)(_stock_target >> 24);
            stock_target[1] = (byte)(_stock_target >> 16);
            stock_target[2] = (byte)(_stock_target >> 8);
            stock_target[3] = (byte)_stock_target;
            //8 10 12 14 16
            ushort StartAddress = start_address_Target_Stock;
            try
            {
                MB_WriteMultipleReg(StartAddress, ref _MB, stock_target, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write stock target err: " + ex.Message);
            }
        }
        //doc TARGET cho Stock
        private void MB_Read_stock_Target(ref ModbusTCP _MB)
        {
            ushort Taks_ID = 9999;
            if (_MB == MB[DungChung.SoPLC])
            {
                Taks_ID = (ushort)(MB_Task_enum.r11Atg);
            }
            if (_MB == MB[DungChung.SoPLC+1])
            {
                Taks_ID = (ushort)(MB_Task_enum.r12Atg);
            }
            try
            {
                MB_Read_Hold_Reg(start_address_Target_Stock, 2, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read stock target err: " + ex.Message);
            }
        }

        //Ghi DIFF cho stock
        private void MB_Write_stock_Diff(ref ModbusTCP _MB, uint _stock_diff)
        {
            ushort Taks_ID = 9999;
            if (_MB == MB[DungChung.SoPLC])
            {
                Taks_ID = (ushort)(MB_Task_enum.w11Adf);
            }
            if (_MB == MB[DungChung.SoPLC+1])
            { Taks_ID = (ushort)(MB_Task_enum.w12Adf); }

            byte[] stock_diff = new byte[4];
            stock_diff[0] = (byte)(_stock_diff >> 24);
            stock_diff[1] = (byte)(_stock_diff >> 16);
            stock_diff[2] = (byte)(_stock_diff >> 8);
            stock_diff[3] = (byte)_stock_diff;
            //8 10 12 14 16
            ushort StartAddress = start_address_Diff_Stock;
            try
            {
                MB_WriteMultipleReg(StartAddress, ref _MB, stock_diff, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write stock diff err: " + ex.Message);
            }
        }
        //doc DIFF TIME cho Stock
        private void MB_Read_stock_Diff(ref ModbusTCP _MB)
        {
            ushort Taks_ID = 9999;
            if (_MB == MB[DungChung.SoPLC])
            {
                Taks_ID = (ushort)(MB_Task_enum.r11Adf);
            }
            if (_MB == MB[DungChung.SoPLC+1])
            {
                Taks_ID = (ushort)(MB_Task_enum.r12Adf);
            }
            try
            {
                MB_Read_Hold_Reg(start_address_Diff_Stock, 2, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read stock diff err: " + ex.Message);
            }
        }

        //Ghi TACT TIME cho stock
        private void MB_Write_stock_Tact_time(ref ModbusTCP _MB, uint _stock_tact_time)
        {
            ushort Taks_ID = 9999;
            if (_MB == MB[DungChung.SoPLC])
            {
                Taks_ID = (ushort)(MB_Task_enum.w11Att);
            }
            if (_MB == MB[DungChung.SoPLC+1])
            { Taks_ID = (ushort)(MB_Task_enum.w12Att); }

            byte[] stock_tact_time = new byte[4];
            stock_tact_time[0] = (byte)(_stock_tact_time >> 24);
            stock_tact_time[1] = (byte)(_stock_tact_time >> 16);
            stock_tact_time[2] = (byte)(_stock_tact_time >> 8);
            stock_tact_time[3] = (byte)_stock_tact_time;
            //8 10 12 14 16
            ushort StartAddress = start_address_Tact_time_Stock;
            try
            {
                MB_WriteMultipleReg(StartAddress, ref _MB, stock_tact_time, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Write stock tact time err: " + ex.Message);
            }
        }
        //Doc TACT TIME cho stock
        private void MB_Read_stock_Tact_time(ref ModbusTCP _MB)
        {
            ushort Taks_ID = 9999;
            if (_MB == MB[DungChung.SoPLC])
            {
                Taks_ID = (ushort)(MB_Task_enum.r11Att);
            }
            if (_MB == MB[DungChung.SoPLC+1])
            {
                Taks_ID = (ushort)(MB_Task_enum.r12Att);
            }
            try
            {
                MB_Read_Hold_Reg(start_address_Tact_time_Stock, 2, ref _MB, Taks_ID);
            }
            catch (Exception ex)
            {
                Display_listbox_stt(_MB.IP_address + " Read stock tact time err: " + ex.Message);
            }
        }
    }
}