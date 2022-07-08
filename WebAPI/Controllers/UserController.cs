using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Controllers;

public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpPost]
    public async Task Subscribe(string topic)
    {
        _context.Subscriptions.Add(new Subscription
        {
            UserId = HttpContext.User.Identity.Name,
            Topic = topic
        });

        await _context.SaveChangesAsync();
    }
    
    [HttpPost]
    public async Task Unsubscribe(string topic)
    {
        var subscription = _context.Subscriptions
            .FirstOrDefaultAsync(u => u.Topic == topic 
                                      & u.UserId == HttpContext.User.Identity.Name);

        _context.Remove(subscription);
        
        await _context.SaveChangesAsync();
    }
}