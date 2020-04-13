﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequenceDataLib {

    /// <summary>
    /// Mod phase command.
    /// </summary>
    public class ModDepthCommand : SequenceCommand {

        /// <summary>
        /// Set defauls.
        /// </summary>
        public ModDepthCommand() {

            //Set identifier.
            Identifier = (byte)CommandType.ModDepth;

            //Parameter types.
            SequenceParameterTypes = new List<SequenceParameterType>() { SequenceParameterType.s8 };

            //Set parameters.
            Parameters = new object[SequenceParameterTypes.Count];

            //Set thing.
            Depth = 0;

        }

        /// <summary>
        /// Depth.
        /// </summary>
        public sbyte Depth {
            get { return (sbyte)Parameters[0]; }
            set { Parameters[0] = value; }
        }

    }

}
