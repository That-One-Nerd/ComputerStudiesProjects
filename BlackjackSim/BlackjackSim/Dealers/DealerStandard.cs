namespace BlackjackSim.Dealers;

public class DealerStandard : DealerBase
{
    public override void OnGameBegin(Game game)
    {

    }
    public override bool ShouldResetShoe()
    {
        return false;
    }
}
