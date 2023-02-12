using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class DungChung
    {
        public static bool[] Enabel_PLC = { true, true, true, true, true, true, true, true, true, true, true, true };
        public static string[] diachiIP = new string[15] { "192.168.15.241", "192.168.15.242", "192.168.15.243", "192.168.15.244",
                                                             "192.168.15.245", "192.168.15.248", "192.168.15.238", "192.168.15.239",
                                                                "192.168.15.246", "192.168.15.247","","","","","" };//em sua day nua
        
        //su dung de kiem tra da duoc reset init trong ca san xuat hay chua
        public static bool[] initted = new bool[12] { true, true, true, true, true, true, true, true, true, true, true, true };
        public static bool[] reseted = new bool[12] { true, true, true, true, true, true, true, true, true, true, true, true };
        public static int[] L_PLC = new int[12] { 7, 7, 7, 5, 7, 3, 7, 4, 0, 0, 0, 0 };
        //fix 2017-05-11
        public static int SoPLC = 8;//so luong PLC quan li cac line chua ke 2 stock
        public static int soLine = 47;
    }
}
