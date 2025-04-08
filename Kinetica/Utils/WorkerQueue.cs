using System;
using System.Collections.Generic;


namespace kinetica.Utils
{
    internal sealed class WorkerQueue<T>
    {
        public System.Uri url { get; private set; }
        private readonly int capacity;
        private readonly bool has_primary_key;
        private readonly bool update_on_existing_pk;
        private List<T> queue;
        private Dictionary<RecordKey, int> primary_key_map;


        /// <summary>
        /// Creates an insertion queue for a given worker.
        /// </summary>
        /// <param name="url"></param>
        public WorkerQueue( System.Uri url )
        {
            this.url = url;
            this.capacity = 1;

            queue = [];
        }  // end constructor WorkerQueue<T>



        /// <summary>
        /// Creates an insertion queue for a given worker.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="capacity"></param>
        /// <param name="has_primary_key"></param>
        /// <param name="update_on_existing_pk"></param>
        public WorkerQueue(System.Uri url, int capacity, bool has_primary_key, bool update_on_existing_pk)
        {
            this.url = url;
            this.capacity = capacity;

            queue = [];

        }  // end constructor WorkerQueue<T>



        /// <summary>
        /// Returns the current queue and creates a new empty one.
        /// </summary>
        /// <returns>A list of records to be inserted.</returns>
        public IList<T> flush()
        {
            IList<T> old_queue = this.queue;
            queue = new List<T>(this.capacity);

            return old_queue;
        }  // end flush



        /// <summary>
        /// Inserts a record into the queue (if all conditions are
        /// favourable).  Returns the queue if it becomes full upon insertion.
        /// </summary>
        /// <param name="record">The record to insert into the queue.</param>
        /// <param name="key">A primary key, if any.</param>
        /// <returns>The list of records (if the queue is full), or null.</returns>
        public IList<T>? insert(T record, RecordKey key)
        {
            queue.Add(record);
            // If the queue is full, then flush and return the 'old' queue
            if (queue.Count == capacity)
                return flush();
            else // no records to return
                return null;
        }  // end insert
    }  // end class WorkerQueue

}   // end namespace kinetica.Utils
