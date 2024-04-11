namespace LiveSplit.Web.SRL
{
    public class SRLIRCUser
    {
        public SRLIRCRights Rights { get; protected set; }
        public string Name { get; protected set; }

        public SRLIRCUser(string name, SRLIRCRights rights = SRLIRCRights.Normal)
        {
            Name = name;
            Rights = rights;
        }
    }
}
