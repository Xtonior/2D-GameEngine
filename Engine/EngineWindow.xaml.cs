using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;
using static Engine.MainWindow;

namespace Engine
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public enum EntityTypes
        {
            Type1,
            Type2,
            Type3,
            Type4
        }

        public static List<GameEntity> entitiesOnScene = new List<GameEntity>();
        public string scenePath = @"C:\scene.xts";

        public static Script script = new Script();

        public DispatcherTimer timer;
        public long fps = 66;

        private static Action EmptyDelegate = delegate () { };

        public MainWindow()
        {
            InitializeComponent();
            LoadScene(scenePath);

            UserInterfaceCustomScale(1);
            this.Dispatcher.BeginInvoke(DispatcherPriority.Render, EmptyDelegate);

            MainGrid.RenderTransformOrigin = new Point(0, 0);

            timer = new DispatcherTimer();
            timer.Tick += Update;
            timer.Interval = new TimeSpan(fps);
            timer.Start();

            Title = scenePath;
        }

        private void Update(object sender, EventArgs e)
        {
            for (int i = 0; i < entitiesOnScene.Count; i++)
            {
                if (entitiesOnScene[i].initialized)
                {
                    entitiesOnScene[i].Update();
                }
            }

            script.Update();
        }

        public void LoadScene(string path)
        {
            if (File.Exists(path))
            {
                entitiesOnScene.Clear();

                XmlSerializer s = new XmlSerializer(typeof(Scene));

                using (Stream reader = new FileStream(scenePath, FileMode.Open))
                {
                    Scene scene = (Scene)s.Deserialize(reader);

                    for (int i = 0; i < scene.entities.Count; i++)
                    {
                        entitiesOnScene.Add(new GameEntity());
                        entitiesOnScene[entitiesOnScene.Count - 1].Initialize(scene.entities[i], this);
                    }
                }
            }

            script.Start();
        }

        private void UserInterfaceCustomScale(double customScale)
        {
            this.LayoutTransform = new ScaleTransform(customScale, customScale, 0, 0);
            Width *= customScale;
            Height *= customScale;

            var screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            var screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;

            Top = (screenHeight - Height) / 2;
            Left = (screenHeight - Width) / 2;
        }
    }
}

