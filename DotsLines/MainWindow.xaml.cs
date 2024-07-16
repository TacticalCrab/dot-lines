using DotsLines.objects;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Shapes;

namespace DotsLines
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Dictionary<int, Dot> Dots { get; set; } = [];
        
        public Dictionary<(int, int), Line> DotsConnections { get; set; } = [];

        public Dictionary<string, List<int>> Paths { get; set; } = [];

        public string? SelectedPathName { get; set; } = null;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitDotsAndPaths();
            InitPathsBox();
        }

        private void CreateRandomPath(int length)
        {
            var path = new List<int>();
            for (var i = 0; i < length; i++)
            {
                path.Add(Random.Shared.Next(1, 10));
            }

            var name = $"RandomPath{Paths.Count + 1}";
            Paths.Add(name, path);
        }

        private void PathComboBox_Selected(object sender, RoutedEventArgs e)
        {
            var selectedPath = PathComboBox.SelectedItem.ToString();
            SelectedPathName = selectedPath;

            if (SelectedPathName is null || !Paths.ContainsKey(SelectedPathName))
            {
                SelectedPathName = null;
                return;
            }

            HighlightPath(Paths[SelectedPathName]);
        }

        private void InitDotsAndPaths()
        {
            var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "bongologo.txt");
            var fileContent = File.ReadAllText(filePath, Encoding.UTF8);
            var rawPaths = fileContent.Split('\n');

            foreach (var path in rawPaths)
            {
                var splitPath = path.Split(',');

                var name = splitPath[0];
                var parsedPaths = splitPath.Skip(1).Select(int.Parse).ToList();

                Paths.Add(name, parsedPaths);
            }

            CreateRandomPath(100);
            CreateRandomPath(100);

            foreach (var path in Paths.Values)
            {
                for (var i = 1; i < path.Count; i++)
                {
                    var prevValue = path[i - 1];
                    var currentValue = path[i];

                    var prevDot = GetDot(prevValue);
                    var currentDot = GetDot(currentValue);

                    var key = Utils.GetOrderedTuple(prevValue, currentValue);
                    if (DotsConnections.ContainsKey(key))
                        continue;

                    var line = new Line()
                    {
                        X1 = prevDot.Cords.Item1 * DotsCanvas.ActualWidth,
                        Y1 = prevDot.Cords.Item2 * DotsCanvas.ActualHeight,
                        X2 = currentDot.Cords.Item1 * DotsCanvas.ActualWidth,
                        Y2 = currentDot.Cords.Item2 * DotsCanvas.ActualHeight,
                        Stroke = (DisplayAllDots.IsChecked == true)? System.Windows.Media.Brushes.Black: System.Windows.Media.Brushes.Transparent,
                        StrokeThickness = 2
                    };

                    DotsConnections.Add(key, line);

                    DotsCanvas.Children.Add(line);
                }
            }
        }

        private Dot GetDot(int value)
        {
            if (!Dots.TryGetValue(value, out Dot? dot))
            {
                dot = new Dot(value)
                {
                    Cords = (Random.Shared.NextDouble(), Random.Shared.NextDouble())
                };

                Dots.Add(value, dot);
                DotsCanvas.Children.Add(dot.Shape);

                Canvas.SetLeft(dot.Shape, dot.X * DotsCanvas.ActualWidth - dot.Shape.Width / 2);
                Canvas.SetTop(dot.Shape, dot.Y * DotsCanvas.ActualHeight - dot.Shape.Height / 2);
                Canvas.SetZIndex(dot.Shape, 5);

                DotsCanvas.Children.Add(dot.Label);
                Canvas.SetLeft(dot.Label, dot.X * DotsCanvas.ActualWidth - dot.Label.ActualWidth / 2);
                Canvas.SetTop(dot.Label, dot.Y * DotsCanvas.ActualHeight - dot.Label.ActualHeight / 2);
                Canvas.SetZIndex(dot.Label, 6);
            }

            return dot;
        }

        private async void HighlightPath(List<int> path)
        {
            ResetPaths();

            for (var i = 1; i < path.Count; i++)
            {
                var prevValue = path[i - 1];
                var prevDot = Dots[prevValue];
                prevDot.Brush = System.Windows.Media.Brushes.Red;
                await Task.Delay(500);

                int currentValue = path[i];

                var key = Utils.GetOrderedTuple(prevValue, currentValue);
                
                var line = DotsConnections[key];
                line.Stroke = System.Windows.Media.Brushes.Red;
                Canvas.SetZIndex(line, 4);
                await Task.Delay(500);
                
                prevDot.Brush = System.Windows.Media.Brushes.Black;

                if (PresistLines.IsChecked == true)
                    line.Stroke = System.Windows.Media.Brushes.Black;
                else
                    line.Stroke = System.Windows.Media.Brushes.Transparent;
            }
        }

        private void ResetPaths()
        {
            foreach (var line in DotsConnections)
            {
                if (DisplayAllDots.IsChecked == true)
                    line.Value.Stroke = System.Windows.Media.Brushes.Black;
                else
                    line.Value.Stroke = System.Windows.Media.Brushes.Transparent;

                Canvas.SetZIndex(line.Value, 1);
            }

            foreach (var dot in Dots)
            {
                dot.Value.Brush = System.Windows.Media.Brushes.Black;
            }   
        }

        private void InitPathsBox()
        {
            PathComboBox.Items.Clear();
            foreach (var path in Paths)
            {
                PathComboBox.Items.Add(path.Key);
            }
        }

        private void DisplayAllDots_Changed(object sender, RoutedEventArgs e)
        {
            ResetPaths();
        }

        private void PresistLines_Changed(object sender, RoutedEventArgs e)
        {
            
        }
    }
}