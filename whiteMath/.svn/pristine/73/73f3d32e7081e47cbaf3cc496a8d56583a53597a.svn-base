using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

//  Этот файл содержит объявления и определения для событий и обработчиков событий
//  для различных компонентов, используемых в библиотеке.

namespace whiteMath.Graphers
{
    /// <summary>
    /// Handler for the event called when the user clicks on the graph.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void GraphicDrawerGraphClickEventHandler(object source, GraphicDrawerGraphClickEventArgs e);

    /// <summary>
    /// The event arguments for GraphicDrawerGraphClick.
    /// Contains the coordinates of the pixel clicked in two forms:
    /// 
    /// 1) Pixel component
    /// 2) Function component
    /// </summary>
    public class GraphicDrawerGraphClickEventArgs : EventArgs
    {
        public int xPixel;      // пикселя
        public int yPixel;      // пикселя

        public double xValue;   // функции
        public double yValue;   // функции

        /// <summary>
        /// Gets the relative (to the upper-left point) coordinates of the image pixel clicked.
        /// </summary>
        public Point PixelClicked
        {
            get { return new Point(xPixel, yPixel); }
        }

        /// <summary>
        /// Gets the X coordinate in the real (non-screen, but functional) coordinate system.
        /// </summary>
        public double X
        {
            get { return xValue; }
        }

        /// <summary>
        /// Gets the Y coordinate in the real (non-screen, but functional) coordinate system.
        /// </summary>
        public double Y
        {
            get { return yValue; }
        }

        public GraphicDrawerGraphClickEventArgs(int xPixel, int yPixel, double X, double Y)
        {
            this.xPixel = xPixel;
            this.yPixel = yPixel;
            this.xValue = X;
            this.yValue = Y;
        }
    }
}
