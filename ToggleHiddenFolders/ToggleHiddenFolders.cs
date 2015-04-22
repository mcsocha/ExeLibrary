namespace ToggleHiddenFolders
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml.Linq;
    using Microsoft.Win32;
    using System.Runtime.InteropServices;


    public class ToggleHiddenFolders
    {
        private static bool _testing = false;

        [Flags]
        public enum HChangeNotifyEventID
        {
            SHCNE_ASSOCCHANGED = 0x08000000,
        }

        [Flags]
        public enum HChangeNotifyFlags
        {
            SHCNF_IDLIST = 0x0000
        }

        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_ABORTIFHUNG = 0x2,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("shell32.dll")]
        public static extern void SHChangeNotify(HChangeNotifyEventID wEventId, HChangeNotifyFlags uFlags, IntPtr dwItem1, IntPtr dwItem2);

        static void Main(string[] args)
        {
            IntPtr HWND_BROADCAST = new IntPtr(0xffff);
            UInt32 WM_SETTINGCHANGE = 0x1A;
            IntPtr NULL = IntPtr.Zero;

            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced");
            if (key != null)
            {
                if (key.GetValue("Hidden").ToString() == "1")
                {
                    key.SetValue("Hidden", 2);
                    Console.WriteLine("The files should now be hidden.");
                }
                else
                {
                    key.SetValue("Hidden", 1);
                    Console.WriteLine("The files should now be visible.");
                }
            }

            IntPtr refresh = new IntPtr(0x7103); //Refresh
            uint WM_COMMAND = 0x0111;
            IntPtr output= SendMessage(HWND_BROADCAST, WM_COMMAND, refresh, IntPtr.Zero);
            SHChangeNotify(HChangeNotifyEventID.SHCNE_ASSOCCHANGED, HChangeNotifyFlags.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
        }

        private static void PromptAnyKey()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    
        public static void Main(string[] args, bool testing)
        {
            _testing = testing;
            Main(args);
        }
    }
}