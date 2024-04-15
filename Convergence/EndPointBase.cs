using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convergence
{
    public class EndPointBase<TSettings>
    {
        public TSettings Settings { get; set; }
        public EndPointID EndPointID { get; set; }

        // TODO: This mothod doesnt makes sense, it should be removed.
        public global::Convergence.IO.EPICS.Settings ConvertToEPICSSettings()
        {
            if (Settings is global::Convergence.IO.EPICS.Settings epicsSettings)
            {
                return epicsSettings;
            }
            else
            {
                // Handle other types or throw an exception
                throw new InvalidOperationException("Unsupported T type");
            }
        }
    }
}
