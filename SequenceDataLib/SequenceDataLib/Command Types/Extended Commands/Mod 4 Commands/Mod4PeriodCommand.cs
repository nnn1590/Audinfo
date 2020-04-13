﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequenceDataLib {

    /// <summary>
    /// Mod period command.
    /// </summary>
    public class Mod4PeriodCommand : SequenceCommand {

        /// <summary>
        /// Set defauls.
        /// </summary>
        public Mod4PeriodCommand() {

            //Set identifier.
            Identifier = (byte)ExtendedCommandType.Mod4Period;

            //Parameter types.
            SequenceParameterTypes = new List<SequenceParameterType>() { SequenceParameterType.u16 };

            //Set parameters.
            Parameters = new object[SequenceParameterTypes.Count];

            //Set thing.
            Period = 16;

        }

        /// <summary>
        /// Speed.
        /// </summary>
        public ushort Period {
            get { return (ushort)Parameters[0]; }
            set { Parameters[0] = value; }
        }

    }

}
