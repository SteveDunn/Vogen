using System.Threading.Tasks;

namespace Vogen.Examples;

public interface IScenario
{
    Task Run();

    public string GetDescription() => string.Empty;
}