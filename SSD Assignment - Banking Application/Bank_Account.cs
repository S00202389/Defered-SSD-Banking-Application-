using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Banking_Application
{
    public abstract class Bank_Account
    {

        public string accountNo; // Account Number is not sensitive, so we're not encrypting it
        private byte[] encryptedBalance;
        public string name, address_line_1, address_line_2, address_line_3, town;
        public double balance;

        private static byte[] encryptionKey = Encoding.UTF8.GetBytes("A_16_BYTE_KEY!!"); // AES-128 requires 16 bytes key

        protected double GetDecryptedBalance()
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = encryptionKey;
                byte[] decrypted = Decrypt(aes, encryptedBalance);
                return BitConverter.ToDouble(decrypted, 0);
            }
        }

        protected void SetEncryptedBalance(double balance)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = encryptionKey;
                encryptedBalance = Encrypt(aes, BitConverter.GetBytes(balance));
            }
        }

        private byte[] Encrypt(Aes aes, byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }
        }

        private byte[] Decrypt(Aes aes, byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }
        }
        public abstract double getAvailableFunds();
        public void lodge(double amount)
        {
            balance += amount;
        }
        public abstract bool withdraw(double amount);

        ~Bank_Account() // Destructor to clear sensitive data from memory
        {
            encryptedBalance = null;
            GC.Collect(); // Manual garbage collection
        }
    }
}
