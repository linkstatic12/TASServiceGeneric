using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TASDownloadService.Model;
using TASDownloadService.AttProcessDaily;

namespace WMSFFService
{
    public class ProcessEditAttendanceEntries
    {
        TAS2013Entities newDB = new TAS2013Entities();
        MyCustomFunctions _myHelperClass = new MyCustomFunctions();
        public void ProcessManualEditAttendance(DateTime _dateStart, DateTime _dateEnd)
        {
            List<AttDataManEdit> _attEdit = new List<AttDataManEdit>();
            List<AttData> _AttData = new List<AttData>();
            AttData _TempAttData = new AttData();
            using (var ctx = new TAS2013Entities())
            {

                if (_dateStart == _dateEnd)
                {
                    //_attEdit = ctx.AttDataManEdits.Where(aa => aa.NewTimeIn == _dateStart).OrderBy(aa => aa.EditDateTime).ToList();
                    _dateEnd = _dateEnd + new TimeSpan(23, 59, 59);
                    _attEdit = ctx.AttDataManEdits.Where(aa => aa.NewTimeIn >= _dateStart && aa.NewTimeIn <= _dateEnd).OrderBy(aa => aa.EditDateTime).ToList();
                    _AttData = ctx.AttDatas.Where(aa => aa.AttDate == _dateStart).ToList();
                }
                else
                {
                    _dateEnd = _dateEnd + new TimeSpan(23, 59, 59);
                    _attEdit = ctx.AttDataManEdits.Where(aa => aa.NewTimeIn >= _dateStart && aa.NewTimeOut <= _dateEnd).OrderBy(aa => aa.EditDateTime).ToList();
                    //_attEdit = ctx.AttDataManEdits.Where(aa => aa.NewTimeIn >= _dateStart && (aa.NewTimeOut <= _dateEnd && aa.EmpID == 472)).OrderBy(aa => aa.EditDateTime).ToList();
                    _AttData = ctx.AttDatas.Where(aa => aa.AttDate >= _dateStart && aa.AttDate <= _dateEnd).ToList();
                }


                foreach (var item in _attEdit)
                {
                    _TempAttData = _AttData.First(aa => aa.EmpDate == item.EmpDate);
                    _TempAttData.TimeIn = item.NewTimeIn;
                    _TempAttData.TimeOut = item.NewTimeOut;
                    _TempAttData.DutyCode = item.NewDutyCode;
                    _TempAttData.DutyTime = item.NewDutyTime;
                    _TempAttData.ShifMin = item.NewShiftMin;
                    switch (_TempAttData.DutyCode)
                    {
                        case "D":
                            _TempAttData.StatusAB = true;
                            _TempAttData.StatusP = false;
                            _TempAttData.StatusMN = true;
                            _TempAttData.StatusDO = false;
                            _TempAttData.StatusGZ = false;
                            _TempAttData.StatusLeave = false;
                            _TempAttData.StatusOT = false;
                            _TempAttData.OTMin = null;
                            _TempAttData.EarlyIn = null;
                            _TempAttData.EarlyOut = null;
                            _TempAttData.LateIn = null;
                            _TempAttData.LateOut = null;
                            _TempAttData.WorkMin = null;
                            _TempAttData.GZOTMin = null;
                            break;
                        case "G":
                            _TempAttData.StatusAB = false;
                            _TempAttData.StatusP = false;
                            _TempAttData.StatusMN = true;
                            _TempAttData.StatusDO = false;
                            _TempAttData.StatusGZ = true;
                            _TempAttData.StatusLeave = false;
                            _TempAttData.StatusOT = false;
                            _TempAttData.OTMin = null;
                            _TempAttData.EarlyIn = null;
                            _TempAttData.EarlyOut = null;
                            _TempAttData.LateIn = null;
                            _TempAttData.LateOut = null;
                            _TempAttData.WorkMin = null;
                            _TempAttData.GZOTMin = null;
                            break;
                        case "R":
                            _TempAttData.StatusAB = false;
                            _TempAttData.StatusP = false;
                            _TempAttData.StatusMN = true;
                            _TempAttData.StatusDO = true;
                            _TempAttData.StatusGZ = false;
                            _TempAttData.StatusLeave = false;
                            _TempAttData.StatusOT = false;
                            _TempAttData.OTMin = null;
                            _TempAttData.EarlyIn = null;
                            _TempAttData.EarlyOut = null;
                            _TempAttData.LateIn = null;
                            _TempAttData.LateOut = null;
                            _TempAttData.WorkMin = null;
                            _TempAttData.GZOTMin = null;
                            break;
                    }
                    if (_TempAttData.TimeIn != null && _TempAttData.TimeOut != null)
                    {
                        //If TimeIn = TimeOut then calculate according to DutyCode
                        if (_TempAttData.TimeIn == _TempAttData.TimeOut)
                        {
                            CalculateInEqualToOut(_TempAttData);
                        }
                        else
                        {
                            if (_TempAttData.DutyTime == new TimeSpan(0, 0, 0))
                            {
                                CalculateWorkMins.CalculateOpenShiftTimes(_TempAttData, _TempAttData.Emp.Shift);
                            }
                            else
                            {
                                //if (attendanceRecord.TimeIn.Value.Date.Day == attendanceRecord.TimeOut.Value.Date.Day)
                                //{
                                CalculateWorkMins.CalculateShiftTimes(_TempAttData, _TempAttData.Emp.Shift);
                                //}
                                //else
                                //{
                                //    CalculateOpenShiftTimes(attendanceRecord, shift);
                                //}
                            }
                            _TempAttData.Remarks = _TempAttData.Remarks + "[Manual]";
                        }

                        //If TimeIn = TimeOut then calculate according to DutyCode
                    }
                    ctx.SaveChanges();
                }
                ctx.Dispose();


            }
            _myHelperClass.WriteToLogFile("ProcessManual Attendance Completed: ");
        }
        private void CalculateInEqualToOut(AttData attendanceRecord)
        {
            switch (attendanceRecord.DutyCode)
            {
                case "G":
                    attendanceRecord.StatusAB = false;
                    attendanceRecord.StatusGZ = true;
                    attendanceRecord.WorkMin = 0;
                    attendanceRecord.EarlyIn = 0;
                    attendanceRecord.EarlyOut = 0;
                    attendanceRecord.LateIn = 0;
                    attendanceRecord.LateOut = 0;
                    attendanceRecord.OTMin = 0;
                    attendanceRecord.GZOTMin = 0;
                    attendanceRecord.StatusGZOT = false;
                    attendanceRecord.TimeIn = null;
                    attendanceRecord.TimeOut = null;
                    attendanceRecord.Remarks = "[GZ][Manual]";
                    break;
                case "R":
                    attendanceRecord.StatusAB = false;
                    attendanceRecord.StatusGZ = false;
                    attendanceRecord.WorkMin = 0;
                    attendanceRecord.EarlyIn = 0;
                    attendanceRecord.EarlyOut = 0;
                    attendanceRecord.LateIn = 0;
                    attendanceRecord.LateOut = 0;
                    attendanceRecord.OTMin = 0;
                    attendanceRecord.GZOTMin = 0;
                    attendanceRecord.StatusGZOT = false;
                    attendanceRecord.TimeIn = null;
                    attendanceRecord.TimeOut = null;
                    attendanceRecord.StatusDO = true;
                    attendanceRecord.Remarks = "[DO][Manual]";
                    break;
                case "D":
                    attendanceRecord.StatusAB = true;
                    attendanceRecord.StatusGZ = false;
                    attendanceRecord.WorkMin = 0;
                    attendanceRecord.EarlyIn = 0;
                    attendanceRecord.EarlyOut = 0;
                    attendanceRecord.LateIn = 0;
                    attendanceRecord.LateOut = 0;
                    attendanceRecord.OTMin = 0;
                    attendanceRecord.GZOTMin = 0;
                    attendanceRecord.StatusGZOT = false;
                    attendanceRecord.TimeIn = null;
                    attendanceRecord.TimeOut = null;
                    attendanceRecord.StatusDO = false;
                    attendanceRecord.StatusP = false;
                    attendanceRecord.Remarks = "[Absent][Manual]";
                    break;
            }
        }
    }
}
