﻿
using Caliburn.Micro;
using Dev2.Composition;
using Dev2.Studio.Core;
using Dev2.Studio.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Dev2.ViewModels.Dialogs;

namespace Dev2.Studio.ViewModels.Dialogs
{
    public class Dev2MessageBoxViewModel : Screen
    {
        #region Fields

        private string _messageBoxText;
        private string _caption;
        private MessageBoxButton _button;
        private MessageBoxImage _icon;
        private MessageBoxResult _result;
        private readonly MessageBoxResult _defaultResult;
        private readonly string _dontShowAgainKey;
        private bool _dontShowAgain;
        private bool _isButtonClickedForClosed;
        
        private static Dictionary<string, MessageBoxResult> DontShowAgainOptions;

        #endregion Fields

        #region Constructor

        public Dev2MessageBoxViewModel(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon,
                                       MessageBoxResult defaultResult, string dontShowAgainKey)
        {
            _messageBoxText = messageBoxText;
            _caption = caption;
            _button = button;
            _icon = icon;
            _result = defaultResult;
            _defaultResult = defaultResult;
            _messageBoxText = messageBoxText;
            _dontShowAgainKey = dontShowAgainKey;
        }

        #endregion

        #region Properties

        public bool FocusOk
        {
            get
            {
                return _defaultResult == MessageBoxResult.OK;
            }
        }

        public bool FocusYes
        {
            get
            {
                return _defaultResult == MessageBoxResult.Yes;
            }
        }

        public bool FocusNo
        {
            get
            {
                return _defaultResult == MessageBoxResult.No;
            }
        }

        public bool FocusCancel
        {
            get
            {
                return _defaultResult == MessageBoxResult.Cancel;
            }
        }

        public string MessageBoxText
        {
            get
            {
                return _messageBoxText;
            }
            set
            {
                _messageBoxText = value;
                NotifyOfPropertyChange(() => MessageBoxText);
            }
        }

        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                NotifyOfPropertyChange(() => Caption);
            }
        }

        public MessageBoxButton Button
        {
            get
            {
                return _button;
            }
            set
            {
                _button = value;
                NotifyOfPropertyChange(() => Button);
            }
        }

        public MessageBoxImage Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                NotifyOfPropertyChange(() => Icon);
            }
        }

        public MessageBoxResult Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                NotifyOfPropertyChange(() => Result);
            }
        }

        public bool DontShowAgain
        {
            get
            {
                return _dontShowAgain;
            }
            set
            {
                _dontShowAgain = value;
                NotifyOfPropertyChange(() => DontShowAgain);
            }
        }

        public string DontShowAgainKey
        {
            get
            {
                return _dontShowAgainKey;
            }
        }

        #endregion Properties

        #region Methods

        public void Ok()
        {
            _isButtonClickedForClosed = true;
            Result = MessageBoxResult.OK;
            TryClose();
        }

        public void Yes()
        {
            _isButtonClickedForClosed = true;
            Result = MessageBoxResult.Yes;
            TryClose();
        }

        public void No()
        {
            _isButtonClickedForClosed = true;
            Result = MessageBoxResult.No;
            TryClose();
        }

        public void Cancel()
        {
            _isButtonClickedForClosed = true;
            Result = MessageBoxResult.Cancel;
            TryClose();
        }

        public void Closed()
        {
            if (!_isButtonClickedForClosed)
            {
                Result = MessageBoxResult.None;
                TryClose();
            }
        }

        #endregion

        #region Static Methods

        private static string GetDontShowAgainPersistencePath()
        {
            string path = Path.Combine(new[] 
                { 
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                    StringResources.App_Data_Directory, 
                    StringResources.User_Interface_Layouts_Directory 
                });

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return Path.Combine(path, "DontShowAgainOptions.xml");
        }

        private static void LoadDontShowAgainOptions()
        {
            try
            {
                IFilePersistenceProvider filePersistenceProviderInst = ImportService.GetExportValue<IFilePersistenceProvider>();
                string data = filePersistenceProviderInst.Read(GetDontShowAgainPersistencePath());
                DontShowAgainOptions = new Dictionary<string, MessageBoxResult>();

                foreach(XElement element in XElement.Parse(data).Elements())
                {
                    string key = element.Attribute("Key").Value;
                    MessageBoxResult val = (MessageBoxResult)Enum.Parse(typeof(MessageBoxResult), element.Attribute("Value").Value);
                    DontShowAgainOptions.Add(key, val);
                }
            }
            catch(Exception)
            {
                // If deserialization fails then create a blank dicitonary so that when a save occurs it will be saved in teh correct format.
                DontShowAgainOptions = new Dictionary<string, MessageBoxResult>();
            }
        }

        private static void SaveDontShowAgainOptions()
        {
            try
            {
                XElement root;
                if (DontShowAgainOptions != null)
                {
                    root = new XElement("root",
                                                DontShowAgainOptions.Select(k => new XElement("Option", new XAttribute("Key", k.Key), new XAttribute("Value", k.Value))));
                }
                else
                {
                    root = new XElement("root");
                }

                IFilePersistenceProvider filePersistenceProviderInst = ImportService.GetExportValue<IFilePersistenceProvider>();
                filePersistenceProviderInst.Write(GetDontShowAgainPersistencePath(), root.ToString());
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
            {
                // If persisting the data fails then do nothing
                // TODO when loggin support is added to the studio write to the log here
            }
        }

        public static Tuple<bool, MessageBoxResult> GetDontShowAgainOption(string dontShowAgainKey)
        {
            // If no key then return false result
            if (string.IsNullOrEmpty(dontShowAgainKey))
            {
                return new Tuple<bool, MessageBoxResult>(false, MessageBoxResult.None);
            }

            // Load if null
            if (DontShowAgainOptions == null)
            {
                LoadDontShowAgainOptions();
            }

            // Check if there an option for the key
            Tuple<bool, MessageBoxResult> result;
            MessageBoxResult tmp;
            if (DontShowAgainOptions != null && DontShowAgainOptions.TryGetValue(dontShowAgainKey, out tmp))
            {
                result = new Tuple<bool, MessageBoxResult>(true, tmp);
            }
            else
            {
                result = new Tuple<bool, MessageBoxResult>(false, MessageBoxResult.None);
            }

            return result;
        }

        public static void SetDontShowAgainOption(string dontShowAgainKey, MessageBoxResult result)
        {
            // If no key do nothing
            if (string.IsNullOrEmpty(dontShowAgainKey))
            {
                return;
            }

            // Load if null
            if (DontShowAgainOptions == null)
            {
                LoadDontShowAgainOptions();
            }

            // Add/Update option
            if(DontShowAgainOptions != null)
            {
                DontShowAgainOptions[dontShowAgainKey] = result;
            }

            // Save
            SaveDontShowAgainOptions();
        }

        public static void ResetDontShowAgainOption(string dontShowAgainKey)
        {
            // If no key do nothing
            if (string.IsNullOrEmpty(dontShowAgainKey))
            {
                return;
            }

            // Load if null
            if (DontShowAgainOptions == null)
            {
                LoadDontShowAgainOptions();
            }

            // Remove option
            if(DontShowAgainOptions != null)
            {
                DontShowAgainOptions.Remove(dontShowAgainKey);
            }

            // Save
            SaveDontShowAgainOptions();
        }

        public static void ResetAllDontShowAgainOptions()
        {
            // Clear all options
            if(DontShowAgainOptions != null)
            {
                DontShowAgainOptions.Clear();
            }

            // Save
            SaveDontShowAgainOptions();

            // Reset dictionary
            DontShowAgainOptions = null;
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, string dontShowAgainKey)
        {
            // Claculate the appropriate default result
            MessageBoxResult defaultResult = MessageBoxResult.OK;
            if (button == MessageBoxButton.OK || button == MessageBoxButton.OKCancel)
            {
                defaultResult = MessageBoxResult.OK;
            }
            else if (button == MessageBoxButton.YesNo || button == MessageBoxButton.YesNoCancel)
            {
                defaultResult = MessageBoxResult.Yes;
            }

            return Show(messageBoxText, caption, button, icon, defaultResult, dontShowAgainKey);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            // Claculate the appropriate default result
            MessageBoxResult defaultResult = MessageBoxResult.OK;
            if (button == MessageBoxButton.OK || button == MessageBoxButton.OKCancel)
            {
                defaultResult = MessageBoxResult.OK;
            }
            else if (button == MessageBoxButton.YesNo || button == MessageBoxButton.YesNoCancel)
            {
                defaultResult = MessageBoxResult.Yes;
            }

            return Show(messageBoxText, caption, button, icon, defaultResult, null);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon,
                                            MessageBoxResult defaultResult, string dontShowAgainKey)
        {
            // Check for don't show again option
            Tuple<bool, MessageBoxResult> dontShowAgainOption = GetDontShowAgainOption(dontShowAgainKey);
            if (dontShowAgainOption.Item1)
            {
                // Return the remembered option
                return dontShowAgainOption.Item2;
            }

            // Construct and show the message box
            Dev2MessageBoxViewModel dev2MessageBoxViewModel = new Dev2MessageBoxViewModel(messageBoxText, caption, button, icon, defaultResult, dontShowAgainKey);
            IWindowManager windowManager = ImportService.GetExportValue<IWindowManager>();

            if (windowManager == null)
            {
                throw new Exception("Unable to locate an instance of the window manager.");
            }

            windowManager.ShowDialog(dev2MessageBoxViewModel);

            // Save don't so again option
            if (dev2MessageBoxViewModel.DontShowAgain)
            {
                SetDontShowAgainOption(dontShowAgainKey, dev2MessageBoxViewModel.Result);
            }

            return dev2MessageBoxViewModel.Result;
        }

        ///<summary>
        ///Creates a YesNoCancel if 3 strings are passed, YesNo if 2 are passed and an Ok dialog if one is passed.
        ///</summary>
        public static MessageBoxResult ShowWithCustomButtons(string messageBoxText, string caption, List<string> buttons, MessageBoxImage icon,
                                            MessageBoxResult defaultResult, string dontShowAgainKey)
        {
            // Check for don't show again option
            Tuple<bool, MessageBoxResult> dontShowAgainOption = GetDontShowAgainOption(dontShowAgainKey);
            if (dontShowAgainOption.Item1)
            {
                // Return the remembered option
                return dontShowAgainOption.Item2;
            }

            // Show the message box
            Dev2MessageBoxWithCustomButtons msg = null;
            switch(buttons.Count)
            {
                case 0:
                    msg = new Dev2MessageBoxWithCustomButtons(messageBoxText, caption, icon);
                    break;
                case 1:
                    msg = new Dev2MessageBoxWithCustomButtons(messageBoxText, caption, MessageBoxButton.OK, icon)
                    {
                        OkButtonText = buttons[0]
                    };
                    break;
                case 2:
                    msg = new Dev2MessageBoxWithCustomButtons(messageBoxText, caption, MessageBoxButton.YesNo, icon)
                    {
                        YesButtonText = buttons[0],
                        NoButtonText = buttons[1]
                    };
                    break;
                case 3:
                    msg = new Dev2MessageBoxWithCustomButtons(messageBoxText, caption, MessageBoxButton.YesNoCancel, icon)
                    {
                        YesButtonText = buttons[0],
                        NoButtonText = buttons[1],
                        CancelButtonText = buttons[2]
                    };
                    break;
            }

            if(msg != null)
            {
                msg.ShowDialog();

                return msg.Result;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        #endregion Static Methods
    }
}