using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Net.Http;
using MyLog.WS;

/// <summary>
/// Namespace default of MyLog
/// </summary>
namespace MyLog
{
    /// <summary>
    /// The static class that manage all logs in the system
    /// </summary>
    /// <typeparam name="T">A identify type for the logs objects</typeparam>
    public class TypeOf<T>
    {
        /// <summary>
        /// The type of delegate that is called to keep the defaults properties and the instance properties synchronized
        /// </summary>
        /// <param name="propertyCaller">Do not pass this argument in some override, because it will come from CLR</param>
        protected delegate void _propChange([System.Runtime.CompilerServices.CallerMemberName] string propertyCaller = "");
        /// <summary>
        /// The event to synchronize all properties
        /// </summary>
        protected event _propChange OnPropertyChange;
        /// <summary>
        /// The folder to save the files
        /// </summary>
        protected string _folder;

        /// <summary>
        /// Default folder witch the files will be saved
        /// </summary>
        public string DefaultFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_folder) || !Directory.Exists(_folder))
                    _folder = AppDomain.CurrentDomain.BaseDirectory;

                return _folder;
            }
            set
            {
                if (!Directory.Exists(value))
                    Directory.CreateDirectory(value);

                _folder = value;

                this.OnPropertyChange();
            }
        }

        /// <summary>
        /// Some text to be added to the beginning of file name 
        /// </summary>
        protected string _defaultFileNamePrefix;

        /// <summary>
        /// Some text to be added to the beginning of file name 
        /// </summary>
        public string DefaultFileNamePrefix
        {
            get
            {
                if (string.IsNullOrEmpty(_folder))
                    _defaultFileNamePrefix = "";

                return _defaultFileNamePrefix;
            }
            set
            {
                _defaultFileNamePrefix = value.RemoveSpecialChars();

                this.OnPropertyChange();
            }
        }

        /// <summary>
        /// Max quantity of files will be keeped in the folder, Atention !, not is taken in account the identifier of object, but the number of files in folder        
        /// </summary>
        protected int _maxFiles;

        /// <summary>
        /// Max quantity of files will be keeped in the folder, Atention !, not is taken in account the identifier of object, but the number of files in folder        
        /// </summary>
        public int DefaultMaxFiles
        {
            get
            {
                return _maxFiles | 1;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("O valor maximo de arquivo não deve ser igual ou menor que zero");

                _maxFiles = value;

                this.OnPropertyChange();

            }
        }

        /// <summary>
        /// The default size of llg files
        /// </summary>
        protected int _fileSize;

        /// <summary>
        /// The default size of llg files in KB
        /// </summary>
        public int DefaultFileSize
        {
            get
            {
                return _fileSize | 1;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("O valor maximo de arquivo não deve ser igual ou menor que zero");

                _fileSize = value;

                this.OnPropertyChange();

            }
        }

        /// <summary>
        /// Defines if the <see cref="System.Diagnostics.StackTrace"/> must be writen too
        /// </summary>
        protected bool _fullStack;

        /// <summary>
        /// Defines if the <see cref="System.Diagnostics.StackTrace"/> must be writen too
        /// </summary>
        public bool DefaultFullStack
        {
            get
            {
                return _fullStack;
            }
            set
            {
                _fullStack = value;

                this.OnPropertyChange();

            }
        }

        /// <summary>
        /// Defines the type of log
        /// </summary>
        protected LogsType _logsType = LogsType.FILE;

        /// <summary>
        /// Defines the type of log
        /// </summary>
        public LogsType DefaultLogType
        {
            get
            {
                return _logsType;
            }
            set
            {
                _logsType = value;

                this.OnPropertyChange();

            }
        }

        /// <summary>
        /// This object contains information of how to handler the WS Log
        /// </summary>
        protected LogWSConfig _wsConfig;

        /// <summary>
        /// This object contains information of how to handler the WS Log
        /// </summary>
        public LogWSConfig DefaultWSConfig
        {
            get
            {
                return _wsConfig;
            }
            set
            {
                _wsConfig = value;

                this.OnPropertyChange();

            }
        }

        /// <summary>
        /// The binding context of objects
        /// </summary>
        protected List<KeyValuePair<T, Writer>> _logs = new List<KeyValuePair<T, Writer>>();
        /// <summary>
        /// The instance of class to be possible we have a singleton unity
        /// </summary>
        protected static TypeOf<T> _instance = null;

        /// <summary>
        /// A log object collection
        /// Pass a value of the identifier type to retrive a log object, if its dont exists, will be created         
        /// </summary>
        public static TypeOf<T> Collection
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TypeOf<T>();

                    _instance.OnPropertyChange += propertyCaller => {

                        if (_instance._logs.Count > 0)
                        {
                            _instance._logs.ForEach(b =>
                            {
                                foreach (PropertyInfo propertyInfo in b.Value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                                {
                                    if (propertyInfo.Name == propertyCaller.Replace("Default", ""))
                                    {
                                        if (propertyInfo.SetMethod != null)
                                            propertyInfo.SetValue(b.Value, _instance.GetType().GetProperty(propertyCaller).GetValue(_instance));
                                    }
                                }
                            });
                        }
                    };
                }

                return _instance;
            }
        }


        /// <summary>
        /// Inform the identifier
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Returns a log object</returns>
        public Writer this[T key]
        {
            get
            {
                if (_logs.Any(l => l.Key.Equals(key)))
                    return _logs.First(l => l.Key.Equals(key)).Value;
                else
                {
                    if (!Directory.Exists(DefaultFolder))
                        DefaultFolder = AppDomain.CurrentDomain.BaseDirectory;

                    if (DefaultMaxFiles <= 0)
                        DefaultMaxFiles = 1;

                    if (DefaultFileSize <= 0)
                        DefaultFileSize = 100;

                    Writer newLog = new Writer(this.DefaultFolder, this.DefaultMaxFiles, this.DefaultFileSize, this.DefaultFullStack, this.DefaultLogType, this.DefaultWSConfig, this.DefaultFileNamePrefix);
                    _logs.Add(new KeyValuePair<T, Writer>(key, newLog));
                    return newLog;
                }
            }
            set
            {
                if (_logs.Any(l => l.Key.Equals(key)))
                {
                    KeyValuePair<T, Writer> previous = _logs.First(l => l.Key.Equals(key));
                    previous = new KeyValuePair<T, Writer>(key, value);

                }

            }
        }

    }

    /// <summary>
    /// This class encapsulates the log writer
    /// </summary>
    public class Writer
    {
        /// <summary>
        /// The folder witch the files will be saved
        /// </summary>
        protected string _folder;

        /// <summary>
        /// The folder witch the files will be saved
        /// </summary>
        public string Folder
        {
            get
            {
                if (string.IsNullOrEmpty(_folder) || !Directory.Exists(_folder))
                    _folder = AppDomain.CurrentDomain.BaseDirectory;

                return _folder;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _folder = AppDomain.CurrentDomain.BaseDirectory;
                }
                else
                {
                    if (!Directory.Exists(value))
                        Directory.CreateDirectory(value);

                    _folder = value;
                }
            }
        }

        /// <summary>
        /// Some text to be added to the beginning of file name 
        /// </summary>
        public string FileNamePrefix { get; set; }

        /// <summary>
        /// The quantity of files in folder
        /// </summary>
        public int MaxFiles { get; set; }

        /// <summary>
        /// The file´s sizes in KB
        /// </summary>
        public int FileSize { get; set; }

        private bool _tempStackTrace;

        /// <summary>
        /// Define if the log will write information about the StackTrace
        /// </summary>
        public bool FullStack { get; set; }

        /// <summary>
        /// Defines the type of log
        /// </summary>
        public LogsType LogType { get; set; }

        /// <summary>
        /// This object contains information of how to handler the WS Log
        /// </summary>
        public LogWSConfig WSConfig { get; set; }

        /// <summary>
        /// The extension of logs files
        /// </summary>
        protected string _ext = ".llg";

        /// <summary>
        /// This method is used to write the log, the optional params, if not informed, will bet get from Common Language Runtime in runtime, thus making this informations avaliable in a future       
        /// </summary>
        /// <param name="caller">The method who called the logger</param>
        /// <param name="line">The line of code that the logger is called from</param>
        /// <param name="path">The code´s path</param>
        /// <param name="message">All logs message</param>
        public void WriteLog([CallerMemberName] string caller = "", [CallerLineNumber] int line = 0, [CallerFilePath] string path = "", params string[] message)
        {
            //get the next available file
            FileInfo fileToAppend = this._GetNextFile();

            string enfase = "|||||||||||||||||||||||||||||||||"; //some mark text used only in FILE type

            string header = $"{enfase}{DateTime.Now.Day}/{DateTime.Now.Month}/{DateTime.Now.Year} {DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}:{DateTime.Now.Millisecond}{enfase}"; //get header text

            List<string> linhas = new List<string>
            {
                header,
                $"MODULO: {path}",
                $"METODO: {caller}",
                $"LINHA: {line}",
            }; //write the log´s lines

            message.ToList().ForEach(m => { linhas.Add($"MENSAGEM:{m}"); });


            if (this.FullStack || _tempStackTrace)
            {
                linhas.Add("STACKTRACE:");
                System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(true);

                foreach (System.Diagnostics.StackFrame stackFrame in stackTrace.GetFrames())
                {
                    try
                    {
                        if (stackFrame != null)
                        {
                            string stackFileName = stackFrame.GetFileName();

                            if (stackFileName != null && stackFileName.Length > 0)
                                linhas.Add($"MODULE: {stackFileName} -> METHOD: {stackFrame.GetMethod()} -> LINE:COLUMN: {stackFrame.GetFileLineNumber()}:{stackFrame.GetFileColumnNumber()}");

                        }
                    }
                    catch { }
                }

                _tempStackTrace = false;
            }

            linhas.Add("\r\n");

            if ((this.LogType & LogsType.FILE) > 0) // if is to write in file
            {
                File.AppendAllLines(fileToAppend.FullName, linhas); // append
            }

            if ((this.LogType & LogsType.WS) > 0 && this.WSConfig != null) //if is to send to WS
            {

                _ = System.Threading.Tasks.Task.Run(async () => //create a task and run asyncronous
                {
                    try
                    {
                        using (HttpClient httpClient = new HttpClient())
                        {
                            WSObject wSObject = new WSObject()
                            {
                                Date = DateTime.Now,
                                Logs = linhas.GetRange(1, linhas.Count - 2)
                            };

                            HttpContent content = new StringContent(WSConfig.Serializer?.Invoke(wSObject), WSConfig.Encoding, WSConfig.MediaType);

                            foreach (KeyValuePair<string, string> httpHeader in WSConfig.Headers)
                            {
                                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(httpHeader.Key, httpHeader.Value);
                            }

                            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(WSConfig.URL, content);
                            WSConfig.Callback?.Invoke(httpResponseMessage);

                        }

                    }
                    catch (Exception ex)
                    {
                        WSConfig.ErrorHandler?.Invoke(ex);
                    }

                });
            }
        }

        /// <summary>
        /// Defines if is to save the full stack trace is this log write
        /// </summary>
        /// <param name="useStack"></param>
        /// <returns></returns>
        public Writer WriteWithStackTrace(bool useStack = true)
        {
            this._tempStackTrace = useStack;
            return this;
        }

        /// <summary>
        /// Get the next file avaliable. 
        /// This method will make the control of files.
        /// </summary>
        /// <returns>Returns the file that must be write</returns>
        protected virtual FileInfo _GetNextFile()
        {
            if (!Directory.Exists(this.Folder))
                Directory.CreateDirectory(this.Folder);

            List<FileInfo> fileInfoList = new List<FileInfo>();
            foreach (string filePath in Directory.GetFiles(Folder).Where(d => d.EndsWith(this._ext)))
            {
                fileInfoList.Add(new FileInfo(filePath));
            }

            if (fileInfoList.Any(d => d.Length / 1024 < this.FileSize))
            {
                FileInfo currentFile = fileInfoList.OrderByDescending(d => d.LastWriteTime).First();

                return currentFile;
            }
            else
            {
                if (fileInfoList.Count >= MaxFiles)
                {
                    List<FileInfo> fileToKeep = fileInfoList.OrderByDescending(d => d.LastWriteTime).Take(MaxFiles - 1).ToList();

                    List<FileInfo> filesToDelete = fileInfoList.Where(d => !fileToKeep.Contains(d)).ToList();

                    fileInfoList.RemoveAll(d => filesToDelete.Contains(d));

                    filesToDelete.ForEach(d =>
                    {
                        try
                        {
                            File.Delete(d.FullName);
                        }
                        catch
                        {

                        }
                    });
                }

                FileInfo newFile = new FileInfo(Path.Combine(this.Folder, $"{this.FileNamePrefix.RemoveSpecialChars()}_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}_log{this._ext}"));
                return newFile;
            }
        }

        /// <summary>
        /// Create a log object
        /// </summary>
        /// <param name="folder">The folder witch the files will be saved</param>
        /// <param name="maxFiles">The max quantity of files that will be keeped </param>
        /// <param name="fileSize">The max size of files</param>
        /// <param name="fullStack">If the <see cref="System.Diagnostics.StackTrace"/> must be writed</param>
        /// <param name="logType">The type of log</param>
        /// <param name="wsConfig">Configurations to connect to WS</param>
        /// <param name="fileNamePrefix">Prefix of logs files</param>
        public Writer(string folder, int maxFiles, int fileSize, bool fullStack = false, LogsType logType = LogsType.FILE, WS.LogWSConfig wsConfig = null, string fileNamePrefix = "")
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            if (maxFiles <= 0)
                throw new ArgumentException("The max number of files must be equals or greater than zero");

            if (fileSize <= 0)
                throw new ArgumentException("The max size of files must be equals or greater than zero KBs");

            this.Folder = folder;
            this.MaxFiles = maxFiles;
            this.FileSize = fileSize;
            this.FullStack = fullStack;
            this.LogType = logType;
            this.WSConfig = wsConfig;
            this.FileNamePrefix = fileNamePrefix.RemoveSpecialChars();
        }
    }

    /// <summary>
    /// Helper class
    /// </summary>
    internal static class Ext
    {
        /// <summary>
        /// Remove all specials chars of a string
        /// </summary>
        /// <param name="strIn">The input string</param>
        /// <returns>The formated string</returns>
        public static string RemoveSpecialChars(this string strIn)
        {
            if (string.IsNullOrEmpty(strIn))
                return string.Empty;

            System.Text.StringBuilder formatedString = new System.Text.StringBuilder();
            foreach (char position in strIn)
            {
                if (
                       ((position >= 'a' && position <= 'z')
                    || (position >= 'A' && position <= 'Z')
                    || (position >= '0' && position <= '9'))
                        || position == '_'
                    )
                {
                    formatedString.Append(position);
                }
            }

            return formatedString.ToString();
        }
    }

}
