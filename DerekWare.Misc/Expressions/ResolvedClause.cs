namespace DerekWare.Query
{
    public struct ResolvedClause
    {
        public IClause Resolved;
        public IClause Source;

        public override string ToString()
        {
            return Source?.ToString() ?? base.ToString();
        }
    }
}
