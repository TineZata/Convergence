using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convergence
{
    public class EndPointID(Protocols protocol, Guid id)
    {
        private string endPointName;
        private Protocols ePICS;
        private Guid guid;

        public static EndPointID Empty { get; } = new EndPointID(Protocols.None, Guid.Empty);
        public Protocols Protocol { get; } = protocol;
        public Guid Id { get; } = id;
        public string EndPointName { get; }
        public override string ToString()
        {
            return $"{nameof(Protocol)}:{Id}";
        }

        public EndPointID(Protocols ePICS, Guid guid, string endPointName)
            : this(ePICS, guid)
        {
            this.endPointName = endPointName;
        }
    }
}
