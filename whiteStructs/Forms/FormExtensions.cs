using System.Windows.Forms;
using System.Diagnostics.Contracts;

namespace whiteStructs.Forms
{
    /// <summary>
    /// This class provides different extension methods
    /// for Windows Forms.
    /// </summary>
    [ContractVerification(true)]
    public static class FormExtensions
    {
        /// <summary>
        /// Shows an error message in a message box
        /// which belongs to the caller window.
        /// </summary>
        /// <param name="owner">The caller window.</param>
        /// <param name="message">The error message to show.</param>
        public static void ShowErrorInMessageBox(this IWin32Window owner, string message)
        {
            string errorCaption =
                (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "RU" ?
                "Ошибка" : "Error");

            MessageBox.Show(
                owner,
                message,
                errorCaption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Shows an warning message in a message box
        /// which belongs to the caller window.
        /// </summary>
        /// <param name="owner">The caller window.</param>
        /// <param name="message">The warning message to show.</param>
        /// <param name="question">
        /// If this parameter is <c>true</c>, the message box
        /// will be displayed with 'Yes' and 'No' buttons instead of an 'OK' button.
        /// </param>
        public static DialogResult ShowWarningInMessageBox(this IWin32Window owner, string message, bool question)
        {
            string errorCaption =
                (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "RU" ?
                "Внимание!" : "Warning!");

            return MessageBox.Show(
                owner,
                message,
                errorCaption,
                (question? MessageBoxButtons.YesNo : MessageBoxButtons.OK),
                MessageBoxIcon.Warning,
                (question? MessageBoxDefaultButton.Button2 : MessageBoxDefaultButton.Button1));
        }
    }
}
