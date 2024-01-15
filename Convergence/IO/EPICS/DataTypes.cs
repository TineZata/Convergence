using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convergence.IO.EPICS
{
    /// <summary>
    /// Enum PvaType
    /// </summary>
    public enum DataTypes
    {
        /// <summary>
        /// signed 8-bit integer
        /// </summary>
        PVA_int8,
        /// <summary>
        /// signed 16-bit integer
        /// </summary>
        PVA_int16,
        /// <summary>
        /// signed 32-bit integer
        /// </summary>
        PVA_int32,
        /// <summary>
        /// signed 64-bit integer
        /// </summary>
        PVA_int64,
        /// <summary>
        /// unsigned 8-bit integer
        /// </summary>
        PVA_uint8,
        /// <summary>
        /// unsigned 16-bit integer
        /// </summary>
        PVA_uint16,
        /// <summary>
        /// unsigned 32-bit integer
        /// </summary>
        PVA_uint32,
        /// <summary>
        /// unsigned 64-bit integer
        /// </summary>
        PVA_uint64,
        /// <summary>
        /// 32-bit floating point
        /// </summary>
        PVA_float32,
        /// <summary>
        /// 64-bit floating point
        /// </summary>
        PVA_float64,
        /// <summary>
        /// string of up to 39 characters
        /// </summary>
        PVA_string,
        /// <summary>
        /// boolean value
        /// </summary>
        PVA_boolean,
        /// <summary>
        /// string of up to 39 characters
        /// </summary>
        CA_DBF_STRING,
        /// <summary>
        /// 8-bit unsigned integer
        /// </summary>
        CA_DBF_CHAR,
        /// <summary>
        /// 16-bit signed integer
        /// </summary>
        CA_DBF_SHORT,
        /// <summary>
        /// 32-bit signed integer
        /// </summary>
        CA_DBF_LONG,
        /// <summary>
        /// single precision floating point, 32 bits
        /// </summary>
        CA_DBF_FLOAT,
        /// <summary>
        /// double precision floating point, 64 bits
        /// </summary>
        CA_DBF_DOUBLE,
        /// <summary>
        /// enumerated value with up to 16 options
        /// </summary>
        CA_DBF_ENUM
    }
}
