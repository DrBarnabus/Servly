namespace Servly.Authentication;

/// <typeparam name="TState">The type to use for the <see cref="IAuthenticationContextState"/> implementation</typeparam>
public interface IAuthenticationContext<out TState>
    where TState : class, IAuthenticationContextState, new()
{
    /// <summary>
    ///     Returns the current state of the authentication context.
    /// </summary>
    /// <returns>The <see cref="TState"/> representing the current authentication state.</returns>
    TState? GetState();

    /// <summary>
    ///     Sets the current state of the authentication context.
    /// </summary>
    /// <param name="setupContext">An action that allows setting the new context state.</param>
    /// <returns>Returns an <see cref="IDisposable"/> that when disposed pops the state back to it's previous value.</returns>
    IDisposable SetContext(Action<TState> setupContext);
}
