namespace ChatRoom.Centralized.Shared.ObjectModels;

public class PublicServerInfo : IPublicInfo
{
    public string Name { get; set; } = "Unnamed Server";
    public string Description { get; set; } = "No Description Provided.";
    public bool AllowsSignup { get; set; } = true;
}
