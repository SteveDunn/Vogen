using RefitExample;
using ServiceStackDotTextExample;
using Vogen;

[assembly: VogenDefaults(conversions:Conversions.ServiceStackDotText | Conversions.SystemTextJson)]

await ServiceStackTextRunner.Run();
await RefitRunner.Run();


