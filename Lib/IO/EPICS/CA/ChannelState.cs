namespace Convergence.IO.EPICS.CA
{
    public enum ChannelState
    {
        NeverConnected,      // 0 : Server not found or unavailable
        PreviouslyConnected, // 1 : Was previously connected to server ... SERVER MIGHT CHANGE ?????
        CurrentlyConnected,  // 2 : Is currently connected to server
        Closed               // 3 : Channel has been closed
    };

}
