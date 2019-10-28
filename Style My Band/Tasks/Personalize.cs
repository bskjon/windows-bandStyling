using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Tasks
{
    public sealed class Personalize : IBackgroundTask
    {

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //bool res = await ReSetTrigger.GetTask(taskInstance.Task.Name, taskInstance.Task.Trigger);


        }

        public static void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {

        }

    }
}
