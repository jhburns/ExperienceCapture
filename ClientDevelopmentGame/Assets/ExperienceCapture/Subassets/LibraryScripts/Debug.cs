namespace Capture.Internal.Debug
{
    /// <summary>
    /// Finds the minimum or maximum value in a set, incrementally.
    /// </summary>
    public class MinMax
    {
        /// <summary>
        /// A minimum or maximum value.
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// When true, find the minimum value. False maximum.
        /// </summary>
        private bool isMinimum;

        /// <summary>
        /// Construct a MinMax object.
        /// </summary>
        /// <param name="m">Set the isMinimim property.</param>
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

        /// <summary>
        /// Add a value to the set.
        /// </summary>
        /// <param name="next">A new value to be processed.</param>
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
