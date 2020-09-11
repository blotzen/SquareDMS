using System;
using System.Collections.Generic;

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
        public ManipulationResult(int errorCode, Operation manipulatedEntity,
            params Operation[] manipulatedEntities)
        {
            ErrorCode = errorCode;
            _manipulatedEntities.Add(manipulatedEntity);    // Trick for at least one entry...

            foreach (var manipulation in manipulatedEntities)
            {
                _manipulatedEntities.Add(manipulation);
            }
        }

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

        public int ErrorCode { get; }
    }
}