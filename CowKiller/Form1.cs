// Copyright 2009 Strom
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace CowKiller {
    public partial class Form1 : Form {
        private static int scenario = 0;
        private static int engineState = 0;
        private static int expLimit = 0;
        private static int expEarned = 0;
        private static int encountersSkipped = 0;
        private static int cowsKilled = 0;
        private static DateTime startTime = DateTime.UtcNow;
        private static DateTime firstaidTime = DateTime.UtcNow;
        private static TimeSpan downTime = TimeSpan.FromSeconds(0.0);

        private static Queue<string> logQueue = new Queue<string>();

        private static int fo_hwnd;
        private static int fo_width;
        private static int fo_height;
        private static int fo_x;
        private static int fo_y;

        private static Bitmap mapArrow = new Bitmap(@"data\map_arrow.bmp");
        private static Bitmap mapMenu = new Bitmap(@"data\map_menu.bmp");
        private static Bitmap mapEncounterCancel = new Bitmap(@"data\map_encounter_cancel.bmp");
        private static Bitmap mapSquareNavarro = new Bitmap(@"data\map_square_navarro.bmp");
        private static Bitmap mapQuarterSquareNavarro = new Bitmap(@"data\map_quartersquare_navarro.bmp");
        private static Bitmap mapHalfSquareKlamath = new Bitmap(@"data\map_halfsquare_klamath.bmp");
        private static Bitmap mapSquareReplication3 = new Bitmap(@"data\map_square_replication3.bmp");
        private static Bitmap mapSquareMariposa = new Bitmap(@"data\map_square_mariposa.bmp");
        private static Bitmap loginEnter2238 = new Bitmap(@"data\login_enter_2238.bmp");

        #region SUBCLASS
        #region API

        [DllImport("kernel32.dll", EntryPoint = "GetLastError", SetLastError = false, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetLastError();

        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        private class User32
        {
            public enum WindowState
            {
                SW_SHOWNORMAL = 1,
                SW_SHOWMINIMIZED = 2,
                SW_SHOWMAXIMIZED = 3
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct WINDOWPLACEMENT
            {
                public int length;
                public int flags;
                public int showCmd;
                public System.Drawing.Point ptMinPosition;
                public System.Drawing.Point ptMaxPosition;
                public System.Drawing.Rectangle rcNormalPosition;
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern int GetForegroundWindow();
            [DllImport("user32.dll")]
            public static extern int FindWindow(string lpClassName, string lpWindowName);
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
            [DllImport("user32.dll")]
            public static extern IntPtr GetClientRect(IntPtr hWnd, ref RECT rect);
            [DllImport("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);


            [DllImport("user32.dll")]
            public static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 dwData, IntPtr dwExtraInfo);

            public const int MOUSEEVENTF_LEFTDOWN = 0x02;
            public const int MOUSEEVENTF_LEFTUP = 0x04;
            public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
            public const int MOUSEEVENTF_RIGHTUP = 0x10;
        }
        #endregion
        #endregion

        public Form1() {
            InitializeComponent();
        }

        private void SendLeftClick() {
            User32.mouse_event(User32.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, new System.IntPtr());
            User32.mouse_event(User32.MOUSEEVENTF_LEFTUP, 0, 0, 0, new System.IntPtr());
        }

        private void SendRightClick() {
            User32.mouse_event(User32.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, new System.IntPtr());
            User32.mouse_event(User32.MOUSEEVENTF_RIGHTUP, 0, 0, 0, new System.IntPtr());
        }

        private void DragRightClick(int x1, int y1, int x2, int y2, int waitDrag) {
            Cursor.Position = new Point(x1, y1);
            Thread.Sleep(10); // Wait for the cursor to move

            User32.mouse_event(User32.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, new System.IntPtr());

            Thread.Sleep(50);

            Cursor.Position = new Point(x2, y2);

            Thread.Sleep(10);

            Thread.Sleep(waitDrag);

            User32.mouse_event(User32.MOUSEEVENTF_RIGHTUP, 0, 0, 0, new System.IntPtr());

            Thread.Sleep(50);
        }

        private void DelayedLeftClick(int x, int y, int delayMS, bool doubleClick) {
            Cursor.Position = new Point(x, y);
            Thread.Sleep(10); // Wait for the cursor to move
            SendLeftClick();
            if (doubleClick) {
                Thread.Sleep(10);
                SendLeftClick();
            }

            if (delayMS > 0) {
                Thread.Sleep(delayMS); // Wait for the click to do something
            }
        }

        private void DelayedRightClick(int x, int y, int delayMS, bool doubleClick) {
            Cursor.Position = new Point(x, y);
            Thread.Sleep(10); // Wait for the cursor to move
            SendRightClick();
            if (doubleClick) {
                Thread.Sleep(10);
                SendRightClick();
            }

            if (delayMS > 0) {
                Thread.Sleep(delayMS); // Wait for the click to do something
            }
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Keys k = Keys.NumPad4;
            HotKey.RegisterHotKey(this, k);
        }

        // CF Note: The WndProc is not present in the Compact Framework (as of vers. 3.5)! please derive from the MessageWindow class in order to handle WM_HOTKEY
        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);

            if (m.Msg == HotKey.WM_HOTKEY) {
                //DoEyeShot();
                DoDupeClick();
            }
        }

        /// <summary>
        /// Take screenshot
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns>Image captured</returns>
        public Image ImageCapture(int Width, int Height, int X, int Y)
        {
            //this.Opacity = 0;
            Rectangle ScreenBounds = new Rectangle(X, Y, Width, Height);
            Bitmap Screenshot = new Bitmap(ScreenBounds.Width, ScreenBounds.Height, PixelFormat.Format32bppArgb);
            Graphics ScreenGraph = Graphics.FromImage(Screenshot);
            ScreenGraph.CopyFromScreen(ScreenBounds.X, ScreenBounds.Y, 0, 0, ScreenBounds.Size, CopyPixelOperation.SourceCopy);
            Image Img = (Image)Screenshot;
            //this.Opacity = 100;
            return Img;
        }

        /// <summary>
        /// Capture window image
        /// </summary>
        public void ScreenshotWindow()
        {
            //int hWnd = User32.GetForegroundWindow();
            int hWnd = User32.FindWindow(null, "Fallout Online");
            if (hWnd == 0) {
                int error = GetLastError();

                AddLog("ERROR finding window: " + error);
                return;
            }
            User32.RECT bounds = new User32.RECT();
            User32.GetClientRect((IntPtr)hWnd, ref bounds);
            int width = bounds.right - bounds.left;
            int height = bounds.bottom - bounds.top;
            User32.WINDOWPLACEMENT wp = new User32.WINDOWPLACEMENT();
            User32.GetWindowPlacement((IntPtr)hWnd, ref wp);
            int x;
            int y;

            if (wp.showCmd == (int)User32.WindowState.SW_SHOWMAXIMIZED)
            {
                x = wp.ptMaxPosition.X;
                y = wp.ptMaxPosition.Y;
            }
            else
            {
                x = wp.rcNormalPosition.X;
                y = wp.rcNormalPosition.Y;
            }

            if ((x + width) > Screen.PrimaryScreen.WorkingArea.Width)
                width = width - ((x + width) - Screen.PrimaryScreen.WorkingArea.Width);

            if ((y + height) > Screen.PrimaryScreen.WorkingArea.Height)
                height = height - ((y + height) - Screen.PrimaryScreen.WorkingArea.Height);

            fo_width = width;
            fo_height = height;
            fo_x = x + 2; // h4x
            fo_y = y + 20; // h4x

            fo_hwnd = hWnd;
        }

        private bool FuzzyComparePixels(Color c1, Color c2, int threshold) {
            int fuz = threshold / 2;
            if (c1.R + fuz >= c2.R && c1.R - fuz <= c2.R &&
                c1.G + fuz >= c2.G && c1.G - fuz <= c2.G &&
                c1.B + fuz >= c2.B && c1.B - fuz <= c2.B) {
                return true;
            }

            return false;
        }

        private bool FindArea(Bitmap needle, Bitmap haystack, out Point location) {
            location = new Point(0, 0);

            if (needle.Width > haystack.Width || needle.Height > haystack.Height) {
                return false;
            }

            int maxWidth = haystack.Width - needle.Width;
            int maxHeight = haystack.Height - needle.Height;

            var firstNeedlePixel = needle.GetPixel(0, 0);

            for (int y = 0; y < maxHeight; y++) {
                for (int x = 0; x < maxWidth; x++) {
                    if (FuzzyComparePixels(haystack.GetPixel(x, y), firstNeedlePixel, 10)) {
                        // A possible match, check it
                        int maxY2 = y + needle.Height;
                        int maxX2 = x + needle.Width;

                        bool error = false;
                        for (int y2 = y; y2 < maxY2; y2++) {
                            for (int x2 = x + 1; x2 < maxX2; x2++) {
                                if (!FuzzyComparePixels(haystack.GetPixel(x2, y2), needle.GetPixel((x2 - x), (y2 - y)), 10)) {
                                    error = true;
                                    break;
                                }
                            }

                            if (error) {
                                break;
                            }
                        }

                        if (!error) {
                            // Found needle!
                            location = new Point(x, y);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool CheckForCow(Bitmap haystack, out Point location) {
            location = new Point(0, 0);

            Color pixelColor;
            for (int y = 0; y < haystack.Height; y++) {
                for (int x = 0; x < haystack.Width; x++) {
                    pixelColor = haystack.GetPixel(x, y);
                    if (pixelColor.G == 0 && pixelColor.B == 0 && pixelColor.R >= 100) {
                        // But are there enough of these around these parts? (60x60)
                        int maxY2 = y + 60;
                        int maxX2 = x + 60;

                        // Is there a large enough group to check here?
                        if (maxY2 > haystack.Height || maxX2 > haystack.Width) {
                            continue;
                        }

                        int colorCount = 1;
                        for (int y2 = y; y2 < maxY2; y2++) {
                            for (int x2 = x + 1; x2 < maxX2; x2++) {
                                pixelColor = haystack.GetPixel(x2, y2);
                                if (pixelColor.G == 0 && pixelColor.B == 0 && pixelColor.R >= 100) {
                                    colorCount++;
                                }
                            }
                        }

                        // Found cow?
                        if (colorCount >= 100) {
                            location = new Point(x, y);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void ClearLog() {
            textLog.Text = "";
        }

        private void AddLog(string text) {
            lock (logQueue) {
                logQueue.Enqueue(text);
            }
        }

        private void FlushLog() {
            lock (logQueue) {
                while (logQueue.Count > 0) {
                    textLog.Text += logQueue.Dequeue() + "\r\n";
                    textLog.SelectionStart = textLog.Text.Length - 1;
                    textLog.ScrollToCaret();
                    Application.DoEvents();
                }
            }
        }

        private void timerLog_Tick(object sender, EventArgs e) {
            FlushLog();

            // Update stats

            lblTime.Text = "Elapsed: " + GetNiceTime(DateTime.UtcNow - startTime - downTime);

            if (downTime.TotalSeconds > 0.0) {
                lblTime.Text += " + Downtime: " + GetNiceTime(downTime);
            }

            if (expEarned > 0) {
                lblExp.Text = "Experience earned: " + expEarned;
                lblRate.Text = "Rate: " + Convert.ToInt32(Math.Round(Convert.ToDouble(expEarned) / (DateTime.UtcNow - startTime - downTime).TotalHours)).ToString() + " exp/h";
                lblETA.Text = "ETA: " + GetNiceTime(TimeSpan.FromHours((expLimit - expEarned) / (Convert.ToDouble(expEarned) / (DateTime.UtcNow - startTime - downTime).TotalHours)));
            }

            if (engineState == 0) {
                button1.Enabled = false;
                timerLog.Enabled = false;
                comboScenario.Enabled = true;
                txtExpLimit.ReadOnly = false;
                button2.Enabled = true;
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            button1.Enabled = false;
            StopEngine();
        }

        private void StopEngine() {
            if (engineState != 0) {
                engineState = 2;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            button2.Enabled = false;
            comboScenario.Enabled = false;
            txtExpLimit.ReadOnly = true;

            expLimit = Int32.Parse(txtExpLimit.Text);

            if (comboScenario.Text == "Cow") {
                scenario = 1;
            }
            else if (comboScenario.Text == "Outdoorsman - Navarro 2238") {
                scenario = 2;
            }
            else if (comboScenario.Text == "Outdoorsman - Klamath") {
                scenario = 3;
            }
            else if (comboScenario.Text == "Outdoorsman - Replication 3") {
                scenario = 4;
            }
            else if (comboScenario.Text == "Outdoorsman - Mariposa") {
                scenario = 5;
            }

            encountersSkipped = 0;
            cowsKilled = 0;
            startTime = DateTime.UtcNow;
            firstaidTime = DateTime.UtcNow;

            ClearLog();
            timerLog.Enabled = true;

            ScreenshotWindow();
            AddLog("Found: " + fo_width + " * " + fo_height);

            if (fo_width < 200 || fo_height < 200) {
                AddLog("Thumbnail window problem detected! Please hover your mouse over some non-fonline window on the bottom windows toolbar.");
                button2.Enabled = true;
                comboScenario.Enabled = true;
                txtExpLimit.ReadOnly = false;
                timerLog.Enabled = false;

                FlushLog();

                return;
            }
            else if (fo_width < 1000 || fo_height < 1000) {
                AddLog("Abnormally small window detected. Please double-check the FOnline resolution and window placement, but we are going to try to run anyway.");
            }

            AddLog("Setting foreground window..");
            User32.SetForegroundWindow((IntPtr)fo_hwnd);

            engineState = 1;
            Thread t = new Thread(new ThreadStart(MainLoop));
            t.Start();

            button1.Enabled = true;
        }

        private string GetNiceTime(TimeSpan ts) {
            int hours = Convert.ToInt32(Math.Floor(ts.TotalHours));
            int minutes = ts.Minutes;
            int seconds = ts.Seconds;

            string result = "";

            if (hours < 10) {
                result += "0";
            }
            result += hours.ToString() + ":";

            if (minutes < 10) {
                result += "0";
            }
            result += minutes.ToString() + ":";

            if (seconds < 10) {
                result += "0";
            }
            result += seconds.ToString();

            return result;
        }

        private void MainLoop() {
            if (scenario == 1) {
                CowLoop();
            }
            else if (scenario >= 2) {
                OutdoorsmanLoop();
            }
        }

        private void CowLoop() {
            bool waitForMap = true;

            bool result = false;
            Point location;
            Image img;

            while (true) {
                if (expEarned >= expLimit) {
                    StopEngine();
                }

                // Are we allowed to run?
                if (engineState == 2) {
                    // Nope, time to shutdown
                    engineState = 0;
                    return;
                }

                // Are we waiting for the world map?
                if (waitForMap) {
                    AddLog("Capturing image..");
                    img = ImageCapture(fo_width, fo_height, fo_x, fo_y);

                    // World map? Enter instance!
                    result = FindArea(mapArrow, (Bitmap)img, out location);

                    if (result) {
                        // Click to enter and wait for the instance to load
                        DelayedLeftClick(location.X + fo_x + 10, location.Y + fo_y + 5, 0, false);

                        AddLog("Waiting for the instance to load...");
                        Thread.Sleep(3000);

                        waitForMap = false;
                    }
                    else {
                        // We're not on the world map, so time to escape this instance
                        EscapeFromInstance(1);

                        // Wait some time before continueing on (time to run, load world map etc)
                        Thread.Sleep(5000);
                    }

                    continue;
                }

                // Check for cow instance...

                // Activate aiming
                AddLog("Activating aiming...");
                DelayedLeftClick(fo_x + 750, fo_y + 990, 100, false);

                // Take new screen
                AddLog("Capturing image..");
                img = ImageCapture(fo_width, fo_height, fo_x, fo_y);

                //AddLog("Saving image..");
                //img.Save("capture2.bmp", ImageFormat.Bmp);
                //AddLog("Saving done!");

                // Check for cow
                AddLog("Checking for cow..");
                result = CheckForCow((Bitmap)img, out location);

                if (result) {
                    // Walk infront of cow
                    AddLog("Walking to cow..");
                    DelayedRightClick(location.X + fo_x + 40, location.Y + fo_y + 60, 50, true);

                    DelayedLeftClick(location.X + fo_x + 40, location.Y + fo_y + 60, 50, true);

                    // Wait for the running
                    AddLog("Waiting for running animation..");
                    Thread.Sleep(2500);

                    // Activate aiming
                    AddLog("Activating aiming...");
                    DelayedLeftClick(fo_x + 750, fo_y + 990, 100, false);

                    int shotsDone = 0;
                    DateTime lastShot;
                    while (true) {
                        // Target the cow
                        AddLog("Targeting the cow..");
                        DelayedLeftClick(location.X + fo_x + 20, location.Y + fo_y + 30, 100, false);

                        // Choose eyes
                        AddLog("Choosing eyes..");
                        DelayedLeftClick(fo_x + 485, fo_y + 465, 0, false);
                        shotsDone++;
                        lastShot = DateTime.UtcNow;

                        // Wait for the shot and animations
                        AddLog("Waiting for the shot and animations..");
                        Thread.Sleep(1000);

                        if (shotsDone >= 2) {
                            // 2+ shots done, now wait for AP to recover
                            AddLog("Waiting for AP to recover..");
                            Thread.Sleep(3000);
                        }

                        // Take new screen
                        AddLog("Capturing image..");
                        img = ImageCapture(fo_width, fo_height, fo_x, fo_y);

                        // Check for cow
                        AddLog("Checking for cow..");
                        result = CheckForCow((Bitmap)img, out location);

                        if (!result) {
                            // No cow!

                            // Activate reload bar
                            AddLog("Activating reload bar...");
                            DelayedRightClick(fo_x + 750, fo_y + 990, 100, true); // doubleclick true for burst weapons

                            // Reload weapon
                            AddLog("Reloading weapon...");
                            DelayedLeftClick(fo_x + 750, fo_y + 990, 0, false);

                            // Wait for weapon to reload + AP recovery
                            AddLog("Waiting for weapon reload + AP recovery");
                            Thread.Sleep(2000);

                            // Activate aimed shot bar
                            AddLog("Activating aimed shot bar...");
                            DelayedRightClick(fo_x + 750, fo_y + 990, 100, false);

                            // Change mouse cursor back to normal
                            AddLog("Changing cursor back to normal..");
                            DelayedRightClick(fo_x + 100, fo_y + 100, 50, false);

                            // Go near the instance exit
                            GoNearInstanceExit();

                            // Wait battle timeout
                            TimeSpan battleTS = TimeSpan.FromSeconds(31.0) - (DateTime.UtcNow - lastShot);
                            AddLog("Waiting battle timeout (" + battleTS.TotalSeconds.ToString() + ") ..");
                            Thread.Sleep(battleTS); // 30s total timeout, but we already spent some time usefully (still 1s extra for safe margin)

                            // First aid available?
                            if ((DateTime.UtcNow - firstaidTime).TotalMinutes > 12.0) {
                                // Chose skilldex
                                AddLog("Choosing skilldex..");
                                DelayedLeftClick(fo_x + 890, fo_y + 945, 50, false);

                                // Activate first aid
                                AddLog("Activating first aid..");
                                DelayedLeftClick(fo_x + 1150, fo_y + 530, 50, false);

                                // Using first aid on self
                                AddLog("Using first aid on self..");
                                DelayedLeftClick(fo_x + 270, fo_y + 850, 0, false);

                                AddLog("Waiting for first aid animation..");
                                Thread.Sleep(1500);

                                // Update last use time
                                firstaidTime = DateTime.UtcNow;
                            }

                            // Chose skilldex
                            AddLog("Choosing skilldex..");
                            DelayedLeftClick(fo_x + 890, fo_y + 945, 50, false);

                            // Activate sneak
                            AddLog("Activating sneak..");
                            DelayedLeftClick(fo_x + 1150, fo_y + 390, 50, false);

                            // Wait for sneak to properly sink in *** AP recovery isn't neccessery, due to world map*** (+ 2 AP recovery)
                            //AddLog("Waiting for sneak to properly sink in..");
                            //Thread.Sleep(1000);

                            // Escape this instance
                            EscapeFromInstance(1);

                            // Wait for the running
                            Thread.Sleep(4000);

                            break;
                        }
                    }

                    cowsKilled++;
                    expEarned += 80;
                }
                else {
                    AddLog("Couldn't find cow!");

                    // Change mouse cursor back to normal
                    AddLog("Changing cursor back to normal..");
                    DelayedRightClick(fo_x + 100, fo_y + 100, 50, false);

                    // Escape this instance
                    EscapeFromInstance(1);

                    // Wait for the running
                    Thread.Sleep(13000);
                }

                // Mark it, that we need to wait for the map
                waitForMap = true;
            }
        }

        private void OutdoorsmanLoop() {
            bool result = false;
            Point location;
            Image img;

            Random r = new Random();
            Point newDestination = new Point(0, 0);
            Point locationCity = new Point(0, 0);

            double escapeTries = 0.0;

            while (true) {
                if (expEarned >= expLimit) {
                    StopEngine();
                }

                // Are we allowed to run?
                if (engineState == 2) {
                    // Nope, time to shutdown
                    engineState = 0;
                    return;
                }

                AddLog("Capturing image..");
                img = ImageCapture(fo_width, fo_height, fo_x, fo_y);

                // Are we on the login screen?
                result = FindArea(loginEnter2238, (Bitmap)img, out location);

                if (result) {
                    // Yeap, so lets try to enter - and also mark some downtime

                    AddLog("Login screen detected! Entering game..");
                    DelayedLeftClick(location.X + fo_x + 12, location.Y + fo_y + 12, 500, false);

                    AddLog("Sleeping for 1 minute..");
                    Thread.Sleep(TimeSpan.FromMinutes(1.0));
                    downTime = downTime.Add(TimeSpan.FromMinutes(1.0));

                    continue;
                }

                // Are we on the world map?
                result = FindArea(mapMenu, (Bitmap)img, out location);

                if (!result) {
                    // No, we are not - time to exit the instance

                    escapeTries++;

                    // Escape this instance
                    if (Math.Floor(escapeTries / 7.0) % 2 == 1) {
                        EscapeFromInstance(3);
                    }
                    else {
                        EscapeFromInstance(2);
                    }

                    // Wait for the running
                    Thread.Sleep(5000);

                    continue;
                }

                escapeTries = 0.0;

                // Do we have an encounter handled by outdoorsman?
                result = FindArea(mapEncounterCancel, (Bitmap)img, out location);

                if (result) {
                    // Yeap, we sure do - so press cancel

                    AddLog("Pressing cancel on encounter..");
                    DelayedLeftClick(location.X + fo_x - 10, location.Y + fo_y + 10, 500, false);

                    // Move cursor out of the way on the map
                    Cursor.Position = new Point(fo_x + 50, fo_y + 400);

                    encountersSkipped++;

                    if (scenario == 2) {
                        expEarned += 45;
                    }
                    else if (scenario == 3) {
                        expEarned += 10;
                    }
                    else if (scenario == 4) {
                        expEarned += 45;
                    }
                    else if (scenario == 5) {
                        expEarned += 75;
                    }

                    continue;
                }

                // Standing on world map? Time to move!
                result = FindArea(mapArrow, (Bitmap)img, out location);

                if (result) {
                    location.X += 11;
                    location.Y += 6;

                    // Find city location
                    bool foundCity = false;

                    if (scenario == 2) {
                        foundCity = FindArea(mapQuarterSquareNavarro, (Bitmap)img, out locationCity);
                    }
                    else if (scenario == 3) {
                        foundCity = FindArea(mapHalfSquareKlamath, (Bitmap)img, out locationCity);
                    }
                    else if (scenario == 4) {
                        foundCity = FindArea(mapSquareReplication3, (Bitmap)img, out locationCity);
                    }
                    else if (scenario == 5) {
                        foundCity = FindArea(mapSquareMariposa, (Bitmap)img, out locationCity);
                    }

                    if (!foundCity) {
                        // Something is wrong
                        AddLog("Could not find city!");

                        Thread.Sleep(5000);

                        continue;
                    }

                    // Do we need to move the map?
                    if (scenario == 5 && locationCity.Y >= 800) {
                        AddLog("Adjusting the world map..");
                        DragRightClick(locationCity.X + 25 + fo_x, locationCity.Y + 25 + fo_y, locationCity.X + 25 + fo_x, locationCity.Y - 100 + fo_y, 500);

                        // Move cursor out of the way on the map
                        Cursor.Position = new Point(fo_x + 50, fo_y + 400);

                        continue;
                    }

                    // Generate a new random spot to go to
                    while (true) {
                        if (scenario == 2) {
                            newDestination.X = locationCity.X + r.Next(50, 100);
                            newDestination.Y = locationCity.Y + r.Next(-70, 80);
                        }
                        else if (scenario == 3) {
                            newDestination.X = locationCity.X + r.Next(-150, 50);
                            newDestination.Y = locationCity.Y + r.Next(-50, -25);
                        }
                        else if (scenario == 4) {
                            newDestination.X = locationCity.X + r.Next(-200, 50);
                            newDestination.Y = locationCity.Y + r.Next(75, 200);
                        }
                        else if (scenario == 5) {
                            newDestination.X = locationCity.X + r.Next(65, 400);
                            //newDestination.Y = locationCity.Y + r.Next(50, 90);
                            newDestination.Y = locationCity.Y + 80;
                        }

                        // Check that we are gonna click outside the green triangle
                        if (Math.Abs(location.X - newDestination.X) > 15 ||
                            Math.Abs(location.Y - newDestination.Y) > 10) {
                            // Click in a safe distance from the green triangle

                            // But is the traveling distance enough for us?
                            if (Math.Sqrt(Math.Pow(Math.Abs(location.X - newDestination.X), 2.0) + Math.Pow(Math.Abs(location.Y - newDestination.Y), 2.0)) < 50.0) {
                                // Distance under 50 pixels, so generate a new destination point
                                continue;
                            }

                            // Everything is ok with this destination
                            break;
                        }

                        /*
                        if (newDestination.X >= 600) {
                            MessageBox.Show("WAT?");
                        }
                        */
                    }

                    // Click to move
                    AddLog("Moving on the world map..");
                    DelayedLeftClick(newDestination.X + fo_x, newDestination.Y + fo_y, 1000, false);

                    // Move cursor out of the way on the map
                    Cursor.Position = new Point(fo_x + 50, fo_y + 400);

                    continue;
                }

                // Nothing to do - wait for action
                Thread.Sleep(500);
            }
        }

        private void GoNearInstanceExit() {
            // Scroll to bottom right
            AddLog("Scrolling to bottom right..");
            Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width - 1, Screen.PrimaryScreen.Bounds.Height - 1);

            // Wait for the scroll
            Thread.Sleep(1000);

            // Run to the exit grid
            AddLog("Running to near the exit grid..");

            // Change cursor to movement
            DelayedRightClick(fo_x + 280, fo_y + 890, 50, false);

            // Double-click to run
            DelayedLeftClick(fo_x + 280, fo_y + 890, 0, true);

            // Change cursor back to normal
            DelayedRightClick(fo_x + 280, fo_y + 890, 50, false);
        }

        private void EscapeFromInstance(int method) {
            if (method == 1 || method == 2) {
                // Scroll to bottom right
                AddLog("Scrolling to bottom right..");
                Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width - 1, Screen.PrimaryScreen.Bounds.Height - 1);
            }
            else if (method == 3) {
                // Scroll to bottom left
                AddLog("Scrolling to bottom left..");
                Cursor.Position = new Point(1, Screen.PrimaryScreen.Bounds.Height - 1);
            }

            // Wait for the scroll
            Thread.Sleep(1000);

            // Run to the exit grid
            AddLog("Running to exit grid..");

            int exitX = fo_x + 220;
            int exitY = fo_y + 1010;

            // Add some randomness, to be able to fix lag/combat issues on the grid
            var r = new Random();
            if (r.Next(0, 2) == 0) {
                exitX -= 50;
            }

            if (method == 2) {
                exitX = fo_x + 1260;
                exitY = fo_y + 450;

                if (r.Next(0, 2) == 0) {
                    exitY -= 50;
                }
            }
            else if (method == 3) {
                exitX = fo_x + 20;
                exitY = fo_y + 450;

                if (r.Next(0, 2) == 0) {
                    exitY -= 50;
                }
            }

            // Change cursor (to movement hopefully)
            DelayedRightClick(exitX, exitY, 50, false);

            // Double-click to run
            DelayedLeftClick(exitX, exitY, 0, true);

            // We're not going to change cursor back, because this way we produce error tolerance and we'll eventually get out
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            StopEngine();
            HotKey.UnregisterHotKey(this);
        }

        private void DoDupeClick() {
            SendLeftClick();
            Thread.Sleep(130);
            SendLeftClick();
            Thread.Sleep(130);
            SendLeftClick();
        }

        private void DoEyeShot() {
            int hWnd = User32.GetForegroundWindow();
            if (hWnd == 0) {
                int error = GetLastError();

                AddLog("ERROR finding window: " + error);
                return;
            }
            User32.RECT bounds = new User32.RECT();
            User32.GetClientRect((IntPtr)hWnd, ref bounds);
            int width = bounds.right - bounds.left;
            int height = bounds.bottom - bounds.top;
            User32.WINDOWPLACEMENT wp = new User32.WINDOWPLACEMENT();
            User32.GetWindowPlacement((IntPtr)hWnd, ref wp);
            int x;
            int y;

            if (wp.showCmd == (int)User32.WindowState.SW_SHOWMAXIMIZED) {
                x = wp.ptMaxPosition.X;
                y = wp.ptMaxPosition.Y;
            }
            else {
                x = wp.rcNormalPosition.X;
                y = wp.rcNormalPosition.Y;
            }

            /*
            if ((x + width) > Screen.PrimaryScreen.WorkingArea.Width)
                width = width - ((x + width) - Screen.PrimaryScreen.WorkingArea.Width);

            if ((y + height) > Screen.PrimaryScreen.WorkingArea.Height)
                height = height - ((y + height) - Screen.PrimaryScreen.WorkingArea.Height);
            */

            Point curPos = Cursor.Position;

            DelayedLeftClick(curPos.X, curPos.Y, 25, false);
            DelayedLeftClick(x + 120, y + 136, 25, false);

            Cursor.Position = curPos;
        }
    }
}