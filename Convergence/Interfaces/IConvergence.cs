﻿using Convergence.IO;
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
        /// <param name="connectCallback"></param>
        /// <returns>Guid</returns>
        public Task<EndPointStatus> ConnectAsync<T1, T2>(EndPointBase<T1> endPointArgs, T2? connectCallback);

        /// <summary>
        /// Disconnects the EndPointID from the network.
        /// </summary>
        /// <param name="endPointID"></param>
        public bool Disconnect(EndPointID endPointID);

        /// <summary>
        /// Asynchronously reads the value of the EndPointID and returns the value in a callback.
        /// </summary>
        /// <param name="endPointID"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Task<EndPointStatus> ReadAsync<T>(EndPointID endPointID, T callback);

        /// <summary>
        /// Asynchronously writes the value to the EndPointID and returns and acknowledgement in a callback.
        /// </summary>
        /// <param name="endPointID"></param>
        /// <param name="value"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Task<EndPointStatus> WriteAsync<T>(EndPointID endPointID, nint value, T? callback);

        /// <summary>
        /// Asynchronously subscribes to the EndPointID and returns the value in a callback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endPointID"></param>
        /// <param name="monitorType"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Task<EndPointStatus> SubscribeAsync<T1, T2>(EndPointID endPointID, T1 monitorType, T2 callback);
    }
}
