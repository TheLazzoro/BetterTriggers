using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace GUI.Components.Shared
{
    public class TreeItemAdornerSquare : Adorner
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element">Element to attach this adorner to.</param>
        /// <param name="isBefore">Whether to display the insertion in the top or bottom.</param>
        public TreeItemAdornerSquare(UIElement element) : base(element)
        {
        }

        // A common way to implement an adorner's rendering behavior is to override the OnRender
        // method, which is called by the layout system as part of a rendering pass.
        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.RenderSize);
            adornedElementRect.Height = 16;

            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.AliceBlue);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)), 1);
            double renderRadius = 5.0;

            // Draw a circle at each corner.
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);

            drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);

        }

    }
}
