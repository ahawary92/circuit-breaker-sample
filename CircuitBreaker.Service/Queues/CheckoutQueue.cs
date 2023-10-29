using System.Collections.Concurrent;

namespace CircuitBreaker.Service.Queues
{
    public static class CheckoutQueue
    {
        private static readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

        public static void EnqueueRequest(Action request)
        {
            queue.Enqueue(request);
        }

        public static bool TryDequeueRequest(out Action request)
        {
            return queue.TryDequeue(out request);
        }

        public static int Count()
        {
            return queue.Count;
        }
    }
}
