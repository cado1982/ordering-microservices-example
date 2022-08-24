using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.EventProcessing
{
    public interface IEventHandler
    {
        public void Handle(string message);
        public EventType EventType { get; }
    }
}