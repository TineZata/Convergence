using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convergence
{
    public class EndPointID(Protocols protocol, Guid id)
    {
        public static EndPointID Empty { get; } = new EndPointID(Protocols.None, Guid.Empty);
        public Protocols Protocol { get; set; } = protocol;
        public Guid UniqueId { get; set; } = id;
        public string ? EndPointName { get; set; }

        public override string ToString()
        {
            return $"{nameof(Protocol)}:{UniqueId}";
        }

        public EndPointID(Protocols protocol, Guid guid, string endPointName)
            : this(protocol, guid)
        {
            this.EndPointName = endPointName;
        }

        public EndPointID(Protocols protocol, string endPointName)
            : this(protocol, Guid.NewGuid(), endPointName)
        {
            this.Protocol = protocol;
        }
    }
}
