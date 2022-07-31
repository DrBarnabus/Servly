namespace Servly.Authentication;

public class AuthenticationContextState : IAuthenticationContextState
{
    /// <inheritdoc />
    public bool IsAuthenticated { get; set; }

    /// <inheritdoc />
    public Guid? SubjectId { get; set; }

    /// <inheritdoc />
    public virtual IAuthenticationContextState Clone()
    {
        return new AuthenticationContextState
        {
            IsAuthenticated = IsAuthenticated,
            SubjectId = SubjectId
        };
    }
}
