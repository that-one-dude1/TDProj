using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics;

namespace TDProj
{
    public class TowerData
    {
        public float PosX { get; set; }
        public float PosY { get; set; }

        public TowerType Type { get; set; }

        public int CellX { get; set; }
        public int CellY { get; set; }

        public int Level { get; set; }
        public int UpgradeTotal { get; set; }
    }

    public class SaveData
    {
        public int CurrentWaveIndex { get; set; }
        public int PlayerLives { get; set; }
        public int PlayerMoney { get; set; }
        public List<TowerData> Towers { get; set; }
        public List<Point> OccupiedCells { get; set; }
    }

    public static class AesStatic
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); //32 chars
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567890123456"); //16 chars

        public static string Encrypt(string plainText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using StreamWriter sw = new(cs);

            sw.Write(plainText);

            sw.Flush();
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            using MemoryStream ms = new(Convert.FromBase64String(cipherText));
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader sr = new(cs);

            return sr.ReadToEnd();
        }
    }

    public class SaveManager
    {
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public SaveData CreateSave(int currentWave, int playerLives, int playerMoney, List<Tower> Towers, HashSet<Point> OccupiedCells)
        {
            return new SaveData
            {
                CurrentWaveIndex = currentWave,
                PlayerLives = playerLives,
                PlayerMoney = playerMoney,
                Towers = Towers.Select(t => t.ToData()).ToList(),
                OccupiedCells = OccupiedCells.ToList()
            };
        }

        public static SaveData LoadFile()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                Title = "Load Game"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string encrypted = File.ReadAllText(dialog.FileName);
                string json = AesStatic.Decrypt(encrypted);
                return JsonSerializer.Deserialize<SaveData>(json);
            }

            

            return null;
        }

        public void SaveFile(int currentWave, int playerLives, int playerMoney, List<Tower> Towers, HashSet<Point> OccupiedCells)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                Title = "Save Game As"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string json = JsonSerializer.Serialize(CreateSave(currentWave, playerLives, playerMoney, Towers, OccupiedCells), jsonOptions);
                string encrypted = AesStatic.Encrypt(json);
                File.WriteAllText(dialog.FileName, encrypted);
            }
        }
    }
}
