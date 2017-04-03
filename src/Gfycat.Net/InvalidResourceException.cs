#pragma warning disable CS1591
using System;

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
