using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;

namespace OfacLocalDbSyncApp
{
    public partial class Service1 : ServiceBase
    {
        private Timer _timer = null;
        public Service1()
        {
            InitializeComponent();
        }

        public void onDebug()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            // Pass in the time you want to start and the interval
            int startTime;
            int interval;
            startTime = int.Parse(ConfigurationManager.AppSettings["startTime"].ToString());
            interval = int.Parse(ConfigurationManager.AppSettings["interval"].ToString());
            StartTimer(new TimeSpan(startTime, 0, 0), new TimeSpan(interval, 0, 0));

        }
        protected void StartTimer(TimeSpan scheduledRunTime, TimeSpan timeBetweenEachRun)
        {
            // Initialize timer
            double current = DateTime.Now.TimeOfDay.TotalMilliseconds;
            double scheduledTime = scheduledRunTime.TotalMilliseconds;
            double intervalPeriod = timeBetweenEachRun.TotalMilliseconds;
            double firstExecution = current > scheduledTime ? intervalPeriod - (current - scheduledTime) : scheduledTime - current;

            // This is the method that is called on every interval
            TimerCallback callback = new TimerCallback(RunXMLService);

            _timer = new Timer(callback, null, Convert.ToInt32(firstExecution), Convert.ToInt32(intervalPeriod));

        }
        public void RunXMLService(object state)
        {
            // Code that runs every interval period
            OFACXML ofacXMLObj = new OFACXML();

            ofacXMLObj.loadOfacXML();
        }

        protected override void OnStop()
        {

        }

        

    }
}
