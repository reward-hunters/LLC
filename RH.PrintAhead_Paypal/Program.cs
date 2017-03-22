using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using RH.Core;
using RH.PrintAhead_Paypal;
using RH.WebCore;

namespace RH.OneClick
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var currentDomain = AppDomain.CurrentDomain;
                currentDomain.AssemblyResolve += LoadSubLibs;

                ProgramCore.CurrentProgram = ProgramCore.ProgramMode.PrintAhead_Online;
                ProgramCore.paypalHelper = new TruePaypalHelper();
                ProgramCore.IsFreeVersion = File.Exists(Path.Combine(Application.StartupPath, "bin", "rh_Saqr.dlib"));


                var objectCreator = new ObjCreator();
                objectCreator.CreateObj(1, "http://www.gimpart.org/wp-content/uploads/2011/12/lady.jpg", "1fxp4j4ixurjv1uyetgvkyj4");

                //FTPHelper.IsFileExists("ftp://108.167.164.209/public_html/printahead.online/PrintAhead_images/wqlofu1vq4p4a2x0rnslcikm.jpeg");
                //    var objectCreator = new ObjCreator();
                //    objectCreator.CreateObj(ManType.Female);
                // CropHelper.Pass1(@"http://i0.wp.com/peopledotcom.files.wordpress.com/2016/11/prince-harry7.jpg?crop=0px%2C0px%2C1427px%2C1427px&resize=1000%2C1000&ssl=1");

                ProgramCore.MainForm = new frmMain(args.Length == 0 ? string.Empty : args[0]);
                Application.Run(ProgramCore.MainForm);
            }
            catch (Exception e)
            {
                ProgramCore.EchoToLog(e);
                MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace, e.Message);
            }
        }

        static Assembly LoadSubLibs(object sender, ResolveEventArgs args)
        {
            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var assemblyPath = Path.Combine(folderPath, "bin", "lib", new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath)) return null;
            var assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }
    }
}
