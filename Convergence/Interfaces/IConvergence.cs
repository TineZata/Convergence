using Convergence.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Convergence.EventCallbackDelegate;
using static Convergence.IO.EPICS.CaEventCallbackDelegate;

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
        public Task<EndPointStatus> ConnectAsync<T>(EndPointBase<T> endPointArgs);

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
        public Task<EndPointStatus> ReadAsync<T>(EndPointID endPointID, T? callback);

        /// <summary>
        /// Asynchronously writes the value to the EndPointID and returns and acknowledgement in a callback.
        /// </summary>
        /// <param name="endPointID"></param>
        /// <param name="value"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Task<EndPointStatus> WriteAsync<T>(EndPointID endPointID, IntPtr value, T? callback);

        /// <summary>
        /// Asynchronously subscribes to the EndPointID and returns the value in a callback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endPointID"></param>
        /// <param name="monitorType"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Task<EndPointStatus> SubscribeAsync<T1, T2>(EndPointID endPointID, T1? monitorType, T2? callback);
    }
}
