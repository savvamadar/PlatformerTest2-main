using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;

namespace Platformer.Gameplay
{

    /// <summary>
    /// This event is triggered when the player character enters a trigger with a VictoryZone component.
    /// </summary>
    /// <typeparam name="PlayerEnteredVictoryZone"></typeparam>
    public class PlayerEnteredVictoryZone : Simulation.Event<PlayerEnteredVictoryZone>
    {
        public VictoryZone victoryZone;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            //we won so...
            //let's pause the timer keep track of how long we've been alive
            //we aren't dead but we are so aren't really alive - this just makes sense
            DeathTracker.PauseTimer();

            //we won - it's time to, potentially, record our new score
            HighScore.RecordScore(DeathTracker.GetTimeAlive());

            //let's update the visual texts with or scores
            HighScore.UpdateScores();

            //in 0.2f open the leader board
            HighScore.InvokeMethod("OpenLeaderBoard", 0.2f);

            //in 5.5 seconds let's play again
            HighScore.InvokeMethod("ReloadLevel", 5.5f);

            //let's make the player say their time for extra flair
            model.player.Speak(DeathTracker.GetTimeAlive().ToString("F2"));

            model.player.animator.SetTrigger("victory");
            model.player.controlEnabled = false;
        }
    }
}