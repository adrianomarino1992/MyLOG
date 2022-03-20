using System;
using System.Collections.Generic;
using System.Net.Http;


namespace MyLog.WS
{
    /// <summary>
    /// Enum that defines a type of log
    /// </summary>
    [Flags]
    public enum LogsType
    {
        /// <summary>
        /// Defines to save log only in WS
        /// </summary>
        WS = 1, //only WS
        /// <summary>
        /// Defines to save log only in files
        /// </summary>
        FILE = 2, //only files
        /// <summary>
        /// Define to save log in both, WS and file
        /// </summary>
        WSANDFILE = WS | FILE //both 
    }

    /// <summary>
    /// Async delegate to handle the HTTP call callback
    /// </summary>
    /// <param name="message"></param>
    public delegate void AsyncWSCallback(HttpResponseMessage message);

    /// <summary>
    /// A delegate to show how to serializer the <see cref="WSObject"/>, this way allow to send JSON or XML data
    /// </summary>
    /// <param name="obj">The <see cref="WSObject"/> witch will be send</param>
    /// <returns>Returns the <see cref="WSObject"/> serialized as JSON or XML string</returns>
    public delegate string WSSerializer(WSObject obj);

    /// <summary>
    /// Delegate to handle the WS error 
    /// </summary>
    /// <param name="ex"></param>
    public delegate void WSErrorHandler(Exception ex);

    /// <summary>
    /// Classes to encapsulates WS configurations
    /// </summary>
    public class LogWSConfig
    {        
        /// <summary>
        /// The Universal Resource Identifier of WS
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// A list of headers to attach on POST request
        /// </summary>
        public List<KeyValuePair<string, string>> Headers { get; set; } = new List<KeyValuePair<string, string>>();
        /// <summary>
        /// A callback function to handler the WS response
        /// </summary>
        public AsyncWSCallback Callback { get; set; }
        /// <summary>
        /// The encoding of data
        /// </summary>
        public System.Text.Encoding Encoding { get; set; }
        /// <summary>
        /// The way that content will serialized, e.g JSON or XML
        /// </summary>
        public WSSerializer Serializer { get; set; }
        /// <summary>
        /// A function to handler the error of process that send the request to WS
        /// </summary>
        public WSErrorHandler ErrorHandler { get; set; }
        /// <summary>
        /// The media type of content serialized
        /// </summary>
        public string MediaType { get; set; }

    }

    /// <summary>
    /// Classe that encapsulates a HTTP content
    /// </summary>
    public class WSObject
    {
        /// <summary>
        /// The date of log must be marked
        /// </summary>
        public DateTime Date { get; set; }   
        /// <summary>
        /// The list of logs
        /// </summary>
        public List<string> Logs { get; set; }
        /// <summary>
        /// A empty constructor to help with serialization with reflections
        /// </summary>
        public WSObject()
        {

        }
    }

}
