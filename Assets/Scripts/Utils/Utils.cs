using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using GameSystem;
using UnityEngine;

public static class Utils
{
    public static T Get<T>(GameObject gameObj) where T : MonoBehaviour
    {
        T t = default(T);

        if (!gameObj)
        {
            return t;
        }

        if (t == null)
        {
            t = gameObj.GetComponent<T>();
            if (t == null)
            {
                t = gameObj.AddComponent<T>();
            }
        }

        return t;
    }

    public static T GetOrAddComponent<T>(this GameObject gameObj) where T : Component
    {
        var component = gameObj.GetComponent<T>();
        if (component == null)
        {
            component = gameObj.AddComponent<T>();
        }

        return component;
    }

    public static T GetOrAddComponent<T>(this MonoBehaviour mono) where T : Component
    {
        if(mono == null)
        {
            return default(T);
        }

        var component = mono.gameObject.GetComponent<T>();
        if (component == null)
        {
            component = mono.gameObject.AddComponent<T>();
        }

        return component;
    }

    public static string Decrypt(this string encrypt, string key)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged()
        {
            Mode = CipherMode.CBC,
            Padding = PaddingMode.PKCS7,
            KeySize = 128,
            BlockSize = 128,
        };

        byte[] encryptedData = Convert.FromBase64String(encrypt);
        byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(key);
        byte[] keyBytes = new byte[16];

        int len = pwdBytes.Length;

            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }

        System.Array.Copy(pwdBytes, keyBytes, len);

        rijndaelCipher.Key = keyBytes;
        rijndaelCipher.IV = keyBytes;

        byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);

        return System.Text.Encoding.UTF8.GetString(plainText);
    }

    public static string Encrypt(this string textToEncrypt, string key)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged()
        {
            Mode = CipherMode.CBC,
            Padding = PaddingMode.PKCS7,
            KeySize = 128,
            BlockSize = 128,
        };
        //rijndaelCipher.Mode = CipherMode.CBC;
        //rijndaelCipher.Padding = PaddingMode.PKCS7;

        //rijndaelCipher.KeySize = 128;

        //rijndaelCipher.BlockSize = 128;

        byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(key);
        byte[] keyBytes = new byte[16];
        int len = pwdBytes.Length;

        if (len > keyBytes.Length)
        {
            len = keyBytes.Length;
        }

        System.Array.Copy(pwdBytes, keyBytes, len);

        rijndaelCipher.Key = keyBytes;
        rijndaelCipher.IV = keyBytes;

        ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

        byte[] plainText = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);

        return System.Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
    }
}

