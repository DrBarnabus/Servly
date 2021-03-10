// Copyright (c) 2021 DrBarnabus

namespace Servly.Core.Exceptions
{
    public class ServlyHostAlreadyBuiltException : ServlyException
    {
        public ServlyHostAlreadyBuiltException()
            : base($"{nameof(IServlyHost)} has already been built.")
        {
        }

        public override string Code { get; } = "host_already_built";
    }
}
