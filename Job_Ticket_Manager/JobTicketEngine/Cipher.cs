/// <summary>
///  Cipher.cs
///  Job Ticket Manager Project
///  Creator: John D. Sbur
/// </summary>
/// 
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Job_Ticket_Manager")]

namespace JobTicketEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    ///  Contains static methods for encrpything and decrypting strings using a transposition cipher
    /// </summary>
    internal static class Cipher
    {
        /// <summary>
        ///  Uses attributes of itself as a key and SHA-256 to transpose itself into an encrypted string
        /// </summary>
        /// <param name="rawString"></param>
        /// <returns>
        ///     Encrypted string or unencrypted if length of string is less than 3.
        /// </returns>
        public static string TranspositionEncrypt(string rawString)
        {
            /// If length of string is less than 3, return with no work done to it.
            if (rawString.Length < 3)
            {
                return rawString;
            }

            // Retrieve key
            ulong key = Cipher.RetrieveTranspositionKey(rawString);

            // Create a character array of size equal to string's length
            char[] encryptedStringArray = new char[rawString.Length];
            // Create a character array from rawString
            char[] rawStringArray = rawString.ToCharArray();

            // Fix key to be within size of string length
            key = key % (ulong)rawString.Length;
            if (key < 2)
            {
                key = 2;
            }

            // Transpose rawString
            bool finished = false;
            ulong currentRawStringIndex = 0;
            ulong currentEncryptedStringIndex = 0;
            int valuesWritten = 0;
            while (!finished)
            {
                // Find next index to take from inside the rawString. Validate.
                currentRawStringIndex += key;
                do
                {
                    // Wrap index around.
                    if (currentRawStringIndex >= (ulong)encryptedStringArray.Length)
                    {
                        currentRawStringIndex %= (ulong)rawString.Length;
                    }

                    // If we have a blank space on the current index, move to the next one.
                    if (rawStringArray[currentRawStringIndex] == '\0')
                    {
                        currentRawStringIndex++;
                    }

                    // Check for wrap around twice for security.
                    if (currentRawStringIndex >= (ulong)encryptedStringArray.Length)
                    {
                        currentRawStringIndex %= (ulong)rawString.Length;
                    }

                    // If current index is null, go to next one
                } while (rawStringArray[currentRawStringIndex] == '\0');

                // Once we have our valid index, add it to the encrypted array and mark the rawString array at the found index as '\0'. Marks it as the equivalent of empty.
                // Increment values written.
                encryptedStringArray[currentEncryptedStringIndex] = rawStringArray[currentRawStringIndex];
                currentEncryptedStringIndex++;
                rawStringArray[currentRawStringIndex] = '\0';
                valuesWritten++;

                // Once values written becomes equal to length of rawString, we are finished writing
                if (valuesWritten == rawString.Length)
                {
                    finished = true;
                }
            }

            return new string(encryptedStringArray);
        }

        /// <summary>
        ///  Uses attributes of itself as a key and SHA-256 to reverse transpose itself into a decrypted string
        /// </summary>
        /// <param name="rawString"></param>
        /// <returns>
        ///     Decrypted string or itself if length of string is less than 3.
        /// </returns>
        public static string TranspositionDecrypt(string encryptedString)
        {
            /// If length of string is less than 3, return with no work done to it.
            if (encryptedString.Length < 3)
            {
                return encryptedString;
            }

            // Retrieve key
            ulong key = Cipher.RetrieveTranspositionKey(encryptedString);

            // Create a character array of size equal to string's length
            char[] encryptedStringArray = encryptedString.ToCharArray(); 
            // Create a character array from rawString
            char[] rawStringArray = new char[encryptedString.Length];

            // Fix key to be within size of string length
            key = key % (ulong)encryptedString.Length;
            if (key < 2)
            {
                key = 2;
            }

            // Transpose rawString
            bool finished = false;
            ulong currentTranslatedStringIndex = 0;
            ulong currentEncryptedStringIndex = 0;
            int valuesWritten = 0;
            while (!finished)
            {
                // Find next index to take from inside the rawString. Validate.
                currentTranslatedStringIndex += key;
                do
                {
                    // Wrap index around.
                    if (currentTranslatedStringIndex >= (ulong)encryptedStringArray.Length)
                    {
                        currentTranslatedStringIndex %= (ulong)encryptedStringArray.Length;
                    }

                    // If we have a blank space on the current index, move to the next one.
                    if (rawStringArray[currentTranslatedStringIndex] != '\0')
                    {
                        currentTranslatedStringIndex++;
                    }

                    // Check for wrap around twice for security.
                    if (currentTranslatedStringIndex >= (ulong)encryptedStringArray.Length)
                    {
                        currentTranslatedStringIndex %= (ulong)encryptedStringArray.Length;
                    }

                // If current index is null, go to next one
                } while (rawStringArray[currentTranslatedStringIndex] != '\0');

                // Once we have our valid index, add it to the encrypted array and mark the rawString array at the found index as '\0'. Marks it as the equivalent of empty.
                // Increment values written.
                rawStringArray[currentTranslatedStringIndex] = encryptedStringArray[currentEncryptedStringIndex];
                encryptedStringArray[currentEncryptedStringIndex] = '\0';
                currentEncryptedStringIndex++;
                valuesWritten++;

                // Once values written becomes equal to length of rawString, we are finished writing
                if (valuesWritten == encryptedString.Length)
                {
                    finished = true;
                }
            }
            return new string(rawStringArray);
        }

        /// <summary>
        ///  Based on an inputted string, this function grabs the key associated with the string.
        /// </summary>
        /// <param name="keyString"></param>
        /// <returns>
        ///     Unsigned integer representing the key.
        /// </returns>
        private static ulong RetrieveTranspositionKey(string keyString)
        {
            // Holder to be returned later
            ulong holder = 0;

            // Send string through SHA-256 hash
            using (SHA256 sha256 = SHA256.Create())
            {
                // Add up each value in converted array.
                foreach (byte value in keyString.Select(c => (byte)c).ToArray())
                {
                    holder += value;
                }

                // Turn holder into a byte array
                byte[] holderArray = sha256.ComputeHash(BitConverter.GetBytes(holder));

                holder = 0;
                // Add up each value in converted array.
                foreach (byte value in holderArray)
                {
                    holder += value;
                }

                // Return when finished
                return holder;
            }
        }
    }
}
