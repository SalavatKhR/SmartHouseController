using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SubscriptionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var t = User;
        var claims = Handlers.TokenHandler.GetClaims(Request);
        
        var subs = await _context.Subscriptions
            .Where(w => w.UserId == claims.Id)
            .Select(s => new SubscriptionDto
            {
                DeviceName = s.DeviceName,
                DeviceDescription = s.DeviceDescription,
                Topic = s.Topic,
            })
            .ToListAsync();

        return Ok(subs);
    }
    
    [HttpPost]
    public async Task<IActionResult> Subscribe([FromForm] SubscriptionDto subscriptionDto)
    {
        if (string.IsNullOrEmpty(subscriptionDto.Topic) || 
            string.IsNullOrEmpty(subscriptionDto.DeviceName))
            return BadRequest("Empty topic");
        
        var claims = Handlers.TokenHandler.GetClaims(Request);

        var sub = _context.Subscriptions
            .FirstOrDefault(u => u.Topic == subscriptionDto.Topic 
                                 && u.UserId == claims.Id);

        if (sub != null)
            return BadRequest("Already subscribed");
        
        _context.Subscriptions.Add(new Subscription
        {
            UserId = claims.Id,
            Topic = subscriptionDto.Topic,
            DeviceName = subscriptionDto.DeviceName,
            DeviceDescription = subscriptionDto.DeviceDescription
        });

        await _context.SaveChangesAsync();

        return Ok();
    }
    
    [HttpDelete]
    public async Task<IActionResult> Unsubscribe(string topic)
    {
        if (string.IsNullOrEmpty(topic))
            return BadRequest("Empty topic");
        
        var claims = Handlers.TokenHandler.GetClaims(Request);

        var user = _context.Subscriptions
            .FirstOrDefault(u => u.User.UserName == claims.Name);

        if (user == null)
            return BadRequest("Couldn't resolve token");
        
        var subscription = _context.Subscriptions
            .FirstOrDefault(u => u.Topic == topic 
                                      & u.UserId == claims.Id);

        if (subscription == null)
            return BadRequest("Couldn't find subscription");
        
        _context.Remove(subscription);
        
        await _context.SaveChangesAsync();

        return Ok();
    }
}