using System;
using System.Text;
using System.Security.Cryptography;

public class PasswordHash{

    public static (string hash, string salt) hashPassword(string password) {


            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);


            using (var sha256 = SHA256.Create()) {

                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] combineBytes = new byte[salt.Length + passwordBytes.Length];

                Buffer.BlockCopy(salt, 0, combineBytes, 0, salt.Length);
                Buffer.BlockCopy(passwordBytes, 0, combineBytes, salt.Length, passwordBytes.Length);
                
                byte[] hash = sha256.ComputeHash(combineBytes);

                Console.WriteLine($"Hash as hex?: {BitConverter.ToString(hash).Replace("-", "").ToLower()}");

                return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));

            }

    }

    public static bool CheckSum(string enteredPassword, string storedHash, string storedSalt) {

        using (var sha256 = SHA256.Create()) {

            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(enteredPassword);
            byte[] combineBytes = new byte[saltBytes.Length + passwordBytes.Length];

            Buffer.BlockCopy(saltBytes, 0, combineBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, combineBytes, saltBytes.Length, passwordBytes.Length);

            byte[] hash = sha256.ComputeHash(combineBytes);
            return Convert.ToBase64String(hash) == storedHash;

        }

    }

    public static void Main() {
        Console.WriteLine("Testing this shit again ig\n");

        string ogPassword = "yur";

        string testPassword = "nur";

        var (hash, salt) = hashPassword(ogPassword);
        Console.WriteLine($"Original Password {ogPassword}");
        Console.WriteLine($"Test Password {testPassword}");

        bool verifyPass = CheckSum(testPassword, hash, salt);

        Console.WriteLine(verifyPass ? "All Gucci" : "Ah hell nah");


    }


}
