using UnityEngine;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Mirror;

public enum DB_MessageCode
{
    Success = 1,
    LoginSuccess,
    LoginFail,
    INSERT_Fall,
    ConnectionFail,
    Fail,
    Duplicate
}

public struct RequestDBMessage
{
    public NetworkConnection _connection;
    public string _userId;
    public string _userPassword;
}

public struct ReceiveDBMessage
{
    public byte _code;
    public string _message;
}

/// <summary>
/// Server
/// </summary>
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

    private static Dictionary<NetworkConnection, string> _userDictionary = new Dictionary<NetworkConnection, string>();

    private const string _dataBaseQuery = "Server=localhost;Port=3306;Database=game_database;Uid=root;Pwd=tlawnsgud~159357";

    private const string _checkId = "SELECT COUNT(*) FROM game_database.player_information WHERE player_id = @player_id;";

    private const string _checkPassword = "SELECT COUNT(*) FROM game_database.player_information WHERE player_id = @player_id AND player_password = @player_password;";

    private const string _insertId = "INSERT INTO game_database.player_information(`player_id`, `player_password`) VALUES (@player_id, @player_password);";

    private const string _parameter_id = "@player_id";

    private const string _parameter_password = "@player_password";

    private const string _parameter_nickName = "@player_nickname";


    [Server]
    public ReceiveDBMessage Login(RequestDBMessage message)
    {
        if (!ConnectionDataBase())
        {
            return CreateReceiveMessage(DB_MessageCode.ConnectionFail);
        }

        return LoginCheck(message);
    }

    [Server]
    public ReceiveDBMessage DuplicateCheck(RequestDBMessage message)
    {
        if (!ConnectionDataBase())
        {
            return CreateReceiveMessage(DB_MessageCode.ConnectionFail);
        }

        return Duplicate(message);
    }

    [Server]
    public ReceiveDBMessage INSERT_ID(RequestDBMessage message)
    {
        if (!ConnectionDataBase())
        {
            return CreateReceiveMessage(DB_MessageCode.ConnectionFail);
        }

        return OnInsertRequest(message);
    }

    [Server]
    public bool UpdateNickName(NetworkConnection connection, string nickName)
    {
        var userid = _userDictionary[connection];

        string query = $"UPDATE game_database.player_information SET `player_nickname` = {_parameter_nickName} WHERE `player_id` = {_parameter_id};";

        try
        {
            using (MySqlCommand mySqlCommand = new MySqlCommand(query, _connection))
            {
                mySqlCommand.Parameters.AddWithValue(_parameter_nickName, nickName);

                mySqlCommand.Parameters.AddWithValue(_parameter_id, userid);

                _connection.Open();

                mySqlCommand.ExecuteNonQuery();
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return false;
        }
    }

    [Server]
    public void RemoveConnectUser(NetworkConnection connection)
    {
        if (_userDictionary.ContainsKey(connection))
        {
            _userDictionary.Remove(connection);
        }
    }

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

            var key = message._connection;

            var value = message._userId;

            if (key != null && !_userDictionary.ContainsKey(key))
            {
                _userDictionary.Add(key, value);
            }

            return CreateReceiveMessage(DB_MessageCode.Success);
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
                return CreateReceiveMessage(DB_MessageCode.Duplicate);
            }

            return CreateReceiveMessage(DB_MessageCode.Success);
        }
        catch (Exception ex)
        {
            return CreateReceiveMessage(DB_MessageCode.Fail, ex.Message);
        }
    }

    

    private bool ConnectionDataBase()
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

            return true;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return false;
        }
    }

    private bool CheckUserPassword(RequestDBMessage message)
    {
        try
        {
            using (MySqlCommand mySqlCommand = new MySqlCommand(_checkPassword, _connection))
            {
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
            _code = (byte)dB_MessageCode,
            _message = message
        };

        return receiveDBMessage;
    }
}


