
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace TriangleCalculator.ViewModels
{
    public enum TriangleElementType
    {
        Angle,
        Side,
        Hypotenuse
    }

    public class TriangleElementViewModel
    {
        private bool isEnabled;
        private double val;
        private TriangleViewModel master { get; set; }

        public TriangleElementViewModel(TriangleElementType elementType, TriangleViewModel master)
        {
            ElementType = elementType;
            this.master = master;
        }

        public TriangleElementType ElementType { get; set; }
        public bool Recalculating { get; set; }
        public bool CanEnableBeToggled
        {
            get
            {
                return master.CanEnableAnyElement || isEnabled;
            }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                master.OnPropertyChanged("TriangleElements");
            }
        }

        public double Value
        {
            get
            {
                return val;
            }
            set
            {
                if (val != value && value > 0)
                {
                    if(ElementType == TriangleElementType.Angle)
                    {
                        val = Math.Round(value, 0);
                    }
                    else
                    {
                        val = value;
                    }
                    if (!Recalculating)
                    {
                        master.Recalculate();
                    }
                }
            }
        }
    }

    public class TriangleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Dictionary<string, TriangleElementViewModel> TriangleElements { get; set; }

        public TriangleViewModel()
        {
            TriangleElements = new Dictionary<string, TriangleElementViewModel>()
            {
                ["AC"] = new TriangleElementViewModel(TriangleElementType.Angle, this),
                ["BC"] = new TriangleElementViewModel(TriangleElementType.Angle, this),
                ["A"] = new TriangleElementViewModel(TriangleElementType.Side, this),
                ["B"] = new TriangleElementViewModel(TriangleElementType.Side, this),
                ["C"] = new TriangleElementViewModel(TriangleElementType.Hypotenuse, this)
            };
        }

        public TriangleViewModel Triangle
        {
            get
            {
                return this;
            }
            set
            {
                OnPropertyChanged("Triangle");
            }
        }

        public void Recalculate()
        {
            var enabledElements = TriangleElements.Where(x => x.Value.IsEnabled && x.Value.Value > 0);
            if (enabledElements.Count() == 2)
            {
                foreach (var item in TriangleElements)
                {
                    item.Value.Recalculating = true;
                }
                if (enabledElements.Where(x => x.Value.ElementType == TriangleElementType.Side).Count() == 2)
                {
                    var sideA = TriangleElements["A"].Value;
                    var sideB = TriangleElements["B"].Value;
                    TriangleElements["C"].Value = Math.Sqrt(sideA * sideA + sideB * sideB);
                    TriangleElements["AC"].Value = Math.Atan(sideB / sideA) * 180 / Math.PI;
                    TriangleElements["BC"].Value = 90 - TriangleElements["AC"].Value;
                }
                else if (enabledElements.Where(x => x.Value.ElementType == TriangleElementType.Side
                || x.Value.ElementType == TriangleElementType.Hypotenuse).Count() == 2)
                {
                    var sideC = TriangleElements["C"].Value;
                    var side = TriangleElements["A"].IsEnabled ? TriangleElements["A"].Value : TriangleElements["B"].Value;
                    var side2 = Math.Sqrt(sideC * sideC - side * side);
                    if (TriangleElements["A"].IsEnabled)
                        TriangleElements["B"].Value = side2;
                    else
                        TriangleElements["A"].Value = side2;
                    TriangleElements["AC"].Value = Math.Atan(TriangleElements["B"].Value / TriangleElements["A"].Value) * 180 / Math.PI;
                    TriangleElements["BC"].Value = 90 - TriangleElements["AC"].Value;
                }
                else if (enabledElements.Where(x => x.Value.ElementType == TriangleElementType.Angle).Count() == 2)
                {

                }
                else if (enabledElements.Where(x => x.Value.ElementType == TriangleElementType.Angle
                || x.Value.ElementType == TriangleElementType.Hypotenuse).Count() == 2)
                {
                    if (TriangleElements["AC"].IsEnabled)
                    {
                        TriangleElements["B"].Value = TriangleElements["C"].Value * Math.Sin(TriangleElements["AC"].Value * Math.PI / 180);
                        TriangleElements["A"].Value = Math.Sqrt(TriangleElements["C"].Value * TriangleElements["C"].Value - TriangleElements["B"].Value * TriangleElements["B"].Value);
                    }
                    else
                    {
                        TriangleElements["A"].Value = TriangleElements["C"].Value * Math.Sin(TriangleElements["BC"].Value * Math.PI / 180);
                        TriangleElements["B"].Value = Math.Sqrt(TriangleElements["C"].Value * TriangleElements["C"].Value - TriangleElements["A"].Value * TriangleElements["A"].Value);
                    }
                }
                else if (enabledElements.Where(x => x.Value.ElementType == TriangleElementType.Side
                || x.Value.ElementType == TriangleElementType.Angle).Count() == 2)
                {

                }

                OnPropertyChanged("TriangleElements");
                Triangle = this;

                foreach (var item in TriangleElements)
                {
                    item.Value.Recalculating = false;
                }
            }
        }

        public bool CanEnableAnyElement
        {
            get
            {
                return TriangleElements.Where(x => x.Value.IsEnabled).Count() != 2;
            }
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Draw(SKCanvas skcanvas)
        {
            skcanvas.Translate(300, 100);
            //skcanvas.RotateDegrees(90);
            // DoDraw (skcanvas);
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Color = new SKColor(0xff, 0x00, 0x00);
                paint.StrokeCap = SKStrokeCap.Square;
                paint.StrokeWidth = 5;
                
                float a = (float)TriangleElements["A"].Value * 25;
                float b = (float)TriangleElements["B"].Value * 25;
                skcanvas.DrawLine(0, 0, a, 0, paint);
                skcanvas.DrawLine(0, 0, 0, b, paint);
                skcanvas.DrawLine(a, 0, 0, b, paint);
            }
        }

    }
}
