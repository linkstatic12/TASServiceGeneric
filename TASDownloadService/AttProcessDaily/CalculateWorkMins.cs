using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TASDownloadService.Model;

namespace TASDownloadService.AttProcessDaily
{
    public static class CalculateWorkMins
    {
        #region -- Calculate Work Times --

        public static void CalculateShiftTimes(AttData attendanceRecord, Shift shift)
        {
            try
            {
                //Calculate WorkMin
                attendanceRecord.Remarks = "";
                TimeSpan mins = (TimeSpan)(attendanceRecord.TimeOut - attendanceRecord.TimeIn);
                //Check if GZ holiday then place all WorkMin in GZOTMin
                if (attendanceRecord.StatusGZ == true)
                {
                    attendanceRecord.GZOTMin = (short)mins.TotalMinutes;
                    attendanceRecord.WorkMin = (short)mins.TotalMinutes;
                    attendanceRecord.StatusGZOT = true;
                    attendanceRecord.Remarks = attendanceRecord.Remarks + "[G-OT]";
                }
                //if Rest day then place all WorkMin in OTMin
                else if (attendanceRecord.StatusDO == true)
                {
                    if (attendanceRecord.Emp.HasOT != false)
                    {
                        attendanceRecord.OTMin = (short)mins.TotalMinutes;
                        attendanceRecord.WorkMin = (short)mins.TotalMinutes;
                        attendanceRecord.StatusOT = true;
                        attendanceRecord.Remarks = attendanceRecord.Remarks + "[R-OT]";
                        // RoundOff Overtime
                        if ((attendanceRecord.Emp.EmpType.CatID == 2 || attendanceRecord.Emp.EmpType.CatID == 4) && attendanceRecord.Emp.CompanyID == 1)
                        {
                            if (attendanceRecord.OTMin > 0)
                            {
                                float OTmins = (float)attendanceRecord.OTMin;
                                float remainder = OTmins / 60;
                                int intpart = (int)remainder;
                                double fracpart = remainder - intpart;
                                if (fracpart < 0.5)
                                {
                                    attendanceRecord.OTMin = (short)(intpart * 60);
                                }
                            }
                        }
                    }
                }
                else
                {
                    /////////// to-do -----calculate Margins for those shifts which has break mins 
                    if (shift.HasBreak == true)
                    {
                        attendanceRecord.WorkMin = (short)(mins.TotalMinutes - shift.BreakMin);
                        attendanceRecord.ShifMin = (short)(ProcessSupportFunc.CalculateShiftMinutes(shift, attendanceRecord.AttDate.Value.DayOfWeek) - (short)shift.BreakMin);
                    }
                    else
                    {
                        attendanceRecord.Remarks.Replace("[Absent]", "");
                        attendanceRecord.StatusAB = false;
                        attendanceRecord.StatusP = true;
                        //Calculate Late IN, Compare margin with Shift Late In
                        if (attendanceRecord.TimeIn.Value.TimeOfDay > attendanceRecord.DutyTime)
                        {
                            TimeSpan lateMinsSpan = (TimeSpan)(attendanceRecord.TimeIn.Value.TimeOfDay - attendanceRecord.DutyTime);
                            if (lateMinsSpan.TotalMinutes > shift.LateIn)
                            {
                                attendanceRecord.LateIn = (short)lateMinsSpan.TotalMinutes;
                                attendanceRecord.StatusLI = true;
                                attendanceRecord.EarlyIn = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks + "[LI]";
                            }
                            else
                            {
                                attendanceRecord.StatusLI = null;
                                attendanceRecord.LateIn = null;
                                attendanceRecord.Remarks.Replace("[LI]", "");
                            }
                        }
                        else
                        {
                            attendanceRecord.StatusLI = null;
                            attendanceRecord.LateIn = null;
                            attendanceRecord.Remarks.Replace("[LI]", "");
                        }

                        //Calculate Early In, Compare margin with Shift Early In
                        if (attendanceRecord.TimeIn.Value.TimeOfDay < attendanceRecord.DutyTime)
                        {
                            TimeSpan EarlyInMinsSpan = (TimeSpan)(attendanceRecord.DutyTime - attendanceRecord.TimeIn.Value.TimeOfDay);
                            if (EarlyInMinsSpan.TotalMinutes > shift.EarlyIn)
                            {
                                attendanceRecord.EarlyIn = (short)EarlyInMinsSpan.TotalMinutes;
                                attendanceRecord.StatusEI = true;
                                attendanceRecord.LateIn = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks + "[EI]";
                            }
                            else
                            {
                                attendanceRecord.StatusEI = null;
                                attendanceRecord.EarlyIn = null;
                                attendanceRecord.Remarks.Replace("[EI]", "");
                            }
                        }
                        else
                        {
                            attendanceRecord.StatusEI = null;
                            attendanceRecord.EarlyIn = null;
                            attendanceRecord.Remarks.Replace("[EI]", "");
                        }

                        // CalculateShiftEndTime = ShiftStart + DutyHours
                        DateTime shiftEnd = ProcessSupportFunc.CalculateShiftEndTimeWithAttData(attendanceRecord.AttDate.Value, attendanceRecord.DutyTime.Value, attendanceRecord.ShifMin);

                        //Calculate Early Out, Compare margin with Shift Early Out
                        if (attendanceRecord.TimeOut < shiftEnd)
                        {
                            TimeSpan EarlyOutMinsSpan = (TimeSpan)(shiftEnd - attendanceRecord.TimeOut);
                            if (EarlyOutMinsSpan.TotalMinutes > shift.EarlyOut)
                            {
                                attendanceRecord.EarlyOut = (short)EarlyOutMinsSpan.TotalMinutes;
                                attendanceRecord.StatusEO = true;
                                attendanceRecord.LateOut = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks + "[EO]";
                            }
                            else
                            {
                                attendanceRecord.StatusEO = null;
                                attendanceRecord.EarlyOut = null;
                                attendanceRecord.Remarks.Replace("[EO]", "");
                            }
                        }
                        else
                        {
                            attendanceRecord.StatusEO = null;
                            attendanceRecord.EarlyOut = null;
                            attendanceRecord.Remarks.Replace("[EO]", "");
                        }
                        //Calculate Late Out, Compare margin with Shift Late Out
                        if (attendanceRecord.TimeOut > shiftEnd)
                        {
                            TimeSpan LateOutMinsSpan = (TimeSpan)(attendanceRecord.TimeOut - shiftEnd);
                            if (LateOutMinsSpan.TotalMinutes > shift.LateOut)
                            {
                                attendanceRecord.LateOut = (short)LateOutMinsSpan.TotalMinutes;
                                // Late Out cannot have an early out, In case of poll at multiple times before and after shiftend
                                attendanceRecord.EarlyOut = null;
                                attendanceRecord.StatusLO = true;
                                attendanceRecord.Remarks = attendanceRecord.Remarks + "[LO]";
                            }
                            else
                            {
                                attendanceRecord.StatusLO = null;
                                attendanceRecord.LateOut = null;
                                attendanceRecord.Remarks.Replace("[LO]", "");
                            }
                        }
                        else
                        {
                            attendanceRecord.StatusLO = null;
                            attendanceRecord.LateOut = null;
                            attendanceRecord.Remarks.Replace("[LO]", "");
                        }

                        //Subtract EarlyIn and LateOut from Work Minutes
                        //////-------to-do--------- Automate earlyin,lateout from shift setup
                        attendanceRecord.WorkMin = (short)(mins.TotalMinutes);
                        if (attendanceRecord.EarlyIn != null && attendanceRecord.EarlyIn > shift.EarlyIn)
                        {
                            attendanceRecord.WorkMin = (short)(attendanceRecord.WorkMin - attendanceRecord.EarlyIn);
                        }
                        if (attendanceRecord.LateOut != null && attendanceRecord.LateOut > shift.LateOut)
                        {
                            attendanceRecord.WorkMin = (short)(attendanceRecord.WorkMin - attendanceRecord.LateOut);
                        }
                        if (attendanceRecord.LateOut != null || attendanceRecord.EarlyIn != null)

                            // round off work mins if overtime less than shift.OverTimeMin >
                            if (attendanceRecord.WorkMin > attendanceRecord.ShifMin && (attendanceRecord.WorkMin <= (attendanceRecord.ShifMin + shift.OverTimeMin)))
                            {
                                attendanceRecord.WorkMin = attendanceRecord.ShifMin;
                            }
                        //Calculate OverTime = OT, Compare margin with Shift OverTime
                        //----to-do----- Handle from shift
                        //if (attendanceRecord.EarlyIn > shift.EarlyIn || attendanceRecord.LateOut > shift.LateOut)
                        //{
                        //    if (attendanceRecord.StatusGZ != true || attendanceRecord.StatusDO != true)
                        //    {
                        //        short _EarlyIn;
                        //        short _LateOut;
                        //        if (attendanceRecord.EarlyIn == null)
                        //            _EarlyIn = 0;
                        //        else
                        //            _EarlyIn = 0;

                        //        if (attendanceRecord.LateOut == null)
                        //            _LateOut = 0;
                        //        else
                        //            _LateOut = (short)attendanceRecord.LateOut;

                        //        attendanceRecord.OTMin = (short)(_EarlyIn + _LateOut);
                        //        attendanceRecord.StatusOT = true;
                        //        attendanceRecord.Remarks = attendanceRecord.Remarks + "[N-OT]";
                        //    }
                        //}
                        if ((attendanceRecord.StatusGZ != true || attendanceRecord.StatusDO != true) && attendanceRecord.Emp.HasOT == true)
                        {
                            if (attendanceRecord.LateOut != null)
                            {
                                attendanceRecord.OTMin = attendanceRecord.LateOut;
                                attendanceRecord.StatusOT = true;
                                attendanceRecord.Remarks = attendanceRecord.Remarks + "[N-OT]";
                            }
                        }
                        // RoundOff Overtime
                        if ((attendanceRecord.Emp.EmpType.CatID == 2 || attendanceRecord.Emp.EmpType.CatID == 4) && attendanceRecord.Emp.CompanyID == 1)
                        {
                            if (attendanceRecord.OTMin > 0)
                            {
                                float OTmins = (float)attendanceRecord.OTMin;
                                float remainder = OTmins / 60;
                                int intpart = (int)remainder;
                                double fracpart = remainder - intpart;
                                if (fracpart < 0.5)
                                {
                                    attendanceRecord.OTMin = (short)(intpart * 60);
                                }
                            }
                        }
                        //Mark Absent if less than 4 hours
                        if (attendanceRecord.AttDate.Value.DayOfWeek != DayOfWeek.Friday && attendanceRecord.StatusDO != true && attendanceRecord.StatusGZ != true)
                        {
                            short MinShiftMin = (short)shift.MinHrs;
                            if (attendanceRecord.WorkMin < MinShiftMin)
                            {
                                attendanceRecord.StatusAB = true;
                                attendanceRecord.StatusP = false;
                                attendanceRecord.Remarks = "[Absent]";
                            }
                            else
                            {
                                attendanceRecord.StatusAB = false;
                                attendanceRecord.StatusP = true;
                                attendanceRecord.Remarks.Replace("[Absent]", "");
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void CalculateOpenShiftTimes(AttData attendanceRecord, Shift shift)
        {
            try
            {
                //Calculate WorkMin
                if (attendanceRecord != null)
                {
                    if (attendanceRecord.TimeOut != null && attendanceRecord.TimeIn != null)
                    {
                        attendanceRecord.Remarks = "";
                        TimeSpan mins = (TimeSpan)(attendanceRecord.TimeOut - attendanceRecord.TimeIn);
                        //Check if GZ holiday then place all WorkMin in GZOTMin
                        if (attendanceRecord.StatusGZ == true)
                        {
                            attendanceRecord.GZOTMin = (short)mins.TotalMinutes;
                            attendanceRecord.WorkMin = (short)mins.TotalMinutes;
                            attendanceRecord.StatusGZOT = true;
                            attendanceRecord.Remarks = attendanceRecord.Remarks + "[GZ-OT]";
                        }
                        else if (attendanceRecord.StatusDO == true)
                        {
                            attendanceRecord.OTMin = (short)mins.TotalMinutes;
                            attendanceRecord.WorkMin = (short)mins.TotalMinutes;
                            attendanceRecord.StatusOT = true;
                            attendanceRecord.Remarks = attendanceRecord.Remarks + "[R-OT]";
                            // RoundOff Overtime
                            if ((attendanceRecord.Emp.EmpType.CatID == 2 || attendanceRecord.Emp.EmpType.CatID == 4) && attendanceRecord.Emp.CompanyID == 1)
                            {
                                if (attendanceRecord.OTMin > 0)
                                {
                                    float OTmins = (float)attendanceRecord.OTMin;
                                    float remainder = OTmins / 60;
                                    int intpart = (int)remainder;
                                    double fracpart = remainder - intpart;
                                    if (fracpart < 0.5)
                                    {
                                        attendanceRecord.OTMin = (short)(intpart * 60);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (shift.HasBreak == true)
                            {
                                attendanceRecord.WorkMin = (short)(mins.TotalMinutes - shift.BreakMin);
                                attendanceRecord.ShifMin = (short)(ProcessSupportFunc.CalculateShiftMinutes(shift, attendanceRecord.AttDate.Value.DayOfWeek) - (short)shift.BreakMin);
                            }
                            else
                            {
                                attendanceRecord.Remarks.Replace("[Absent]", "");
                                attendanceRecord.StatusAB = false;
                                attendanceRecord.StatusP = true;
                                // CalculateShiftEndTime = ShiftStart + DutyHours
                                //TimeSpan shiftEnd = ProcessSupportFunc.CalculateShiftEndTime(shift, attendanceRecord.AttDate.Value.DayOfWeek);
                                attendanceRecord.WorkMin = (short)(mins.TotalMinutes);
                                //Calculate OverTIme, 
                                if ((mins.TotalMinutes > (attendanceRecord.ShifMin + shift.OverTimeMin)) && attendanceRecord.Emp.HasOT == true)
                                {
                                    attendanceRecord.OTMin = (Int16)(Convert.ToInt16(mins.TotalMinutes) - attendanceRecord.ShifMin);
                                    attendanceRecord.WorkMin = (short)((mins.TotalMinutes) - attendanceRecord.OTMin);
                                    attendanceRecord.StatusOT = true;
                                    attendanceRecord.Remarks = attendanceRecord.Remarks + "[N-OT]";
                                }
                                //Calculate Early Out
                                if (mins.TotalMinutes < (attendanceRecord.ShifMin - shift.EarlyOut))
                                {
                                    Int16 EarlyoutMin = (Int16)(attendanceRecord.ShifMin - Convert.ToInt16(mins.TotalMinutes));
                                    if (EarlyoutMin > shift.EarlyOut)
                                    {
                                        attendanceRecord.EarlyOut = EarlyoutMin;
                                        attendanceRecord.StatusEO = true;
                                        attendanceRecord.Remarks = attendanceRecord.Remarks + "[EO]";
                                    }
                                    else
                                    {
                                        attendanceRecord.StatusEO = null;
                                        attendanceRecord.EarlyOut = null;
                                        attendanceRecord.Remarks.Replace("[EO]", "");
                                    }
                                }
                                else
                                {
                                    attendanceRecord.StatusEO = null;
                                    attendanceRecord.EarlyOut = null;
                                    attendanceRecord.Remarks.Replace("[EO]", "");
                                }
                                // round off work mins if overtime less than shift.OverTimeMin >
                                if (attendanceRecord.WorkMin > attendanceRecord.ShifMin && (attendanceRecord.WorkMin <= (attendanceRecord.ShifMin + shift.OverTimeMin)))
                                {
                                    attendanceRecord.WorkMin = attendanceRecord.ShifMin;
                                }
                                // RoundOff Overtime
                                if ((attendanceRecord.Emp.EmpType.CatID == 2 || attendanceRecord.Emp.EmpType.CatID == 4) && attendanceRecord.Emp.CompanyID == 1)
                                {
                                    if (attendanceRecord.OTMin > 0)
                                    {
                                        float OTmins = (float)attendanceRecord.OTMin;
                                        float remainder = OTmins / 60;
                                        int intpart = (int)remainder;
                                        double fracpart = remainder - intpart;
                                        if (fracpart < 0.5)
                                        {
                                            attendanceRecord.OTMin = (short)(intpart * 60);
                                        }
                                    }
                                }
                                //Mark Absent if less than 4 hours
                                if (attendanceRecord.AttDate.Value.DayOfWeek != DayOfWeek.Friday && attendanceRecord.StatusDO != true && attendanceRecord.StatusGZ != true)
                                {
                                    short MinShiftMin = (short)shift.MinHrs;
                                    if (attendanceRecord.WorkMin < MinShiftMin)
                                    {
                                        attendanceRecord.StatusAB = true;
                                        attendanceRecord.StatusP = false;
                                        attendanceRecord.Remarks = attendanceRecord.Remarks + "[Absent]";
                                    }
                                    else
                                    {
                                        attendanceRecord.StatusAB = false;
                                        attendanceRecord.StatusP = true;
                                        attendanceRecord.Remarks.Replace("[Absent]", "");
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void CalculateRosterTimes(AttData attendanceRecord, Roster roster, Shift _shift)
        {
            try
            {
                TimeSpan mins = (TimeSpan)(attendanceRecord.TimeOut - attendanceRecord.TimeIn);
                attendanceRecord.Remarks = "";
                if (attendanceRecord.StatusGZ == true)
                {
                    attendanceRecord.GZOTMin = (short)mins.TotalMinutes;
                    attendanceRecord.WorkMin = (short)mins.TotalMinutes;
                    attendanceRecord.StatusGZOT = true;
                    attendanceRecord.Remarks = attendanceRecord.Remarks + "[GZ-OT]";
                }
                else if (attendanceRecord.StatusDO == true)
                {
                    attendanceRecord.OTMin = (short)mins.TotalMinutes;
                    attendanceRecord.WorkMin = (short)mins.TotalMinutes;
                    attendanceRecord.StatusOT = true;
                    attendanceRecord.Remarks = attendanceRecord.Remarks + "[R-OT]";
                    // RoundOff Overtime
                    if ((attendanceRecord.Emp.EmpType.CatID == 2 || attendanceRecord.Emp.EmpType.CatID == 4) && attendanceRecord.Emp.CompanyID == 1)
                    {
                        if (attendanceRecord.OTMin > 0)
                        {
                            float OTmins = (float)attendanceRecord.OTMin;
                            float remainder = OTmins / 60;
                            int intpart = (int)remainder;
                            double fracpart = remainder - intpart;
                            if (fracpart < 0.5)
                            {
                                attendanceRecord.OTMin = (short)(intpart * 60);
                            }
                        }
                    }
                }
                else
                {
                    attendanceRecord.Remarks.Replace("[Absent]", "");
                    attendanceRecord.StatusAB = false;
                    attendanceRecord.StatusP = true;
                    ////------to-do ----------handle shift break time
                    //Calculate Late IN, Compare margin with Shift Late In
                    if (attendanceRecord.TimeIn.Value.TimeOfDay > roster.DutyTime)
                    {
                        TimeSpan lateMinsSpan = (TimeSpan)(attendanceRecord.TimeIn.Value.TimeOfDay - attendanceRecord.DutyTime);
                        if (lateMinsSpan.TotalMinutes > _shift.LateIn)
                        {
                            attendanceRecord.LateIn = (short)lateMinsSpan.TotalMinutes;
                            attendanceRecord.StatusLI = true;
                            attendanceRecord.EarlyIn = null;
                            attendanceRecord.Remarks = attendanceRecord.Remarks + "[LI]";
                        }
                        else
                        {
                            attendanceRecord.LateIn = null;
                            attendanceRecord.StatusLI = null;
                            attendanceRecord.Remarks.Replace("[LI]", "");
                        }
                    }
                    else
                    {
                        attendanceRecord.LateIn = null;
                        attendanceRecord.StatusLI = null;
                        attendanceRecord.Remarks.Replace("[LI]", "");
                    }

                    //Calculate Early In, Compare margin with Shift Early In
                    if (attendanceRecord.TimeIn.Value.TimeOfDay < attendanceRecord.DutyTime)
                    {
                        TimeSpan EarlyInMinsSpan = (TimeSpan)(attendanceRecord.DutyTime - attendanceRecord.TimeIn.Value.TimeOfDay);
                        if (EarlyInMinsSpan.TotalMinutes > _shift.EarlyIn)
                        {
                            attendanceRecord.EarlyIn = (short)EarlyInMinsSpan.TotalMinutes;
                            attendanceRecord.StatusEI = true;
                            attendanceRecord.LateIn = null;
                            attendanceRecord.Remarks = attendanceRecord.Remarks + "[EI]";
                        }
                        else
                        {
                            attendanceRecord.StatusEI = null;
                            attendanceRecord.EarlyIn = null;
                            attendanceRecord.Remarks.Replace("[EI]", "");
                        }
                    }
                    else
                    {
                        attendanceRecord.StatusEI = null;
                        attendanceRecord.EarlyIn = null;
                        attendanceRecord.Remarks.Replace("[EI]", "");
                    }

                    // CalculateShiftEndTime = ShiftStart + DutyHours
                    TimeSpan shiftEnd = (TimeSpan)attendanceRecord.DutyTime + (new TimeSpan(0, (int)roster.WorkMin, 0));

                    //Calculate Early Out, Compare margin with Shift Early Out
                    if (attendanceRecord.TimeOut.Value.TimeOfDay < shiftEnd)
                    {
                        TimeSpan EarlyOutMinsSpan = (TimeSpan)(shiftEnd - attendanceRecord.TimeOut.Value.TimeOfDay);
                        if (EarlyOutMinsSpan.TotalMinutes > _shift.EarlyOut)
                        {
                            attendanceRecord.EarlyOut = (short)EarlyOutMinsSpan.TotalMinutes;
                            attendanceRecord.StatusEO = true;
                            attendanceRecord.LateOut = null;
                            attendanceRecord.Remarks = attendanceRecord.Remarks + "[EO]";
                        }
                        else
                        {
                            attendanceRecord.StatusEO = null;
                            attendanceRecord.EarlyOut = null;
                            attendanceRecord.Remarks.Replace("[EO]", "");
                        }
                    }
                    else
                    {
                        attendanceRecord.StatusEO = null;
                        attendanceRecord.EarlyOut = null;
                        attendanceRecord.Remarks.Replace("[EO]", "");
                    }
                    //Calculate Late Out, Compare margin with Shift Late Out
                    if (attendanceRecord.TimeOut.Value.TimeOfDay > shiftEnd)
                    {
                        TimeSpan LateOutMinsSpan = (TimeSpan)(attendanceRecord.TimeOut.Value.TimeOfDay - shiftEnd);
                        if (LateOutMinsSpan.TotalMinutes > _shift.LateOut)
                        {
                            attendanceRecord.LateOut = (short)LateOutMinsSpan.TotalMinutes;
                            // Late Out cannot have an early out, In case of poll at multiple times before and after shiftend
                            attendanceRecord.EarlyOut = null;
                            attendanceRecord.StatusLO = true;
                            attendanceRecord.Remarks = attendanceRecord.Remarks + "[LO]";
                        }
                        else
                        {
                            attendanceRecord.LateOut = null;
                            attendanceRecord.LateOut = null;
                            attendanceRecord.Remarks.Replace("[LO]", "");
                        }
                    }
                    else
                    {
                        attendanceRecord.LateOut = null;
                        attendanceRecord.LateOut = null;
                        attendanceRecord.Remarks.Replace("[LO]", "");
                    }
                    attendanceRecord.WorkMin = (short)(mins.TotalMinutes);
                    if (attendanceRecord.EarlyIn != null && attendanceRecord.EarlyIn > _shift.EarlyIn)
                    {
                        attendanceRecord.WorkMin = (short)(attendanceRecord.WorkMin - attendanceRecord.EarlyIn);
                    }
                    if (attendanceRecord.LateOut != null && attendanceRecord.LateOut > _shift.LateOut)
                    {
                        attendanceRecord.WorkMin = (short)(attendanceRecord.WorkMin - attendanceRecord.LateOut);
                    }
                    if (attendanceRecord.EarlyIn == null && attendanceRecord.LateOut == null)
                    {

                    }
                    //round off work minutes
                    if (attendanceRecord.WorkMin > attendanceRecord.ShifMin && (attendanceRecord.WorkMin <= (attendanceRecord.ShifMin + _shift.OverTimeMin)))
                    {
                        attendanceRecord.WorkMin = attendanceRecord.ShifMin;
                    }
                    // RoundOff Overtime
                    if ((attendanceRecord.Emp.EmpType.CatID == 2 || attendanceRecord.Emp.EmpType.CatID == 4) && attendanceRecord.Emp.CompanyID == 1)
                    {
                        if (attendanceRecord.OTMin > 0)
                        {
                            float OTmins = (float)attendanceRecord.OTMin;
                            float remainder = OTmins / 60;
                            int intpart = (int)remainder;
                            double fracpart = remainder - intpart;
                            if (fracpart < 0.5)
                            {
                                attendanceRecord.OTMin = (short)(intpart * 60);
                            }
                        }
                    }
                    ////Calculate OverTime, Compare margin with Shift OverTime
                    //if (attendanceRecord.EarlyIn > _shift.EarlyIn || attendanceRecord.LateOut > _shift.LateOut)
                    //{
                    //    if (attendanceRecord.StatusGZ != true || attendanceRecord.StatusDO != true)
                    //    {
                    //        short _EarlyIn;
                    //        short _LateOut;
                    //        if (attendanceRecord.EarlyIn == null)
                    //            _EarlyIn = 0;
                    //        else
                    //            _EarlyIn = 0;

                    //        if (attendanceRecord.LateOut == null)
                    //            _LateOut = 0;
                    //        else
                    //            _LateOut = (short)attendanceRecord.LateOut;

                    //        attendanceRecord.OTMin = (short)(_EarlyIn + _LateOut);
                    //        attendanceRecord.StatusOT = true;
                    //    }
                    //}
                    if ((attendanceRecord.StatusGZ != true || attendanceRecord.StatusDO != true) && attendanceRecord.Emp.HasOT == true)
                    {
                        if (attendanceRecord.LateOut != null)
                        {
                            attendanceRecord.OTMin = attendanceRecord.LateOut;
                            attendanceRecord.StatusOT = true;
                            attendanceRecord.Remarks = attendanceRecord.Remarks + "[N-OT]";
                        }
                    }
                    //Mark Absent if less than 4 hours
                    if (attendanceRecord.AttDate.Value.DayOfWeek != DayOfWeek.Friday && attendanceRecord.StatusDO != true && attendanceRecord.StatusGZ != true)
                    {
                        short MinShiftMin = (short)_shift.MinHrs;
                        if (attendanceRecord.WorkMin < MinShiftMin)
                        {
                            attendanceRecord.StatusAB = true;
                            attendanceRecord.StatusP = false;
                            attendanceRecord.Remarks = attendanceRecord.Remarks + "[Absent]";
                        }
                        else
                        {
                            attendanceRecord.StatusAB = false;
                            attendanceRecord.StatusP = true;
                            attendanceRecord.Remarks.Replace("[Absent]", "");
                        }

                    }
                }


            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }
}
