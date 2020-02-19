using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WindowsInput;
using WindowsInput.Native;
using Newtonsoft.Json;

namespace JumpCancelSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LowLevelKeyboardListener listener;
        InputSimulator inputSimulator;

        public MainWindow()
        {
            InitializeComponent();
            listener = new LowLevelKeyboardListener();
            listener.OnKeyPressed += Listener_OnKeyPressed;
            listener.HookKeyboard();
            inputSimulator = new InputSimulator();
            Loaded += MainWindow_Loaded;
            SizeChanged += MainWindow_SizeChanged;
            LocationChanged += MainWindow_LocationChanged;

            if (File.Exists("settings.json"))
            {
                var json = File.ReadAllText("settings.json");
                var settings = JsonConvert.DeserializeObject<Settings>(json);
                Left = settings.Left;
                Top = settings.Top;
                Width = settings.Width;
                Height = settings.Height;
                ((MainViewModel)DataContext).Delay = settings.Delay;
                ((MainViewModel)DataContext).IsEnabled = settings.IsEnabled;
                ((MainViewModel)DataContext).Settings = settings;
            }
            else
            {
                ((MainViewModel)DataContext).Settings = new Settings
                {
                    Left = Left,
                    Top = Top,
                    Width = ActualWidth,
                    Height = ActualHeight
                };
            }
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            ((MainViewModel)DataContext).Settings.Left = Left;
            ((MainViewModel)DataContext).Settings.Top = Top;
            ((MainViewModel)DataContext).SaveSettings();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((MainViewModel)DataContext).Settings.Width = ActualWidth;
            ((MainViewModel)DataContext).Settings.Height = ActualHeight;
            ((MainViewModel)DataContext).SaveSettings();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).Load();
        }

        private bool locked;
        private async void Listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
            if(locked) return;

            if (!((MainViewModel)DataContext).IsEnabled)
                return;

            var keyPressed = ((MainViewModel)DataContext).Mappings.FirstOrDefault(i => i.Key.Equals((int)e.KeyPressed));
            if (keyPressed == null)
                return;

            locked = true;
            var delay = ((MainViewModel)DataContext).Delay;
            var isHooked = ((MainViewModel)DataContext).IsHooked;
            inputSimulator.Keyboard.KeyDown(VirtualKeyCode.LSHIFT);

            switch ((Jump)keyPressed.Jump)
            {
                case Jump.Frame1:
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
                    break;
                case Jump.Frame2:
                case Jump.Frame3:
                case Jump.Frame4:
                case Jump.Frame5:
                case Jump.Frame6:
                case Jump.Frame7:
                case Jump.Frame8:
                case Jump.Frame9:
                case Jump.Frame10:
                case Jump.Frame11:
                case Jump.Frame12:
                case Jump.Frame13:
                case Jump.Frame14:
                case Jump.Frame15:
                case Jump.Frame16:
                case Jump.Frame17:
                case Jump.Frame18:
                case Jump.Frame19:
                case Jump.Frame20:
                case Jump.Frame21:
                case Jump.Frame22:
                case Jump.Frame23:
                    if (isHooked)
                    {
                        await captureInterface.WaitNextFrameAsync();
                    }

                    await Task.Delay(delay * (keyPressed.Jump - 1));
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
                    switch ((Cactus)keyPressed.Option)
                    {
                        case Cactus.None:
                            break;
                        case Cactus.Cactus1:
                        case Cactus.Cactus2:
                        case Cactus.Cactus3:
                            for (int i = 0; i <= keyPressed.Option; i++)
                            {
                                inputSimulator.Keyboard.KeyUp(VirtualKeyCode.RSHIFT);
                                await Task.Delay(delay);
                            }
                            break;
                    }

                    break;
                case Jump.JumpCancel:
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
                    if (isHooked)
                    {
                        await captureInterface.WaitNextFrameAsync();
                    }
                    else
                    {
                        await Task.Delay(delay);
                    }

                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.RSHIFT);
                    break;
                case Jump.MiniF:
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
                    if (isHooked)
                    {
                        await captureInterface.WaitNextFrameAsync();
                    }
                    await Task.Delay(delay);
                    await Task.Delay(delay);

                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.RSHIFT);
                    break;
                case Jump.MegaJump3:
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.RSHIFT);
                    await Task.Delay(delay);
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.RSHIFT);
                    await Task.Delay(delay);
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.RSHIFT);
                    await Task.Delay(delay);
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.RSHIFT);
                    await Task.Delay(delay);
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.RSHIFT);
                    await Task.Delay(delay);
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.RSHIFT);
                    //inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
                    //inputSimulator.Keyboard.KeyUp(VirtualKeyCode.RSHIFT);
                    //if (isHooked)
                    //{
                    //    await captureInterface.WaitNextFrameAsync();
                    //}
                    //for (int i = 0; i < 6; i++)
                    //{
                    //    await Task.Delay(delay);
                    //    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
                    //}
                    break;
            }

            locked = false;
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).AddMapping();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            var json = JsonConvert.SerializeObject(((MainViewModel)DataContext).Mappings);
            File.WriteAllText("mappings.json", json);
        }

        private async void SetKey(object sender, MouseButtonEventArgs e)
        {

            var mapping = (sender as TextBlock).DataContext as Mapping;
            mapping.AnyKeyVisibility = Visibility.Visible;
            await ((MainViewModel)DataContext).SetKeyAsync(mapping);
            mapping.AnyKeyVisibility = Visibility.Collapsed;
        }

        private void DeleteItem(object sender, MouseButtonEventArgs e)
        {
            var mapping = (sender as Grid).DataContext as Mapping;
            ((MainViewModel)DataContext).Mappings.Remove(mapping);
        }

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                var txtBox = sender as TextBox;
                txtBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
            }
        }

        private void InjectClick(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).Inject();
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).Refresh();
        }

        private CaptureInterface captureInterface;
        public void Callback(CaptureInterface captureInterface)
        {
            this.captureInterface = captureInterface;
            this.captureInterface.Detached += CaptureInterface_Detached;
            DxHookCheckbox.Foreground = Brushes.Green;
        }

        private void CaptureInterface_Detached(object sender, EventArgs e)
        {
            ((MainViewModel)DataContext).IsHooked = false;
            DxHookCheckbox.Foreground = Brushes.Black;
        }
    }
}
