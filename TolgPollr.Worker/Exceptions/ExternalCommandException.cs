using System;

namespace TolgPollr.Worker.Exceptions
{
    public class ExternalCommandException : Exception
    {
        public ExternalCommandException(string error) : base(error) { }
    }
}