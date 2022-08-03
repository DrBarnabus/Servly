namespace Servly.Core.Exceptions;

public class ModuleAlreadyRegisteredException : ServlyException
{
    public ModuleAlreadyRegisteredException(string moduleName)
        : base($"Servly module '{moduleName}' has already been registered and cannot be registered again")
    {
    }

    public override string Code => "module_already_registered";
}
