using UnityEngine;
using MySql.Data.MySqlClient;
using System;
using System.Data;

public class TEST_DBManager : MonoBehaviour
{
    private static MySqlConnection _connection;

    private const string _testQuery = "Server=localhost;Port=3306;Database=test_database;Uid=root;Pwd=tlawnsgud~159357";

    private void Start()
    {
        //var dbManager = DataBaseManager.Instance;

        //if(dbManager == null)
        //{
        //    return;
        //}

        //RequestDBMessage requestDBMessage = new RequestDBMessage()
        //{
        //    _userId = "jjjj",
        //    _userPassword = "pppp"
        //};

        //var receiveDBMessage = dbManager.DuplicateCheck(requestDBMessage);

        //if(receiveDBMessage._code == 1)
        //{
        //    Debug.Log("중복검사완료");
        //}
        //else if(receiveDBMessage._code == 6)
        //{
        //    Debug.Log("중복검사실패");
        //}
        //else if(receiveDBMessage._code == 7)
        //{
        //    Debug.Log("중복");
        //}

        //receiveDBMessage = dbManager.INSERT_ID(requestDBMessage);

        //if(receiveDBMessage._code == 1)
        //{
        //    Debug.Log("성공");
        //}
        //else if(receiveDBMessage._code == 4)
        //{
        //    Debug.Log("실패");
        //    Debug.Log(receiveDBMessage._message);
        //}

        //if(dbManager.UpdateNickName(requestDBMessage._userId, "ASDF"))
        //{
        //    Debug.Log("성공");
        //}
        //else
        //{
        //    Debug.Log("실패");
        //}
        


        //if (!TestDBConnection())
        //{
        //    return;
        //}

        //string selectQuery = "SELECT * FROM test_table";

        //var dataSet = OnSelectRequest(selectQuery, "test_table");

        //if(dataSet != null)
        //{
        //    var message = FormatedTable(dataSet);

        //    Debug.Log(message);
        //}

        //string insertQuery = "INSERT INTO test_database.test_table(`product_name`, `cost`, `make_date`, `company`, `amount`) VALUES ('사탕', 10000, '2022-08-09', '빅빅', 10);";

        //var isInsert = OnInsertRequest(insertQuery);

        //Debug.Log(isInsert);
    }
    //INSERT INTO `test_database`.`test_table` (`product_name`, `cost`, `make_date`, `company`, `amount`) VALUES ('김밥', '1000', '2022-03-03', '김밥천국', '10');
    private bool TestDBConnection()
    {
        try
        {
            using(MySqlConnection connection = new MySqlConnection(_testQuery))
            {
                _connection = connection;
                connection.Open();
            }

            Debug.Log("연결성공");
            return true;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log("실패");
            return false;
        }
    }

    private DataSet OnSelectRequest(string query, string tableName)
    {
        try
        {
            _connection.Open();
            MySqlCommand mySqlCommand = new MySqlCommand();
            mySqlCommand.Connection = _connection;
            mySqlCommand.CommandText = query;

            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
            DataSet dataSet = new DataSet();

            mySqlDataAdapter.Fill(dataSet, tableName);

            _connection.Close();
            return dataSet;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log("<OnSelectRequest> Return null");
            return null;
        }
    }

    private bool OnInsertRequest(string query)
    {
        try
        {
            MySqlCommand mySqlCommand = new MySqlCommand();
            mySqlCommand.Connection = _connection;
            mySqlCommand.CommandText = query;

            _connection.Open();
            mySqlCommand.ExecuteNonQuery();
            _connection.Close();

            return true;    
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log("<OnInsertRequest> 실패");
            return false;
        }
    }
    
    private string FormatedTable(DataSet dataSet)
    {
        string message = string.Empty;

        foreach(DataTable table in dataSet.Tables)
        {
            foreach(DataRow row in table.Rows)
            {
                foreach(DataColumn column in table.Columns)
                {
                    message += $"{column.ColumnName} : {row[column]}";
                }
            }
        }

        return message; 
    }
}
