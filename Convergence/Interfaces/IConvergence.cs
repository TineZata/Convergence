﻿using Convergence.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Convergence.IO.EPICS.EventCallbackDelegate;

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

        /// <summary>
        /// Asynchronously reads the value of the EndPointID and returns the value in a callback.
        /// </summary>
        /// <param name="endPointID"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Task<EndPointStatus> ReadAsync(EndPointID endPointID, ReadCallback? callback);

        /// <summary>
        /// Asynchronously writes the value to the EndPointID and returns and acknowledgement in a callback.
        /// </summary>
        /// <param name="endPointID"></param>
        /// <param name="value"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Task<EndPointStatus> WriteAsync(EndPointID endPointID, IntPtr value, WriteCallback? callback);
    }
}
