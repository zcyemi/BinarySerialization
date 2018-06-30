using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Text;

namespace Serialization
{


    public enum SerializeTypeEnum : byte
    {

        None = 0,
        Byte = 1,
        Bool = 2,
        Int16 = 3,
        Int32 = 4,
        Int64 = 5,
        UInt16 = 6,
        UInt32 = 7,
        UInt64 = 8,
        Float = 9,
        Double = 10,
        String = 11,
        Custom = 12,
    }

    public struct SerializeFieldData : IEquatable<SerializeFieldData>
    {

        public object Value;
        public bool IsArray;
        public Type FieldType;
        public Type ElementType;
        public bool IsPrimitive;
        public Int32 DataSize;
        public string FieldName;
        public SerializeTypeEnum TypeEnum;

        public bool Equals(SerializeFieldData other)
        {
            if(IsArray != other.IsArray) return false;
            if(IsPrimitive != other.IsPrimitive) return false;
            if(IsPrimitive){
                if(TypeEnum != other.TypeEnum) return false;
            }
            else{
                if(FieldType != null && other.FieldType != null && FieldType != other.FieldType) return false;
            }
            return true;
        }
    }

    public class SerializeDataInfo
    {
        private static Dictionary<Type,List<FieldInfo>> s_fieldInfo = new Dictionary<Type, List<FieldInfo>>();
        public List<SerializeFieldData> FieldData;

        public static List<FieldInfo> GetFieldInfos(Type t){
            List<FieldInfo> finfo = null;
            if(s_fieldInfo.TryGetValue(t,out finfo)){
                return finfo;
            }
            finfo = new List<FieldInfo>(t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            s_fieldInfo.Add(t,finfo);
            return finfo;
        }
 
        public static SerializeDataInfo Parse<T>(T t){

            var datainfo = new SerializeDataInfo();
            var finfos = GetFieldInfos(typeof(T)); 

            datainfo.FieldData = new List<SerializeFieldData>();
            for (var i = 0; i < finfos.Count; i++)
            {
                var finfo = finfos[i];
                var val = t != null? finfo.GetValue(t): null;

                var fieldType = finfo.FieldType;
                var isArray = fieldType.IsArray;
                var elementType = isArray ? fieldType.GetElementType() : fieldType;
                var typeEnum = GetTypeEnum(elementType);
                var isPrimitive = typeEnum != SerializeTypeEnum.Custom;

                Int32 datasize = 0;
                if(isPrimitive){
                    if(typeEnum == SerializeTypeEnum.String){

                    }
                    else if(isArray){
                        var ary = val as Array;
                        datasize = ary.Length * Marshal.SizeOf(elementType);
                    }
                    else{
                        datasize = Marshal.SizeOf(elementType);
                    }
                }
                else{
                    datasize = -1;
                }

                var data = new SerializeFieldData
                {
                    Value = val,
                    FieldType = fieldType,
                    ElementType = elementType,
                    IsArray = isArray,
                    IsPrimitive = isPrimitive,
                    DataSize = datasize,
                    FieldName = finfo.Name,
                    TypeEnum = typeEnum,
                };

                datainfo.FieldData.Add(data);
            }
            return datainfo;
        }


        private static Dictionary<Type, SerializeTypeEnum> s_typeMap = new Dictionary<Type, SerializeTypeEnum>{
            { typeof(byte), SerializeTypeEnum.Byte},
            { typeof(bool), SerializeTypeEnum.Bool},
            { typeof(Int16), SerializeTypeEnum.Int16},
            { typeof(Int32), SerializeTypeEnum.Int32},
            { typeof(Int64), SerializeTypeEnum.Int64},
            { typeof(UInt16), SerializeTypeEnum.UInt16},
            { typeof(UInt32), SerializeTypeEnum.UInt32},
            { typeof(UInt64), SerializeTypeEnum.UInt64},
            { typeof(float), SerializeTypeEnum.Float},
            { typeof(double), SerializeTypeEnum.Double},
            { typeof(string), SerializeTypeEnum.String}
        };

        public static SerializeTypeEnum GetTypeEnum(Type t)
        {
            if (s_typeMap.ContainsKey(t)) return s_typeMap[t];
            return SerializeTypeEnum.Custom;
        }

        public void WriteToStream(Stream s){
            UInt16 count = (UInt16)FieldData.Count;
            s.WriteUInt16(count);
            foreach(var fdata in FieldData){
                s.WriteBool(fdata.IsArray);
                s.WriteByte((byte)fdata.TypeEnum);
                s.WriteString(fdata.FieldName,Encoding.ASCII);
            }
        }

        public static SerializeDataInfo ReadFromStream<T>(Stream s){
            var typeDataInfo = SerializeDataInfo.Parse<T>(default(T));

            var deserializeDataInfo = ReadFromStream(s);
            if(!typeDataInfo.Verify(deserializeDataInfo)){
                throw new Exception("type verify failed!");
            }
            
            return deserializeDataInfo;
        }

        public bool Verify(SerializeDataInfo o){
            if(this.FieldData == null || o.FieldData == null) throw new Exception();
            if(this.FieldData.Count != o.FieldData.Count) return false;

            var count = this.FieldData.Count;
            for(var i=0;i<count;i++){
                if(!FieldData[i].Equals(o.FieldData[i])) return false;
            }
            return true;
        }

        public static SerializeDataInfo  ReadFromStream(Stream s){
            UInt16 count = s.ReadUInt16();

            var datainfo =new SerializeDataInfo();
            
            List<SerializeFieldData> fieldDataList = new List<SerializeFieldData>();
            for(var i=0;i<count;i++){
                bool isarray = s.ReadBool();
                var typeenum = (SerializeTypeEnum)s.ReadByte();
                var fname = s.ReadString(Encoding.ASCII);

                var fdata = new SerializeFieldData();
                fdata.FieldName = fname;
                fdata.IsArray = isarray;
                fdata.TypeEnum = typeenum;
                fdata.IsPrimitive = typeenum != SerializeTypeEnum.Custom;

                
                fieldDataList.Add(fdata);
            }

            datainfo.FieldData = fieldDataList;
            return datainfo;
        }
    }
}