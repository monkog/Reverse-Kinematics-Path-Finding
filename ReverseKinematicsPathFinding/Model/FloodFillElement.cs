using System.Collections.Generic;

namespace ReverseKinematicsPathFinding.Model
{
    public class FloodFillElement
    {
        #region Public Properties

        /// <summary>
        /// Gets the sequence number.
        /// </summary>
        public int SequenceNumber { get; private set; }

        /// <summary>
        /// Gets the parents.
        /// </summary>
        public List<FloodFillElement> Parents { get; private set; }

        #endregion Public Properties

        #region Constructors

        public FloodFillElement(int sequenceNumber)
        {
            SequenceNumber = sequenceNumber;
            Parents = new List<FloodFillElement>();
        }

        #endregion Constructors
    }
}

