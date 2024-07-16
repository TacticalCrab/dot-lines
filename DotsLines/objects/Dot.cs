using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace DotsLines.objects
{
    public class Dot
    {
        public int Value { get; set; }

        public Ellipse Shape { get; set; }

        public Label Label { get; set; }

        public (double, double) Cords { get; set; } = (0, 0);

        public double X {
            get => Cords.Item1;
            set {
                var (_, y) = Cords;
                Cords = (value, y);
            }
        }

        public double Y {
            get => Cords.Item2;
            set {
                var (x, _) = Cords;
                Cords = (x, value);
            }
        }

        public System.Windows.Media.Brush Brush {
            get => Shape.Fill;
            set => Shape.Fill = value;
        }

        public Dot(int value)
        {
            Value = value;

            Shape = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = System.Windows.Media.Brushes.Black
            };

            Label = new Label
            {
                Content = value,
                Foreground = System.Windows.Media.Brushes.Red

            };
        }
    }
}
