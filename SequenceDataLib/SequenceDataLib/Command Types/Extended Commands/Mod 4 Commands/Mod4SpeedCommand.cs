﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequenceDataLib {

    /// <summary>
    /// Mod speed command.
    /// </summary>
    public class Mod4SpeedCommand : SequenceCommand {

        /// <summary>
        /// Set defauls.
        /// </summary>
        public Mod4SpeedCommand() {

            //Set identifier.
            Identifier = (byte)ExtendedCommandType.Mod4Speed;

            //Parameter types.
            SequenceParameterTypes = new List<SequenceParameterType>() { SequenceParameterType.s8 };

            //Set parameters.
            Parameters = new object[SequenceParameterTypes.Count];

            //Set thing.
            Speed = 16;

        }

        /// <summary>
        /// Speed.
        /// </summary>
        public sbyte Speed {
            get { return (sbyte)Parameters[0]; }
            set { Parameters[0] = value; }
        }

    }

}
