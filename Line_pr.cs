using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Line_pr
    {
        public DateTime[] Start_time { get; set; }
        public DateTime[] Stop_time { get; set; }
        public string[] RS { get; set; }
        public bool[] In_time { get; set; }
        public int[] Status { get; set; }
        public int[] Plan { get; set; }
        public int[] Actual{ get; set; }
        public int[,] H_Actual { get; set; } //cai nay de luu actual trong tung h
        public int[] Diff { get; set; }
        public int[] Target { get; set; }
        public uint[] Tact_time { get; set; }

        public string [] RS_Stock { get; set; }
        public bool[] In_time_Stock { get; set; }
        public int[] Status_Stock { get; set; }
        public uint[] Plan_Stock { get; set; }
        public uint[] Actual_Stock { get; set; }
        public int[] Diff_Stock { get; set; }
        public uint[] Target_Stock { get; set; }
        public uint[] Tact_time_Stock { get; set; }
        public uint[,] H_Actual_Stock { get; set; }
        public Line_pr()
        {
            /*TabuchiFix*///thêm 3 phần tử cho mảng các giá trị 
            Start_time = new DateTime[DungChung.soLine];
            Stop_time = new DateTime[DungChung.soLine];
            RS = new string[DungChung.soLine];
            In_time = new bool[DungChung.soLine];
            Status = new int[DungChung.soLine];
            Plan = new int[DungChung.soLine];
            Actual = new int[DungChung.soLine];
            Diff = new int[DungChung.soLine];
            Target = new int[DungChung.soLine];
            Tact_time = new uint[DungChung.soLine];
            H_Actual = new int[DungChung.soLine, 12];

            RS_Stock = new string[2];
            In_time_Stock = new bool[2];
            Status_Stock = new int[2];
            Plan_Stock = new uint[2];
            Actual_Stock = new uint[2];
            Diff_Stock = new int[2];
            Target_Stock = new uint[2];
            Tact_time_Stock = new uint[2];
            H_Actual_Stock = new uint[2,12];
            for (int i = 0; i < DungChung.soLine; i++) 
            {
                RS[i] = "False";
                In_time[i] = false;
                Status[i] = 0;
                Plan[i] = 0;
                Actual[i] = 0;
                Diff[i] = 0;
                Target[i] = 0;
                Tact_time[i] = 0;
                for (int j = 0; j < 12; j++)
                {
                    H_Actual[i, j] = 0;
                }
            }
            for (int i = 0; i < 2; i++)
            {
                RS_Stock[i] = "False";
                In_time_Stock[i] = false;
                Status_Stock[i] = 0;
                Plan_Stock[i] = 0;
                Actual_Stock[i] = 0;
                Diff_Stock[i] = 0;
                Target_Stock[i] = 0;
                Tact_time_Stock[i] = 0;
                for (int j = 0; j < 12; j++)
                {
                    H_Actual_Stock[i, j] = 0;
                }
            }
        }
    }
}
