using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using EasyHook;
using Newtonsoft.Json;

namespace JumpCancelSimulator
{
    public class MainViewModel : BaseViewModel
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        private ObservableCollection<Mapping> _mappings;
        private int _delay;
        private bool _isEnabled;
        private bool _isHooked;
        private Visibility _hookVisibility;
        private string _injectButtonText;
        private List<Process> _processList;

        public MainViewModel()
        {
            Delay = 20;
            IsEnabled = true;
            HookVisibility = Visibility.Collapsed;
            IsHooked = false;
            InjectButtonText = "Inject";
        }
        public int Delay
        {
            get => _delay;
            set
            {
                _delay = value;
                OnPropertyChanged();
                SaveSettings();
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                SaveSettings();
            }
        }

        public ObservableCollection<Mapping> Mappings
        {
            get => _mappings;
            set
            {
                if (Equals(value, _mappings)) return;
                _mappings = value;
                OnPropertyChanged();
            }
        }

        public Settings Settings { get; set; }

        public bool IsHooked
        {
            get => _isHooked;
            set
            {
                if (value == _isHooked) return;
                _isHooked = value;
                OnPropertyChanged();
                if (_isHooked)
                {
                    HookVisibility = Visibility.Visible;
                    Refresh();
                }
                else
                {
                    ChannelName = null;
                    HookVisibility = Visibility.Collapsed;
                }
            }
        }

        public Visibility HookVisibility
        {
            get => _hookVisibility;
            set
            {
                if (value == _hookVisibility) return;
                _hookVisibility = value;
                OnPropertyChanged();
            }
        }

        public string InjectButtonText
        {
            get => _injectButtonText;
            set
            {
                if (value == _injectButtonText) return;
                _injectButtonText = value;
                OnPropertyChanged();
            }
        }

        public List<Process> ProcessList
        {
            get => _processList;
            set
            {
                if (Equals(value, _processList)) return;
                _processList = value;
                OnPropertyChanged();
            }
        }

        public Process SelectedProcess { get; set; }


        public void AddMapping()
        {
            Mappings.Add(new Mapping());

            //Mappings = new List<Mapping>(Mappings);
        }

        public void Load()
        {
            try
            {
                var json = File.ReadAllText("mappings.json");
                Mappings = JsonConvert.DeserializeObject<ObservableCollection<Mapping>>(json) ?? new ObservableCollection<Mapping>();
            }
            catch (FileNotFoundException)
            {
                //ok
                Mappings = new ObservableCollection<Mapping>();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Mappings = new ObservableCollection<Mapping>();
            }
        }

        public async Task SetKeyAsync(Mapping mapping)
        {
            var allPossibleKeys = Enum.GetValues(typeof(Key));

            bool results = false;
            while (!results)
            {
                await Task.Delay(1);
                foreach (Key currentKey in allPossibleKeys)
                {
                    Key key = currentKey;
                    if (key != Key.None)
                        if (Keyboard.IsKeyDown(currentKey))
                        {
                            mapping.Key = (int)currentKey;
                            results = true;
                            break;
                        }
                }
            }

            Mappings = new ObservableCollection<Mapping>(Mappings);
            //OnPropertyChanged(nameof(Mappings));
        }

        public void SaveSettings()
        {
            if (Settings != null)
            {
                var settings = new Settings
                {
                    Top = Settings.Top,
                    Left = Settings.Left,
                    Delay = Delay,
                    Height = Settings.Height,
                    Width = Settings.Width,
                    IsEnabled = IsEnabled
                };
                var json = JsonConvert.SerializeObject(settings);
                File.WriteAllText("settings.json", json);
            }
        }

        public void Refresh()
        {
            var temp = Process.GetProcesses();
            var temp2 = new List<Process>();
            //YYGameMakerYY
            //TApplication
            foreach (var process in temp)
            {
                var className = new StringBuilder(100);
                GetClassName(process.MainWindowHandle, className, className.Capacity);
                if (className.ToString().Equals("YYGameMakerYY") || className.ToString().Equals("TApplication"))
                    temp2.Add(process);
            }

            ProcessList = temp2;
            temp = null;
            temp2 = null;
        }

        private static string ChannelName;
        public void Inject()
        {
            ChannelName = null;
            var dll = "JumpCancelSimulator.Capture.dll";
            Config.Register("Jump Cancel Simulator", "JumpCancelSimulator.exe", dll);
            RemoteHooking.IpcCreateServer<CaptureInterface>(ref ChannelName, WellKnownObjectMode.Singleton);
            RemoteHooking.Inject(SelectedProcess.Id, dll, dll, ChannelName);
            
        }
    }
}
