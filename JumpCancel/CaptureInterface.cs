using System;
using System.Threading.Tasks;
using System.Windows;

namespace JumpCancelSimulator
{
    public class CaptureInterface : MarshalByRefObject
    {
        private bool nextFrame;
        public event EventHandler Detached;
        public void Frame()
        {
            nextFrame = true;
        }

        public void SetCallback()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = (Application.Current.MainWindow as MainWindow);
                window.Callback(this);
            });
        }

        public async Task WaitNextFrameAsync()
        {
            nextFrame = false;
            DateTime timeout = DateTime.Now;
            while (!nextFrame)
            {
                if ((DateTime.Now - timeout).Seconds > 3)
                {
                    Detached?.Invoke(this, null);
                    break;
                }
            }
        }
    }
}
