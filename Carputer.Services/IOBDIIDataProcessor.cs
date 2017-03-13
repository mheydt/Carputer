using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carputer.Services
{
    public interface IOBDIIDataProcessor
    {
        Task InitializeAsync();
    }
}
