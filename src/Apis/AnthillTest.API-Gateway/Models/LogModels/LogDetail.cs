namespace AnthillTest.API_Gateway.Models.LogModels;

public class LogDetail
{
    public string? MethodName { get; set; }
    public string? Explanation { get; set; }
    public byte Risk { get; set; }
    public string? LoggingTime { get; set; }
}
