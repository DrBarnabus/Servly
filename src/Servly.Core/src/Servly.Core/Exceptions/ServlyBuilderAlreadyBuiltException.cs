// Copyright (c) 2021 DrBarnabus

namespace Servly.Core.Exceptions
{
    public class ServlyBuilderAlreadyBuiltException : ServlyException
    {
        public ServlyBuilderAlreadyBuiltException()
            : base($"{nameof(IServlyBuilder)} has already been built.")
        {
        }

        public override string Code { get; } = "builder_already_built";
    }
}
