namespace LiveSplit.Model;

public static class AttemptDeletionHelper
{
    public static void DeleteAttempt(IRun run, int attemptIndex)
    {
        // Remove from AttemptHistory
        for (int i = 0; i < run.AttemptHistory.Count; i++)
        {
            if (run.AttemptHistory[i].Index == attemptIndex)
            {
                run.AttemptHistory.RemoveAt(i);
                break;
            }
        }

        // Remove from all SegmentHistories
        foreach (ISegment segment in run)
        {
            segment.SegmentHistory.Remove(attemptIndex);
        }
    }
}
