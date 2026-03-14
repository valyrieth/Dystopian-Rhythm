/// <summary>
/// RhythmKeyBot: A simple rhythm game bot that detects specific colors under the mouse cursor and simulates key/mouse input accordingly.
/// </summary>
/// <remarks>
/// - Uses Win32 API to read pixel color, send mouse/keyboard input, and detect toggle key state.
/// - Toggles bot on/off with F8 key.
/// - Holds left mouse button ("Z" action) when blue or orange color is detected.
/// - Holds "X" key (via scan code) when green or purple color is detected.
/// - Continuously updates status in the console with a spinner and debug info.
/// </remarks>
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

class Program
{
    // Win32 pixel reading
    [DllImport("user32.dll")]
    static extern IntPtr GetDC(IntPtr hwnd);
    [DllImport("gdi32.dll")]
    static extern uint GetPixel(IntPtr hdc, int x, int y);

    // Toggle key
    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);

    // SendInput for mouse and keyboard
    [DllImport("user32.dll")]
    static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [StructLayout(LayoutKind.Sequential)]
    struct INPUT { public int type; public InputUnion mkhi; }

    [StructLayout(LayoutKind.Explicit)]
    struct InputUnion { [FieldOffset(0)] public KEYBDINPUT ki; [FieldOffset(0)] public MOUSEINPUT mi; }

    [StructLayout(LayoutKind.Sequential)]
    struct KEYBDINPUT { public ushort wVk; public ushort wScan; public uint dwFlags; public uint time; public IntPtr dwExtraInfo; }

    [StructLayout(LayoutKind.Sequential)]
    struct MOUSEINPUT { public int dx; public int dy; public uint mouseData; public uint dwFlags; public uint time; public IntPtr dwExtraInfo; }

    const int INPUT_KEYBOARD = 1;
    const int INPUT_MOUSE = 0;
    const uint KEYEVENTF_SCANCODE = 0x0008;
    const uint KEYEVENTF_KEYUP = 0x0002;
    const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    const uint MOUSEEVENTF_LEFTUP = 0x0004;

    const int TOGGLE_KEY = 0x77; // F8

    static bool enabled = true;
    static bool holdingZ = false;
    static bool holdingX = false;

    static void MouseLeftDown() => SendMouse(MOUSEEVENTF_LEFTDOWN);
    static void MouseLeftUp() => SendMouse(MOUSEEVENTF_LEFTUP);
    static void ScanKeyDown(ushort scanCode) => SendScan(scanCode, false);
    static void ScanKeyUp(ushort scanCode) => SendScan(scanCode, true);

    static void SendMouse(uint flags)
    {
        INPUT[] input = new INPUT[1];
        input[0].type = INPUT_MOUSE;
        input[0].mkhi.mi.dwFlags = flags;
        SendInput(1, input, Marshal.SizeOf(typeof(INPUT)));
    }

    static void SendScan(ushort scan, bool up)
    {
        INPUT[] input = new INPUT[1];
        input[0].type = INPUT_KEYBOARD;
        input[0].mkhi.ki.wScan = scan;
        input[0].mkhi.ki.dwFlags = KEYEVENTF_SCANCODE | (up ? KEYEVENTF_KEYUP : 0);
        SendInput(1, input, Marshal.SizeOf(typeof(INPUT)));
    }

    static bool Match(Color c, Color target) => c.R == target.R && c.G == target.G && c.B == target.B;

    static void ReleaseKeys()
    {
        if (holdingZ) { MouseLeftUp(); holdingZ = false; }
        if (holdingX) { ScanKeyUp(0x2D); holdingX = false; } // X scan code
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Color blue = ColorTranslator.FromHtml("#2783F2");
        Color orange = ColorTranslator.FromHtml("#FF6A29");
        Color green = ColorTranslator.FromHtml("#009144");
        Color purple = ColorTranslator.FromHtml("#502EBC");

        IntPtr dc = GetDC(IntPtr.Zero);
        string[] spin = { "⡿","⣟","⣯","⣷","⣾","⣽","⣻","⢿" };
        int spinner = 0;
        bool togglePressed = false;

        // Optional: fixed pixel coordinate for rhythm lane
        // Point pos = new Point(960, 540);

        while (true)
        {
            // F8 toggle
            bool keyDown = (GetAsyncKeyState(TOGGLE_KEY) & 0x8000) != 0;
            if (keyDown && !togglePressed)
            {
                enabled = !enabled;
                if (!enabled) ReleaseKeys();
                togglePressed = true;
            }
            if (!keyDown) togglePressed = false;

            var pos = Cursor.Position; // or use fixed pos
            uint pixel = GetPixel(dc, pos.X, pos.Y);
            Color c = Color.FromArgb((int)(pixel & 0xFF), (int)((pixel >> 8) & 0xFF), (int)((pixel >> 16) & 0xFF));

            bool shouldHoldZ = Match(c, blue) || Match(c, orange);
            bool shouldHoldX = Match(c, green) || Match(c, purple);

            if (enabled)
            {
                // Left click = Z
                if (shouldHoldZ && !holdingZ) { MouseLeftDown(); holdingZ = true; }
                else if (!shouldHoldZ && holdingZ) { MouseLeftUp(); holdingZ = false; }

                // Scan code = X
                if (shouldHoldX && !holdingX) { ScanKeyDown(0x2D); holdingX = true; }
                else if (!shouldHoldX && holdingX) { ScanKeyUp(0x2D); holdingX = false; }
            }

            // Spinner & debug output
            spinner = (spinner + 1) % spin.Length;
            Console.SetCursorPosition(0, 0);
            Console.Write(
                $"Bot:{(enabled ? "ON " : "OFF")} {spin[spinner]}  " +
                $"RGB({c.R,3},{c.G,3},{c.B,3}) #{c.R:X2}{c.G:X2}{c.B:X2}  " +
                $"Z:{(holdingZ ? "DOWN " : "UP   ")} " +
                $"X:{(holdingX ? "DOWN " : "UP   ")}     "
            );

            Thread.Sleep(1);
        }
    }
}