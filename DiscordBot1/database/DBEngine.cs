﻿using Npgsql;
using System;
using System.IO;
using Newtonsoft.Json;
using DiscordBot1.config;
using DotNetEnv;

namespace DiscordBot1.database
{
    public class DBEngine
    {

        string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        public bool isLevelledUp = false;

        public async Task<bool> StoreUserAsync(DUser user)
        {
            var totalUsers = await GetTotalUsersAsync();
            if (totalUsers.Item1 != true)
            {
                throw new Exception();
            }
            else
            {
                totalUsers.Item2++;
            }

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "INSERT INTO data.userinfo (userno, username, serverid, avatarurl, level, xp, xplimit)" +
                                   $"VALUES ('{totalUsers.Item2}', '{user.UserName}', '{user.GuildID}', '{user.AvatarURL}', '{user.Level}', '{user.XP}', '{user.XPLimit}')";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<bool> CheckUserExistsAsync(string username, ulong serverID)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = $"SELECT EXISTS (SELECT 1 FROM data.userinfo WHERE username = '{username}' AND serverid = {serverID} LIMIT 1);";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        bool doesExist = (bool)await cmd.ExecuteScalarAsync();

                        if (doesExist == true)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<(bool, DUser)> GetUserAsync(string username, ulong serverID)
        {
            DUser result;

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT u.username, u.serverid, u.avatarurl, u.level, u.xp, u.xplimit " +
                                   "FROM data.userinfo u " +
                                   $"WHERE username = '{username}' AND serverid = {serverID}";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var reader = await cmd.ExecuteReaderAsync();
                        await reader.ReadAsync();


                        result = new DUser
                        {
                            UserName = reader.GetString(0),
                            GuildID = (ulong)reader.GetInt64(1),
                            AvatarURL = reader.GetString(2),
                            Level = reader.GetInt32(3),
                            XP = reader.GetInt32(4),
                            XPLimit = reader.GetInt32(5),
                        };
                    }
                }

                return (true, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (false, null);
            }
        }

        public async Task<bool> AddXPAsync(string username, ulong serverID)
        {
            var XPAmounts = await DetermineXPAsync(username, serverID);

            
            double xpAmount = XPAmounts.Item1;
            int xpLimit = XPAmounts.Item2;

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    // Parameterized query to prevent SQL injection and properly handle data types
                    string query = "UPDATE data.userinfo " +
                                   "SET xp = xp + @xpAmount, xplimit = @xpLimit " +
                                   "WHERE username = @username AND serverid = @serverID";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        // Cast the serverID (UInt64/ulong) to Int64 (long) before adding to the command
                        cmd.Parameters.AddWithValue("xpAmount", xpAmount);
                        cmd.Parameters.AddWithValue("xpLimit", xpLimit);
                        cmd.Parameters.AddWithValue("username", username);
                        cmd.Parameters.AddWithValue("serverID", (long)serverID); // Cast serverID to long

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<bool> LevelUpAsync(string username, ulong serverID)
        {
            isLevelledUp = false;
            var XPAmounts = await DetermineXPAsync(username, serverID);

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "UPDATE data.userinfo " +
                                   $"SET level = level + 1, xp = 0, xplimit = {XPAmounts.Item2} " +
                                   $"WHERE username = '{username}' AND serverid = {serverID}";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                isLevelledUp = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        private async Task<(bool, long)> GetTotalUsersAsync()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT COUNT (*) FROM data.userinfo";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var userCount = await cmd.ExecuteScalarAsync();
                        return (true, Convert.ToInt64(userCount));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (false, -1);
            }
        }

        private async Task<(double, int)> DetermineXPAsync(string username, ulong serverID)
        {
            var user = await GetUserAsync(username, serverID);

            int level = user.Item2.Level;


            const int BaseXP = 100;
            const int XPIncrement = 50;
            const double BaseXPPerMessage = 10.0;
            const double XPDecayFactor = 0.1;


            double xpPerMessage = BaseXPPerMessage / (1 + level * XPDecayFactor);
            if (xpPerMessage < 1.0) xpPerMessage = 1.0; // Ensure XP gain never drops below 1


            int xpThreshold = BaseXP + (level * XPIncrement);

            return (xpPerMessage, xpThreshold);
        }

    }
}
