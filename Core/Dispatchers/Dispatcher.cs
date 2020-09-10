namespace SquareDMS.Core.Dispatchers
{
    /// <summary>
    /// Abstract base class for all dispatchers
    /// </summary>
    public abstract class Dispatcher
    {
        public Dispatcher()
        {
            DbConnectionString = Globals.MSSQL_CONNECTION_STRING;
        }

        public string DbConnectionString { get; }
    }
}
