using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOCL
{
    public interface IIOConvergenceApi
    {
        /// <summary>
        /// Establish a connection and return a Guid.
        /// 
        /// If a connection already exists, the Guid for the concerned connection is returned otherwise a new
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="channelId"></param>
        /// <param name="endPointArgs"></param>
        /// <returns>Guid</returns>
        Guid Connect(Protocols protocol, string channelId, EndPointBase<object> endPointArgs);
    }
}
