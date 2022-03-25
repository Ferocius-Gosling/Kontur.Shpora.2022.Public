using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                    Monitor.Wait(rwLock); //отпускаем поток ждать. если писателей больше нуля
                readersCount++; //читатель +1.
            }

            action();
            

            lock (rwLock)
            {
                readersCount--; //вот тут мы прочитали уже.
                if (readersCount == 0)
                    Monitor.PulseAll(rwLock); //просигналили всем потокам, что мы тут закончили.
            }
        }

        public void WriteLocked(Action action)
        {
            lock (rwLock)
            {
                while (readersCount > 0 || writersCount > 0) // если мы пришли читать или писать,
                                                   // но кто-то уже пишет или читает, то надо ждать.
                    Monitor.Wait(rwLock); //то они должны ждать
                writersCount++;
            }

            action();
            
            lock (rwLock)
            {
                writersCount--;
                if (readersCount == 0 && writersCount == 0) // если все закончили свои дела
                    Monitor.PulseAll(rwLock);   // сигналим
            }
        }
    }
}
