using SoundFlow.Components;
namespace Dazed.Vitals.Audio
{
    public class SoundEmitter : IDisposable
    {
        public SoundPlayer? Player => _playerExists && !_isDisposed ? _player : null;
        public AudioClip Clip => _clip;
        private SoundPlayer _player;
        private AudioClip _clip;
        private bool _playerExists, _isDisposed;

        private SoundEmissionSettings Settings
        {
            get => field;
            set
            {
                field = value;
                SetPlayerSettings();
            }
        }

        public SoundEmitter(string pathToClip, SoundEmissionSettings? settings = null)
        {
            if (settings == null)
                settings = SoundEmissionSettings.Default;
            Settings = settings.Value;
            _clip = new AudioClip(pathToClip);
            SetNewPlayer(_clip);
            SetPlayerSettings();
            OmniSonus.OnTermination += Dispose;
        }

        public void ChangeClip(string clipPath)
        {
            _player?.Stop();
            _player?.Dispose();
            _clip = new(clipPath);
            SetNewPlayer(_clip);
            SetPlayerSettings();
        }

        private void SetPlayerSettings()
        {
            if (!_playerExists)
                return;
            _player.IsLooping = Settings.Loop;
            _player.Volume = Settings.Volume;
            _player.Pan = Settings.Panning;
            _player.PlaybackSpeed = Settings.Speed;
            if (Settings.PlayOnStart)
                _player.Play();
        }

        private void SetNewPlayer(AudioClip clip)
        {
            if (_player != null)
                OmniSonus.MainDevice.MasterMixer.RemoveComponent(_player);
            _player = new(OmniSonus.Engine, OmniSonus.Format, clip.Provider);
            OmniSonus.MainDevice.MasterMixer.AddComponent(_player);
            _playerExists = true;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _player.Stop();
            OmniSonus.MainDevice.MasterMixer.RemoveComponent(_player);
            _playerExists = false;
            _isDisposed = true;
            OmniSonus.OnTermination -= Dispose;
        }
    }
}
