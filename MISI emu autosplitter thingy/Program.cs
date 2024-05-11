using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Memory;

namespace MISI_emu_autosplitter_thingy
{
    class Program
    {
        // some shit idk thanks google
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

        static Mem mem;
        

        const byte p_key = 0x50;


        const uint key_down = 0x0001;
        const uint key_up = 0x0002;

        static UInt64 BaseAddress = 0x0;
        static string BaseString = "0x0";
        static bool ScanningBase = false;


        public static byte?[] CheckBytesEE { get; private set; } = new byte?[]
        { 0x01, 0x80, 0x1A, 0x3C, null, null, 0x59, 0xFF, 0x00, 0x68, 0x19, 0x40,
        0x01, 0x80, 0x1A, 0x3C, 0x7C, 0x00, 0x39, 0x33, 0x21, 0xD0, 0x59, 0x03, null,
        null, null, 0x8F, 0x01, 0x80, 0x19, 0x3C, 0x08, 0x00, 0x40, 0x03, null, null, 0x39, 0xDF };

        static void Main(string[] args)
        {
            mem = new();

            

            while (true)
            {
                LogSomething("Looking for emu");

                string proc_name = GetEmuName();
                while (proc_name == null)
                {
                    proc_name = GetEmuName();
                    Thread.Sleep(500);
                }
                if (!mem.OpenProcess(proc_name))
                {
                    LogSomething("No emu");
                    Thread.Sleep(100);
                    continue;
                }

                LogSomething("Emu found");
                LogSomething("Looking for EE base");

                bool lost_while_ee_scan = false;
                while (!GoodBase)
                {
                    if (!ScanningBase)
                    {
                        GetBaseAddress();
                        if (BaseAddress > 0)
                        {
                            break;
                        }
                    }
                    if (!mem.OpenProcess(proc_name))
                    {
                        LogSomething("Lost emu while scanning for base", is_bad: true);
                        lost_while_ee_scan = true;
                        break;
                    }
                    Thread.Sleep(50);
                }

                if (lost_while_ee_scan) continue;

                LogSomething("Found memory base: " + BaseAddress.ToString("X"));

                LogSomething("Everything should work");

                bool working = true;
                while(working)
                {
                    if (shouldpause())
                    {
                        fake_keypress(p_key);
                    }
                    if (shouldresume())
                    {
                        fake_keypress(p_key);
                    }
                    
                    Thread.Sleep(50);

                    if(!mem.OpenProcess(proc_name))
                    {
                        LogSomething("Lost emu", is_bad: true);
                        break;
                    }
                }
                
            }
        }

        static void LogSomething(string message, bool is_bad = false)
        {
            string timestamp = System.DateTime.Now.ToString("HH:mm:ss");

            Console.ForegroundColor = (is_bad ? ConsoleColor.Red : ConsoleColor.White);
            Console.WriteLine($"{timestamp}: {message}");
        }

        static string GetEmuName()
        {
            Process[] processes = Process.GetProcesses();
            foreach(Process p in processes)
            {
                string pname = p.ProcessName;
                if(pname.StartsWith("pcsx2"))
                {
                    return pname;
                }
            }
            return null;
        }

        static bool GoodBase = false;
        static void GetBaseAddress()
        {
            
            if (!GoodBase)
            {
                ScanningBase = true;

                List<byte?> checkBytes = new List<byte?>();

                for (int i = 0; i < CheckBytesEE.Length; i++)
                {
                    checkBytes.Add(CheckBytesEE[i]);
                }

                UInt64 start = 0;

                while (start < 0x800000000000)
                {
                    bool good = true;

                    for (int i = 0; i < checkBytes.Count; i += 4)
                    {
                        if (checkBytes[i] == null) continue;

                        byte[] helpByteArr = new byte[]
                        {
                            (byte)checkBytes[i],
                            (byte)checkBytes[i+1],
                            (byte)checkBytes[i+2],
                            (byte)checkBytes[i+3]
                        };

                        int val = BitConverter.ToInt32(helpByteArr, 0);

                        if (mem.ReadUIntPtr((UIntPtr)start + i) != val)
                        {
                            good = false;
                            break;
                        }
                    }


                    if (!good)
                    {
                        start += 0x10000000;
                        continue;
                    }

                    if (start == BaseAddress)
                    {
                        ScanningBase = false;
                        GoodBase = true;
                        return;
                    }

                    BaseAddress = (UInt64)start;
                    BaseString = "0x" + BaseAddress.ToString("X");
                    Debug.WriteLine(BaseString);

                    ScanningBase = false;
                    GoodBase = true;
                    return;
                }
                Thread.Sleep(1000);
                ScanningBase = false;
                GoodBase = false;
                return;
            }
        }

        static bool awaitingpause = true;
        static bool shouldpause()
        {
            if(awaitingpause)
            {
                int gamestate = mem.ReadByte((BaseAddress+0x3FF868).ToString("X"));
                //LogSomething($"Gamestate value: {gamestate}");
                if (gamestate == 0xA || gamestate == 0x1A)
                {
                    awaitingpause = false;
                    LogSomething("Should pause");
                    return true;
                }
            }
            return false;
        }
        static bool shouldresume()
        {
            if (!awaitingpause)
            {
                int gamestate = mem.ReadByte((BaseAddress + 0x3FF868).ToString("X"));
                //LogSomething($"Gamestate value: {gamestate}");
                if (gamestate == 0x2)
                {
                    awaitingpause = true;
                    LogSomething("Should resume");
                    return true;
                }
            }
            return false;
        }

        static void fake_keypress(byte keycode)
        {
            keybd_event(keycode, 0, key_down, IntPtr.Zero);
            Thread.Sleep(100);
            keybd_event(keycode, 0, key_up, IntPtr.Zero);
            LogSomething("Pressed P");
        }
    }
}
