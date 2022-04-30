using System.Collections.Generic;
using PKU.DataModels;
using PKU.DBContext;
using MySql.Data.MySqlClient;
using System;
using UnityEngine;
using System.Linq;

namespace PKU.Services
{

    //Not implemented yet
    public class PlayerService : IPlayerService
    {
        /// <summary>
        /// �����ã�Ϊ�˰�ȫ���Ժ���Ҫ��
        /// </summary>
        private const string connectionString = "server=localhost;port=3306;database=PKUVirtualSpace;uid=root;password=114555";
        public PlayerService()
        {

        }

        public PlayerData GetPlayerDataById(int ID)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (PlayerDBContext contextDB = new PlayerDBContext(connection, false))
                {
                    contextDB.Database.CreateIfNotExists();
                }

                connection.Open();
                try
                {
                    PlayerData playerData = null;
                    using (PlayerDBContext context = new PlayerDBContext(connection, false))
                    {
                        return context.Players.Find(ID);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.StackTrace);
                    Debug.Log(e.Message);
                    Debug.Log("Got exception, roll back");
                    return null;
                }
            }
        }

        public PlayerData GetPlayerDataByEmail(string email)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (PlayerDBContext contextDB = new PlayerDBContext(connection, false))
                {
                    contextDB.Database.CreateIfNotExists();
                }

                connection.Open();
                try
                {
                    using (PlayerDBContext context = new PlayerDBContext(connection, false))
                    {
                        var playerDataList = context.Players.Where(p => p.Email.Equals(email));
                        var count = playerDataList.Count();
                        if (count > 1)
                        {
                            Debug.Log($"Got duplicated email '{email}'");
                            //for safety, do not return the query result
                            return null;
                        }
                        else if(count == 0)
                        {
                            return null;
                        }
                        return playerDataList.First();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Got exception");
                    return null;
                }
            }
        }

        public bool RegisterPlayer(PlayerData playerData)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (PlayerDBContext contextDB = new PlayerDBContext(connection, false))
                {
                    contextDB.Database.CreateIfNotExists();
                }

                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    using (PlayerDBContext context = new PlayerDBContext(connection, false))
                    {
                        context.Database.UseTransaction(transaction);
                        context.Players.Add(playerData);
                        context.SaveChanges();
                    }
                    transaction.Commit();
                }
                catch(Exception e)
                {
                    Debug.Log(e.StackTrace);
                    Debug.Log(e.Message);
                    Debug.Log("Got exception, roll back");
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// �����ڲ���
    /// </summary>
    public class FakePlayerService : IPlayerService
    {
        private List<PlayerData> playerDataList = new List<PlayerData>();
        public FakePlayerService()
        {
        }

        public PlayerData GetPlayerDataById(int ID)
        {
            return playerDataList.Find(p => p.Id == ID);
        }


        public PlayerData GetPlayerDataByEmail(string email)
        {
            return playerDataList.Find(p => p.Email.Equals(email));
        }

        public bool RegisterPlayer(PlayerData playerData)
        {
            playerDataList.Add(playerData);
            return true;
        }
    }

    public interface IPlayerService
    {
        PlayerData GetPlayerDataById(int ID);

        PlayerData GetPlayerDataByEmail(string email);

        /// <summary>
        /// ע�����û�
        /// </summary>
        /// <param name="playerData">���û����ݣ�Id���Ժ���</param>
        /// <returns>�Ƿ�ע��ɹ�</returns>
        bool RegisterPlayer(PlayerData playerData);
    }
}

