//------------------------------------------------------------------------------
// <copyright file="CSSqlFunction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data.SqlTypes;
using System.IO;
using System.IO.Compression;
using System.Text;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlBinary CompressString(SqlString rawString)
    {
        if (rawString.IsNull)
            return null;

        var originalBytes = Encoding.UTF8.GetBytes(rawString.Value);

        byte[] compressedBytes = null;

        using (MemoryStream destStream = new MemoryStream())
        using (MemoryStream sourceStream = new MemoryStream(originalBytes))
        using (DeflateStream compressor = new DeflateStream(destStream, CompressionMode.Compress))
        {
            sourceStream.WriteTo(compressor);
            compressor.Close();

            compressedBytes = destStream.ToArray();
        }

        return new SqlBinary(compressedBytes);
    }

    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString DecompressString(SqlBinary rawBinary)
    {
        if (rawBinary.IsNull)
            return null;

        byte[] compressedBinary = rawBinary.Value;

        string finalString = null;

        using(MemoryStream decompressedStream = new MemoryStream())
        using (MemoryStream compressedStream = new MemoryStream(compressedBinary))
        using (DeflateStream deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
        {
            CopyStream(deflateStream, decompressedStream);
            
            //deflateStream.CopyTo(decompressedStream);

            finalString = Encoding.UTF8.GetString(decompressedStream.ToArray());
        }

        return new SqlString(finalString);
    }

    private static void CopyStream(Stream input, MemoryStream output)
    {
        //MemoryStream output = new MemoryStream();
        
        byte[] buffer = new byte[16 * 1024];
        int bytesRead;
        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, bytesRead);
        }

       // return output;
    }

    
}
