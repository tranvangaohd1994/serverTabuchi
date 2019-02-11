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
        //set giá trị sản phẩm vào từng giờ 
        public void loadData(int line, int actual, int diff, int target, DataTable _dt, DateTime t, int PlanId)
        {
            int real = t.Hour;
            double rate = 0;
            if (target != 0)
            {
                rate = actual * 100 / target;
                if (real > 6 && real < 19)
                {
                    db.xulydulieu("Update LineInputDay set Archived_Rate= '" + rate + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");
                }
                else
                {
                    db.xulydulieu("Update LineInputNight set Archived_Rate= '" + rate + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");
                }
            }
            else return;


            //set giá sản phẩm của từng giờ vào database
            #region tung khung gio
            switch (real)
            {
                case 0:
                    {
                        //đếm tổng số Sp từ trước
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H19"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H20"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H21"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H22"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H23"].ToString());
                        //tính số sản phẩm của giờ hiện tại
                        countByHourReal = actual - countByHourBefore;
                        if(countByHourReal >=0)
                        db.xulydulieu("Update LineInputNight set [H00] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H00"].ToString());
                            db.xulydulieu("UPDATE LineInputNight SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputNight set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 1:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H19"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H20"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H21"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H22"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H23"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H00"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        {
                            db.xulydulieu("Update LineInputNight set [H01] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        }
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H01"].ToString());
                            db.xulydulieu("UPDATE LineInputNight SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputNight set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    
                    }
                    break;
                case 2:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H19"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H20"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H21"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H22"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H23"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H00"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H01"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputNight set [H02] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");

                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H02"].ToString());
                            db.xulydulieu("UPDATE LineInputNight SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputNight set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 3:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H19"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H20"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H21"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H22"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H23"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H00"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H01"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H02"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputNight set [H03] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H03"].ToString());
                            db.xulydulieu("UPDATE LineInputNight SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputNight set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 4:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H19"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H20"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H21"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H22"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H23"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H00"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H01"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H02"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H03"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputNight set [H04] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H04"].ToString());
                            db.xulydulieu("UPDATE LineInputNight SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputNight set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 5:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H19"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H20"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H21"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H22"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H23"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H00"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H01"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H02"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H03"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H04"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputNight set [H05] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H05"].ToString());
                            db.xulydulieu("UPDATE LineInputNight SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputNight set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 6:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H19"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H20"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H21"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H22"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H23"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H00"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H01"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H02"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H03"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H04"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H05"].ToString()); ;
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputNight set [H06] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H06"].ToString());
                            db.xulydulieu("UPDATE LineInputNight SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputNight set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 7:
                    {
                        countByHourBefore = 0;
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal > 2000)
                        { // plc chua reset van con du lieu tu ca truoc
                            if (resetDataAt7h == 0)
                            {
                                resetDataAt7h = 1;//co bien
                            }
                        }
                        else if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputDay set [H07] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H07"].ToString());
                            db.xulydulieu("UPDATE LineInputDay SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputDay set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 8:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H07"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputDay set [H08] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H08"].ToString());
                            db.xulydulieu("UPDATE LineInputDay SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputDay set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 9:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H07"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H08"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputDay set [H09] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H09"].ToString());
                            db.xulydulieu("UPDATE LineInputDay SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputDay set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 10:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H07"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H08"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H09"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputDay set [H10] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H10"].ToString());
                            db.xulydulieu("UPDATE LineInputDay SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputDay set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 11:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H07"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H08"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H09"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H10"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputDay set [H11] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H11"].ToString());
                            db.xulydulieu("UPDATE LineInputDay SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputDay set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 12:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H07"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H08"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H09"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H10"].ToString())
                                         + ConvertInt(_dt.Rows[line - 1]["H11"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputDay set [H12] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H12"].ToString());
                            db.xulydulieu("UPDATE LineInputDay SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputDay set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 13:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H07"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H08"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H09"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H10"].ToString())
                                         + ConvertInt(_dt.Rows[line - 1]["H11"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H12"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputDay set [H13] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H13"].ToString());
                            db.xulydulieu("UPDATE LineInputDay SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputDay set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 14:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H07"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H08"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H09"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H10"].ToString())
                                         + ConvertInt(_dt.Rows[line - 1]["H11"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H12"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H13"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputDay set [H14] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H14"].ToString());
                            db.xulydulieu("UPDATE LineInputDay SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputDay set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 15:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H07"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H08"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H09"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H10"].ToString())
                                         + ConvertInt(_dt.Rows[line - 1]["H11"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H12"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H13"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H14"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputDay set [H15] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H15"].ToString());
                            db.xulydulieu("UPDATE LineInputDay SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputDay set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 16:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H07"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H08"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H09"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H10"].ToString())
                        + ConvertInt(_dt.Rows[line - 1]["H11"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H12"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H13"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H14"].ToString())
                        + ConvertInt(_dt.Rows[line - 1]["H15"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputDay set [H16] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H16"].ToString());
                            db.xulydulieu("UPDATE LineInputDay SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputDay set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    
                    }
                    break;
                case 17:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H07"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H08"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H09"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H10"].ToString())
                                         + ConvertInt(_dt.Rows[line - 1]["H11"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H12"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H13"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H14"].ToString())
                                         + ConvertInt(_dt.Rows[line - 1]["H15"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H16"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputDay set [H17] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H17"].ToString());
                            db.xulydulieu("UPDATE LineInputDay SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputDay set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    
                    }
                    break;
                case 18:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H07"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H08"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H09"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H10"].ToString())
                                         + ConvertInt(_dt.Rows[line - 1]["H11"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H12"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H13"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H14"].ToString())
                                         + ConvertInt(_dt.Rows[line - 1]["H15"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H16"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H17"].ToString());
                        
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                            db.xulydulieu("Update LineInputDay set [H18] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H18"].ToString());
                            db.xulydulieu("UPDATE LineInputDay SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H-target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputDay set Archived_Rate= '" + ((countByHourBefore + actual_H)/target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    
                    }
                    break;
                case 19:
                    {
                        countByHourBefore = 0;
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputNight set [H19] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H19"].ToString());
                            db.xulydulieu("UPDATE LineInputNight SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputNight set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 20:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H19"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputNight set [H20] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H20"].ToString());
                            db.xulydulieu("UPDATE LineInputNight SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputNight set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 21:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H19"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H20"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputNight set [H21] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H21"].ToString());
                            db.xulydulieu("UPDATE LineInputNight SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputNight set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 22:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H19"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H20"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H21"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputNight set [H22] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H22"].ToString());
                            db.xulydulieu("UPDATE LineInputNight SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputNight set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
                case 23:
                    {
                        countByHourBefore = ConvertInt(_dt.Rows[line - 1]["H19"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H20"].ToString())
                            + ConvertInt(_dt.Rows[line - 1]["H21"].ToString()) + ConvertInt(_dt.Rows[line - 1]["H22"].ToString());
                        countByHourReal = actual - countByHourBefore;
                        if (countByHourReal >= 0)
                        db.xulydulieu("Update LineInputNight set [H23] = '" + countByHourReal + "' where LineName='" + line + "' and PlanId = '" + PlanId + "'");
                        else
                        {
                            int actual_H = ConvertInt(_dt.Rows[line - 1]["H23"].ToString());
                            db.xulydulieu("UPDATE LineInputNight SET Actual='" + (countByHourBefore + actual_H).ToString() + "',Diff='" + (countByHourBefore + actual_H - target).ToString() + "', [Status]='Run' WHERE LineName = '" + line + "' AND PlanId = '" + PlanId + "'");
                            db.xulydulieu("Update LineInputNight set Archived_Rate= '" + ((countByHourBefore + actual_H) / target).ToString() + "' where LineName ='" + line + "' and PlanId = '" + PlanId + "'");

                        }
                    }
                    break;
            }
            #endregion
        }

        public void loadColorByHour(int line, int actual, string model, int hour, int PlanId, DateTime t)
        {
            int _countByHourReal = 0;
            DateTime real = t;
            string hourstr = this.ConvertTextTime(hour);
            //set màu cho ca ngày
            #region xet mau cho ca ngay
            if (hour >= 7 && hour <= 18)
            {
                for (int i = 17; i < 29; i = i + 2)//duyệt từng ca làm việc, khung giờ làm việc ấy 
                {
                    _countByHourReal = ConvertInt(dt.Rows[line - 1]["H" + hourstr].ToString());
                    timeStart = dt.Rows[line - 1][i].ToString();
                    timeFinish = dt.Rows[line - 1][i + 1].ToString();
                    if (timeStart.Equals("") || timeFinish.Equals(""))
                    {
                        continue;
                    }
                    streamingTimeS = DateTime.ParseExact(timeStart, "HH:mm", System.Globalization.CultureInfo.CurrentCulture);
                    streamingTimeF = DateTime.ParseExact(timeFinish, "HH:mm", System.Globalization.CultureInfo.CurrentCulture);
                    timeSR = real.Subtract(streamingTimeS);
                    timeRF = streamingTimeF.Subtract(real);
                    if (timeSR.TotalSeconds >= 0 && timeRF.TotalSeconds >= 0)
                    {
                        double Tact_Time_ = double.Parse(dt.Rows[line - 1]["Tact_Time"].ToString());
                        if (Tact_Time_ == 0) return;
                        if (streamingTimeS.Hour == real.Hour && streamingTimeF.Hour == real.Hour)
                        {
                            targetReal = Convert.ToInt32(timeSR.TotalSeconds / Tact_Time_);
                            // lay thoi gian luu
                            db.setCountColor("LineColorDay", line.ToString(), targetReal, "[Count" + hourstr + "H]", PlanId);
                            string color = this.setColorCell(rateReal(_countByHourReal, targetReal));
                            db.setColor("LineColorDay", line.ToString(), "[Color" + hourstr + "H]", color, PlanId);
                        }
                        else if (streamingTimeS.Hour == real.Hour && streamingTimeF.Hour != real.Hour)
                        {
                            targetReal = Convert.ToInt32(timeSR.TotalSeconds / Tact_Time_);
                            targetReal = targetReal + db.getCountColor("LineColorDay", line.ToString(), "[Count" + hourstr + "H]", PlanId);
                            string color = this.setColorCell(rateReal(_countByHourReal, targetReal));
                            db.setColor("LineColorDay", line.ToString(), "[Color" + hourstr + "H]", color, PlanId);
                        }
                        else if (streamingTimeS.Hour != real.Hour && streamingTimeF.Hour == real.Hour)
                        {
                            streamingTimeS = this.changeHour(real.Hour);
                            timeSR = real.Subtract(streamingTimeS);
                            targetReal = Convert.ToInt32(timeSR.TotalSeconds / Tact_Time_);
                            db.setCountColor("LineColorDay", line.ToString(), targetReal, "[Count" + hourstr + "H]", PlanId);
                            string color = this.setColorCell(rateReal(_countByHourReal, targetReal));
                            db.setColor("LineColorDay", line.ToString(), "[Color" + hourstr + "H]", color, PlanId);
                        }
                        else
                        {
                            streamingTimeS = this.changeHour(real.Hour);
                            timeSR = real.Subtract(streamingTimeS);
                            targetReal = Convert.ToInt32(timeSR.TotalSeconds / Tact_Time_);
                            string color = this.setColorCell(rateReal(_countByHourReal, targetReal));
                            db.setColor("LineColorDay", line.ToString(), "[Color" + hourstr + "H]", color, PlanId);
                        }
                    }
                }
            }
            #endregion
            else
            #region xet mau cho ca dem
            {
                for (int i = 17; i < 29; i = i + 2)
                {
                    countByHourReal = ConvertInt(dt.Rows[line - 1]["H" + hourstr].ToString());
                    timeStart = dt.Rows[line - 1][i].ToString();
                    timeFinish = dt.Rows[line - 1][i + 1].ToString();
                    if (timeStart.Equals("") || timeFinish.Equals(""))
                    {
                        continue;
                    }
                    TimeSpan streamTimeReal;
                    streamingTimeS = DateTime.ParseExact(timeStart, "HH:mm", System.Globalization.CultureInfo.CurrentCulture);
                    streamingTimeF = DateTime.ParseExact(timeFinish, "HH:mm", System.Globalization.CultureInfo.CurrentCulture);
                    timeSR = real.Subtract(streamingTimeS);
                    timeRF = streamingTimeF.Subtract(real);
                    streamTimeReal = streamingTimeS.Subtract(streamingTimeF);
                    if (timeSR.TotalSeconds >= 0 && timeRF.TotalSeconds >= 0)
                    {
                        double Tact_Time_ = double.Parse(dt.Rows[line - 1]["Tact_Time"].ToString());
                        if (Tact_Time_ == 0) return;
                        if (streamingTimeS.Hour == real.Hour && streamingTimeF.Hour == real.Hour)
                        {
                            targetReal = Convert.ToInt32(timeSR.TotalSeconds / Tact_Time_);
                            // lay thoi gian luu
                            db.setCountColor("ColorNight", line.ToString(), targetReal, "[Count" + hourstr + "H]", PlanId);
                            string color = this.setColorCell(rateReal(countByHourReal, targetReal));
                            db.setColor("LineColorNight", line.ToString(), "[Color" + hourstr + "H]", color, PlanId);
                        }
                        else if (streamingTimeS.Hour == real.Hour && streamingTimeF.Hour != real.Hour)
                        {
                            targetReal = Convert.ToInt32(timeSR.TotalSeconds / Tact_Time_);
                            targetReal = targetReal + db.getCountColor("LineColorNight", line.ToString(), "[Count" + hourstr + "H]", PlanId);
                            string color = this.setColorCell(rateReal(countByHourReal, targetReal));
                            db.setColor("LineColorNight", line.ToString(), "[Color" + hourstr + "H]", color, PlanId);
                        }
                        else if (streamingTimeS.Hour != real.Hour && streamingTimeF.Hour == real.Hour)
                        {
                            streamingTimeS = this.changeHour(real.Hour);
                            timeSR = real.Subtract(streamingTimeS);
                            targetReal = Convert.ToInt32(timeSR.TotalSeconds / Tact_Time_);
                            db.setCountColor("LineColorNight", line.ToString(), targetReal, "[Count" + hourstr + "H]", PlanId);
                            string color = this.setColorCell(rateReal(countByHourReal, targetReal));
                            db.setColor("LineColorNight", line.ToString(), "[Color" + hourstr + "H]", color, PlanId);
                        }
                        else
                        {
                            streamingTimeS = this.changeHour(real.Hour);
                            timeSR = real.Subtract(streamingTimeS);
                            targetReal = Convert.ToInt32(timeSR.TotalSeconds / Tact_Time_);
                            string color = this.setColorCell(rateReal(countByHourReal, targetReal));
                            db.setColor("LineColorNight", line.ToString(), "[Color" + hourstr + "H]", color, PlanId);
                        }
                    }

                    // can them
                    if ((streamTimeReal.TotalMinutes > 0) && ((timeSR.TotalMinutes >= 0 && timeRF.TotalMinutes <= 0) || (timeSR.TotalMinutes <= 0 && timeRF.TotalMinutes >= 0)))
                    {
                        double Tact_Time_ = double.Parse(dt.Rows[line - 1]["Tact_Time"].ToString());
                        if (Tact_Time_ == 0) return; 
                        if (streamingTimeS.Hour == real.Hour && streamingTimeF.Hour == real.Hour)
                        {
                            targetReal = Convert.ToInt32(timeSR.TotalSeconds / Tact_Time_);
                            // lay thoi gian luu
                            db.setCountColor("ColorNight", line.ToString(), targetReal, "[Count" + hourstr + "H]", PlanId);
                            string color = this.setColorCell(rateReal(countByHourReal, targetReal));
                            db.setColor("LineColorNight", line.ToString(), "[Color" + hourstr + "H]", color, PlanId);
                        }
                        else if (streamingTimeS.Hour == real.Hour && streamingTimeF.Hour != real.Hour)
                        {
                            targetReal = Convert.ToInt32(timeSR.TotalSeconds / Tact_Time_);
                            targetReal = targetReal + db.getCountColor("LineColorNight", line.ToString(), "[Count" + hourstr + "H]", PlanId);
                            string color = this.setColorCell(rateReal(countByHourReal, targetReal));
                            db.setColor("LineColorNight", line.ToString(), "[Color" + hourstr + "H]", color, PlanId);
                        }
                        else if (streamingTimeS.Hour != real.Hour && streamingTimeF.Hour == real.Hour)
                        {
                            streamingTimeS = this.changeHour(real.Hour);
                            timeSR = real.Subtract(streamingTimeS);
                            targetReal = Convert.ToInt32(timeSR.TotalSeconds / Tact_Time_);
                            db.setCountColor("LineColorNight", line.ToString(), targetReal, "[Count" + hourstr + "H]", PlanId);
                            string color = this.setColorCell(rateReal(countByHourReal, targetReal));
                            db.setColor("LineColorNight", line.ToString(), "[Color" + hourstr + "H]", color, PlanId);
                        }
                        else
                        {
                            streamingTimeS = this.changeHour(real.Hour);
                            timeSR = real.Subtract(streamingTimeS);
                            targetReal = Convert.ToInt32(timeSR.TotalSeconds / Tact_Time_);
                            string color = this.setColorCell(rateReal(countByHourReal, targetReal));
                            db.setColor("LineColorNight", line.ToString(), "[Color" + hourstr + "H]", color, PlanId);
                        }
                    }
                }
            }
            #endregion
        }
        //kiểm tra line này có được làm việc trong gày hôm nay hay không
        public void check_line_intime(DateTime t)//aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
        {

            int i = 0;
            int ln_index = -1;
            /*TabuchiFix*/
            #region kiem tra 5 Line ban dau
            //kiểm tra các 5 PLC ở line mỗi PLC quản lí  7line : 5*7=35 line 
            for (i = 0; i < DungChung.SoPLC; i++)
            {
              
                if (MB[i] != null)
                {
                    for (int j = 0; j < DungChung.L_PLC[i]; j++)
                    {
                        ln_index++;
                        
                        MB[i].RS[j] = dt.Rows[ln_index]["RS"].ToString();
                        if (MB[i].RS[j].Equals("True"))//kiểm tra line này có được làm việc trong gày hôm nay hay không 
                        {
                            //kiem tra intime
                            MB[i].Intime[j] = false;
                            bool check_start_time = false;

                            for (int k = 17; k < 29; k = k + 2)//cac cot trong data base
                            {
                                //thoi gian bat dau kip (kieu chuoi)
                                timeStart = dt.Rows[ln_index][k].ToString();
                                //thoi gian ket thuc kip
                                timeFinish = dt.Rows[ln_index][k + 1].ToString();
                                if (timeStart.Equals("") || timeFinish.Equals(""))
                                {
                                    continue;
                                }

                                //chuyen kieu chuoi sang kieu datetime
                                if (check_start_time != true)
                                {
                                    Line_base_on_data.Start_time[ln_index] = DateTime.ParseExact(timeStart, "HH:mm",
                                        System.Globalization.CultureInfo.CurrentCulture);
                                }
                                check_start_time = true;
                                Line_base_on_data.Stop_time[ln_index] = DateTime.ParseExact(timeFinish, "HH:mm", System.Globalization.CultureInfo.CurrentCulture);
                                int minute_start = ConvertInt(timeStart.Substring(0, 2)) * 60 + ConvertInt(timeStart.Substring(3, 2));
                                int minute_stop = ConvertInt(timeFinish.Substring(0, 2)) * 60 + ConvertInt(timeFinish.Substring(3, 2));
                                int minute_now = t.Hour * 60 + t.Minute;
                                //kiem tra over 24h
                                int timesr = minute_now - minute_start;
                                int timerf = minute_stop - minute_now;
                                int t24;
                                t24 = minute_start - minute_stop;
                                if (timesr >= 0 && timerf > 0)
                                {
                                    // truong hop thoi gian bat dau va ket thuc cung 1 ngay 
                                    MB[i].Intime[j] = true;
                                    break;
                                }
                                if ((t24 > 0) && ((timesr >= 0 && timerf < 0) || (timesr <= 0 && timerf > 0)))
                                {
                                    //truong hop thoi gian bat dau va ket thuc kip trong 2 ngay khac nhau
                                    MB[i].Intime[j] = true;
                                    break;
                                }
                            }
                        }
                        
                    }
                }
                else
                {
                    ln_index += DungChung.L_PLC[i];
                }
            }
            #endregion        
        }

        //cap nhat cac thong so tu EDB//*TabuchiFix*/
        public void Update_from_EDB()
        {
            int i = 0;
            ///*TabuchiFix*/ update 5 line ban đầu
            for (i = 0; i < DungChung.SoPLC; i++)//soPLC la cac plc cho cac line khon tinh stock
            {
                
                if (plcStatus[i] == true && MB[i] != null)
                {
                    #region update date from EDB
                    if (MB[i].M_connected == true)
                    {
                        //Check status 7 line lien ke
                        MB_Read_Status(ref MB[i]);
                        Thread.Sleep(25);
                        MB_Read_Plan(ref MB[i]);
                        Thread.Sleep(25);
                        MB_Read_Target(ref MB[i]);
                        Thread.Sleep(25);
                        MB_Read_Actual(ref MB[i]);
                        Thread.Sleep(25);
                        MB_Read_Tact_time(ref MB[i]);
                        Thread.Sleep(25);
                        for (int j = 0; j < DungChung.L_PLC[i]; j++)
                        {
                            MB[i].Diff[j] = MB[i].Actual[j] - MB[i].Target[j];
                        }
                        if (DungChung.Enabel_PLC[i] == true)
                        Display_listbox_stt("Connected: " + diachi[i]);
                    }
                    else
                    {
                        if (DungChung.Enabel_PLC[i] == true)
                        Display_listbox_stt("Connection is lost: " + diachi[i]);
                    }
                    #endregion
                }
                else
                {
                    if (DungChung.Enabel_PLC[i] == true)
                    Display_listbox_stt("Link test to PLC: " + diachi[i]+" disconnected");
                }
            }
        }
        //Cap nhat cac gia tri tu database: plan, tact time, change actual?
        //Neu co thay doi thi thuc hien thay doi cho EDB

        //hàm thực thi nếu thay dổi các thông số của  line
        /*TabuchiFix*/
        public void Excute_if_change_param(DateTime t, int PlanId)
        {
            int i = 0;
            int ln_index = -1;
            #region change_param
            for (i = 0; i < DungChung.SoPLC; i++)
            {
                if (plcStatus[i] == true && MB[i] != null)
                {
                    for (int j = 0; j < DungChung.L_PLC[i]; j++)
                    {//plc cuoi khong su dung het 7 line nen phai chan lai

                        ln_index++;
                        //kiem nguoi dung co thay doi actual hay khong
                        //Neu co thi thuc hien ghi len EDB
                        #region ACTUAL CHANGE
                        if (mainThr_1st == false)
                        {
                            string changed = "";
                            changed = dt.Rows[ln_index]["ChangedPlaned"].ToString();
                            //Line_base_on_data.Actual[line_index] = ConvertInt(dt.Rows[line_index]["Actual"].ToString());
                            if (changed.Equals("True"))
                            {
                                check_H_Change(ref MB[i], ln_index, t, PlanId);
                            }
                        }
                        #endregion ACTUAL CHANGE
                        //Cap nhat gia tri plan va tact time tu data base
                        //Neu co thay doi thi thuc hien cap nhat lai plan tact time va target moi
                        #region PLAN - TACTTIME CHANGE
                        Line_base_on_data.Plan[ln_index] = (int)ConvertInt(dt.Rows[ln_index]["Plan"].ToString());
                        Line_base_on_data.Tact_time[ln_index] = ConvertInt2_x1000(dt.Rows[ln_index]["Tact_Time"].ToString());
                        if (mainThr_1st == false)
                        {
                            //kiem tra xem plan va tact time co thay doi hay khong
                            if (MB[i].RS[j] == "True")
                            {
                                if ((MB[i].Plan[j] != Line_base_on_data.Plan[ln_index]) || (MB[i].Tact_time[j] != Line_base_on_data.Tact_time[ln_index]))
                                {
                                    Display_listbox_stt(MB[i].IP_address.ToString() + " PlanPLC= " + (MB[i].Plan[j].ToString() + "  Line.Plan= " + Line_base_on_data.Plan[ln_index].ToString() + "    TactPlc=" + MB[i].Tact_time[j].ToString() + " Line.Tacttime=" + Line_base_on_data.Tact_time[ln_index]));
                                    MB_Write_Plan(ref MB[i], Line_base_on_data.Plan[ln_index], (j + 1));
                                    Thread.Sleep(25);
                                    MB_Write_Tact_time(ref MB[i], Line_base_on_data.Tact_time[ln_index], (j + 1));
                                    Thread.Sleep(25);
                                    //Tinh lai target va ghi lai cho EDB
                                    int new_target = cal_target(t, ln_index, (int)Line_base_on_data.Tact_time[ln_index]);

                                    MB_Write_Target(ref MB[i], new_target, (j + 1));//------->>>


                                    MB[i].Target[j] = new_target;
                                    Thread.Sleep(25);
                                    if (MB[i].Intime[j] == true)
                                    {
                                        Display_listbox_stt(t.ToString() + "  Plan or Task time of Line " + (ln_index + 1) + " (in time) has been changed");
                                    }
                                    else
                                    {
                                        Display_listbox_stt(t.ToString() + "  Plan or Task time of Line " + (ln_index + 1) + " (out time) has been changed");
                                    }
                                }
                            }
                        }
                        #endregion PLAN - TACTTIME CHANGE
                    }
                }
                else {
                    ln_index += DungChung.L_PLC[i];
                }
            }
            #endregion

        }
        //Thong bao trang thai cho EDB/*TabuchiFix*/
        public void Update_work_status(DateTime t)
        {
            int ln_index = -1;
            for (int i = 0; i < DungChung.SoPLC; i++)
            {
                if (plcStatus[i] == true && MB[i] != null)
                {
                    for (int j = 0; j < DungChung.L_PLC[i]; j++)
                    {

                        ln_index++;

                        if (MB[i].RS[j].Equals("True"))
                        {
                            // Line intime
                            if (MB[i].Intime[j] == true)
                            {
                                #region cho start neu chua start
                                //kiem tra xem stt ==2 chua, neu chua thi cho hoat dong
                                if (MB[i].Status[j] != 2 || (mainThr_1st == true))
                                {
                                    MB_Write_Status(ref MB[i], 2, (j + 1));
                                    Thread.Sleep(25);
                                    //thong bao
                                    Display_listbox_stt(t.ToString() + " Line " + (ln_index + 1) + " has been started");
                                }
                                #endregion
                            }

                            #region line not intime
                            else
                            {
                                //Thong bao line dang nghi
                                #region neu line chua stop thi cho stop
                                if ((MB[i].Status[j] != 3) || (mainThr_1st == true))
                                {
                                    //khong cho PLC Sleep nua /*Tabuchifix*/

                                    // MB_Write_Status(ref MB[i], 3, (ln_index + 1));
                                    Thread.Sleep(25);
                                    //thong bao
                                    Display_listbox_stt(t.ToString() + " Line " + (ln_index + 1) + " has been halted");
                                }
                                #endregion
                            }
                            #endregion
                            #region Cap nhat cac thong so cho line_based_on_database
                            Line_base_on_data.Actual[ln_index] = ConvertInt(dt.Rows[ln_index]["Actual"].ToString());
                            #endregion
                        }
                        #region Thong bao line khong hoat dong
                        else
                        {
                            //Thong bao Line Khong hoat dong
                            if ((MB[i].Status[j] != 4) || mainThr_1st == true)
                            {
                                MB_Write_Status(ref MB[i], 4, (j + 1));
                                Thread.Sleep(25);
                                Display_listbox_stt(t.ToString() + " Line " + (ln_index + 1) + " does not work today");
                            }
                        }
                        #endregion
                    }
                }
                else {
                    ln_index += DungChung.L_PLC[i];
                }
            }
        }

        //cap nhat cac thong so vao database/*TabuchiFix*/
        public void Update_to_database(DateTime t, int PlanId)
        {
            int ln_index = -1;
            for (int i = 0; i < DungChung.SoPLC; i++)//sửa thành quét 8PLC
            {
                if (plcStatus[i] == true && MB[i] != null)
                {
                    for (int j = 0; j < DungChung.L_PLC[i]; j++)
                    {
                        ln_index++;
                        //phần dưới này không thay đổi gì cả/*TabuchiFix*/
                        if (MB[i].RS[j].Equals("True"))
                        {
                            #region Line intime
                            if (MB[i].Intime[j] == true)
                            {
                                if (Line_base_on_data.Plan[ln_index] < MB[i].Target[j])
                                {
                                    MB[i].Target[j] = Line_base_on_data.Plan[ln_index];//neu target lon hon plan thi cho target bang Plan
                                }
                                //cap nhat data base;
                                #region cap nhat vao databse
                                if (t.Hour < 19 && t.Hour > 6)
                                {

                                    db.xulydulieu("UPDATE LineInputDay SET Actual='" + (MB[i].Actual[j]).ToString() + "',Diff='" + (MB[i].Diff[j]).ToString() + "', Target='" + MB[i].Target[j] + "', [Status]='Run' WHERE LineName = '" + (ln_index + 1) + "' AND PlanId = '" + PlanId + "'");
                                    loadData((ln_index + 1), MB[i].Actual[j], MB[i].Diff[j], MB[i].Target[j], dt, t, PlanId);
                                    loadColorByHour((ln_index + 1), MB[i].Actual[j], dt.Rows[ln_index]["Model"].ToString(), t.Hour, PlanId, t);
                                }
                                else
                                {
                                    db.xulydulieu("UPDATE LineInputNight SET Actual='" + (MB[i].Actual[j]).ToString() + "',Diff='" + (MB[i].Diff[j]).ToString() + "', Target='" + MB[i].Target[j] + "', [Status]='Run' WHERE LineName = '" + (ln_index + 1) + "' AND PlanId = '" + PlanId + "'");
                                    loadData((ln_index + 1), MB[i].Actual[j], MB[i].Diff[j], MB[i].Target[j], dt, t, PlanId);
                                    loadColorByHour((ln_index + 1), MB[i].Actual[j], dt.Rows[ln_index]["Model"].ToString(), t.Hour, PlanId, t);
                                }
                                #endregion
                            }
                            #endregion
                            #region line not intime
                            else
                            {
                                #region cap nhat database voi thong bao line dang stop
                                if (t.Hour < 19 && t.Hour > 6)
                                {
                                    db.xulydulieu("Update LineInputDay SET [Status]='Stop' WHERE LineName = '" + (ln_index + 1) + "' AND PlanId = '" + PlanId + "'");
                                }
                                else
                                {
                                    db.xulydulieu("Update LineInputNight SET [Status]='Stop' WHERE LineName = '" + (ln_index + 1) + "' AND PlanId = '" + PlanId + "'");
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            //Thong bao Line Khong hoat dong
                            #region Thong bao line khong hoat dong
                            if (MB[i].Status[j] == 4)
                            {
                                if (t.Hour < 19 && t.Hour > 6)
                                {
                                    db.xulydulieu("Update LineInputDay SET [Status]='-' WHERE LineName = '" + (ln_index + 1) + "' AND PlanId = '" + PlanId + "'");
                                }
                                else
                                {
                                    db.xulydulieu("Update LineInputNight SET [Status]='-' WHERE LineName = '" + (ln_index + 1) + "' AND PlanId = '" + PlanId + "'");
                                }
                            }
                            #endregion
                        }
                    }
                }
                else {
                    ln_index += DungChung.L_PLC[i];
                }
            }
        }

        //cap nhat lai target/*TabuchiFix*/
        bool update = false;
        public void Update_target(DateTime t)
        {
            // 5 phut chinh lai 1 lan
            if (((t.Minute + 1) % 5) == 0)
            {
                if (update == true)
                {
                    int ln_index = -1;
                    update = false;
                    for (int i = 0; i < DungChung.SoPLC; i++)
                    {

                        if (plcStatus[i] == true && (MB[i] != null))//Ping va duoc khoi tao
                        {
                            if (MB[i].M_connected == true)//ket noi OK
                            {
                                for (int j = 0; j < DungChung.L_PLC[i]; j++)
                                {
                                    ln_index++;
                                    if (MB[i].RS[j] == "True")//Co san xuat
                                    {
                                        int new_target = cal_target(t, ln_index, (int)Line_base_on_data.Tact_time[ln_index]);
                                        MB_Write_Target(ref MB[i], new_target, (j + 1));
                                        Thread.Sleep(25);
                                        MB[i].Target[j] = new_target;
                                    }
                                }
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
                }
            }
            else { update = true; }
        }

        //Kiem tra khung gio nao khong co mau, thi gan no bang null/*TabuchiFix*/
        public void Set_null_for_white(DateTime t, int PlanId)
        {
            try
            {
                get_color_table(t, PlanId);
                getTablefromDB(t, PlanId);
                /*TabuchiFix*/
                for (int i = 0; i < DungChung.soLine; i++)
                {
                    string RS = dt.Rows[i]["RS"].ToString();
                    if (RS.Equals("True"))
                    {
                        #region xet ca sang
                        if (t.Hour < 19 && t.Hour > 6)
                        {
                                int H = t.Hour;
                                string str_H = "Color";
                                string str_H_act = "";
                                if (H < 10)
                                {
                                    str_H = str_H + "0" + H.ToString() + "H";
                                    str_H_act = "H0" + H.ToString();
                                }
                                else
                                {
                                    str_H = str_H + H.ToString() + "H";
                                    str_H_act = "H" + H.ToString();
                                }
                                string Color_value;
                                string actual_H = dt.Rows[i][str_H_act].ToString();
                                Color_value = color_table.Rows[i][str_H].ToString();
                                //neu mau cua khung H dang xet == null -> set gia tri actual trong database o khung H nay bang null
                                if (Color_value == "NULL" || Color_value=="" )//cho nay nhe
                                {
                                        db.xulydulieu("Update LineInputDay set [" + str_H_act + "] = null where LineName = '" + (i + 1) + "' and PlanId = '" + PlanId + "'");
                                }

                                if (Color_value == "Red")
                                {
                                    if (actual_H == "" || actual_H=="NULL")
                                    {
                                        db.xulydulieu("Update LineInputDay set [" + str_H_act + "] = '0' where LineName = '" + (i + 1) + "' and PlanId = '" + PlanId + "'");
                                    }
                                }
                        }
                        #endregion
                        else
                        /*TabuchiFix*/
                        //sao chỗ này không xét từ 0h-6h sáng nhỉ 
                        #region xet ca toi
                        {
                            if (t.Hour >= 19)
                            {
                                for (int H = t.Hour; H <= 23; H++)
                                {
                                    string str_H = "Color";
                                    string str_H_act = "";

                                    str_H = str_H + H.ToString() + "H";
                                    str_H_act = "H" + H.ToString();

                                    string Color_value;
                                    Color_value = color_table.Rows[i][str_H].ToString();

                                    //neu mau cua khung H dang xet == null -> set gia tri actual trong database o khung H nay bang null
                                    if (Color_value == "NULL")
                                    {
                                        db.xulydulieu("Update LineInputNight set [" + str_H_act + "] = null where LineName='" + (i + 1) + "' and PlanId = '" + PlanId + "'");
                                    }
                                    if (Color_value == "Red")
                                    {
                                        string actual_H = dt.Rows[i][str_H_act].ToString();
                                        if (actual_H == "")
                                        {
                                            db.xulydulieu("Update LineInputNight set [" + str_H_act + "] = '0' where LineName = '" + (i + 1) + "' and PlanId = '" + PlanId + "'");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int H = t.Hour; H <= 6; H++)
                                {
                                    string str_H = "Color";
                                    string str_H_act = "";

                                    str_H = str_H + "0" + H.ToString() + "H";
                                    str_H_act = "H0" + H.ToString();

                                    string Color_value;
                                    Color_value = color_table.Rows[i][str_H].ToString();

                                    //neu mau cua khung H dang xet == null -> set gia tri actual trong database o khung H nay bang null
                                    if (Color_value == "NULL")
                                    {
                                        db.xulydulieu("Update LineInputNight set [" + str_H_act + "] = null where LineName='" + (i + 1) + "' and PlanId = '" + PlanId + "'");
                                    }
                                    if (Color_value == "Red")
                                    {
                                        string actual_H = dt.Rows[i][str_H_act].ToString();
                                        if (actual_H == "")
                                        {
                                            db.xulydulieu("Update LineInputNight set [" + str_H_act + "] = '0' where LineName = '" + (i + 1) + "' and PlanId = '" + PlanId + "'");
                                        }
                                    }
                                }
                            }

                        }
                        #endregion
                    }
                }
            }
            catch {
                MessageBox.Show("chưa tạo kế hoạch cho ca sản xuất !");
            //chua tao ke hoach cho ca san xuat
            }
        }

        //ham kiem tra thay doi actual trong tung khung gio cua database
        //neu co thay doi thi thuc hien cap nhat moi len EDB/*TabuchiFix*/ không thay đổi gì 
        public void check_H_Change(ref ModbusTCP MB, int line_index, DateTime t, int PlanId)
        {
            int line_in_EDB_index = line_index;
            if (line_index > 6)
            {
                line_in_EDB_index = (line_index) % 7;
            }
            //tinh toan actual co the duoc tang them trong truong hop vong lap dang doi 5s de chuyen tiep
            int delta_actual = MB.Actual[line_in_EDB_index] - Line_base_on_data.Actual[line_index];
            //Ghi lai actual
            int New_actual = ConvertInt(dt.Rows[line_index]["Actual"].ToString()) + delta_actual;
            MB_Write_Actual(ref MB, New_actual, line_in_EDB_index + 1);
            Thread.Sleep(50);
            //cap nhat lai truc tiep actual EDB o day vi no giong nhau
            MB.Actual[line_in_EDB_index] = New_actual;
            db.xulydulieu("Update LineInputDay SET ChangedPlaned ='FALSE' WHERE LineName = '" + (line_index + 1) + "' AND PlanId = '" + PlanId + "'");
        }

        ///*TabuchiFix*/ không thay đổi gì hàm này -không sửa
        //ham cap nhat lai toan bo gia tri actual cac khung h den thoi diem dang xet cua tung line
        public void update_H_actual_byline(int line_index, DateTime t)
        {
            if (t.Hour < 19 && t.Hour > 6)//database ca ngay
            {
                for (int j = 0; j < 12; j++)
                {
                    string H_colum = (j + 7).ToString();
                    if (H_colum.Length < 2)
                    {
                        H_colum = "0" + H_colum;
                    }
                    H_colum = "H" + H_colum;
                    Line_base_on_data.H_Actual[line_index, j] = ConvertInt(dt.Rows[line_index][H_colum].ToString());
                }
            }
            else //database ca dem
            {
                for (int j = 0; j < 12; j++)
                {
                    if (j < 5)
                    {
                        string H_colum = (j + 19).ToString();
                        H_colum = "H" + H_colum;
                        Line_base_on_data.H_Actual[line_index, j] = ConvertInt(dt.Rows[line_index][H_colum].ToString());
                    }
                    else
                    {
                        string H_colum = (j - 5).ToString();
                        H_colum = "H0" + H_colum;
                        Line_base_on_data.H_Actual[line_index, j] = ConvertInt(dt.Rows[line_index][H_colum].ToString());
                    }
                }
            }
        }
        
        //ham thuc hien lay thoi gian da lam viec trong gio - không sửa
        public int Get_work_time_H(int Hour, int line_index)
        {
            int work_time_minute = 0;
            string[] from = new string[5];
            string[] to = new string[5];
            int[] Fr_H = new int[5];
            int[] Fr_M = new int[5];
            int[] To_H = new int[5];
            int[] To_M = new int[5];
            for (int i = 0; i < 5; i++)
            {
                string _fr = "From" + (i + 1).ToString();
                string _to = "To" + (i + 1).ToString();
                from[i] = dt.Rows[line_index][_fr].ToString();
                to[i] = dt.Rows[line_index][_to].ToString();
                if (!from[i].Equals(""))
                {
                    Fr_H[i] = ConvertInt(from[i].Substring(0, 2).ToString());
                    Fr_M[i] = ConvertInt(from[i].Substring(3, 2).ToString());
                    To_H[i] = ConvertInt(to[i].Substring(0, 2).ToString());
                    To_M[i] = ConvertInt(to[i].Substring(3, 2).ToString());
                    if (Hour < 19 && Hour > 6)
                    {
                        if (To_H[i] < Hour)
                        { continue; }
                        if (Fr_H[i] < Hour)
                        {
                            if (To_H[i] == Hour)
                            {
                                work_time_minute += To_M[i];//xettiep
                            }
                            if (To_H[i] > Hour)
                            { return work_time_minute = 60; }
                        }
                        if (Fr_H[i] == Hour)
                        {
                            if (To_H[i] == Hour)
                            {
                                work_time_minute += (To_M[i] - Fr_M[i]);//xet tiep
                            }
                            if (To_H[i] > Hour)
                            {
                                return work_time_minute += (60 - Fr_M[i]);
                            }
                        }
                    }
                    else
                    {
                        if (Hour == 23)
                        {
                            if (To_H[i] < Hour)// 
                            { continue; }
                            if (Fr_H[i] < Hour)
                            {
                                if (To_H[i] == Hour)
                                {
                                    work_time_minute += To_M[i];//xettiep
                                }
                                if (To_H[i] < 7)
                                { return work_time_minute = 60; }
                            }
                            if (Fr_H[i] == Hour)
                            {
                                if (To_H[i] == Hour)
                                {
                                    work_time_minute += (To_M[i] - Fr_M[i]);//xet tiep
                                }
                                if (To_H[i] < 7)
                                {
                                    return work_time_minute += (60 - Fr_M[i]);
                                }
                            }
                        }
                        if (Hour < 23 && Hour >= 19)
                        {
                            if (To_H[i] < Hour)
                            { continue; }
                            if (Fr_H[i] == Hour)
                            {
                                if (To_H[i] == Hour)
                                {
                                    work_time_minute += (To_M[i] - Fr_M[i]);//xet tiep
                                }
                                if (To_H[i] > Hour || (To_H[i] > 0 && To_H[i] < 7))
                                {
                                    return work_time_minute += (60 - Fr_M[i]);
                                }
                            }
                            if (Fr_H[i] < Hour)
                            {
                                if (To_H[i] == Hour)
                                {
                                    work_time_minute += To_M[i];//xettiep
                                }
                                if (To_H[i] > Hour || (To_H[i] < 7))
                                {
                                    return work_time_minute = 60;
                                }
                            }
                        }
                        if (Hour >= 0 && Hour < 7)
                        {
                            if (To_H[i] >= 19 || To_H[i] < Hour)
                            { continue; }
                            else
                            {
                                if (Fr_H[i] <= 23 && Fr_H[i] >= 19 || Fr_H[i] < Hour)
                                {
                                    if (To_H[i] > Hour)
                                    {
                                        return work_time_minute = 60;
                                    }
                                    if (To_H[i] == Hour)
                                    {
                                        work_time_minute += To_M[i];
                                    }
                                }
                                if (Fr_H[i] == Hour)
                                {
                                    if (To_H[i] == Hour)
                                    {
                                        work_time_minute += (To_M[i] - Fr_M[i]);
                                    }
                                    if (To_H[i] > Hour)
                                    {
                                        return work_time_minute += (60 - Fr_M[i]);
                                    }
                                }
                            }
                        }

                    }
                }
            }
            return work_time_minute;
        }
        //ham cap nhat color theo gio//không sửa
        public void update_color_by_hour(int Hour, int line_index, DateTime t, int PlanId)
        {
            int work_minute = Get_work_time_H(Hour, line_index);
            if (work_minute != 0)
            {
                string H = Hour.ToString();
                if (H.Length < 2)
                {
                    H = "H0" + H;
                }
                else { H = "H" + H; }
                int actual_inH = ConvertInt(dt.Rows[line_index][H].ToString());
                string hourstr = this.ConvertTextTime(Hour);
                double Tact_Time_ = double.Parse(dt.Rows[line_index]["Tact_Time"].ToString());
                if (Tact_Time_ == 0) return;
                int target_real = Convert.ToInt32(work_minute * 60.0 / Tact_Time_);
                //if (actual_inH != null )
                {
                    string color = this.setColorCell(rateReal(actual_inH, target_real));
                    if (t.Hour < 19 && t.Hour > 6)
                    {
                        //TabuchiFix
                        if (actual_inH >= 0)
                            db.setColor("LineColorDay", (line_index + 1).ToString(), "[Color" + hourstr + "H]", color, PlanId);
                    }
                    else
                    {
                        if (actual_inH >= 0) 
                            db.setColor("LineColorNight", (line_index + 1).ToString(), "[Color" + hourstr + "H]", color, PlanId);
                    }
                }
                }
                else
                {
                    string hourstr = this.ConvertTextTime(Hour);
                    if (t.Hour < 19 && t.Hour > 6)
                    {
                        db.setColor("LineColorDay", (line_index + 1).ToString(), "[Color" + hourstr + "H]", "NULL", PlanId);
                    }
                    else { db.setColor("LineColorNight", (line_index + 1).ToString(), "[Color" + hourstr + "H]", "NULL", PlanId); }
                }
            
        }

        ///*TabuchiFix*/sửa ở đây
        public void update_clolor_all_hour(DateTime t, int PlanId)
        {
            int ln_index = -1;
            for (int i = 0; i < DungChung.SoPLC; i++)
            {
                
                for (int j = 0; j < DungChung.L_PLC[i]; j++)
                {
                    ln_index ++;
                                        
                    if (MB[i] != null)
                    {
                        if (plcStatus[i] == true && MB[i].M_connected == true)
                        {
                            if (t.Hour < 19 && t.Hour > 6)//update ca sang
                            {
                                for (int k = 7; k < t.Hour; k++)
                                {
                                    update_color_by_hour(k, ln_index, t, PlanId);
                                }
                            }
                            else//update ca toi
                            {
                                if (t.Hour <= 23)//ca toi khong qua 0H
                                {
                                    for (int k = 19; k < t.Hour; k++)
                                    {
                                        update_color_by_hour(k, ln_index, t, PlanId);
                                    }
                                }
                                else
                                {
                                    for (int k = 19; k <= 23; k++) //ca toi qua 0H
                                    {
                                        update_color_by_hour(k, ln_index, t, PlanId);
                                    }
                                    for (int k = 0; k < t.Hour; k++)
                                    {
                                        update_color_by_hour(k, ln_index, t, PlanId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //ham tinh thoi gian da lam viec cho den thoi diem hien tai (minute) không sửa 
        public int Update_total_worktime(DateTime Time_C, int line_index)
        {
            int work_time = 0;
            string[] from = new string[5];
            string[] to = new string[5];
            DateTime[] _Fr = new DateTime[5];
            DateTime[] _To = new DateTime[5];
            for (int i = 0; i < 5; i++)
            {
                string _fr = "From" + (i + 1).ToString();
                string _to = "To" + (i + 1).ToString();
                from[i] = dt.Rows[line_index][_fr].ToString();
                to[i] = dt.Rows[line_index][_to].ToString();
                if (!from[i].Equals(""))
                {
                    _Fr[i] = DateTime.ParseExact(from[i], "HH:mm", System.Globalization.CultureInfo.CurrentCulture);
                    _To[i] = DateTime.ParseExact(to[i], "HH:mm", System.Globalization.CultureInfo.CurrentCulture);
                    if (Time_C.Hour < 7)//neu thoi diem hien tai nam trong khaong 0 den 6h 
                    {
                        if (Time_C.Hour > 7) // neu thoi diem hien tai lon hon 6h thi cong them gia tri ngay la 1
                        {
                            Time_C = Time_C.AddDays(-1);
                        }
                        if (_Fr[i].Hour > 7)
                        {
                            _Fr[i] = _Fr[i].AddDays(-1);
                        }
                        if (_To[i].Hour > 7)
                        {
                            _To[i] = _To[i].AddDays(-1);
                        }
                    }
                    int dt1 = (int)(Time_C.Subtract(_Fr[i]).TotalMinutes);
                    int dt2 = (int)(Time_C.Subtract(_To[i]).TotalMinutes);
                    if (dt1 < 0)
                    {
                        break;
                    }
                    if (dt1 >= 0 && dt2 < 0)
                    {
                        work_time += dt1;
                        break;
                    }
                    if (dt1 >= 0 && dt2 >= 0)
                    {
                        work_time += (int)(_To[i].Subtract(_Fr[i]).TotalMinutes);
                    }
                }
            }
            return work_time;
        }

        //ham tinh lai target -không sửa
        public int cal_target(DateTime Time_C, int line_index, int tact_time)
        {
            int target = 0;
            double time_work = Update_total_worktime(Time_C, line_index) * (1.0);
            target = (int)((time_work * 60000.0 + Time_C.Second * 1000.0) / (tact_time * 1.0) + 0.5);
            
            return target;
        }
        //da sua file nay
    }
}