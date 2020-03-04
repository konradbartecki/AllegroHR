using System;
using System.Collections.Generic;
using System.Text;

namespace Me.Bartecki.Allegro.Infrastructure.Model
{
    public class ErrorMessage
    {
        public ErrorCodes ErrorCode { get; set; }
        public string Message { get; set; }

    }
}
