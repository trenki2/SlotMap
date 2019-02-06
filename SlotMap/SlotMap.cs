using System;
using System.Collections.Generic;

namespace SlotMap
{
    public class ObjectNotFoundException : Exception
    {
        public ObjectNotFoundException() : this("The object was not found in the slotmap")
        {
        }

        public ObjectNotFoundException(string message) : base(message)
        {
        }

        public ObjectNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    [Serializable]
    public class SlotMap<T>
    {
        [Serializable]
        private struct Item
        {
            public T obj;
            public int version;
        }

        private int version = 0;
        private List<Item> items = new List<Item>();
        private Queue<int> freeList = new Queue<int>();

        public ulong Add(T obj)
        {
            int index = 0;
            int version = 0;

            if (freeList.Count == 0)
            {
                items.Add(new Item() { obj = obj, version = this.version });
                index = items.Count - 1;
                version = this.version++;
            }
            else
            {
                index = freeList.Dequeue();
                version = this.version++;
                items[index] = new Item() { obj = obj, version = version };
            }

            var key = MakeKey(index, version);
            return key;
        }

        private ulong MakeKey(int index, int version)
        {
            return ((ulong)version) << 32 | (uint)index;
        }

        private void SplitKey(ulong key, out int index, out int version)
        {
            index = (int)(key & 0xFFFFFFFF);
            version = (int)(key >> 32);
        }

        public T Get(ulong key)
        {
            if (!TryGet(key, out T obj))
                throw new ObjectNotFoundException();
            return obj;
        }

        public bool TryGet(ulong key, out T obj)
        {
            obj = default(T);

            SplitKey(key, out int index, out int version);

            if (index >= items.Count)
                return false;

            var item = items[index];

            if (item.version != version)
                return false;

            obj = item.obj;
            return true;
        }

        public void Remove(ulong key)
        {
            if (!TryRemove(key))
                throw new ObjectNotFoundException();
        }

        public bool TryRemove(ulong key)
        {
            SplitKey(key, out int index, out int version);

            if (index >= items.Count)
                return false;

            var item = items[index];

            if (item.version != version)
                return false;

            freeList.Enqueue(index);
            item.version = -1;
            item.obj = default(T);
            items[index] = item;

            return true;
        }
    }
}