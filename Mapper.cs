using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using EmuladorCajero.DTO;

namespace EmuladorCajero
{
    class Mapper
    {
        private static Assembly _thisAssemble;

        private static Assembly GetAssembly()
        {
            if (_thisAssemble == null)
            {
                _thisAssemble = Assembly.GetExecutingAssembly();
            }
            return _thisAssemble;
        }
        private static void SetObjectProperty(string propertyName, object value, object obj)
        {
            try
            {
                PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);

                if (propertyInfo != null)
                {
                    if (value is Hashtable)
                        MapObject((Hashtable)value, propertyInfo.GetValue(obj, null));
                    else if (value is ArrayList)
                    {
                        ArrayList v = value as ArrayList;
                        IList arr = (IList)propertyInfo.GetValue(obj, null);
                        string arrName = Convert.ToString(arr);
                        object newItem = GetAssembly().CreateInstance(arrName.Substring(arrName.IndexOf('[') + 1, arrName.Length - arrName.IndexOf('[') - 2));
                        foreach (var item in v)
                        {
                            MapObject((Hashtable)item, newItem);
                            arr.Add(newItem);
                        }
                    }
                    else
                        propertyInfo.SetValue(obj, value, null);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(propertyName + ": " + ex.Message);
            }
        }

        public static ResponseDTO MapResponse(object obj, Hashtable ht)
        {
            ResponseDTO res = new ResponseDTO() { responseData = obj};
            MapObject(ht, res);
            return res;
        }

        public static void MapObject(Hashtable ht, object res)
        {
            foreach (DictionaryEntry entry in ht)
            {
                if (res != null)
                    SetObjectProperty(Convert.ToString(entry.Key), entry.Value, res);    
            }
        }

    }
}
