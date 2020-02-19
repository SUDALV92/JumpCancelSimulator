using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;

namespace JumpCancelSimulator
{
    public class Mapping : BaseViewModel
    {
        private Visibility _anyKeyVisibility;
        private bool _optionAvailable;
        private int _jump;
        private int _option;
        public int Key { get; set; }

        [JsonIgnore]
        public string KeyString => ((Key)Key).ToString();

        public int Jump
        {
            get => _jump;
            set
            {
                _jump = value;
                switch ((Jump)value)
                {
                    case JumpCancelSimulator.Jump.JumpCancel:
                    case JumpCancelSimulator.Jump.MiniF:
                    case JumpCancelSimulator.Jump.MegaJump3:
                        OptionAvailable = false;
                        Option = (int)Cactus.None;
                        break;
                    default:
                        OptionAvailable = true;
                        break;
                }
            }
        }

        public int Option
        {
            get => _option;
            set { _option = value; OnPropertyChanged(); }
        }

        [JsonIgnore]
        public Dictionary<int, string> Options { get; set; }

        [JsonIgnore]
        public bool OptionAvailable
        {
            get => _optionAvailable;
            set
            {
                if (value == _optionAvailable) return;
                _optionAvailable = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public Visibility AnyKeyVisibility
        {
            get => _anyKeyVisibility;
            set
            {
                if (value == _anyKeyVisibility) return;
                _anyKeyVisibility = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public Dictionary<int, string> Jumps { get; set; }

        public Mapping()
        {
            AnyKeyVisibility = Visibility.Collapsed;
            OptionAvailable = true;
            Jumps = new Dictionary<int, string>();
            var array = Enum.GetValues(typeof(Jump));
            foreach (Jump jump in array)
            {
                Jumps.Add((int)jump, jump.ToString());
            }

            var cactusArray = Enum.GetValues(typeof(Cactus));
            Options = new Dictionary<int, string>();
            foreach (Cactus cactus in cactusArray)
            {
                Options.Add((int)cactus, cactus.ToString());
            }

        }
    }
}