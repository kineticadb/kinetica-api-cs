using System;
using System.Collections.Generic;


namespace kinetica.Utils
{
    internal sealed class WorkerQueue<T>
    {
        public System.Uri url { get; private set; }
        private int capacity;
        private bool has_primary_key;
        private bool update_on_existing_pk;
        private IList<T> queue;
        private Dictionary<RecordKey, int> primary_key_map;


        /// <summary>
        /// Creates an insertion queue for a given worker.
        /// </summary>
        /// <param name="url"></param>
        public WorkerQueue( System.Uri url )
        {
            this.url = url;
            this.capacity = 1;
            this.has_primary_key = false;
            this.update_on_existing_pk = false;

            queue = new List<T>();
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
            this.has_primary_key = has_primary_key;
            this.update_on_existing_pk = update_on_existing_pk;

            queue = new List<T>();

            // If the type has primary keys, then initialize with a
            // capacity of 75% of the final capacity
            if (this.has_primary_key)
                primary_key_map = new Dictionary<RecordKey, int>((int)Math.Round(this.capacity / 0.75));
        }  // end constructor WorkerQueue<T>



        /// <summary>
        /// Returns the current queue and creates a new empty one.
        /// </summary>
        /// <returns>A list of records to be inserted.</returns>
        public IList<T> flush()
        {
            IList<T> old_queue = this.queue;
            queue = new List<T>(this.capacity);

            // Clear the primary key map if one exists
            if (this.primary_key_map != null)
                this.primary_key_map.Clear();

            return old_queue;
        }  // end flush



        /// <summary>
        /// Inserts a record into the queue (if all conditions are
        /// favourable).  Returns the queue if it becomes full upon insertion.
        /// </summary>
        /// <param name="record">The record to insert into the queue.</param>
        /// <param name="key">A primary key, if any.</param>
        /// <returns>The list of records (if the queue is full), or null.</returns>
        public IList<T> insert(T record, RecordKey key)
        {
            if (this.has_primary_key && key.isValid())
            {
                // We are to update the record even if the primary key already exists
                if (this.update_on_existing_pk)
                {
                    int key_idx;

                    if (this.primary_key_map.TryGetValue(key, out key_idx))
                    {
                        // Key exists, so we need to replace the associated record
                        this.queue[key_idx] = record;
                    }
                    else  // key does not exist; add the record and
                    {     // update the key->record mapping
                        this.queue.Add(record);
                        this.primary_key_map.Add(key, (this.queue.Count - 1));
                    }
                }
                else // do NOT update/add the record if the key already exists
                {
                    if (this.primary_key_map.ContainsKey(key))
                        return null;  // yup, the key already exists

                    // The key does not exist, so add the record and
                    // update the key->record map
                    this.queue.Add(record);
                    this.primary_key_map.Add(key, (this.queue.Count - 1));
                }
            }
            else  // simply add the record
            {
                queue.Add(record);
            }

            // If the queue is full, then flush and return the 'old' queue
            if (queue.Count == capacity)
                return flush();
            else // no records to return
                return null;
        }  // end insert
    }  // end class WorkerQueue

}   // end namespace kinetica.Utils
