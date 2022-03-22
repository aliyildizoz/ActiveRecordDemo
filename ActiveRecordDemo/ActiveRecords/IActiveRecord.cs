using System;
using System.Collections.Generic;
using System.Text;

namespace ActiveRecordDemo.ActiveRecords
{
    public interface IActiveRecord<out T> : IActiveRecord where T : class, new()
    {
        void Save();
        void Update();
        void Delete();
        IEnumerable<T> Read();
        T FindById();
    }

    public interface IActiveRecord
    {
        string GetSchemaName();
    }
}
