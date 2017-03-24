using System;
using System.Collections.Generic;
using System.Text;

namespace Gfycat
{
    /// <summary>
    /// Represents the error that occurs when requesting a non-existing method/resource with a valid token
    /// </summary>
    public class InvalidResourceException : Exception
    {
        public InvalidResourceException() : base() { }
        public InvalidResourceException(string message) : base(message) { }
    }
}
