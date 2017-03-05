using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.Fx.OBDII
{
    public class OBDIIUpdate
    {
        public Dictionary<string, string> Properties { get; private set; }

        public OBDIIUpdate(Dictionary<string, string> state)
        {
            var ret = new Dictionary<string, string>();
            foreach (var key in state.Keys)
            {
                ret.Add(key, state[key]);
            }

            Properties = ret;
        }
    }
}
