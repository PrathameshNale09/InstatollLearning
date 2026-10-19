using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using Timer = System.Timers.Timer;

namespace InstaTollLearningApp.Services
{
    internal class ActivitySummaryService
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        private static long _created;
        private static long _updated;
        private static long _deleted;

        private static Timer _timer;
        private static readonly object TimerLock = new object();

        public static void Start()
        {
            lock (TimerLock)
            {
                if (_timer != null)
                    return;

                int minutes = 60;
                int configuredMinutes;

                if (int.TryParse(
                    ConfigurationManager.AppSettings[
                        "HourlySummaryIntervalMinutes"],
                    out configuredMinutes)
                    && configuredMinutes > 0)
                {
                    minutes = configuredMinutes;
                }

                _timer = new Timer(minutes * 60 * 1000);
                _timer.AutoReset = true;
                _timer.Elapsed += OnTimerElapsed;
                _timer.Start();

                Logger.Info(
                    "Activity summary timer started. Interval: {0} minutes.",
                    minutes);
            }
        }

        public static void RecordCreated()
        {
            Interlocked.Increment(ref _created);
        }

        public static void RecordUpdated()
        {
            Interlocked.Increment(ref _updated);
        }

        public static void RecordDeleted()
        {
            Interlocked.Increment(ref _deleted);
        }

        private static void OnTimerElapsed(
            object sender, ElapsedEventArgs e)
        {
            WriteSummary();
        }

        public static void WriteSummary()
        {
            long created = Interlocked.Exchange(ref _created, 0);
            long updated = Interlocked.Exchange(ref _updated, 0);
            long deleted = Interlocked.Exchange(ref _deleted, 0);

            Logger.Info(
                "Hourly Summary | Created: {0} | Updated: {1} | Deleted: {2}",
                created,
                updated,
                deleted);
        }

        public static void Stop()
        {
            lock (TimerLock)
            {
                if (_timer == null)
                    return;

                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }

            WriteSummary();
            Logger.Info("Activity summary timer stopped.");
        }
    }
}
