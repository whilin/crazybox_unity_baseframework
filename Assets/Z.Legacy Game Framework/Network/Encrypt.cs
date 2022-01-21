using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
/**
 * 
 * 
 * http://www.codeproject.com/Articles/5719/Simple-encrypting-and-decrypting-data-in-C
 * 
 * http://www.deltasblog.co.uk/code-snippets/basic-encryptiondecryption-c/
 * 
 * http://aspsnippets.com/Articles/AES-Encryption-Decryption-Cryptography-Tutorial-with-example-in-ASPNet-using-C-and-VBNet.aspx
 * 
 */

public class CryptLib 
{
    //const string myKey = "gunmaneo";
   // const string EncryptionKey = "MAKV2SPBNI99212";
    const string EncryptionKey = "gunmaneo";

    public static string Encrypt(string input)
    {
        return input;
    }

    public static string Decrypt(string input)
    {
        return input;
    }

    //public static string Encrypt(string input)
    //{
    //    if (string.IsNullOrEmpty(input))
    //        return string.Empty;

    //    try
    //    {
    //        /*
    //        byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
    //        TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
    //        tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
    //        tripleDES.Mode = CipherMode.ECB;
    //        tripleDES.Padding = PaddingMode.PKCS7;
    //        ICryptoTransform cTransform = tripleDES.CreateEncryptor();
    //        byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
    //        tripleDES.Clear();
    //        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    //         */
    //        //string EncryptionKey = "MAKV2SPBNI99212";
    //        byte[] clearBytes = Encoding.Unicode.GetBytes(input);
    //        using (Aes encryptor = Aes.Create())
    //        {
    //            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
    //            encryptor.Key = pdb.GetBytes(32);
    //            encryptor.IV = pdb.GetBytes(16);
    //            using (MemoryStream ms = new MemoryStream())
    //            {
    //                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
    //                {
    //                    cs.Write(clearBytes, 0, clearBytes.Length);
    //                    cs.Close();
    //                }
    //                input = Convert.ToBase64String(ms.ToArray());
    //            }
    //        }
    //        return input;

    //    }
    //    catch (Exception e)
    //    {
    //        UnityEngine.Debug.LogError(e);
    //    }

    //    return string.Empty;
    //}

    //public static string Decrypt(string input)
    //{
    //    if (string.IsNullOrEmpty(input))
    //        return string.Empty;

    //    try
    //    {
    //        /*
    //        byte[] inputArray = Convert.FromBase64String(input);
    //        TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
    //        tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
    //        tripleDES.Mode = CipherMode.ECB;
    //        tripleDES.Padding = PaddingMode.PKCS7;
    //        ICryptoTransform cTransform = tripleDES.CreateDecryptor();
    //        byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
    //        tripleDES.Clear();
    //        return UTF8Encoding.UTF8.GetString(resultArray);
    //         */

    //        //string EncryptionKey = "MAKV2SPBNI99212";
    //        byte[] cipherBytes = Convert.FromBase64String(input);
    //        using (Aes encryptor = Aes.Create())
    //        {
    //            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
    //            encryptor.Key = pdb.GetBytes(32);
    //            encryptor.IV = pdb.GetBytes(16);
    //            using (MemoryStream ms = new MemoryStream())
    //            {
    //                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
    //                {
    //                    cs.Write(cipherBytes, 0, cipherBytes.Length);
    //                    cs.Close();
    //                }
    //                input = Encoding.Unicode.GetString(ms.ToArray());
    //            }
    //        }
    //        return input;
    //    }
    //    catch (Exception e)
    //    {
    //        UnityEngine.Debug.LogError(e);
    //    }

    //    return string.Empty;
    //}

}
