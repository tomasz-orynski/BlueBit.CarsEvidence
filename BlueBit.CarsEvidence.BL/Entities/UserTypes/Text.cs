using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using System;
using System.Data;

namespace BlueBit.CarsEvidence.BL.Entities.UserTypes
{
    public class Text : 
        IUserType
    {
        public bool IsMutable { get { return false; } }
        public Type ReturnedType { get { return typeof(string); } }
        public SqlType[] SqlTypes { get { return new[] { NHibernateUtil.String.SqlType }; } }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var obj = NHibernateUtil.String.NullSafeGet(rs, names[0]);
            if (obj == null)
                return null;

            var value = (string)obj;
            if (String.IsNullOrEmpty(value))
                return null;

            return value;
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            ((IDataParameter)cmd.Parameters[index]).Value =
                value == null || String.IsNullOrEmpty((string)value)
                ? DBNull.Value
                : value;
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public new bool Equals(object x, object y)
        {
            if (x == null || y == null)
                return false;
            if (ReferenceEquals(x, y))
                return true;
            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            return x == null 
                ? typeof(string).GetHashCode() + 473 
                : x.GetHashCode();
        }
    }
}
