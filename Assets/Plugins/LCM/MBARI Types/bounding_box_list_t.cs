/* LCM type definition class file
 * This file was automatically generated by lcm-gen
 * DO NOT MODIFY BY HAND!!!!
 */

using System;
using System.Collections.Generic;
using System.IO;
using LCM.LCM;
 
namespace mwt
{
    public sealed class bounding_box_list_t : LCM.LCM.LCMEncodable
    {
        public long utime;
        public sbyte num_boxes;
        public mwt.bounding_box_t[] boxes;
        public String model_name;
 
        public bounding_box_list_t()
        {
        }
 
        public static readonly ulong LCM_FINGERPRINT;
        public static readonly ulong LCM_FINGERPRINT_BASE = 0x48ac9fddb1ad13c4L;
 
        static bounding_box_list_t()
        {
            LCM_FINGERPRINT = _hashRecursive(new List<String>());
        }
 
        public static ulong _hashRecursive(List<String> classes)
        {
            if (classes.Contains("mwt.bounding_box_list_t"))
                return 0L;
 
            classes.Add("mwt.bounding_box_list_t");
            ulong hash = LCM_FINGERPRINT_BASE
                 + mwt.bounding_box_t._hashRecursive(classes)
                ;
            classes.RemoveAt(classes.Count - 1);
            return (hash<<1) + ((hash>>63)&1);
        }
 
        public void Encode(LCMDataOutputStream outs)
        {
            outs.Write((long) LCM_FINGERPRINT);
            _encodeRecursive(outs);
        }
 
        public void _encodeRecursive(LCMDataOutputStream outs)
        {
            byte[] __strbuf = null;
            outs.Write(this.utime); 
 
            outs.Write(this.num_boxes); 
 
            for (int a = 0; a < this.num_boxes; a++) {
                this.boxes[a]._encodeRecursive(outs); 
            }
 
            __strbuf = System.Text.Encoding.GetEncoding("US-ASCII").GetBytes(this.model_name); outs.Write(__strbuf.Length+1); outs.Write(__strbuf, 0, __strbuf.Length); outs.Write((byte) 0); 
 
        }
 
        public bounding_box_list_t(byte[] data) : this(new LCMDataInputStream(data))
        {
        }
 
        public bounding_box_list_t(LCMDataInputStream ins)
        {
            if ((ulong) ins.ReadInt64() != LCM_FINGERPRINT)
                throw new System.IO.IOException("LCM Decode error: bad fingerprint");
 
            _decodeRecursive(ins);
        }
 
        public static mwt.bounding_box_list_t _decodeRecursiveFactory(LCMDataInputStream ins)
        {
            mwt.bounding_box_list_t o = new mwt.bounding_box_list_t();
            o._decodeRecursive(ins);
            return o;
        }
 
        public void _decodeRecursive(LCMDataInputStream ins)
        {
            byte[] __strbuf = null;
            this.utime = ins.ReadInt64();
 
            this.num_boxes = ins.ReadSByte();
 
            this.boxes = new mwt.bounding_box_t[(int) num_boxes];
            for (int a = 0; a < this.num_boxes; a++) {
                this.boxes[a] = mwt.bounding_box_t._decodeRecursiveFactory(ins);
            }
 
            __strbuf = new byte[ins.ReadInt32()-1]; ins.ReadFully(__strbuf); ins.ReadByte(); this.model_name = System.Text.Encoding.GetEncoding("US-ASCII").GetString(__strbuf);
 
        }
 
        public mwt.bounding_box_list_t Copy()
        {
            mwt.bounding_box_list_t outobj = new mwt.bounding_box_list_t();
            outobj.utime = this.utime;
 
            outobj.num_boxes = this.num_boxes;
 
            outobj.boxes = new mwt.bounding_box_t[(int) num_boxes];
            for (int a = 0; a < this.num_boxes; a++) {
                outobj.boxes[a] = this.boxes[a].Copy();
            }
 
            outobj.model_name = this.model_name;
 
            return outobj;
        }
    }
}

