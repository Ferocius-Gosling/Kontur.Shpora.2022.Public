using System;
using System.Threading;

namespace ReaderWriterLock
{
    public class RwLock : IRwLock
    {
        private readonly object rwLock = new();
        private int readersCount;
        private int writersCount;

        public void ReadLocked(Action action)
        {
            lock (rwLock)
            {
                while (writersCount > 0)
                    Monitor.Wait(rwLock);
                readersCount++; 
            }

            action();
            
            lock (rwLock)
            {
                readersCount--; 
                if (readersCount == 0)
                    Monitor.PulseAll(rwLock);
            }
        }

        public void WriteLocked(Action action)
        {
            lock (rwLock)
            {
                while (readersCount > 0 || writersCount > 0) 
                    Monitor.Wait(rwLock); 
                writersCount++;
            }

            action();
            
            lock (rwLock)
            {
                writersCount--;
                if (readersCount == 0 && writersCount == 0)
                    Monitor.PulseAll(rwLock);
            }
        }
    }
}
