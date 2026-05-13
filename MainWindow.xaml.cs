using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace OFGB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        [LibraryImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute")]
        internal static partial int DwmSetWindowAttribute(IntPtr hwnd, int attr, [In] int[] attrValue, int attrSize);

        const string cur_ver = "Software\\Microsoft\\Windows\\CurrentVersion\\";

        // Prevents registry writes while checkboxes are being set during initialization
        private bool _initializing = true;

        public MainWindow()
        {
            InitializeComponent();
            InitializeKeys();
            _initializing = false;

            DwmSetWindowAttribute(new WindowInteropHelper(Application.Current.MainWindow).EnsureHandle(), 33, [2], sizeof(int));
        }

        private void InitializeKeys()
        {
            // Sync provider notifications in File Explorer
            cb1.IsChecked = ReadKey(cur_ver + "Explorer\\Advanced", "ShowSyncProviderNotifications");

            // Get fun facts, tips, tricks, and more on your lock screen
            cb2.IsChecked = ReadKey(cur_ver + "ContentDeliveryManager", "RotatingLockScreenOverlayEnabled")
                         && ReadKey(cur_ver + "ContentDeliveryManager", "SubscribedContent-338387Enabled");

            // Show suggested content in Settings app
            cb3.IsChecked = ReadKey(cur_ver + "ContentDeliveryManager", "SubscribedContent-338393Enabled")
                         && ReadKey(cur_ver + "ContentDeliveryManager", "SubscribedContent-353694Enabled")
                         && ReadKey(cur_ver + "ContentDeliveryManager", "SubscribedContent-353696Enabled");

            // Get tips and suggestions when using Windows
            cb4.IsChecked = ReadKey(cur_ver + "ContentDeliveryManager", "SubscribedContent-338389Enabled");

            // Suggest ways to get the most out of Windows and finish setting up this device
            cb5.IsChecked = ReadKey(cur_ver + "UserProfileEngagement", "ScoobeSystemSettingEnabled");

            // Show me the Windows welcome experience after updates
            cb6.IsChecked = ReadKey(cur_ver + "ContentDeliveryManager", "SubscribedContent-310093Enabled");

            // Let apps show me personalized ads by using my advertising ID
            cb7.IsChecked = ReadKey(cur_ver + "AdvertisingInfo", "Enabled");

            // Tailored experiences
            cb8.IsChecked = ReadKey(cur_ver + "Privacy", "TailoredExperiencesWithDiagnosticDataEnabled");

            // "Show recommendations for tips, shortcuts, new apps, and more" on Start
            cb9.IsChecked = ReadKey(cur_ver + "Explorer\\Advanced", "Start_IrisRecommendations");

            // "Turn off notifications from <app>?"
            cb10.IsChecked = ReadKey(cur_ver + "Notifications\\Settings\\Windows.ActionCenter.SmartOptOut", "Enabled");

            // Show Bing Results in Windows Search
            // DisableSearchBoxSuggestions=1 means Bing is OFF, so checkbox (Bing ON) = inverted
            bool bingDisabled = ReadKey("Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions");
            bool bingEnabled  = ReadKey(cur_ver + "Search", "BingSearchEnabled");
            cb11.IsChecked = !bingDisabled && bingEnabled;

            // Edge desktop search widget bar
            cb12.IsChecked = ReadKey("Software\\Policies\\Microsoft\\Edge", "WebWidgetAllowed");
        }

        /// <summary>
        /// Reads a DWORD registry value under HKCU. Returns false if the key or value does not exist.
        /// Does NOT create or write anything.
        /// </summary>
        private static bool ReadKey(string subKey, string valueName)
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(subKey, writable: false);
            if (key is null)
                return false;

            object? val = key.GetValue(valueName);
            if (val is null)
                return false;

            return Convert.ToBoolean(val);
        }

        private static void ToggleOptions(string checkboxName, bool enable)
        {
            switch (checkboxName)
            {
                case "cb1":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "Explorer\\Advanced", "ShowSyncProviderNotifications", Convert.ToInt32(enable));
                    break;
                case "cb2":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "ContentDeliveryManager", "RotatingLockScreenOverlayEnabled", Convert.ToInt32(enable));
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "ContentDeliveryManager", "SubscribedContent-338387Enabled", Convert.ToInt32(enable));
                    break;
                case "cb3":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "ContentDeliveryManager", "SubscribedContent-338393Enabled", Convert.ToInt32(enable));
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "ContentDeliveryManager", "SubscribedContent-353694Enabled", Convert.ToInt32(enable));
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "ContentDeliveryManager", "SubscribedContent-353696Enabled", Convert.ToInt32(enable));
                    break;
                case "cb4":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "ContentDeliveryManager", "SubscribedContent-338389Enabled", Convert.ToInt32(enable));
                    break;
                case "cb5":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "UserProfileEngagement", "ScoobeSystemSettingEnabled", Convert.ToInt32(enable));
                    break;
                case "cb6":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "ContentDeliveryManager", "SubscribedContent-310093Enabled", Convert.ToInt32(enable));
                    break;
                case "cb7":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "AdvertisingInfo", "Enabled", Convert.ToInt32(enable));
                    break;
                case "cb8":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", Convert.ToInt32(enable));
                    break;
                case "cb9":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "Explorer\\Advanced", "Start_IrisRecommendations", Convert.ToInt32(enable));
                    break;
                case "cb10":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "Notifications\\Settings\\Windows.ActionCenter.SmartOptOut", "Enabled", Convert.ToInt32(enable));
                    break;
                case "cb11":
                    // Inverted: checkbox checked = Bing ON = DisableSearchBoxSuggestions 0, BingSearchEnabled 1
                    Registry.SetValue("HKEY_CURRENT_USER\\Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions", Convert.ToInt32(!enable));
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "Search", "BingSearchEnabled", Convert.ToInt32(enable));
                    break;
                case "cb12":
                    Registry.SetValue("HKEY_CURRENT_USER\\Software\\Policies\\Microsoft\\Edge", "WebWidgetAllowed", Convert.ToInt32(enable));
                    break;
            }
        }

        private void Checked(object sender, RoutedEventArgs e)
        {
            if (_initializing) return;
            ToggleOptions(((CheckBox)sender).Name, true);
        }

        private void Unchecked(object sender, RoutedEventArgs e)
        {
            if (_initializing) return;
            ToggleOptions(((CheckBox)sender).Name, false);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
