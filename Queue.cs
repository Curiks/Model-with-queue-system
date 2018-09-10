using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_LW1
{
    class QueueObject { }
    class L1 : QueueObject { }
    class L2 : QueueObject { }
    class Queue
    {
        private List<QueueObject> list = new List<QueueObject>();
        private int queueCount = 1;
        private int queueSum;

        public int Size
        {
            get { return list.Count; }
        }

        public bool Empty
        {
            get { return list.Count == 0; }
        }

        public double AvgInQueue
        {
            get { return queueSum / queueCount; }
        }

        public int CountObj(Type objClass)
        {
            return list.Count(x => x.GetType() == objClass); //GetType gets the runtime type of an instance.
        }

        public void Put(QueueObject obj)
        {
            list.Add(obj);
            queueCount++;
            queueSum += list.Count;
        }

        public QueueObject Pop()
        {
            if (list.Count > 0)
            {
                QueueObject obj = list[0];
                list.RemoveAt(0);
                queueCount++;
                queueSum += list.Count;

                return obj;
            }

            return null;
        }
    }
}
