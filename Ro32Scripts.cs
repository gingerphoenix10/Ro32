using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using Microsoft.Win32;
using System.Drawing.Drawing2D;

namespace Ro32
{
    public partial class Ro32 : Form
    {
        static string GameMessageEntry = "[FLog::Output] [Ro32]";
        [DllImport("user32")]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);
        [DllImport("user32")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
        [DllImport("user32.dll")]
        static extern int SetWindowText(IntPtr hWnd, string text);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        public Ro32()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //AllocConsole();
            LaunchR32(Status, Logo);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        static void LaunchR32(Label lbl1, PictureBox Logo)
        {
            string wpEngine = "";
            Process wp = new Process();
            bool IsDisposed = false;
            bool Connected = false;
            async Task StartWatcher()
            {
                string LogLocation = null!;

                int delay = 1000;

                //if (App.Settings.Prop.PowerTools)
                //    delay = 250;

                string logDirectory = Path.Combine(Paths.LocalAppData, "Roblox\\logs");

                if (!Directory.Exists(logDirectory))
                    return;

                FileInfo logFileInfo;

                // we need to make sure we're fetching the absolute latest log file
                // if roblox doesn't start quickly enough, we can wind up fetching the previous log file
                // good rule of thumb is to find a log file that was created in the last 15 seconds or so

                Console.WriteLine("Opening Roblox log file...");

                while (true)
                {
                    logFileInfo = new DirectoryInfo(logDirectory)
                        .GetFiles()
                        .Where(x => x.CreationTime <= DateTime.Now)
                        .OrderByDescending(x => x.CreationTime)
                        .First();

                    if (logFileInfo.CreationTime.AddSeconds(15) > DateTime.Now)
                        break;

                    Console.WriteLine($"Could not find recent enough log file, waiting... (newest is {logFileInfo.Name})");
                    await Task.Delay(1000);
                }

                LogLocation = logFileInfo.FullName;
                FileStream logFileStream = logFileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                Console.WriteLine($"Opened {LogLocation}");
                lbl1.Text = "Opened.";
                Connected = true;

                AutoResetEvent logUpdatedEvent = new(false);
                FileSystemWatcher logWatcher = new()
                {
                    Path = logDirectory,
                    Filter = Path.GetFileName(logFileInfo.FullName),
                    EnableRaisingEvents = true
                };
                logWatcher.Changed += (s, e) => logUpdatedEvent.Set();

                using StreamReader sr = new(logFileStream);

                while (!IsDisposed)
                {
                    string? log = await sr.ReadLineAsync();

                    if (string.IsNullOrEmpty(log))
                        //logUpdatedEvent.WaitOne(delay);
                        await Task.Delay(100);
                    else
                        ExamineLogEntry(log);
                }
                return;

            }
            void ExamineLogEntry(string entry)
            {
                try
                {
                    if (entry.Contains(GameMessageEntry))
                    {
                        string messagePlain = entry.Substring(entry.IndexOf(GameMessageEntry) + GameMessageEntry.Length + 1);
                        Message? message;
                        Console.WriteLine($"Received message: '{messagePlain}'");
                        message = JsonSerializer.Deserialize<Message>(messagePlain);

                        Process rbxProc;
                        try { rbxProc = Process.GetProcessesByName("RobloxPlayerBeta")[0]; }
                        catch { rbxProc = Process.GetProcessesByName("RobloxStudioBeta")[0]; }
                        IntPtr windowHandle = rbxProc.MainWindowHandle;
                        Console.WriteLine(windowHandle);
                        switch (message.Command)
                        {
                            case "OpenDialog":
                                Dialog? dialog;
                                dialog = JsonSerializer.Deserialize<Dialog>(message.Data);
                                int msgBox = MessageBox(windowHandle, dialog.Text, dialog.Title, 0);
                                break;
                            case "Window":
                                Window? window;
                                window = JsonSerializer.Deserialize<Window>(message.Data);
                                switch (window.setType)
                                {
                                    case "Minimize":
                                        Console.WriteLine("Attempting to minimize " + windowHandle);
                                        ShowWindow(windowHandle, 6);
                                        break;
                                    case "Maximize":
                                        Console.WriteLine("Attempting to maximize " + windowHandle);
                                        ShowWindow(windowHandle, 3);
                                        break;
                                    case "Rename":
                                        Rename? rename;
                                        rename = JsonSerializer.Deserialize<Rename>(window.Value);
                                        switch (rename.SetType)
                                        {
                                            case "Set":
                                                Console.WriteLine("Attempting to rename Roblox to: "+rename.Name);
                                                SetWindowText(windowHandle, rename.Name);
                                                break;
                                            case "Reset":
                                                Console.WriteLine("Resetting Roblox's title.");
                                                SetWindowText(windowHandle, "Roblox");
                                                break;
                                        }
                                        break;
                                    case "Float":
                                        Float? floatEvent;
                                        floatEvent = JsonSerializer.Deserialize<Float>(window.Value);
                                        Console.WriteLine("Attemping to move Roblox to X: "+floatEvent.PositionX+", Y: "+floatEvent.PositionY+", Width: "+floatEvent.Width+", Height: "+floatEvent.Height+".");
                                        SetWindowPos(windowHandle, 0, floatEvent.PositionX, floatEvent.PositionY, floatEvent.Width, floatEvent.Height, 0X4);
                                        break;
                                }
                                break;
                            case "Wallpaper":
                                WallpaperCommand? wcmd;
                                wcmd = JsonSerializer.Deserialize<WallpaperCommand>(message.Data);
                                Console.WriteLine("type: " + message.Data);
                                switch (wcmd.setType)
                                {
                                    case "Set":
                                        if (wpEngine != "") { wp.Kill(); }
                                        WallpaperSetCommand? wcmdSet;
                                        wcmdSet = JsonSerializer.Deserialize<WallpaperSetCommand>(wcmd.Value);
                                        Wallpaper.Style style = Wallpaper.Style.Stretched;
                                        if (wcmdSet.FitType == "Center") style = Wallpaper.Style.Centered;
                                        else if (wcmdSet.FitType == "Fit") style = Wallpaper.Style.Tiled;
                                        Console.WriteLine(wcmdSet.Image);
                                        Console.WriteLine(style);
                                        Uri url = new Uri(wcmdSet.Image);
                                        Wallpaper.Set(url, style);
                                        break;
                                    case "Reset":
                                        Wallpaper.Set(new Uri(Path.Combine(Paths.LocalAppData, "Roblox", "Ro32", "wallpaper.bmp")), Wallpaper.Style.Centered);
                                        if (wpEngine != "")
                                        {
                                            Process.Start(wpEngine);
                                        }
                                        break;
                                }
                                break;
                            case "FileSystem":
                                FilesystemCommand? fsCmd;
                                fsCmd = JsonSerializer.Deserialize<FilesystemCommand>(message.Data);
                                switch (fsCmd.setType)
                                {
                                    case "Create":
                                        FilesystemCreate? fsCreate;
                                        fsCreate = JsonSerializer.Deserialize<FilesystemCreate>(fsCmd.Value);
                                        Console.WriteLine(fsCreate.FilePath + ", " + fsCreate.TextData);
                                        File.WriteAllText(fsCreate.FilePath, fsCreate.TextData);
                                        break;
                                }
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR - " + e.ToString());
                }
            }
            float deg = 0;
            async Task SpinLogo()
            {
                Image originalImage = Logo.Image;
                DateTime prev = DateTime.Now;
                while (true) //while (!Connected)
                {
                    await Task.Delay(10);
                    if (Logo.Image != null)
                    {
                        if (originalImage == null) originalImage = Logo.Image;
                        deg += (DateTime.Now.Ticks - prev.Ticks) / 1000000f;
                        Bitmap bmp = new Bitmap(originalImage.Width, originalImage.Height);
                        Graphics gfx = Graphics.FromImage(bmp);
                        gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
                        gfx.RotateTransform(deg);
                        gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
                        gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gfx.DrawImage(originalImage, new Point(0, 0));
                        gfx.Dispose();
                        Logo.Image = bmp;
                    }
                    prev = DateTime.Now;
                }
            }
            SpinLogo();
            RegistryKey rkWallpaper = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false);
            string WallpaperPath = rkWallpaper.GetValue("WallPaper").ToString();
            rkWallpaper.Close();
            Directory.CreateDirectory(Path.Combine(Paths.LocalAppData, "Roblox", "Ro32"));
            File.Copy(WallpaperPath, Path.Combine(Paths.LocalAppData, "Roblox", "Ro32", "wallpaper.bmp"), true);
            if (Process.GetProcessesByName("wallpaper64").Length > 0)
            {
                wp = Process.GetProcessesByName("wallpaper64")[0];
                wpEngine = wp.MainModule.FileName;
            }
            else if (Process.GetProcessesByName("wallpaper32").Length > 0)
            {
                wp = Process.GetProcessesByName("wallpaper32")[0];
                wpEngine = wp.MainModule.FileName;
            }
            Console.WriteLine(wpEngine);
            StartWatcher();
        }
        private void Logo_Click(object sender, EventArgs e)
        {

        }
    }
}