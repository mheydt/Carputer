using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Carputer.UWP.Messages
{
    public class SuspendStateMessage
    {
        public SuspendStateMessage(SuspendingOperation operation)
        {
            Operation = operation;
        }

        public SuspendingOperation Operation { get; }
    }
}
