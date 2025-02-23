using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using AdBlockerPoc.Models;

namespace AdBlockerPoc.Services
{
    public class BatchProcessor
    {
        private readonly DispatcherTimer _timer;
        private readonly ConcurrentQueue<Item> _itemQueue;
        private readonly ObservableCollection<Item> _items;
        private readonly Action<Item> _onItemProcessed;

        public BatchProcessor(ObservableCollection<Item> items, ConcurrentQueue<Item> itemQueue, int intervalMilliseconds, Action<Item> onItemProcessed)
        {
            _items = items;
            _itemQueue = itemQueue;
            _onItemProcessed = onItemProcessed;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(intervalMilliseconds)
            };
            _timer.Tick += (sender, e) => ProcessQueue();
        }

        public void Start()
        {
            _timer.Start();
        }

        private void ProcessQueue()
        {
            while (_itemQueue.TryDequeue(out var newItem))
            {
                _items.Add(newItem);
                _onItemProcessed(newItem);
            }
        }
    }
}
