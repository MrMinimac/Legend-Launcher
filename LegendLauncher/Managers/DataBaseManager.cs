using LegendLauncher.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Threading.Tasks;
using System.Windows;


namespace LegendLauncher.Managers
{
    public class DataBaseManager
    {
        private static string ConnectionString => GetConnectionString();

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        }

        #region Formating UUID
        private static string FormatUUID(string inputUuid)
        {
            if (inputUuid != null && inputUuid.Length == 32)
            {
                // Добавляем дефисы в нужные позиции
                string formattedUuid = $"{inputUuid.Substring(0, 8)}-{inputUuid.Substring(8, 4)}-{inputUuid.Substring(12, 4)}-{inputUuid.Substring(16, 4)}-{inputUuid.Substring(20)}";
                return formattedUuid;
            }
            else
            {
                // Возвращаем исходное значение, если оно не соответствует ожидаемой длине
                return inputUuid;
            }
        }
        #endregion

        #region Check Database Connection
        public static bool CheckDatabaseConnectionAsync()
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(ConnectionString);
                connection.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region Get UUID
        public static string GetUUID(string username)
        {
            string uuid = string.Empty;

            try
            {
                using MySqlConnection connection = new MySqlConnection(ConnectionString);
                connection.Open();

                using MySqlCommand selectCommand = new MySqlCommand("SELECT uuid FROM luckperms_players WHERE username = @username", connection);
                selectCommand.Parameters.AddWithValue("@username", username);

                using MySqlDataReader reader = selectCommand.ExecuteReader();
                if (reader.Read())
                {
                    uuid = reader.GetString("uuid");
                    Console.WriteLine("UUID: " + uuid);
                }
            }
            catch // (Exception ex)
            {

                // MessageBox.Show(ex.Message);
            }

            return uuid;
        }
        #endregion

        #region Get Statistics
        public static int GetStats(string uuid, string StatName)
        {
            int totalWorldTime = 0;

            try
            {
                using MySqlConnection connection = new MySqlConnection(ConnectionString);
                connection.Open();

                using MySqlCommand selectCommand = new MySqlCommand($"SELECT {StatName} FROM stats WHERE uuid = @uuid ", connection);
                selectCommand.Parameters.AddWithValue("@uuid", FormatUUID(uuid));

                using MySqlDataReader reader = selectCommand.ExecuteReader();
                if (reader.Read())
                {
                    totalWorldTime = reader.GetInt32($"{StatName}");
                }
            }
            catch // (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }

            return totalWorldTime;
        }
        #endregion

        #region Get Balance
        public static string GetBalance(string uuid)
        {
            int balance = 0;

            try
            {
                using MySqlConnection connection = new MySqlConnection(ConnectionString);
                connection.Open();

                using MySqlCommand selectCommand = new MySqlCommand("SELECT money FROM eco_accounts WHERE player_uuid = @uuid", connection);
                selectCommand.Parameters.AddWithValue("@uuid", FormatUUID(uuid));

                using MySqlDataReader reader = selectCommand.ExecuteReader();
                if (reader.Read())
                {
                    // Если запись найдена, считываем значение из колонки 'val'
                    balance = reader.GetInt32("money");
                }
            }
            catch // (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }

            return balance.ToString();
        }
        #endregion

        #region Get Avatar
        public static string GetAvatar(string uuid)
        {
            string skin = null;

            try
            {
                using MySqlConnection connection = new MySqlConnection(ConnectionString);
                connection.Open();

                using MySqlCommand selectCommand = new MySqlCommand($"SELECT skin_identifier FROM sr_players WHERE uuid = @uuid ", connection);
                selectCommand.Parameters.AddWithValue("@uuid", FormatUUID(uuid));

                using MySqlDataReader reader = selectCommand.ExecuteReader();
                if (reader.Read())
                {
                    skin = reader.GetString("skin_identifier");
                }
            }
            catch //(Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }

            return skin;
        }
        #endregion

        #region Get Players List
        public static async Task<List<Player>> GetAllPlayers()
        {
            List<Player> players = new List<Player>();

            try
            {
                using MySqlConnection connection = new MySqlConnection(ConnectionString);
                await connection.OpenAsync();

                // Выбираем данные из обеих таблиц и объединяем их по player_id
                string query = "SELECT eco_accounts.player_uuid, eco_accounts.player_name, eco_accounts.money, " +
                               "stats.is_online, stats.PLAY_ONE_TICK, sr_players.skin_identifier FROM eco_accounts " +
                               "LEFT JOIN stats ON eco_accounts.player_uuid = stats.uuid " +
                               "LEFT JOIN sr_players ON eco_accounts.player_uuid = sr_players.uuid  ";

                using MySqlCommand selectCommand = new MySqlCommand(query, connection);
                using DbDataReader reader = await selectCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    string playerName = reader["player_name"].ToString();
                    string balance = reader["money"].ToString();
                    int isOnline = Convert.ToInt32(reader["is_online"]);
                    string SkinID = reader["skin_identifier"].ToString();
                    int PlayOneTock = Convert.ToInt32(reader["PLAY_ONE_TICK"]);
                    string _isOnline = "DarkRed";
                    string playTime = $"{PlayOneTock / 1200 / 60} ч. {PlayOneTock / 1200 % 60} м.";


                    if (isOnline == 1)
                    {
                        _isOnline = "Green";
                    }

                    Player player = new Player
                    {
                        Name = playerName,
                        Balance = $"{balance}$",
                        IsOnline = _isOnline,
                        Avatar = $"https://mc-heads.net/avatar/{SkinID}",
                        PlayTime = playTime,
                    };

                    players.Add(player);
                }
            }
            catch // (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }

            return players;
        }
        #endregion

        #region Get Skins UUID
        public static async Task<List<Skins>> GetSkinsUUID()
        {
            List<Skins> uuid = new List<Skins>();

            try
            {
                using MySqlConnection connection = new MySqlConnection(ConnectionString);
                await connection.OpenAsync();

                string query = "SELECT name, uuid FROM sr_cache";
                using MySqlCommand selectCommand = new MySqlCommand(query, connection);
                using DbDataReader reader = await selectCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    string _name = reader["name"].ToString();
                    string _uuid = reader["uuid"].ToString();
                    Skins skin = new Skins
                    {
                        Name = _name,
                        Uuid = _uuid
                    };
                    uuid.Add(skin);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return uuid;
        }
        #endregion

        #region Set Avatar
        public static void SetAvatar(string uuid, string newSkin)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(ConnectionString);
                connection.Open();

                using MySqlCommand updateCommand = new MySqlCommand($"UPDATE sr_players SET skin_identifier = @newSkin WHERE uuid = @uuid", connection);
                updateCommand.Parameters.AddWithValue("@uuid", FormatUUID(uuid));
                updateCommand.Parameters.AddWithValue("@newSkin", newSkin);

                updateCommand.ExecuteNonQuery();
            }
            catch // (Exception ex)
            {
                // Обработка исключения, например, логирование ошибки
                // MessageBox.Show(ex.Message);
            }
        }
        #endregion

    }
}
