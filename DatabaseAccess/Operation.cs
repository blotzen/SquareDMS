using System;

namespace SquareDMS.DatabaseAccess
{
    public class Operation
    {
        /// <summary>
        /// Represents the result of a Operation. Show the affected entries
        /// and the corresponding Entity.
        /// </summary>
        public Operation(Type entity, int affectedEntries, OperationType operationType)
        {
            Entity = entity;
            AffectedEntries = affectedEntries;
            OperationType = operationType;
        }

        public Type Entity { get; }

        public int AffectedEntries { get; }

        public OperationType OperationType { get; }
    }
}
