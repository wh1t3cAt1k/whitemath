using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace whiteMath.General
{
    public static class ContainerExtensions
    {
        /// <summary>
        /// Creates the form that contains the invoking user control as its only child component.
        /// </summary>
        public static Form createContainingForm(this UserControl ctrl, FormBorderStyle borderStyle = FormBorderStyle.Sizable)
        {
            Form obj = new Form();
            Panel pane = new Panel();

            pane.Parent = obj;
            pane.Dock = DockStyle.Fill;

            pane.MinimumSize = ctrl.MinimumSize;
            pane.MaximumSize = ctrl.MaximumSize;

            // determine minimum size for the form

            if (!pane.MinimumSize.IsEmpty)
            {
                pane.Size = pane.MinimumSize;

                obj.ClientSize = pane.Size;
                obj.MinimumSize = obj.Size;
            }

            // determine maximum size for the form

            if (!pane.MaximumSize.IsEmpty)
            {
                pane.Size = pane.MaximumSize;

                obj.ClientSize = pane.Size;
                obj.MaximumSize = obj.Size;
            }

            // set normal

            pane.Size = ctrl.Size;
            obj.ClientSize = pane.Size;

            // make it.

            ctrl.Parent = pane;
            ctrl.Dock = DockStyle.Fill;

            obj.FormBorderStyle = borderStyle;
            obj.Text = ctrl.Text;

            // keyboard events should be passed to the form now.

            obj.KeyPreview = true;

            return obj;
        }
    }
}
