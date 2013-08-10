using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;

namespace whiteMath.Imaging
{
    public interface IDecal
    {
        void drawMe(Graphics G);
    }

    /// <summary>
    /// Represents a decal on the image with a path shape.
    /// </summary>
    public class PathDecal: IDecal
    {
        public GraphicsPath Path { get; set; }

        private Pen boundPen = null;
        private Brush innerBrush = null;

        public PathDecal(GraphicsPath path, Color? boundColor = null, Color? innerColor = null)
        {
            this.Path = path;

            if (boundColor.HasValue)
                this.boundPen = new Pen(boundColor.Value);
            if (innerColor.HasValue)
                this.innerBrush = new SolidBrush(innerColor.Value);
        }

        public PathDecal(GraphicsPath path, Pen boundPen = null, Brush innerBrush = null)
        {
            this.Path = path;

            this.boundPen = boundPen;
            this.innerBrush = innerBrush;
        }

        public void drawMe(Graphics G)
        {
            if (innerBrush != null)
                G.FillPath(innerBrush, Path);

            if (boundPen != null)
                G.DrawPath(boundPen, Path);
        }
    }

    // ----------------------------------

    /// <summary>
    /// Represents a decal on the image with a shape of a rectangle.
    /// </summary>
    public class RectangleDecal: IDecal
    {
        public Rectangle rectangle { get; set; }
        public Pen boundPen { get; set; }
        public Brush innerBrush { get; set; }

        public RectangleDecal(Rectangle rect, Color? boundColor, Color? innerColor)
        {
            this.rectangle = rect;

            if (boundColor.HasValue)
                boundPen = new Pen(boundColor.Value);

            if (innerColor.HasValue)
                innerBrush = new SolidBrush(innerColor.Value);
        }

        public RectangleDecal(Rectangle rect, Pen boundPen, Brush innerBrush)
        {
            this.rectangle = rect;

            this.boundPen = boundPen;
            this.innerBrush = innerBrush;
        }

        public void drawMe(Graphics G)
        {
            if (innerBrush != null)
                G.FillRectangle(innerBrush, rectangle);

            if (boundPen != null)
                G.DrawRectangle(boundPen, rectangle);
        }
    }
}
