using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TASDownloadService.Model;

namespace WMSFFService
{
    public class ProcessJobCard
    {
        
        public void ProcessJobCards(DateTime _AttDate)
        {
            using (var ctx = new TAS2013Entities())
            {
                if (ctx.AttProcesses.Where(aa => aa.ProcessDate == _AttDate).Count() > 0)
                {
                    List<JobCardEmp> _EmpJobCard = new List<JobCardEmp>();
                    _EmpJobCard = ctx.JobCardEmps.Where(aa => aa.Dated == _AttDate).ToList();
                    foreach (var jobCard in _EmpJobCard)
                    {
                        switch (jobCard.WrkCardID)
                        {
                            case 1://Day Off
                                AddJCDayOffToAttData(jobCard.EmpDate, (int)jobCard.EmpID, (DateTime)jobCard.Dated, (short)jobCard.WrkCardID);
                                break;
                            case 2://GZ Holiday
                                AddJCGZDayToAttData(jobCard.EmpDate, (int)jobCard.EmpID, (DateTime)jobCard.Dated, (short)jobCard.WrkCardID);
                                break;
                            case 3://Absent
                                AddJCAbsentToAttData(jobCard.EmpDate, (int)jobCard.EmpID, (DateTime)jobCard.Dated, (short)jobCard.WrkCardID);
                                break;
                            case 4://official Duty
                                AddJCODDayToAttData(jobCard.EmpDate, (int)jobCard.EmpID, (DateTime)jobCard.Dated, (short)jobCard.WrkCardID);
                                break;
                            case 8:// Double Duty
                                AddDoubleDutyAttData(jobCard.EmpDate, (int)jobCard.EmpID, (DateTime)jobCard.Dated,(short)jobCard.WorkMin);
                                break;
                            case 9:// Badli Duty
                                AddBadliAttData(jobCard.EmpDate, (int)jobCard.EmpID, (DateTime)jobCard.Dated);
                                break;
                        }
                    }
                } 
            }

        }

        public void ProcessJCForAttCreateFunctionDaily(DateTime _AttDate)
        {
            using (var ctx = new TAS2013Entities())
            {
                    List<JobCardEmp> _EmpJobCard = new List<JobCardEmp>();
                    _EmpJobCard = ctx.JobCardEmps.Where(aa => aa.Dated == _AttDate).ToList();
                    foreach (var jobCard in _EmpJobCard)
                    {
                        switch (jobCard.WrkCardID)
                        {
                            case 1://Day Off
                                AddJCDayOffToAttData(jobCard.EmpDate, (int)jobCard.EmpID, (DateTime)jobCard.Dated, (short)jobCard.WrkCardID);
                                break;
                            case 2://GZ Holiday
                                AddJCGZDayToAttData(jobCard.EmpDate, (int)jobCard.EmpID, (DateTime)jobCard.Dated, (short)jobCard.WrkCardID);
                                break;
                            case 3://Absent
                                AddJCAbsentToAttData(jobCard.EmpDate, (int)jobCard.EmpID, (DateTime)jobCard.Dated, (short)jobCard.WrkCardID);
                                break;
                            case 4://official Duty
                                AddJCODDayToAttData(jobCard.EmpDate, (int)jobCard.EmpID, (DateTime)jobCard.Dated, (short)jobCard.WrkCardID);
                                break;
                            case 8:// Double Duty
                                AddDoubleDutyAttData(jobCard.EmpDate, (int)jobCard.EmpID, (DateTime)jobCard.Dated, (short)jobCard.WorkMin);
                                break;
                            case 9:// Badli Duty
                                AddBadliAttData(jobCard.EmpDate, (int)jobCard.EmpID, (DateTime)jobCard.Dated);
                                break;
                        }
                    }
            }

        }
        #region --Job Cards - AttData ---
        private bool AddJCNorrmalDayAttData(string _empDate, int _empID, DateTime _Date, short _WorkCardID)
        {
            bool check = false;
            try
            {
                //Normal Duty
                using (var context = new TAS2013Entities())
                {
                    AttData _attdata = context.AttDatas.FirstOrDefault(aa => aa.EmpDate == _empDate);
                    JobCard _jcCard = context.JobCards.FirstOrDefault(aa => aa.WorkCardID == _WorkCardID);
                    if (_attdata != null)
                    {
                        _attdata.DutyCode = "D";
                        _attdata.StatusAB = false;
                        _attdata.StatusDO = false;
                        _attdata.StatusLeave = false;
                        _attdata.StatusP = true;
                        _attdata.WorkMin = _jcCard.WorkMin;
                        _attdata.ShifMin = _jcCard.WorkMin;
                        _attdata.Remarks = "[Present][Manual]";
                        _attdata.TimeIn = null;
                        _attdata.TimeOut = null;
                        _attdata.EarlyIn = null;
                        _attdata.EarlyOut = null;
                        _attdata.LateIn = null;
                        _attdata.LateOut = null;
                        _attdata.OTMin = null;
                        _attdata.StatusEI = null;
                        _attdata.StatusEO = null;
                        _attdata.StatusLI = null;
                        _attdata.StatusLO = null;
                        _attdata.StatusP = true;
                    }
                    context.SaveChanges();
                    if (context.SaveChanges() > 0)
                        check = true;
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
            }
            return check;
        }

        private bool AddDoubleDutyAttData(string _empDate, int _empID, DateTime _Date, short WorkMins)
        {
            bool check = false;
            try
            {
                //Normal Duty
                using (var context = new TAS2013Entities())
                {
                    AttData _attdata = context.AttDatas.FirstOrDefault(aa => aa.EmpDate == _empDate);
                    if (_attdata != null)
                    {
                        _attdata.DutyCode = "D";
                        _attdata.StatusAB = false;
                        _attdata.StatusDO = false;
                        _attdata.StatusLeave = false;
                        _attdata.StatusP = true;
                        _attdata.WorkMin = WorkMins;
                        _attdata.Remarks = _attdata.Remarks + "[DD][Manual]";
                        _attdata.StatusMN = true;
                        _attdata.TimeIn = null;
                        _attdata.TimeOut = null;
                        _attdata.EarlyIn = null;
                        _attdata.EarlyOut = null;
                        _attdata.LateIn = null;
                        _attdata.LateOut = null;
                        _attdata.OTMin = null;
                        _attdata.StatusEI = null;
                        _attdata.StatusEO = null;
                        _attdata.StatusLI = null;
                        _attdata.StatusLO = null;
                        _attdata.StatusP = true;
                    }
                    context.SaveChanges();
                    if (context.SaveChanges() > 0)
                        check = true;
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
            }
            return check;
        }

        private bool AddBadliAttData(string _empDate, int _empID, DateTime _Date)
        {
            bool check = false;
            try
            {
                //Normal Duty
                using (var context = new TAS2013Entities())
                {
                    AttData _attdata = context.AttDatas.FirstOrDefault(aa => aa.EmpDate == _empDate);
                    if (_attdata != null)
                    {
                        _attdata.DutyCode = "D";
                        _attdata.StatusAB = false;
                        _attdata.StatusLeave = false;
                        _attdata.StatusP = true;
                        _attdata.Remarks = _attdata.Remarks + "[Badli][Manual]";
                        _attdata.StatusMN = true;
                    }
                    context.SaveChanges();
                    if (context.SaveChanges() > 0)
                        check = true;
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
            }
            return check;
        }
        private bool AddJCODDayToAttData(string _empDate, int _empID, DateTime _Date, short _WorkCardID)
        {

            bool check = false;
            try
            {
                //Official Duty
                using (var context = new TAS2013Entities())
                {
                    AttData _attdata = context.AttDatas.FirstOrDefault(aa => aa.EmpDate == _empDate);
                    if (_attdata != null)
                    {
                        _attdata.DutyCode = "O";
                        _attdata.StatusAB = false;
                        _attdata.StatusDO = false;
                        _attdata.StatusLeave = false;
                        _attdata.StatusP = true;
                        _attdata.WorkMin = _attdata.ShifMin;
                        _attdata.Remarks = _attdata.Remarks + "[Official Duty][Manual]";
                        _attdata.TimeIn = null;
                        _attdata.TimeOut = null;
                        _attdata.WorkMin = null;
                        _attdata.EarlyIn = null;
                        _attdata.EarlyOut = null;
                        _attdata.LateIn = null;
                        _attdata.LateOut = null;
                        _attdata.OTMin = null;
                        _attdata.StatusEI = null;
                        _attdata.StatusEO = null;
                        _attdata.StatusLI = null;
                        _attdata.StatusLO = null;
                        _attdata.StatusP = null;
                        _attdata.StatusGZ = false;
                    }
                    context.SaveChanges();
                    if (context.SaveChanges() > 0)
                        check = true;
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
            return check;
        }

        private bool AddJCAbsentToAttData(string _empDate, int _empID, DateTime _Date, short _WorkCardID)
        {
            bool check = false;
            try
            {
                //Absent
                using (var context = new TAS2013Entities())
                {
                    AttData _attdata = context.AttDatas.FirstOrDefault(aa => aa.EmpDate == _empDate);
                    if (_attdata != null)
                    {
                        _attdata.DutyCode = "D";
                        _attdata.StatusAB = true;
                        _attdata.StatusDO = false;
                        _attdata.StatusLeave = false;
                        _attdata.Remarks = "[Absent][Manual]";
                        _attdata.TimeIn = null;
                        _attdata.TimeOut = null;
                        _attdata.WorkMin = null;
                        _attdata.EarlyIn = null;
                        _attdata.EarlyOut = null;
                        _attdata.LateIn = null;
                        _attdata.LateOut = null;
                        _attdata.OTMin = null;
                        _attdata.StatusEI = null;
                        _attdata.StatusEO = null;
                        _attdata.StatusLI = null;
                        _attdata.StatusLO = null;
                        _attdata.StatusP = null;
                    }
                    context.SaveChanges();
                    if (context.SaveChanges() > 0)
                        check = true;
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
            return check;
        }

        private bool AddJCGZDayToAttData(string _empDate, int _empID, DateTime _Date, short _WorkCardID)
        {
            bool check = false;
            try
            {
                //GZ Holiday
                using (var context = new TAS2013Entities())
                {
                    AttData _attdata = context.AttDatas.FirstOrDefault(aa => aa.EmpDate == _empDate);
                    if (_attdata != null)
                    {
                        _attdata.DutyCode = "G";
                        _attdata.StatusAB = false;
                        _attdata.StatusDO = true;
                        _attdata.StatusLeave = false;
                        _attdata.StatusGZ = true;
                        _attdata.Remarks = "[GZ][Manual]";
                        _attdata.TimeIn = null;
                        _attdata.TimeOut = null;
                        _attdata.WorkMin = null;
                        _attdata.EarlyIn = null;
                        _attdata.EarlyOut = null;
                        _attdata.LateIn = null;
                        _attdata.LateOut = null;
                        _attdata.OTMin = null;
                        _attdata.StatusEI = null;
                        _attdata.StatusEO = null;
                        _attdata.StatusLI = null;
                        _attdata.StatusLO = null;
                        _attdata.StatusP = null;
                    }
                    context.SaveChanges();
                    if (context.SaveChanges() > 0)
                        check = true;
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
            return check;
        }

        private bool AddJCDayOffToAttData(string _empDate, int _empID, DateTime _Date,short _WorkCardID)
        {
            bool check = false;
            try
            {
                //Day Off
                using (var context = new TAS2013Entities())
                {
                    AttData _attdata = context.AttDatas.FirstOrDefault(aa => aa.EmpDate == _empDate);
                    if (_attdata != null)
                    {
                        _attdata.DutyCode = "R";
                        _attdata.StatusAB = false;
                        _attdata.StatusDO = true;
                        _attdata.StatusLeave = false;
                        _attdata.Remarks = "[DO][Manual]";
                        _attdata.TimeIn = null;
                        _attdata.TimeOut = null;
                        _attdata.WorkMin = null;
                        _attdata.EarlyIn = null;
                        _attdata.EarlyOut = null;
                        _attdata.LateIn = null;
                        _attdata.LateOut = null;
                        _attdata.OTMin = null;
                        _attdata.StatusEI = null;
                        _attdata.StatusEO = null;
                        _attdata.StatusLI = null;
                        _attdata.StatusLO = null;
                        _attdata.StatusP = null;
                    }
                    context.SaveChanges();
                    if (context.SaveChanges() > 0)
                        check = true;
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
            return check;
        }

        private bool AddLateInMarginAttData(string _empDate, int _empID, DateTime _Date, short _WorkCardID)
        {
            bool check = false;
            try
            {
                //Late In Job Card
                short LateInMins = 0;
                using (var context = new TAS2013Entities())
                {
                    AttData _attdata = context.AttDatas.FirstOrDefault(aa => aa.EmpDate == _empDate);
                    if (_attdata != null)
                    {
                        _attdata.StatusAB = false;
                        _attdata.Remarks.Replace("[LI]", "");
                        _attdata.Remarks = _attdata.Remarks + "[LI Job Card]";
                        _attdata.LateIn = 0;
                        _attdata.WorkMin = (short)(_attdata.WorkMin + (short)LateInMins);
                        _attdata.StatusLI = false;
                    }
                    context.SaveChanges();
                    if (context.SaveChanges() > 0)
                        check = true;
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
            return check;
        }

        #endregion
    }
}
