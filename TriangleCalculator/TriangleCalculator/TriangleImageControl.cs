
using TriangleCalculator.ViewModels;
using Xamarin.Forms;

namespace TriangleCalculator
{
    public class TriangleImageControl : Image
    {
        public static readonly BindableProperty TriangleProperty =
            BindableProperty.Create((TriangleImageControl x) => x.Triangle, new TriangleViewModel());

        public TriangleViewModel Triangle {
            get
            {
                return (TriangleViewModel)GetValue(TriangleProperty);
            }
            set
            {
                SetValue(TriangleProperty, value);
                OnPropertyChanged();
            }
        }

    }
}
