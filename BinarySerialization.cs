using System;
using System.IO;
using System.Text;

namespace Serialization{

    public static class BinarySeralizer{
        public static byte[] Serialize<T>(T t){
            

            var stream = new MemoryStream();
            var dataInfo = SerializeDataInfo.Parse(t);

            dataInfo.WriteToStream(stream);
            //WriteData
            var finfo = dataInfo.FieldData;
            for(var i=0;i<finfo.Count;i++){
                var fdata = finfo[i];
                if(fdata.IsPrimitive){
                    if(fdata.IsArray){

                    }
                    else{
                        WritePrimitiveData(stream,fdata.TypeEnum,fdata.Value);
                    }
                }
                else{
                    throw new Exception();
                }
            }

            return stream.ToArray();
        }

        private static object ReadPrimitiveData(Stream s,SerializeTypeEnum type){
            switch(type){
                case SerializeTypeEnum.Bool:
                    return s.ReadBool();
                case SerializeTypeEnum.Byte:
                    return (byte)s.ReadByte();
                case SerializeTypeEnum.Double:
                    return s.ReadDouble();
                case SerializeTypeEnum.Float:
                    return s.ReadFloat();
                case SerializeTypeEnum.Int16:
                    return s.ReadInt16();
                case SerializeTypeEnum.Int32:
                    return s.ReadInt32();
                case SerializeTypeEnum.Int64:
                    return s.ReadInt64();
                case SerializeTypeEnum.UInt16:
                    return s.ReadUInt16();
                case SerializeTypeEnum.UInt32:
                    return s.ReadUInt32();
                case SerializeTypeEnum.UInt64:
                    return s.ReadUInt64();
                case SerializeTypeEnum.String:
                    return s.ReadString(Encoding.ASCII);
                default:
                    throw new Exception();
            }
        }

        private static void WritePrimitiveData(Stream s,SerializeTypeEnum type,object val){
            switch(type){
                case SerializeTypeEnum.Bool:
                    s.WriteBool((bool)val);
                    break;
                case SerializeTypeEnum.Byte:
                    s.WriteByte((byte)val);
                    break;
                case SerializeTypeEnum.Double:
                    s.WriteDouble((Double)val);
                    break;
                case SerializeTypeEnum.Float:
                    s.WriteFloat((float)val);
                    break;
                case SerializeTypeEnum.Int16:
                    s.WriteInt16((Int16)val);
                    break;
                case SerializeTypeEnum.Int32:
                    s.WriteInt32((Int32)val);
                    break;
                case SerializeTypeEnum.Int64:
                    s.WriteInt64((Int64)val);
                    break;
                case SerializeTypeEnum.UInt16:
                    s.WriteUInt16((UInt16)val);
                    break;
                case SerializeTypeEnum.UInt32:
                    s.WriteUInt32((UInt32)val);
                    break;
                case SerializeTypeEnum.UInt64:
                    s.WriteUInt64((UInt64)val);
                    break;
                case SerializeTypeEnum.String:
                    s.WriteString((string)val,Encoding.ASCII);
                    break;
                default:
                    throw new Exception();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            return Deserialize<T>(new MemoryStream(data));
        }

        public static T Deserialize<T>(Stream stream){
            
            var dataInfo  =SerializeDataInfo.ReadFromStream<T>(stream);
            var type = typeof(T);
            var fieldInfo  = SerializeDataInfo.GetFieldInfos(type);
            T t = (T)Activator.CreateInstance(type);


            var finfo = dataInfo.FieldData;
            for(var i=0;i<finfo.Count;i++){
                var fdata = finfo[i];
                if(fdata.IsPrimitive){
                    if(fdata.IsArray){

                    }
                    else{
                        var val = ReadPrimitiveData(stream,fdata.TypeEnum);
                        fieldInfo[i].SetValue(t,val);    
                    }
                }
                else{
                    throw new Exception();
                }
            }
            return t;
        }
    }
}