using System;
using System.Security.Cryptography;
using System.Text;

class MerkleSignature
{
    // Хеширование строки с помощью SHA-256
    public static string Hash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }

    // Строим дерево Меркла
    public static string BuildMerkleTree(string[] leaves)
    {
        string[] layer = new string[leaves.Length];

        // Преобразуем листья в хеши
        for (int i = 0; i < leaves.Length; i++)
        {
            layer[i] = Hash(leaves[i]);
        }

        // Создаем дерево
        while (layer.Length > 1)
        {
            string[] nextLayer = new string[layer.Length / 2];

            for (int i = 0; i < nextLayer.Length; i++)
            {
                if (2 * i + 1 < layer.Length)
                {
                    nextLayer[i] = Hash(layer[2 * i] + layer[2 * i + 1]);
                }
                else
                {
                    nextLayer[i] = layer[2 * i]; // Дублируем последний элемент
                }
            }

            layer = nextLayer;
        }

        // Корень дерева Меркла (открытый ключ)
        return layer[0];
    }

    // Генерация подписи
    public static string GenerateSignature(string message, string[] X, string[] Y, int i)
    {
        string x_i = X[i];  // Приватный ключ
        string y_i = Y[i];  // Публичный ключ

        // Создание хеша сообщения
        string hashMessage = Hash(message);

        // Путь аутентификации
        string auth_1 = Hash(Y[(i + 1) % Y.Length]);  // Следующий публичный ключ
        string auth_2 = Hash(Hash(Hash(Y[(i + 2) % Y.Length]) + Hash(Y[(i + 3) % Y.Length])));  // Пример на 4 ключах

        // Подпись для i-ого ключа
        string signature = Hash(x_i + hashMessage);

        return $"{signature}|{auth_1}|{auth_2}";
    }

    static void Main()
    {
        
        string[] X = { "DA1", "DA2", "DA3", "DA4" }; // приватные ключи
        string[] Y = { "AD1", "AD2", "AD3", "AD4" }; // публичные ключи

        // дерево Меркла и получаем корень
        string root = BuildMerkleTree(Y);
        Console.WriteLine("Root: " + root);

        // Пример сообщения
        string message = "Пример сообщения";

        // Генерация подписи для первого ключа
        string signature = GenerateSignature(message, X, Y, 0);
        Console.WriteLine("Signature: " + signature);

    }
}
