using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CustomControl
{
    public sealed partial class ProgressControl : UserControl
    {

        public ProgressControl()
        {
            this.InitializeComponent();

            status.ValueChanged += Status_ValueChanged;
            this.Unloaded += ProgressControl_Unloaded;
        }

        public static BindingStatus status = new BindingStatus();

        public bool Update { get; set; }
        private bool StopUpdate = false;

        private int state;
        /// <summary>
        /// This represents the state of the animation
        /// </summary>
        /// <returns>
        /// -1 = unknown
        /// 0 = running
        /// 1 = stopped
        /// </returns>
        public int State
        {
            get { return state; }
            set
            {
                state = value;
                if (state != status.Updating)
                    status.Updating = state;
            }
        }



        private void ProgressControl_Unloaded(object sender, RoutedEventArgs e)
        {
            status.ValueChanged -= Status_ValueChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void Status_ValueChanged(int value)
        {
            switch (value)
            {
                case -1:
                    if (rotatingUpdate != null)
                        rotatingUpdate.Stop();
                    if (shrinkUpdate != null)
                        shrinkUpdate.Stop();
                    if (expandUpdate != null)
                        expandUpdate.Stop();
                    if (expandAccept != null)
                        expandAccept.Stop();
                    StopUpdate = false;
                    AcceptDone = false;
                    break;

                case 1:
                    StopUpdate = true;
                    break;

                case 2:
                    /// Starts the animation
                    /// 
                    AcceptDone = false;
                    if (shrinkUpdate != null && expandAccept != null && expandUpdate != null)
                    {
                        shrinkUpdate.Stop();
                        expandAccept.Stop();
                        expandUpdate.Stop();
                    }

                    Start_ProgressIcon();
                    break;

            }



        }

        
        private Storyboard rotatingUpdate;

        public async void Start_ProgressIcon()
        {
            StopUpdate = false;

            rotatingUpdate = new Storyboard();

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 360;
            animation.To = 0;
            animation.BeginTime = TimeSpan.FromSeconds(1);
            animation.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard.SetTarget(animation, projectionTransform_UpdateIcon);
            Storyboard.SetTargetProperty(animation, "RotationZ");

            rotatingUpdate.Children.Clear();
            rotatingUpdate.Children.Add(animation);
            rotatingUpdate.Begin();

            while (!StopUpdate)
            {
                await Task.Delay(100);
            }
            ShrinkProgressIcon();
        }

        Storyboard shrinkUpdate;

        private void ShrinkProgressIcon()
        {
            shrinkUpdate = new Storyboard();

            DoubleAnimation animationX = new DoubleAnimation();
            DoubleAnimation animationY = new DoubleAnimation();
            animationX.Duration = TimeSpan.FromMilliseconds(1000);
            animationY.Duration = TimeSpan.FromMilliseconds(1000);

            animationX.From = 1;
            animationY.From = 1;

            animationX.To = 0;
            animationY.To = 0;


            Storyboard.SetTarget(shrinkUpdate, scaleTransform_UpdateIcon);
            Storyboard.SetTargetProperty(animationX, "ScaleX");
            Storyboard.SetTargetProperty(animationY, "ScaleY");


            shrinkUpdate.Children.Clear();

            shrinkUpdate.Children.Add(animationX);
            shrinkUpdate.Children.Add(animationY);

            shrinkUpdate.Completed += ShrinkUpdate_Completed;

            shrinkUpdate.Begin();

        }

        private void ShrinkUpdate_Completed(object sender, object e)
        {
            rotatingUpdate.Stop();
            ExpandProgressCircle();
            ExpandAcceptIcon();
        }

        Storyboard expandUpdate;

        private void ExpandProgressCircle()
        {
            //scaleTransform_AcceptIcon


            expandUpdate = new Storyboard();

            DoubleAnimation animationX = new DoubleAnimation();
            DoubleAnimation animationY = new DoubleAnimation();
            animationX.Duration = TimeSpan.FromMilliseconds(800);
            animationY.Duration = TimeSpan.FromMilliseconds(800);

            animationX.From = 0;
            animationY.From = 0;

            animationX.To = 1;
            animationY.To = 1;



            Storyboard.SetTarget(expandUpdate, scaleTransform_AcceptCircle);
            Storyboard.SetTargetProperty(animationX, "ScaleX");
            Storyboard.SetTargetProperty(animationY, "ScaleY");


            expandUpdate.Children.Clear();

            expandUpdate.Children.Add(animationX);
            expandUpdate.Children.Add(animationY);

            expandUpdate.Completed += ExpandUpdate_Completed;

            expandUpdate.Begin();

        }

        private void ExpandUpdate_Completed(object sender, object e)
        {

        }

        Storyboard expandAccept;

        private void ExpandAcceptIcon()
        {
            //scaleTransform_AcceptIcon

            expandAccept = new Storyboard();

            DoubleAnimation animationX = new DoubleAnimation();
            DoubleAnimation animationY = new DoubleAnimation();
            animationX.Duration = TimeSpan.FromMilliseconds(800);
            animationY.Duration = TimeSpan.FromMilliseconds(800);

            animationX.From = 0;
            animationY.From = 0;

            animationX.To = 1;
            animationY.To = 1;


            Storyboard.SetTarget(expandAccept, scaleTransform_AcceptIcon);
            Storyboard.SetTargetProperty(animationX, "ScaleX");
            Storyboard.SetTargetProperty(animationY, "ScaleY");


            expandAccept.Children.Clear();

            expandAccept.Children.Add(animationX);
            expandAccept.Children.Add(animationY);

            expandAccept.Completed += ExpandAccept_Completed;

            expandAccept.Begin();

        }

        private bool AcceptDone = false;

        public async Task<bool> AnimationFinished()
        {
            while (!AcceptDone)
            {
                await Task.Delay(100);
            }

            return true;

        }

        private void ExpandAccept_Completed(object sender, object e)
        {
            AcceptDone = true;
        }
    }




    public class BindingStatus
    {
        private int _updating;
        public int Updating
        {
            get { return _updating; }
            set
            {
                _updating = value;
                if (ValueChanged != null)
                    ValueChanged(value);
            }
        }
        public event ValueChangedEventHandler ValueChanged;
    }


    public delegate void ValueChangedEventHandler(int value);

}
