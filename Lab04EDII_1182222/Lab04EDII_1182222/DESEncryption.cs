using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lab04EDII_1182222
{
    public class DESEncryption
    {
        private static string key = "ok:uo1IN";

        public static void EncryptFile(string inputFilePath, string outputFilePath)
        {
            try
            {
                byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);

                byte[] fileBytes = File.ReadAllBytes(inputFilePath);

                // Crear un proveedor DES
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    des.Mode = CipherMode.ECB; // Modo ECB
                    des.Padding = PaddingMode.PKCS7; // Padding PKCS7

                    // Crear un encriptador con la clave
                    ICryptoTransform encryptor = des.CreateEncryptor(keyBytes, null);

                    // Encriptar los bytes del archivo
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(fileBytes, 0, fileBytes.Length);

                    File.WriteAllBytes(outputFilePath, encryptedBytes);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al encriptar el archivo: {ex.Message}");
            }
        }

        public static void DecryptFile(string inputFilePath, string outputFilePath)
        {
            try
            {
                byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);

                byte[] encryptedBytes = File.ReadAllBytes(inputFilePath);

                // Crear un proveedor DES
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    des.Mode = CipherMode.ECB; // Modo ECB
                    des.Padding = PaddingMode.PKCS7; // Padding PKCS7

                    // Crear un desencriptador con la clave
                    ICryptoTransform decryptor = des.CreateDecryptor(keyBytes, null);

                    // Desencriptar los bytes del archivo
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                    // Guardar los bytes desencriptados en el archivo de salida
                    File.WriteAllBytes(outputFilePath, decryptedBytes);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al desencriptar el archivo: {ex.Message}");
            }
        }
    }
}
