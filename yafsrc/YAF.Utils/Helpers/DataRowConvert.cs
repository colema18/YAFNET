using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAF.Utils.Helpers
{
    public class DataRowConvert
    {
        private DataRow _dbRow;

        public DataRowConvert(DataRow dbRow)
        {
            _dbRow = dbRow;
        }

        public string AsString(string columnName)
        {
            if (_dbRow[columnName] == DBNull.Value) return null;
            return _dbRow[columnName].ToString();
        }

        public bool AsBool(string columnName)
        {
            if (_dbRow[columnName] == DBNull.Value) return false;
            return Convert.ToBoolean(_dbRow[columnName]);
        }

        public DateTime? AsDateTime(string columnName)
        {
            if (_dbRow[columnName] == DBNull.Value) return null;
            return Convert.ToDateTime(_dbRow[columnName]);
        }

        public int? AsInt32(string columnName)
        {
            if (_dbRow[columnName] == DBNull.Value) return null;
            return Convert.ToInt32(_dbRow[columnName]);
        }

        public long? AsInt64(string columnName)
        {
            if (_dbRow[columnName] == DBNull.Value) return null;
            return Convert.ToInt64(_dbRow[columnName]);
        }
    }
}
