using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carputer.Services
{
    public interface IAppServices
    {
        Task InitializeAsync();
    }

    public class AppServices : IAppServices
    {
        private IOBDIIDataProcessor _obd2;

        public AppServices(IOBDIIDataProcessor obd2)
        {
            _obd2 = obd2;
        }

        public async Task InitializeAsync()
        {
            await _obd2.InitializeAsync();
        }
    }
}
