using UnityEngine;
using System.Collections;
using System.IO;

namespace ChromeAppBuilder
{
	public class CrxHeader
	{

		public char[] magic_number;
		public uint version;
		public uint public_key_length;
		public uint signature_length;
		public byte[] public_key;
		public byte[] signature;

		public string PublicKey {
			get {
				if (public_key != null) {
					return System.Convert.ToBase64String (public_key);
				}
				return "";
			}
		}

		public string Signature {
			get {
				if (signature != null) {
					return System.Convert.ToBase64String (signature);
				}
				return "";
			}
		}

        public CrxHeader(string filename)
        {
            if (File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        this.magic_number = br.ReadChars(4);
                        if (new string(this.magic_number) != "Cr24")
                        {
                            throw new System.Exception("Invalid .crx file header : magic_number.");
                        }
                        this.version = br.ReadUInt32();
                        if (this.version != 2)
                        {
                            throw new System.Exception("this version of .crx file is currently unsupported.");
                        }
                        this.public_key_length = br.ReadUInt32();
                        this.signature_length = br.ReadUInt32();
                        this.public_key = br.ReadBytes((int)this.public_key_length);
                        this.signature = br.ReadBytes((int)this.signature_length);
                    }
                }
            }
        }
    }
}