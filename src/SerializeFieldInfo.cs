using System;

namespace Rigel.Serialization{
    public struct SerializeFieldInfo : IEquatable<SerializeFieldInfo>
    {
        public bool IsArray;
        public Type FieldType;
        public Type ElementType;
        public bool IsPrimitive;
        public string FieldName;
        public SerializeTypeEnum TypeEnum;
        public byte CustomTypeIndex;

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
    }
}