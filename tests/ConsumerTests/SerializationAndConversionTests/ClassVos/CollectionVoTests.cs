#nullable disable
using System.Text.Json;
using Vogen.Tests.Types;

namespace Vogen.IntegrationTests.SerializationAndConversionTests.ClassVos;

public class CollectionVoTests
{
    [Fact]
    public void Serialized_json_is_same_for_vo_and_underlying()
    {
        var vo = FileHash.From(new Hash<byte>([1, 2, 3]));

        string jsonFromVo = JsonSerializer.Serialize(vo);
        string jsonFromHash = JsonSerializer.Serialize(vo);

        jsonFromVo.Equals(jsonFromHash).Should().BeTrue();
    }

    [Fact]
    public void Roundtrip()
    {
        var vo = FileHash.From(new Hash<byte>([1, 2, 3]));

        string jsonFromVo = JsonSerializer.Serialize(vo);
        var vo2 = JsonSerializer.Deserialize<FileHash>(jsonFromVo);

        vo.Equals(vo2).Should().BeTrue();
    }
}