using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using SkiaSharp;
using System.Collections.Generic;
using TriangleCalculator.ViewModels;

namespace TriangleCalculator.Droid
{
    public class TriangleView : ImageView
    {
        public TriangleViewModel Triangle { get; set; }
        private float translateX;
        private float translateY;
        private float moveTranslateX;
        private float moveTranslateY;
        private float startTouchX;
        private float startTouchY;

        public TriangleView(Context context, TriangleViewModel model) : base(context)
        {
            Triangle = model;
            Triangle.PropertyChanged += (s, args) => { Invalidate(); };
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            var touchX = e.GetX();
            var touchY = e.GetY();

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    startTouchX = touchX;
                    startTouchY = touchY;
                    break;
                case MotionEventActions.Move:
                    moveTranslateX = touchX - startTouchX;
                    moveTranslateY = touchY - startTouchY;
                    break;
                case MotionEventActions.Up:
                    moveTranslateX = moveTranslateY = 0;
                    translateX += touchX - startTouchX;
                    translateY += touchY - startTouchY;
                    break;
                default:
                    return false;
            }
            Invalidate();

            return true;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            var width = (float)Width;
            var height = (float)Height;

            using (var bitmap = Bitmap.CreateBitmap(canvas.Width, canvas.Height, Bitmap.Config.Argb8888))
            {
                try
                {
                    using (var surface = SKSurface.Create(canvas.Width, canvas.Height, SKColorType.Rgba_8888, SKAlphaType.Premul, bitmap.LockPixels(), canvas.Width * 4))
                    {
                        var skcanvas = surface.Canvas;
                        skcanvas.Scale(((float)canvas.Width) / width, ((float)canvas.Height) / height);
                        skcanvas.Translate(translateX + moveTranslateX, translateY + moveTranslateY);
                        Triangle.Draw(skcanvas);
                    }
                }
                finally
                {
                    bitmap.UnlockPixels();
                }
                canvas.DrawBitmap(bitmap, 0, 0, null);
            }
        }
    }
}