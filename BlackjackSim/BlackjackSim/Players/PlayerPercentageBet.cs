namespace BlackjackSim.Players;

public class PlayerPercentageBet : PlayerBase
{
    public double PercentageBet { get; set; }

    public PlayerPercentageBet(double percentage) : base($"Percentage Bet {percentage}%")
    {
        PercentageBet = percentage;
    }

    public override double PlaceInitialBet() => Money * (PercentageBet * 0.01);
    public override double MakeInsuranceBet() => 0;
    public override bool ShouldHit(Hand hand) => hand.GetValue() <= 16;
    public override bool ShouldDouble(Hand hand) => false;
    public override bool ShouldSplit(Hand hand) => false;
}
