using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JustAProgressBar
{
    public partial class MainWindow : Window
    {
        private const string FilePath = "data.json";
        public MainWindow()
        {
            InitializeComponent();
            int cur=1, total=10;
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                var data = JsonSerializer.Deserialize<Data>(json);
                if (data != null)
                {
                    cur = data.cur;
                    total = data.total;
                }
            }
            Total.Text = $"{total}";
            Cur.Text = $"{cur}";

            this.Closing += (s, e) => SaveData();
        }

        private void SaveData()
        {
            var data = new Data { cur = int.Parse(Cur.Text), total=int.Parse(Total.Text) };
            string json = JsonSerializer.Serialize(data);
            File.WriteAllText(FilePath, json);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            var cur = int.Parse(Cur.Text);
            var total=int.Parse(Total.Text);
            if (cur != total)
            {
                Cur.Text = $"{int.Parse(Cur.Text) + 1}";
                UpdateProgressSmoothly(int.Parse(Cur.Text));
            }
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
                var cur = int.Parse(Cur.Text);
            var total = int.Parse(Total.Text);
            if (cur != 0)
            {
                Cur.Text = $"{int.Parse(Cur.Text) - 1}";
                UpdateProgressSmoothly(int.Parse(Cur.Text));
            }
        }

        void UpdateProgressSmoothly(double targetValue, int time = 1000)
        {
            if (targetValue < Bar.Value)
            {
                Bar.BeginAnimation(ProgressBar.ValueProperty, null);
                Bar.Value = targetValue;
                return;
            }
            DoubleAnimation animation = new DoubleAnimation(targetValue, TimeSpan.FromMilliseconds(time));
            animation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
            Bar.BeginAnimation(ProgressBar.ValueProperty, animation);
        }

        private void CurChanged(object sender, TextChangedEventArgs e)
        {
            if (Cur != null)
                UpdateProgressSmoothly(int.Parse(Cur.Text));
        }

        private void TotalChanged(object sender, TextChangedEventArgs e)
        {
            if (Cur != null)
            {
                Bar.Maximum = int.Parse(Total.Text);
                UpdateProgressSmoothly(int.Parse(Cur.Text));
            }
        }

        private void Total_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text[0]);
        }

        private void Cur_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text[0]);
        }
    }

    public class Data
    {
        public int cur { get; set; }
        public int total { get; set; }
    }
}