using Convergence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convergence
{
    /// <summary>
    /// Registry of all the end points.
    /// Provides a way to look up end points by their GUID.
    /// Provides static 'Instance' property that will make it evident that
    // client code is using a shared instance (potentially with multi threading issues.
    /// </summary>
    public class EndPointRegistry
    {
        // Singleton instance of the registry.
        private static EndPointRegistry _instance;
        // Dictionary of all the end points.
        private static System.Collections.Concurrent.ConcurrentDictionary
            <EndPointID, EndPointBase<ISettings>> _endPoints;
        /// <summary>
        /// Instance of the registry.
        /// 
        /// This property makes it evident that client code is using a shared instance (potentially with multi threading issues).
        /// </summary>
        public static EndPointRegistry Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EndPointRegistry();
                }
                return _instance;
            }
        }
        // Private constructor for singleton, to prevent external instantiation.
        private EndPointRegistry()
        {
            _endPoints = new System.Collections.Concurrent.ConcurrentDictionary<EndPointID, EndPointBase<ISettings>>();
        }
        /// <summary>
        /// Registers a new end point in the registry.
        /// </summary>
        /// <param name="endPoint">End point to register.</param>
        /// <returns>True if end point was registered successfully, false if end point was already registered.</returns>
        public bool Register(EndPointBase<ISettings> endPoint)
        {
            if (_endPoints.ContainsKey(endPoint.EndPointID))
            {
                return false;
            }
            _endPoints.TryAdd(endPoint.EndPointID, endPoint);
            return true;
        }
        /// <summary>
        /// Unregisters an end point from the registry.
        /// </summary>
        /// <param name="endPoint">End point to unregister.</param>
        /// <returns>True if end point was unregistered successfully, false if end point was not registered.</returns>
        public bool Unregister(EndPointBase<ISettings> endPoint)
        {
            if (!_endPoints.ContainsKey(endPoint.EndPointID))
            {
                return false;
            }
            _endPoints.TryRemove(endPoint.EndPointID, out _);
            return true;
        }
        /// <summary>
        /// Gets an end point by its GUID.
        /// </summary>
        /// <param name="id">GUID of the end point.</param>
        /// <returns>The end point, or null if end point was not found.</returns>
        public EndPointBase<ISettings> Get(EndPointID id)
        {
            if (!_endPoints.ContainsKey(id))
            {
                return null;
            }
            return _endPoints[id];
        }
    }
}
