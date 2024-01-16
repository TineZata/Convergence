using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convergence.IO.EPICS
{
    public enum PreemptiveCallbacks
    {
        // ca_disable_preemptive_callback
        DISABLE = 0,
        //ca_enable_preemptive_callback 
        ENABLE = 1,
    };

}
