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
        /// Establish an EPICS connection and return a EndPointID.
        /// 
        /// If a connection already exists, the Guid for the concerned connection is returned otherwise a new
        /// </summary>
        /// <param name="id"></param>
        /// <param name="endPointArgs"></param>
        /// <returns>Guid</returns>
        EndPointID Connect<T>(EndPointID id, EndPointBase<T> endPointArgs);
    }
}
