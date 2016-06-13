
using TriangleCalculator.ViewModels;
using Xamarin.Forms;

namespace TriangleCalculator.Views
{
    public partial class CalculatorPage : ContentPage
    {
        public CalculatorPage()
        {
            InitializeComponent();

            redrawButton.Clicked += RedrawButton_Clicked;
        }

        private void RedrawButton_Clicked(object sender, System.EventArgs e)
        {
            triangle.Triangle = (TriangleViewModel)Content.BindingContext;
        }
    }
}
