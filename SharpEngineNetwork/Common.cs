using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.CSharp.RuntimeBinder;

namespace SharpEngineNetwork;

public static class Common
{
    public static dynamic ReadPacket(NetDataReader reader, string packetType, Type type)
    {
        var numberProp = reader.GetInt();
        var numberField = reader.GetInt();
        dynamic? packet = Activator.CreateInstance(type);
        try
        {
            if (packet == null)
                throw new UnknownPacketException($"Packet : {packetType}");
        }
        catch(RuntimeBinderException) {}
                
        for (var i = 0; i < numberProp; i++)
        {
            var propName = reader.GetString();
            var prop = type.GetProperty(propName);
            if (prop == null)
                throw new UnknownPropertyException($"Property : {propName} - Packet : {packetType}");
                
            #region Basic Type
    
            if (prop.PropertyType == typeof(string))
                prop.SetValue(packet, reader.GetString());
            else if(prop.PropertyType == typeof(int))
                prop.SetValue(packet, reader.GetInt());
            else if(prop.PropertyType == typeof(short))
                prop.SetValue(packet, reader.GetShort());
            else if(prop.PropertyType == typeof(byte))
                prop.SetValue(packet, reader.GetByte());
            else if(prop.PropertyType == typeof(char))
                prop.SetValue(packet, reader.GetChar());
            else if(prop.PropertyType == typeof(float))
                prop.SetValue(packet, reader.GetFloat());
            else if(prop.PropertyType == typeof(double))
                prop.SetValue(packet, reader.GetDouble());
            else if(prop.PropertyType == typeof(bool))
                prop.SetValue(packet, reader.GetBool());

            #endregion

            #region Array Type
    
            else if (prop.PropertyType == typeof(string[]))
                prop.SetValue(packet, reader.GetStringArray());
            else if(prop.PropertyType == typeof(int[]))
                prop.SetValue(packet, reader.GetIntArray());
            else if(prop.PropertyType == typeof(short[]))
                prop.SetValue(packet, reader.GetShortArray());
            else if(prop.PropertyType == typeof(float[]))
                prop.SetValue(packet, reader.GetFloatArray());
            else if(prop.PropertyType == typeof(double[]))
                prop.SetValue(packet, reader.GetDoubleArray());
            else if (prop.PropertyType == typeof(bool[]))
                prop.SetValue(packet, reader.GetBoolArray());

            #endregion
        }
                        
        for (var i = 0; i < numberField; i++)
        {
            var fieldName = reader.GetString();
            var field = type.GetField(fieldName);
            if (field == null)
                throw new UnknownPropertyException($"Field : {fieldName} - Packet : {packetType}");
                
            #region Basic Type
    
            if (field.FieldType == typeof(string))
                field.SetValue(packet, reader.GetString());
            else if(field.FieldType == typeof(int))
                field.SetValue(packet, reader.GetInt());
            else if(field.FieldType == typeof(short))
                field.SetValue(packet, reader.GetShort());
            else if(field.FieldType == typeof(byte))
                field.SetValue(packet, reader.GetByte());
            else if(field.FieldType == typeof(char))
                field.SetValue(packet, reader.GetChar());
            else if(field.FieldType == typeof(float))
                field.SetValue(packet, reader.GetFloat());
            else if(field.FieldType == typeof(double))
                field.SetValue(packet, reader.GetDouble());
            else if(field.FieldType == typeof(bool))
                field.SetValue(packet, reader.GetBool());

            #endregion

            #region Array Type
    
            else if (field.FieldType == typeof(string[]))
                field.SetValue(packet, reader.GetStringArray());
            else if(field.FieldType == typeof(int[]))
                field.SetValue(packet, reader.GetIntArray());
            else if(field.FieldType == typeof(short[]))
                field.SetValue(packet, reader.GetShortArray());
            else if(field.FieldType == typeof(float[]))
                field.SetValue(packet, reader.GetFloatArray());
            else if(field.FieldType == typeof(double[]))
                field.SetValue(packet, reader.GetDoubleArray());
            else if (field.FieldType == typeof(bool[]))
                field.SetValue(packet, reader.GetBoolArray());

            #endregion
        }

        return packet!;
    }
    
    public static void SendPacket<T>(NetPeer peer, T packet) where T: notnull
    {
        var writer = new NetDataWriter();
        
        var properties = packet.GetType().GetProperties();
        var fields = packet.GetType().GetFields();
        
        writer.Put(packet.GetType().Name);
        writer.Put(properties.Length);
        writer.Put(fields.Length);
        
        foreach (var prop in properties)
        {
            var value = prop.GetValue(packet, null);
            if (value == null)
                continue;
            
            writer.Put(prop.Name);

            #region Basic Type
            
            if (prop.PropertyType == typeof(string))
                writer.Put((string)value);
            else if(prop.PropertyType == typeof(int))
                writer.Put((int)value);
            else if(prop.PropertyType == typeof(short))
                writer.Put((short)value);
            else if(prop.PropertyType == typeof(byte))
                writer.Put((byte)value);
            else if(prop.PropertyType == typeof(char))
                writer.Put((char)value);
            else if(prop.PropertyType == typeof(float))
                writer.Put((float)value);
            else if(prop.PropertyType == typeof(double))
                writer.Put((double)value);
            else if(prop.PropertyType == typeof(bool))
                writer.Put((bool)value);

            #endregion

            #region Array Type
            
            else if (prop.PropertyType == typeof(string[]))
                writer.PutArray((string[])value);
            else if(prop.PropertyType == typeof(int[]))
                writer.PutArray((int[])value);
            else if(prop.PropertyType == typeof(short[]))
                writer.PutArray((short[])value);
            else if(prop.PropertyType == typeof(float[]))
                writer.PutArray((float[])value);
            else if(prop.PropertyType == typeof(double[]))
                writer.PutArray((double[])value);
            else if (prop.PropertyType == typeof(bool[]))
                writer.PutArray((bool[])value);

            #endregion

            else
                throw new UnknownPropertyTypeException($"Type : {prop.PropertyType.Name}");
        }
        foreach (var field in fields)
        {
            var value = field.GetValue(packet);
            if (value == null)
                continue;
            
            writer.Put(field.Name);

            #region Basic Type
            
            if (field.FieldType == typeof(string))
                writer.Put((string)value);
            else if(field.FieldType == typeof(int))
                writer.Put((int)value);
            else if(field.FieldType == typeof(short))
                writer.Put((short)value);
            else if(field.FieldType == typeof(byte))
                writer.Put((byte)value);
            else if(field.FieldType == typeof(char))
                writer.Put((char)value);
            else if(field.FieldType == typeof(float))
                writer.Put((float)value);
            else if(field.FieldType == typeof(double))
                writer.Put((double)value);
            else if(field.FieldType == typeof(bool))
                writer.Put((bool)value);

            #endregion

            #region Array Type
            
            else if (field.FieldType == typeof(string[]))
                writer.PutArray((string[])value);
            else if(field.FieldType == typeof(int[]))
                writer.PutArray((int[])value);
            else if(field.FieldType == typeof(short[]))
                writer.PutArray((short[])value);
            else if(field.FieldType == typeof(float[]))
                writer.PutArray((float[])value);
            else if(field.FieldType == typeof(double[]))
                writer.PutArray((double[])value);
            else if (field.FieldType == typeof(bool[]))
                writer.PutArray((bool[])value);

            #endregion

            else
                throw new UnknownFieldTypeException($"Type : {field.FieldType.Name}");
        }
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }
}