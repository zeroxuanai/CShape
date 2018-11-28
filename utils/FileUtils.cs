using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;


public class FileUtils {

    private FileUtils() {}

    private static string bytesToString(byte[] bytes) {
        if (bytes == null) {
            Debug.LogError("转换失败，需要转换的字节为空！！！");
            return null;
        }
        return Encoding.UTF8.GetString(bytes);
    }

    private static byte[] stringToBytes(string str) {

        if (string.IsNullOrEmpty(str)) {
            Debug.LogError("转换失败，需要转换的字符串数据为空！！！");
            return null;
        }
        return Encoding.UTF8.GetBytes(str);
    }

    private static Stream bytesToStream(byte[] bytes) {
        if (bytes == null) {
            Debug.LogError("转换失败，需要转换的bytes为空！！！");
            return null;
        }
        return new MemoryStream(bytes);
    }

    private static byte[] streamToBytes(Stream stream) {

        if (stream == null) {
            Debug.LogError("转换失败，需要转换的stream为空！！！");
            return null;
        }

        byte[] bytes = new byte[stream.Length];
        stream.Seek(0, SeekOrigin.Begin);
        stream.Read(bytes, 0, bytes.Length);
        return bytes;
    }

    private static Stream stringToStream(string str) {
        byte[] bytes = stringToBytes(str);
        if (bytes == null) {
            Debug.LogError("转换失败，需要转换的字符串为空！！！");
            return null;
        }
        return bytesToStream(bytes);
    }

    private static string streamToString(Stream stream) {

        byte[] bytes = streamToBytes(stream);
        if (bytes == null) {
            Debug.LogError("转换失败，需要转换的stream为空！！！");
            return null;
        }
        return bytesToString(bytes);
    }



    /// <summary>
    /// 获取文件流
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>FileStream</returns>
    private static FileStream getFileStream(string filePath) {
        FileStream fs = null;

        if (!File.Exists(filePath)) {
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
    private static byte[] getFileBytes(string filePath) {
        byte[] bytes = null;
        FileStream fs = null;
        BufferedStream bufferedInput = null;

        try {
            fs = getFileStream(filePath);
            bytes = new byte[fs.Length];
            bufferedInput = new BufferedStream(fs);
            bufferedInput.Seek(0, SeekOrigin.Begin);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            long destinationIndex = 0;
            while ((bytesRead = bufferedInput.Read(buffer, 0, buffer.Length)) > 0) {
                Array.Copy(buffer, 0, bytes, destinationIndex, bytesRead);
                destinationIndex = destinationIndex + bytesRead;
            }
        }
        catch (IOException e) {
            Debug.LogError("获取文件字节流错误：" + e.Message);
        }
        finally {
            if (fs != null) {
                fs.Dispose();
            }

            if (bufferedInput != null) {
                bufferedInput.Dispose();
            }
        }

        return bytes;
    }

    /// <summary>
    /// 获取文本文件中的内容
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns></returns>
    private static string getString(string filePath) {

        StringBuilder stringBuilder = null;

        FileStream fs = null;
        BufferedStream bufferedInput = null;

        try {
            stringBuilder = new StringBuilder();
            fs = getFileStream(filePath);
            bufferedInput = new BufferedStream(fs);
            bufferedInput.Seek(0, SeekOrigin.Begin);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = bufferedInput.Read(buffer, 0, buffer.Length)) > 0) {
                byte[] dest = new byte[bytesRead];
                Array.Copy(buffer, 0, dest, 0, bytesRead);
                string str = bytesToString(dest);
                stringBuilder.Append(str);
            }
        }
        catch (IOException e) {
            Debug.LogError(" 获取字符串错误：" + e.Message);
        }
        finally {

            if (fs != null) {
                fs.Dispose();
            }

            if (bufferedInput != null) {
                bufferedInput.Dispose();
            }
        }

        return stringBuilder.ToString();

    }


    /// <summary>
    /// 保存字节数据
    /// </summary>
    /// <param name="bytes">需要保存的字节数据</param>
    /// <param name="filePath">需要保存的文件路径</param>
    /// <returns></returns>
    private static bool saveBytes(byte[] bytes, string filePath) {

        FileStream fs = null;
        Stream memoryStream = null;
        BufferedStream bufferedOutput = null;
        BufferedStream bufferedInput = null;

        try {
            memoryStream = bytesToStream(bytes);

            bufferedInput = new BufferedStream(memoryStream);
            bufferedInput.Seek(0, SeekOrigin.Begin);

            if (string.IsNullOrEmpty(filePath)) {
                Debug.LogError("指定的文件路径为空");
                return false;
            }

            Debug.Log("文件保存路径：" + filePath);
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }
            fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            bufferedOutput = new BufferedStream(fs);
            //use BufferedStream write
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = bufferedInput.Read(buffer, 0, buffer.Length)) > 0) {
                bufferedOutput.Write(buffer, 0, bytesRead);
            }
            bufferedOutput.Flush();
        }
        catch (IOException e) {
            Debug.LogError("保存文件异常：" + e.Message);
        }
        finally {
            if (fs != null) {
                fs.Dispose();
            }

            if (memoryStream != null) {
                memoryStream.Dispose();

            }

            if (bufferedInput != null) {
                bufferedInput.Dispose();
            }

            if (bufferedOutput != null) {
                bufferedOutput.Dispose();
            }
        }

        return true;
    }

    /// <summary>
    /// 保存字符串到文本文件
    /// </summary>
    /// <param name="str">需要保存的字符串</param>
    /// <param name="filePath">文件路径</param>
    /// <returns> bool </returns>
    private static bool saveString(string str, string filePath) {

        if (string.IsNullOrEmpty(str)) {
            Debug.LogError("保存的文本字符串为空");
            return false;
        }
        Debug.Log("需要保存的文本：" + str);

        byte[] byteArray = null;
        try {
            byteArray = stringToBytes(str);
        }
        catch (Exception e) {
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
    private static bool appendString(string str, string filePath) {

        bool isAppend = false;
        FileStream fs = null;

        try {
            fs = getFileStream(filePath);
            fs.Position = fs.Length;
            byte[] newlineBytes = stringToBytes("\r\n");
            fs.Write(newlineBytes, 0, newlineBytes.Length);
            byte[] bytes = stringToBytes(str);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            isAppend = true;
        }
        catch (IOException e) {
            Debug.LogError("Append String is Error:" + e.Message);
        }
        finally {
            if (fs != null) {
                fs.Dispose();
            }

        }

        return isAppend;
    }




    public static string BytesToString(byte[] bytes) {
        return bytesToString(bytes);
    }

    public static byte[] StringToBytes(string str) {
        return stringToBytes(str);
    }

    public static Stream BytesToStream(byte[] bytes) {
        return bytesToStream(bytes);
    }

    public static byte[] StreamToBytes(Stream stream) {
        return streamToBytes(stream);
    }

    public static Stream StringToStream(string str) {
        return stringToStream(str);
    }

    public static string StreamToString(Stream stream) {
        return streamToString(stream);
    }

    public static FileStream GetFileStream(string filePath) {
        return getFileStream(filePath);
    }



    public static byte[] GetFileBytes(string filePath) {
        return getFileBytes(filePath);
    }


    public static string GetString(string filePath) {
        return getString(filePath);
    }


    public static bool SaveBytes(byte[] bytes, string filePath) {
        return saveBytes(bytes, filePath);
    }


    public static bool SaveString(string str, string filePath) {
        return saveString(str, filePath);
    }


    public static bool AppendString(string str, string filePath) {
        return appendString(str, filePath);
    }
}
