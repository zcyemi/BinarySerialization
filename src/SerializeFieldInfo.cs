using System;
using System.IO;
using System.Text;

namespace Rigel.Serialization
{
    public struct SerializeFieldInfo : IEquatable<SerializeFieldInfo>
    {
        public bool FieldActive;
        public bool HasLabel;
        public bool IsArray;
        public Type FieldType;
        public Type ElementType;
        public bool IsPrimitive;
        public string FieldName;
        public SerializeTypeEnum TypeEnum;
        public byte CustomTypeIndex;

        // 0 0 0 0 0000
        // 0: active
        // 1: has label
        // 2: is array
        // 3: is primitive
        // 4-7: type enum
        private byte CalculateRawByte()
        {

            var rawdata = (FieldActive ? 1 : 0) << 7;
            rawdata |= ((HasLabel ? 1 : 0) << 6);
            rawdata |= ((IsArray ? 1 : 0) << 5);
            rawdata |= ((IsPrimitive ? 1 : 0) << 4);
            if (IsPrimitive)
            {
                rawdata |= (byte)TypeEnum;
            }
            else
            {
                rawdata |= (CustomTypeIndex >= 15 ? 0b1111 : CustomTypeIndex);
            }
            return (byte)rawdata;
        }

        public void ApplyRawData(byte rawdata)
        {
            FieldActive = (rawdata & 0b10000000) > 0;
            HasLabel = (rawdata & 0b01000000) > 0;
            IsArray = (rawdata & 0b00100000) > 0;
            IsPrimitive = (rawdata & 0b00010000) > 0;

            byte typeIndex = (byte)(rawdata & 0b00001111);
            if (IsPrimitive)
            {
                TypeEnum = (SerializeTypeEnum)typeIndex;
            }
            else
            {
                TypeEnum = SerializeTypeEnum.Custom;
                CustomTypeIndex = typeIndex;
            }
        }

        public bool Equals(SerializeFieldInfo other)
        {
            if (IsArray != other.IsArray) return false;
            if (IsPrimitive != other.IsPrimitive) return false;
            if (IsPrimitive)
            {
                if (TypeEnum != other.TypeEnum) return false;
            }
            else
            {
                if (FieldType != null && other.FieldType != null && FieldType != other.FieldType) return false;
            }
            return true;
        }

        public void WriteToStream(Stream s, bool extraInfo = false)
        {
            HasLabel = extraInfo;
            var rawdata = CalculateRawByte();
            s.WriteByte(rawdata);
            if (CustomTypeIndex >= 15)
            {
                s.WriteByte(CustomTypeIndex);
            }
            if (HasLabel)
            {
                s.WriteString(FieldName, Encoding.UTF8);
            }
        }

        public static SerializeFieldInfo ReadFromStream(Stream s)
        {
            var fdata = new SerializeFieldInfo();

            var rawdata = (byte)s.ReadByte();
            fdata.ApplyRawData(rawdata);
            if (!fdata.IsPrimitive && fdata.CustomTypeIndex == 15)
            {
                Console.WriteLine("read extra byte");
                fdata.CustomTypeIndex = (byte)s.ReadByte();
            }
            if (fdata.HasLabel)
            {
                fdata.FieldName = s.ReadString(Encoding.UTF8);
            }
            return fdata;
        }
    }
}