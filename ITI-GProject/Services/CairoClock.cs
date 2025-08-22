namespace ITI_GProject.Services
{
    public class CairoClock: ILocalClock
    {
        private readonly TimeZoneInfo _tz;

        public CairoClock()
        {
            _tz = ResolveTz();
        }

        private static TimeZoneInfo ResolveTz()
        {
            string[] ids = { "Africa/Cairo", "Egypt Standard Time" };
            foreach (var id in ids)
            {
                try { return TimeZoneInfo.FindSystemTimeZoneById(id); } catch { }
            }
            return TimeZoneInfo.Local; // fallback
        }

        public DateTime Now()
        {
            var utc = DateTime.UtcNow;
            return TimeZoneInfo.ConvertTimeFromUtc(utc, _tz);
        }

        public DateTimeOffset NowOffset()
        {
            var utcNow = DateTimeOffset.UtcNow;
            var offset = _tz.GetUtcOffset(utcNow);
            return utcNow.ToOffset(offset);
        }
    }
}
