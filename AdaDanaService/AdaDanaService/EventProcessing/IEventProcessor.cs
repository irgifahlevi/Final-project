using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdaDanaService.EventProcessing
{
    public interface IEventProcessor
    {
        void ProccessEvent(string message);
    }
}