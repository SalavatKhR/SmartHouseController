using System.Text;
using Microsoft.EntityFrameworkCore;
using MQTTnet.Server;
using SmartHouseController.MQTT.Broker.Domain.Data;
using SmartHouseController.MQTT.Broker.Domain.Entities;

namespace SmartHouseController.MQTT.Broker.Server.Handlers;

public static class ServerActionHandler
{
    public static async Task OnInterceptPublishAsync(InterceptingPublishEventArgs e, List<string> topics)
    {
        await using var db = new ApplicationDbContext();
        var topic = e.ApplicationMessage.Topic;

        if (topics.Contains(topic))
        {
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage!.Payload);
            var date = DateOnly.FromDateTime(DateTime.Now);

            await AddOrReplaceRecordAsync(topic, payload, date, db);

            await RemoveOldRecordAsync(date, db);
            
            await db.SaveChangesAsync();
        }
    }

    private static async Task AddOrReplaceRecordAsync(string topic, string payload, DateOnly date, ApplicationDbContext db)
    {
        var existingRecord = await db.Statistics.FirstOrDefaultAsync(r => r.date == date && r.topic == topic);
       
        if (existingRecord != null)
            existingRecord.payload = payload;
        else
            await db.AddAsync(new Staistics
            {
                date = date,
                topic = topic,
                payload = payload
            });
    }

    private static async Task RemoveOldRecordAsync(DateOnly date, ApplicationDbContext db)
    {
        var oldRecords = await db.Statistics.Where(r =>
            r.date.AddMonths(1) <= date
        ).ToListAsync();
        
        db.RemoveRange(oldRecords);
    }
}