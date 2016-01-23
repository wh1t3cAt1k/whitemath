using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace whiteMath.Imaging
{
    public static class Extensions
    {
        /// <summary>
        /// Extension method that draws all the decals from the list specified using the calling graphics object.
        /// </summary>
        /// <param name="G"></param>
        /// <param name="decals"></param>
        public static void drawDecals(this Graphics G, IList<IDecal> decals)
        {
            foreach (IDecal decal in decals)
				decal.drawMe(G);
        }

        /// <summary>
        /// Extension method that inverts the calling color object.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color invert(this Color color)
        {
            return Color.FromArgb(color.A, 255 - color.R, 255 - color.G, 255 - color.B);
        }

        /// <summary>
        /// Extension method that creates a pen array from a color list.
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        public static Pen[] pensFromColors(this IList<Color> colors)
        {
            List<Pen> pens = new List<Pen>();

            foreach (Color color in colors)
                pens.Add(new Pen(color));

            return pens.ToArray();
        }
    }
}
