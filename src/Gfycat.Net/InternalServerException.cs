#pragma warning disable CS1591
using System;
using System.Net.Http;
using System.Text;

namespace Gfycat
{
    /// <summary>
    /// Represents an internal Gfycat server error with message, type, and stack trace
    /// </summary>
    public class InternalServerException : Exception
    {
        public string ErrorMessage { get; }

        public string ErrorType { get; }

        public string[] ServerStackTrace { get; }

        public Uri RequestUri { get; }

        public HttpMethod RequestMethod { get; }

        internal InternalServerException(string message, string type, string[] stackTrace, HttpMethod method, Uri requestUri) : base($"{type} : {message}")
        {
            ErrorMessage = message;
            ErrorType = type;
            ServerStackTrace = stackTrace;
            RequestMethod = method;
            RequestUri = requestUri;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder($"{ErrorType} on {RequestMethod} at {RequestUri.PathAndQuery}: {ErrorMessage}\n");
            foreach (string stackFrame in ServerStackTrace)
                builder.AppendLine($"\t at {stackFrame}");
            return builder.ToString();
        }
    }
}
