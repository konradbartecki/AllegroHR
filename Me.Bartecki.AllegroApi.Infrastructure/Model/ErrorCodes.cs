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
