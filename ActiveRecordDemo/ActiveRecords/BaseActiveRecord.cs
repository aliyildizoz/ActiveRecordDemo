using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;

namespace ActiveRecordDemo.ActiveRecords
{
    public class BaseActiveRecord<T>  where T : class, IActiveRecord, new()
    {
        private static string _connectionString = "Server=DESKTOP-2CK1IFD; Database=ActiveRecordPatternDb; Trusted_Connection=True;";
       
        public T Save(T activeRecord)
        {
            T t = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                int rowEffected = conn.Execute(CreateInsertCommand(activeRecord.GetSchemaName()), activeRecord);
                if (rowEffected > 0)
                {
                    t = conn.QueryFirstOrDefault<T>(CreateFindQuery(activeRecord));
                }
                conn.Close();
            }
            return t;
        }
        public void Update(T activeRecord)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                conn.Execute(CreateUpdateCommand(activeRecord), activeRecord);
                conn.Close();
            }
        }
        public void Delete(T activeRecord)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                conn.Execute(CreateDeleteCommand(activeRecord), activeRecord);
                conn.Close();
            }
        }
        public static IEnumerable<T> Read()
        {
            IEnumerable<T> tList = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                tList = conn.Query<T>($"SELECT * FROM {new T().GetSchemaName()}.{typeof(T).Name}");
                conn.Close();
            }
            return tList;
        }
        public static T FindById(int id)
        {
            T t = null;
            var idInfo = GetColumnsWithValues(new T()).FirstOrDefault(t => t.Item1.ToLowerInvariant().Contains("id"));
            string tableName = typeof(T).Name;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query =
                    $"SELECT * FROM {new T().GetSchemaName()}.{typeof(T).Name} WHERE {idInfo.Item1}={id}";
                t = conn.QueryFirstOrDefault<T>(query);
                conn.Close();
            }
            return t;
        }

        private string CreateFindQuery(T activeRecord)
        {
            var columnsWithValues = GetColumnsWithValues(activeRecord).Where(t => !t.Item1.ToLowerInvariant().Contains("id")).ToList();
            string tableName = typeof(T).Name;
            List<string> setClouseList = new List<string>();
            for (int i = 0; i < columnsWithValues.Count; i++)
            {
                var columnsWithValue = columnsWithValues.ElementAt(i);
                string value = columnsWithValue.Item3.Contains("int")
                    ? columnsWithValue.Item2.ToString()
                    : $"'{columnsWithValue.Item2.ToString()}'";
                setClouseList.Add($"{columnsWithValue.Item1}={value}");
            }
            return $"SELECT * FROM {activeRecord.GetSchemaName()}.{tableName} WHERE {string.Join(" AND ", setClouseList)}";
        }
        private string CreateInsertCommand(string schemaName)
        {
            string[] columns = GetColumnsExceptPK();
            string tableName = typeof(T).Name;
            return $"INSERT INTO {schemaName}.{tableName}({string.Join(",", columns)}) VALUES(@{string.Join(",@", columns)})";
        }
        private static List<Tuple<string, object, string>> GetColumnsWithValues(T activeRecord)
        {
            return typeof(T).GetProperties()
                .Select(p => new Tuple<string, object, string>(p.Name, p.GetValue(activeRecord), p.PropertyType.Name.ToLowerInvariant())).ToList();
        }
        private string[] GetColumnsExceptPK()
        {
            return typeof(T).GetProperties().Select(p => p.Name).Where(p => !p.Contains("Id")).ToArray();
        }
        private string CreateUpdateCommand(T activeRecord)
        {
            var id = GetColumnsWithValues(activeRecord).FirstOrDefault(t => t.Item1.ToLowerInvariant().Contains("id"));
            var columnsWithValues = GetColumnsWithValues(activeRecord).Where(t => !t.Item1.ToLowerInvariant().Contains("id")).ToList();
            string tableName = typeof(T).Name;
            List<string> setClouseList = new List<string>();
            for (int i = 0; i < columnsWithValues.Count; i++)
            {
                var columnsWithValue = columnsWithValues.ElementAt(i);
                string value = columnsWithValue.Item3.Contains("int")
                    ? columnsWithValue.Item2.ToString()
                    : $"'{columnsWithValue.Item2.ToString()}'";
                setClouseList.Add($"{columnsWithValue.Item1}={value}");
            }
            return $"UPDATE {activeRecord.GetSchemaName()}.{tableName} SET {string.Join(", ", setClouseList)} WHERE {id.Item1}={id.Item2.ToString()}";
        }
        private string CreateDeleteCommand(T activeRecord)
        {
            var id = GetColumnsWithValues(activeRecord).FirstOrDefault(t => t.Item1.ToLowerInvariant().Contains("id"));
            string tableName = typeof(T).Name;
            return $"DELETE FROM {activeRecord.GetSchemaName()}.{tableName} WHERE {id.Item1}={id.Item2.ToString()}";
        }
    }
}
