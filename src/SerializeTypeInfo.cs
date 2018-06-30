using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Text;
using System.Linq;

namespace Rigel.Serialization
{
    public class SerializeTypeInfo
    {
        public Type DataType = null;
        private static Dictionary<Type, List<FieldInfo>> s_fieldInfo = new Dictionary<Type, List<FieldInfo>>();
        public List<SerializeFieldInfo> FieldData;

        public List<SerializeTypeInfo> CustomDataTypeInfo;

        public static List<FieldInfo>GetFieldInfos(Type t)
        {
            if(t == null){
                throw new Exception("Type can not be null");
            }
            if(t == typeof(object)){
                throw new Exception("Type can not be object");
            }
            List<FieldInfo> finfo = null;
            if (s_fieldInfo.TryGetValue(t, out finfo))
            {
                return finfo;
            }

            var refFields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            finfo = new List<FieldInfo>(refFields);
            s_fieldInfo.Add(t, finfo);
            return finfo;
        }

        public static SerializeTypeInfo Parse<T>()
        {
            return Parse(typeof(T),null);
        }
        public static SerializeTypeInfo Parse<T>(T v)
        {
            return Parse(typeof(T),v);
        }

       public static SerializeTypeInfo Parse(Type type,object t)
        {

            var datainfo = new SerializeTypeInfo();
            datainfo.DataType = type;
            var finfos = GetFieldInfos(type);

            datainfo.FieldData = new List<SerializeFieldInfo>();
            var customDataTypeInfo = new List<SerializeTypeInfo>();
            datainfo.CustomDataTypeInfo = customDataTypeInfo;
            for (var i = 0; i < finfos.Count; i++)
            {
                var finfo = finfos[i];
                var fieldType = finfo.FieldType;
                var isArray = fieldType.IsArray;
                var elementType = isArray ? fieldType.GetElementType() : fieldType;
                var typeEnum = GetTypeEnum(elementType);
                var isPrimitive = typeEnum != SerializeTypeEnum.Custom;
                var customTypeIndex =0;
                if (isPrimitive)
                {

                }
                else
                {
                    var index = customDataTypeInfo.FindIndex(f=>f.DataType == elementType);
                    if(index == -1){
                        customTypeIndex = customDataTypeInfo.Count;
                        customDataTypeInfo.Add(Parse(elementType,null));
                    }
                    else{
                        customTypeIndex = index;
                    }
                }

                if(customTypeIndex>255){
                    throw new Exception("Custom type count must under 255");
                }
                var data = new SerializeFieldInfo
                {
                    FieldActive = true,
                    HasLabel = false,
                    FieldType = fieldType,
                    ElementType = elementType,
                    IsArray = isArray,
                    IsPrimitive = isPrimitive,
                    FieldName = finfo.Name,
                    TypeEnum = typeEnum,
                    CustomTypeIndex = (byte)customTypeIndex,
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

        public void WriteToStream(Stream s, bool extraInfo = false)
        {
            //FieldInfo
            UInt16 count = (UInt16)FieldData.Count;
            s.WriteUInt16(count);
            foreach (var fdata in FieldData)
            {
                fdata.WriteToStream(s,extraInfo);
            }
            //CustomType
            byte customTypeCount = (byte)CustomDataTypeInfo.Count;
            s.WriteByte(customTypeCount);
            for(var i=0;i< customTypeCount;i++){
                CustomDataTypeInfo[i].WriteToStream(s);
            }
        }

        public static SerializeTypeInfo ReadFromStream<T>(Stream s)
        {
            var typeDataInfo = SerializeTypeInfo.Parse<T>();

            var deserializeDataInfo = ReadFromStream(s);
            if (!typeDataInfo.Verify(deserializeDataInfo))
            {
                throw new Exception("type verify failed!");
            }

            return deserializeDataInfo;
        }

        public static SerializeTypeInfo ReadFromStream(Stream s)
        {
            UInt16 count = s.ReadUInt16();
            var datainfo = new SerializeTypeInfo();

            //FieldInfo
            List<SerializeFieldInfo> fieldDataList = new List<SerializeFieldInfo>();
            for (var i = 0; i < count; i++)
            {
                var fdata = SerializeFieldInfo.ReadFromStream(s);
                fieldDataList.Add(fdata);
            }
            datainfo.FieldData = fieldDataList;

            //CustomTypeInfo
            var customCount = s.ReadByte();
            if(customCount > 0){
                datainfo.CustomDataTypeInfo = new List<SerializeTypeInfo>();
                for(var i=0;i<customCount;i++){
                    datainfo.CustomDataTypeInfo.Add(ReadFromStream(s));
                }
            }
            
            return datainfo;
        }

        
        public bool Verify(SerializeTypeInfo o)
        {
            if (this.FieldData == null || o.FieldData == null) throw new Exception();
            if (this.FieldData.Count != o.FieldData.Count) return false;

            var count = this.FieldData.Count;
            for (var i = 0; i < count; i++)
            {
                if (!FieldData[i].Equals(o.FieldData[i])) return false;
            }
            return true;
        }
    }
}