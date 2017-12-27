using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FedexRatesAndTime.Core {
    public abstract class AbstractBrowserDriver {
        protected TimeSpan CommandTimeout = TimeSpan.FromSeconds(10);

    }
}
