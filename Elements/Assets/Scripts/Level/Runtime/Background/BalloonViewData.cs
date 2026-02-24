namespace Elements.Level
{
    public readonly struct BalloonViewData
    {
        public readonly float Speed;
        public readonly float Direction;
        public readonly float BaseY;
        public readonly float Amplitude;
        public readonly float Frequency;
        public readonly float ExitX;

        public BalloonViewData(float speed, float direction, float baseY, float amplitude, float frequency, float exitX)
        {
            Speed = speed;
            Direction = direction;
            BaseY = baseY;
            Amplitude = amplitude;
            Frequency = frequency;
            ExitX = exitX;
        }
    }
}
