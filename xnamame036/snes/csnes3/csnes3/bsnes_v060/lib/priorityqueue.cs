using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    public delegate void Callback(uint arg);
    //priority queue implementation using binary min-heap array;
    //does not require normalize() function.
    //O(1)     find   (tick)
    //O(log n) insert (enqueue)
    //O(log n) remove (dequeue)

    public class PriorityQueue
    {
        public void priority_queue_nocallback(uint arg) { }

        public void tick(uint ticks)
        {
            basecounter += ticks;
            while (heapsize != 0 && basecounter >= heap[0].counter)
            {
                callback(dequeue());
            }
        }

        //counter is relative to current time (eg enqueue(64, ...) fires in 64 ticks);
        //counter cannot exceed std::numeric_limits<uint>::max() >> 1.
        public void enqueue(uint counter, uint Event)
        {
            uint child = heapsize++;
            counter += basecounter;

            while (child != 0)
            {
                uint parent = (child - 1) >> 1;
                if (counter >= heap[parent].counter)
                {
                    break;
                }

                heap[child].counter = heap[parent].counter;
                heap[child].Event = heap[parent].Event;
                child = parent;
            }

            heap[child].counter = counter;
            heap[child].Event = Event;
        }

        public uint dequeue()
        {
            uint Event = heap[0].Event;
            uint parent = 0;
            uint counter = heap[--heapsize].counter;

            while (true)
            {
                uint child = (parent << 1) + 1;
                if (child >= heapsize)
                {
                    break;
                }
                if (child + 1 < heapsize && heap[child].counter >= heap[child + 1].counter)
                {
                    child++;
                }
                if (heap[child].counter >= counter)
                {
                    break;
                }

                heap[parent].counter = heap[child].counter;
                heap[parent].Event = heap[child].Event;
                parent = child;
            }

            heap[parent].counter = counter;
            heap[parent].Event = heap[heapsize].Event;
            return Event;
        }

        public void reset()
        {
            basecounter = 0;
            heapsize = 0;
        }

        public void serialize(Serializer s)
        {
            s.integer(basecounter, "basecounter");
            s.integer(heapsize, "heapsize");
            for (uint n = 0; n < heapcapacity; n++)
            {
                s.integer(heap[n].counter, "heap[n].counter");
                s.integer(heap[n].Event, "heap[n].event");
            }
        }

        public PriorityQueue(uint size, Callback callback_ = null)
        {
            if (ReferenceEquals(callback_, null))
            {
                callback = priority_queue_nocallback;
            }
            else
            {
                callback = callback_;
            }
            heap = new Heap[size];
            heapcapacity = size;
            reset();
        }

        private Callback callback;
        private uint basecounter;
        private uint heapsize;
        private uint heapcapacity;

        private struct Heap
        {
            public uint counter;
            public uint Event;
        }
        Heap[] heap;
    }
}
