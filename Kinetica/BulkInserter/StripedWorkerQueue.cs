using System;
using System.Collections.Generic;
using System.Threading;

namespace kinetica;

/// <summary>
/// A stripe within a worker queue. Each stripe has its own lock to reduce contention.
    /// </summary>
    /// <typeparam name="T">The record type.</typeparam>
    internal sealed class Stripe<T>
    {
        private readonly object _lock = new();
        private List<T> _records;
        private readonly int _capacity;

        public Stripe(int capacity)
        {
            _capacity = capacity;
            _records = new List<T>(capacity);
        }

        /// <summary>
        /// Adds a record to the stripe. Returns the batch if capacity is reached, null otherwise.
        /// </summary>
        public List<T>? Add(T record)
        {
            lock (_lock)
            {
                _records.Add(record);
                if (_records.Count >= _capacity)
                {
                    var batch = _records;
                    _records = new List<T>(_capacity);
                    return batch;
                }
                return null;
            }
        }

        /// <summary>
        /// Adds multiple records to the stripe. Returns any complete batches.
        /// </summary>
        public List<List<T>> AddRange(IReadOnlyList<T> records)
        {
            var batches = new List<List<T>>();
            lock (_lock)
            {
                foreach (var record in records)
                {
                    _records.Add(record);
                    if (_records.Count >= _capacity)
                    {
                        batches.Add(_records);
                        _records = new List<T>(_capacity);
                    }
                }
            }
            return batches;
        }

        /// <summary>
        /// Flushes all records from the stripe, regardless of count.
        /// </summary>
        public List<T>? Flush()
        {
            lock (_lock)
            {
                if (_records.Count == 0)
                    return null;

                var batch = _records;
                _records = new List<T>(_capacity);
                return batch;
            }
        }

        /// <summary>
        /// Gets the current count of records in the stripe.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _records.Count;
                }
            }
        }
    }

    /// <summary>
    /// A worker queue with multiple stripes to reduce lock contention.
    /// Each stripe operates independently, allowing concurrent inserts to different stripes.
    /// </summary>
    /// <typeparam name="T">The record type.</typeparam>
    internal sealed class StripedWorkerQueue<T>
    {
        private readonly Stripe<T>[] _stripes;
        private readonly int _numStripes;
        private readonly int _stripeMask;

        /// <summary>
        /// URL of the worker this queue is associated with.
        /// </summary>
        public Uri Url { get; }

        /// <summary>
        /// Index of this worker in the worker list.
        /// </summary>
        public int WorkerIndex { get; }

        /// <summary>
        /// Creates a new striped worker queue.
        /// </summary>
        /// <param name="url">Worker URL.</param>
        /// <param name="workerIndex">Worker index.</param>
        /// <param name="numStripes">Number of stripes (should be power of 2 for efficient modulo).</param>
        /// <param name="batchSize">Batch size per stripe.</param>
        public StripedWorkerQueue(Uri url, int workerIndex, int numStripes, int batchSize)
        {
            Url = url;
            WorkerIndex = workerIndex;

            // Round up to nearest power of 2 for efficient modulo operation
            _numStripes = RoundUpToPowerOf2(numStripes);
            _stripeMask = _numStripes - 1;

            _stripes = new Stripe<T>[_numStripes];
            for (int i = 0; i < _numStripes; i++)
            {
                _stripes[i] = new Stripe<T>(batchSize);
            }
        }

        private static int RoundUpToPowerOf2(int n)
        {
            n--;
            n |= n >> 1;
            n |= n >> 2;
            n |= n >> 4;
            n |= n >> 8;
            n |= n >> 16;
            n++;
            return Math.Max(1, n);
        }

        /// <summary>
        /// Adds a record to the appropriate stripe based on the stripe hash.
        /// Returns a batch if the stripe reaches capacity.
        /// </summary>
        /// <param name="record">The record to add.</param>
        /// <param name="stripeHash">Hash value to determine stripe placement.</param>
        /// <param name="stripeIndex">Output: the stripe index used.</param>
        /// <returns>A batch if capacity reached, null otherwise.</returns>
        public List<T>? Add(T record, long stripeHash, out int stripeIndex)
        {
            stripeIndex = (int)(stripeHash & _stripeMask);
            return _stripes[stripeIndex].Add(record);
        }

        /// <summary>
        /// Adds a record to a specific stripe.
        /// </summary>
        public List<T>? AddToStripe(T record, int stripeIndex)
        {
            return _stripes[stripeIndex & _stripeMask].Add(record);
        }

        /// <summary>
        /// Adds multiple records to a specific stripe.
        /// </summary>
        public List<List<T>> AddRangeToStripe(IReadOnlyList<T> records, int stripeIndex)
        {
            return _stripes[stripeIndex & _stripeMask].AddRange(records);
        }

        /// <summary>
        /// Flushes all stripes and returns all batches.
        /// </summary>
        /// <returns>List of (stripeIndex, batch) tuples.</returns>
        public List<(int stripeIndex, List<T> batch)> FlushAll()
        {
            var batches = new List<(int, List<T>)>();
            for (int i = 0; i < _numStripes; i++)
            {
                var batch = _stripes[i].Flush();
                if (batch != null && batch.Count > 0)
                {
                    batches.Add((i, batch));
                }
            }
            return batches;
        }

        /// <summary>
        /// Gets the total count of records across all stripes.
        /// </summary>
        public int TotalCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < _numStripes; i++)
                {
                    count += _stripes[i].Count;
                }
                return count;
            }
        }

        /// <summary>
        /// Number of stripes in this queue.
        /// </summary>
        public int NumStripes => _numStripes;
    }
