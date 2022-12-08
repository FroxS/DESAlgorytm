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
        public string Password { private get; set; }
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

            if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Podaj hasło");
                return;
            }

            if (Password.Length < 8)
            {
                MessageBox.Show("Za krótkie hasło minimum 8 znaków");
                return;
            }
            try
            {
                string encryptedMessage = EncryptedText;
                string password = Password;

                byte[] encryptedMessageBytes = Convert.FromBase64String(encryptedMessage);
                byte[] passwordBytes = ASCIIEncoding.ASCII.GetBytes(password);

                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                ICryptoTransform transform = provider.CreateDecryptor(passwordBytes, passwordBytes);
                CryptoStreamMode mode = CryptoStreamMode.Write;


                MemoryStream memStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memStream, transform, mode);
                cryptoStream.Write(encryptedMessageBytes, 0, encryptedMessageBytes.Length);
                cryptoStream.FlushFinalBlock();

                byte[] decryptedMessageBytes = new byte[memStream.Length];
                memStream.Position = 0;
                memStream.Read(decryptedMessageBytes, 0, decryptedMessageBytes.Length);

                string message = ASCIIEncoding.ASCII.GetString(decryptedMessageBytes);


                DecryptedText = message;
                OnPropertyChanged(nameof(DecryptedText));
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Nie udało się odszyfrować: {DecryptedText}");
                MessageBox.Show(ex.Message);
            }
            
        }

        public void Encrypt()
        {
            if (string.IsNullOrEmpty(DecryptedText))
            {
                MessageBox.Show("Wypełnij text to zaszyfrowania");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Podaj hasło");
                return;
            }

            if (Password.Length < 8)
            {
                MessageBox.Show("Za krótkie hasło minimum 8 znaków");
                return;
            }

            try
            {
                string message = DecryptedText;
                string password = Password;

                byte[] messageBytes = ASCIIEncoding.ASCII.GetBytes(message);
                byte[] passwordBytes = ASCIIEncoding.ASCII.GetBytes(password);

                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                ICryptoTransform transform = provider.CreateEncryptor(passwordBytes, passwordBytes);
                CryptoStreamMode mode = CryptoStreamMode.Write;


                MemoryStream memStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memStream, transform, mode);
                cryptoStream.Write(messageBytes, 0, messageBytes.Length);
                cryptoStream.FlushFinalBlock();

                byte[] encryptedMessageBytes = new byte[memStream.Length];
                memStream.Position = 0;
                memStream.Read(encryptedMessageBytes, 0, encryptedMessageBytes.Length);

                string encryptedMessage = Convert.ToBase64String(encryptedMessageBytes);


                EncryptedText = encryptedMessage;
                OnPropertyChanged(nameof(EncryptedText));
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Nie udało się zaszyfrować: {EncryptedText}");
                MessageBox.Show(ex.Message);
            }

            
        }
    }
}
