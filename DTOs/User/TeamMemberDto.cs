using System.Text.Json.Serialization;

namespace TimeTrack.API.DTOs.User;

public class TeamMemberDto
{
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
