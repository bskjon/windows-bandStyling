using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Popups;
using Tasks;

namespace Core
{
    class BackgroundTask
    {
        public enum Triggers
        {
            TimeTrigger
        }


        public static async Task<bool> CreateBackgroundTask(Triggers trigger, uint time, string taskname)
        {
            if ( await VerifyPermission() == false) { MessageDialog msg = new MessageDialog("The applications request to register background tasks were denied..", "Access Denied.."); await msg.ShowAsync(); return false; }
            if ( await TaskPresent(taskname) != false) { MessageDialog msg = new MessageDialog("The task you are trying to register has already been created according to the system..", "Creating task failed.."); msg.ShowAsync(); return false; }
            string EntryPoint = await SetEntryPoint(trigger);
            if (EntryPoint == "") { MessageDialog msg = new MessageDialog("Some details regarding the TaskEntryPoint is missing to create task..", "Missing.."); msg.ShowAsync(); return false; }

            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
            builder.Name = taskname;
            builder.TaskEntryPoint = EntryPoint;
            builder.SetTrigger(await SetTrigger(trigger, time));

            BackgroundTaskRegistration regiser = builder.Register();
            regiser.Completed += new BackgroundTaskCompletedEventHandler(Tasks.Personalize.OnCompleted);

            return true;
            
        }

        public static async Task<IBackgroundTrigger> SetTrigger(Triggers trigger, uint time)
        {
            IBackgroundTrigger ntrigger = null;
            switch (trigger)
            {
                case Triggers.TimeTrigger:
                    ntrigger = new TimeTrigger(time, false);
                    break;
            }

            return ntrigger;
        }

        public static async Task<string> SetEntryPoint(Triggers trigger)
        {
            switch (trigger)
            {
                case Triggers.TimeTrigger:
                    return typeof(Tasks.Personalize).FullName;
                    break;
            }
            return "";
        }

        public static async Task<bool> TaskPresent(string taskname)
        {
            foreach ( var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskname)
                {
                    return true;
                }
            }
            return false;
        }

        public static async Task<bool> VerifyPermission()
        {
            var accessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (accessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity || accessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
