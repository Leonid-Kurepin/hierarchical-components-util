using System;

namespace ASKON_TestTask.Common.Exceptions
{
    public class BusinessLogicException : Exception
    {
        public BusinessLogicException(string message)
            : base(message)
        {
        }
    }
}
