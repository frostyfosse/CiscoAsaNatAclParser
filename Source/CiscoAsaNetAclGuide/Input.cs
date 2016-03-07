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

        void openLogButton_Click(object sender, EventArgs e)
        {
            if (!File.Exists(_logFilePath))
                MessageBox.Show("There is no log activity to display yet.", "Log File", MessageBoxButtons.OK, MessageBoxIcon.Question);
            else
                Process.Start(_logFilePath);
        }

        void browseButton_Click(object sender, EventArgs e)
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

            if (result.Failed)
            {
                WriteAndRaiseFailedResult(result);
                return;
            }

            try
            {
                LogEventToFile(string.Format("Writing result to '{0}'.", SelectedOutputDirectory), StatusType.Information);

                //Object Network output
                var objectNetworkFilePath = Path.Combine(SelectedOutputDirectory, ObjectNetworkFileName);

                File.WriteAllLines(objectNetworkFilePath,
                                   result.GetCommaDelimitedResults(ParseResult.OutputResultType.ObjectNetwork));

                //Access Network output
                var accessListFilePath = Path.Combine(SelectedOutputDirectory, AccessListFilename);

                File.WriteAllLines(accessListFilePath,
                                   result.GetCommaDelimitedResults(ParseResult.OutputResultType.AccessList));

                //Opening Directory
                Process.Start(SelectedOutputDirectory);

                //Opening files
                //Process.Start(objectNetworkFilePath);
                //Process.Start(accessListFilePath);

                RaiseCompleteStatus(objectNetworkFilePath, accessListFilePath);
            }
            catch (Exception e)
            {
                var title = "An error occurred while writing the results to file.";
                var logDetail = string.Format("{0} {1}. {2}", DateTime.Now, title, e);

                LogEventToFile(string.Join(" ", title, logDetail), StatusType.Error);
                RaiseErrorStatus(title, true);
            }
        }

        void WriteAndRaiseFailedResult(ParseResult result)
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(result.Title))
                builder.AppendLine(result.Title);
            if (result.Messages != null)
                result.Messages.ForEach(x => builder.AppendLine(x));

            LogEventToFile(builder.ToString(), StatusType.Error);
            RaiseErrorStatus(result.Title, true);
        }

        //Not something the user wants
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

                result.Failed = true;
                result.Title = title;
                result.Messages.Add(e.ToString());
            }

            return result;
        }

        #region Status related
        void RaiseCompleteStatus(string objectNetworkFilePath, string accessListFilePath)
        {
            UpdateStatus(string.Format("Complete. Results written to '{0}'{1}and {2}.", objectNetworkFilePath,
                                                                                        Environment.NewLine,
                                                                                        accessListFilePath), 
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
    }
}
