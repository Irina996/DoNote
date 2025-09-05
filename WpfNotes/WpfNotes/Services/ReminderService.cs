using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using WpfNotes.Models.TaskItem;
using Timer = System.Timers.Timer;

namespace WpfNotes.Services
{
    public class ReminderService
    {
        private IEnumerable<TaskItem> _taskItems;
        private readonly TaskbarIcon _icon;
        private readonly Timer _timer;

        public ReminderService()
        {
            this._taskItems = null;
            this._icon = new TaskbarIcon();

            // Check every 30 seconds
            _timer = new Timer(TimeSpan.FromSeconds(30).TotalMilliseconds);
            _timer.Elapsed += CheckReminders;
        }

        private void CheckReminders(object? sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now;
            var buffer = TimeSpan.FromSeconds(30);
            var currentTasks = _taskItems
                .Where(t => !t.IsCompleted && t.Notification <= now && t.Notification + buffer >= now)
                .ToList();
            foreach (var task in currentTasks)
            {
                _icon.ShowBalloonTip("Task", task.Content, BalloonIcon.Info);
            }
        }

        public void Start()
        {
            _timer.Start();
        }

        public void SetTasks(IEnumerable<TaskItem> tasks)
        {
            _taskItems = tasks;
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void Dispose()
        {
            
        }
    }
}
