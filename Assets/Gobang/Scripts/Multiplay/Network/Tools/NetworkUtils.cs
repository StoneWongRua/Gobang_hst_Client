using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Reflection;

public static class NetworkUtils
{
    /// <summary>
    /// obj->bytes，如果obj未被标记成[serializable] 则返回null
    /// 参考DirectProtoBufTools.Serialize(Object obj)
    /// </summary>
    public static byte[] Serialize(object obj)
    {
        //物体不为空且可被序列化
        if (obj == null || !obj.GetType().IsSerializable)
        {
            return null;
        }

        IFormatter formatter = new BinaryFormatter();
        formatter.Binder = new UBinder();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, obj);
            byte[] data = stream.ToArray();
            return data;
        }
    }

    /// <summary>
    /// bytes->obj，如果obj未被标记成[serializable] 则返回null
    /// 参考DirectProtoBufTools.Deserialize(Object obj)
    /// </summary>
    public static T Deserialize<T>(byte[] data) where T : class
    {
        //数据不为空且T是可序列化的类型
        if (data == null || !typeof(T).IsSerializable)
        {
            return null;
        }

        IFormatter formatter = new BinaryFormatter();
        formatter.Binder = new UBinder();
        using (MemoryStream stream = new MemoryStream(data))
        {
            object obj = formatter.Deserialize(stream);
            return obj as T;

        }
    }

    /// <summary>
    /// 获取本机IPv4,获取失败则返回null
    /// </summary>
    public static string GetLocalIPv4()
    {
        string hostname = Dns.GetHostName();
        IPHostEntry ipEntry = Dns.GetHostEntry(hostname);
        for (int i = 0; i < ipEntry.AddressList.Length; i++)
        {
            //从IP地址列表中筛选出IPv4类型的IP地址
            if (ipEntry.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ipEntry.AddressList[i].ToString();
            }
        }
        return null;
    }

    /// <summary>
    /// 比特数组 -> 字符串
    /// </summary>
    public static string Byte2String(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// 字符串 -> 比特数组
    /// </summary>
    public static byte[] String2Byte(string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }
}

public class UBinder : SerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName)
    {
        Assembly ass = Assembly.GetExecutingAssembly();
        return ass.GetType(typeName);
    }
}