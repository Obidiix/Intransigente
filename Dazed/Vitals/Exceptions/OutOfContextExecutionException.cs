using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dazed.Vitals.Exceptions
{
    /// <summary>
    /// Limits the path of execution from a function, this may be due to it being a function reserved for an editor's scope.
    /// </summary>
    public class OutOfContextExecutionException : Exception
    {
        public override string Message => "Current execution path leads to out of context functions.";
    }
}
