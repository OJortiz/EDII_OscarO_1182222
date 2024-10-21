using System;
using System.Text;				

 public class DES
 {

    // Table to drop parity bits from the key
    private static readonly int[] ParityDropTable =
     [
         57, 49, 41, 33, 25, 17, 9,
         1, 58, 50, 42, 34, 26, 18,
         10, 2, 59, 51, 43, 35, 27,
         19, 11, 3, 60, 52, 44, 36,

         63, 55, 47, 39, 31, 23, 15,
         7, 62, 54, 46, 38, 30, 22,
         14, 6, 61, 53, 45, 37, 29,
         21, 13, 5, 28, 20, 12, 4
     ];

     // Table to define left shifts for key shifts
     private static readonly int[] ShiftTable =
     [
         1, 1, 2, 2, 2, 2, 2, 2,
         1, 2, 2, 2, 2, 2, 2, 1
     ];

     // Table to compress the generated key
     private static readonly int[] KeyCompressionTable =
     [
         14, 17, 11, 24, 1, 5,
         3, 28, 15, 6, 21, 10,
         23, 19, 12, 4, 26, 8,
         16, 7, 27, 20, 13, 2,
         41, 52, 31, 37, 47, 55,
         30, 40, 51, 45, 33, 48,
         44, 49, 39, 56, 34, 53,
         46, 42, 50, 36, 29, 32
     ];

     // Initial permutation table
     private static readonly int[] InitialPermutationTable = [
         58, 50, 42, 34, 26, 18, 10, 2,
         60, 52, 44, 36, 28, 20, 12, 4,
         62, 54, 46, 38, 30, 22, 14, 6,
         64, 56, 48, 40, 32, 24, 16, 8,
         57, 49, 41, 33, 25, 17, 9, 1,
         59, 51, 43, 35, 27, 19, 11, 3,
         61, 53, 45, 37, 29, 21, 13, 5,
         63, 55, 47, 39, 31, 23, 15, 7
     ];

     // Expansion permutation table used in each round
     private static readonly int[] ExpansionPermutationTable = [
          32, 1, 2, 3, 4, 5,
          4, 5, 6, 7, 8, 9,
          8, 9, 10, 11, 12, 13,
          12, 13, 14, 15, 16, 17,
          16, 17, 18, 19, 20, 21,
          20, 21, 22, 23, 24, 25,
          24, 25, 26, 27, 28, 29,
          28, 29, 30, 31, 32, 1
     ];

     // S-Box table for substitution
     private static readonly int[][,] SBoxTable =
      [
         new int[,] {
             { 14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 },
             { 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 },
             { 4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 },
             { 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 }
         },
         new int[,] {
             { 15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10 },
             { 3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5 },
             { 0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15 },
             { 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 }
         },
         new int[,] {
             { 10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8 },
             { 13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1 },
             { 13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7 },
             { 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12 }
         },
         new int[,] {
             { 7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 },
             { 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9 },
             { 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 },
             { 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14 }
         },
         new int[,] {
             { 2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9 },
             { 14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6 },
             { 4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14 },
             { 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3 }
         },
         new int[,] {
             { 12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11 },
             { 10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8 },
             { 9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6 },
             { 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 }
         },
         new int[,] {
             { 4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1 },
             { 13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6 },
             { 1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2 },
             { 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12 }
         },
         new int[,] {
             { 13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7 },
             { 1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2 },
             { 7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8 },
             { 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11 }
         }
      ];

     // P-Box permutation table for the final permutation
     private static readonly int[] PBoxPermutationTable =
     [
         16, 7, 20, 21, 29, 12, 28, 17,
         1, 15, 23, 26, 5, 18, 31, 10,
         2, 8, 24, 14, 32, 27, 3, 9,
         19, 13, 30, 6, 22, 11, 4, 25
     ];

     // Final permutation table to rearrange bits
     private static readonly int[] FinalPermutationTable =
     [
         40, 8, 48, 16, 56, 24, 64, 32,
         39, 7, 47, 15, 55, 23, 63, 31,
         38, 6, 46, 14, 54, 22, 62, 30,
         37, 5, 45, 13, 53, 21, 61, 29,
         36, 4, 44, 12, 52, 20, 60, 28,
         35, 3, 43, 11, 51, 19, 59, 27,
         34, 2, 42, 10, 50, 18, 58, 26,
         33, 1, 41, 9, 49, 17, 57, 25
     ];

     /// <summary>
     /// Processes the input data using the DES algorithm.
     /// </summary>
     /// <param name="inputData">The input data to be processed.</param>
     /// <param name="encryptionKey">The encryption key.</param>
     /// <param name="isAscending">Indicates whether to process in ascending order (encryption) or descending order (decryption).</param>
     /// <returns>The processed data.</returns>
     private static byte[] DESProcess(byte[] inputData, byte[] encryptionKey, bool isAscending)
     {
         //Resultant byte array for processed data
         byte[] processedData = new byte[inputData.Length];
         int blockCount = inputData.Length / 8; //Each block is 8 bytes (64 bits)
         byte[][] roundKeys = GenerateKeys(encryptionKey, isAscending); //Generate round keys
         byte[] blockBuffer = new byte[8]; //Buffer for holding the current block
         byte[] leftHalf = new byte[4]; //Left half of the block
         byte[] rightHalf = new byte[4]; //Right half of the block
         byte[] expandedRightHalf; //Expanded right half after expansion permutation
         byte[] substitutedRightHalf = new byte[4]; //Substituted result
         byte[] tempRightHalf; //Temporary storage for swapping

         //Iterate through each block of the data
         for (int blockNum = 0; blockNum < blockCount; blockNum++)
         {
             // Copy current block and apply initial permutation
             Array.Copy(inputData, blockNum * 8, blockBuffer, 0, 8);
             blockBuffer = Permute(blockBuffer, InitialPermutationTable);

             // 16 rounds of processing
             for (int round = 0; round < 16; round++)
             {
                 // Split block into left and right halves
                 Buffer.BlockCopy(blockBuffer, 0, leftHalf, 0, 4);
                 Buffer.BlockCopy(blockBuffer, 4, rightHalf, 0, 4);

                 // Expand the right half for S-Box processing
                 expandedRightHalf = Permute(rightHalf, ExpansionPermutationTable);

                 // XOR with the round key
                 expandedRightHalf = XOR(expandedRightHalf, roundKeys[round]);

                 // S-Box substitution for each section
                 for (int section = 0; section < 8; section++)
                 {
                     // Calculate row and column for S-Box lookup
                     int row = (GetBitAt(expandedRightHalf, section * 6) << 1) | GetBitAt(expandedRightHalf, section * 6 + 5);
                     int column = 0;
                     for (int bitIndex = 0; bitIndex < 4; bitIndex++)
                     {
                         column |= GetBitAt(expandedRightHalf, section * 6 + bitIndex + 1) << (3 - bitIndex);
                     }

                     // Retrieve value from the S-Box
                     int sBoxValue = SBoxTable[section][row, column];
                     for (int bitIndex = 0; bitIndex < 4; bitIndex++)
                     {
                         SetBitAt(substitutedRightHalf, section * 4 + bitIndex, (sBoxValue >> (3 - bitIndex)) & 1);
                     }
                 }

                 // Permute the substituted result
                 substitutedRightHalf = Permute(substitutedRightHalf, PBoxPermutationTable);

                 // XOR left half with substituted result
                 tempRightHalf = XOR(leftHalf, substitutedRightHalf);

                 // Swap halves for next round, except for the last round
                 if (round != 15)
                 {
                     Buffer.BlockCopy(rightHalf, 0, blockBuffer, 0, 4);
                     Buffer.BlockCopy(tempRightHalf, 0, blockBuffer, 4, 4);
                 }
                 else
                 {
                     Buffer.BlockCopy(tempRightHalf, 0, blockBuffer, 0, 4);
                     Buffer.BlockCopy(rightHalf, 0, blockBuffer, 4, 4);
                 }
             }

             // Apply the final permutation after all rounds
             blockBuffer = Permute(blockBuffer, FinalPermutationTable);
             Buffer.BlockCopy(blockBuffer, 0, processedData, blockNum * 8, 8);
         }

         return processedData;
     }

     /// <summary>
     /// Encrypts the given data using DES encryption.
     /// </summary>
     /// <param name="data">The data to be encrypted.</param>
     /// <param name="key">The 8-byte encryption key.</param>
     /// <param name="addPadding">Indicates whether to add PKCS7 padding.</param>
     /// <returns>The encrypted data.</returns>
     public static byte[] Encrypt(byte[] data, byte[] key, bool addPadding = true)
     {
         if (key.Length != 8)
         {
             throw new ArgumentException("Key length must be 8 bytes");
         }

         if (addPadding)
         {
             data = AddPkcs7Padding(data, 8); // Add padding if necessary
         }

         return DESProcess(data, key, true);  // Process data for encryption
     }

    public static void EncryptFile(string inputFilePath, string outputFilePath, byte[] key)
    {
        // Leer el contenido del archivo como un arreglo de bytes
        byte[] fileBytes = File.ReadAllBytes(inputFilePath);

        // Cifrar los bytes usando DES
        byte[] encryptedBytes = Encrypt(fileBytes, key);

        // Escribir los bytes cifrados en el archivo de salida
        File.WriteAllBytes(outputFilePath, encryptedBytes);
    }

    public static void DecryptFile(string inputFilePath, string outputFilePath, byte[] key)
    {
        byte[] encryptedBytes = File.ReadAllBytes(inputFilePath);

        byte[] decryptedBytes = Decrypt(encryptedBytes, key);

        File.WriteAllBytes(outputFilePath, decryptedBytes);
    }



    /// <summary>
    /// Decrypts the given data using DES decryption.
    /// </summary>
    /// <param name="data">The data to be decrypted.</param>
    /// <param name="key">The 8-byte decryption key.</param>
    /// <param name="removePadding">Indicates whether to remove PKCS7 padding.</param>
    /// <returns>The decrypted data.</returns>
    public static byte[] Decrypt(byte[] data, byte[] key, bool removePadding = true)
     {
         if (key.Length != 8)
         {
             throw new ArgumentException("Key length must be 8 bytes");
         }

         // Ensure data length is a multiple of 8 bytes
         if (data.Length % 8 != 0)
         {
             throw new ArgumentException("Data length must be multiple of 8 bytes");
         }

         // Process data for decryption
         var result = DESProcess(data, key, false);
         if (removePadding)
         {
             result = RemovePkcs7Padding(result); // Remove padding if necessary
         }

         return result; // Return decrypted data
     }

     /// <summary>
     /// Generates the 16 round keys for the DES algorithm.
     /// </summary>
     /// <param name="initialKey">The initial 8-byte key.</param>
     /// <param name="isAscending">Indicates whether to generate keys in ascending order (encryption) or descending order (decryption).</param>
     /// <returns>An array of 16 round keys.</returns>
     private static byte[][] GenerateKeys(byte[] initialKey, bool isAscending = true)
     {
         byte[][] roundKeys = new byte[16][]; // Array to hold round keys
         byte[] permutedKey = Permute(initialKey, ParityDropTable); // Permute initial key to drop parity bits
         for (int round = 0; round < 16; round++)
         {
             // Split key into left and right halves
             byte[] leftHalf = SelectBits(permutedKey, 0, 28);
             byte[] rightHalf = SelectBits(permutedKey, 28, 28);

             // Perform left shifts
             leftHalf = LeftShift(leftHalf, 28, ShiftTable[round]);
             rightHalf = LeftShift(rightHalf, 28, ShiftTable[round]);

             // Combine halves and compress to generate round key
             byte[] combinedKey = JoinKey(leftHalf, rightHalf);
             roundKeys[round] = Permute(combinedKey, KeyCompressionTable);
             permutedKey = combinedKey; // Update permutedKey for next round
         }

         if (!isAscending)
         {
             Array.Reverse(roundKeys); // Reverse keys if not in ascending order (Decryption)
         }

         return roundKeys;
     }

     /// <summary>
     /// Permutes the given data using the specified permutation table.
     /// </summary>
     /// <param name="source">The data to be permuted.</param>
     /// <param name="table">The permutation table.</param>
     /// <returns>The permuted data.</returns>
     private static byte[] Permute(byte[] source, int[] table)
     {
         int length = (table.Length - 1) / 8 + 1; // Calculate result length
         byte[] result = new byte[length];
         for (int i = 0; i < table.Length; i++)
         {
             SetBitAt(result, i, GetBitAt(source, table[i] - 1)); // Set bits according to permutation table
         }
         return result;
     }

     /// <summary>
     /// Left shifts the given data by the specified number of bits.
     /// </summary>
     /// <param name="data">The data to be shifted.</param>
     /// <param name="len">The length of the data in bits.</param>
     /// <param name="shift">The number of bits to shift.</param>
     /// <returns>The shifted data.</returns>
     private static byte[] LeftShift(byte[] data, int len, int shift)
     {
         byte[] outer = new byte[(len - 1) / 8 + 1]; // Create new byte array for shifted bits
         for (int i = 0; i < len; i++)
         {
             int val = GetBitAt(data, (i + shift) % len); // Calculate shifted value
             SetBitAt(outer, i, val);
         }

         return outer; // Return shifted result
     }

     /// <summary>
     /// Performs a bitwise XOR operation on two byte arrays.
     /// </summary>
     /// <param name="first">The first byte array.</param>
     /// <param name="second">The second byte array.</param>
     /// <returns>The result of the XOR operation.</returns>
     private static byte[] XOR(byte[] first, byte[] second)
     {
         byte[] result = new byte[first.Length]; // Resultant byte array
         for (int i = 0; i < first.Length; i++)
         {
             result[i] = (byte)(first[i] ^ second[i]); // Perform XOR operation
         }

         return result;  // Return XOR result
     }

     /// <summary>
     /// Gets the bit at the specified position in the data.
     /// </summary>
     /// <param name="data">The data.</param>
     /// <param name="position">The bit position.</param>
     /// <returns>The bit value (0 or 1).</returns>
     private static int GetBitAt(byte[] data, int position)
     {
         int posByte = position / 8; // Calculate byte position
         int posBit = position % 8; // Calculate bit position
         return data[posByte] >> (7 - posBit) & 1; // Return bit value
     }

     /// <summary>
     /// Sets the bit at the specified position in the data to the given value.
     /// </summary>
     /// <param name="data">The data.</param>
     /// <param name="position">The bit position.</param>
     /// <param name="value">The bit value (0 or 1).</param>
     private static void SetBitAt(byte[] data, int position, int value)
     {
         int posByte = position / 8; // Calculate byte position
         int posBit = position % 8; // Calculate bit position            
         if (value == 1)
             data[posByte] |= (byte)(1 << (7 - posBit)); // Set bit to 1
         else
             data[posByte] &= (byte)~(1 << (7 - posBit)); // Set bit to 0
     }

     /// <summary>
     /// Selects a subset of bits from the data starting at the specified position.
     /// </summary>
     /// <param name="data">The data.</param>
     /// <param name="position">The starting bit position.</param>
     /// <param name="len">The number of bits to select.</param>
     /// <returns>The selected bits as a byte array.</returns>
     private static byte[] SelectBits(byte[] source, int start, int count)
     {
         byte[] result = new byte[(count - 1) / 8 + 1]; // Create result array
         for (int i = 0; i < count; i++)
         {
             SetBitAt(result, i, GetBitAt(source, start + i)); // Set bits in result array
         }

         return result; // Return selected bits
     }

     /// <summary>
     /// Joins two byte arrays into one.
     /// </summary>
     /// <param name="left">The left byte array.</param>
     /// <param name="right">The right byte array.</param>
     /// <returns>The joined byte array.</returns>
     private static byte[] JoinKey(byte[] leftHalf, byte[] rightHalf)
     {
         byte[] result = new byte[7]; // Allocate new byte array for result
         for (int i = 0; i < 3; i++)
         {
             result[i] = leftHalf[i];
         }
         for (int i = 0; i < 4; i++)
         {
             int val = GetBitAt(leftHalf, 24 + i); //24-27
             SetBitAt(result, 24 + i, val);
         }
         for (int i = 0; i < 28; i++)
         {
             int val = GetBitAt(rightHalf, i);
             SetBitAt(result, 28 + i, val);
         }
         return result;
     }

     /// <summary>
     /// Adds PKCS#7 padding to the data to make it a multiple of the block size.
     /// </summary>
     /// <param name="data">The data to be padded.</param>
     /// <param name="blockSize">The block size in bytes.</param>
     /// <returns>The padded data.</returns>
     /// <exception cref="ArgumentNullException">Thrown when the data is null.</exception>
     /// <exception cref="ArgumentException">Thrown when the block size is less than or equal to zero.</exception>
     public static byte[] AddPkcs7Padding(byte[] data, int blockSize)
     {
         if (data == null)
         {
             throw new ArgumentNullException(nameof(data), "Data cannot be null");
         }

         if (blockSize <= 0)
         {
             throw new ArgumentException("Block size must be greater than zero", nameof(blockSize));
         }

         int count = data.Length;
         int paddingRemainder = count % blockSize;
         int paddingSize = blockSize - paddingRemainder;

         if (paddingSize == 0)
         {
             paddingSize = blockSize;
         }

         byte[] paddedData = new byte[data.Length + paddingSize];
         Buffer.BlockCopy(data, 0, paddedData, 0, data.Length);

         byte paddingByte = (byte)paddingSize;
         for (int i = data.Length; i < paddedData.Length; i++)
         {
             paddedData[i] = paddingByte;
         }

         return paddedData;
     }

     /// <summary>
     /// Removes PKCS#7 padding from the padded data.
     /// </summary>
     /// <param name="paddedByteArray">The padded data.</param>
     /// <returns>The data with padding removed.</returns>
     /// <exception cref="ArgumentNullException">Thrown when the padded byte array is null.</exception>
     /// <exception cref="ArgumentException">Thrown when the padded byte array is empty or has invalid padding.</exception>
     public static byte[] RemovePkcs7Padding(byte[] paddedByteArray)
     {
         if (paddedByteArray == null)
         {
             throw new ArgumentNullException(nameof(paddedByteArray), "Padded byte array cannot be null");
         }

         if (paddedByteArray.Length == 0)
         {
             throw new ArgumentException("Padded byte array cannot be empty", nameof(paddedByteArray));
         }

         int paddingSize = paddedByteArray[paddedByteArray.Length - 1];
         if (paddingSize < 1 || paddingSize > paddedByteArray.Length)
         {
             throw new ArgumentException("Invalid padding size.");
         }

         for (int i = paddedByteArray.Length - paddingSize; i < paddedByteArray.Length; i++)
         {
             if (paddedByteArray[i] != paddingSize)
             {
                 throw new ArgumentException("Invalid padding.");
             }
         }

         int resultLength = paddedByteArray.Length - paddingSize;
         byte[] result = new byte[resultLength];
         Buffer.BlockCopy(paddedByteArray, 0, result, 0, resultLength);

         return result;
     }
 }