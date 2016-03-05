using System;
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
            outputPath.Text = _defaultOutputPath;
            Directory.CreateDirectory(_defaultOutputDirectory);
            Directory.CreateDirectory(_logFileDirectory);
        }

        static string FileDateFormat = string.Join("", DateTime.Now.Year, 
                                                       DateTime.Now.Month.ToString().PadLeft(2, '0'), 
                                                       DateTime.Now.Day.ToString().PadLeft(2, '0'));
        static string _defaultFilename = string.Format("{0}{1}.csv", FormName.Replace(" ", ""), FileDateFormat);
        static string _defaultXmlFilename = string.Format("{0}{1}.xml", FormName.Replace(" ", ""), FileDateFormat);
        static string _defaultOutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Output");
        static string _defaultOutputPath = Path.Combine(_defaultOutputDirectory, _defaultFilename);
        static string _logFileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        static string _logFilename = string.Join("", FormName, "_", FileDateFormat, ".log");
        static string _logFilePath = Path.Combine(_logFileDirectory, _logFilename);

        #region event methods
        void clearButton_Click(object sender, EventArgs e)
        {
            logContent.Clear();
        }

        void parseButton_Click(object sender, EventArgs e)
        {
            var value = logContent.Text;

            if (value is string)
            {
                ParseData(value);
            }
            else
            {
                RaiseErrorStatus("Generate failed. Must contain only text.");
            }
        }

        void logContent_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(logContent.Text))
            {
                CiscoAsaNetAclParser.Properties.Settings.Default.EnableButtons = false;
            }
            else
                CiscoAsaNetAclParser.Properties.Settings.Default.EnableButtons = true;

            UpdateStatus(null, StatusType.None, Color.Black);
        }
        #endregion

        void ParseData(string data)
        {
            if (string.IsNullOrEmpty(outputPath.Text))
            {
                RaiseInfoStatus("The output path cannot be empty.");
                return;
            }

            var textArray = data.Split(new[] { "\r\n","\n" }, StringSplitOptions.RemoveEmptyEntries);

            ParseResult result = ManageParser(textArray);

            if (result.Failed)
            {
                var builder = new StringBuilder();

                if (!string.IsNullOrEmpty(result.Title))
                    builder.AppendLine(result.Title);
                if (result.Messages != null)
                    result.Messages.ForEach(x => builder.AppendLine(x));

                LogEventToFile(builder.ToString(), StatusType.Error);
                RaiseErrorStatus(result.Title, true);

                return;
            }

            try
            {
                LogEventToFile(string.Format("Writing result to '{0}'.", outputPath.Text), StatusType.Information);

                var directory = Path.GetDirectoryName(outputPath.Text);
                var xmlFilePath = Path.Combine(directory, _defaultXmlFilename);
                var csvFilePath = outputPath.Text;

                //CSV File
                var linesForFile = result.GetCommaDelimitedResults();
                File.WriteAllLines(outputPath.Text, linesForFile);

                //XML File
                var serializer = new XmlSerializer(result.Results.GetType(),
                                                   new XmlRootAttribute(string.Join("", typeof(ObjectNetwork).Name, "s")));
                var path = File.Create(xmlFilePath);
                serializer.Serialize(path, result.Results);
                path.Close();
                RaiseCompleteStatus(csvFilePath, xmlFilePath);

                Process.Start(outputPath.Text);
            }
            catch(Exception e)
            {
                var title = "An error occurred while writing the results to file.";
                var logDetail = string.Format("{0} {1}. {2}", DateTime.Now, title, e);

                LogEventToFile(string.Join(" ", title, logDetail), StatusType.Error);
                RaiseErrorStatus(title, true);
            }
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

                result.Failed = true;
                result.Title = title;
                result.Messages.Add(e.ToString());
            }

            return result;
        }

        void browseButton_Click(object sender, EventArgs e)
        {
            var browser = new FolderBrowserDialog()
            {
                Description = "Browse for a location to store the output file.",
                ShowNewFolderButton = true
            };

            browser.ShowDialog();

            _defaultOutputPath = browser.SelectedPath;
        }

        #region Status related
        void RaiseCompleteStatus(string csvFile, string xmlFile)
        {
            UpdateStatus(string.Format("Complete. Results written to '{0}'{1}and {2}.", csvFile,
                                                                                        Environment.NewLine,
                                                                                        xmlFile), 
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

        void RaiseErrorStatus(string text, bool referenceLogFile = false)
        {
            var message = text;
            var logFileReferenceIncluded = string.Join(" ", text, "Please see the log file for details.");

            if (referenceLogFile)
                message = logFileReferenceIncluded;

            UpdateStatus(message, StatusType.Error, Color.Red);
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
            File.AppendAllText(_logFilePath, string.Format("{0} {1} - {2}", DateTime.Now, statusType.ToString(), text));
        }

        private void openLogButton_Click(object sender, EventArgs e)
        {
            Process.Start(_logFilePath);
        }
    }
}
