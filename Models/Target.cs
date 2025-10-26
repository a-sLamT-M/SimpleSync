namespace Models;

public class Target
{
    public string From { get; set; } = "./";
    public string To { get; set; } = "/root/target";
    public string? AfterSync { get; set; } = "";
}
