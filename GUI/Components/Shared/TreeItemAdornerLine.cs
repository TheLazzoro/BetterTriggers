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
    public class TreeItemAdornerLine : Adorner
    {
        private bool isBefore;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element">Element to attach this adorner to.</param>
        /// <param name="isBefore">Whether to display the insertion in the top or bottom.</param>
        public TreeItemAdornerLine(UIElement element, bool isBefore) : base(element)
        {
            this.isBefore = isBefore;
        }

        // A common way to implement an adorner's rendering behavior is to override the OnRender
        // method, which is called by the layout system as part of a rendering pass.
        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.RenderSize);

            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            var color = (Color)Application.Current.Resources["treeItemAdornerColor"];
            Pen renderPen = new Pen(new SolidColorBrush(color), 1.5);
            double renderRadius = 5.0;

            // Draw a circle at each corner.
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);

            if (isBefore)
                drawingContext.DrawLine(renderPen, adornedElementRect.TopLeft, adornedElementRect.TopRight);
            else
                drawingContext.DrawLine(renderPen, adornedElementRect.BottomLeft, adornedElementRect.BottomRight);

        }

    }
}
