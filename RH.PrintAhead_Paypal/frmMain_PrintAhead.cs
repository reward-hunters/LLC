using CefSharp;
using RH.Core;
using RH.PrintAhead_Paypal.Controls;

namespace RH.PrintAhead_Paypal
{
    public partial class frmMain_PrintAhead : frmMain
    {
        public frmMain_PrintAhead(string fn)
            : base(fn)
        {
            //  InitializeComponent();

            var settings = new CefSettings();           // инициализация хромимума. браузер для показа paypal
                                                                // Initialize cef with the provided settings
            Cef.Initialize(settings);
        }
        ~frmMain_PrintAhead()
        {
            Cef.Shutdown();     // хромиум пускаем по пизде
        }

        public override bool MakePayment(string price, string description)
        {
            var ctrl = new ctrlPaypalPayment();
            if (!ctrl.CreatePayment(price, description))
                return false;

            ProgramCore.ShowDialog(ctrl, "Please pay for print!", false, false);
            return ctrl.IsSuccess;
        }
    }
}
