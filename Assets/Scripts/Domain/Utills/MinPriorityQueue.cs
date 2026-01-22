using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Domain.Utills {
    public class MinPriorityQueue<T> {
        readonly List<(T item, int priority)> heap = new();
        
        public int Count => heap.Count;
        
        public void Enqueue(T item, int priority) {
            heap.Add((item, priority));
            HeapifyUp(heap.Count - 1);
        }

        public T Dequeue() {
            if (heap.Count == 0)
                throw new InvalidOperationException("Queue is empty.");
            
            var result = heap[0].item;
            
            var last = heap.Count - 1;
            heap[0] = heap[last];
            heap.RemoveAt(last);

            if (heap.Count > 0)
                HeapifyDown(0);

            return result;
        }

        void HeapifyUp(int i) {
            while (i > 0) {
                var parent = (i - 1) / 2;
                if (heap[i].priority >= heap[parent].priority)
                    break;
                
                (heap[i], heap[parent]) = (heap[parent], heap[i]);
                i = parent;
            }
        }

        void HeapifyDown(int i) {
            int last = heap.Count - 1;
            while (true)
            {
                int left = i * 2 + 1;
                int right = left + 1;
                if (left > last) break;

                int smallest = left;
                if (right <= last && heap[right].priority < heap[left].priority)
                    smallest = right;

                if (heap[i].priority <= heap[smallest].priority)
                    break;

                (heap[i], heap[smallest]) = (heap[smallest], heap[i]);
                i = smallest;
            }
        }
    }
}