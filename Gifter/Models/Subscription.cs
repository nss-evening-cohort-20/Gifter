namespace Gifter.Models;

public class Subscription
{
    public int Id { get; set; }
    public int SubscriberId { get; set; }
    public UserProfile? Subscriber { get; set; }
    public int ProviderId { get; set; }
    public UserProfile? Provider { get; set; }
}
