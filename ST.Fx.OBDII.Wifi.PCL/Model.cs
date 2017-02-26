using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.Fx.OBDII
{
    [ImplementPropertyChanged]
    public class Model
    {
        public string RPM { get; set; }
    }
}
