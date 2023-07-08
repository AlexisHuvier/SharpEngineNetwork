namespace SharpEngineNetwork;

public class UnknownPropertyTypeException : Exception
{
    public UnknownPropertyTypeException(string? message): base(message) {}
}

public class UnknownFieldTypeException : Exception
{
    public UnknownFieldTypeException(string? message): base(message) {}
}

public class UnknownPacketException : Exception
{
    public UnknownPacketException(string? message): base(message) {}
}

public class UnknownPropertyException : Exception
{
    public UnknownPropertyException(string? message): base(message) {}
}