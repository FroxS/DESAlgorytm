using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace DESAlgorytmEncryptDecrypt
{
    public class MainViewModel :ObservableObject
    {
        private string DESKey = "AQWSEDRF";
        private string DESIV = "HGFEDCBA";
        public string EncryptedText { get; set; }

        public string DecryptedText { get; set; }

        public ICommand DecryptCommand { get; }

        public ICommand EncryptCommand { get; }
        public MainViewModel()
        {
            DecryptCommand = new RelayCommand((o) => Decrypt(), o => !string.IsNullOrEmpty(EncryptedText));
            EncryptCommand = new RelayCommand((o) => Encrypt(), o => !string.IsNullOrEmpty(DecryptedText));
        }


        public void Decrypt()
        {
            if (string.IsNullOrEmpty(EncryptedText))
            {
                MessageBox.Show("Podaj text do odszyfrowania");
                return;
            }

            try
            {
                string encryptedMessage = EncryptedText;

                byte[] encryptedMessageBytes = Convert.FromBase64String(encryptedMessage);
                byte[] DESKeyBytes = ASCIIEncoding.ASCII.GetBytes(DESKey);
                byte[] DESIVBytes = ASCIIEncoding.ASCII.GetBytes(DESIV);

                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                ICryptoTransform transform = provider.CreateDecryptor(DESKeyBytes, DESIVBytes);
                CryptoStreamMode mode = CryptoStreamMode.Write;


                MemoryStream memStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memStream, transform, mode);
                cryptoStream.Write(encryptedMessageBytes, 0, encryptedMessageBytes.Length);
                cryptoStream.FlushFinalBlock();

                byte[] decryptedMessageBytes = new byte[memStream.Length];
                memStream.Position = 0;
                memStream.Read(decryptedMessageBytes, 0, decryptedMessageBytes.Length);

                string message = ASCIIEncoding.ASCII.GetString(decryptedMessageBytes);
                EncryptedText = string.Empty;

                DecryptedText = message;
                OnPropertyChanged(nameof(DecryptedText));
                OnPropertyChanged(nameof(EncryptedText));
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Nie udało się odszyfrować: {EncryptedText}");
            }
            
        }

        public void Encrypt()
        {
            if (string.IsNullOrEmpty(DecryptedText))
            {
                MessageBox.Show("Wypełnij text to zaszyfrowania");
                return;
            }

            try
            {
                string message = DecryptedText;

                byte[] messageBytes = ASCIIEncoding.ASCII.GetBytes(message);
                byte[] DESKeyBytes = ASCIIEncoding.ASCII.GetBytes(DESKey);
                byte[] DESIVBytes = ASCIIEncoding.ASCII.GetBytes(DESIV);

                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                ICryptoTransform transform = provider.CreateEncryptor(DESKeyBytes, DESIVBytes);
                CryptoStreamMode mode = CryptoStreamMode.Write;


                MemoryStream memStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memStream, transform, mode);
                cryptoStream.Write(messageBytes, 0, messageBytes.Length);
                cryptoStream.FlushFinalBlock();

                byte[] encryptedMessageBytes = new byte[memStream.Length];
                memStream.Position = 0;
                memStream.Read(encryptedMessageBytes, 0, encryptedMessageBytes.Length);

                string encryptedMessage = Convert.ToBase64String(encryptedMessageBytes);
                DecryptedText = string.Empty;

                EncryptedText = encryptedMessage;
                OnPropertyChanged(nameof(DecryptedText));
                OnPropertyChanged(nameof(EncryptedText));
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Nie udało się zaszyfrować: {DecryptedText}");
            }

            
        }
    }
}
