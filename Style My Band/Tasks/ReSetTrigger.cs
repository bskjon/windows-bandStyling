using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Popups;

namespace Tasks
{
    public static class ReSetTrigger
    {

        public static async Task<bool> GetTask(string Name, object trigger)
        {
           if (trigger is TimeTrigger)
           {
                MessageDialog msg = new MessageDialog("TimeTrigger"); msg.ShowAsync();
           }
           else { MessageDialog msg = new MessageDialog(trigger.ToString()); msg.ShowAsync(); }

            return false;
        }



    }
}
