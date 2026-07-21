using UsingTypesFromTheDomainLayer;
using UsingTypesGeneratedInTheSameProject;
using Vogen;

// We reference the Domain project, which emits the IVogen static abstract interface, so we don't want to emit it here too.
[assembly: VogenDefaults(staticAbstractsGeneration: StaticAbstractsGeneration.Omit)]

SystemTextJsonSourceGenerationScenario_UsingTypesFromTheDomainLayer.Run();
SystemTextJsonSourceGenerationScenario_UsingTypesGeneratedInTheSameProject.Run();
EfCoreScenario.Run();

[ValueObject]
public partial class MyVo;
