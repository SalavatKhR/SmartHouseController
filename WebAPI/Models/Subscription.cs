using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models;

public class Subscription
{
    [ForeignKey("User")]
    public string UserId { get; set; }
    public string DeviceName { get;set; }
    public string DeviceDescription { get;set; }
    public string Topic { get; set; }
    public virtual AppUser User { get; set; }
}