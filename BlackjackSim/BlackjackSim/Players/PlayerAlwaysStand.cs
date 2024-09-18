namespace BlackjackSim.Players;

public class PlayerAlwaysStand : PlayerBase
{
    public double BetSize { get; set; } = 100;

    public PlayerAlwaysStand() : base("Always Stand") { }

    public override double PlaceInitialBet() => BetSize;
    public override double MakeInsuranceBet() => 0;
    public override bool ShouldHit(Hand hand) => false;
    public override bool ShouldDouble(Hand hand) => false;
    public override bool ShouldSplit(Hand hand) => false;
}
