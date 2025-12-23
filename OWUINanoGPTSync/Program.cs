using System.Diagnostics;
using OWUINanoGPTSync.APIAccess;
using OWUINanoGPTSync.Transformation;

namespace OWUINanoGPTSync;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine($"starting nano gpt sync @ {DateTime.UtcNow} UTC...");
        
        var createdModelCount = 0;
        var updatedModelCount = 0;
        
        var sw = Stopwatch.StartNew();
        
        try
        {
            var models = NanoGPTAccessor.GetAllModels();

            var allBaseModels = OWUIAccessor.GetAllBaseModels();

            var allConfiguredBaseModels = OWUIAccessor.GetAllConfiguredBaseModels();

            Console.WriteLine($"models in nano gpt: {models.Count}");
            Console.WriteLine($"base models in owui: {allBaseModels.Count}");
            Console.WriteLine($"configured base models in owui: {allConfiguredBaseModels.Count}");
            
            foreach (var (key, value) in models)
            {
                if (allBaseModels.TryGetValue(key, out var baseModel) &&
                    baseModel != null)
                {
                    allConfiguredBaseModels.TryGetValue(key, out var configuredBaseModel);

                    var transformed = ModelTransformer.Transform(baseModel, configuredBaseModel, value);

                    var create = transformed.Item1;
                    var model = transformed.Item2;
                
                    if (create)
                    {
                        OWUIAccessor.CreateModel(model);
                    
                        createdModelCount++;
                    }
                    else
                    {
                        OWUIAccessor.UpdateModel(model);
                    
                        updatedModelCount++;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error: {ex.Message}");
            
            Environment.Exit(1);
        }

        sw.Stop();
        
        Console.WriteLine();
        Console.WriteLine($"created models: {createdModelCount}, " +
                          $"updated models: {updatedModelCount} " +
                          $"in {sw.Elapsed}");
    }
}