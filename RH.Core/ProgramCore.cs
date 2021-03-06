﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using RH.Core.Controls;
using RH.Core.Controls.Progress;
using RH.Core.Helpers;
using RH.Core.IO;
using Timer = System.Timers.Timer;

namespace RH.Core
{
    public static class ProgramCore
    {
        #region Var

        /// <summary> Main form </summary>
        public static frmMain MainForm
        {
            get;
            set;
        }

        /// <summary> Splash screen</summary>
        public static Form Splash
        {
            get;
            private set;
        }

        public static Project Project
        {
            get;
            set;
        }

        /// <summary> Требуется ли показывать инструкции пользователю </summary>
        public static bool IsTutorialVisible = true;

        public static NumberFormatInfo Nfi;
        public static string RegistryPath = "Software\\RH\\HeadShop";       // Registry path to keep persistent data

        public const bool UseDefaultDots = false;
        public const bool Debug = false;
        public static bool PluginMode = false;      // запускаем ли прогу из DAZ или просто

        public static bool DefaultIsSmile = true;

        public static bool IsFreeVersion;       // бесплатная версия PrintAhead для пожертвований в школы.

        public static ProgramMode CurrentProgram = ProgramMode.PrintAhead_PayPal;
        public enum ProgramMode
        {
            PrintAhead,                 // обычный PrintAhead
            PrintAhead_PayPal,          // PrintAhead с интеграцией PayPal. Нет возможности сохранения, экспорта. Только печать за деньги
            PrintAhead_Online,          // Онлайн версия (незначительные изменение с paypal версией: загружается ряд аксессуаров по дефолту, добавлены АПИ вызовы для сайта)
            HeadShop_v10_2,             // HeadShop 10.2
            HeadShop_v11,               // HeadShop 11 (РАБОЧАЯ версия с повернутами головами)
            HeadShop_OneClick,          // урезанная версия HeadShop. Без возможности сохранения и с одной активной вкладкой Front
            HeadShop_Rotator,           // Версия HeadShop.11 в которой возможно работа с повернутыми головами на фотографиях. (Версия НЕ РАБОЧАЯ! Делали в качестве эксперимента, результат не понравился старику)
            HeadShop_OneClick_v2,        // Урезаная версия HeadShop 11
            FaceAge2_Partial            // Неполноценная версия FaceAge, предназначен для работы с photoshop. В данном варианте НЕ работает как плагин, а просто как прога с
        }
        public static string ProgramCaption
        {
            get
            {
                switch (CurrentProgram)
                {
                    case ProgramMode.HeadShop_v10_2:
                        return "HeadShop 10.2";
                    case ProgramMode.HeadShop_v11:
                        return "HeadShop 11";
                    case ProgramCore.ProgramMode.HeadShop_OneClick_v2:
                        return "HeadShop OneClick 2";
                    case ProgramMode.HeadShop_Rotator:
                        return "HeadShop 11";
                    case ProgramMode.PrintAhead:
                        return "PrintAhead";
                    case ProgramMode.PrintAhead_PayPal:
                        return "PrintAhead 2.0";
                    case ProgramMode.PrintAhead_Online:
                        return "PrintAhead Online";
                    case ProgramMode.HeadShop_OneClick:
                        return "HeadShop OneClick";
                    case ProgramCore.ProgramMode.FaceAge2_Partial:
                        return "FaceAge 2";
                }
                return "Abalone LLC";
            }
        }
        public static string ProgramFolderCaption
        {
            get
            {
                switch (CurrentProgram)
                {
                    case ProgramMode.HeadShop_v10_2:
                    case ProgramMode.HeadShop_v11:
                    case ProgramCore.ProgramMode.HeadShop_OneClick_v2:
                    case ProgramMode.HeadShop_Rotator:
                    case ProgramMode.HeadShop_OneClick:
                        return "HeadShop";
                    case ProgramMode.PrintAhead:
                    case ProgramMode.PrintAhead_PayPal:
                    case ProgramMode.PrintAhead_Online:
                        return "PrintAhead";
                    case ProgramCore.ProgramMode.FaceAge2_Partial:
                        return "FaceAge";
                }
                return "Abalone LLC";
            }
        }

        public static PaypalHelper paypalHelper;

        #endregion

        static ProgramCore()
        {
            Nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };
            Splash = new frmSplash();

            LogFolder = "log"; // determine the name of the folder that will be used for logging
            logFileName = String.Format("log_{0}.txt", DateTime.Now.ToShortDateString().Replace(".", "_").Replace("/", "_")); // determine log filename. It has prefix "log_" and current date in short format (28.03.2014).

            tmr.Interval = 40;
            tmr.Elapsed += tmr_Elapsed;
            tmr.Start();
        }

        #region Echo To Log

        /// <summary> File, where will log writing. </summary>
        private static readonly string logFileName;

        /// <summary> Folder, where all log files storaging </summary>
        private static string logFolder;

        /// <summary> Folder, where all log files storaging </summary>
        private static string LogFolder
        {
            set
            {
                try
                {
                    if (value.Trim() == "") // just check. TRIM() funtion for string - remove all spaces from start and end. If our value is empty - just set folder by default. it's- logValue
                        value = "log";
                    //Application.StartupPath
                    var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Abalone");
                    value = Path.Combine(path, value); // Path.Combine - concatinating path to directory, where our program and filename.
                    Directory.CreateDirectory(value); // check and create folder 
                    logFolder = value;
                }
                catch
                {
                }
            }
        }

        /// <summary> Private method. Write message to log file </summary>
        /// <param name="message"> Message, that will writing to log file</param>
        private static void WriteMessageToLog(string message)
        {
            try
            {
                var fileName = Path.Combine(Application.StartupPath, Path.Combine(logFolder, logFileName)); // get actual filename for logfile
                using (var streamW = new StreamWriter(fileName, true, Encoding.Default)) // open stream for writing. using statement allow not use "reader.Close()".
                    streamW.WriteLine(message); // and save our message to file.
            }
            catch
            {
            }
        }

        /// <summary> Public method for posting message in logFile </summary>
        /// <param name="text">Message for saving</param>
        /// <param name="messageType">Message type (error, warning or information)</param>
        /// <param name="showMessage">Show messagebox to user</param>
        public static void EchoToLog(string text, EchoMessageType messageType, bool showMessage = false)
        {
            var date = DateTime.Now;
            WriteMessageToLog(date.ToLongDateString() + " " + date.ToLongTimeString() + " " + date.Millisecond + " " + // get current date and time
                              messageType.GetTitle() + " " + // get message type caption
                              text); // Such precision in time will facilitate program debugging, in case of errors

            if (showMessage)
                MessageBox.Show(text, messageType.GetTitle(), MessageBoxButtons.OK);
        }

        /// <summary> Write error to logFile </summary>
        /// <param name="ex">Occured exception</param>
        /// <param name="showMessage">Show messagebox to user</param>
        public static void EchoToLog(Exception ex, bool showMessage = false)
        {
            var date = DateTime.Now;
            WriteMessageToLog(date.ToLongDateString() + " " + date.ToLongTimeString() + " " + date.Millisecond + " " + // get current date and time   
                              EchoMessageType.Error + " " +
                              ex.Message +
                              (ex.InnerException == null || String.IsNullOrEmpty(ex.InnerException.Message) ? "" : "\n" + ex.InnerException.Message)); // get error message ( facilitate program debugging)

            if (showMessage)
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
        }

        #endregion

        #region ContolBoxBasedDialog

        public static DialogResult ShowDialog(Control control)
        {
            return ShowDialog(MainForm, control, "", MessageBoxButtons.OK, 0, false, true);
        }
        public static DialogResult ShowDialog(IWin32Window owner, Control control)
        {
            return ShowDialog(owner, control, "", MessageBoxButtons.OK, 0, false, true);
        }
        public static DialogResult ShowDialog(Control control, bool canResize)
        {
            return ShowDialog(MainForm, control, "", MessageBoxButtons.OK, 0, canResize, true);
        }
        public static DialogResult ShowDialog(Control control, bool canResize, bool needButtons)
        {
            return ShowDialog(MainForm, control, "", MessageBoxButtons.OK, 0, canResize, needButtons);
        }
        public static DialogResult ShowDialog(IWin32Window owner, Control control, bool canResize)
        {
            return ShowDialog(owner, control, "", MessageBoxButtons.OK, 0, canResize, true);
        }
        public static DialogResult ShowDialog(Control control, string caption)
        {
            return ShowDialog(MainForm, control, caption, MessageBoxButtons.OK, 0, false, true);
        }
        public static DialogResult ShowDialog(IWin32Window owner, Control control, string caption)
        {
            return ShowDialog(owner, control, caption, MessageBoxButtons.OK, 0, false, true);
        }
        public static DialogResult ShowDialog(Control control, string caption, bool canResize)
        {
            return ShowDialog(MainForm, control, caption, MessageBoxButtons.OK, 0, canResize, true);
        }
        public static DialogResult ShowDialog(Control control, string caption, bool canResize, bool useDefaultButtons)
        {
            if (useDefaultButtons)
                return ShowDialog(MainForm, control, caption, MessageBoxButtons.OK, 0, canResize, true);
            return ControlBox.Show(MainForm, control, caption, canResize);
        }
        public static DialogResult ShowDialog(IWin32Window owner, Control control, string caption, bool canResize, bool useDefaultButtons)
        {
            if (useDefaultButtons)
                return ShowDialog(MainForm, control, caption, MessageBoxButtons.OK, 0, canResize, true);
            return ControlBox.Show(MainForm, control, caption, canResize);
        }

        public static DialogResult ShowDialog(IWin32Window owner, Control control, string caption, bool canResize)
        {
            return ShowDialog(owner, control, caption, MessageBoxButtons.OK, 0, canResize, true);
        }
        public static DialogResult ShowDialog(Control control, MessageBoxButtons buttons)
        {
            return ShowDialog(MainForm, control, "", buttons, 0, false, true);
        }
        public static DialogResult ShowDialog(IWin32Window owner, Control control, MessageBoxButtons buttons)
        {
            return ShowDialog(owner, control, "", buttons, 0, false, true);
        }
        public static DialogResult ShowDialog(Control control, MessageBoxButtons buttons, bool canResize)
        {
            return ShowDialog(MainForm, control, "", buttons, 0, canResize, true);
        }
        public static DialogResult ShowDialog(IWin32Window owner, Control control, MessageBoxButtons buttons, bool canResize)
        {
            return ShowDialog(owner, control, "", buttons, 0, canResize, true);
        }
        public static DialogResult ShowDialog(Control control, string caption, MessageBoxButtons buttons)
        {
            return ShowDialog(MainForm, control, caption, buttons, 0, false, true);
        }
        public static DialogResult ShowDialog(IWin32Window owner, Control control, string caption, MessageBoxButtons buttons)
        {
            return ShowDialog(owner, control, caption, buttons, 0, false, true);
        }
        public static DialogResult ShowDialog(Control control, string caption, MessageBoxButtons buttons, bool canResize)
        {
            return ShowDialog(MainForm, control, caption, buttons, 0, canResize, true);
        }
        public static DialogResult ShowDialog(IWin32Window owner, Control control, string caption, MessageBoxButtons buttons, bool canResize)
        {
            return ShowDialog(owner, control, caption, buttons, 0, canResize, true);
        }
        public static DialogResult ShowDialog(Control control, MessageBoxButtons buttons, int defaultButton)
        {
            return ShowDialog(MainForm, control, "", buttons, defaultButton, false, true);
        }
        public static DialogResult ShowDialog(IWin32Window owner, Control control, MessageBoxButtons buttons, int defaultButton)
        {
            return ShowDialog(owner, control, "", buttons, defaultButton, false, true);
        }
        public static DialogResult ShowDialog(Control control, MessageBoxButtons buttons, int defaultButton, bool canResize)
        {
            return ShowDialog(MainForm, control, "", buttons, defaultButton, canResize, true);
        }
        public static DialogResult ShowDialog(IWin32Window owner, Control control, MessageBoxButtons buttons, int defaultButton, bool canResize)
        {
            return ShowDialog(owner, control, "", buttons, defaultButton, canResize, true);
        }
        public static DialogResult ShowDialog(Control control, string caption, MessageBoxButtons buttons, int defaultButton)
        {
            return ShowDialog(MainForm, control, caption, buttons, defaultButton, false, true);
        }
        public static DialogResult ShowDialog(IWin32Window owner, Control control, string caption, MessageBoxButtons buttons, int defaultButton)
        {
            return ShowDialog(owner, control, caption, buttons, defaultButton, false, true);
        }
        public static DialogResult ShowDialog(Control control, string caption, MessageBoxButtons buttons, int defaultButton, bool canResize)
        {
            return ShowDialog(MainForm, control, caption, buttons, defaultButton, canResize, true);
        }
        public static DialogResult ShowDialog(IWin32Window owner, Control control, string caption, MessageBoxButtons buttons, int defaultButton, bool canResize, bool needButtons)
        {
            return ControlBox.Show(owner, control, caption, buttons, defaultButton, canResize, needButtons);
        }

        public static void ShowFloatWindow(Control control, string caption, bool canResize, bool allowMaximize = false)
        {
            ControlBox.ShowFloatWindow(MainForm, control, caption, canResize, allowMaximize);
        }

        #endregion

        #region ProgressProc

        private static readonly List<ProgressProcEventHandler> progressProcs = new List<ProgressProcEventHandler>();

        public static event ProgressProcEventHandler ProgressProc
        {
            add
            {
                if (!progressProcs.Contains(value))
                    progressProcs.Add(value);
            }
            remove
            {
                if (progressProcs.Contains(value))
                    progressProcs.Remove(value);
            }
        }

        public static void Progress(object sender, ProgressProcEventArgs e)
        {
            Progress(sender, e.Status, e.PercentDone, e.SubProgressStatus, e.SubProgressPercentDone);
        }

        private static readonly object locker = new object();
        private static bool progressLocked = true;
        private static readonly Timer tmr = new Timer();

        private static void tmr_Elapsed(object sender, EventArgs e)
        {
            lock (locker)
                progressLocked = false;
        }

        public static void Progress(object sender, string status, double percentDone, string subProgressStatus, double subProgressPercentDone)
        {
            bool b;
            lock (locker)
                b = progressLocked;
            if (!b)
            {
                lock (locker)
                    progressLocked = true;
                callStackReleasedTimerLocked = true;
                try
                {
                    if (progressProcs.Count > 0)
                        progressProcs[progressProcs.Count - 1](sender, new ProgressProcEventArgs(status, percentDone, subProgressStatus, subProgressPercentDone));
                }
                finally
                {
                    callStackReleasedTimerLocked = false;
                }
            }
        }
        public static void Progress(string status, double percentDone, string subProgressStatus, double subProgressPercentDone)
        {
            Progress(MainForm, status, percentDone, subProgressStatus, subProgressPercentDone);
        }
        public static void Progress(object sender, string status, double percentDone)
        {
            Progress(sender, status, percentDone, null, 0);
        }
        public static void Progress(string status, double percentDone)
        {
            Progress(MainForm, status, percentDone);
        }
        public static void Progress(string status)
        {
            Progress(MainForm, status, 0);
        }

        private static EventHandler callStackReleasedHandlers;
        private static System.Windows.Forms.Timer callStackReleasedTimer;
        private static bool callStackReleasedTimerLocked;

        private static void releaseHandlers()
        {
            lock (locker)
            {
                var hs = callStackReleasedHandlers;
                callStackReleasedHandlers = null;
                hs?.Invoke(null, EventArgs.Empty);
            }
        }

        private static void OnTimerTick(object sender, EventArgs e)
        {
            if (callStackReleasedTimerLocked)
                return;
            callStackReleasedTimer.Stop();
            releaseHandlers();
        }

        public static void AddCallStackReleasedProc(EventHandler handler)
        {
            callStackReleasedHandlers -= handler;
            callStackReleasedHandlers += handler;
            if (callStackReleasedTimer == null)
            {
                callStackReleasedTimer = new System.Windows.Forms.Timer
                {
                    Interval = 10
                };
                callStackReleasedTimer.Tick += OnTimerTick;
            }
            callStackReleasedTimer.Start();
        }

        #endregion
    }
}
