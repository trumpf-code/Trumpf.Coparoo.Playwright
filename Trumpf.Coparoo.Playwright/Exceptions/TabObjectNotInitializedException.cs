using System;
using System.Collections.Generic;
using System.Text;

namespace Trumpf.Coparoo.Playwright.Exceptions
{
    /// <summary>
    /// TabObject not initialized exception
    /// </summary>
    public class TabObjectNotInitializedException : Exception
    {
        /// <summary>
        /// Represents an exception that is thrown when a tab object is accessed before it has been properly
        /// initialized.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TabObjectNotInitializedException(string message) : base(message) { }
    }
}
