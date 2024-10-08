namespace Convergence.IO.EPICS.CA
{

    //
    // These are the 'primitive types' that we deal with.
    //
    // A PV 'field' can have a value that's a single instance, or an array.
    //
    // It's only the VAL field that can return an array !!
    // Other fields can only return a scalar value,
    // ie an array with a single element. 
    //

    /// <summary>
    /// Type of a PV's value : string, short, float, double, enum etc.
    /// </summary>

    public enum DbFieldType : short
    {
        DBF_STRING_s39 = ChannelAccessConstants.DBF_STRING, // 0 ; Up to 39 characters
        DBF_SHORT_i16 = ChannelAccessConstants.DBF_SHORT,  // 1 ; 16 bit integer
        DBF_FLOAT_f32 = ChannelAccessConstants.DBF_FLOAT,  // 2 ; Floating point single precision
        DBF_ENUM_i16 = ChannelAccessConstants.DBF_ENUM,   // 3 ; 16 bit integer, 0..15 max
        DBF_CHAR_byte = ChannelAccessConstants.DBF_CHAR,   // 4 ; Char == Byte
        DBF_LONG_i32 = ChannelAccessConstants.DBF_LONG,   // 5 ; Long == 32 bit integer
        DBF_DOUBLE_f64 = ChannelAccessConstants.DBF_DOUBLE,  // 6 ; Floating point double precision 
        // Control fields
        DBR_CTRL_STRING = ChannelAccessConstants.DBR_CTRL_STRING,
        DBR_CTRL_SHORT = ChannelAccessConstants.DBR_CTRL_SHORT,
        DBR_CTRL_INT = DBR_CTRL_SHORT,
        DBR_CTRL_FLOAT = ChannelAccessConstants.DBR_CTRL_FLOAT,
        DBR_CTRL_ENUM = ChannelAccessConstants.DBR_CTRL_ENUM,
        DBR_CTRL_CHAR = ChannelAccessConstants.DBR_CTRL_CHAR,
        DBR_CTRL_LONG = ChannelAccessConstants.DBR_CTRL_LONG,
        DBR_CTRL_DOUBLE = ChannelAccessConstants.DBR_CTRL_DOUBLE,
    }

}
