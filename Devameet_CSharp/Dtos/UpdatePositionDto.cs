namespace Devameet_CSharp.Dtos;

public class UpdatePositionDto
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Orientation { get; set; } = null!;
}