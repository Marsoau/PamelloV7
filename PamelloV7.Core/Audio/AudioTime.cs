using PamelloV7.Core.Exceptions;

namespace PamelloV7.Core.Audio
{
    public struct AudioTime
    {
        public static long FrequencyMultiplier = 192000;

        private long _timeValue;
        public long TimeValue {
            get => _timeValue;
            set {
                var oldSeconds = TotalSeconds;
                _timeValue = value;

                if (oldSeconds != TotalSeconds) {
                    OnSecondTick?.Invoke();
                }
            }
        }

        public int TotalSeconds
        {
            get => (int)(TimeValue / FrequencyMultiplier);
            set => TimeValue = value * FrequencyMultiplier;
        }
        public int Seconds
        {
            get => TotalSeconds % 60;
            set => TotalSeconds = TotalSeconds - Seconds + value;
        }

        public int TotalMinutes
        {
            get => TotalSeconds / 60;
            set => TimeValue = value * 60 * FrequencyMultiplier;
        }
        public int Minutes
        {
            get => TotalMinutes % 60;
            set => TotalMinutes = TotalMinutes - Minutes + value;
        }

        public int TotalHours
        {
            get => TotalMinutes / 60;
            set => TimeValue = value * 3600 * FrequencyMultiplier;
        }

        public event Action? OnSecondTick;

        public AudioTime(long timeValue)
        {
            TimeValue = timeValue;
        }
        public AudioTime(int seconds)
        {
            TotalSeconds = seconds;
        }
        public AudioTime(int minutes, int seconds)
        {
            TotalSeconds = seconds + minutes * 60;
        }
        public AudioTime(int hours, int minutes, int seconds)
        {
            TotalSeconds = seconds + minutes * 60 + hours * 3600;
        }

        public static AudioTime operator *(AudioTime time, int scalar) {
            return new AudioTime(time.TimeValue * scalar);
        }

        public static AudioTime FromStrTime(string strTime) {
            var segments = strTime.Split(':').Reverse().ToArray();
            var seconds = 0;

            try {
                if (segments.Length >= 1) {
                    seconds += int.Parse(segments[0]);
                }
                if (segments.Count() >= 2) {
                    seconds += int.Parse(segments[1]) * 60;
                }
                if (segments.Count() == 3) {
                    seconds += int.Parse(segments[2]) * 3600;
                }
            }
            catch {
                throw new PamelloException("Wrong time format");
            }

            return new AudioTime(seconds);
        }

        public override string ToString()
        {
            return $"{(TotalHours < 10 ? '0' : "")}{TotalHours}:{(Minutes < 10 ? '0' : "")}{Minutes}:{(Seconds < 10 ? '0' : "")}{Seconds}";
        }
        public string ToShortString()
        {
            if (TotalHours != 0)
                return $"{(TotalHours < 10 ? '0' : "")}{TotalHours}:{(Minutes < 10 ? '0' : "")}{Minutes}:{(Seconds < 10 ? '0' : "")}{Seconds}";
            else
                return $"{(Minutes < 10 ? '0' : "")}{Minutes}:{(Seconds < 10 ? '0' : "")}{Seconds}";
        }
    }
}
