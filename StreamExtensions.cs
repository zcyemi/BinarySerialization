using System;
using System.IO;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime;
using System.Diagnostics;

namespace Serialization
{
    public static class StreamExtensions
    {
        public static byte ReadByte(this Stream s)
        {
            return (byte)s.ReadByte();
        }

        public static UInt32 ReadUInt32(this Stream s)
        {
            Span<byte> temp = stackalloc byte[4];
            s.Read(temp);
            return BitConverter.ToUInt32(temp);
        }

        public static Int32 ReadInt32(this Stream s)
        {
            Span<byte> temp = stackalloc byte[4];
            s.Read(temp);
            return BitConverter.ToInt32(temp);
        }

        public static Int16 ReadInt16(this Stream s)
        {
            Span<byte> temp = stackalloc byte[2];
            s.Read(temp);
            return BitConverter.ToInt16(temp);
        }

        public static UInt16 ReadUInt16(this Stream s)
        {
            Span<byte> temp = stackalloc byte[2];
            s.Read(temp);
            return BitConverter.ToUInt16(temp);
        }

        public static UInt64 ReadUInt64(this Stream s)
        {
            Span<byte> temp = stackalloc byte[8];
            s.Read(temp);
            return BitConverter.ToUInt64(temp);
        }

        public static Int64 ReadInt64(this Stream s)
        {
            Span<byte> temp = stackalloc byte[8];
            s.Read(temp);
            return BitConverter.ToInt64(temp);
        }

        public static float ReadFloat(this Stream s)
        {
            Span<byte> temp = stackalloc byte[4];
            s.Read(temp);
            return BitConverter.ToSingle(temp);
        }

        public static double ReadDouble(this Stream s)
        {
            Span<byte> temp = stackalloc byte[8];
            s.Read(temp);
            return BitConverter.ToDouble(temp);
        }

        public static string ReadString(this Stream s, int count, Encoding encoding)
        {
            byte[] temp = new byte[count];
            s.Read(temp);
            return encoding.GetString(temp);
        }

        public static void WriteInt32(this Stream s, int v)
        {
            s.Write(BitConverter.GetBytes(v));
        }

        public static void WriteUInt32(this Stream s, uint v)
        {
            s.Write(BitConverter.GetBytes(v));
        }

        public static void WriteInt16(this Stream s, Int16 v)
        {
            s.Write(BitConverter.GetBytes(v));
        }
        public static void WriteUInt16(this Stream s, UInt16 v)
        {
            s.Write(BitConverter.GetBytes(v));
        }

        public static void WriteInt64(this Stream s, Int64 v)
        {
            s.Write(BitConverter.GetBytes(v));
        }

        public static void WriteUInt64(this Stream s, UInt64 v)
        {
            s.Write(BitConverter.GetBytes(v));
        }

        public static void WriteFloat(this Stream s, float v)
        {
            s.Write(BitConverter.GetBytes(v));
        }

        public static void WriteDouble(this Stream s, double v)
        {
            s.Write(BitConverter.GetBytes(v));
        }

        public static int WriteString(this Stream s, string str, Encoding encoding)
        {
            if(str == null){
                s.WriteInt32(-1);
                return 0;
            }

            if(str == ""){
                s.WriteInt32(0);
                return 0;
            }

            var bytes = encoding.GetBytes(str);
            s.WriteInt32(bytes.Length);
            s.Write(bytes);
            return bytes.Length;
        }

        public static int WriteStringWithLength(this Stream s, string str, Encoding encoding)
        {
            if(str == null){
                s.WriteInt32(-1);
                return 0;
            }

            if(str == ""){
                s.WriteInt32(0);
                return 0;
            }

            var bytes = encoding.GetBytes(str);
            var len = bytes.Length;
            s.WriteInt32(len);
            s.Write(bytes);
            return len;
        }

        public static string ReadString(this Stream s, Encoding encoding)
        {
            var len = s.ReadInt32();
            if (len == 0) return "";
            if(len == -1) return null;
            var temp = new byte[len];
            s.Read(temp, 0, len);
            return encoding.GetString(temp);
        }

        public static void WriteBool(this Stream s, bool v)
        {
            s.WriteByte((byte)(v ? 1 : 0));
        }

        public static bool ReadBool(this Stream s)
        {
            return s.ReadByte() == 1;
        }
        
        public static T[] ReadToArray<T>(this Stream s,int length){
            var size = Marshal.SizeOf<T>();
            var bytesize = size * length;

            byte[] temp = new byte[bytesize];
            s.Read(temp,0,bytesize);

            T[] ary = new T[length];
            var handle = GCHandle.Alloc(ary, GCHandleType.Pinned);
            Marshal.Copy(temp,0,handle.AddrOfPinnedObject(),bytesize);
            handle.Free();
            return ary;
        }


    }
}