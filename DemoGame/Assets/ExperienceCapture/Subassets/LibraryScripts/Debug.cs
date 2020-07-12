namespace Capture.Internal.Debug
{
    public class MinMax
    {
        public float Value { get; private set; }
        private bool isMinimum;

        public MinMax(bool m)
        {
            this.isMinimum = m;
            
            if (this.isMinimum)
            {
                this.Value = float.MaxValue;
            }
            else
            {
                this.Value = float.MinValue;
            }
        }

        public void Include(float next)
        {
            if (this.isMinimum)
            {
                if (next < this.Value)
                {
                    this.Value = next;
                }
            }
            else
            {
                if (next > this.Value)
                {
                    this.Value = next;
                }
            }
        }
    }
}
