namespace GameFramework.AI.GOAP
{
    public class Patient : GoapAgent
    {
        private void Start()
        {
            base.Start();

            var s1 = new Goal("isWaiting", 1, true);
            AddGoal(s1, 3);
        }
    }
}
