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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Collections;

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
        [System.ComponentModel.Browsable(false)]
        public IntPtr Handle { get; }
        protected override void OnLoad(EventArgs e)
        {
            Visible = true; // Hide form window.
            ShowInTaskbar = true; // Remove from taskbar.
            Opacity = 1;

            base.OnLoad(e);
        }
        List<string> args;
        public Ro32(string[] arguments)
        {
            args = arguments.ToList();
            InitializeComponent();
        }
        private void ToTray()
        {
            this.Visible = false;
            this.ShowInTaskbar = false;
            this.Opacity = 0;
        }
        private void Ro32_Load(object sender, EventArgs e)
        {
            args = (from s in args select s.ToLower()).ToList();
            if (args.Contains("-console")) AllocConsole();
            if (args.Contains("-minimized") || args.Contains("-minimize")) ToTray();
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

                string logDirectory = Path.Combine(Paths.LocalAppData, "Roblox\\logs");

                if (!Directory.Exists(logDirectory))
                    return;

                FileInfo logFileInfo;

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
                                                Console.WriteLine("Attempting to rename Roblox to: " + rename.Name);
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
                                        SetWindowPos(windowHandle, 0, floatEvent.PositionX, floatEvent.PositionY, floatEvent.Width, floatEvent.Height, 0X4);
                                        break;
                                    case "Close": //caseoh
                                        rbxProc.Kill(); //I could honestly just crash the game with scripts lol
                                        break;
                                }
                                break;
                            case "Wallpaper":
                                WallpaperCommand? wcmd;
                                wcmd = JsonSerializer.Deserialize<WallpaperCommand>(message.Data);
                                switch (wcmd.setType)
                                {
                                    case "Set":
                                        if (wpEngine != "") { wp.Kill(); }
                                        WallpaperSetCommand? wcmdSet;
                                        wcmdSet = JsonSerializer.Deserialize<WallpaperSetCommand>(wcmd.Value);
                                        Wallpaper.Style style = Wallpaper.Style.Stretched;
                                        if (wcmdSet.FitType == "Center") style = Wallpaper.Style.Centered;
                                        else if (wcmdSet.FitType == "Fit") style = Wallpaper.Style.Tiled;
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
                while (true)
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
            StartWatcher();
        }
        private void Logo_Click(object sender, EventArgs e)
        {

        }
        private void ctrlPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.ShowInTaskbar = true;
            this.Opacity = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ToTray();
            int msgBox = MessageBox(Handle, "Ro32 has been minimized to the System tray.", "Minimized", 0);
        }
    }
}