﻿using System;
using System.IO;
using System.Collections.Generic;
using Syroot.BinaryData;
using System.Diagnostics;
using IsabelleLib;

/// <summary>
/// Citra structures.
/// </summary>
namespace CitraFileLoaderz
{   

	//Endianess
	public enum endianNess {little, big};


	/// <summary>
	/// String block.
	/// </summary>
	public class strgBlock {

        //Endian.
        public endianNess endian;

        //General stuff.
        public char[] magic; //STRG.
        public UInt32 fileSize; //File size.

        public UInt16 stringTableIdentifier; //0x2400.
        public UInt16 reservedTable; //Reserved space.
        public UInt32 stringTableOffset; //Add 8 to get offset.

        public UInt16 lookupTableIdentifier; //0x2401.
        public UInt16 reservedLookup; //Reserved space.
        public UInt32 lookupTableOffset; //Add 8 to get offset.

        public stringTableRecords tableRecord; //Table record.
		public List<stringEntry> stringEntries; //String entries.

		public lookupTableRecords lookupRecord; //Lookup record.


        /// <summary>
        /// String Table Records.
        /// </summary>
        public struct stringTableRecords {

            public UInt32 nCount; //Amount of records.
            public List<stringTableRecord> records; //Records.

        }


		/// <summary>
		/// Record in a string table.
		/// </summary>
        public struct stringTableRecord {

            public UInt32 identifier; //0x1F01
            public UInt32 offset; //Relative to start of table.
            public UInt32 length; //Subtract by 1 to get the string.

        }


		//String entries.
		public struct stringEntry {

			public char[] data; //String data.
			public byte seperator; //Seperator.

		}


		/// <summary>
		/// Lookup table records.
		/// </summary>
		public struct lookupTableRecords {

			public UInt32 rootNode; //Root node.
			public UInt32 amountOfNodes; //Amount of records.
			public List<lookupTableRecord> record; //Records
		
		}


		/// <summary>
		/// Lookup table record.
		/// </summary>
		public struct lookupTableRecord {

			public UInt16 leafNodeFlag; //1 if leaf node.
			public UInt16 searchIndex; //Bit index from left.
			public UInt32 leftIndex; //Bit index from left.
			public UInt32 rightIndex; //Bit index from right.
			public UInt32 stringIndex; //Index of string.
			public UInt32 id; //ID of this node.

		}



		/// <summary>
		/// Load a file.
		/// </summary>
		/// <param name="b">The blue component.</param>
		public void load(byte[] b, endianNess endian) {

			//Set endian.
			this.endian = endian;

			//Reader.
			MemoryStream src = new MemoryStream (b);
			BinaryDataReader br = new BinaryDataReader (src);

			//Endian.
			if (endian == endianNess.big) {
				br.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
			} else {
				br.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
			}

			//Stuff.
			magic = br.ReadChars(4);
			fileSize = br.ReadUInt32 ();

			stringTableIdentifier = br.ReadUInt16 ();
			reservedTable = br.ReadUInt16 ();
			stringTableOffset = br.ReadUInt32 () + 8;

			lookupTableIdentifier = br.ReadUInt16 ();
			reservedLookup = br.ReadUInt16 ();
			lookupTableOffset = br.ReadUInt32 () + 8;

            //String table.
            br.Position = (int)stringTableOffset;
            tableRecord.nCount = br.ReadUInt32();
            tableRecord.records = new List<stringTableRecord>();
            for (int i = 0; i < (int)tableRecord.nCount; i++) {

                stringTableRecord s = new stringTableRecord();
                s.identifier = br.ReadUInt32();
                s.offset = br.ReadUInt32();
                s.length = br.ReadUInt32();
                tableRecord.records.Add(s);

            }

            //Read strings.
            stringEntries = new List<stringEntry>();
            for (int i = 0; i < (int)tableRecord.nCount; i++) {

                br.Position = (int)tableRecord.records[i].offset + 24;
                stringEntry s = new stringEntry();
                s.data = br.ReadChars((int)tableRecord.records[i].length - 1);
                s.seperator = br.ReadByte();
                stringEntries.Add(s);

            }

            //Read lookup table.
            br.Position = (int)lookupTableOffset;
            lookupRecord.rootNode = br.ReadUInt32();
            lookupRecord.amountOfNodes = br.ReadUInt32();
            lookupRecord.record = new List<lookupTableRecord>();
            for (int i = 0; i < (int)lookupRecord.amountOfNodes; i++)
            {

                lookupTableRecord s = new lookupTableRecord();
                s.leafNodeFlag = br.ReadUInt16();
                s.searchIndex = br.ReadUInt16();
                s.leftIndex = br.ReadUInt32();
                s.rightIndex = br.ReadUInt32();
                s.stringIndex = br.ReadUInt32();
                s.id = br.ReadUInt32();
                lookupRecord.record.Add(s);

            }

        }



		public byte[] toBytes(endianNess endian) {
			
			byte[] b = new byte[2];
			return b;

		}

	}


	/// <summary>
	/// Info block.
	/// </summary>
	public class infoBlock {

		byte[] file;


		/// <summary>
		/// Load a file.
		/// </summary>
		/// <param name="b">The blue component.</param>
		public void load(byte[] b, endianNess endian) {

			file = b;

		}


		public byte[] toBytes(endianNess endian) {
			return file;
		}

	}


	/// <summary>
	/// File block.
	/// </summary>
	public class fileBlock {

		byte[] file;


		/// <summary>
		/// Load a file.
		/// </summary>
		/// <param name="b">The blue component.</param>
		public void load(byte[] b, endianNess endian) {

			file = b;

		}


		public byte[] toBytes(endianNess endian) {
			return file;
		}


    }


    /// <summary>
    /// Stream file.
    /// </summary>
    public class b_stmO
    {

        string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        public endianNess endian; //Endian.
        public int numSamples = -1; //Number of samples.

        public char[] magic; //CSTM, FSTM.
        public UInt16 byteOrder; //0xFEFF Big, 0xFFFE Small.
        public UInt16 headerSize; //Header size.
        public UInt32 version; //File version.
        public UInt32 fileSize; //File size.
        public UInt16 nBlocks; //Always 3.
        public UInt16 padding; //Padding.

        public sizedReference infoRecord; //Info record.
        public sizedReference seekRecord; //Seek record.
        public sizedReference dataRecord; //Data record.

        public byte[] reserved; //Reserved space for alignment.

        public infoBlock info;
        public seekBlock seek;
        public dataBlock data;


        /// <summary>
        /// Info block. 0x20 aligned.
        /// </summary>
        public struct infoBlock
        {

            public char[] magic; //INFO.
            public UInt32 size; //Size of this block.

            public reference streamRecord; //Stream record.
            public reference trackRecord; //Track record. Leads to track references.
            public reference channelRecord; //Channel record.

            public referenceTable trackReferences; //References to all the tracks.
            public referenceTable channelReferences; //References to all the channels.

            public streamInfo stream; //Stream info.
            public List<trackInfo> track; //Track info.
            public List<channelInfo> channel; //Channel info.



            /// <summary>
            /// Stream info.
            /// </summary>
            public struct streamInfo
            {

                public byte encoding; //0 = PCM8, 1 = PCM16, 2 = DSP ADPCM, 3 = IMA ADPCM.
                public byte loop; //I'm no expert, but this probably signifies a loop.
                public byte numberOfChannels; //How many channels.
                public byte numberOfRegions; //Used to define the amount of regions in Pokemon Red.
                public UInt32 sampleRate; //Rate of sampling.
                public UInt32 loopStart; //Loop start.
                public UInt32 loopEnd; //Loop end. If nonexistant, number of frames.
                public UInt32 sampleBlockCount; //Number of sample blocks.
                public UInt32 sampleBlockSize; //Size of sample block.
                public UInt32 sampleBlockSampleCount; //Amount of samples in the sample block which is shoved with samples in the sample.
                public UInt32 lastSampleBlockSize; //Size of the last sample block, no padding.
                public UInt32 lastSampleBlockSampleCount; //Samples in the last block.
                public UInt32 lastSampleBlockPaddingSize; //Size of padding in last sample block.
                public UInt32 seekSize; //Size of seek data. Seems to be 4 always?
                public UInt32 seekIntervalSampleCount; //Seek interval sample count interval. Always 38?
                public reference sampleRecord; //Sample record.


                //For V > 2.1? and above.
                public UInt16 regionSize; //Region Size?????
                public UInt16 padding; //Padding.
                public reference regionRecord; //Region record.

                //For V4 and above.
                public UInt32 originalLoopStart; //Loop start. (AKA 0)
                public UInt32 originalLoopEnd; //Loop End. (AKA number of frames)

            }


            /// <summary>
            /// Track info.
            /// </summary>
            public struct trackInfo
            {

                public byte volume; //Volume.
                public byte pan; //Pan.
                public UInt16 flags; //Front Bypass???
                public reference byteTableRecord; //Byte table reference.
                public byteTableTrack byteTable; //Byte table.

                public byte[] reserved; //Extra 0s to make structure divisible by 4.

                /// <summary>
                /// Byte table structure.
                /// </summary>
                public struct byteTableTrack
                {

                    public UInt32 count;
                    public List<byte> channelIndexes; //Channel indexes. Index of sample to use for the track.

                }

            }


            /// <summary>
            /// Channel info.
            /// </summary>
            public struct channelInfo
            {

                public reference sampleRecord; //Sample reference.

                public dspAdpcmInfo dspAdpcm; //Adpcm.
                public imaAdpcmInfo imaAdpcm; //Adpcm.

            }


            /// <summary>
            /// Adpcm info.
            /// </summary>
            public struct dspAdpcmInfo
            {

                public Int16[] coefficients; //16 ADPCM coefficients.
                public UInt16 predScale; //Predictor scale.
                public UInt16 yn1; //History sample 1.
                public UInt16 yn2; //History sample 2.
                public UInt16 loopPredScale; //Loop predictor scale.
                public UInt16 loopYn1; //Loop History sample.
                public UInt16 loopYn2; //Loop History sample 2.
                public UInt16 padding; //Padding.

            }


            /// <summary>
            /// Ima adpcm info.
            /// </summary>
            public struct imaAdpcmInfo
            {

                public UInt16 data; //Data.
                public byte tableIndex; //Table Index.
                public byte padding; //Padding.

                public UInt16 loopData; //Loop data.
                public byte loopTableIndex; //Loop table index.
                public byte loopPadding; //Loop padding.

            }


        }


        /// <summary>
        /// Seek block. 0x20 aligned.
        /// </summary>
        public struct seekBlock
        {

            public char[] magic; //Magic.
            public UInt32 size; //Blocksize.

            public byte[] data; //Data.

        }



        /// <summary>
        /// Data block. 0x20 aligned.
        /// </summary>
        public struct dataBlock
        {

            public char[] magic; //DATA.
            public UInt32 fileSize; //File size.
            public byte[] padding; //Extra thicc padding.

            public List<byte[]> samples; //Samples.
            public List<UInt16[]> pcm16; //PCM16.

            public byte[] reserved; //Resered.

        }


        /// <summary>
        /// A reference used to an offset.
        /// </summary>
        public struct reference
        {

            public UInt16 identifier; //Identifier.
                                      /*
                                          0x0100	Byte Table
                                          0x0101	Reference Table
                                          0x0300	DSP ADPCM Info
                                          0x0301	IMA ADPCM Info
                                          0x1F00	Sample Data
                                          0x4000	Info Block
                                          0x4001	Seek Block
                                          0x4002	Data Block
                                          0x4100	Stream Info
                                          0x4101	Track Info
                                          0x4102	Channel Info
                                          */
            public UInt16 padding; //Padding woo.
            public UInt32 offset; //Offset.

        }


        /// <summary>
        /// Sized reference.
        /// </summary>
        public struct sizedReference
        {

            public reference r; //Reference.
            public UInt32 size; //Size.

        }


        /// <summary>
        /// Reference table.
        /// </summary>
        public struct referenceTable
        {

            public UInt32 count; //Count of references.
            public List<reference> references; //References.

        }


        /// <summary>
        /// Load a file.
        /// </summary>
        /// <param name="b"></param>
        public void load(byte[] b)
        {

            MemoryStream src = new MemoryStream(b);
            BinaryDataReader br = new BinaryDataReader(src);
            br.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            //Read magic.
            magic = br.ReadChars(4);

            //Get endianess.
            byteOrder = br.ReadUInt16();
            if (byteOrder == 0xFFFE)
            {
                byteOrder = 0xFEFF;
                endian = endianNess.little;
                br.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
            }
            else
            {
                byteOrder = 0xFFFE;
                endian = endianNess.big;
                br.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
            }

            //Get header length.
            headerSize = br.ReadUInt16();
            version = br.ReadUInt32();
            fileSize = br.ReadUInt32();
            nBlocks = br.ReadUInt16();
            padding = br.ReadUInt16();

            infoRecord = new sizedReference();
            seekRecord = new sizedReference();
            dataRecord = new sizedReference();

            infoRecord.r.identifier = br.ReadUInt16();
            infoRecord.r.padding = br.ReadUInt16();
            infoRecord.r.offset = br.ReadUInt32();
            infoRecord.size = br.ReadUInt32();

            seekRecord.r.identifier = br.ReadUInt16();
            seekRecord.r.padding = br.ReadUInt16();
            seekRecord.r.offset = br.ReadUInt32();
            seekRecord.size = br.ReadUInt32();

            dataRecord.r.identifier = br.ReadUInt16();
            dataRecord.r.padding = br.ReadUInt16();
            dataRecord.r.offset = br.ReadUInt32();
            dataRecord.size = br.ReadUInt32();

            reserved = br.ReadBytes((int)(headerSize - br.Position));

            if ((int)nBlocks == 2)
            {
                dataRecord = seekRecord;
            }

            //Read INFO.
            br.Position = (int)infoRecord.r.offset;
            info.magic = br.ReadChars(4);
            info.size = br.ReadUInt32();

            info.streamRecord = new reference();
            info.trackRecord = new reference();
            info.channelRecord = new reference();
            info.trackReferences = new referenceTable();
            info.channelReferences = new referenceTable();
            info.track = new List<infoBlock.trackInfo>();
            info.channel = new List<infoBlock.channelInfo>();

            info.streamRecord.identifier = br.ReadUInt16();
            info.streamRecord.padding = br.ReadUInt16();
            info.streamRecord.offset = br.ReadUInt32();

            info.trackRecord.identifier = br.ReadUInt16();
            info.trackRecord.padding = br.ReadUInt16();
            info.trackRecord.offset = br.ReadUInt32();

            info.channelRecord.identifier = br.ReadUInt16();
            info.channelRecord.padding = br.ReadUInt16();
            info.channelRecord.offset = br.ReadUInt32();

            //Stream info.
            br.Position = (int)(infoRecord.r.offset + 8 + info.streamRecord.offset);
            info.stream.encoding = br.ReadByte();
            info.stream.loop = br.ReadByte();
            info.stream.numberOfChannels = br.ReadByte();
            info.stream.numberOfRegions = br.ReadByte();
            info.stream.sampleRate = br.ReadUInt32();
            info.stream.loopStart = br.ReadUInt32();
            info.stream.loopEnd = br.ReadUInt32();
            info.stream.sampleBlockCount = br.ReadUInt32();
            info.stream.sampleBlockSize = br.ReadUInt32();
            info.stream.sampleBlockSampleCount = br.ReadUInt32();
            info.stream.lastSampleBlockSize = br.ReadUInt32();
            info.stream.lastSampleBlockSampleCount = br.ReadUInt32();
            info.stream.lastSampleBlockPaddingSize = br.ReadUInt32();
            info.stream.seekSize = br.ReadUInt32();
            info.stream.seekIntervalSampleCount = br.ReadUInt32();

            numSamples = (int)(info.stream.sampleBlockCount * info.stream.sampleBlockSampleCount + info.stream.lastSampleBlockSampleCount);

            info.stream.sampleRecord = new reference();
            info.stream.sampleRecord.identifier = br.ReadUInt16();
            info.stream.sampleRecord.padding = br.ReadUInt16();
            info.stream.sampleRecord.offset = br.ReadUInt32();

            //Track and channel records.
            if (info.trackRecord.offset != 0xFFFFFFFF) //Could be null if no tracks.
            {
                br.Position = (int)(infoRecord.r.offset + 8 + info.trackRecord.offset);
                info.trackReferences = new referenceTable();
                info.trackReferences.references = new List<reference>();
                info.trackReferences.count = br.ReadUInt32();
                for (int i = 0; i < (int)info.trackReferences.count; i++)
                {
                    try
                    {
                        reference r = new reference();
                        r.identifier = br.ReadUInt16();
                        r.padding = br.ReadUInt16();
                        r.offset = br.ReadUInt32();
                        info.trackReferences.references.Add(r);
                    } catch { }
                }
            }
            br.Position = (int)(infoRecord.r.offset + 8 + info.channelRecord.offset);
            info.channelReferences = new referenceTable();
            info.channelReferences.references = new List<reference>();
            info.channelReferences.count = br.ReadUInt32();
            for (int i = 0; i < (int)info.channelReferences.count; i++)
            {
                reference r = new reference();
                r.identifier = br.ReadUInt16();
                r.padding = br.ReadUInt16();
                r.offset = br.ReadUInt32();
                info.channelReferences.references.Add(r);
            }

            //Read tracks.
            if (info.trackRecord.offset != 0xFFFFFFFF)
            {

                foreach (reference r in info.trackReferences.references)
                {
                    try
                    {
                        br.Position = (UInt32)(infoRecord.r.offset + 8 + info.trackRecord.offset + r.offset);

                        infoBlock.trackInfo t = new infoBlock.trackInfo();
                        t.volume = br.ReadByte();
                        t.pan = br.ReadByte();
                        t.flags = br.ReadUInt16();

                        t.byteTableRecord = new reference();
                        t.byteTableRecord.identifier = br.ReadUInt16();
                        t.byteTableRecord.padding = br.ReadUInt16();
                        t.byteTableRecord.offset = br.ReadUInt32();

                        br.Position = (UInt32)(infoRecord.r.offset + 8 + info.trackRecord.offset + r.offset + t.byteTableRecord.offset);
                        t.byteTable = new infoBlock.trackInfo.byteTableTrack();
                        t.byteTable.count = br.ReadUInt32();
                        t.byteTable.channelIndexes = new List<byte>();
                        for (int j = 0; j < t.byteTable.count; j++)
                        {
                            t.byteTable.channelIndexes.Add(br.ReadByte());
                        }

                        int reservedSize = (int)t.byteTable.count;
                        while (reservedSize % 4 != 0)
                        {
                            reservedSize += 1;
                        }
                        t.reserved = br.ReadBytes(reservedSize - (int)t.byteTable.count);

                        info.track.Add(t);
                    }
                    catch { }
                }
            }

            //Read channels.
            info.channel = new List<infoBlock.channelInfo>();
            foreach (reference r in info.channelReferences.references)
            {

                br.Position = (UInt32)(infoRecord.r.offset + 8 + info.channelRecord.offset + r.offset);

                infoBlock.channelInfo c = new infoBlock.channelInfo();
                c.sampleRecord = new reference();
                c.sampleRecord.identifier = br.ReadUInt16();
                c.sampleRecord.padding = br.ReadUInt16();
                c.sampleRecord.offset = br.ReadUInt32();

                c.dspAdpcm = new infoBlock.dspAdpcmInfo();
                c.imaAdpcm = new infoBlock.imaAdpcmInfo();

                if (info.stream.encoding == 2)
                {

                    br.Position = (UInt32)(infoRecord.r.offset + 8 + info.channelRecord.offset + r.offset + c.sampleRecord.offset);

                    c.dspAdpcm.coefficients = br.ReadInt16s(16);
                    c.dspAdpcm.predScale = br.ReadUInt16();
                    c.dspAdpcm.yn1 = br.ReadUInt16();
                    c.dspAdpcm.yn2 = br.ReadUInt16();
                    c.dspAdpcm.loopPredScale = br.ReadUInt16();
                    c.dspAdpcm.loopYn1 = br.ReadUInt16();
                    c.dspAdpcm.loopYn2 = br.ReadUInt16();
                    c.dspAdpcm.padding = br.ReadUInt16();

                }
                else if (info.stream.encoding == 3)
                {

                    br.Position = (UInt32)(infoRecord.r.offset + 8 + info.channelRecord.offset + r.offset + c.sampleRecord.offset);

                    c.imaAdpcm.data = br.ReadUInt16();
                    c.imaAdpcm.tableIndex = br.ReadByte();
                    c.imaAdpcm.padding = br.ReadByte();

                    c.imaAdpcm.loopData = br.ReadUInt16();
                    c.imaAdpcm.loopTableIndex = br.ReadByte();
                    c.imaAdpcm.loopPadding = br.ReadByte();

                }

                info.channel.Add(c);

            }


            //Read SEEK.
            br.Position = (int)seekRecord.r.offset;
            seek.magic = br.ReadChars(4);
            seek.size = br.ReadUInt32();
            seek.data = br.ReadBytes((int)seek.size - 8);

            //Read DATA.
            br.Position = (int)dataRecord.r.offset;
            data.magic = br.ReadChars(4);
            data.fileSize = br.ReadUInt32();
            data.padding = br.ReadBytes((int)info.stream.sampleRecord.offset);


            //Get padding size.
            int paddingSize = (int)(info.stream.lastSampleBlockPaddingSize - info.stream.lastSampleBlockSize);


            //If Non-PCM16.
            if (info.stream.encoding != 1)
            {

                br.Position = (int)(dataRecord.r.offset + 8 + data.padding.Length);

                byte[][] rawSampleData = new byte[(int)info.stream.numberOfChannels + 1][];
                rawSampleData[0] = br.ReadBytes((int)(info.stream.numberOfChannels * (info.stream.sampleBlockSize * (info.stream.sampleBlockCount - 1))));
                for (int i = 1; i < info.stream.numberOfChannels + 1; i++)
                {

                    rawSampleData[i] = br.ReadBytes((int)info.stream.lastSampleBlockSize);
                    br.ReadBytes(paddingSize);

                }

                //Combine all the data into a huge byte array.
                MemoryStream o2 = new MemoryStream();
                BinaryDataWriter bw2 = new BinaryDataWriter(o2);
                foreach (byte[] by in rawSampleData)
                {
                    bw2.Write(by);
                }
                byte[] sampleDataCombined = o2.ToArray();
                MemoryStream src2 = new MemoryStream(sampleDataCombined);
                BinaryDataReader br2 = new BinaryDataReader(src2);

                //Now time for the hard part: trying to figure out how the f to sort them by channel.
                /*
                List<byte[]>[] sampleData = new List<byte[]>[info.stream.numberOfChannels];
                for (int i = 0; i < info.stream.numberOfChannels; i++) {

                    br2.Position = 0;

                    sampleData[i] = new List<byte[]>();
                    for (int j = 0; j < info.stream.sampleBlockCount - 1; j++) {

                        for (int k = 0; k < i; k++) {
                            br2.ReadBytes((int)info.stream.sampleBlockSize);
                        }

                        sampleData[i].Add(br2.ReadBytes((int)info.stream.sampleBlockSize));

                        for (int l = i; l < info.stream.numberOfChannels-1; l++)
                        {
                            br2.ReadBytes((int)info.stream.sampleBlockSize);
                        }

                    }

                    for (int m = 0; m < i; m++)
                    {
                        br2.ReadBytes((int)info.stream.lastSampleBlockSize);
                    }

                    sampleData[i].Add(br2.ReadBytes((int)info.stream.lastSampleBlockSize));

                }

				//Now convert to a list of samples.
				data.samples = new List<byte[]>();
				foreach (List<byte[]> channel in sampleData) {
				
					MemoryStream o3 = new MemoryStream ();
					BinaryWriter bw3 = new BinaryWriter (o3);

					foreach (byte[] dat in channel) {
						bw3.Write (dat);
					}

					data.samples.Add(o3.ToArray());
				
				}*/

                //Read each block.
                List<byte[]> blocks = new List<byte[]>();
                for (int i = 0; i < (info.stream.sampleBlockCount - 1) * info.stream.numberOfChannels; i++)
                {
                    blocks.Add(br2.ReadBytes((int)info.stream.sampleBlockSize));
                }
                for (int i = 0; i < info.stream.numberOfChannels; i++)
                {
                    blocks.Add(br2.ReadBytes((int)info.stream.lastSampleBlockSize));
                }

                //Convert blocks to samples.
                List<byte[]>[] sampleData = new List<byte[]>[(int)info.stream.numberOfChannels];
                for (int i = 0; i < sampleData.Length; i++)
                {

                    sampleData[i] = new List<byte[]>();

                    for (int j = i; j < blocks.Count; j += info.stream.numberOfChannels)
                    {

                        sampleData[i].Add(blocks[j]);

                    }

                }

                //Now convert to a list of samples.
                data.samples = new List<byte[]>();
                foreach (List<byte[]> channel in sampleData)
                {

                    MemoryStream o3 = new MemoryStream();
                    BinaryWriter bw3 = new BinaryWriter(o3);

                    foreach (byte[] dat in channel)
                    {
                        bw3.Write(dat);
                    }

                    data.samples.Add(o3.ToArray());

                }



            }

            //PCM16. Sorry, I can't fucking do this. PCM16 streams ONLY HAVE 2 BLOCKS, and it fucks with all my code. Sorry for the dissapointment, but other tools exist.
            else
            {

                throw new NotImplementedException();

                /*
                br.Position = (int)(dataRecord.r.offset + 8 + data.padding.Length);

                UInt16[][] rawSampleData = new UInt16[(int)info.stream.numberOfChannels + 1][];
                rawSampleData[0] = br.ReadUInt16s((int)(info.stream.numberOfChannels * (info.stream.sampleBlockSampleCount * (info.stream.sampleBlockCount - 1))));
                for (int i = 1; i < info.stream.numberOfChannels + 1; i++)
                {

                    rawSampleData[i] = br.ReadUInt16s((int)info.stream.lastSampleBlockSampleCount);
                    br.ReadUInt16s(paddingSize/2);

                }

                //Combine all the data into a huge byte array.
                MemoryStream o2 = new MemoryStream();
                BinaryDataWriter bw2 = new BinaryDataWriter(o2);
                bw2.ByteOrder = bw2.ByteOrder;
                foreach (UInt16[] by in rawSampleData)
                {
                    bw2.Write(by);
                }
                byte[] sampleDataCombined = o2.ToArray();
                MemoryStream src2 = new MemoryStream(sampleDataCombined);
                BinaryDataReader br2 = new BinaryDataReader(src2);
                br2.ByteOrder = br.ByteOrder;


                //Read each block.
                List<UInt16[]> blocks = new List<UInt16[]>();
                for (int i = 0; i < (info.stream.sampleBlockCount - 1) * info.stream.numberOfChannels; i++)
                {
                    blocks.Add(br2.ReadUInt16s((int)info.stream.sampleBlockSampleCount));
                }
                for (int i = 0; i < info.stream.numberOfChannels; i++)
                {
                    blocks.Add(br2.ReadUInt16s((int)info.stream.lastSampleBlockSampleCount));
                }

                //Convert blocks to samples.
                List<UInt16[]>[] sampleData = new List<UInt16[]>[(int)info.stream.numberOfChannels];
                for (int i = 0; i < sampleData.Length; i++)
                {

                    sampleData[i] = new List<UInt16[]>();

                    for (int j = i; j < blocks.Count; j += info.stream.numberOfChannels)
                    {

                        sampleData[i].Add(blocks[j]);

                    }

                }

                //Now convert to a list of samples.
                data.pcm16 = new List<UInt16[]>();
                List<byte[]> channelData = new List<byte[]>();
                foreach (List<UInt16[]> channel in sampleData)
                {

                    MemoryStream o3 = new MemoryStream();
                    BinaryDataWriter bw3 = new BinaryDataWriter(o3);
                    bw3.ByteOrder = br.ByteOrder;

                    foreach (UInt16[] dat in channel)
                    {
                        bw3.Write(dat);
                    }

                    channelData.Add(o3.ToArray());

                }

                //Write read the blocks.
                foreach (byte[] block in channelData) {

                    BinaryDataReader br4 = new BinaryDataReader(new MemoryStream(block));
                    br4.ByteOrder = br.ByteOrder;

                    List<UInt16> samplers = new List<UInt16>();
                    while (br4.Position != block.Length) {
                        samplers.Add(br4.ReadUInt16());
                    }

                    data.pcm16.Add(samplers.ToArray());

                }
                */
            }


        }





        /// <summary>
        /// Convert to bytes.
        /// </summary>
        /// <param name="b"></param>
        public byte[] toBytes2(endianNess e)
        {

            update(e, false);

            MemoryStream o = new MemoryStream();
            BinaryDataWriter bw = new BinaryDataWriter(o);

            //Magic.
            bw.Write(magic);

            //Get endianess.
            if (e == endianNess.little)
            {
                bw.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
            }
            else
            {
                bw.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
            }
            bw.Write(byteOrder);

            //Header stuff.
            bw.Write(headerSize);
            bw.Write(version);
            bw.Write(fileSize);
            bw.Write(nBlocks);
            bw.Write(padding);

            bw.Write(infoRecord.r.identifier);
            bw.Write(infoRecord.r.padding);
            bw.Write(infoRecord.r.offset);
            bw.Write(infoRecord.size);

            bw.Write(seekRecord.r.identifier);
            bw.Write(seekRecord.r.padding);
            bw.Write(seekRecord.r.offset);
            bw.Write(seekRecord.size);

            bw.Write(dataRecord.r.identifier);
            bw.Write(dataRecord.r.padding);
            bw.Write(dataRecord.r.offset);
            bw.Write(dataRecord.size);

            //Write padding.
            while (bw.Position % 0x20 != 0)
            {
                bw.Write((byte)0);
            }


            //Write INFO.
            bw.Write(info.magic);
            bw.Write(info.size);

            bw.Write(info.streamRecord.identifier);
            bw.Write(info.streamRecord.padding);
            bw.Write(info.streamRecord.offset);

            bw.Write(info.trackRecord.identifier);
            bw.Write(info.trackRecord.padding);
            bw.Write(info.trackRecord.offset);

            bw.Write(info.channelRecord.identifier);
            bw.Write(info.channelRecord.padding);
            bw.Write(info.channelRecord.offset);

            //Stream info.
            bw.Write(info.stream.encoding);
            bw.Write(info.stream.loop);
            bw.Write(info.stream.numberOfChannels);
            bw.Write(info.stream.numberOfRegions);
            bw.Write(info.stream.sampleRate);
            bw.Write(info.stream.loopStart);
            bw.Write(info.stream.loopEnd);
            bw.Write(info.stream.sampleBlockCount);
            bw.Write(info.stream.sampleBlockSize);
            bw.Write(info.stream.sampleBlockSampleCount);
            bw.Write(info.stream.lastSampleBlockSize);
            bw.Write(info.stream.lastSampleBlockSampleCount);
            bw.Write(info.stream.lastSampleBlockPaddingSize);
            bw.Write(info.stream.seekSize);
            bw.Write(info.stream.seekIntervalSampleCount);

            bw.Write(info.stream.sampleRecord.identifier);
            bw.Write(info.stream.sampleRecord.padding);
            bw.Write(info.stream.sampleRecord.offset);


            //Write track and channel records. I un-nullify all records so I write them.
            bw.Write(info.trackReferences.count);
            foreach (reference r in info.trackReferences.references)
            {
                bw.Write(r.identifier);
                bw.Write(r.padding);
                bw.Write(r.offset);
            }

            bw.Write(info.channelReferences.count);
            foreach (reference r in info.channelReferences.references)
            {
                bw.Write(r.identifier);
                bw.Write(r.padding);
                bw.Write(r.offset);
            }


            //Write tracks.
            foreach (infoBlock.trackInfo t in info.track)
            {

                bw.Write(t.volume);
                bw.Write(t.pan);
                bw.Write(t.flags);

                bw.Write(t.byteTableRecord.identifier);
                bw.Write(t.byteTableRecord.padding);
                bw.Write(t.byteTableRecord.offset);

                bw.Write(t.byteTable.count);
                for (int j = 0; j < t.byteTable.channelIndexes.Count; j++)
                {
                    bw.Write(t.byteTable.channelIndexes[j]);
                }

                bw.Write(t.reserved);

            }

            //Write channels.
            foreach (infoBlock.channelInfo c in info.channel)
            {

                bw.Write(c.sampleRecord.identifier);
                bw.Write(c.sampleRecord.padding);
                bw.Write(c.sampleRecord.offset);

            }


            foreach (infoBlock.channelInfo c in info.channel)
            {

                if (info.stream.encoding == 2)
                {

                    bw.Write(c.dspAdpcm.coefficients);
                    bw.Write(c.dspAdpcm.predScale);
                    bw.Write(c.dspAdpcm.yn1);
                    bw.Write(c.dspAdpcm.yn2);
                    bw.Write(c.dspAdpcm.loopPredScale);
                    bw.Write(c.dspAdpcm.loopYn1);
                    bw.Write(c.dspAdpcm.loopYn2);
                    bw.Write(c.dspAdpcm.padding);

                }
                else if (info.stream.encoding == 3)
                {

                    bw.Write(c.imaAdpcm.data);
                    bw.Write(c.imaAdpcm.tableIndex);
                    bw.Write(c.imaAdpcm.padding);

                    bw.Write(c.imaAdpcm.loopData);
                    bw.Write(c.imaAdpcm.loopTableIndex);
                    bw.Write(c.imaAdpcm.loopPadding);

                }

            }

            //Write padding.
            while (bw.Position % 0x20 != 0)
            {
                bw.Write((byte)0);
            }

            //Write SEEK.
            bw.Write(seek.magic);
            bw.Write(seek.size);
            bw.Write(seek.data);


            //Great, time to write the data somehow.
            bw.Write(data.magic);
            bw.Write(data.fileSize);
            bw.Write(data.padding);

            //Write out each channel
            MemoryStream soundDataOut = new MemoryStream();
            BinaryDataWriter bw2 = new BinaryDataWriter(soundDataOut);

            int[] positions = new int[(int)info.stream.numberOfChannels];
            for (int j = 0; j < info.stream.sampleBlockCount - 1; j++)
            {

                for (int i = 0; i < info.stream.numberOfChannels; i++)
                {

                    BinaryDataReader br = new BinaryDataReader(new MemoryStream(data.samples[i]));
                    br.Position = positions[i];

                    bw2.Write(br.ReadBytes((int)info.stream.sampleBlockSize));

                    positions[i] = (int)br.Position;

                }

            }

            for (int i = 0; i < info.stream.numberOfChannels; i++)
            {

                BinaryDataReader br = new BinaryDataReader(new MemoryStream(data.samples[i]));
                br.Position = positions[i];

                //Write the data then the padding!
                bw2.Write(br.ReadBytes((int)info.stream.lastSampleBlockSize));

                int paddingSize = (int)(info.stream.lastSampleBlockPaddingSize - info.stream.lastSampleBlockSize);
                bw2.Write(new byte[paddingSize]);

            }

            bw.Write(soundDataOut.ToArray());

            return o.ToArray();
        }


        /// <summary>
        /// Update the final.
        /// </summary>
        /// <param name="e"></param>
        public void update(endianNess e, bool updateLastSampleBlock)
        {

            MemoryStream byteTrackerWhole = new MemoryStream();
            BinaryDataWriter byteTrackerWholeWriter = new BinaryDataWriter(byteTrackerWhole);

            //Some basic stuff.
            if (e == endianNess.little)
            {
                magic = "CSTM".ToCharArray();
            }
            else
            {
                magic = "FSTM".ToCharArray();
            }

            byteOrder = 0xFEFF;
            headerSize = 0x40;
            version = 0x02000000;
            fileSize = 0xFFFFFFFF;
            nBlocks = 3;
            padding = 0;

            infoRecord = new sizedReference();
            infoRecord.size = 0xFFFFFFFF;
            infoRecord.r.identifier = 0x4000;
            infoRecord.r.padding = 0;
            infoRecord.r.offset = 0x0040;

            seekRecord = new sizedReference();
            seekRecord.size = seek.size;
            seekRecord.r.identifier = 0x4001;
            seekRecord.r.padding = 0;
            seekRecord.r.offset = 0xFFFFFFFF;

            //dataRecord = new sizedReference();
            //dataRecord.size = 0xFFFFFFFF;
            //dataRecord.r.identifier = 0x4002;
            //dataRecord.r.padding = 0;
            //dataRecord.r.offset = 0xFFFFFFFF;

            reserved = new byte[8];

            byteTrackerWholeWriter.Write(new byte[0x40]);


            MemoryStream infoT = new MemoryStream();
            BinaryDataWriter infoTracker = new BinaryDataWriter(infoT);

            //Info block.
            info.magic = "INFO".ToCharArray();
            info.size = 0xFFFFFFFF;
            info.streamRecord = new reference();
            info.streamRecord.identifier = 0x4100;
            info.streamRecord.padding = 0;
            info.streamRecord.offset = 0x00000018;

            info.trackRecord = new reference();
            info.trackRecord.identifier = 0x0101;
            info.trackRecord.padding = 0;
            info.trackRecord.offset = 0x00000050;

            info.channelRecord = new reference();
            info.channelRecord.identifier = 0x0101;
            info.channelRecord.padding = 0;
            info.channelRecord.offset = 0xFFFFFFFF;

            infoTracker.Write(new byte[24]);

            info.stream.numberOfRegions = 0;
            info.stream.numberOfChannels = (byte)info.channel.Count;

            //info.stream.seekSize = 4;
            //info.stream.seekIntervalSampleCount = 0x00003800;

            infoTracker.Write(new byte[56]);

            //Get data.
           // info.stream.sampleBlockSize = 0x2000;
           // info.stream.sampleBlockSampleCount = 14336;
            //info.stream.sampleBlockCount = (UInt32)(Math.Ceiling((decimal)data.samples[0].Length / (decimal)info.stream.sampleBlockSize));

            //if (numSamples < 0) { throw new Exception("No amount of samples specified!"); }
            //else {

                //info.stream.lastSampleBlockSampleCount = (UInt32)(numSamples % info.stream.sampleBlockSampleCount);
                //info.stream.lastSampleBlockSize = (UInt32)Math.Ceiling((decimal)((decimal)(1/.875)*((decimal)info.stream.lastSampleBlockSampleCount/ (decimal)2)));

            //}
            
            //int paddingSize = (int)info.stream.lastSampleBlockSize;
            //while (paddingSize % 0x20 != 0)
            //{
            //    paddingSize += 1;
            //}
            //info.stream.lastSampleBlockPaddingSize = (UInt32)paddingSize;

            //info.stream.sampleRecord = new reference();
            //info.stream.sampleRecord.identifier = 0x1F00;
            //info.stream.sampleRecord.padding = 0;
            //info.stream.sampleRecord.offset = 0x18;




            MemoryStream trackT = new MemoryStream();
            BinaryDataWriter trackWriter = new BinaryDataWriter(trackT);

            //Tracks.
            info.trackReferences = new referenceTable();
            info.trackReferences.count = (UInt32)info.track.Count;
            info.trackReferences.references = new List<reference>();
            if (info.track.Count == 0)
            {
                infoBlock.trackInfo t = new infoBlock.trackInfo();
                t.pan = 64;
                t.volume = 127;
                t.flags = 1;
                t.byteTable = new infoBlock.trackInfo.byteTableTrack();
                t.byteTable.count = info.stream.numberOfChannels;
                t.byteTable.channelIndexes = new List<byte>();
                for (int i = 0; i < info.stream.numberOfChannels; i++)
                {
                    t.byteTable.channelIndexes.Add((byte)i);
                }
                t.byteTableRecord = new reference();
                t.byteTableRecord.identifier = 0x0100;
                t.byteTableRecord.padding = 0;
                t.byteTableRecord.offset = 0x0C;

                int position = 0x10 + info.stream.numberOfChannels;
                while (position % 4 != 0)
                {
                    position += 1;
                }
                t.reserved = new byte[(int)(position - 0x10 - info.stream.numberOfChannels)];
                info.track.Add(t);

                trackWriter.Write(new byte[position + info.stream.numberOfChannels]);
                infoTracker.Write(trackT.ToArray());
                byteTrackerWholeWriter.Write(trackT.ToArray());

                info.trackReferences = new referenceTable();
                info.trackReferences.count = 1;
                info.trackReferences.references = new List<reference>();
                info.trackReferences.references.Add(new reference());

            }
            for (int i = 0; i < info.trackReferences.count; i++)
            {

                reference r = new reference();
                r.identifier = 0x4101;
                r.padding = 0;
                r.offset = 0xFFFFFFFF;
                info.trackReferences.references.Add(r);

            }

            for (int i = 0; i < info.track.Count; i++)
            {

                infoBlock.trackInfo t = info.track[i];
                t.byteTableRecord = new reference();
                t.byteTableRecord.identifier = 0x0100;
                t.byteTableRecord.padding = 0;
                t.byteTableRecord.offset = 0x0C;

                int position = 0x10 + (int)t.byteTable.count;
                while (position % 4 != 0)
                {
                    position += 1;
                }
                t.reserved = new byte[(int)(position - 0x10 - t.byteTable.count)];
                info.track[i] = t;

            }

            info.channelRecord.offset = (UInt32)infoT.ToArray().Length;
            MemoryStream channelT = new MemoryStream();
            BinaryDataWriter channelWriter = new BinaryDataWriter(channelT);

            //Channels.
            info.channelReferences = new referenceTable();
            info.channelReferences.count = (UInt32)info.channel.Count;
            info.channelReferences.references = new List<reference>();
            for (int i = 0; i < info.channel.Count; i++)
            {

                infoBlock.channelInfo c = info.channel[i];

                reference r = new reference();
                r.identifier = 0x4102;
                r.padding = 0;
                r.offset = 0xFFFFFFFF;
                info.channelReferences.references.Add(r);

                c.sampleRecord = new reference();
                switch (info.stream.encoding)
                {
                    case 0:
                    case 1:
                        c.sampleRecord.identifier = 0;
                        c.sampleRecord.padding = 0;
                        c.sampleRecord.offset = 0xFFFFFFFF;
                        break;
                    case 2:
                        c.sampleRecord.identifier = 0x0300;
                        c.sampleRecord.padding = 0;
                        c.sampleRecord.offset = 8;
                        break;
                    case 3:
                        c.sampleRecord.identifier = 0x0301;
                        c.sampleRecord.padding = 0;
                        c.sampleRecord.offset = 8;
                        break;
                }

                info.channel[i] = c;

            }


            seek = new seekBlock();
            seek.magic = "SEEK".ToCharArray();
            seek.data = new byte[(int)((info.stream.sampleBlockCount + 1) * info.stream.seekSize * info.channel.Count)];

            List<byte> extraSeekData = new List<byte>(seek.data);
            while ((extraSeekData.Count+8) % 0x20 != 0) {
                extraSeekData.Add(0);
            }
            seek.data = extraSeekData.ToArray();

            seek.size = (UInt32)(seek.data.Length + 8);
            seekRecord.size = seek.size;

            data.magic = "DATA".ToCharArray();

            /*
            infoTracker.Write(new UInt32[2]);

            info.size = (UInt32)infoT.ToArray().Length;
            byteTrackerWholeWriter.Write(infoT.ToArray());
			*/


            //Lazy way to update offsets. :p

            MemoryStream o = new MemoryStream();
            BinaryDataWriter bw = new BinaryDataWriter(o);

            //Magic.
            bw.Write(magic);

            //Get endianess.
            if (e == endianNess.little)
            {
                bw.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
            }
            else
            {
                bw.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
            }
            bw.Write(byteOrder);

            //Header stuff.
            bw.Write(headerSize);
            bw.Write(version);
            bw.Write(fileSize);
            bw.Write(nBlocks);
            bw.Write(padding);

            bw.Write(infoRecord.r.identifier);
            bw.Write(infoRecord.r.padding);
            bw.Write(infoRecord.r.offset);
            bw.Write(infoRecord.size);

            bw.Write(seekRecord.r.identifier);
            bw.Write(seekRecord.r.padding);
            bw.Write(seekRecord.r.offset);
            bw.Write(seekRecord.size);

            bw.Write(dataRecord.r.identifier);
            bw.Write(dataRecord.r.padding);
            bw.Write(dataRecord.r.offset);
            bw.Write(dataRecord.size);

            //Write padding.
            while (bw.Position % 0x20 != 0)
            {
                bw.Write((byte)0);
            }

            infoRecord.r.offset = (UInt32)o.ToArray().Length;

            //Write INFO.
            bw.Write(info.magic);
            bw.Write(info.size);

            int infoStartRecords = o.ToArray().Length;

            bw.Write(info.streamRecord.identifier);
            bw.Write(info.streamRecord.padding);
            bw.Write(info.streamRecord.offset);

            bw.Write(info.trackRecord.identifier);
            bw.Write(info.trackRecord.padding);
            bw.Write(info.trackRecord.offset);

            bw.Write(info.channelRecord.identifier);
            bw.Write(info.channelRecord.padding);
            bw.Write(info.channelRecord.offset);

            info.streamRecord.offset = (UInt32)(o.ToArray().Length - infoStartRecords);

            //Stream info.
            bw.Write(info.stream.encoding);
            bw.Write(info.stream.loop);
            bw.Write(info.stream.numberOfChannels);
            bw.Write(info.stream.numberOfRegions);
            bw.Write(info.stream.sampleRate);
            bw.Write(info.stream.loopStart);
            bw.Write(info.stream.loopEnd);
            bw.Write(info.stream.sampleBlockCount);
            bw.Write(info.stream.sampleBlockSize);
            bw.Write(info.stream.sampleBlockSampleCount);
            bw.Write(info.stream.lastSampleBlockSize);
            bw.Write(info.stream.lastSampleBlockSampleCount);
            bw.Write(info.stream.lastSampleBlockPaddingSize);
            bw.Write(info.stream.seekSize);
            bw.Write(info.stream.seekIntervalSampleCount);

            bw.Write(info.stream.sampleRecord.identifier);
            bw.Write(info.stream.sampleRecord.padding);
            bw.Write(info.stream.sampleRecord.offset);

            info.trackRecord.offset = (UInt32)(o.ToArray().Length - infoStartRecords);

            //Write track and channel records. I un-nullify all records so I write them.
            int trackOffsetTracker = o.ToArray().Length;
            bw.Write(info.trackReferences.count);
            foreach (reference r in info.trackReferences.references)
            {
                bw.Write(r.identifier);
                bw.Write(r.padding);
                bw.Write(r.offset);
            }

            info.channelRecord.offset = (UInt32)(o.ToArray().Length - infoStartRecords);

            int channelOffsetTracker = o.ToArray().Length;
            bw.Write(info.channelReferences.count);
            foreach (reference r in info.channelReferences.references)
            {
                bw.Write(r.identifier);
                bw.Write(r.padding);
                bw.Write(r.offset);
            }


            //Write tracks.
            int trackCount = 0;
            foreach (infoBlock.trackInfo t in info.track)
            {

                reference r = info.trackReferences.references[trackCount];
                r.offset = (UInt32)(o.ToArray().Length - trackOffsetTracker);
                info.trackReferences.references[trackCount] = r;
                trackCount += 1;

                bw.Write(t.volume);
                bw.Write(t.pan);
                bw.Write(t.flags);

                bw.Write(t.byteTableRecord.identifier);
                bw.Write(t.byteTableRecord.padding);
                bw.Write(t.byteTableRecord.offset);

                bw.Write(t.byteTable.count);
                for (int j = 0; j < t.byteTable.channelIndexes.Count; j++)
                {
                    bw.Write(t.byteTable.channelIndexes[j]);
                }

                bw.Write(t.reserved);

            }


            //Write channels.
            int channelCount = 0;


            MemoryStream o2 = new MemoryStream();
            BinaryDataWriter bw3 = new BinaryDataWriter(o2);
            int randCounter = 0;

            for (int i = 0; i < info.channel.Count; i++)
            {
                bw3.Write(info.channel[i].sampleRecord.identifier);
                bw3.Write(info.channel[i].sampleRecord.padding);
                bw3.Write(info.channel[i].sampleRecord.offset);
            }

            for (int i = 0; i < info.channel.Count; i++)
            {

                infoBlock.channelInfo r = info.channel[i];
                r.sampleRecord.offset = (UInt32)(o2.ToArray().Length - randCounter);
                info.channel[i] = r;
                randCounter += 8;

                if (info.stream.encoding == 2)
                {

                    bw3.Write(info.channel[i].dspAdpcm.coefficients);
                    bw3.Write(info.channel[i].dspAdpcm.predScale);
                    bw3.Write(info.channel[i].dspAdpcm.yn1);
                    bw3.Write(info.channel[i].dspAdpcm.yn2);
                    bw3.Write(info.channel[i].dspAdpcm.loopPredScale);
                    bw3.Write(info.channel[i].dspAdpcm.loopYn1);
                    bw3.Write(info.channel[i].dspAdpcm.loopYn2);
                    bw3.Write(info.channel[i].dspAdpcm.padding);

                }
                else if (info.stream.encoding == 3)
                {

                    bw3.Write(info.channel[i].imaAdpcm.data);
                    bw3.Write(info.channel[i].imaAdpcm.tableIndex);
                    bw3.Write(info.channel[i].imaAdpcm.padding);

                    bw3.Write(info.channel[i].imaAdpcm.loopData);
                    bw3.Write(info.channel[i].imaAdpcm.loopTableIndex);
                    bw3.Write(info.channel[i].imaAdpcm.loopPadding);

                }

            }



            foreach (infoBlock.channelInfo c in info.channel)
            {

                reference r = info.channelReferences.references[channelCount];
                r.offset = (UInt32)(o.ToArray().Length - channelOffsetTracker);
                info.channelReferences.references[channelCount] = r;
                channelCount += 1;

                bw.Write(c.sampleRecord.identifier);
                bw.Write(c.sampleRecord.padding);
                bw.Write(c.sampleRecord.offset);

            }

            foreach (infoBlock.channelInfo c in info.channel)
            {

                if (info.stream.encoding == 2)
                {

                    bw.Write(c.dspAdpcm.coefficients);
                    bw.Write(c.dspAdpcm.predScale);
                    bw.Write(c.dspAdpcm.yn1);
                    bw.Write(c.dspAdpcm.yn2);
                    bw.Write(c.dspAdpcm.loopPredScale);
                    bw.Write(c.dspAdpcm.loopYn1);
                    bw.Write(c.dspAdpcm.loopYn2);
                    bw.Write(c.dspAdpcm.padding);

                }
                else if (info.stream.encoding == 3)
                {

                    bw.Write(c.imaAdpcm.data);
                    bw.Write(c.imaAdpcm.tableIndex);
                    bw.Write(c.imaAdpcm.padding);

                    bw.Write(c.imaAdpcm.loopData);
                    bw.Write(c.imaAdpcm.loopTableIndex);
                    bw.Write(c.imaAdpcm.loopPadding);

                }

            }

            //Write padding.
            while (bw.Position % 0x20 != 0)
            {
                bw.Write((byte)0);
            }

            infoRecord.size = (UInt32)(o.ToArray().Length - infoRecord.r.offset);
            info.size = infoRecord.size;
            seekRecord.r.offset = (UInt32)o.ToArray().Length;

            //Write SEEK.
            bw.Write(seek.magic);
            bw.Write(seek.size);
            bw.Write(seek.data);


            seek.size = (UInt32)(o.ToArray().Length - seekRecord.r.offset);
            seekRecord.size = seek.size;
            dataRecord.r.offset = (UInt32)o.ToArray().Length;

            fileSize = (UInt32)info.size + headerSize + data.fileSize;

        }



        /// <summary>
        /// Convert to b_wavO.
        /// </summary>
        public b_wavO tob_wavO()
        {

            update(endianNess.big, false);

            b_wavO b = new b_wavO();
            b.info.loop = info.stream.loop;
            b.info.loopEnd = info.stream.loopEnd;
            b.info.loopStart = info.stream.loopStart;

            b.info.soundEncoding = info.stream.encoding;
            b.info.samplingRate = info.stream.sampleRate;

            b.info.channels = new List<b_wavO.channelInfo>();
            for (int i = 0; i < info.channel.Count; i++)
            {

                b_wavO.channelInfo c = new b_wavO.channelInfo();

                if (b.info.soundEncoding == 2)
                {

                    c.dspAdpcm.coefficients = info.channel[i].dspAdpcm.coefficients;
                    c.dspAdpcm.loopPredScale = info.channel[i].dspAdpcm.loopPredScale;
                    c.dspAdpcm.loopYn1 = info.channel[i].dspAdpcm.loopYn1;
                    c.dspAdpcm.loopYn2 = info.channel[i].dspAdpcm.loopYn2;
                    c.dspAdpcm.padding = info.channel[i].dspAdpcm.padding;
                    c.dspAdpcm.predScale = info.channel[i].dspAdpcm.predScale;
                    c.dspAdpcm.yn1 = info.channel[i].dspAdpcm.yn1;
                    c.dspAdpcm.yn2 = info.channel[i].dspAdpcm.yn2;

                }
                else if (b.info.soundEncoding == 3)
                {

                    c.imaAdpcm.data = info.channel[i].imaAdpcm.data;
                    c.imaAdpcm.loopData = info.channel[i].imaAdpcm.loopData;
                    c.imaAdpcm.loopPadding = info.channel[i].imaAdpcm.loopPadding;
                    c.imaAdpcm.loopTableIndex = info.channel[i].imaAdpcm.loopTableIndex;
                    c.imaAdpcm.padding = info.channel[i].imaAdpcm.padding;
                    c.imaAdpcm.tableIndex = info.channel[i].imaAdpcm.tableIndex;

                }

                b.info.channels.Add(c);

            }

            b.data.samples = data.samples;
            b.data.pcm16 = data.pcm16;

            return b;

        }


    }



    /// <summary>
    /// Wave file.
    /// </summary>
    public class b_wavO
    {

        string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        public endianNess endian; //Endian.

        public char[] magic; //CWAV, FWAV.
        public UInt16 byteOrder; //0xFEFF Big, 0xFFEF Small.
        public UInt16 headerSize; //Header Size.
        public UInt32 version; //Version of the format.
        public UInt32 fileSize; //File size.
        public UInt16 nBlocks; //Always 2.
        public UInt16 padding; //Useless padding.

        public sizedReference infoRecord; //Info Record.
        public sizedReference dataRecord; //Data Record.

        public byte[] reserved; //Reserved to make header size divisible by 0x40.

        //Blocks.
        public infoBlock info; //Info.
        public dataBlock data; //Data.

        /// <summary>
        /// Info block. 0x20 aligned.
        /// </summary>
        public struct infoBlock
        {

            public char[] magic; //INFO.
            public UInt32 size; //Size of the block.
            public byte soundEncoding; //PCM8, PCM16, DSP ADPCM, IMA ADPCM.
            public byte loop; //If there is a loop.
            public UInt16 padding; //Nintendo loves it.
            public UInt32 samplingRate; //Sampling rate.
            public UInt32 loopStart; //Loop start.
            public UInt32 loopEnd; //Loop end.
            public UInt32 padding2; //More padding.

            public referenceTable channelRecords; //References. Leads to channel info(s).
            public List<channelInfo> channels; //Channels.

            public byte[] reserved; //Remaining reserved space.


        }



        /// <summary>
        /// Channel info.
        /// </summary>
        public struct channelInfo
        {

            public reference sampleRecord; //Sample reference.
            public reference adpcmRecord; //Adpcm reference.
            public UInt32 reserved; //Reserved.

            public dspAdpcmInfo dspAdpcm; //Adpcm.
            public imaAdpcmInfo imaAdpcm; //Adpcm.

        }


        /// <summary>
        /// Adpcm info.
        /// </summary>
        public struct dspAdpcmInfo
        {

            public Int16[] coefficients; //16 ADPCM coefficients.
            public UInt16 predScale; //Predictor scale.
            public UInt16 yn1; //History sample 1.
            public UInt16 yn2; //History sample 2.
            public UInt16 loopPredScale; //Loop predictor scale.
            public UInt16 loopYn1; //Loop History sample.
            public UInt16 loopYn2; //Loop History sample 2.
            public UInt16 padding; //Padding.

        }


        /// <summary>
        /// Ima adpcm info.
        /// </summary>
        public struct imaAdpcmInfo
        {

            public UInt16 data; //Data.
            public byte tableIndex; //Table Index.
            public byte padding; //Padding.

            public UInt16 loopData; //Loop data.
            public byte loopTableIndex; //Loop table index.
            public byte loopPadding; //Loop padding.

        }


        /// <summary>
        /// Data block. 0x20 aligned.
        /// </summary>
        public struct dataBlock
        {

            public char[] magic; //DATA.
            public UInt32 size; //Size of this block.

            public byte[] padding; //Padding.
            public List<byte[]> samples; //Samples.

            public List<UInt16[]> pcm16; //PCM16 samples. Only needs to be bothered with with endianess.

        }




        /// <summary>
        /// A reference used to an offset.
        /// </summary>
        public struct reference
        {

            public UInt16 identifier; //Identifier.
                                      /*
                                          0x0100	Byte Table
                                          0x0101	Reference Table
                                          0x0300	DSP ADPCM Info
                                          0x0301	IMA ADPCM Info
                                          0x1F00	Sample Data
                                          0x4000	Info Block
                                          0x4001	Seek Block
                                          0x4002	Data Block
                                          0x4100	Stream Info
                                          0x4101	Track Info
                                          0x4102	Channel Info
                                          */
            public UInt16 padding; //Padding woo.
            public UInt32 offset; //Offset.

        }


        /// <summary>
        /// Sized reference.
        /// </summary>
        public struct sizedReference
        {

            public reference r; //Reference.
            public UInt32 size; //Size.

        }


        /// <summary>
        /// Reference table.
        /// </summary>
        public struct referenceTable
        {

            public UInt32 count; //Count of references.
            public List<reference> references; //References.

        }





        /// <summary>
        /// Load a file.
        /// </summary>
        public void load(byte[] b)
        {

            //Reader stuff.
            MemoryStream src = new MemoryStream(b);
            BinaryDataReader br = new BinaryDataReader(src);
            br.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            //Read magic.
            magic = br.ReadChars(4);

            //Get endianess.
            byteOrder = br.ReadUInt16();

            if (byteOrder == 0xFEFF)
            {

                br.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                endian = endianNess.big;

            }
            else if (byteOrder == 0xFFFE)
            {

                br.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                endian = endianNess.little;
                byteOrder = 0xFEFF;

            }

            //Other data.
            headerSize = br.ReadUInt16();
            version = br.ReadUInt32();
            fileSize = br.ReadUInt32();
            nBlocks = br.ReadUInt16();
            padding = br.ReadUInt16();

            //Main references.
            infoRecord = new sizedReference();
            infoRecord.r = new reference();
            infoRecord.r.identifier = br.ReadUInt16();
            infoRecord.r.padding = br.ReadUInt16();
            infoRecord.r.offset = br.ReadUInt32();
            infoRecord.size = br.ReadUInt32();

            dataRecord = new sizedReference();
            dataRecord.r = new reference();
            dataRecord.r.identifier = br.ReadUInt16();
            dataRecord.r.padding = br.ReadUInt16();
            dataRecord.r.offset = br.ReadUInt32();
            dataRecord.size = br.ReadUInt32();


            //Info block.
            info = new infoBlock();
            br.Position = (int)infoRecord.r.offset;

            info.magic = br.ReadChars(4);
            info.size = br.ReadUInt32();
            info.soundEncoding = br.ReadByte();
            info.loop = br.ReadByte();
            info.padding = br.ReadUInt16();
            info.samplingRate = br.ReadUInt32();
            info.loopStart = br.ReadUInt32();
            info.loopEnd = br.ReadUInt32();
            info.padding2 = br.ReadUInt32();

            //Read channel record.
            long channelRecordStart = br.Position;
            info.channelRecords = new referenceTable();
            info.channelRecords.count = br.ReadUInt32();
            info.channelRecords.references = new List<reference>();
            for (int i = 0; i < (int)info.channelRecords.count; i++)
            {

                reference r = new reference();
                r.identifier = br.ReadUInt16();
                r.padding = br.ReadUInt16();
                r.offset = br.ReadUInt32();
                info.channelRecords.references.Add(r);

            }

            info.channels = new List<channelInfo>();
            for (int i = 0; i < (int)info.channelRecords.count; i++)
            {

                br.Position = (int)channelRecordStart + (int)info.channelRecords.references[i].offset;

                long adpcmInfoStartingOffset = br.Position;

                channelInfo c = new channelInfo();
                c.sampleRecord = new reference();
                c.sampleRecord.identifier = br.ReadUInt16();
                c.sampleRecord.padding = br.ReadUInt16();
                c.sampleRecord.offset = br.ReadUInt32();
                c.adpcmRecord = new reference();
                c.adpcmRecord.identifier = br.ReadUInt16();
                c.adpcmRecord.padding = br.ReadUInt16();
                c.adpcmRecord.offset = br.ReadUInt32();
                c.reserved = br.ReadUInt32();

                c.dspAdpcm = new dspAdpcmInfo();
                c.imaAdpcm = new imaAdpcmInfo();

                br.Position = (int)adpcmInfoStartingOffset + (int)c.adpcmRecord.offset;

                if (c.adpcmRecord.identifier == 0x0300)
                {

                    c.dspAdpcm.coefficients = br.ReadInt16s(16);
                    c.dspAdpcm.predScale = br.ReadUInt16();
                    c.dspAdpcm.yn1 = br.ReadUInt16();
                    c.dspAdpcm.yn2 = br.ReadUInt16();
                    c.dspAdpcm.loopPredScale = br.ReadUInt16();
                    c.dspAdpcm.loopYn1 = br.ReadUInt16();
                    c.dspAdpcm.loopYn2 = br.ReadUInt16();
                    c.dspAdpcm.padding = br.ReadUInt16();

                }
                else if (c.adpcmRecord.identifier == 0x0301)
                {

                    c.imaAdpcm.data = br.ReadUInt16();
                    c.imaAdpcm.tableIndex = br.ReadByte();
                    c.imaAdpcm.padding = br.ReadByte();
                    c.imaAdpcm.loopData = br.ReadUInt16();
                    c.imaAdpcm.loopTableIndex = br.ReadByte();
                    c.imaAdpcm.loopPadding = br.ReadByte();

                }

                info.channels.Add(c);

            }

            //Read reserved.
            int reservedSize = (int)info.size + headerSize - (int)br.Position;
            reserved = br.ReadBytes(reservedSize);


            //Read data.
            br.Position = (int)dataRecord.r.offset;
            data.magic = br.ReadChars(4);
            data.size = br.ReadUInt32();
            data.padding = br.ReadBytes((int)info.channels[0].sampleRecord.offset);

            //Read samples.
            data.samples = new List<byte[]>();
            data.pcm16 = new List<ushort[]>();
            int size = (int)data.size - 8 - (int)info.channels[info.channels.Count - 1].sampleRecord.offset;
            for (int i = 0; i < (int)info.channelRecords.count; i++)
            {

                br.Position = (int)info.channels[i].sampleRecord.offset + 8 + (int)dataRecord.r.offset;

                if (info.soundEncoding != 1) { byte[] s = br.ReadBytes(size); data.samples.Add(s); }
                else { UInt16[] p = br.ReadUInt16s(size / 2); data.pcm16.Add(p); }

            }

        }

        /// <summary>
        /// Write to bytes.
        /// </summary>
        /// <param name="e"></param>
        public byte[] toBytes(endianNess e)
        {

            //Update.
            update(e);

            //Writer.
            MemoryStream o = new MemoryStream();
            BinaryDataWriter bw = new BinaryDataWriter(o);

            //Endianess.
            if (e == endianNess.big)
            {
                bw.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
            }
            else
            {
                bw.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
            }

            //Write stuff.
            bw.Write(magic);
            bw.Write(byteOrder);

            //Other data.
            bw.Write(headerSize);
            bw.Write(version);
            bw.Write(fileSize);
            bw.Write(nBlocks);
            bw.Write(padding);

            //Main references.
            bw.Write(infoRecord.r.identifier);
            bw.Write(infoRecord.r.padding);
            bw.Write(infoRecord.r.offset);
            bw.Write(infoRecord.size);

            bw.Write(dataRecord.r.identifier);
            bw.Write(dataRecord.r.padding);
            bw.Write(dataRecord.r.offset);
            bw.Write(dataRecord.size);
            bw.Write(reserved);

            //Info block.
            bw.Write(info.magic);
            bw.Write(info.size);
            bw.Write(info.soundEncoding);
            bw.Write(info.loop);
            bw.Write(info.padding);
            bw.Write(info.samplingRate);
            bw.Write(info.loopStart);
            bw.Write(info.loopEnd);
            bw.Write(info.padding2);

            bw.Write(info.channelRecords.count);
            for (int i = 0; i < (int)info.channelRecords.count; i++)
            {

                bw.Write(info.channelRecords.references[i].identifier);
                bw.Write(info.channelRecords.references[i].padding);
                bw.Write(info.channelRecords.references[i].offset);

            }

            for (int i = 0; i < (int)info.channelRecords.count; i++)
            {

                bw.Write(info.channels[i].sampleRecord.identifier);
                bw.Write(info.channels[i].sampleRecord.padding);
                bw.Write(info.channels[i].sampleRecord.offset);

                bw.Write(info.channels[i].adpcmRecord.identifier);
                bw.Write(info.channels[i].adpcmRecord.padding);
                bw.Write(info.channels[i].adpcmRecord.offset);

                bw.Write(info.channels[i].reserved);

                if (info.channels[i].adpcmRecord.identifier == 0x0300)
                {

                    bw.Write(info.channels[i].dspAdpcm.coefficients);
                    bw.Write(info.channels[i].dspAdpcm.predScale);
                    bw.Write(info.channels[i].dspAdpcm.yn1);
                    bw.Write(info.channels[i].dspAdpcm.yn2);
                    bw.Write(info.channels[i].dspAdpcm.loopPredScale);
                    bw.Write(info.channels[i].dspAdpcm.loopYn1);
                    bw.Write(info.channels[i].dspAdpcm.loopYn2);
                    bw.Write(info.channels[i].dspAdpcm.padding);

                }
                else if (info.channels[i].adpcmRecord.identifier == 0x0301)
                {

                    bw.Write(info.channels[i].imaAdpcm.data);
                    bw.Write(info.channels[i].imaAdpcm.tableIndex);
                    bw.Write(info.channels[i].imaAdpcm.padding);
                    bw.Write(info.channels[i].imaAdpcm.loopData);
                    bw.Write(info.channels[i].imaAdpcm.loopTableIndex);
                    bw.Write(info.channels[i].imaAdpcm.loopPadding);

                }

            }

            //Reserved.
            bw.Write(info.reserved);


            //Data.
            bw.Write(data.magic);
            bw.Write(data.size);
            bw.Write(data.padding);



            //Samples.
            for (int i = 0; i < (int)info.channels.Count; i++)
            {
                if (info.soundEncoding != 1) { bw.Write(data.samples[i]); }
                else { bw.Write(data.pcm16[i]); }
            }

            return o.ToArray();

        }



        /// <summary>
        /// Update.
        /// </summary>
        /// <param name="e"></param>
        public void update(endianNess e)
        {

            if (e == endianNess.big)
            {
                magic = "FWAV".ToCharArray();
                version = 0x00010100;
            }
            else
            {
                magic = "CWAV".ToCharArray();
                version = 0x02010000;
            }

            //Basic data.
            byteOrder = 0xFEFF;
            headerSize = 0x0040;
            fileSize = 0xFFFFFFFF;
            nBlocks = 2;
            padding = 0;

            infoRecord = new sizedReference();
            infoRecord.r = new reference();
            infoRecord.r.identifier = 0x7000;
            infoRecord.r.padding = 0;
            infoRecord.r.offset = 0x0040;
            infoRecord.size = 0xFFFFFFFF;

            dataRecord = new sizedReference();
            dataRecord.r = new reference();
            dataRecord.r.identifier = 0x7001;
            dataRecord.r.padding = 0;
            dataRecord.r.offset = 0xFFFF;
            dataRecord.size = 0xFFFFFFFF;

            reserved = new byte[0x14];

            //Will take care of sizes later.


            //Info.
            UInt32 infoSize = 32;
            info.magic = "INFO".ToCharArray();
            info.size = 0xFFFFFFFF;
            info.padding = 0;
            info.padding2 = 0;


            //Channels
            info.channelRecords = new referenceTable();
            info.channelRecords.count = (UInt32)info.channels.Count;
            UInt32 channelOffsets = 4;
            info.channelRecords.references = new List<reference>();
            for (int i = 0; i < (int)info.channelRecords.count; i++)
            {

                reference r = new reference();
                r.identifier = 0x7100;
                r.padding = 0;
                r.offset = 0xFFFFFFFF;
                info.channelRecords.references.Add(r);
                channelOffsets += 8;
                infoSize += 8;

            }

            for (int i = 0; i < info.channels.Count; i++)
            {

                reference r = info.channelRecords.references[i];
                r.offset = (UInt32)channelOffsets;
                info.channelRecords.references[i] = r;

                channelInfo c = info.channels[i];

                c.sampleRecord = new reference();
                c.sampleRecord.identifier = 0x1F00;
                c.sampleRecord.padding = 0;
                c.sampleRecord.offset = 0xFFFFFFFF;

                c.adpcmRecord = new reference();

                if (info.soundEncoding == 2) { c.adpcmRecord.identifier = 0x0300; channelOffsets += 46; infoSize += 46; }
                else if (info.soundEncoding == 3) { c.adpcmRecord.identifier = 0x0301; channelOffsets += 8; infoSize += 8; }
                else { c.adpcmRecord.identifier = 0; }

                c.adpcmRecord.padding = 0;
                c.adpcmRecord.offset = 0xFFFFFFFF;
                if (info.soundEncoding == 2 || info.soundEncoding == 3) { c.adpcmRecord.offset = 0x14; }

                c.reserved = 0;

                c.dspAdpcm.padding = 0;
                c.imaAdpcm.padding = 0;
                c.imaAdpcm.loopPadding = 0;

                info.channels[i] = c;

                channelOffsets += 20;
                infoSize += 20;

            }


            //Reserved.
            List<byte> res = new List<byte>();
            while (infoSize % 0x20 != 0)
            {

                res.Add(0);
                infoSize += 1;

            }
            info.reserved = res.ToArray();
            info.size = infoSize;


            //Data.
            data.magic = "DATA".ToCharArray();
            data.size = 0xFFFFFFFF;
            data.padding = new byte[0x18];

            UInt32 dataSize = 8 + 0x18;

            //Samples.
            UInt32 sampleOffsets = 0x18;
            for (int i = 0; i < (int)info.channels.Count; i++)
            {
                channelInfo c = info.channels[i];
                c.sampleRecord.offset = sampleOffsets;
                info.channels[i] = c;
                if (info.soundEncoding != 1)
                {
                    dataSize += (UInt32)data.samples[i].Length;
                    sampleOffsets += (UInt32)data.samples[i].Length;
                }
                else
                {
                    dataSize += (UInt32)data.pcm16[i].Length * 2;
                    sampleOffsets += (UInt32)data.pcm16[i].Length * 2;
                }
            }

            data.size = dataSize;

            //File size.
            fileSize = (UInt32)headerSize + info.size + data.size;

            //Write those offsets from earlier.
            infoRecord.size = info.size;
            dataRecord.size = data.size;
            dataRecord.r.offset = (UInt32)headerSize + info.size;

        }


    }



    /// <summary>
	/// Warchive.
	/// </summary>
	public class b_warO
    {

        public Syroot.BinaryData.ByteOrder endian; //Endian.

        public char[] magic; //FWAR; CWAR.
        public UInt16 byteOrder; //0xFEFF Big, 0xFFFE Little.
        public UInt16 headerSize; //Size of header.
        public UInt32 version; //0x00010000 for FWAR.
        public UInt32 fileSize; //File size.
        public UInt16 nBlocks; //Number of blocks, 2.
        public UInt16 padding; //Padding.

        public sizedReference infoReference; //INFO reference.
        public sizedReference fileReference; //FILE reference.

        public byte[] reserved; //Padding to make it aligned to the headerSize.

        public infoBlock info; //Info.
        public fileBlock file; //Files.


        /// <summary>
        /// Info block.
        /// </summary>
        public struct infoBlock
        {

            public char[] magic; //INFO.
            public UInt32 size; //Size.

            public sizedReferenceTable entries; //Entries.
            public byte[] reserved; //For 0x20 alignment.

        }


        /// <summary>
        /// File block.
        /// </summary>
        public struct fileBlock
        {

            public char[] magic; //FILE.
            public UInt32 size; //Size.
            public byte[] reserved; //Reserved.

            public List<fileEntry> files; //Files.

            /// <summary>
            /// File entry.
            /// </summary>
            public struct fileEntry
            {

                public byte[] file; //File.
                public byte[] padding; //To make it 0x20 aligned.

            }

        }


        /// <summary>
        /// Sized reference.
        /// </summary>
        public struct sizedReference
        {

            public UInt16 identifier; //Identifier.
                                      /*
                                       * 0x6800 - INFO.
                                       * 0x6801 - FILE.
                                       * 0x1F00 - INFO ENTRY.
                                       */
            public UInt16 padding; //Padding.
            public UInt32 offset; //Offset.
            public UInt32 size; //Size.

        }


        /// <summary>
        /// Sized reference table.
        /// </summary>
        public struct sizedReferenceTable
        {

            public UInt32 count; //Count.
            public List<sizedReference> references; //References.

        }


        /// <summary>
        /// Load the file.
        /// </summary>
        public void load(byte[] b)
        {

            //Reader.
            MemoryStream src = new MemoryStream(b);
            BinaryDataReader br = new BinaryDataReader(src);
            br.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            //Read stuff.
            magic = br.ReadChars(4);
            byteOrder = br.ReadUInt16();

            if (byteOrder == 0xFEFF)
            {
                endian = Syroot.BinaryData.ByteOrder.BigEndian;
                br.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
            }
            else
            {
                endian = Syroot.BinaryData.ByteOrder.LittleEndian;
                br.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                byteOrder = 0xFEFF;
            }
            headerSize = br.ReadUInt16();
            version = br.ReadUInt32();
            fileSize = br.ReadUInt32();
            nBlocks = br.ReadUInt16();
            padding = br.ReadUInt16();

            infoReference = new sizedReference();
            infoReference.identifier = br.ReadUInt16();
            infoReference.padding = br.ReadUInt16();
            infoReference.offset = br.ReadUInt32();
            infoReference.size = br.ReadUInt32();

            fileReference = new sizedReference();
            fileReference.identifier = br.ReadUInt16();
            fileReference.padding = br.ReadUInt16();
            fileReference.offset = br.ReadUInt32();
            fileReference.size = br.ReadUInt32();

            reserved = br.ReadBytes((int)headerSize - (int)br.Position);

            //Info block.
            info = new infoBlock();
            br.Position = (int)infoReference.offset;

            info.magic = br.ReadChars(4);
            info.size = br.ReadUInt32();
            info.entries = new sizedReferenceTable();
            info.entries.count = br.ReadUInt32();
            info.entries.references = new List<sizedReference>();
            for (int i = 0; i < info.entries.count; i++)
            {

                sizedReference r = new sizedReference();
                r.identifier = br.ReadUInt16();
                r.padding = br.ReadUInt16();
                r.offset = br.ReadUInt32();
                r.size = br.ReadUInt32();
                info.entries.references.Add(r);

            }
            List<byte> reservedTemp = new List<byte>();
            int size = 12 + (int)info.entries.count * 12;
            while (size % 0x20 != 0)
            {
                reservedTemp.Add(0);
                size += 1;
            }
            info.reserved = reservedTemp.ToArray();

            //File block.
            file = new fileBlock();
            br.Position = (int)fileReference.offset;

            file.magic = br.ReadChars(4);
            file.size = br.ReadUInt32();
            int relOffset = (int)br.Position;
            file.reserved = br.ReadBytes(0x18);
            file.files = new List<fileBlock.fileEntry>();
            foreach (sizedReference r in info.entries.references)
            {

                br.Position = relOffset + (int)(r.offset);
                fileBlock.fileEntry e = new fileBlock.fileEntry();
                e.file = br.ReadBytes((int)r.size);

                List<byte> paddingTemp = new List<byte>();
                int size2 = (int)r.size;
                while (size2 % 0x20 != 0)
                {
                    paddingTemp.Add(0);
                    size2 += 1;
                }
                e.padding = paddingTemp.ToArray();

                file.files.Add(e);

            }

        }


        /// <summary>
        /// Convert to bytes.
        /// </summary>
        /// <returns></returns>
        public byte[] toBytes(Syroot.BinaryData.ByteOrder endian)
        {

            //Update.
            update(endian);


            //New writer.
            MemoryStream o = new MemoryStream();
            BinaryDataWriter bw = new BinaryDataWriter(o);
            bw.ByteOrder = endian;


            //Start writing.
            bw.Write(magic);
            bw.Write(byteOrder);
            bw.Write(headerSize);
            bw.Write(version);
            bw.Write(fileSize);
            bw.Write(nBlocks);
            bw.Write(padding);

            bw.Write(infoReference.identifier);
            bw.Write(infoReference.padding);
            bw.Write(infoReference.offset);
            bw.Write(infoReference.size);

            bw.Write(fileReference.identifier);
            bw.Write(fileReference.padding);
            bw.Write(fileReference.offset);
            bw.Write(fileReference.size);

            bw.Write(reserved);


            //Info.
            bw.Write(info.magic);
            bw.Write(info.size);
            bw.Write(info.entries.count);
            foreach (sizedReference r in info.entries.references)
            {
                bw.Write(r.identifier);
                bw.Write(r.padding);
                bw.Write(r.offset);
                bw.Write(r.size);
            }
            bw.Write(info.reserved);


            //File.
            bw.Write(file.magic);
            bw.Write(file.size);
            bw.Write(file.reserved);
            foreach (fileBlock.fileEntry e in file.files)
            {
                bw.Write(e.file);
                bw.Write(e.padding);
            }


            //Return new file.
            return o.ToArray();

        }


        /// <summary>
        /// Update the file.
        /// </summary>
        /// <param name="endian"></param>
        public void update(Syroot.BinaryData.ByteOrder endian)
        {

            //General stuff.
            if (endian == Syroot.BinaryData.ByteOrder.BigEndian)
            {
                magic = "FWAR".ToCharArray();
            }
            else
            {
                magic = "CWAR".ToCharArray();
            }

            for (int i = 0; i < file.files.Count; i++)
            {
                b_wavO bW = new b_wavO();
                bW.load(file.files[i].file);
                fileBlock.fileEntry e = file.files[i];
                if (endian == Syroot.BinaryData.ByteOrder.BigEndian) { e.file = bW.toBytes(endianNess.big); } else { e.file = bW.toBytes(endianNess.little); }
                file.files[i] = e;
            }

            byteOrder = 0xFEFF;
            headerSize = 0x40;
            version = 0x00010000;
            fileSize = 0xFFFFFFFF;
            nBlocks = 2;
            padding = 0;

            infoReference = new sizedReference();
            infoReference.identifier = 0x6800;
            infoReference.padding = 0;
            infoReference.offset = 0x40;
            infoReference.size = 0xFFFFFFFF;

            fileReference = new sizedReference();
            fileReference.identifier = 0x6801;
            fileReference.padding = 0;
            fileReference.offset = 0xFFFFFFFF;
            fileReference.size = 0xFFFFFFFF;

            reserved = new byte[0x14];


            //Info.
            info.magic = "INFO".ToCharArray();
            info.size = 0xFFFFFFFF;
            info.entries = new sizedReferenceTable();
            info.entries.references = new List<sizedReference>();
            UInt32 offset = 0x18;
            UInt32 filesSizes = 0;
            for (int i = 0; i < file.files.Count; i++)
            {

                fileBlock.fileEntry f = file.files[i];

                sizedReference s = new sizedReference();
                s.offset = offset;
                s.padding = 0;
                s.identifier = 0x1F00;
                s.size = (UInt32)f.file.Length;

                int size = (int)s.size;
                List<byte> reservedT = new List<byte>();
                while (size % 0x20 != 0)
                {
                    reservedT.Add(0);
                    size += 1;
                }
                f.padding = reservedT.ToArray();
                offset += s.size + (UInt32)reservedT.Count;
                filesSizes += s.size + (UInt32)reservedT.Count;
                file.files[i] = f;
                info.entries.references.Add(s);

            }
            info.entries.count = (UInt32)info.entries.references.Count;

            int newReserved = 12 + 12 * (int)info.entries.count;
            List<byte> newReserved2 = new List<byte>();
            while (newReserved % 0x20 != 0)
            {
                newReserved2.Add(0);
                newReserved += 1;
            }
            info.reserved = newReserved2.ToArray();
            info.size = (UInt32)newReserved;
            infoReference.size = info.size;

            //File.
            file.magic = "FILE".ToCharArray();
            file.size = filesSizes + 8 + 0x18;
            file.reserved = new byte[0x18];
            fileReference.size = file.size;
            fileReference.offset = 0x40 + info.size;

            fileSize = 0x40 + info.size + file.size;

        }


        /// <summary>
        /// Extract to folder.
        /// </summary>
        /// <param name="path"></param>
        public void extract(string path, Syroot.BinaryData.ByteOrder endian)
        {

            int id = 0;
            foreach (fileBlock.fileEntry file in file.files)
            {
                b_wavO b = new b_wavO();
                b.load(file.file);
                if (endian == Syroot.BinaryData.ByteOrder.BigEndian)
                {
                    File.WriteAllBytes(path + "/" + id.ToString("D4") + ".bfwav", b.toBytes(endianNess.big));
                }
                else
                {
                    File.WriteAllBytes(path + "/" + id.ToString("D4") + ".bcwav", b.toBytes(endianNess.little));
                }
                id += 1;
            }

        }


        /// <summary>
        /// Compress path to file.
        /// </summary>
        /// <param name="path"></param>
        public void compress(string path)
        {

            string[] names = Directory.GetFiles(path);
            List<byte[]> files = new List<byte[]>();
            foreach (string name in names)
            {
                files.Add(File.ReadAllBytes(name));
            }

            file = new fileBlock();
            file.files = new List<fileBlock.fileEntry>();
            foreach (byte[] b in files)
            {

                fileBlock.fileEntry e = new fileBlock.fileEntry();
                e.file = b;
                e.padding = new byte[0];
                file.files.Add(e);

            }
            update(Syroot.BinaryData.ByteOrder.BigEndian);

        }

    }



    /// <summary>
    /// Group file.
    /// </summary>
    public class b_grpO
    {

        public char[] magic; //FGRP, CGRP.
        public UInt16 byteOrder; //0xFEFF Big, 0xFFEF Little.
        public UInt16 headerSize; //0x40.
        public UInt32 version; //0x00010000
        public UInt32 fileSize; //File size.
        public UInt16 nBlocks; //Number of blocks, 3.
        public UInt16 padding; //Padding.

        public sizedReference infoReference; //Info reference.
        public sizedReference fileReference; //File reference.
        public sizedReference infxReference; //Infx reference.

        public byte[] reserved; //Reserved area.

        public infoBlock info; //Info.
        public fileBlock file; //File.
        public infxBlock infx; //Infx.


        /// <summary>
        /// Info block.
        /// </summary>
        public struct infoBlock
        {

            public char[] magic; //INFO.
            public UInt32 size; //Size of block.

            public referenceTable groupReferences; //Group References.
            public List<groupEntry> entries; //Group entries.
            public byte[] reserved; //To make it 0x20 byte aligned.


            /// <summary>
            /// Group entry.
            /// </summary>
            public struct groupEntry
            {

                public UInt32 fileId; //File ID.
                public sizedReference fileEntry; //Entry to file.

                public bool isNull; //If null.

            }

        }


        /// <summary>
        /// File block.
        /// </summary>
        public struct fileBlock
        {

            public char[] magic; //FILE.
            public UInt32 size; //Size of block.
            public byte[] reserved; //Reserved.
            public List<fileEntry> files; //Files.

            /// <summary>
            /// File entry.
            /// </summary>
            public struct fileEntry
            {

                public byte[] file; //File.
                public byte[] reserved; //For 0x20 alignment.

                public int fileId; //File ID.
                public bool isNull; //If null.

            }

        }


        /// <summary>
        /// Info Ex block.
        /// </summary>
        public struct infxBlock
        {

            public char[] magic; //INFX.
            public UInt32 size; //Size of block.

            public referenceTable references; //References.
            public List<infxEntry> entries; //Entries.

            public byte[] reserved; //Reserved for 0x20 alignment.

            /// <summary>
            /// Infx entry.
            /// </summary>
            public struct infxEntry
            {

                public UInt32 itemId; //Item Id. Uses ID structure.
                public UInt32 loadFlag; // Contains the _SAR's Load Flag bitwise OR.

                public bool isNull; //If null.

                /*
                 * enum LoadFlag
                // Sound
                LOAD_SEQ    = ( 1 << 0 ),   //
                LOAD_WSD    = ( 1 << 1 ),   //

                // Bank
                LOAD_BANK   = ( 1 << 2 ),   //

                // Waveform Archive
                LOAD_WARC   = ( 1 << 3 ),   //

                LOAD_ALL    = 0xffffffff    //
                */

            }

        }


        /// <summary>
        /// Reference.
        /// </summary>
        public struct reference
        {

            public UInt16 identifier; //Id of block.
                                      /*
                                       * 0x7900 Group Entry.
                                       * 0x7901 Loader Entry.
                                       */
            public UInt16 padding; //Padding.
            public UInt32 offset; //Offset.

        }


        /// <summary>
        /// Reference table.
        /// </summary>
        public struct referenceTable
        {

            public UInt32 count; //Count.
            public List<reference> references; //References.

        }

        /// <summary>
        /// Sized reference.
        /// </summary>
        public struct sizedReference
        {

            public UInt16 identifier; //Id of block.
                                      /*
                                       * 0x7800 INFO.
                                       * 0x7801 FILE.
                                       * 0x7802 INFX.
                                       * 0x1F00 File Entry.
                                       */
            public UInt16 padding; //Padding.
            public UInt32 offset; //Offset.
            public UInt32 size; //Size.

        }



        /// <summary>
        /// Load a file from bytes.
        /// </summary>
        /// <param name="b"></param>
        public void load(byte[] b)
        {

            //Reader.
            MemoryStream src = new MemoryStream(b);
            BinaryDataReader br = new BinaryDataReader(src);
            br.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            //Read general stuff.
            magic = br.ReadChars(4);
            byteOrder = br.ReadUInt16();
            if (byteOrder != 0xFEFF)
            {
                br.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                byteOrder = 0xFEFF;
            }
            headerSize = br.ReadUInt16();
            version = br.ReadUInt32();
            fileSize = br.ReadUInt32();
            nBlocks = br.ReadUInt16();
            padding = br.ReadUInt16();

            infoReference = new sizedReference();
            fileReference = new sizedReference();
            infxReference = new sizedReference();

            infoReference.identifier = br.ReadUInt16();
            infoReference.padding = br.ReadUInt16();
            infoReference.offset = br.ReadUInt32();
            infoReference.size = br.ReadUInt32();

            fileReference.identifier = br.ReadUInt16();
            fileReference.padding = br.ReadUInt16();
            fileReference.offset = br.ReadUInt32();
            fileReference.size = br.ReadUInt32();

            infxReference.identifier = br.ReadUInt16();
            infxReference.padding = br.ReadUInt16();
            infxReference.offset = br.ReadUInt32();
            infxReference.size = br.ReadUInt32();

            reserved = br.ReadBytes((int)(headerSize - br.Position));

            info = new infoBlock();
            file = new fileBlock();
            infx = new infxBlock();

            //Info block.
            br.Position = infoReference.offset;
            info.magic = br.ReadChars(4);
            info.size = br.ReadUInt32();

            info.groupReferences = new referenceTable();
            info.groupReferences.count = br.ReadUInt32();
            info.groupReferences.references = new List<reference>();

            for (int i = 0; i < info.groupReferences.count; i++)
            {

                reference r = new reference();
                r.identifier = br.ReadUInt16();
                r.padding = br.ReadUInt16();
                r.offset = br.ReadUInt32();
                info.groupReferences.references.Add(r);

            }

            //Group entries.
            info.entries = new List<infoBlock.groupEntry>();
            foreach (reference r in info.groupReferences.references)
            {

                br.Position = r.offset + infoReference.offset + 8;
                infoBlock.groupEntry e = new infoBlock.groupEntry();
                e.fileId = br.ReadUInt32();
                e.fileEntry = new sizedReference();
                e.fileEntry.identifier = br.ReadUInt16();
                e.fileEntry.padding = br.ReadUInt16();
                e.fileEntry.offset = br.ReadUInt32();
                e.fileEntry.size = br.ReadUInt32();

                if (e.fileEntry.offset == 0xFFFFFFFF) { e.isNull = true; } else { e.isNull = false; }
                info.entries.Add(e);

            }


            //FILE.
            br.Position = fileReference.offset;
            file.magic = br.ReadChars(4);
            file.size = br.ReadUInt32();
            file.reserved = br.ReadBytes(0x18);
            file.files = new List<fileBlock.fileEntry>();

            int fileIdCount = 0;
            foreach (infoBlock.groupEntry e in info.entries)
            {

                fileBlock.fileEntry f = new fileBlock.fileEntry();
                if (e.isNull)
                {

                    f.isNull = true;
                    br.Position = e.fileEntry.offset + 0x8 + fileReference.offset;
                    f.fileId = (int)info.entries[fileIdCount].fileId;

                }
                else
                {

                    f.isNull = false;
                    br.Position = e.fileEntry.offset + 0x8 + fileReference.offset;
                    f.file = br.ReadBytes((int)e.fileEntry.size);
                    f.reserved = new byte[0];
                    f.fileId = (int)info.entries[fileIdCount].fileId;

                }
                file.files.Add(f);
                fileIdCount += 1;

            }

            //INFX.
            br.Position = infxReference.offset;
            infx.magic = br.ReadChars(4);
            infx.size = br.ReadUInt32();
            infx.references = new referenceTable();
            infx.references.references = new List<reference>();
            infx.entries = new List<infxBlock.infxEntry>();
            infx.reserved = new byte[0];

            infx.references.count = br.ReadUInt32();
            for (int i = 0; i < infx.references.count; i++)
            {

                reference r = new reference();
                r.identifier = br.ReadUInt16();
                r.padding = br.ReadUInt16();
                r.offset = br.ReadUInt32();
                infx.references.references.Add(r);

            }

            foreach (reference r in infx.references.references)
            {

                infxBlock.infxEntry e = new infxBlock.infxEntry();
                if (r.offset != 0xFFFFFFFF)
                {
                    e.isNull = false;
                    br.Position = infxReference.offset + 8 + r.offset;
                    e.itemId = br.ReadUInt32();
                    e.loadFlag = br.ReadUInt32();
                    infx.entries.Add(e);
                }
                else
                {
                    e.isNull = true;
                    infx.entries.Add(e);
                }

            }

        }



        /// <summary>
        /// Update the file.
        /// </summary>
        /// <param name="endian"></param>
        public void update(Syroot.BinaryData.ByteOrder endian)
        {

            if (endian == Syroot.BinaryData.ByteOrder.BigEndian)
            {
                magic = "FGRP".ToCharArray();
            }
            else
            {
                magic = "CGRP".ToCharArray();
            }
            byteOrder = 0xFEFF;
            headerSize = 0x40;
            version = 0x00010000;
            fileSize = 0xFFFFFFFF;
            nBlocks = 3;
            padding = 0;
            reserved = new byte[8];

            infoReference = new sizedReference();
            fileReference = new sizedReference();
            infxReference = new sizedReference();

            infoReference.identifier = 0x7800;
            infoReference.padding = 0;
            infoReference.offset = 0xFFFFFFFF;
            infoReference.size = 0xFFFFFFFF;

            fileReference.identifier = 0x7801;
            fileReference.padding = 0;
            fileReference.offset = 0xFFFFFFFF;
            fileReference.size = 0xFFFFFFFF;

            infxReference.identifier = 0x7802;
            infxReference.padding = 0;
            infxReference.offset = 0xFFFFFFFF;
            infxReference.size = 0xFFFFFFFF;

            //INFO.
            info.magic = "INFO".ToCharArray();
            info.size = 0xFFFFFFFF;
            info.groupReferences = new referenceTable();

            info.groupReferences.count = (UInt32)info.entries.Count;
            info.groupReferences.references = new List<reference>();
            int offsetGroupRefCount = 4 + 8 * (int)info.groupReferences.count;
            foreach (infoBlock.groupEntry e in info.entries)
            {

                reference r = new reference();
                r.identifier = 0x7900;
                r.padding = 0;
                r.offset = (UInt32)offsetGroupRefCount;
                offsetGroupRefCount += 0x10;
                info.groupReferences.references.Add(r);

            }

            info.entries = new List<infoBlock.groupEntry>();
            int fileOffsetRefCount = 0x18;
            for (int i = 0; i < file.files.Count; i++)
            {

                fileBlock.fileEntry f = file.files[i];
                infoBlock.groupEntry e = new infoBlock.groupEntry();
                if (f.isNull)
                {

                    e.isNull = true;
                    e.fileId = (UInt32)f.fileId;

                }
                else
                {

                    //Notice that this also takes care of file padding. Kills two birds with one stone.
                    e.fileId = (UInt32)f.fileId;
                    e.isNull = false;
                    e.fileEntry = new sizedReference();
                    e.fileEntry.identifier = 0x1F00;
                    e.fileEntry.padding = 0;
                    e.fileEntry.offset = (UInt32)fileOffsetRefCount;
                    e.fileEntry.size = (UInt32)f.file.Length;

                    //Get padding.
                    int size = (int)e.fileEntry.size;
                    while (size % 0x20 != 0)
                    {
                        size += 1;
                    }
                    fileOffsetRefCount += size;
                    f.reserved = new byte[size - (int)e.fileEntry.size];
                    file.files[i] = f;

                }
                info.entries.Add(e);

            }


            //INFO padding.
            int infoSize = 12 + (int)info.groupReferences.count * 8 + info.entries.Count * 0x10;
            int paddingInfo = 0;
            while (infoSize % 0x20 != 0)
            {
                infoSize += 1;
                paddingInfo += 1;
            }
            info.size = (UInt32)infoSize;
            infoReference.size = (UInt32)infoSize;
            info.reserved = new byte[paddingInfo];
            infoReference.offset = headerSize;

            //FILE.
            file.magic = "FILE".ToCharArray();
            file.size = 0xFFFFFFFF;
            file.reserved = new byte[0x18];

            int fileSizeCount = 0x20;
            foreach (fileBlock.fileEntry f in file.files)
            {
                if (!f.isNull)
                {
                    fileSizeCount += f.file.Length + f.reserved.Length;
                }
            }
            file.size = (UInt32)fileSizeCount;
            fileReference.size = (UInt32)fileSizeCount;
            fileReference.offset = headerSize + info.size;

            //INFX.
            infx.magic = "INFX".ToCharArray();
            infx.size = 0xFFFFFFFF;
            infxReference.offset = headerSize + info.size + file.size;
            infx.references = new referenceTable();
            infx.references.references = new List<reference>();
            infx.references.count = (UInt32)infx.entries.Count;
            int infxOffsetRefCount = 4 + (int)infx.references.count * 8;
            foreach (infxBlock.infxEntry e in infx.entries)
            {

                reference r = new reference();
                if (e.isNull)
                {

                    r.identifier = 0;
                    r.offset = 0xFFFFFFFF;
                    r.padding = 0;

                }
                else
                {

                    r.identifier = 0x7901;
                    r.offset = 0;
                    r.offset = (UInt32)infxOffsetRefCount;
                    infxOffsetRefCount += 8;

                }
                infx.references.references.Add(r);

            }

            int infxSize = infxOffsetRefCount + 8;
            int infxPaddingSize = 0;
            while (infxSize % 0x20 != 0)
            {
                infxSize += 1;
                infxPaddingSize += 1;
            }
            infx.reserved = new byte[infxPaddingSize];
            infx.size = (UInt32)infxSize;
            infxReference.size = (UInt32)infxSize;
            fileSize = (UInt32)headerSize + info.size + file.size + infx.size;

        }



        /// <summary>
        /// Convert file to bytes.
        /// </summary>
        public byte[] toBytes(Syroot.BinaryData.ByteOrder endian)
        {

            //Update.
            update(endian);

            //Writer.
            MemoryStream o = new MemoryStream();
            BinaryDataWriter bw = new BinaryDataWriter(o);
            bw.ByteOrder = endian;

            //Write general stuff.
            bw.Write(magic);
            bw.Write(byteOrder);
            bw.Write(headerSize);
            bw.Write(version);
            bw.Write(fileSize);
            bw.Write(nBlocks);
            bw.Write(padding);

            bw.Write(infoReference.identifier);
            bw.Write(infoReference.padding);
            bw.Write(infoReference.offset);
            bw.Write(infoReference.size);

            bw.Write(fileReference.identifier);
            bw.Write(fileReference.padding);
            bw.Write(fileReference.offset);
            bw.Write(fileReference.size);

            bw.Write(infxReference.identifier);
            bw.Write(infxReference.padding);
            bw.Write(infxReference.offset);
            bw.Write(infxReference.size);

            bw.Write(reserved);


            //Info block.
            bw.Write(info.magic);
            bw.Write(info.size);

            bw.Write(info.groupReferences.count);
            foreach (reference r in info.groupReferences.references)
            {

                bw.Write(r.identifier);
                bw.Write(r.padding);
                bw.Write(r.offset);

            }

            //Group entries.
            foreach (infoBlock.groupEntry e in info.entries)
            {

                if (e.isNull)
                {

                    bw.Write(e.fileId);
                    bw.Write((UInt32)0x0);
                    bw.Write((UInt32)0xFFFFFFFF);
                    bw.Write((UInt32)0xFFFFFFFF);

                }
                else
                {

                    bw.Write(e.fileId);
                    bw.Write(e.fileEntry.identifier);
                    bw.Write(e.fileEntry.padding);
                    bw.Write(e.fileEntry.offset);
                    bw.Write(e.fileEntry.size);

                }

            }
            bw.Write(info.reserved);


            //FILE.
            bw.Write(file.magic);
            bw.Write(file.size);
            bw.Write(file.reserved);

            foreach (fileBlock.fileEntry f in file.files)
            {

                if (!f.isNull)
                {

                    bw.Write(f.file);
                    bw.Write(f.reserved);

                }

            }

            //INFX.
            bw.Write(infx.magic);
            bw.Write(infx.size);

            bw.Write(infx.references.count);
            int entryCount = 0;
            foreach (reference r in infx.references.references)
            {

                if (infx.entries[entryCount].isNull)
                {
                    bw.Write((UInt32)0x0);
                    bw.Write((UInt32)0xFFFFFFFF);
                }
                else
                {
                    bw.Write(r.identifier);
                    bw.Write(r.padding);
                    bw.Write(r.offset);
                }
                entryCount += 1;

            }

            foreach (infxBlock.infxEntry e in infx.entries)
            {

                if (!e.isNull)
                {
                    bw.Write(e.itemId);
                    bw.Write(e.loadFlag);
                }

            }
            bw.Write(infx.reserved);

            return o.ToArray();
        }



        /// <summary>
        /// Extract the file to a folder.
        /// </summary>
        /// <param name="path"></param>
        public void extract(string path) {

            //Update
            update(Syroot.BinaryData.ByteOrder.BigEndian);

            //First create INFX file.
            List<string> infoEx = new List<string>();
            infoEx.Add("--- [InfoEx Rescource Loading File] ---");
            infoEx.Add("Usage: FileType[1=SEQ, 2=WSDSET/SEQSET, 3=BNK, 5=WAR] IndexOfItem Flags[0=SEQ, 1=WSD, 2=BNK, 3=WAR, 4=All; Use multiple consecutive numbers if needed.]");
            foreach (infxBlock.infxEntry e in infx.entries) {

                int fileType = (int)((e.itemId & 0xFF000000) >> 24);
                int index = (int)(e.itemId & 0x00FFFFFF);
                List<int> flags = new List<int>();
                if ((e.loadFlag & 0b1) > 0) { flags.Add(0); }
                if ((e.loadFlag & 0b10) > 0) { flags.Add(1); }
                if ((e.loadFlag & 0b100) > 0) { flags.Add(2); }
                if ((e.loadFlag & 0b1000) > 0) { flags.Add(3); }
                if ((e.loadFlag & 0b10000) > 0) { flags = new List<int>(); flags.Add(4); }
                string h = "";
                foreach (int i in flags) { h += "" + i; }
                infoEx.Add("" + fileType + index + h);

            }
            infoEx.Add("--- [End] ---");
            File.WriteAllLines("infoEx.txt", infoEx.ToArray());

            Directory.CreateDirectory(path + "/SEQ");
            Directory.CreateDirectory(path + "/WSD");
            Directory.CreateDirectory(path + "/BNK");
            Directory.CreateDirectory(path + "/WAR");

        }

    }

    

}

