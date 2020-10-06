using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;

namespace animationsticker
{
    public struct Resource
    {
        public Resource(int index, int frame, int count, string trayName, string path)
        {
            mIndex = index;
            mFrame = frame;
            mCount = count;
            mTrayName = trayName;
            mPath = path;
        }

        public int mIndex;
        public int mFrame;
        public int mCount;
        public string mTrayName;
        public string mPath;
    }

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double   SINGLE_FRAME = 0.01666667;
        private const int      FRAME_WIDTH = 100;
        private const int      FRAME_HEIGHT = 100;
        private const double   FRAME_SCALER = 0.7;
        private const int      MAX_CHARACTER_COUNT = 10;

        private Bitmap         mOriginalBitmap;
        private Bitmap[]       mBitmapFrames;
        private ImageSource[]  mImageFrames;

        private int            mFrame = -1;
        private int            mIndex = 0;
        private int            mFrameSize = 0;

        private List<Resource> mResources = new List<Resource>();
        private string         mTrayIconPath;
        private string         mTrayName;
        private string         mTrayExitString = "그렇게 여정은 끝이 났습니다.";

        /* for release bitmap */
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        private void Init()
        {
            /* Read string data from json file */
            StreamReader reader = new StreamReader("Resources/Config.json");
            string fileData = "";

            while (reader.EndOfStream == false)
            {
                fileData += reader.ReadLine();
            }
            reader.Close();

            /* Parse json-type string */
            JObject jo = JObject.Parse(fileData);
            var config = jo.SelectToken("config");
            int index = 0;

            foreach (var elem in config)
            {
                string type = elem.SelectToken("Type").ToString();
                int frame = Convert.ToInt32(elem.SelectToken("Frame").ToString());
                int count = Convert.ToInt32(elem.SelectToken("Count").ToString());
                string trayName = elem.SelectToken("TrayName").ToString();
                string path = elem.SelectToken("Path").ToString();

                if(count > MAX_CHARACTER_COUNT) { continue; }

                switch (type)
                {
                    case "Resource":
                        mResources.Add(new Resource(index, frame, count, trayName, path));
                        ++index;
                        break;

                    case "TrayIcon":
                        mTrayName = trayName;
                        mTrayIconPath = path;
                        break;
                }
            }
        }

        public MainWindow()
        {
            /* Initialize */
            InitializeComponent();
            Init();

            /* Start drawing first character */
            ChangeFrames(null, EventArgs.Empty);

            /* Create timer */
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(SINGLE_FRAME * mResources[mIndex].mFrame / 150 * 60 * 0.779); // 60FPS
            timer.Tick += NextFrame;
            timer.Start();

            /* Set MouseDown action */
            MouseDown += MainWindow_MouseDown;
            MouseDown += ChangeFrames;

            /* for modify icon */
            Bitmap tempIconBmp = System.Drawing.Image.FromFile(mTrayIconPath) as Bitmap;
            Bitmap tempFrame = new Bitmap(FRAME_WIDTH, FRAME_HEIGHT);
            using (Graphics g = Graphics.FromImage(tempFrame))
            {
                g.DrawImage(tempIconBmp,
                    new System.Drawing.Rectangle(0, 0, FRAME_WIDTH, FRAME_HEIGHT),
                    new System.Drawing.Rectangle(0, 0, FRAME_WIDTH, FRAME_HEIGHT),
                    GraphicsUnit.Pixel);
            }

            /* Make system tray */
            var menu = new System.Windows.Forms.ContextMenu();
            var noti = new System.Windows.Forms.NotifyIcon
            {
                Icon = System.Drawing.Icon.FromHandle(tempFrame.GetHicon()),
                Visible = true,
                Text = mTrayName,
                ContextMenu = menu,
            };

            /* Add system tray menus */
            foreach (var resource in mResources)
            {
                var tempMenuItem = new System.Windows.Forms.MenuItem
                {
                    Index = resource.mIndex,
                    Text = mResources[resource.mIndex].mTrayName
                };
                tempMenuItem.Click += (object original, EventArgs e) => { mIndex = resource.mIndex; };
                tempMenuItem.Click += ChangeFrames;
                menu.MenuItems.Add(tempMenuItem);
            }

            var quit = new System.Windows.Forms.MenuItem
            {
                Index = mResources.Count,
                Text = mTrayExitString,
            };
            quit.Click += (object o, EventArgs e) => { Application.Current.Shutdown(); };

            menu.MenuItems.Add(quit);
            noti.ContextMenu = menu;
        }

        private void ChangeFrames(object sender, EventArgs e)
        {
            mOriginalBitmap = System.Drawing.Image.FromFile(mResources[mIndex].mPath) as Bitmap;

            mFrameSize = mResources[mIndex].mFrame;
            mBitmapFrames = new Bitmap[mFrameSize];
            mImageFrames = new ImageSource[mFrameSize];

            for (int i = 0; i < mFrameSize; ++i)
            {
                int windowWidth = FRAME_WIDTH + (int)(FRAME_WIDTH * FRAME_SCALER * (mResources[mIndex].mCount - 1));
                mBitmapFrames[i] = new Bitmap(windowWidth, FRAME_HEIGHT);
                using (Graphics g = Graphics.FromImage(mBitmapFrames[i]))
                {
                    for (int j = 0; j < mResources[mIndex].mCount; ++j)
                    {
                        g.DrawImage(mOriginalBitmap,
                            new System.Drawing.Rectangle((int)(FRAME_WIDTH * FRAME_SCALER * j), 0, FRAME_WIDTH, FRAME_HEIGHT),
                            new System.Drawing.Rectangle(i * FRAME_WIDTH, 0, FRAME_WIDTH, FRAME_HEIGHT),
                            GraphicsUnit.Pixel);
                    }
                }

                var handle = mBitmapFrames[i].GetHbitmap();
                try
                {
                    mImageFrames[i] = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    DeleteObject(handle);
                }
            }
        }

        private void NextFrame(object sender, EventArgs e)
        {
            mFrame = (mFrame + 1) % mResources[mIndex].mFrame;
            Devil.Source = mImageFrames[mFrame];
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }

            else if (e.ChangedButton == MouseButton.Right)
            {
                ++mIndex;
                mIndex %= mResources.Count;
            }

            else if (e.ChangedButton == MouseButton.Middle)
            {
                Application.Current.Shutdown();
            }
        }
    }
}