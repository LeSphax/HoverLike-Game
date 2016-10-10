
using System;

class UnhandledSwitchCaseException : Exception
{
    public UnhandledSwitchCaseException(object switchCase) : base("This switch case should be handled " + switchCase)
    {

    }
}

