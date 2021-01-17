using NAudio.Wave;

namespace Soundboard
{
    [System.Serializable]
    public class SoundPair
    {
        public WaveOutEvent pub;
        public WaveOutEvent loc;

        public SoundPair(WaveOutEvent _pub, WaveOutEvent _loc)
        {
            pub = _pub;
            loc = _loc;
        }
    }
}
