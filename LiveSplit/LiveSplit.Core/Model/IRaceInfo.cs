using System.Collections.Generic;

namespace LiveSplit.Model
{
    public interface IRaceInfo
    {
        int Finishes { get;  }
        int Forfeits { get; }
        string GameId { get; }
        string GameName { get; }
        string Goal { get; }
        string Id { get; }
        IEnumerable<string> LiveStreams { get; }
        int NumEntrants { get; }
        int Starttime { get; }
        int State { get; }

        bool IsParticipant(string username);
    }
}
