﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequenceDataLib {

    /// <summary>
    /// Compare less than or equal to command.
    /// </summary>
    public class CompareLessThanOrEqualToCommand : SequenceCommand {

        /// <summary>
        /// Set defaults.
        /// </summary>
        public CompareLessThanOrEqualToCommand() {

            //Set identifier.
            Identifier = (byte)ExtendedCommandType.CompareLessThanOrEqualTo;

            //Parameter types.
            SequenceParameterTypes = new List<SequenceParameterType>() { SequenceParameterType.u8, SequenceParameterType.s16};

            //Set parameters.
            Parameters = new object[SequenceParameterTypes.Count];

        }

        /// <summary>
        /// Variable.
        /// </summary>
        public byte Variable {
            get { return (byte)Parameters[0]; }
            set { Parameters[0] = value; }
        }

        /// <summary>
        /// Actual value.
        /// </summary>
        public short Value {
            get { return (short)Parameters[1]; }
            set { Parameters[1] = value; }
        }

    }

}
