namespace Convergence.IO.EPICS.CA
{
    // This is deliberately mutable ...
    // Hmm, is that a good idea ???
    // Might be better to allow a null instance ?

    internal class ChannelHandle
    {

        public ChannelHandle(nint? value = null)
        {
            Value = value ?? nint.Zero;
        }

        public nint Value { get; private set; }

        public bool IsValidHandle => !IsNull;

        public bool IsNull => Value == nint.Zero;

        public void SetAsNonNull(nint value)
        => Value = value!;

        public void SetAsNull() => Value = nint.Zero;

        // public static readonly ChannelHandle Zero = new(System.IntPtr.Zero) ;

        public static implicit operator nint(ChannelHandle channel)

        => channel.Value!;

        public static implicit operator ChannelHandle(nint pContext)
        => new ChannelHandle(pContext);
    }
}
