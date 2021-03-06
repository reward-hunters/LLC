﻿using System;
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

                ProgramCore.CurrentProgram = ProgramCore.ProgramMode.PrintAhead;
                ProgramCore.paypalHelper = new TruePaypalHelper();
                ProgramCore.IsFreeVersion = File.Exists(Path.Combine(Application.StartupPath, "bin", "rh_Saqr.dlib"));

             //   var objectCreator = new ObjCreator();
             //   objectCreator.CreateObj(2, "ph4ea4veilnuswyq2viwbm1m","","", "", "", "", "", "", "", "", "", "",0,0,0,0, "test");

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
