namespace CircuitBreaker.Service
{
    public static class RateLimiter
    {
        private static readonly object rateLock = new object();
        private static readonly int maxRateLimit;
        private static readonly TimeSpan resetInterval;
        private static int requestCount;
        private static DateTime lastResetTime;

        static RateLimiter()
        {
            maxRateLimit = 2;
            requestCount = 0;
            resetInterval = TimeSpan.FromMinutes(1);
            lastResetTime = DateTime.Now;

            var timer = new System.Timers.Timer(resetInterval.TotalMilliseconds);
            timer.Elapsed += (sender, e) => ResetRate();
            timer.Start();
        }

        public static bool CanRequest()
        {
            lock (rateLock)
            {
                if (DateTime.Now - lastResetTime > resetInterval)
                {
                    ResetRate();
                }

                if (requestCount < maxRateLimit)
                {
                    requestCount++;
                    return true;
                }
                return false;
            }
        }

        private static void ResetRate()
        {
            lock (rateLock)
            {
                requestCount = 0;
                lastResetTime = DateTime.Now;
            }
        }
    }
}
