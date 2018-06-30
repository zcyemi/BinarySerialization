using System;
using System.IO;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Serialization
{

    public static class BinarySeralizer
    {
        public static byte[] Serialize<T>(T t, bool extraInfo = false)
        {
            var stream = new MemoryStream();
            var dataInfo = SerializeDataInfo.Parse(t);

            dataInfo.WriteToStream(stream, extraInfo);
            //WriteData
            var finfo = dataInfo.FieldData;
            for (var i = 0; i < finfo.Count; i++)
            {
                var fdata = finfo[i];
                if (fdata.IsPrimitive)
                {
                    if (fdata.IsArray)
                    {
                        if (fdata.Value == null)
                        {
                            stream.WriteBool(false);
                        }
                        else
                        {
                            stream.WriteBool(true);
                            var array = fdata.Value as Array;
                            stream.WriteUInt32((uint)array.Length);
                            if (array.Length != 0)
                            {
                                WritePrimitiveDataArray(stream, fdata.TypeEnum, array);
                            }
                        }
                    }
                    else
                    {
                        WritePrimitiveData(stream, fdata.TypeEnum, fdata.Value);
                    }
                }
                else
                {
                    throw new Exception();
                }
            }

            return stream.ToArray();
        }

        private static object ReadPrimitiveData(Stream s, SerializeTypeEnum type)
        {
            switch (type)
            {
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

        private static void WritePrimitiveData(Stream s, SerializeTypeEnum type, object val)
        {
            switch (type)
            {
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
                    s.WriteString((string)val, Encoding.ASCII);
                    break;
                default:
                    throw new Exception();
            }
        }

        private static void WritePrimitiveDataArray(Stream s, SerializeTypeEnum type, Array tval)
        {
            switch (type)
            {
                case SerializeTypeEnum.Bool:
                    {
                        var val = tval as bool[];
                        for (var i = 0; i < val.Length; i++) s.WriteBool(val[i]);
                    }
                    break;
                case SerializeTypeEnum.Byte:
                    {
                        var val = tval as byte[];
                        s.Write(val, 0, val.Length);
                    }
                    break;
                case SerializeTypeEnum.Double:
                    {
                        var val = tval as double[];
                        for (var i = 0; i < val.Length; i++) s.WriteDouble(val[i]);
                    }
                    break;
                case SerializeTypeEnum.Float:
                    {
                        var val = tval as float[];
                        for (var i = 0; i < val.Length; i++) s.WriteFloat(val[i]);
                    }
                    break;
                case SerializeTypeEnum.Int16:
                    {
                        var val = tval as Int16[];
                        for (var i = 0; i < val.Length; i++) s.WriteInt16(val[i]);
                    }
                    break;
                case SerializeTypeEnum.Int32:
                    {
                        var val = tval as Int32[];
                        for (var i = 0; i < val.Length; i++) s.WriteInt32(val[i]);
                    }
                    break;
                case SerializeTypeEnum.Int64:
                    {
                        var val = tval as Int64[];
                        for (var i = 0; i < val.Length; i++) s.WriteInt64(val[i]);
                    }
                    break;
                case SerializeTypeEnum.UInt16:
                    {
                        var val = tval as UInt16[];
                        for (var i = 0; i < val.Length; i++) s.WriteUInt16(val[i]);
                    }
                    break;
                case SerializeTypeEnum.UInt32:
                    {
                        var val = tval as UInt32[];
                        for (var i = 0; i < val.Length; i++) s.WriteUInt32(val[i]);
                    }
                    break;
                case SerializeTypeEnum.UInt64:
                    {
                        var val = tval as UInt64[];
                        for (var i = 0; i < val.Length; i++) s.WriteUInt64(val[i]);
                    }
                    break;
                case SerializeTypeEnum.String:
                    {
                        var val = tval as string[];
                        for (var i = 0; i < val.Length; i++) s.WriteString(val[i], Encoding.UTF8);
                    }
                    break;
                default:
                    throw new Exception();
            }
        }

        public static Array ReadPrimitiveDataArray(Stream s,SerializeTypeEnum type,int length){
             switch (type)
            {
                case SerializeTypeEnum.Bool:
                    return s.ReadToArray<bool>(length);
                case SerializeTypeEnum.Byte:
                    byte[] temp = new byte[length];
                    s.Read(temp,0,length);
                    return temp;
                case SerializeTypeEnum.Double:
                    return s.ReadToArray<double>(length);
                case SerializeTypeEnum.Float:
                    return s.ReadToArray<float>(length);
                case SerializeTypeEnum.Int16:
                    return s.ReadToArray<Int16>(length);
                case SerializeTypeEnum.Int32:
                    return s.ReadToArray<Int32>(length);
                case SerializeTypeEnum.Int64:
                    return s.ReadToArray<Int64>(length);
                case SerializeTypeEnum.UInt16:
                    return s.ReadToArray<UInt16>(length);
                case SerializeTypeEnum.UInt32:
                    return s.ReadToArray<UInt32>(length);
                case SerializeTypeEnum.UInt64:
                    return s.ReadToArray<UInt64>(length);
                case SerializeTypeEnum.String:
                    var ary= new string[length];
                    for(var i=0;i<length;i++){
                        ary[i] = s.ReadString(Encoding.UTF8);
                    }
                    return ary;
                default:
                    throw new Exception();
            }
        }


        public static T Deserialize<T>(byte[] data)
        {
            return Deserialize<T>(new MemoryStream(data));
        }

        public static T Deserialize<T>(Stream stream)
        {

            var dataInfo = SerializeDataInfo.ReadFromStream<T>(stream);
            var type = typeof(T);
            var fieldInfo = SerializeDataInfo.GetFieldInfos(type);
            T t = (T)Activator.CreateInstance(type);


            var finfo = dataInfo.FieldData;
            for (var i = 0; i < finfo.Count; i++)
            {
                var fdata = finfo[i];
                if (fdata.IsPrimitive)
                {
                    if (fdata.IsArray)
                    {
                        bool notNull = stream.ReadBool();
                        if(notNull == false){
                            fieldInfo[i].SetValue(t,null);
                        }
                        else{
                            var length = stream.ReadUInt32();
                            var ary = ReadPrimitiveDataArray(stream,fdata.TypeEnum,(int)length);
                            fieldInfo[i].SetValue(t, ary);
                        }
                    }
                    else
                    {
                        var val = ReadPrimitiveData(stream, fdata.TypeEnum);
                        fieldInfo[i].SetValue(t, val);
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            return t;
        }
    }
}