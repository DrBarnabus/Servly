namespace Servly.Authentication;

public interface IAuthenticationContextState
{
    /// <summary>
    ///     A bool representing the authentication state of the user.
    /// </summary>
    bool IsAuthenticated { get; set; }

    /// <summary>
    ///     The subject (or user) identifier if available.
    /// </summary>
    Guid? SubjectId { get; set; }

    /// <summary>
    ///     Clones this instance of the state into a new object.
    /// </summary>
    /// <returns>The cloned object.</returns>
    IAuthenticationContextState Clone();
}
