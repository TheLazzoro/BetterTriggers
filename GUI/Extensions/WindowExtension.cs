using System.Windows;
using System.Windows.Forms;

namespace GUI.Extensions
{
    public static class WindowExtension
    {
        public static bool IsWithinScreenBounds(this Window window)
        {
            int left = (int)window.Left;
            int top = (int)window.Top;
            var screen = Screen.FromPoint(new System.Drawing.Point(left, top));

            bool isWithinX = left > screen.Bounds.X && left < screen.Bounds.Width;
            bool isWithinY = top > screen.Bounds.Y && top < screen.Bounds.Height;
            if (isWithinX && isWithinY)
            {
                return true;
            }

            return false;
        }

        public static void ResetPositionWhenOutOfScreenBounds(this Window window)
        {
            if(!IsWithinScreenBounds(window))
            {
                window.Top = 100;
                window.Left = 100;
            }
        }
    }
}
