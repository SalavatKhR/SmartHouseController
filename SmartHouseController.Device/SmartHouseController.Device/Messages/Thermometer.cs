namespace SmartHouseController.Device.Messages;

public class Thermometer
{
    public int battery { get; set; }
    public double humidity{ get; set; }
    public int linkquality{ get; set; }
    public int power_outage_count{ get; set; }
    public double pressure{ get; set; }
    public double temperature{ get; set; }
    public int voltage{ get; set; }
}