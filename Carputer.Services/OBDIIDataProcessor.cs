using ST.Fx.OBDII;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carputer.Services
{

    public class OBDIIDataProcessor : IOBDIIDataProcessor
    {
        private IOBDIIService _obd2service;

        public OBDIIDataProcessor(IOBDIIService obd2service)
        {
            _obd2service = obd2service;
        }

        public async Task InitializeAsync()
        {
            await _obd2service.InitAsync();
        }
    }
}
