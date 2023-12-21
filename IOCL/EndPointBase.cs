using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOCL
{
    public class EndPointBase<TSettings>
    {
        public TSettings Settings { get; set; }
    }
}
