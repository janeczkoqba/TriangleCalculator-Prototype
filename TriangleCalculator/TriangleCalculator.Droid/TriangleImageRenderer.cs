using System;
using System.ComponentModel;
using Android.Graphics;
using TriangleCalculator;
using TriangleCalculator.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer (typeof(TriangleImageControl), typeof(TriangleImageRenderer))]
namespace TriangleCalculator.Droid
{
    public class TriangleImageRenderer : ViewRenderer<TriangleImageControl, TriangleView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TriangleImageControl> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                SetNativeControl(new TriangleView(Context, Element.Triangle));
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if(e.PropertyName == TriangleImageControl.TriangleProperty.PropertyName)
            {
                UpdateTriangle();
            }
        }

        private void UpdateTriangle()
        {
            Control.PostInvalidate();
        }
    }
}