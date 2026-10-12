namespace Dazed.Vitals.Audio
{
    public struct SoundEmissionSettings(float volume, float panning, float speed, bool loop, bool threeDimensionalAudio, bool playOnStart = true)
    {
        public float Volume = volume;
        public float Panning = panning;
        public float Speed = speed;
        public bool Loop = loop;
        public bool ThreeDimensionalAudio = threeDimensionalAudio;
        public bool PlayOnStart = playOnStart;

        public static readonly SoundEmissionSettings Default = new(1, 0.5f, 1, false, false);
        public static readonly SoundEmissionSettings DefaultLooped = new(1, 0.5f, 1, true, false);
        public static readonly SoundEmissionSettings Default3D = new(1, 0.5f, 1, false, true);
    }
}
