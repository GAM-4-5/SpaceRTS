namespace Map
{
    public abstract class SubNoiseGenerator
    {
        public virtual float GenerateAtPosition(int x, int y)
        {
            return 0f;
        }
    }


    public class Hills : SubNoiseGenerator
    {
        public override float GenerateAtPosition(int x, int y)
        {
            float value = 0;



            return value;
        }
    }
}