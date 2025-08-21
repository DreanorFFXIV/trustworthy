using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
using Questionable.Model;
using Questionable.QuestPaths;
using Xunit;

namespace QuestPaths.JsonValidator;

public sealed class ValidJsonFilesTest
{
    private static readonly JsonSchema QuestSchema;

    static ValidJsonFilesTest()
    {
        SchemaRegistry.Global.Register(
            new Uri("https://git.carvel.li/liza/Questionable/raw/branch/master/Questionable.Model/common-aethernetshard.json"),
            JsonSchema.FromStream(AssemblyModelLoader.CommonAethernetShard).AsTask().Result);
        SchemaRegistry.Global.Register(
            new Uri("https://git.carvel.li/liza/Questionable/raw/branch/master/Questionable.Model/common-aetheryte.json"),
            JsonSchema.FromStream(AssemblyModelLoader.CommonAetheryte).AsTask().Result);
        SchemaRegistry.Global.Register(
            new Uri("https://git.carvel.li/liza/Questionable/raw/branch/master/Questionable.Model/common-classjob.json"),
            JsonSchema.FromStream(AssemblyModelLoader.CommonClassJob).AsTask().Result);
        SchemaRegistry.Global.Register(
            new Uri("https://git.carvel.li/liza/Questionable/raw/branch/master/Questionable.Model/common-completionflags.json"),
            JsonSchema.FromStream(AssemblyModelLoader.CommonCompletionFlags).AsTask().Result);
        SchemaRegistry.Global.Register(
            new Uri("https://git.carvel.li/liza/Questionable/raw/branch/master/Questionable.Model/common-vector3.json"),
            JsonSchema.FromStream(AssemblyModelLoader.CommonVector3).AsTask().Result);
        
        QuestSchema = JsonSchema.FromStream(AssemblyQuestLoader.QuestSchema).AsTask().Result;
    }

    [Theory]
    [ClassData(typeof(TestQuestLoader))]
    public void QuestShouldValidateAsJson(QuestWrapper quest)
    {
        JsonNode questNode = JsonNode.Parse(quest.AsStream()) ?? throw new InvalidDataException("no quest stream");

        EvaluationResults evaluationResult = QuestSchema.Evaluate(questNode, new EvaluationOptions
        {
            Culture = CultureInfo.InvariantCulture,
            OutputFormat = OutputFormat.List
        });

        if (!evaluationResult.IsValid)
            Assert.Fail($"Quest '{quest.ManifestName}' validation failed");
    }
}
