using System.Windows.Forms;
using CefSharp;
using RH.Core;
using RH.Core.Helpers;
using RH.PrintAhead_Paypal.Controls;

namespace RH.PrintAhead_Paypal
{
    public class TruePaypalHelper : PaypalHelper
    {
        public override void InitializeCef()
        {
            var settings = new CefSettings();           // инициализация хромимума. браузер для показа paypal
                                                        // Initialize cef with the provided settings
            Cef.Initialize(settings);
            Application.ApplicationExit += Application_ApplicationExit;
        }

        private void Application_ApplicationExit(object sender, System.EventArgs e)
        {
            ShutdownCef();
        }

        public override void ShutdownCef()
        {
            Cef.Shutdown();     // хромиум пускаем по пизде
        }

        public override void MakePayment(string price, string description, frmMain.PrintType printType)
        {
            if (ProgramCore.IsFreeVersion)
            {
                ProgramCore.MainForm.SuccessPay(printType);      // бесплатная версия
                return;
            }

            var ctrl = new ctrlPaypalPayment(printType);
            if (!ctrl.CreatePayment(price, description))
            {
                MessageBox.Show("Can't activate paypal service. Check your Internet connection or try later..");
                return;
            }

            ProgramCore.ShowDialog(ProgramCore.MainForm, ctrl, "Please pay for export!", MessageBoxButtons.OK, 0, false, false);
            if (ctrl.IsSuccess)
                ProgramCore.MainForm.SuccessPay(printType);
            else ProgramCore.MainForm.BadPay();
        }

    }



}
