using System;
using System.Collections.Generic;
using SFML.System;

namespace LilRogue
{
    public class ScheduledItem : IComparable<ScheduledItem>
    {
        public int Time { get; set; }
        public Action Action { get; set; }

        public int CompareTo(ScheduledItem other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }
            if (ReferenceEquals(null, other))
            {
                return 1;
            }
            return Time.CompareTo(other.Time);
        }
    }


    public class SchedulingSystem
    {
        private readonly PriorityQueue<ScheduledItem> _scheduler = new PriorityQueue<ScheduledItem>();

        public int time { get; set; }

        public void Schedule(int time, Action action)
        {
            var item = new ScheduledItem { Time = time, Action = action };
            _scheduler.Enqueue(item);
        }

        public void Update(int currentTime)
        {
            while (_scheduler.Count > 0 && _scheduler.Peek().Time <= currentTime)
            {
                var item = _scheduler.Dequeue();
                item.Action.Invoke();
            }
        }
    }

    // A priority queue implementation using a heap
    public class PriorityQueue<T>
    {
        private readonly List<T> _items = new List<T>();
        private readonly IComparer<T> _comparer = Comparer<T>.Default;

        public int Count => _items.Count;

        public void Enqueue(T item)
        {
            _items.Add(item);
            HeapifyUp(_items.Count - 1);
        }

        public T Dequeue()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            T firstItem = _items[0];
            _items[0] = _items[_items.Count - 1];
            _items.RemoveAt(_items.Count - 1);
            HeapifyDown(0);

            return firstItem;
        }

        public T Peek()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            return _items[0];
        }

        private void HeapifyUp(int index)
        {
            int parentIndex = (index - 1) / 2;
            while (index > 0 && _comparer.Compare(_items[index], _items[parentIndex]) < 0)
            {
                Swap(index, parentIndex);
                index = parentIndex;
                parentIndex = (index - 1) / 2;
            }
        }

        private void HeapifyDown(int index)
        {
            int leftChild = 2 * index + 1;
            int rightChild = 2 * index + 2;
            int smallest = index;

            if (leftChild < _items.Count && _comparer.Compare(_items[leftChild], _items[smallest]) < 0)
                smallest = leftChild;

            if (rightChild < _items.Count && _comparer.Compare(_items[rightChild], _items[smallest]) < 0)
                smallest = rightChild;

            if (smallest != index)
            {
                Swap(index, smallest);
                HeapifyDown(smallest);
            }
        }

        private void Swap(int i, int j)
        {
            T temp = _items[i];
            _items[i] = _items[j];
            _items[j] = temp;
        }

    }
}
