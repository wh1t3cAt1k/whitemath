using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

//  Этот файл содержит объявления и определения для событий и обработчиков событий
//  для различных компонентов, используемых в библиотеке.

namespace WhiteMath.Graphers
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
		private int _xPixel;
		private int _yPixel;

		private double _xValue;
		private double _yValue;

        /// <summary>
        /// Gets the relative (to the upper-left point) coordinates of the image pixel clicked.
        /// </summary>
        public Point PixelClicked
        {
            get { return new Point(_xPixel, _yPixel); }
        }

        /// <summary>
        /// Gets the X coordinate in the real (non-screen, but functional) coordinate system.
        /// </summary>
        public double X
        {
            get { return _xValue; }
        }

        /// <summary>
        /// Gets the Y coordinate in the real (non-screen, but functional) coordinate system.
        /// </summary>
        public double Y
        {
            get { return _yValue; }
        }

        public GraphicDrawerGraphClickEventArgs(int xPixel, int yPixel, double X, double Y)
        {
            _xPixel = xPixel;
            _yPixel = yPixel;
            _xValue = X;
            _yValue = Y;
        }
    }
}
