﻿using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using RH.Core;

namespace RH.FaceAge
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

                ProgramCore.CurrentProgram = ProgramCore.ProgramMode.FaceAge2_Partial;
                ProgramCore.IsTutorialVisible = false;
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
