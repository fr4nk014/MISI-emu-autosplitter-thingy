using System;
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



        static void Main(string[] args)
        {
            mem = new();
            


            while (true)
            {
                if(!mem.OpenProcess("pcsx2 VRR.exe"))
                {
                    Console.WriteLine("no emu");
                    continue;
                }
                if(shouldpause())
                {
                    fake_keypress(p_key);
                }
                if(shouldresume())
                {
                    fake_keypress(p_key);
                }
                Thread.Sleep(50);
            }
        }


        static bool awaitingpause = true;
        static bool shouldpause()
        {
            if(awaitingpause)
            {
                int gamestate = mem.ReadByte("0x203FF868");
                //Console.WriteLine($"Gamestate value: {gamestate}");
                if (gamestate == 0xA || gamestate == 0x1A)
                {
                    awaitingpause = false;
                    return true;
                }
            }
            return false;
        }
        static bool shouldresume()
        {
            if (!awaitingpause)
            {
                int gamestate = mem.ReadByte("0x203FF868");
                //Console.WriteLine($"Gamestate value: {gamestate}");
                if (gamestate == 0x2)
                {
                    awaitingpause = true;
                    return true;
                }
            }
            return false;
        }

        static void fake_keypress(byte keycode)
        {
            //Console.WriteLine("Pressed P successfully");
            keybd_event(keycode, 0, key_down, IntPtr.Zero);
            Thread.Sleep(100);
            keybd_event(keycode, 0, key_up, IntPtr.Zero);
        }
    }
}
