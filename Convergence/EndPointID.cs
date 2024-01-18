﻿using System;
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

        public EndPointID(Protocols ePICS, Guid guid, string endPointName)
            : this(ePICS, guid)
        {
            this.EndPointName = endPointName;
        }
    }
}
