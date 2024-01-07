using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convergence
{
    public class EndPointID
    {
        public static EndPointID Empty { get; } = new EndPointID(Protocols.None, Guid.Empty);
        public Protocols Protocol { get; }
        public Guid Id { get; }
        public override string ToString()
        {
            return $"{nameof(Protocol)}:{Id}";
        }

        public EndPointID(Protocols protocol, Guid id)
        {
            Protocol = protocol;
            Id = id;
        }
    }
}
