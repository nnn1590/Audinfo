﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequenceDataLib {

    /// <summary>
    /// Call command.
    /// </summary>
    public class CallCommand : SequenceCommand {

        /// <summary>
        /// Set defaults.
        /// </summary>
        public CallCommand() {

            //Set identifier.
            Identifier = (byte)CommandType.Call;

            //Parameter types.
            SequenceParameterTypes = new List<SequenceParameterType>() { SequenceParameterType.u24 };

            //Set parameters.
            Parameters = new object[SequenceParameterTypes.Count];

            //Set thing.
            Offset = 0;

        }

        /// <summary>
        /// Offset from beginning of sequence data.
        /// </summary>
        public UInt24 Offset {
            get { return (UInt24)Parameters[0]; }
            set { Parameters[0] = value; }
        }

    }

}
