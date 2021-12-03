namespace DerekWare.HomeAutomation.Common
{
    public interface IDescription
    {
        public string Description { get; }
    }

    public interface IFamily
    {
        public string Family { get; }
    }

    public interface IName
    {
        public string Name { get; }
    }

    // IMatch is an alternative to IEquatable when loose comparisons are appropriate
    public interface IMatch
    {
        public bool Matches(object other);
    }
}
