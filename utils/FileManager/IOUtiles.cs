using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;


public class IOUtiles
{

    private IOUtiles()
    {

    }

    private const int DEFAULT_BUFFER_SIZE = 1024 * 4;

    public static string BytesToString(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }

    public static byte[] StringToBytes(string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }

    public static Stream BytesToStream(byte[] bytes)
    {
        return new MemoryStream(bytes);
    }

    public static byte[] StreamToBytes(Stream stream)
    {
        MemoryStream output = new MemoryStream();
        Copy(stream, output);
        return output.ToArray(); ;
    }


    public static Stream StringToStream(string str)
    {
        byte[] bytes = StringToBytes(str);
        return BytesToStream(bytes);
    }

    public static string streamToString(Stream stream)
    {
        byte[] bytes = StreamToBytes(stream);
        return BytesToString(bytes);
    }



    /// <summary>
    /// 获取文件流
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>FileStream</returns>
    public static FileStream getFileStream(string filePath)
    {
        FileStream fs = null;

        if (!File.Exists(filePath))
        {
            Debug.LogError("文件不存在");
            return fs;
        }

        fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
        fs.Seek(0, SeekOrigin.Begin);
        return fs;
    }





    /// <summary>
    /// 获取文件字节流
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns></returns>
    public static byte[] getFileBytes(string filePath)
    {
        MemoryStream output = new MemoryStream();
        using (FileStream fs = getFileStream(filePath))
        {
            Copy(fs, output);
        }
        return output.ToArray();
    }

    /// <summary>
    /// 获取文本文件中的内容
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns></returns>
    public static string getString(string filePath)
    {
        using (FileStream fs = getFileStream(filePath))
        {
            StringWriter output = new StringWriter();
            Copy(fs, output);
            return output.ToString();
        }
    }



    /// <summary>
    /// 保存字节数据
    /// </summary>
    /// <param name="bytes">需要保存的字节数据</param>
    /// <param name="filePath">需要保存的文件路径</param>
    /// <returns></returns>
    public static bool saveBytes(byte[] bytes, string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        using (Stream memoryStream = BytesToStream(bytes))
        {
            if (memoryStream == null)
            {
                return false;
            }
            Copy(memoryStream, fileStream);
            return true;
        }
    }


    public static long Copy(Stream input, Stream output)
    {
        using (BufferedStream bufferInput = new BufferedStream(input))
        using (BufferedStream bufferOutput = new BufferedStream(output))
        {

            byte[] buffer = new byte[DEFAULT_BUFFER_SIZE];
            int n = 0;
            long count = 0L;
            while ((n = bufferInput.Read(buffer, 0, DEFAULT_BUFFER_SIZE)) > 0)
            {
                bufferOutput.Write(buffer, 0, n);
            }
            bufferOutput.Flush();
            return count;
        }

    }


    public static long Copy(Stream input, TextWriter output)
    {
        using (BinaryReader binaryReader = new BinaryReader(input))
        {
            char[] buffer = new char[DEFAULT_BUFFER_SIZE];
            int n = 0;
            long count = 0L;
            while ((n = binaryReader.Read(buffer, 0, DEFAULT_BUFFER_SIZE)) > 0)
                output.Write(buffer, 0, n);
            output.Flush();
            return count;
        }
    }

    public static long Copy(Stream input, MemoryStream output)
    {
        using (BufferedStream buffered = new BufferedStream(input))
        {
            MemoryStream memoryStream = new MemoryStream();
            byte[] buffer = new byte[DEFAULT_BUFFER_SIZE];
            int length = 0;
            long count = 0L;
            while ((length = buffered.Read(buffer, 0, DEFAULT_BUFFER_SIZE)) > 0)
            {
                memoryStream.Write(buffer, 0, length);
                count += length;
            }
            memoryStream.Flush();
            return count;
        }
    }


    /// <summary>
    /// 保存字符串到文本文件
    /// </summary>
    /// <param name="str">需要保存的字符串</param>
    /// <param name="filePath">文件路径</param>
    /// <returns> bool </returns>
    public static bool saveString(string str, string filePath)
    {

        if (string.IsNullOrEmpty(str))
        {
            Debug.LogError("保存的文本字符串为空");
            return false;
        }
        Debug.Log("需要保存的文本：" + str);

        byte[] byteArray = null;
        try
        {
            byteArray = StringToBytes(str);
        }
        catch (Exception e)
        {
            Debug.LogError("string to byte[] error:" + e.Message);
        }

        bool isSave = saveBytes(byteArray, filePath);
        return isSave;

    }

    /// <summary>
    /// 追加字符串到指定文件
    /// </summary>
    /// <param name="str">需要追加的字符串</param>
    /// <param name="filePath">文件路径</param>
    /// <returns></returns>
    public static bool appendString(string str, string filePath)
    {

        bool isAppend = false;
        using (FileStream fs = getFileStream(filePath))
        {
            fs.Position = fs.Length;
            byte[] newlineBytes = StringToBytes("\r\n");
            fs.Write(newlineBytes, 0, newlineBytes.Length);
            byte[] bytes = StringToBytes(str);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            isAppend = true;
        }
        return isAppend;
    }

}
