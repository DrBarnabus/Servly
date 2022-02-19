namespace Servly.Authentication;

/// <inheritdoc />
public class AuthenticationContext<TState> : IAuthenticationContext<TState>
    where TState : class, IAuthenticationContextState, new()
{
    private readonly AsyncLocal<Scope?> _currentScope = new();

    public AuthenticationContext(TState state)
    {
        _currentScope.Value = new Scope(this, state, null);
    }

    /// <inheritdoc />
    public TState? GetState()
    {
        return _currentScope.Value?.State;
    }

    /// <inheritdoc />
    public IDisposable SetContext(Action<TState> setupContext)
    {
        var parent = _currentScope.Value;

        var newContext = (TState?)parent?.State.Clone() ?? new TState();
        setupContext.Invoke(newContext);

        var newScope = new Scope(this, newContext, parent);
        _currentScope.Value = newScope;
        return newScope;
    }

    private class Scope : IDisposable
    {
        private readonly AuthenticationContext<TState> _context;
        private bool _isDisposed;

        public TState State { get; }
        public Scope? Parent { get; }

        public Scope(AuthenticationContext<TState> context, TState state, Scope? parent)
        {
            _context = context;

            State = state;
            Parent = parent;
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _context._currentScope.Value = Parent;
            _isDisposed = true;
        }
    }
}
