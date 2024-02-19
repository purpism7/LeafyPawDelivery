using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using GameSystem;
using UnityEngine;

public static class Utility
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
        if (mono == null)
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

    public static string Decrypt(this string text, string key)
    {
        RijndaelManaged RijndaelCipher = new RijndaelManaged();

        byte[] EncryptedData = Convert.FromBase64String(text);
        byte[] Salt = System.Text.Encoding.ASCII.GetBytes(key.Length.ToString());

        PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(key, Salt);

        //Decryptor 객체를 만든다
        ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

        MemoryStream memoryStream = new MemoryStream(EncryptedData);

        //데이터 읽기(복호화이므로) 용도로 cryptoStream객체를 선언, 초기화
        CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

        //복호화 된 데이터를 담을 바이트 배열을 선언한다.
        // 길이는 알 수 없지만, 암호화 된 데이터 길이보다는 적을 것이기에 그 길이로 선언한다.
        byte[] PlainText = new byte[EncryptedData.Length];

        //복호화 시작
        int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

        memoryStream.Close();
        cryptoStream.Close();

        //복호화 된 데이터를 문자열로 바꾼다.
        string DecryptedData = System.Text.Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);

        return DecryptedData;

        //RijndaelManaged rijndaelCipher = new RijndaelManaged()
        //{
        //    Mode = CipherMode.CBC,
        //    Padding = PaddingMode.PKCS7,
        //    KeySize = 128,
        //    BlockSize = 128,
        //};

        //byte[] encryptedData = Convert.FromBase64String(encrypt);
        //byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(key);
        //byte[] keyBytes = new byte[16];

        //int len = pwdBytes.Length;

        //if (len > keyBytes.Length)
        //{
        //    len = keyBytes.Length;
        //}

        //System.Array.Copy(pwdBytes, keyBytes, len);

        //rijndaelCipher.Key = keyBytes;
        //rijndaelCipher.IV = keyBytes;

        //byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);

        //return System.Text.Encoding.UTF8.GetString(plainText);
    }

    public static string Encrypt(this string text, string key)
    {
        // class 선언, 초기화
        RijndaelManaged RijndaelCipher = new RijndaelManaged();

        // 입력 받은 문자열을 바이트 배열로 변환
        byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(text);

        //딕셔너리 공격을 대비해서 키를 더 풀기 어렵게 만들기 위해서 Salt를 사용한다.
        byte[] Salt = System.Text.Encoding.ASCII.GetBytes(key.Length.ToString());

        //PasswordDeriveBytes 클래스를 사용해서 SecretKey를 얻는다.
        PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(key, Salt);

        ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

        //메모리 스트림 객체를 선언, 초기화
        MemoryStream memoryStream = new MemoryStream();

        //CryptoStream 객체를 암호화 된 데이터를 쓰기 위한 용도로 선언
        CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);

        //암호화 프로세스가 진행된다.
        cryptoStream.Write(PlainText, 0, PlainText.Length);

        //암호화 종료
        cryptoStream.FlushFinalBlock();

        //암호화 된 데이터를 바이트 배열로 담는다.
        byte[] CipherBytes = memoryStream.ToArray();

        //스트림 해제
        memoryStream.Close();
        cryptoStream.Close();

        //암호화 된 데이터를 base64 인코딩 된 문자열로 변환한다.
        string EncryptedData = Convert.ToBase64String(CipherBytes);

        //최종 결과를 리턴
        return EncryptedData;

        //RijndaelManaged rijndaelCipher = new RijndaelManaged()
        //{
        //    Mode = CipherMode.CBC,
        //    Padding = PaddingMode.PKCS7,
        //    KeySize = 128,
        //    BlockSize = 128,
        //};
        ////rijndaelCipher.Mode = CipherMode.CBC;
        ////rijndaelCipher.Padding = PaddingMode.PKCS7;

        ////rijndaelCipher.KeySize = 128;

        ////rijndaelCipher.BlockSize = 128;

        //byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(key);
        //byte[] keyBytes = new byte[16];
        //int len = pwdBytes.Length;

        //if (len > keyBytes.Length)
        //{
        //    len = keyBytes.Length;
        //}

        //System.Array.Copy(pwdBytes, keyBytes, len);

        //rijndaelCipher.Key = keyBytes;
        //rijndaelCipher.IV = keyBytes;

        //ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

        //byte[] plainText = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);

        //return System.Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
    }

    public static string KeyJsonFilePath
    {
        get
        {
            return "JsonFilePath_2";
        }
    }

    public static string GetInfoPath()
    {
//#if UNITY_EDITOR
        //var jsonFilePath = "Assets";
//#else
        //var jsonFilePath = Application.persistentDataPath;
//#endif

        //var jsonFilePath = ;

        //var path = PlayerPrefs.GetString(KeyJsonFilePath);
        //Debug.Log("path = " + path);
        //if (!string.IsNullOrEmpty(path))
        //{
        //    resPath = path;
        //}

        return Path.Combine(Application.persistentDataPath, "Info");
    }
}

