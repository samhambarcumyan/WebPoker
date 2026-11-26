namespace WebPoker.Models;
public class Player
{
    public int Id { get; set; }
    public string? Nickname { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 1500.00m;
    public List<Card> PlayerCards { get; set; }
    // future Raise Bet Fold Check Methods
}
