using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using TASDownloadService.Model;
using System.Net.NetworkInformation;
using TASDownloadService.Helper;
using WMSFFService;
using TASDownloadService.AttProcessDaily;
using TASDownloadService.AttProcessSummary;
using ReadersCommLibrary;

namespace TASDownloadService
{
    public partial class TASService : ServiceBase
    {
        System.Timers.Timer timer;
        static bool isTimerRunning = false;
        Thread DownloadThread = null;
        private static bool isServiceRunning = false;
        TimeSpan ProcessTime = new TimeSpan(16, 20, 00);
        public TASService()
        {
            InitializeComponent();
            timer = new System.Timers.Timer();
            timer.Interval = 5000;
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        }
        MyCustomFunctions _myHelperClass = new MyCustomFunctions();

        #region -- Service Start, Stop --
        // Service Start
        protected override void OnStart(string[] args)
        {
            try
            {
                timer.Start();
                DownloadThread = new Thread(RunService);
                Thread.Sleep(2000);
                DownloadThread.Start();
                isServiceRunning = true;
                _myHelperClass.WriteToLogFile("******************WMS Service Started*************** " );
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnStop()
        {
            try
            {
                isServiceRunning = false;
                timer.Stop();
                DownloadThread.Join();
                _myHelperClass.WriteToLogFile("******************WMS Service Stopped*************** ");
            }
            catch (Exception ex)
            {

            }
        }
        public void StartService()
        {
            OnStart(null);
        }
        public void StopService()
        {
            OnStop();
        }
        #endregion
        
        private void RunService()
        {
            while (isServiceRunning)
            {

            }
        }

        ///////////////---------TIMER CLICKED--------/////////////////
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                timer.Stop();
                using (var context = new TAS2013Entities())
                {
                    // If Service run at specific times
                    if (context.Options.FirstOrDefault().DownTime == true)
                    {
                        List<DownloadTime> _downloadTime = new List<DownloadTime>();
                        _downloadTime = context.DownloadTimes.ToList();
                        foreach (var item in _downloadTime)
                        {
                            // Add 2 minutes extra to download time
                            TimeSpan _dwnTimeEnd = (TimeSpan)item.DownTime + new TimeSpan(0, 2, 0);
                            if (DateTime.Now.TimeOfDay >= item.DownTime && DateTime.Now.TimeOfDay <= _dwnTimeEnd)
                            {
                                GlobalSettings._dateTime = Properties.Settings.Default.MyDateTime;
                                //Download Data from Devices
                                DownloadDataFromDevices();
                                //set Process = 1 in PollData where Date varies from Date ranges
                                AdjustPollData();
                                //Prcoess PollData to Attendance Data
                                ProcessAttendance pa = new ProcessAttendance();
                                pa.ProcessDailyAttendance();

                            }
                            /////////--------Process Monthly Attendance-------//////////
                            // Monthly Process will be run in between 11:50 PM to 11:51 PM
                            TimeSpan monthlyTStart = new TimeSpan(23, 50, 0);
                            TimeSpan monthlyTEnd = new TimeSpan(23, 51, 0);
                            if (DateTime.Now.TimeOfDay >= monthlyTStart && DateTime.Now.TimeOfDay <= monthlyTEnd)
                            {
                                DateTime dtStart = DateTime.Today.AddDays(-2);
                                DateTime dtend = DateTime.Today;
                                //Correct Attendance Status for Monthly Process
                                CorrectAttEntriesWithWrongFlags(dtStart,dtend);
                                //Process Monthly Attendance for Permanent
                                ProcessMonthlyAttForPermanentFF();
                                //Process Monthly Attendance for Contractuals
                                ProcessMonthlyAttForContractualFF();
                                DateTime dateStart = DateTime.Today.AddDays(-10);
                                DateTime dateEnd = DateTime.Today;
                                // Process Attendance Data into Daily Summary
                                DailySummaryClass ds = new DailySummaryClass(dateStart,dateEnd);
                                //////////-------Set Device Data Time on Sundays Night------////////////
                                if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    AdjustDateTimeOfDevices();
                                }
                            }
                        }
                        ///-----------Process Manual Attendance Request--------////////////
                        ProcessManualAttendanceRequest();
                    }
                    else
                    {
                        // if user wants to process the attendance without intervals
                    }

                }

            }
            catch
            {

            }
            finally
            {
                timer.Start();
            }
        }

        #region ---- Set Device Date Time ----
        public void AdjustDateTimeOfDevices()
        {
            try
            {
                using (var ctx = new TAS2013Entities())
                {
                    List<Reader> readers = new List<Reader>();
                    readers = ctx.Readers.Where(aa => aa.Status == true).ToList();
                    foreach (var rdr in readers)
                    {
                        ZKReader zk = new ZKReader();
                        if (IsConnectedToInternet(rdr.IpAdd))
                        {
                            if (zk.Connect(rdr.IpAdd, rdr.IpPort))
                            {
                                zk.SetDateTime();
                                zk.Disconnect();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        private bool IsConnectedToInternet(string _IPAddress)
        {
            //Uri url = new Uri(_IPAddress);
            //string pingurl = string.Format("{0}", url.Host);
            string host = _IPAddress;
            bool result = false;
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(host, 3000);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch { return false; }
            return result;
        }
        #endregion

        #region -- Process Manual Attendance --

        private void ProcessManualAttendance()
        {
            using (var ctx = new TAS2013Entities())
            {
                ManualProcess mp = new ManualProcess();
                List<Emp> emps = new List<Emp>();
                DateTime date = new DateTime(2015, 09, 10);
                emps = ctx.Emps.Where(aa => aa.Status==true).ToList();
                //emps.AddRange(ctx.Emps.Where(aa => (aa.EmpType.CatID == 2 && aa.CompanyID == 1)).ToList());
                //emps.AddRange(ctx.Emps.Where(aa => (aa.EmpType.CatID == 4) && aa.CompanyID == 1).ToList());
                //emps.AddRange(ctx.Emps.Where(aa => (aa.EmpType.CatID == 1) && aa.CompanyID == 1).ToList());
                //mp.BootstrapAttendance(date, emps);
                List<AttData> atts = new List<AttData>();
                //atts.AddRange(ctx.AttDatas.Where(aa => aa.Emp.EmpType.CatID == 2 && aa.Emp.CompanyID == 1 && aa.AttDate == date));
                //atts.AddRange(ctx.AttDatas.Where(aa => aa.Emp.EmpType.CatID == 4 && aa.Emp.CompanyID == 1 && aa.AttDate == date));
                //atts.AddRange(ctx.AttDatas.Where(aa => aa.Emp.EmpType.CatID == 1 && aa.Emp.CompanyID == 1 && aa.AttDate == date));
                atts = ctx.AttDatas.Where(aa => aa.AttDate == date).ToList();
                mp.ManualProcessAttendance(date, emps);
            }
        }

        private void ProcessManualAttendanceDaily( List<Emp> emps,DateTime dt)
        {
            using (var ctx = new TAS2013Entities())
            {
                // check for is attendance processed 
                if (ctx.AttProcesses.Where(aa => aa.ProcessDate == dt).Count() > 0)
                {
                    //process manual attendance
                    ManualProcess mp = new ManualProcess();
                    mp.ManualProcessAttendance(dt, emps);
                    ////////////////////////////
                    //Check for Job Card//
                    //////////////////////////
                    try
                    {
                        ProcessJobCard jc = new ProcessJobCard();
                        jc.ProcessJobCards(dt);
                    }
                    catch (Exception ex)
                    {
                        _myHelperClass.WriteToLogFile("Error at Create Function Process Job Card " + dt.ToString());
                    }
                    ////Process Edit Attendance Entries
                    ProcessEditAttendanceEntries pe = new ProcessEditAttendanceEntries();
                    pe.ProcessManualEditAttendance(dt, dt);

                }
                else
                {
                    //Process new attendance
                    ProcessAttendance pa = new ProcessAttendance();
                    pa.ProcessDailyAttendance();
                }
            }
        }

        private void ProcessManualAttendanceMonthly(List<Emp> emps, AttProcessorScheduler req)
        {
            if (req.DateFrom.Date.Day == 21)
            {
                DateTime dt1 = new DateTime(req.DateFrom.Year,req.DateFrom.Month+1,20);
                ProcessMonthlyPermanent(req.DateFrom, dt1);
            }
            if (req.DateFrom.Date.Day == 1)
            {
                int daysInMonths = System.DateTime.DaysInMonth(req.DateFrom.Year, req.DateFrom.Month);
                DateTime dt1 = new DateTime(req.DateFrom.Year, req.DateFrom.Month, daysInMonths);
                ProcessMonthlyPermanent(req.DateFrom, dt1);
            }
        }

        private void ProcessManualAttendanceRequest()
        {
            try
            {
                using (var cctx = new TAS2013Entities())
                {
                    List<AttProcessorScheduler> requests = new List<AttProcessorScheduler>();
                    requests = cctx.AttProcessorSchedulers.Where(aa => aa.ProcessingDone == false).ToList();
                    foreach (var req in requests)
                    {
                        List<Emp> emps = new List<Emp>();
                        // Process Entries of Location
                        if (req.Criteria=="L")
                        {
                            emps = cctx.Emps.Where(aa => aa.LocID == req.LocationID && aa.Status == true).ToList();
                        }
                        //// Process Entries of Companies
                        //if (req.Criteria=="C")
                        //{
                        //    emps = cctx.Emps.Where(aa => aa.CompanyID == req.CompanyID && aa.Status == true).ToList();
                        //}
                        if (req.ProcessCat==true)
                        {
                            emps = emps.Where(aa => aa.EmpType.CatID == req.CatID).ToList();
                        }
                        DateTime dt = req.DateFrom;
                            switch (req.PeriodTag)
                            {
                                case "M":
                                    ProcessManualAttendanceMonthly(emps, req);
                                    break;
                                case "D":
                                    while (dt <= req.DateTo)
                                    {
                                        ProcessManualAttendanceDaily(emps, dt);
                                        dt = dt.AddDays(1);
                                    }
                                    break;
                            }
                            req.ProcessingDone = true;
                            cctx.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        #endregion

        #region -- Monthly Process Attendance--
        //Process Monthly Attendance for Contractuals
        private void ProcessMonthlyAttForContractualFF()
        {
            //Process Month till end of month
            DateTime endDate = DateTime.Today.Date;
            int currentDay = DateTime.Today.Date.Day;
            int currentMonth = DateTime.Today.Date.Month;
            int currentYear = DateTime.Today.Date.Year;
            DateTime startDate = new DateTime(currentYear, currentMonth, 1);
            if (currentMonth == 1&& currentDay <10)
            {
                currentMonth = 13;
                currentYear = currentYear - 1;
            }
            if (endDate.Day < 10)
            {
                currentMonth = currentMonth - 1;
                int DaysInPreviousMonth = System.DateTime.DaysInMonth(currentYear, currentMonth);
                endDate = new DateTime(currentYear, currentMonth, DaysInPreviousMonth);
                startDate = new DateTime(currentYear, currentMonth, 1);
            }
            else
            {
                startDate = new DateTime(currentYear, currentMonth, 1);
            }
            ProcessMonthlyContractuals(startDate, endDate);
            if(DateTime.Today.Day < 10)
                ProcessMonthlyContractuals(new DateTime(DateTime.Today.Year,DateTime.Today.Month,1),new DateTime(DateTime.Today.Year,DateTime.Today.Month,DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month)));
        }

        //Process Contractuals Monthly Attendance
        private void ProcessMonthlyContractuals(DateTime startDate, DateTime endDate)
        {
            TAS2013Entities ctx = new TAS2013Entities();
            // Pass list of selected emp Attendance data to optimize sql query 
            List<AttData> _AttData = new List<AttData>();
            List<AttData> _EmpAttData = new List<AttData>();
            _AttData = ctx.AttDatas.Where(aa => aa.AttDate >= startDate && aa.AttDate <= endDate).ToList();
            int count = 0;
            //for multi company names
            //List<Emp> _Emp = ctx.Emps.Where(em => em.EmpType.CatID == 3 && em.Status == true && em.CompanyID==1).ToList();
            //for single company
            List<Emp> _Emp = ctx.Emps.Where(em => em.EmpType.CatID == 3 && em.Status == true).ToList();
            //for multi company names
           // List<Emp> _oEmp = ctx.Emps.Where(em => em.CompanyID>1 && em.Status == true).ToList();
            List<Emp> _oEmp = ctx.Emps.Where(em => em.Status == true).ToList();
            _Emp.AddRange(_oEmp);
            int _TE = _Emp.Count;
            foreach (Emp emp in _Emp)
            {
                count++;
                try
                {
                    ContractualMonthlyProcessor cmp = new ContractualMonthlyProcessor();
                    _EmpAttData = _AttData.Where(aa => aa.EmpID == emp.EmpID).ToList();
                    if (!cmp.processContractualMonthlyAttSingle(startDate, endDate, emp, _EmpAttData))
                    {
                    }

                }
                catch (Exception ex)
                {

                }
            }
        }

        //Process Monthly Attendance for Permanent
        private void ProcessMonthlyAttForPermanentFF()
        {
            //Process Month till end of month
            DateTime endDate = DateTime.Today.Date;
            int currentDay = DateTime.Today.Date.Day;
            int currentMonth = DateTime.Today.Date.Month;
            int currentYear = DateTime.Today.Date.Year;

            if (endDate.Day > 20)
            {
                endDate = new DateTime(currentYear, currentMonth, 20);
            }

            if (currentMonth == 1)
            {
                currentMonth = 12;
                currentYear = currentYear - 1;
            }
            else
                currentMonth = currentMonth - 1;
            DateTime startDate = new DateTime(currentYear, currentMonth, 21);
            ProcessMonthlyPermanent(startDate, endDate);
        }

        private void ProcessMonthlyPermanent(DateTime _startDate, DateTime _endDate)
        {
            TAS2013Entities context = new TAS2013Entities();
            // Pass list of selected emp Attendance data to optimize sql query 
            List<AttData> _AttData = new List<AttData>();
            List<AttData> _EmpAttData = new List<AttData>();
            _AttData = context.AttDatas.Where(aa => aa.AttDate >= _startDate && aa.AttDate <= _endDate).ToList();
            int count = 0;
            //multi company 
            //  List<Emp> _Emp = context.Emps.Where(em => (em.EmpType.CatID !=3) && em.CompanyID==1 && em.Status == true).ToList();
            // generic no company
              List<Emp> _Emp = context.Emps.Where(em => (em.EmpType.CatID !=3) && em.Status == true).ToList();
           List<Emp> _Emps = context.Emps.Where(em => (em.LocID==4 || em.LocID==5) && em.Status == true).ToList();
           _Emp.AddRange(_Emps);
            int _TE = context.Emps.Where(em => (em.EmpType.CatID == 4 || em.EmpType.CatID == 2) && em.Status == true).Count();
            foreach (Emp emp in _Emp)
            {
                count++;
                try
                {
                    PermanetMonthlyProcess cmp = new PermanetMonthlyProcess();
                    _EmpAttData = _AttData.Where(aa => aa.EmpID == emp.EmpID).ToList();
                    if (!cmp.processPermanentMonthlyAttSingle(_startDate, _endDate, emp, _EmpAttData))
                    {

                    }

                }
                catch (Exception ex)
                {

                }
            }
        }
        #endregion

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

        private void CorrectAttEntriesWithWrongFlags(DateTime startdate, DateTime endDate)
        {
            using (var ctx = new TAS2013Entities())
            {
                //Set DutyCode=G where StatusGZ=true and DutyCode !=G
                List<AttData> _attDataForGZ = ctx.AttDatas.Where(aa => aa.AttDate >= startdate && aa.AttDate <= endDate && aa.StatusGZ == true && aa.DutyCode != "G").ToList();
                foreach (var item in _attDataForGZ)
                {
                    item.DutyCode = "G";
                }
                ctx.SaveChanges();

                //Set DutyCode=R where StatusDO=true and DutyCode !=R
                List<AttData> _attDataForDO = ctx.AttDatas.Where(aa => aa.AttDate >= startdate && aa.AttDate <= endDate && aa.StatusDO == true && aa.DutyCode != "R").ToList();
                foreach (var item in _attDataForDO)
                {
                    item.DutyCode = "R";
                }
                ctx.SaveChanges();
                ctx.Dispose();
            }
        }

        private void GetDatafromOldDatabase()
        {
            CopyingPollData cpd = new CopyingPollData();
            cpd.CopyingPolls();
        }

        private void DownloadDataFromDevices()
        {
            try
            {
                Downloader d = new Downloader();
                d.DownloadDataInIt();
            }
            catch (Exception ex)
            {

            }
        }

    }
}
