using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TASDownloadService.Model;

namespace TASDownloadService.AttProcessDaily
{
    class RandomFunc
    {
        //private void AdjustDOJUnit()
        //{
        //    using (var ctx = new TAS2013Entities())
        //    {
        //        List<DOJJ> dates = new List<DOJJ>();
        //        List<Emp> emps = new List<Emp>();
        //        emps = ctx.Emps.Where(aa => aa.CompanyID == 13).ToList();
        //        dates = ctx.DOJJs.ToList();
        //        foreach (var emp in emps)
        //        {
        //            if (dates.Where(aa => aa.EmpNo == emp.EmpNo).Count() > 0)
        //            {
        //                emp.JoinDate = dates.First(aa => aa.EmpNo == emp.EmpNo).DateOfJoin;
        //            }
        //        }
        //        ctx.SaveChanges();
        //    }
        //}

        private void CalculateEmpWithWrongSection()
        {
            try
            {
                TAS2013Entities ctx = new TAS2013Entities();
                List<Emp> emps = new List<Emp>();
                List<Section> secs = new List<Section>();
                int count = 0;
                List<Emp> temEmps = new List<Emp>();
                emps = ctx.Emps.ToList();
                secs = ctx.Sections.ToList();
                foreach (var emp in emps)
                {
                    if (secs.Where(aa => aa.SectionID == emp.SecID).Count() == 0)
                    {
                        count++;
                        //short sss = secs.Where(aa => aa.SectionName == "To Be Fixed" && aa.CompanyID == emp.CompanyID).First().SectionID;
                        //emp.SecID = sss;
                        temEmps.Add(emp);
                        //ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void AdjustSections()
        {
            TAS2013Entities ctx = new TAS2013Entities();
          //  List<Section> secs = ctx.Sections.Where(aa => aa.CompanyID != 1).ToList();
            List<Section> secs = ctx.Sections.ToList();
            foreach (var sec in secs)
            {
                try
                {
                    //for multiple companies
                    //if (sec.CompanyID != sec.Department.CompanyID)
                    //{

                    //    List<Department> departments = new List<Department>();
                    //    departments = ctx.Departments.Where(aa => aa.CompanyID == sec.CompanyID && aa.DeptName == sec.Department.DeptName).ToList();
                    //    // check if the company that the employee belongs to contains the departmetn for the section
                    //    if (departments.Count > 0)
                    //    {
                    //        // replace sections's department with the correct one
                    //        sec.DeptID = departments.First().DeptID;
                    //        ctx.SaveChanges();
                    //    }
                    //    else
                    //    {
                    //        // create the section in the new company
                    //        Department department = new Department()
                    //        {
                    //            DeptName = sec.Department.DeptName,
                    //            CompanyID = sec.CompanyID
                    //        };
                    //        ctx.Departments.AddObject(department);
                    //        ctx.SaveChanges();
                    //        Department deptConfirm = ctx.Departments.First(aa => aa.DeptName == sec.Department.DeptName && aa.CompanyID == sec.CompanyID);
                    //        sec.DeptID = deptConfirm.DeptID;
                    //        ctx.SaveChanges();
                    //    }



                    //}
                    

                        List<Department> departments = new List<Department>();
                        departments = ctx.Departments.Where(aa =>aa.DeptName == sec.Department.DeptName).ToList();
                        // check if the company that the employee belongs to contains the departmetn for the section
                        if (departments.Count > 0)
                        {
                            // replace sections's department with the correct one
                            sec.DeptID = departments.First().DeptID;
                            ctx.SaveChanges();
                        }
                        else
                        {
                            // create the section in the new company
                            Department department = new Department()
                            {
                                DeptName = sec.Department.DeptName
                               
                            };
                            ctx.Departments.AddObject(department);
                            ctx.SaveChanges();
                            Department deptConfirm = ctx.Departments.First(aa => aa.DeptName == sec.Department.DeptName);
                            sec.DeptID = deptConfirm.DeptID;
                            ctx.SaveChanges();
                        }



                    
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private void RosterFixes()
        {

            int Cvalue = 142;
            DateTime datestart = new DateTime(2015, 08, 01);
            DateTime dateend = new DateTime(2015, 08, 31);
            while (datestart <= dateend)
            {
                string key = "Crew" + Cvalue.ToString() + datestart.ToString("yyMMdd");
                TAS2013Entities db = new TAS2013Entities();
                if (db.RosterDetails.Where(aa => aa.CriteriaValueDate == key).Count() > 0)
                {
                    RosterDetail roster = db.RosterDetails.First(aa => aa.CriteriaValueDate == key);
                    db.RosterDetails.DeleteObject(roster);
                    db.SaveChanges();

                }
                datestart = datestart.AddDays(1);
            }

        }
        TAS2013Entities db = new TAS2013Entities();
        private void ChangeDateOfEntries()
        {
            DateTime dt1 = new DateTime(2015, 09, 11);
            DateTime dt2 = new DateTime(2015, 09, 12);
            DateTime dt3 = new DateTime(2015, 09, 13);
            DateTime dt4 = new DateTime(2015, 09, 14);

            List<PollData> polls = new List<PollData>();
            polls = db.PollDatas.Where(aa => aa.EntDate == dt1).ToList();
            foreach (var poll in polls)
            {
                poll.EntDate = new DateTime(2015, 08, 1);
                TimeSpan tt = poll.EntTime.TimeOfDay;
                poll.EntTime = new DateTime(2015, 08, 1) + tt;
                poll.EmpDate = poll.EmpID.ToString() + poll.EntDate.ToString("yyMMdd");
                poll.Process = false;
                db.SaveChanges();
            }

            polls = db.PollDatas.Where(aa => aa.EntDate == dt2).ToList();
            foreach (var poll in polls)
            {
                poll.EntDate = new DateTime(2015, 08, 2);
                TimeSpan tt = poll.EntTime.TimeOfDay;
                poll.EntTime = new DateTime(2015, 08, 2) + tt;
                poll.EmpDate = poll.EmpID.ToString() + poll.EntDate.ToString("yyMMdd");
                poll.Process = false;
                db.SaveChanges();
            }

            polls = db.PollDatas.Where(aa => aa.EntDate == dt3).ToList();
            foreach (var poll in polls)
            {
                poll.EntDate = new DateTime(2015, 08, 3);
                TimeSpan tt = poll.EntTime.TimeOfDay;
                poll.EntTime = new DateTime(2015, 08, 3) + tt;
                poll.EmpDate = poll.EmpID.ToString() + poll.EntDate.ToString("yyMMdd");
                poll.Process = false;
                db.SaveChanges();
            }

            polls = db.PollDatas.Where(aa => aa.EntDate == dt4).ToList();
            foreach (var poll in polls)
            {
                poll.EntDate = new DateTime(2015, 08, 4);
                TimeSpan tt = poll.EntTime.TimeOfDay;
                poll.EntTime = new DateTime(2015, 08, 4) + tt;
                poll.EmpDate = poll.EmpID.ToString() + poll.EntDate.ToString("yyMMdd");
                poll.Process = false;
                db.SaveChanges();
            }
        }

        private void RandomGenerator(DateTime date)
        {
            TAS2013Entities ctx = new TAS2013Entities();
            List<Emp> emps = new List<Emp>();
            emps = ctx.Emps.Where(aa => aa.CrewID == 1 && aa.Status == true).ToList();
            DateTime ds = new DateTime(2015, 08, 09);
            DateTime de = new DateTime(2015, 08, 13);
            while (ds <= de)
            {
                foreach (var emp in emps)
                {
                    AttData attdata = new AttData();
                    if (ctx.AttDatas.Where(aa => aa.AttDate == ds && aa.EmpID == emp.EmpID).Count() > 0)
                    {
                        attdata = ctx.AttDatas.First(aa => aa.AttDate == ds && aa.EmpID == emp.EmpID);
                        if (attdata != null)
                            ctx.AttDatas.DeleteObject(attdata);
                        List<PollData> polls = new List<PollData>();
                        polls = ctx.PollDatas.Where(aa => aa.EmpID == emp.EmpID && aa.EntDate >= ds && aa.EntDate <= de).ToList();
                        foreach (var poll in polls)
                        {
                            poll.Process = false;
                            ctx.SaveChanges();
                        }
                        ctx.SaveChanges();
                    }
                }
                ds = ds.AddDays(1);
            }
        }
        private void AdjustPollData()
        {
            int currentYear = DateTime.Today.Date.Year;
            int StartYear = currentYear - 3;
            DateTime endDate = DateTime.Today.AddDays(2);
            DateTime startDate = new DateTime(StartYear, 1, 1);
            using (var ctx = new TAS2013Entities())
            {
                List<PollData> polls = ctx.PollDatas.Where(aa => aa.EntDate <= startDate && aa.Process == false).ToList();
                foreach (var item in polls)
                {
                    item.Process = true;
                }
                ctx.SaveChanges();
                polls.Clear();
                polls = ctx.PollDatas.Where(aa => aa.EntDate >= endDate && aa.Process == false).ToList();
                foreach (var item in polls)
                {
                    item.Process = true;
                }
                ctx.SaveChanges();
                ctx.Dispose();
            }
        }
    }
}
