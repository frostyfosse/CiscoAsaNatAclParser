﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CiscoAsaNetAclParser
{
    public partial class Input : Form
    {
        public const string FormName = "Cisco Asa Parser";

        public Input()
        {
            InitializeComponent();
            Directory.CreateDirectory(_defaultOutputDirectory);
            Directory.CreateDirectory(_logFileDirectory);
            accessListFilenameLabel2.Text = AccessListFilename;
            objectNetworkFilenameLabel2.Text = ObjectNetworkFileName;
            SelectedOutputDirectory = _defaultOutputDirectory;
            outputPath.Text = SelectedOutputDirectory;

        }

        static string FileDateFormat = string.Join("", DateTime.Now.Year, 
                                                       DateTime.Now.Month.ToString().PadLeft(2, '0'), 
                                                       DateTime.Now.Day.ToString().PadLeft(2, '0'));
        static string _defaultFilename = string.Format("{0}{1}", FormName.Replace(" ", ""), FileDateFormat);
        static string _defaultXmlFilename = string.Format("{0}{1}.xml", FormName.Replace(" ", ""), FileDateFormat);
        static string _defaultOutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Output");
        public static string ObjectNetworkFileName = string.Join("", _defaultFilename, "_", typeof(ObjectNetwork).Name, ".csv");
        public static string AccessListFilename = string.Join("", _defaultFilename, "_", typeof(AccessList).Name, ".csv");

        static string _defaultOutputPath = Path.Combine(_defaultOutputDirectory, _defaultFilename);
        static string _logFileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        static string _logFilename = string.Join("", FormName, "_", FileDateFormat, ".log");
        static string _logFilePath = Path.Combine(_logFileDirectory, _logFilename);

        public string SelectedOutputDirectory { get; private set; }

        enum MainMenuOption
        {
            [Description("Open Log File")]
            OpenLogFile,
            [Description("Open Output Directory")]
            OpenOutputDirectory,
            [Description("Close")]
            Close
        }

        #region event methods
        void ClearDataInWindow(object sender, EventArgs e)
        {
            logContent.Clear();
        }

        void GenerateOutputFiles(object sender, EventArgs e)
        {
            var value = logContent.Text;

            if (value is string)
            {
                ParseData(value);
            }
            else
            {
                RaiseErrorStatus(null, "Generate failed. Must contain only text.", StatusType.Error);
            }
        }

        void DataInWindowChangedEvent(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(logContent.Text))
                UpdateEnabledStatus(false);
            else
                UpdateEnabledStatus(true);

            UpdateStatus(null, StatusType.None, Color.Black);
        }

        void UpdateEnabledStatus(bool enabled)
        {
            generateOutputMenu.Enabled = enabled;
            clearWindowMenu.Enabled = enabled;
            CiscoAsaNetAclParser.Properties.Settings.Default.EnableButtons = enabled;
        }

        void OpenLogFile(object sender, EventArgs e)
        {
            if (!File.Exists(_logFilePath))
                MessageBox.Show("There is no log activity to display yet.", "Log File", MessageBoxButtons.OK, MessageBoxIcon.Question);
            else
                Process.Start(_logFilePath);
        }

        void BrowseFoldersForOutputDirectory(object sender, EventArgs e)
        {
            var browser = new FolderBrowserDialog()
            {
                Description = "Browse for a location to store the output files.",
                ShowNewFolderButton = true,
                SelectedPath = _defaultOutputDirectory
            };

            browser.ShowDialog();

            SelectedOutputDirectory = browser.SelectedPath;
            outputPath.Text = SelectedOutputDirectory;
        }
        #endregion

        void ParseData(string data)
        {
            if (string.IsNullOrEmpty(outputPath.Text))
            {
                RaiseWarningStatus("The output path cannot be empty.");
                return;
            }

            var textArray = data.Split(new[] { "\r\n","\n" }, StringSplitOptions.RemoveEmptyEntries);

            ParseResult result = ManageParser(textArray);

            switch (result.CompletionSeverity)
            {
                case ResultSeverity.Warnings:
                    WriteAndRaiseFailedResult(result, StatusType.Warning);
                    break;
                case ResultSeverity.Errors:
                    WriteAndRaiseFailedResult(result, StatusType.Error);
                    return;
                default:
                    break;
            }

            try
            {
                LogEventToFile(string.Format("Writing result to '{0}'.", SelectedOutputDirectory), StatusType.Information);

                //Object Network output
                File.WriteAllLines(Path.Combine(SelectedOutputDirectory, ObjectNetworkFileName),
                                   result.GetCommaDelimitedResults(ParseResult.OutputResultType.ObjectNetwork));

                //Access Network output
                File.WriteAllLines(Path.Combine(SelectedOutputDirectory, AccessListFilename),
                                   result.GetCommaDelimitedResults(ParseResult.OutputResultType.AccessList));

                //Opening files
                //Process.Start(objectNetworkFilePath);
                //Process.Start(accessListFilePath);



                switch (result.CompletionSeverity)
                {
                    case ResultSeverity.Errors:
                    case ResultSeverity.Warnings:
                        RaiseCompleteStatus(ObjectNetworkFileName, AccessListFilename, string.Format("There were {0} {1}.", result.Messages.Count(), result.CompletionSeverity.ToString()));
                        break;
                    default:
                        RaiseCompleteStatus(ObjectNetworkFileName, AccessListFilename);
                        break;
                }
                
                //Opening Directory
                //Process.Start(SelectedOutputDirectory);
                Process.Start(Path.Combine(SelectedOutputDirectory, AccessListFilename));
                Process.Start(Path.Combine(SelectedOutputDirectory, ObjectNetworkFileName));
            }
            catch (Exception e)
            {
                var title = "An error occurred while writing the results to file.";
                var logDetail = string.Format("{0} {1}. {2}", DateTime.Now, title, e);

                LogEventToFile(string.Join(" ", title, logDetail), StatusType.Error);
                RaiseErrorStatus(result, title, StatusType.Error, true);
            }
        }

        void WriteAndRaiseFailedResult(ParseResult result, StatusType status)
        {
            var logs = new List<string>();

            if (!string.IsNullOrEmpty(result.Title))
                logs.Add(result.Title);
            if (result.Messages != null)
                logs.AddRange(result.Messages);

            LogEventToFile(logs, status);

            switch (status)
            {
                case StatusType.Warning:
                    RaiseErrorStatus(result, result.Title, status, true);
                    break;
                case StatusType.Error:
                    RaiseErrorStatus(result, result.Title, status, true);
                    break;
            }
        }

        //Not something the user wants, but wanted to keep around
        void WriteOutputToXml(ParseResult result, string xmlFilePath, string csvFilePath)
        {
            var serializer = new XmlSerializer(result.ObjectNetworkResults.GetType(),
                                               new XmlRootAttribute(string.Join("", typeof(ObjectNetwork).Name, "s")));
            var path = File.Create(xmlFilePath);
            serializer.Serialize(path, result.ObjectNetworkResults);
            path.Close();
        }

        ParseResult ManageParser(string[] textArray)
        {
            var parser = new Parser();
            ParseResult result = new ParseResult();

            try
            {
                result = parser.Parse(textArray);
            }
            catch (Exception e)
            {
                var title = "An unexpected error occurred while parsing the data.";

                //LogEventToFile(string.Join(" ", title, e), StatusType.Error);
                //RaiseErrorStatus(string.Join(" ", title, "Please see error logs for details."));

                result.CompletionSeverity = ResultSeverity.Errors;
                result.Title = title;
                result.Messages.Add(e.ToString());
            }

            return result;
        }

        #region Status related
        void RaiseCompleteStatus(string objectNetworkFilePath, string accessListFilePath, string disclaimers = null)
        {
            var files = string.Join(Environment.NewLine, objectNetworkFilePath, accessListFilePath);
            var mainMessage = string.Format("Complete. Results written to the following files:{0}{1}.", Environment.NewLine,
                                                                                              files);
            var fullMessage = mainMessage;

            //if (!string.IsNullOrEmpty(disclaimers))
            //    fullMessage = string.Join(Environment.NewLine, mainMessage, disclaimers);

            UpdateStatus(fullMessage,
                         StatusType.Complete, 
                         Color.Blue);
        }

        void RaiseInfoStatus(string text)
        {
            UpdateStatus(text, StatusType.Information, Color.Blue);
        }

        void RaiseWarningStatus(string text)
        {
            UpdateStatus(text, StatusType.Warning, Color.OrangeRed);
        }

        void RaiseErrorStatus(ParseResult result, string text, StatusType status, bool referenceLogFile = false)
        {
            var message = text;
            string errorsMessage = "Please see the log file for existing warnings and / or errors.";

            if (result != null && result.Messages.Count() > 0)
                errorsMessage = string.Format("Please see the log file. There were {0} {1}.", result.Messages.Count(), result.CompletionSeverity);

            var logFileReferenceIncluded = string.Format("{0}. {1}", text, errorsMessage);

            Color color;

            if (referenceLogFile)
                message = logFileReferenceIncluded;

            switch (status)
            {
                case StatusType.Warning:
                    color = Color.OrangeRed;
                    break;
                default:
                case StatusType.Error:
                    color = Color.Red;
                    break;
            }

            UpdateStatus(message, status, color);
        }

        void UpdateStatus(string messageText, StatusType statusType, Color foregroundColor)
        {
            this.status.ForeColor = foregroundColor;

            if (statusType == StatusType.Information || statusType == StatusType.None)
                status.Text = null;
            else
                this.status.Text = statusType.ToString();

            if (!string.IsNullOrEmpty(messageText))
                MessageBox.Show(messageText, statusType.ToString());
        }
        #endregion

        enum StatusType
        {
            Information,
            Warning,
            Error,
            Complete,
            None
        }

        void LogEventToFile(string text, StatusType statusType)
        {
            File.AppendAllLines(_logFilePath, new[] { string.Format("{0} {1} - {2}", DateTime.Now, statusType.ToString(), text) });
        }

        void LogEventToFile(IEnumerable<string> lines, StatusType statusType)
        {
            var logs = new List<string>();
            foreach (var line in lines)
            {
                var log = string.Format("{0} {1} - {2}", DateTime.Now, statusType.ToString(), line);
                logs.Add(log);
            }

            File.AppendAllLines(_logFilePath, logs);
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        void OpenOutputDirectory(object sender, EventArgs e)
        {
            Process.Start(SelectedOutputDirectory);
        }

        void CloseApp(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
