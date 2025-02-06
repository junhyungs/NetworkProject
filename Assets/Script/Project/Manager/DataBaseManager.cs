using UnityEngine;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Mirror;

public class DataBaseManager
{
    private static MySqlConnection _connection;
    private static DataBaseManager instance;

    public static DataBaseManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new DataBaseManager();
            }

            return instance;
        }
    }

    private const string _dataBaseQuery = "Server=localhost;Port=3306;Database=game_database;Uid=root;Pwd=tlawnsgud~159357";

    private const string _checkId = "SELECT COUNT(*) FROM game_database.player_information WHERE player_id = @player_id;";

    private const string _checkPassword = "SELECT COUNT(*) FROM game_database.player_information WHERE player_id = @player_id AND player_password = @player_password;";

    private const string _insertId = "INSERT INTO game_database.player_information(`player_id`, `player_password`) VALUES (@player_id, @player_password);";

    private const string _parameter_id = "@player_id";

    private const string _parameter_password = "@player_password";

    private const string _parameter_nickName = "@player_nickname";

    public ReceiveDBMessage ConnectionDataBase()
    {
        try
        {
            if (_connection == null)
            {
                using (MySqlConnection connection = new MySqlConnection(_dataBaseQuery))
                {
                    _connection = connection;

                    connection.Open();
                }
            }

            return CreateReceiveMessage(DB_MessageCode.ConnectionSuccess);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return CreateReceiveMessage(DB_MessageCode.ConnectionFail);
        }
    }

   
    public ReceiveDBMessage Login(RequestDBMessage message)
    {
        return LoginCheck(message);
    }

    
    public ReceiveDBMessage DuplicateCheck(RequestDBMessage message)
    {
        return Duplicate(message);
    }

  
    public ReceiveDBMessage INSERT_ID(RequestDBMessage message)
    {
        return OnInsertRequest(message);
    }

    //[Server]
    //public void UpdateNickName(NetworkConnection connection, string nickName)
    //{
    //    string query = $"UPDATE game_database.player_information SET `player_nickname` = {_parameter_nickName} WHERE `player_id` = {_parameter_id};";

    //    var nickName = (string)connection.authenticationData;

    //    try
    //    {
    //        using (MySqlCommand mySqlCommand = new MySqlCommand(query, _connection))
    //        {
    //            mySqlCommand.Parameters.AddWithValue(_parameter_nickName, nickName);

    //            mySqlCommand.Parameters.AddWithValue(_parameter_id, userid);

    //            _connection.Open();

    //            mySqlCommand.ExecuteNonQuery();
    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.Log(ex.Message);
    //    }
    //}

    private ReceiveDBMessage LoginCheck(RequestDBMessage message)
    {
        try
        {
            bool isUserId = CheckUserID(message);

            bool isUserPassword = CheckUserPassword(message);

            if(isUserId && isUserPassword)
            {
                return CreateReceiveMessage(DB_MessageCode.LoginSuccess);
            }

            return CreateReceiveMessage(DB_MessageCode.LoginFail);
        }
        catch(Exception ex)
        {
            return CreateReceiveMessage(DB_MessageCode.Fail, ex.Message);
        }
    }

    private ReceiveDBMessage OnInsertRequest(RequestDBMessage message)
    {
        try
        {
            using(MySqlCommand mySqlCommand = new MySqlCommand(_insertId, _connection))
            {
                mySqlCommand.Parameters.AddWithValue(_parameter_id, message._userId);

                mySqlCommand.Parameters.AddWithValue(_parameter_password, message._userPassword);

                _connection.Open();

                mySqlCommand.ExecuteNonQuery();
            }      

            return CreateReceiveMessage(DB_MessageCode.INSERT_Success);
        }
        catch(Exception ex)
        {
            return CreateReceiveMessage(DB_MessageCode.INSERT_Fall, ex.Message);
        }
        finally
        {
            _connection.Close();
        }
    }

    private ReceiveDBMessage Duplicate(RequestDBMessage message)
    {
        try
        {
            if (CheckUserID(message))
            {
                return CreateReceiveMessage(DB_MessageCode.DuplicateFall);
            }

            return CreateReceiveMessage(DB_MessageCode.DuplicateSuccess);
        }
        catch (Exception ex)
        {
            return CreateReceiveMessage(DB_MessageCode.Fail, ex.Message);
        }
    }

    private bool CheckUserPassword(RequestDBMessage message)
    {
        
        try
        {
            using (MySqlCommand mySqlCommand = new MySqlCommand(_checkPassword, _connection))
            {
                
                mySqlCommand.Parameters.AddWithValue(_parameter_id, message._userId);
                mySqlCommand.Parameters.AddWithValue(_parameter_password, message._userPassword);

                _connection.Open();
        
                int result = Convert.ToInt32(mySqlCommand.ExecuteScalar());
        
                return result > 0;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return false;
        }
        finally
        {
            _connection.Close();
        }
    }

    private bool CheckUserID(RequestDBMessage message)
    {
        try
        {
            using (MySqlCommand mySqlCommand = new MySqlCommand(_checkId, _connection))
            {
                mySqlCommand.Parameters.AddWithValue(_parameter_id, message._userId);

                _connection.Open();

                int result = Convert.ToInt32(mySqlCommand.ExecuteScalar());

                return result > 0;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return false;
        }
        finally
        {
            _connection.Close();
        }
    }

    private ReceiveDBMessage CreateReceiveMessage(DB_MessageCode dB_MessageCode, string message = null)
    {
        ReceiveDBMessage receiveDBMessage = new ReceiveDBMessage()
        {
            _code = dB_MessageCode,
            _message = message
        };

        return receiveDBMessage;
    }
}


