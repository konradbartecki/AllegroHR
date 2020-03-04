using System;
using System.Collections.Generic;
using System.Text;

namespace Me.Bartecki.Allegro.Infrastructure.Model
{
    public enum ErrorCodes
    {
        UnhandledException = 0,
        UserNotFound,
        UserHasNoRepositories,
        RepositorySource_UnableToReach,
    }
}
