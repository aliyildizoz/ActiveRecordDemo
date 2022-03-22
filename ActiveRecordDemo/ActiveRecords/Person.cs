using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Dapper;

namespace ActiveRecordDemo.ActiveRecords
{
    public class Person : BaseActiveRecord<Person>, IActiveRecord<Person>
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }


        public void Save()
        {
            this.Id = base.Save(this).Id;
        }
        public void Update()
        {
            base.Update(this);
        }
        public void Delete()
        {
            base.Delete(this);
        }
        public IEnumerable<Person> Read()
        {
            return BaseActiveRecord<Person>.Read();
        }
        public Person FindById()
        {
            return BaseActiveRecord<Person>.FindById(this.Id);
        }
        public string SchemaName = "ActiveRecord";
        public string GetSchemaName()
        {
            return SchemaName;
        }
    }
}
