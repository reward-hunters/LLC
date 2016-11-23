using CefSharp;
using RH.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Core;
using RH.PrintAhead_Paypal.Controls;
using System.Windows.Forms;
using static RH.Core.frmMain;

namespace RH.PrintAhead_Paypal
{
    public class TruePaypalHelper : PaypalHelper
    {
        public override void InitializeCef()
        {
            var settings = new CefSettings();           // инициализация хромимума. браузер для показа paypal
                                                        // Initialize cef with the provided settings
            Cef.Initialize(settings);
        }
        public override void ShutdownCef()
        {
            Cef.Shutdown();     // хромиум пускаем по пизде
        }

        public override void MakePayment(string price, string description, frmMain.PrintType printType)
        {
            if (ProgramCore.IsFreeVersion)
            {
                ProgramCore.MainForm.SubExport(printType);
                return;
            }

            var ctrl = new ctrlPaypalPayment(printType);
            if (!ctrl.CreatePayment(price, description))
            {
                MessageBox.Show("Can't activate paypal service. Check your Internet connection or try later..");
                return;
            }

            ctrl.ShowDialog();
        }

        public override void SuccessPay(FormEx parent, PrintType printType)
        {
            CloseSubForm(parent);

            ProgramCore.MainForm.Invoke((MethodInvoker)delegate
            {
                ProgramCore.MainForm.SubExport(printType);
                }
            );
        }
        public override void BadPay(FormEx parent)
        {
            CloseSubForm(parent);

            MessageBox.Show("Payment was failed!");
        }
        public void CloseSubForm(FormEx form)
        {
            if (form != null)
            {
                try
                {
                    form.Invoke((MethodInvoker)delegate
                    {
                        // close the form on the forms thread
                        form.Close();
                    });
                }
                catch
                { }
            }
        }

    }



}
