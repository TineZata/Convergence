using Convergence.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convergence.Interfaces
{
    public interface IConvergence
    {
        /// <summary>
        /// Establish an connection and return a EndPointID.
        /// 
        /// If a connection already exists, the Guid for the concerned connection is returned otherwise a new
        /// </summary>
        /// <param name="endPointArgs"></param>
        /// <returns>Guid</returns>
        void Connect<T>(EndPointBase<T> endPointArgs);

        /// <summary>
        /// Disconnects the EndPointID from the network.
        /// </summary>
        /// <param name="endPointID"></param>
        public void Disconnect(EndPointID endPointID);

        public Task ReadAsync(EndPointID endPointID, ValueUpdateCallback? callback);
    }
}
