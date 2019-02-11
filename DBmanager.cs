using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Windows.Forms;
public class DBmanager
{
    public int idPlan=0;
    public SqlConnection kn = new SqlConnection();
    public void kn_csdl()
    {
        string chuoikn = ConfigurationManager.ConnectionStrings["strConnection"].ConnectionString;
        try
        {
            kn.ConnectionString = chuoikn;
            if (kn.State == ConnectionState.Closed)
            {
                kn.Open();
            }
        }
        catch (Exception)
        {
            MessageBox.Show("khong the ket noi toi co so du lieu", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    //thực thi lệnh SQL
    public int exc(string sql)
    {
        int value;
        // chuoi ket noi
        SqlCommand cmd = new SqlCommand(sql, kn);
        try
        {
            value = Convert.ToInt32(cmd.ExecuteScalar());
        }
        catch (System.Exception)
        {
            value = 0;
        }

        return value;
    }
    public string lay1giatri(string sql)
    {
        string kq = "";
        try
        {
            kn_csdl();

            SqlCommand sqlComm = new SqlCommand(sql, kn);
            SqlDataReader r = sqlComm.ExecuteReader();
            if (r.Read())
            {
                kq = r["Plan"].ToString();
            }
            r.Close();
        }
        catch
        {
        }
        return kq;
    }


    public void dongketnoi()
    {
        if (kn.State == ConnectionState.Open)
        {
            kn.Close();
        }
    }

    public DataTable laybang(string caulenh)//lấy bẳng dữ liệu trong SQL
    {
        DataTable bangdulieu = new DataTable();
        try
        {
            kn_csdl();
            SqlDataAdapter Adapter = new SqlDataAdapter(caulenh, kn);
            DataSet ds = new DataSet();
            //fill bang du lieu
            Adapter.Fill(bangdulieu);
        }
        catch (System.Exception)
        {
            bangdulieu = null;
        }
        finally
        {
            dongketnoi();
        }

        return bangdulieu;
    }

    public int xulydulieu(string caulenhsql)
    {
        int kq = 0;
        try
        {
            kn_csdl();
            SqlCommand lenh = new SqlCommand(caulenhsql, kn);
            kq = lenh.ExecuteNonQuery();
        }
        catch (Exception)
        {
            kq = 0;
        }
        finally
        {
            dongketnoi();
        }
        return kq;
    }
    public string StatusSystem()
    {
        // Start,Stop,Reset
        string status = "Stop";
        return status;
    }
    public bool dangnhap(string id, string pass)
    {
        int check = 0;
        try
        {
            kn_csdl();
            SqlCommand cmd = new SqlCommand("select * from [User] where Username =@username and Password=@password", kn);
            cmd.Parameters.AddWithValue("@username", id);
            cmd.Parameters.AddWithValue("@password", pass);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            check = dt.Rows.Count;
        }
        catch (SqlException)
        {

        }
        finally
        {
            dongketnoi();
        }
        return check > 0;
    }

    public string getColor(string table, string model, string nameColumn)
    {
        string color = "";
        try
        {
            kn_csdl();
            SqlCommand cmd = new SqlCommand("Select " + nameColumn + " from [" + table + "] where [Model] ='" + model + "'", kn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            color = dt.Rows[0][0].ToString();
        }
        catch (Exception ex)
        {
            if (ex is SqlException || ex is IndexOutOfRangeException)
            {
            }
            throw;
        }
        finally
        {
            dongketnoi();
        }
        return color;
    }
    public bool setColor(string table, string line, string column, string color, int PlanId)
    {
        int i = 0;
        try
        {
            kn_csdl();
            SqlCommand cmd = new SqlCommand("UPDATE " + table + " Set " + column + " = @color Where [LineName]= '" + line + "' AND [PlanId] ='" + PlanId + "'", kn);
            cmd.Parameters.Add(new SqlParameter("@color", color));
            i = cmd.ExecuteNonQuery();
        }
        catch (SqlException)
        {

        }
        finally
        {
            dongketnoi();
        }
        return i > 0;
    }

    public bool setCountColor(string table, string line, int count, string namecolumnCount, int PlanId)
    {
        int i = 0;
        try
        {
            kn_csdl();
            SqlCommand cmd = new SqlCommand("UPDATE " + table + " Set " + namecolumnCount + " = '" + count + "' Where [LineName]= '" + line + "' and [PlanId] = '" + PlanId + "'", kn);
            i = cmd.ExecuteNonQuery();
        }
        catch (SqlException)
        {

        }
        finally
        {
            dongketnoi();
        }
        return i > 0;
    }

    public int getCountColor(string table, string line, string columnCount, int PlanId)
    {
        int count = 0;
        try
        {
            kn_csdl();
            SqlCommand cmd = new SqlCommand("Select @colmn from @table Where [LineName]= @Line and [PlanId] = @PlanId", kn);
            cmd.Parameters.AddWithValue("@PlanId", PlanId);
            cmd.Parameters.AddWithValue("@table", table);
            cmd.Parameters.AddWithValue("@Line", line);
            cmd.Parameters.AddWithValue("@colmn", columnCount);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (!int.TryParse(dt.Rows[0][0].ToString(), out count))
                count = 0;
        }
        catch (SqlException)
        {

        }
        finally
        {
            dongketnoi();
        }
        return count;
    }

    public bool setCountColorAll(string table, int count)
    {
        int i = 0;
        try
        {
            kn_csdl();
            SqlCommand cmd = new SqlCommand("UPDATE @color Set [Count] = @count", kn);
            cmd.Parameters.AddWithValue("@color", table);
            cmd.Parameters.AddWithValue("@count", count);
            i = cmd.ExecuteNonQuery();
        }
        catch (SqlException)
        {

        }
        finally
        {
            dongketnoi();
        }
        return i > 0;
    }
    //lay du lieu stocking
    public int Get_stocking_byHour(DateTime t)
    {
        DataTable result = new DataTable();
        int stock_input = 0;
        string date = t.Year.ToString() + "-" + t.Month.ToString() + "-";
        if (t.Day < 10)
        {
            date = date + "0" + t.Day.ToString();
        }
        else
        {
            date = date + t.Day.ToString();
        }
        string H = t.Hour.ToString();
        try
        {
            kn_csdl();
            if (t.Hour < 19 && t.Hour > 6)
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM  Stocking Where CONVERT(date, created_date)= '" + date + "'and DATEPART(hour,created_date)  <'19' and DATEPART(hour,created_date) >'6' ", kn))
                {
                    //command.Parameters.Add(new SqlParameter("@created_date", t.Date));
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(result);
                    int Row_num = result.Rows.Count;
                    for (int i = 0; i < Row_num; i++)
                    {
                        // string s = result.Rows[i]["qty"].ToString();
                        stock_input += Convert.ToInt32(result.Rows[i]["qty"]);
                    }
                    //return planId;
                }
            }
            else
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM  Stocking Where CONVERT(date, created_date)= '" + date + "'and( DATEPART(hour,created_date) >'18' or DATEPART(hour,created_date) <'7' )", kn))
                {
                    //command.Parameters.Add(new SqlParameter("@created_date", t.Date));
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(result);
                    int Row_num = result.Rows.Count;
                    for (int i = 0; i < Row_num; i++)
                    {
                        string s = result.Rows[i]["qty"].ToString();
                        stock_input = Convert.ToInt32(result.Rows[i]["qty"]);
                    }
                    //return planId;
                }
            }
        }
        catch (Exception ex)
        {
            if (ex is SqlException || ex is IndexOutOfRangeException)
            {
            }
            throw;
        }
        finally
        {
            dongketnoi();
        }
        return stock_input;
    }

    //lay du lieu stocking trong tung H (lay gia tri tu 7h-19h cho ca sang va tu 19h - 6h sang ngay hom sau cho ca dem)
    public void Get_stocking_all_H(DateTime t, ref int[] actual_stock)
    {
        #region xac dinh date de lay gia tri
        string date = t.Year.ToString() + "-";
        if (t.Month < 10)
        {
            date = date + "0" + t.Month + "-";
        }
        else
        {
            date = date + t.Month + "-";
        }
        if (t.Day < 10)
        {
            date = date + "0" + t.Day.ToString();
        }
        else
        {
            date = date + t.Day.ToString();
        }
        #endregion
        ///////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////
        try
        {
            kn_csdl();
            #region lay gia tri ban ngay
            if (t.Hour < 19 && t.Hour > 6)
            {
                for (int i = 7; i <= t.Hour; i++)//chi xet tu 7h sang cho den H hien tai
                {
                    actual_stock[i - 7] = 0;
                    //lay ra cac gia tri theo H
                    using (SqlCommand command = new SqlCommand("SELECT * FROM  Stocking Where CONVERT(date, created_date)= '" + date + "'and DATEPART(hour,created_date)  = '" + i + "'", kn))
                    {
                        //command.Parameters.Add(new SqlParameter("@created_date", t.Date));
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        DataTable result = new DataTable();
                        da.Fill(result);
                        if (result != null)//neu lay duoc du lieu
                        {
                            int Row_num = result.Rows.Count;
                            for (int j = 0; j < Row_num; j++)
                            {
                                actual_stock[i - 7] += Convert.ToInt32(result.Rows[j]["qty"]); //lay1giatri du lieu cho khung H[7-19];
                            }
                        }
                    }
                }
            }
            #endregion
            else
            #region lay gia tri ban dem
            {
                if (t.Hour <= 23 && t.Hour >= 19)
                {
                    for (int i = 19; i <= t.Hour; i++)
                    {
                        actual_stock[i - 19] = 0;
                        //lay ra cac gia tri theo H
                        using (SqlCommand command = new SqlCommand("SELECT * FROM  Stocking Where CONVERT(date, created_date)= '" + date + "'and DATEPART(hour,created_date)  = '" + i + "'", kn))
                        {
                            //command.Parameters.Add(new SqlParameter("@created_date", t.Date));
                            SqlDataAdapter da = new SqlDataAdapter(command);
                            DataTable result = new DataTable();
                            da.Fill(result);
                            if (result != null)//neu lay duoc du lieu
                            {
                                int Row_num = result.Rows.Count;
                                for (int j = 0; j < Row_num; j++)
                                {
                                    actual_stock[i - 19] += Convert.ToInt32(result.Rows[j]["qty"]); //lay1giatri du lieu cho khung H[7-19];
                                }
                            }
                        }
                    }
                }
                else
                {
                    //lay gia tri 
                    for (int i = 0; i <= t.Hour; i++)
                    {
                        actual_stock[i + 5] = 0;//gia tri actual trong khung H co index tuong ung
                        //lay ra cac gia tri theo H
                        using (SqlCommand command = new SqlCommand("SELECT * FROM  Stocking Where CONVERT(date, created_date)= '" + date + "'and DATEPART(hour,created_date)  = '" + i + "'", kn))
                        {
                            //command.Parameters.Add(new SqlParameter("@created_date", t.Date));
                            SqlDataAdapter da = new SqlDataAdapter(command);
                            DataTable result = new DataTable();
                            da.Fill(result);
                            if (result != null)//neu lay duoc du lieu
                            {
                                int Row_num = result.Rows.Count;
                                for (int j = 0; j < Row_num; j++)
                                {
                                    actual_stock[i + 5] += Convert.ToInt32(result.Rows[j]["qty"]); //lay1giatri du lieu cho khung H[7-19];
                                }
                            }
                        }
                    }
                    //Lay gia tri cac khung H tu 0 H -> 6H truoc
                    DateTime tmpT = t.AddDays(-1);
                    string date2 = tmpT.Year.ToString() + "-";
                    if (tmpT.Month < 10)
                    {
                        date2 = date2 + "0" + tmpT.Month + "-";
                    }
                    else
                    {
                        date2 = date2 + tmpT.Month + "-";
                    }
                    if (tmpT.Day < 10)
                    {
                        date2 = date2 + "0" + tmpT.Day.ToString();
                    }
                    else
                    {
                        date2 = date2 + tmpT.Day.ToString();
                    }
                    //lay gia tri cac khung H tu 19H->23H cua ngay hom truoc
                    for (int i = 19; i <= 23; i++)
                    {
                        actual_stock[i - 19] = 0;
                        //lay ra cac gia tri theo H
                        using (SqlCommand command = new SqlCommand("SELECT * FROM  Stocking Where CONVERT(date, created_date)= '" + date2 + "'and DATEPART(hour,created_date)  = '" + i + "'", kn))
                        {
                            //command.Parameters.Add(new SqlParameter("@created_date", t.Date));
                            SqlDataAdapter da = new SqlDataAdapter(command);
                            DataTable result = new DataTable();
                            da.Fill(result);
                            if (result != null)//neu lay duoc du lieu
                            {
                                int Row_num = result.Rows.Count;
                                for (int j = 0; j < Row_num; j++)
                                {
                                    actual_stock[i - 19] += Convert.ToInt32(result.Rows[j]["qty"]); //lay1giatri du lieu cho khung H[7-19];

                                }
                            }
                        }
                    }

                }
            }
            #endregion
        }

        catch (Exception ex)
        {
            if (ex is SqlException || ex is IndexOutOfRangeException)
            {
            }
            throw;
        }
        finally
        {
            dongketnoi();
        }
    }
    //lay ID ngay 
    public int findIdByDateTime(DateTime date)
    {
        DataTable result = new DataTable();
        int planId = 0;
        if (date.Hour < 7)
        {
            date = date.AddDays(-1);
        }

        try
        {
            kn_csdl();
            using (SqlCommand command = new SqlCommand("SELECT PlanId FROM  PlanDate Where PlanName = @PlanName", kn))
            {
                command.Parameters.Add(new SqlParameter("@PlanName", date.Date));
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(result);
                if (result.Rows.Count > 0)
                {
                    planId = int.Parse(result.Rows[0][0].ToString());
                }
                //return planId;
            }

        }
        catch (SqlException)
        {
        }
        finally
        {
            dongketnoi();
        }
        idPlan = planId;
        return planId;
    }
    public bool insertWhereConfigIP(bool isLine) {

        return true;
    }
    
}
