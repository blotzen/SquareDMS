using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;

namespace SquareDMS.DataLibrary.ProcedureResults
{
    public class ManipulationResult : IProcedureResult
    {
        private readonly List<Operation> _manipulatedEntities = new List<Operation>();

        /// <summary>
        /// Result for manipulations. Contains which entities have been manipulated
        /// in which amount.
        /// </summary>
        /// <exception cref="ArgumentException">Duplicate entry added to dict.</exception>
        public ManipulationResult(int errorCode, params Operation[] manipulatedEntities)
        {
            ErrorCode = errorCode;

            foreach (var manipulation in manipulatedEntities)
            {
                _manipulatedEntities.Add(manipulation);
            }
        }

        /// <summary>
        /// Operations that have been made
        /// </summary>
        public ReadOnlyCollection<Operation> Operations
        {
            get => new ReadOnlyCollection<Operation>(_manipulatedEntities);
        }

        public int ErrorCode { get; private set; }

        /// <summary>
        /// Contains the id of the manipulated entity. (null if not necessary)
        /// </summary>
        // public int? ManipulatedId { get; set; } // for new feature: put payload into cache after insertion

        /// <summary>
        /// Gets the manipulated amount of entities by the entity and the operation
        /// </summary>
        public int ManipulatedAmount(Type entity, OperationType op)
        {
            // linear search through list
            foreach (var element in _manipulatedEntities)
            {
                if (entity.Equals(element.Entity) && op.Equals(element.OperationType))
                {
                    return element.AffectedEntries;
                }
            }

            return 0;
        }
    }
}