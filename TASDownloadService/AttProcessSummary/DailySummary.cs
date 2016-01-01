using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TASDownloadService.Model;

namespace TASDownloadService.AttProcessSummary
{
    public class DailySummaryClass
    {
        public static void PreviousTenDaysSummary(DateTime dateStart, String criteria, int criteriaValue)
        {
            //for (int i = 0; i < 10; i++)
            //{
            //    DateTime date = dateStart.AddDays(-i);
            //    CalculateSummary(date, criteria, criteriaValue);
            //}
        }

        public DailySummaryClass(DateTime dateStart, DateTime dateEnd)
        {
            List<Company> companies = new List<Company>();
            List<Location> locs = new List<Location>();
            List<Section> secs = new List<Section>();
            List<Department> depts = new List<Department>();
            List<Shift> shifts = new List<Shift>();
            List<EmpType> empTypes = new List<EmpType>();
            List<Category> cats = new List<Category>();
            companies = db.Companies.ToList();
            locs = db.Locations.ToList();
            secs = db.Sections.ToList();
            depts = db.Departments.ToList();
            shifts = db.Shifts.ToList();
            empTypes = db.EmpTypes.ToList();
            cats = db.Categories.ToList();
            while (dateStart <= dateEnd)
            {
                //CalculateSummary(dateStart, "A");
                foreach (var company in companies)
                {
                    CalculateSummary(dateStart, "C", company.CompID,company.CompName);
                }
                foreach (var loc in locs)
                {
                    CalculateSummary(dateStart, "L", loc.LocID,loc.LocName);
                }
                foreach (var sec in secs)
                {
                    CalculateSummary(dateStart, "E", sec.SectionID,sec.SectionName);
                }
                foreach (var dept in depts)
                {
                    CalculateSummary(dateStart, "D", dept.DeptID,dept.DeptName);
                }
                foreach (var shift in shifts)
                {
                    CalculateSummary(dateStart, "S", shift.ShiftID,shift.ShiftName);
                }
                foreach (var emptype in empTypes)
                {
                    CalculateSummary(dateStart, "T", emptype.TypeID,emptype.TypeName);
                }
                foreach (var cat in cats)
                {
                    CalculateSummary(dateStart, "A", cat.CatID,cat.CatName);
                }
                dateStart = dateStart.AddDays(1);
            }
        }
        TAS2013Entities db = new TAS2013Entities();
        public static void CalculateSummary(DateTime dateStart, String criteria, int criteriaValue,string Name)
        {
            bool ProcssedDS = false;
            DateTime makeMe = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
            SummaryEntity summary = new SummaryEntity();

            TAS2013Entities context = new TAS2013Entities();
            ViewAttData vmio = new ViewAttData();
            summary.SummaryDateCriteria = dateStart.ToString("yyMMdd") + criteria + criteriaValue;
            String day = DateTime.Today.DayOfWeek.ToString();
            day = day.Substring(0, 3);
            day = day + "Min";

            List<ViewAttData> attList = new List<ViewAttData>();
            switch (criteria)
            {
                case "C": attList = context.ViewAttDatas.Where(aa => aa.AttDate == dateStart && aa.CompanyID == criteriaValue).ToList();
                    //Change the below line for NHA please
                    summary.criterianame = Name;
                    summary.SummaryDateCriteria = dateStart.ToString("yyMMdd") + criteria + criteriaValue;
                    break;
                case "L": attList = context.ViewAttDatas.Where(aa => aa.AttDate == dateStart && aa.LocID == criteriaValue).ToList();
                    if (attList.Count > 0)
                        summary.criterianame = Name;
                    summary.SummaryDateCriteria = dateStart.ToString("yyMMdd") + criteria + criteriaValue;
                    break;
                case "D": attList = context.ViewAttDatas.Where(aa => aa.AttDate == dateStart && aa.DeptID == criteriaValue).ToList();
                    if (attList.Count > 0)
                        summary.criterianame = Name;
                    summary.SummaryDateCriteria = dateStart.ToString("yyMMdd") + criteria + criteriaValue;
                    break;
                case "E": attList = context.ViewAttDatas.Where(aa => aa.AttDate == dateStart && aa.SecID == criteriaValue).ToList();
                    if (attList.Count > 0)
                        summary.criterianame = Name;
                    summary.SummaryDateCriteria = dateStart.ToString("yyMMdd") + criteria + criteriaValue;
                    break;

                case "S": attList = context.ViewAttDatas.Where(aa => aa.AttDate == dateStart && aa.ShiftID == criteriaValue).ToList();
                    if (attList.Count > 0)
                        summary.criterianame = Name;
                    summary.SummaryDateCriteria = dateStart.ToString("yyMMdd") + criteria + criteriaValue;
                    break;
                case "T": attList = context.ViewAttDatas.Where(aa => aa.AttDate == dateStart && aa.TypeID == criteriaValue).ToList();
                    if (attList.Count > 0)
                        summary.criterianame = Name;
                    summary.SummaryDateCriteria = dateStart.ToString("yyMMdd") + criteria + criteriaValue;
                    break;
                case "A": attList = context.ViewAttDatas.Where(aa => aa.AttDate == dateStart && aa.CatID == criteriaValue).ToList();
                    if (attList.Count > 0)
                        summary.criterianame = Name;
                    summary.SummaryDateCriteria = dateStart.ToString("yyMMdd") + criteria + criteriaValue;
                    break;
            }
            if (attList.Count > 0)
            {
                DailySummary dailysumm = new DailySummary();
                if (context.DailySummaries.Where(aa => aa.SummaryDateCriteria == summary.SummaryDateCriteria).Count() > 0)
                {
                    dailysumm = context.DailySummaries.First(aa => aa.SummaryDateCriteria == summary.SummaryDateCriteria);
                    ProcssedDS = true;
                }
                dailysumm.SummaryDateCriteria = summary.SummaryDateCriteria;
                dailysumm.Criteria = criteria;
                dailysumm.CriteriaValue = (short)criteriaValue;
                dailysumm.CriteriaName = summary.criterianame;
                dailysumm.TotalEmps = (short)attList.Count;
                dailysumm.PresentEmps = (short)attList.Where(aa => aa.StatusP == true).Count<ViewAttData>();
                dailysumm.AbsentEmps = (short)attList.Where(aa => aa.StatusAB == true).Count<ViewAttData>();
                dailysumm.Date = makeMe;
                dailysumm.EIEmps = (short)attList.Where(aa => aa.StatusEI == true).Count<ViewAttData>();
                dailysumm.EOEmps = (short)attList.Where(aa => aa.StatusEO == true).Count<ViewAttData>();
                dailysumm.LIEmps = (short)attList.Where(aa => aa.StatusLI == true).Count<ViewAttData>();
                dailysumm.LOEmps = (short)attList.Where(aa => aa.StatusLO == true).Count<ViewAttData>();
                dailysumm.LvEmps = (short)attList.Where(aa => aa.StatusLeave == true).Count<ViewAttData>();
                dailysumm.DayOffEmps = (short)attList.Where(aa => aa.StatusDO == true).Count<ViewAttData>();
                dailysumm.HalfLvEmps = 0;
                dailysumm.ShortLvEmps = 0;
                dailysumm.ExpectedWorkMins = 0;
                dailysumm.ActualWorkMins = 0;
                dailysumm.LossWorkMins = 0;
                dailysumm.OTMins = 0;
                dailysumm.LIMins = 0;
                dailysumm.LOMins = 0;
                dailysumm.EIMins = 0;
                dailysumm.EOMins = 0;
                dailysumm.AOTMins = 0;
                dailysumm.OTEmps = 0;
                dailysumm.OTMins = 0;
                Int64 averageTimeIn = 0;
                Int64 averageTimeOut = 0;
                //LV ,short half
                foreach (var emp in attList)
                {
                    if (emp.TimeIn != null)
                        averageTimeIn = averageTimeIn + emp.TimeIn.Value.Ticks;
                    if (emp.TimeOut != null)
                        averageTimeOut = averageTimeOut + emp.TimeOut.Value.Ticks;

                    if (emp.LateIn != null)
                        if(emp.LateIn>0)
                            dailysumm.LIMins = dailysumm.LIMins + emp.LateIn;
                    if (emp.LateOut != null)
                        if (emp.LateOut > 0)
                        dailysumm.LOMins = dailysumm.LOMins + emp.LateOut;
                    if (emp.EarlyIn != null)
                        if (emp.EarlyIn > 0)
                        dailysumm.EIMins = dailysumm.EIMins + emp.EarlyIn;
                    if (emp.EarlyOut != null)
                        if (emp.EarlyOut > 0)
                        dailysumm.EOMins = dailysumm.EOMins + emp.EarlyOut;
                    if (emp.WorkMin != null)
                        if (emp.WorkMin > 0)
                        dailysumm.ActualWorkMins = dailysumm.ActualWorkMins + emp.WorkMin;
                    //code leave bundle
                    if (emp.StatusLeave == true)
                        dailysumm.LvEmps = dailysumm.LvEmps++;
                    if (emp.StatusHL == true)
                        dailysumm.HalfLvEmps = dailysumm.HalfLvEmps++;
                    if (emp.StatusSL == true)
                        dailysumm.ShortLvEmps = dailysumm.ShortLvEmps++;

                    //code for over time emps
                    if (emp.StatusOT == true)
                    {
                        dailysumm.OTEmps = (short)(dailysumm.OTEmps + 1);
                        if (emp.OTMin!=null)
                            dailysumm.OTMins = dailysumm.OTMins + emp.OTMin;
                    }

                    //code for day off
                    if (emp.StatusDO == true)
                        dailysumm.DayOffEmps++;


                    //code for expected work mins
                    if (emp.ShifMin != null)
                        dailysumm.ExpectedWorkMins = emp.ShifMin + dailysumm.ExpectedWorkMins;




                }


                // dailysumm.OnTimeEmps = (short)(dailysumm.TotalEmps - dailysumm.OTEmps);

                
                dailysumm.LossWorkMins = dailysumm.ExpectedWorkMins - dailysumm.ActualWorkMins;

                try
                {
                    //if (dailysumm.PresentEmps != 0)
                    //    dailysumm.AverageTimeIn = new DateTime((Int64)(averageTimeIn / dailysumm.PresentEmps)).TimeOfDay;
                    //if (dailysumm.PresentEmps != 0)
                    //    dailysumm.AverageTimeOut = new DateTime((Int64)(averageTimeOut / dailysumm.PresentEmps)).TimeOfDay;
                    if (dailysumm.TotalEmps != 0)
                        dailysumm.AActualMins = (short)(dailysumm.ActualWorkMins / dailysumm.TotalEmps);
                    if (dailysumm.TotalEmps != 0)
                        dailysumm.ALossMins = (short)(dailysumm.LossWorkMins / dailysumm.TotalEmps);
                    if (dailysumm.TotalEmps != 0)
                        dailysumm.AExpectedMins = (short)(dailysumm.ExpectedWorkMins / dailysumm.TotalEmps);
                    if (dailysumm.LIEmps != 0)
                        dailysumm.ALIMins = (short)(dailysumm.LIMins / dailysumm.LIEmps);
                    if (dailysumm.LOEmps != 0)
                        dailysumm.ALOMins = (short)(dailysumm.LOMins / dailysumm.LOEmps);
                    if (dailysumm.EIEmps != 0)
                        dailysumm.AEIMins = (short)(dailysumm.EIMins / dailysumm.EIEmps);
                    if (dailysumm.EOEmps != 0)
                        dailysumm.AEOMins = (short)(dailysumm.EOMins / dailysumm.EOEmps);
                    if (dailysumm.OTEmps != 0)
                        dailysumm.AOTMins = (short)(dailysumm.OTMins / dailysumm.OTEmps);
                }
                catch (Exception e)
                { }
                if (ProcssedDS == false)
                    context.DailySummaries.AddObject(dailysumm);
                context.SaveChanges();



            }


        }

    }
}
