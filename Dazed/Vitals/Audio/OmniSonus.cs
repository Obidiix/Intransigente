using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Structs;

namespace Dazed.Vitals.Audio
{
    public static class OmniSonus
    {
        internal static MiniAudioEngine Engine => _engine;
        internal static AudioPlaybackDevice MainDevice => _mainDevice;
        internal static AudioFormat Format => AudioFormat.Cd;

        private static MiniAudioEngine _engine;
        private static AudioPlaybackDevice _mainDevice;

        internal static event Action? OnTermination;

        static OmniSonus()
        {
            _engine = new MiniAudioEngine();
            _engine.UpdateAudioDevicesInfo();
            _mainDevice = _engine.InitializePlaybackDevice(_engine.PlaybackDevices.FirstOrDefault(x => x.IsDefault), Format);
            MainDevice.Start();
        }

        internal static void Terminate()
        {
            OnTermination?.Invoke();
            _mainDevice.Stop();
            _engine.Dispose();
        }
    }
}
