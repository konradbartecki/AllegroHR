using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Me.Bartecki.Allegro.Infrastructure.Model
{
    public class AllegroApiException : Exception
    {

        public AllegroApiException(ErrorCodes error) : base()
        {
            this.ErrorCode = error;
        }

        public AllegroApiException(ErrorCodes error, string message) : base(message)
        {
            this.ErrorCode = error;
            //This exception is also used for user error reporting,
            //so we want to capture a stacktrace on construction instead of on throw
            //because this exception may never be thrown, but just returned to the client.
            var stackTraceField = typeof(AllegroApiException).BaseType
                .GetField("_stackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
            stackTraceField.SetValue(this, Environment.StackTrace);
        }

        public AllegroApiException(ErrorCodes error, string message, Exception innerException) : base(message, innerException)
        {
            this.ErrorCode = error;
            //This exception is also used for user error reporting,
            //so we want to capture a stacktrace on construction instead of on throw
            //because this exception may never be thrown, but just returned to the client.
            var stackTraceField = typeof(AllegroApiException).BaseType
                .GetField("_stackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
            stackTraceField.SetValue(this, Environment.StackTrace);
        }

        public ErrorCodes ErrorCode { get; }
    }
}
