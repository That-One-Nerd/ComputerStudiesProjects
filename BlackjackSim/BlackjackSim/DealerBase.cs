namespace BlackjackSim;

public abstract class DealerBase : IPerson
{
    public required int DrawTo { get; init; }
    public required double BlackjackPayment { get; init; }
    public required double WinPayment { get; init; }

    public double HouseEdge => HouseWins / (HouseLossesRegular * WinPayment + HouseLossesBlackjack * BlackjackPayment);
    public int HouseWins { get; set; }
    public int HouseLossesRegular { get; set; }
    public int HouseLossesBlackjack { get; set; }

    public abstract void OnGameBegin(Game game);
    public abstract bool ShouldResetShoe();
}
