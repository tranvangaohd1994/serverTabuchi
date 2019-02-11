using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

//using COMExcel = Microsoft.Office.Interop.Excel;

namespace Server
{
    public partial class FormAllDetai : Form
    {
        public int PlanId = 0;
        private SqlConnection con;
        private DataTable dtInputDay = new DataTable();
        private DataTable dtColorDay = new DataTable();
        private DataTable dtRS = new DataTable();

        //  private DataTable dtClass = new DataTable("tblClassName");
        private SqlDataAdapter da = new SqlDataAdapter();
        string selecInputDay = @" H07,H08,H09,H10,H11,H12,H13,H14,H15,H16,H17,H18 from LineInputDay ";
        string selecInputNight = @"H19,H20,H21,H22,H23,H00,H01,H02,H03,H04,H05,H06 from LineInputNight ";
        string selecColorDay = @"select    Color07H,Color08H,Color09H,Color10H,Color11H,Color12H,Color13H,Color14H,Color15H,Color16H,Color17H,Color18H  from LineColorDay";
        string selecColorNight = @"select  Color19H,Color20H,Color21H,Color22H,Color23H,Color00H,Color01H,Color02H,Color03H,Color04H,Color05H,Color06H  from LineColorNight";
        bool bitExport = true;
        bool bittomau = true;

        private void connect()
        {
            String cn = @"Data Source = .\SQLEXPRESS ;Initial Catalog=TabuchiEDBfinal; User Id=user1;Password=123456;Integrated Security=false";
            try
            {
                con = new SqlConnection(cn);
                con.Open();//mo ket noi SQL
            }
            catch (Exception)
            {
                MessageBox.Show("khong the ket noi toi co so du lieu", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                Dispose();
            }
        }

        private void disconnect()
        {
            con.Close();
        }

        public FormAllDetai(int _PlanId)
        {
            PlanId = _PlanId;
            InitializeComponent();
            connect();
            if (DateTime.Now.Hour == Main_form.auto_expD_h && DateTime.Now.Minute == Main_form.auto_expD_m || DateTime.Now.Hour == Main_form.auto_expN_h && DateTime.Now.Minute == Main_form.auto_expN_m)
            {
                if (DateTime.Now.Hour == Main_form.auto_expD_h && DateTime.Now.Minute == Main_form.auto_expD_m)
                {
                    // selecInputDay += " Where PlanId = '" + PlanId + "'";
                    getdata(selecInputDay);
                }
                else
                {
                    // selecInputNight += " Where PlanId = '" + PlanId + "'";
                    getdata(selecInputNight);
                }
            }
            else
            {
                if (DateTime.Now.Hour < 19 && DateTime.Now.Hour > 6)
                {
                    //selecInputDay += " Where PlanId = '" + PlanId + "'";
                    getdata(selecInputDay);
                }
                else
                    //selecInputNight += " Where PlanId = '" + PlanId + "'";
                    getdata(selecInputNight);
            }
        }

        /*tabuchiFix*/
        public void getdata(string selecInput)
        {
            dtInputDay.Clear();
            SqlCommand command = new SqlCommand();// khai bao 1 command 
            command.Connection = con;
            command.CommandType = CommandType.Text;//khai báo kiểu command
            command.CommandText = @"select LineName,Rank,ModelA,ModelB,ModelC,
                                           Comments,
                                           Planed_Worker,
                                           Actual_Worker,
                                           [Plan],Actual,Diff,Target,Archived_Rate,
                                            " + selecInput + " where LineName < "+(DungChung.soLine+1).ToString()+" and PlanId = '" + PlanId + "'";//câu truy vấn SQL

            da.SelectCommand = command;
            da.Fill(dtInputDay);//Nạp dữ liệu cho table

            //load RS
            command.CommandText = @"select LineName,RS," + selecInput + " where LineName < " + (DungChung.soLine + 1).ToString() + " and PlanId = '" + PlanId + "'";
            da.SelectCommand = command;
            da.Fill(dtRS);
            for (int i = 0; i < DungChung.soLine; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    if (dtRS.Rows[i][1].ToString() == "False" && j > 0)
                    {
                        if (j >= 6 && j <= 24)
                            dtInputDay.Rows[i][j] = DBNull.Value;
                        else
                            dtInputDay.Rows[i][j] = null;
                    }
                }
            }
            //tao 1 mang tinh gia tri total cac cot
            // grvData.DataSource = dtInputDay;
            int[] total = new int[19];//co 19 gia tri can tinh
            for (int i = 6; i < 25; i++)//quet cac cell de tinh tong
                for (int j = 0; j < dtInputDay.Rows.Count; j++)
                {
                    if (dtInputDay.Rows[j][i].ToString() != "")
                    {
                        total[i - 6] = total[i - 6] + int.Parse(dtInputDay.Rows[j][i].ToString());
                    }
                }
            try
            {
                total[6] = (total[3] * 100 / total[5]);
            }
            catch (Exception)
            {
                total[6] = 100;
            }
            dtInputDay.Rows.Add(DungChung.soLine+1, "", "TOTAL", "", "", "", total[0], total[1], total[2], total[3], total[4], total[5], total[6], total[7], total[8], total[9], total[10], total[11], total[12], total[13], total[14], total[15], total[16], total[17], total[18]);
            //dien tiep line 36 stocking
            command.CommandText = @"select LineName,Rank,ModelA,ModelB,ModelC,
                                           Comments,
                                           Planed_Worker,
                                           Actual_Worker,
                                           [Plan],Actual,Diff,Target,Archived_Rate,
                                            " + selecInput + " where LineName = " + (DungChung.soLine + 1).ToString() + " and PlanId = '" + PlanId + "'";
            da.SelectCommand = command;
            da.Fill(dtInputDay);

            grvData.DataSource = dtInputDay;
            grvData.Rows[DungChung.soLine+1].Cells[0].Value = DungChung.soLine+2;
            grvData.Rows[DungChung.soLine+1].Cells[2].Value = "STOCKING";
        }

        private void tomau(string selecColorInput)
        {
            dtColorDay.Clear();
            // khai bao 1 command 
            SqlCommand command1 = new SqlCommand();
            command1.Connection = con;
            command1.CommandType = CommandType.Text;//khai báo kiểu command
            selecColorInput += " where PlanId = '" + PlanId + "'";
            command1.CommandText = selecColorInput;//câu truy vấn SQL
            da.SelectCommand = command1;
            da.Fill(dtColorDay);//Nạp dữ liệu cho table dtColorDay
            //grvData.DataSource = dtColorDay;
            // quet từng cell tren gridview rồi set màu cho cell ấy theo điều kiện
            for (int i = 0; i < grvData.Rows.Count; i++)
            {

                for (int j = 0; j < 25; j++)
                {

                    //set tất cả các ô là màu trắng trước rồi mới set màu sau
                    grvData.Rows[i].Cells[j].Style.BackColor = Color.White;

                    if (j >= 13 && j < 25 && i < DungChung.soLine)
                    {

                        if (dtColorDay.Rows[i][j - 13].ToString() == "Red") grvData.Rows[i].Cells[j].Style.BackColor = Color.Red;
                        if (dtColorDay.Rows[i][j - 13].ToString() == "Yellow") grvData.Rows[i].Cells[j].Style.BackColor = Color.Yellow;
                        if (dtColorDay.Rows[i][j - 13].ToString() == "GreenYellow") grvData.Rows[i].Cells[j].Style.BackColor = Color.FromArgb(0, 204, 0);
                    }
                    if (j >= 13 && j < 25 && i == (DungChung.soLine+1))
                    {
                        if (dtColorDay.Rows[i - 1][j - 13].ToString() == "Red") grvData.Rows[i].Cells[j].Style.BackColor = Color.Red;
                        if (dtColorDay.Rows[i - 1][j - 13].ToString() == "Yellow") grvData.Rows[i].Cells[j].Style.BackColor = Color.Yellow;
                        if (dtColorDay.Rows[i - 1][j - 13].ToString() == "GreenYellow") grvData.Rows[i].Cells[j].Style.BackColor = Color.FromArgb(0, 204, 0);
                    }
                    if (i < DungChung.soLine && dtRS.Rows[i][1].ToString() == "False" && j > 0)

                        grvData.Rows[i].Cells[j].Style.BackColor = Color.White;

                }
            }
        }

        private void ExportExcel(string day_night)
        {

            //// tạo ứng dụng Excel
            //Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            //// Tạo WorkBook mới 
            //Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            //// tạo Sheet nới 
            //Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            //Microsoft.Office.Interop.Excel.Worksheet _worksheet = new COMExcel.Worksheet();
            //app.Visible = true;
            ////Khai báo Sheet đầu tiên để làm việc
            //worksheet = workbook.Sheets["Sheet1"];
            //worksheet = workbook.ActiveSheet;
            //// Thay đổi tên Sheet
            //worksheet.Name = "Exported from gridview";
            ////Tao tieu de cho file Excel: EXPORT DAY/NIGHT "YYYY-MM-DD"
            //#region tao tieu de
            //string colum_num = grvData.Columns.Count.ToString();
            //worksheet.get_Range("A1:Y1").Merge();//hop tat cac cac cell dong dau tien lam 1
            //string tittle = "";
            //string Month = "";
            //string Day = "";
            //DateTime T = DateTime.Now;
            //if (day_night == "Day")
            //{
            //    Month = DateTime.Now.Month.ToString();
            //    Day = DateTime.Now.Day.ToString();
            //    if (Month.Length < 2)
            //    {
            //        Month = "0" + Month;
            //    }
            //    if (Day.Length < 2)
            //    {
            //        Day = "0" + Day;
            //    }
            //    tittle = "REPORT DAY " + DateTime.Now.Year.ToString() + " - " + Month + " - " + Day;
            //}
            //if (day_night == "Night")
            //{
            //    if (T.Hour < 7)
            //    {
            //        T = T.AddDays(-1);
            //    }
            //    Month = T.Month.ToString();
            //    Day = T.Day.ToString();
            //    if (Month.Length < 2)
            //    {
            //        Month = "0" + Month;
            //    }
            //    if (Day.Length < 2)
            //    {
            //        Day = "0" + Day;
            //    }
            //    tittle = "REPORT NIGHT " + T.Year.ToString() + " - " + Month + " - " + Day;
            //}
            //worksheet.Cells[1, 1] = tittle;
            //COMExcel.Range rangeTittle = worksheet.Cells[1, 1];
            //worksheet.get_Range("A1", "Y1").Font.Bold = true;
            //worksheet.get_Range("A1", "Y1").Font.Size = 18;
            //rangeTittle.HorizontalAlignment = COMExcel.Constants.xlCenter;
            //#endregion
            //// Lưu trữ dữ liệu cho dòng header
            //for (int i = 1; i < grvData.Columns.Count + 1; i++)
            //{
            //    worksheet.Cells[2, i] = grvData.Columns[i - 1].HeaderText;
            //    if (grvData.Columns[i - 1].HeaderText == "Planed_Worker")
            //    {
            //        worksheet.Cells[2, i] = "Planed\nWorker";
            //    }
            //    if (grvData.Columns[i - 1].HeaderText == "Actual_Worker")
            //    {
            //        worksheet.Cells[2, i] = "Actual\nWorker";
            //    }
            //    if (grvData.Columns[i - 1].HeaderText == "Archived_Rate")
            //    {
            //        worksheet.Cells[2, i] = "Archived\nRate";
            //    }
            //    if (grvData.Columns[i - 1].HeaderText == "LineName")
            //    {
            //        worksheet.Cells[2, i] = "Line\nName";
            //    }
            //    COMExcel.Range range = worksheet.Cells[2, i];
            //    range.Borders.Color = Color.Black;
            //    range.HorizontalAlignment = COMExcel.Constants.xlCenter;
            //}
            //worksheet.get_Range("A2", "Y2").Font.Bold = true;
            //// Lưu trữ dữ liệu và màu sắc cho các cell
            //for (int i = 0; i < grvData.Rows.Count - 1; i++)
            //    for (int j = 0; j < grvData.Columns.Count; j++)
            //    {
            //        COMExcel.Range range = worksheet.Cells[i + 3, j + 1];
            //        //lưu màu của cell
            //        range.Interior.Color = grvData.Rows[i].Cells[j].Style.BackColor;
            //        //tạo viền đen cho đường biên
            //        range.Borders.Color = Color.Black;
            //        //range.DisplayFormat.Font.ThemeColor = Color.GreenYellow;
            //        range.HorizontalAlignment = COMExcel.Constants.xlCenter;//COMExcel.Constants.xlCenter;
            //        //luu dứ liệu cho cell
            //        if (j == 12 && grvData.Rows[i].Cells[j].Value != DBNull.Value) worksheet.Cells[i + 3, j + 1] = grvData.Rows[i].Cells[j].Value.ToString() + "%";
            //        else worksheet.Cells[i + 3, j + 1] = grvData.Rows[i].Cells[j].Value.ToString();
            //    }
            //worksheet.get_Range("A1", "Y1").Font.Bold = true;
            //worksheet.Columns.AutoFit();
            //if (day_night == "Night")
            //{
            //    if (!Directory.Exists("D:\\EDB\\Night_EDB"))
            //    {
            //        Directory.CreateDirectory("D:\\EDB\\Night_EDB");
            //    }
            //}
            //if (day_night == "Day")
            //{
            //    if (!Directory.Exists("D:\\EDB\\Day_EDB"))
            //    {
            //        Directory.CreateDirectory("D:\\EDB\\Day_EDB");
            //    }
            //}
            //int chiso = 0;
            //DateTime Tnow = DateTime.Now;
            //string s = "";
            //string[] files = new string[] { };
            //if (day_night == "Night")
            //{
            //    Tnow = Tnow.AddDays(-1);
            //    s = "D:\\EDB\\Night_EDB\\EDB_" + Tnow.Year.ToString() + "_" + Tnow.Month.ToString() + "_" + Tnow.Day.ToString() + "_" + day_night + "_" + Tnow.Hour.ToString() + "_" + Tnow.Minute.ToString() + " .xls";
            //    files = Directory.GetFiles("D:\\EDB\\Night_EDB");
            //}
            //else
            //{
            //    s = "D:\\EDB\\Day_EDB\\EDB_" + Tnow.Year.ToString() + "_" + Tnow.Month.ToString() + "_" + Tnow.Day.ToString() + "_" + day_night + "_" + Tnow.Hour.ToString() + "_" + Tnow.Minute.ToString() + " .xls";
            //    files = Directory.GetFiles("D:\\EDB\\Day_EDB");
            //}

            //for (int i = 0; i < files.Length; i++)
            //{
            //    if (String.Compare(s, files[i]) == 0)
            //    {
            //        chiso++;
            //        s = s.Split(' ')[0];
            //        s = s + " (" + chiso.ToString() + ").xls";
            //        i = -1;
            //    }
            //}

            //// Lưu file D:\\output.xls
            //workbook.SaveAs(s, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            ////Missing la de trong ko dien vao
            //// Thoát khỏi ứng dụng Excel 
            //app.Quit();
        }

        private void grvData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btExit_Click(object sender, EventArgs e)
        {
            disconnect();
            Close();
            Dispose();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            //tu dong xuat ra file Excel

            if (DateTime.Now.Hour == Main_form.auto_expD_h && DateTime.Now.Minute == Main_form.auto_expD_m && bitExport == true)
            {
                connect();
                getdata(selecInputDay);
                tomau(selecColorDay);
                ExportExcel("Day");
                bitExport = false;
                disconnect();
                Close();
            }
            else
            {
                if (DateTime.Now.Hour == Main_form.auto_expN_h && DateTime.Now.Minute == Main_form.auto_expN_m && bitExport == true)
                {
                    connect();
                    getdata(selecInputNight);
                    tomau(selecColorNight);
                    ExportExcel("Night");
                    bitExport = false;
                    disconnect();
                    Close();
                }
                else
                {
                    bitExport = true;

                    if (bittomau == true && DateTime.Now.Hour < 19 && DateTime.Now.Hour > 6)
                    {
                        tomau(selecColorDay);
                        bittomau = false;
                    }
                    if (bittomau == true && DateTime.Now.Hour > 18 || DateTime.Now.Hour < 7)
                    {
                        tomau(selecColorNight);
                        bittomau = false;
                    }
                }

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
    }
}
