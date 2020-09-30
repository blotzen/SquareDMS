using SquareDMS.DataLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SquareDMS.DataLibrary.ProcedureResults
{
    public class ManipulationResult<T> : IProcedureResult where T : IDataTransferObject
    {
        private readonly List<Operation> _manipulatedEntities = new List<Operation>();

        /// <summary>
        /// Result for manipulations. Contains which entities have been manipulated
        /// in which amount.
        /// </summary>
        /// <exception cref="ArgumentException">Duplicate entry added to dict.</exception>
        public ManipulationResult(int errorCode, T manipulatedEntity, params Operation[] operations)
        {
            ErrorCode = errorCode;
            ManipulatedEntity = manipulatedEntity;

            foreach (var manipulation in operations)
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
        /// Contains the the manipulated entity (only the ids are filled). Currently
        /// only used for creating a entitys.
        /// </summary>
        public T ManipulatedEntity { get; set; } // for new feature: put payload into cache after insertion

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