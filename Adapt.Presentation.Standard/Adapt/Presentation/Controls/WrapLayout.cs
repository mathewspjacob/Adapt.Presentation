﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Adapt.Presentation.Controls
{
    /// <summary>
    /// New WrapLayout
    /// </summary>
    /// <author>Jason Smith</author>
    public class WrapLayout : Layout<View>
    {
        private readonly Dictionary<View, SizeRequest> _LayoutCache = new Dictionary<View, SizeRequest>();

        /// <summary>
        /// Backing Storage for the Spacing property
        /// </summary>
        public static readonly BindableProperty SpacingProperty =
#pragma warning disable CS0618 // Type or member is obsolete
            BindableProperty.Create<WrapLayout, double>(w => w.Spacing, 5,
                propertyChanged: (bindable, oldvalue, newvalue) => ((WrapLayout)bindable)._LayoutCache.Clear());
#pragma warning restore CS0618 // Type or member is obsolete

        /// <summary>
        /// Spacing added between elements (both directions)
        /// </summary>
        /// <value>The spacing.</value>
        public double Spacing
        {
            get => (double)GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }

        public WrapLayout()
        {
            BackgroundColor = Color.Red;

            VerticalOptions = HorizontalOptions = LayoutOptions.FillAndExpand;
        }

        protected override void OnChildMeasureInvalidated()
        {
            base.OnChildMeasureInvalidated();
            _LayoutCache.Clear();
        }

        [Obsolete]
        protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
        {

            NaiveLayout(widthConstraint, out double lastX, out double lastY);

            return new SizeRequest(new Size(lastX, lastY));
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            var layout = NaiveLayout(width, out double lastX, out double lastY);

            foreach (var t in layout)
            {
                var offset = (int)((width - t.Last().Rectangle.Right) / 2);
                foreach (var dingus in t)
                {
                    var location = new Rectangle(dingus.Rectangle.X + x + offset, dingus.Rectangle.Y + y, dingus.Rectangle.Width, dingus.Rectangle.Height);
                    LayoutChildIntoBoundingRegion(dingus.View, location);
                }
            }
        }

        private List<List<ViewAndRectangle>> NaiveLayout(double width, out double lastX, out double lastY)
        {
            double startX = 0;
            double startY = 0;
            var right = width;
            double nextY = 0;

            lastX = 0;
            lastY = 0;

            var result = new List<List<ViewAndRectangle>>();
            var currentList = new List<ViewAndRectangle>();

            foreach (var child in Children)
            {
                if (!_LayoutCache.TryGetValue(child, out SizeRequest sizeRequest))
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    _LayoutCache[child] = sizeRequest = child.GetSizeRequest(double.PositiveInfinity, double.PositiveInfinity);
#pragma warning restore CS0618 // Type or member is obsolete
                }

                var paddedWidth = sizeRequest.Request.Width + Spacing;
                var paddedHeight = sizeRequest.Request.Height + Spacing;

                if (startX + paddedWidth > right)
                {
                    startX = 0;
                    startY += nextY;

                    if (currentList.Count > 0)
                    {
                        result.Add(currentList);
                        currentList = new List<ViewAndRectangle>();
                    }
                }

                currentList.Add(new ViewAndRectangle(child, new Rectangle(startX, startY, sizeRequest.Request.Width, sizeRequest.Request.Height)));

                lastX = Math.Max(lastX, startX + paddedWidth);
                lastY = Math.Max(lastY, startY + paddedHeight);

                nextY = Math.Max(nextY, paddedHeight);
                startX += paddedWidth;
            }
            result.Add(currentList);
            return result;
        }

        private class ViewAndRectangle
        {
            public ViewAndRectangle(View view, Rectangle rectangle)
            {
                View = view;
                Rectangle = rectangle;
            }

            public View View { get; set; }
            public Rectangle Rectangle { get; set; }
        }

    }
}
