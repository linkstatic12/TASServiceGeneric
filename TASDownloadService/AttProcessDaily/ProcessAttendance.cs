using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMSFFService;
using TASDownloadService.Model;
using TASDownloadService.AttProcessDaily;

namespace TASDownloadService
{
    public class ProcessAttendance
    {
        #region --Process Daily Attendance--
        TAS2013Entities context = new TAS2013Entities();
        Emp employee = new Emp();
        public void ProcessDailyAttendance()
        {
            DateTime _dt = GlobalSettings._dateTime;
            DateTime dtTo = new DateTime(2015, 12, 7);
            DateTime dtFrom = new DateTime(2015, 12, 8);
            List<PollData> unprocessedPolls = context.PollDatas.Where(p => (p.Process == false)).OrderBy(e => e.EntTime).ToList();
            //List<PollData> unprocessedPolls = context.PollDatas.Where(p => (p.EntDate >= dtTo && p.EntDate <= dtFrom) && p.EmpID == 180).OrderBy(e => e.EntTime).ToList();
            foreach (PollData up in unprocessedPolls)
            {
                try
                {
                    //Create Attendance if any poll date is not processed already
                    if (context.AttProcesses.Where(ap => ap.ProcessDate == up.EntDate).Count() == 0)
                    {
                        CreateAttendance(up.EntDate.Date);
                    }
                    //Check AttData with EmpDate
                    if (context.AttDatas.Where(attd => attd.EmpDate == up.EmpDate).Count() > 0)
                    {
                        AttData attendanceRecord = context.AttDatas.First(attd => attd.EmpDate == up.EmpDate);
                        employee = attendanceRecord.Emp;
                        Shift shift = employee.Shift;
                        //Set Time In and Time Out in AttData
                        if (attendanceRecord.Emp.Shift.OpenShift == true)
                        {
                            //Set Time In and Time Out for open shift
                            PlaceTimeInOuts.CalculateTimeINOUTOpenShift(attendanceRecord, up);
                        }
                        else
                        {
                            TimeSpan checkTimeEnd = new TimeSpan();
                            DateTime TimeInCheck = new DateTime();
                            if (attendanceRecord.TimeIn == null)
                            {
                                TimeInCheck = attendanceRecord.AttDate.Value.Add(attendanceRecord.DutyTime.Value);
                            }
                            else
                                TimeInCheck = attendanceRecord.TimeIn.Value;
                            if (attendanceRecord.ShifMin == 0)
                                checkTimeEnd = TimeInCheck.TimeOfDay.Add(new TimeSpan(0, 480, 0));
                            else
                                checkTimeEnd = TimeInCheck.TimeOfDay.Add(new TimeSpan(0, (int)attendanceRecord.ShifMin, 0));
                            if (checkTimeEnd.Days > 0)
                            {
                                //if Time out occur at next day
                                if (up.RdrDuty == 5)
                                {
                                    DateTime dt = new DateTime();
                                    dt = up.EntDate.Date.AddDays(-1);
                                    var _attData = context.AttDatas.FirstOrDefault(aa => aa.AttDate == dt && aa.EmpID == up.EmpID);
                                    if (_attData != null)
                                    {

                                        if (_attData.TimeIn != null)
                                        {
                                            TimeSpan t1 = new TimeSpan(11,00,00);
                                            if (up.EntTime.TimeOfDay < t1)
                                            {
                                                if ((up.EntTime - _attData.TimeIn.Value).Hours < 18)
                                                {
                                                    attendanceRecord = _attData;
                                                    up.EmpDate = up.EmpID.ToString() + dt.Date.ToString("yyMMdd");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            attendanceRecord = _attData;
                                            up.EmpDate = up.EmpID.ToString() + dt.Date.ToString("yyMMdd");
                                        }

                                    }
                                }
                            }
                            //Set Time In and Time Out
                            if (up.RdrDuty == 5)
                            {
                                if (attendanceRecord.TimeIn != null)
                                {
                                    TimeSpan dt = (TimeSpan)(up.EntTime.TimeOfDay - attendanceRecord.TimeIn.Value.TimeOfDay);
                                    if (dt.Minutes < 0)
                                    {
                                        DateTime dt1 = new DateTime();
                                        dt1 = up.EntDate.Date.AddDays(-1);
                                        var _attData = context.AttDatas.FirstOrDefault(aa => aa.AttDate == dt1 && aa.EmpID == up.EmpID);
                                        attendanceRecord = _attData;
                                        up.EmpDate = up.EmpID.ToString() + dt1.Date.ToString("yyMMdd");
                                        PlaceTimeInOuts.CalculateTimeINOUT(attendanceRecord, up);
                                    }
                                    else
                                        PlaceTimeInOuts.CalculateTimeINOUT(attendanceRecord, up);
                                }
                                else
                                    PlaceTimeInOuts.CalculateTimeINOUT(attendanceRecord, up);
                            }
                            else
                                PlaceTimeInOuts.CalculateTimeINOUT(attendanceRecord, up);
                        }
                        if (employee.Shift.OpenShift == true)
                        {
                            if (up.EntTime.TimeOfDay < PlaceTimeInOuts.OpenShiftThresholdEnd)
                            {
                                DateTime dt = up.EntDate.Date.AddDays(-1);
                                CalculateWorkMins.CalculateOpenShiftTimes(context.AttDatas.FirstOrDefault(aa => aa.AttDate == dt && aa.EmpID == up.EmpID), shift);
                                context.SaveChanges();
                            }
                        }
                        //If TimeIn and TimeOut are not null, then calculate other Atributes
                        if (attendanceRecord.TimeIn != null && attendanceRecord.TimeOut != null)
                        {
                            if (context.Rosters.Where(r => r.EmpDate == up.EmpDate).Count() > 0)
                            {
                                CalculateWorkMins.CalculateRosterTimes(attendanceRecord, context.Rosters.FirstOrDefault(r => r.EmpDate == up.EmpDate), shift);
                                context.SaveChanges();
                            }
                            else
                            {
                                if (shift.OpenShift == true)
                                {
                                    if (up.EntTime.TimeOfDay < PlaceTimeInOuts.OpenShiftThresholdEnd)
                                    {
                                        DateTime dt = up.EntDate.Date.AddDays(-1);
                                        CalculateWorkMins.CalculateOpenShiftTimes(context.AttDatas.FirstOrDefault(aa => aa.AttDate == dt && aa.EmpID == up.EmpID), shift);
                                        CalculateWorkMins.CalculateOpenShiftTimes(attendanceRecord, shift);
                                        context.SaveChanges();
                                    }
                                    else
                                    {
                                        //Calculate open shifft time of the same date
                                        CalculateWorkMins.CalculateOpenShiftTimes(attendanceRecord, shift);
                                        context.SaveChanges();
                                    }
                                }
                                else
                                {
                                    CalculateWorkMins.CalculateShiftTimes(attendanceRecord, shift);
                                    context.SaveChanges();
                                }
                            }
                        }
                        up.Process = true;
                    }
                }
                catch (Exception ex)
                {
                    string _error = "";
                    if (ex.InnerException.Message != null)
                        _error = ex.InnerException.Message;
                    else
                        _error = ex.Message;
                    _myHelperClass.WriteToLogFile("Attendance Processing Error Level 1 " + _error);
                }
                context.SaveChanges();
            }
            _myHelperClass.WriteToLogFile("Attendance Processing Completed");
            context.Dispose();
        }

        MyCustomFunctions _myHelperClass = new MyCustomFunctions();

        public void CreateAttendance(DateTime dateTime)
        {
            using (var ctx = new TAS2013Entities())
            {
                List<Emp> _emp = new List<Emp>();
                _emp = ctx.Emps.Where(aa => aa.Status == true).ToList();
                List<Roster> _Roster = new List<Roster>();
                _Roster = context.Rosters.Where(aa => aa.RosterDate == dateTime).ToList();
                List<RosterDetail> _NewRoster = new List<RosterDetail>();
                _NewRoster = context.RosterDetails.Where(aa => aa.RosterDate == dateTime).ToList();
                List<LvData> _LvData = new List<LvData>();
                _LvData = context.LvDatas.Where(aa => aa.AttDate == dateTime).ToList();
                List<LvShort> _lvShort = new List<LvShort>();
                _lvShort = context.LvShorts.Where(aa => aa.DutyDate == dateTime).ToList();
                List<AttData> _AttData = context.AttDatas.Where(aa => aa.AttDate == dateTime).ToList();
                _myHelperClass.WriteToLogFile("**********************Attendance Creating Started: Total Employees are:" + _emp.Count + "*********************");
                List<Remark> remarks = new List<Remark>();
                remarks = ctx.Remarks.ToList();
                foreach (var emp in _emp)
                {
                    string empDate = emp.EmpID + dateTime.ToString("yyMMdd");
                    if (_AttData.Where(aa => aa.EmpDate == empDate).Count() == 0)
                    {
                        try
                        {
                            /////////////////////////////////////////////////////
                            //  Mark Everyone Absent while creating Attendance //
                            /////////////////////////////////////////////////////
                            //Set DUTYCODE = D, StatusAB = true, and Remarks = [Absent]
                            AttData att = new AttData();
                            att.AttDate = dateTime.Date;
                            att.DutyCode = "D";
                            att.StatusAB = true;
                            att.Remarks = "[Absent]";
                            if (emp.Shift != null)
                                att.DutyTime = emp.Shift.StartTime;
                            else
                                att.DutyTime = new TimeSpan(07, 45, 00);
                            att.EmpID = emp.EmpID;
                            att.EmpNo = emp.EmpNo;
                            att.EmpDate = emp.EmpID + dateTime.ToString("yyMMdd");
                            att.ShifMin = ProcessSupportFunc.CalculateShiftMinutes(emp.Shift, dateTime.DayOfWeek);
                            //////////////////////////
                            //  Check for Rest Day //
                            ////////////////////////
                            //Set DutyCode = R, StatusAB=false, StatusDO = true, and Remarks=[DO]
                            //Check for 1st Day Off of Shift
                            if (emp.Shift.DaysName.Name == ProcessSupportFunc.ReturnDayOfWeek(dateTime.DayOfWeek))
                            {
                                att.DutyCode = "R";
                                att.StatusAB = false;
                                att.StatusDO = true;
                                att.Remarks = "[DO]";
                            }
                            //Check for 2nd Day Off of shift
                            if (emp.Shift.DaysName1.Name == ProcessSupportFunc.ReturnDayOfWeek(dateTime.DayOfWeek))
                            {
                                att.DutyCode = "R";
                                att.StatusAB = false;
                                att.StatusDO = true;
                                att.Remarks = "[DO]";
                            }
                            //////////////////////////
                            //  Check for Roster   //
                            ////////////////////////
                            //If Roster DutyCode is Rest then change the StatusAB and StatusDO
                            foreach (var roster in _Roster.Where(aa => aa.EmpDate == att.EmpDate))
                            {
                                att.DutyCode = roster.DutyCode.Trim();
                                if (att.DutyCode == "R")
                                {
                                    att.StatusAB = false;
                                    att.StatusDO = true;
                                    att.DutyCode = "R";
                                    att.Remarks = "[DO]";
                                }
                                att.ShifMin = roster.WorkMin;
                                att.DutyTime = roster.DutyTime;
                            }

                            ////New Roster
                            string empCdate = "Emp" + emp.EmpID.ToString() + dateTime.ToString("yyMMdd");
                            string sectionCdate = "Section" + emp.SecID.ToString() + dateTime.ToString("yyMMdd");
                            string crewCdate = "Crew" + emp.CrewID.ToString() + dateTime.ToString("yyMMdd");
                            string shiftCdate = "Shift" + emp.ShiftID.ToString() + dateTime.ToString("yyMMdd");
                            if (_NewRoster.Where(aa => aa.CriteriaValueDate == empCdate).Count() > 0)
                            {
                                var roster = _NewRoster.FirstOrDefault(aa => aa.CriteriaValueDate == empCdate);
                                if (roster.WorkMin == 0)
                                {
                                    att.StatusAB = false;
                                    att.StatusDO = true;
                                    att.Remarks = "[DO]";
                                    att.DutyCode = "R";
                                    att.ShifMin = 0;
                                }
                                else
                                {
                                    att.ShifMin = roster.WorkMin;
                                    att.DutyCode = "D";
                                    att.DutyTime = roster.DutyTime;
                                }
                            }
                            else if (_NewRoster.Where(aa => aa.CriteriaValueDate == sectionCdate).Count() > 0)
                            {
                                var roster = _NewRoster.FirstOrDefault(aa => aa.CriteriaValueDate == sectionCdate);
                                if (roster.WorkMin == 0)
                                {
                                    att.StatusAB = false;
                                    att.StatusDO = true;
                                    att.Remarks = "[DO]";
                                    att.DutyCode = "R";
                                    att.ShifMin = 0;
                                }
                                else
                                {
                                    att.ShifMin = roster.WorkMin;
                                    att.DutyCode = "D";
                                    att.DutyTime = roster.DutyTime;
                                }
                            }
                            else if (_NewRoster.Where(aa => aa.CriteriaValueDate == crewCdate).Count() > 0)
                            {
                                var roster = _NewRoster.FirstOrDefault(aa => aa.CriteriaValueDate == crewCdate);
                                if (roster.WorkMin == 0)
                                {
                                    att.StatusAB = false;
                                    att.StatusDO = true;
                                    att.Remarks = "[DO]";
                                    att.DutyCode = "R";
                                    att.ShifMin = 0;
                                }
                                else
                                {
                                    att.ShifMin = roster.WorkMin;
                                    att.DutyCode = "D";
                                    att.DutyTime = roster.DutyTime;
                                }
                            }
                            else if (_NewRoster.Where(aa => aa.CriteriaValueDate == shiftCdate).Count() > 0)
                            {
                                var roster = _NewRoster.FirstOrDefault(aa => aa.CriteriaValueDate == shiftCdate);
                                if (roster.WorkMin == 0)
                                {
                                    att.StatusAB = false;
                                    att.StatusDO = true;
                                    att.Remarks = "[DO]";
                                    att.DutyCode = "R";
                                    att.ShifMin = 0;
                                }
                                else
                                {
                                    att.ShifMin = roster.WorkMin;
                                    att.DutyCode = "D";
                                    att.DutyTime = roster.DutyTime;
                                }
                            }

                            //////////////////////////
                            //  Check for GZ Day //
                            ////////////////////////
                            //Set DutyCode = R, StatusAB=false, StatusGZ = true, and Remarks=[GZ]
                            if (emp.Shift.GZDays == true)
                            {
                                foreach (var holiday in context.Holidays)
                                {
                                    if (context.Holidays.Where(hol => hol.HolDate.Month == att.AttDate.Value.Month && hol.HolDate.Day == att.AttDate.Value.Day).Count() > 0)
                                    {
                                        att.DutyCode = "G";
                                        att.StatusAB = false;
                                        att.StatusGZ = true;
                                        att.Remarks = "[GZ]";
                                        att.ShifMin = 0;
                                    }
                                }
                            }
                            ////////////////////////////
                            //  Check for Short Leave//
                            //////////////////////////
                            foreach (var sLeave in _lvShort.Where(aa => aa.EmpDate == att.EmpDate))
                            {
                                if (_lvShort.Where(lv => lv.EmpDate == att.EmpDate).Count() > 0)
                                {
                                    att.StatusSL = true;
                                    att.StatusAB = null;
                                    att.DutyCode = "L";
                                    att.Remarks = "[Short Leave]";
                                }
                            }

                            //////////////////////////
                            //   Check for Leave   //
                            ////////////////////////
                            //Set DutyCode = R, StatusAB=false, StatusGZ = true, and Remarks=[GZ]
                            foreach (var Leave in _LvData)
                            {
                                var _Leave = _LvData.Where(lv => lv.EmpDate == att.EmpDate && lv.HalfLeave != true);
                                if (_Leave.Count() > 0)
                                {
                                    att.StatusLeave = true;
                                    att.StatusAB = false;
                                    att.DutyCode = "L";
                                    att.StatusDO = false;
                                    if (Leave.LvCode == "A")
                                        att.Remarks = "[CL]";
                                    else if (Leave.LvCode == "B")
                                        att.Remarks = "[AL]";
                                    else if (Leave.LvCode == "C")
                                        att.Remarks = "[SL]";
                                    else
                                        att.Remarks = "[" + _Leave.FirstOrDefault().LvType.LvDesc + "]";
                                }
                            }

                            /////////////////////////
                            //Check for Half Leave///
                            ////////////////////////
                            var _HalfLeave = _LvData.Where(lv => lv.EmpDate == att.EmpDate && lv.HalfLeave == true);
                            if (_HalfLeave.Count() > 0)
                            {
                                att.StatusLeave = true;
                                att.StatusAB = false;
                                att.DutyCode = "L";
                                att.StatusHL = true;
                                att.StatusDO = false;
                                if (_HalfLeave.FirstOrDefault().LvCode == "A")
                                    att.Remarks = "[H-CL]";
                                else if (_HalfLeave.FirstOrDefault().LvCode == "B")
                                    att.Remarks = "[S-AL]";
                                else if (_HalfLeave.FirstOrDefault().LvCode == "C")
                                    att.Remarks = "[H-SL]";
                                else
                                    att.Remarks = "[Half Leave]";
                            }
                            ctx.AttDatas.AddObject(att);
                            ctx.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            _myHelperClass.WriteToLogFile("-------Error In Creating Attendance of Employee: " + emp.EmpNo + " ------" + ex.InnerException.Message);
                        }
                    }
                }
                _myHelperClass.WriteToLogFile("****************Creating Attendance Completed*****************");
                AttProcess attp = new AttProcess();
                attp.ProcessDate = dateTime;
                ctx.AttProcesses.AddObject(attp);
                ctx.SaveChanges();
                ////////////////////////////
                //Check for Job Card//
                //////////////////////////
                try
                {
                    ProcessJobCard jc = new ProcessJobCard();
                    jc.ProcessJobCards(dateTime);
                }
                catch (Exception ex)
                {
                    _myHelperClass.WriteToLogFile("Error at Create Function Process Job Card " + dateTime.ToString());
                }
                ctx.Dispose();
            }
            // reprocess attendance from last 5 days
            CreateMissingAttendance ca = new CreateMissingAttendance();
            ca.CreatemissingAttendance(dateTime.AddDays(-7), dateTime);
            _myHelperClass.WriteToLogFile("Creating Attendance of Date: " + dateTime.ToString());
        }

        #endregion
    }
}
