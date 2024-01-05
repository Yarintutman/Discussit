using Android.App;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discussit
{
    /// <summary>
    ///  A SharedPreferences object points to a file containing key-value pairs and provides 
    ///  simple methods to read and write them. Each SharedPreferences file is managed by the 
    ///  framework and can be private or shared (here it's private) and stored in the application 
    ///  private data directory 
    /// </summary>
    class SpData
    {
        private readonly ISharedPreferences sp;
        private readonly ISharedPreferencesEditor editor;

        /// <summary>
        /// Constructor - Creates the objects above in order to read and write the data
        /// </summary>
        /// <param name="spFileName">the shared preference file name</param>
        public SpData(string spFileName)
        {
            sp = Application.Context.GetSharedPreferences(spFileName, FileCreationMode.Private);
            editor = sp.Edit();
        }

        /// <summary>
        /// used to save a string value in the shared preference file
        /// </summary>
        /// <param name="key">data key</param>
        /// <param name="value">data value</param>
        /// <returns>success boolean</returns>
        public bool PutString(string key, string value)
        {
            editor.PutString(key, value);
            return editor.Commit();
        }

        /// <summary>
        /// used to read a string value
        /// </summary>
        /// <param name="key">data key</param>
        /// <param name="defValue">default value</param>
        /// <returns>the stored value or the default value</returns>
        public string GetString(string key, string defValue)
        {
            return sp.GetString(key, defValue);
        }

        /// <summary>
        /// used to save a float value in the shared preference file
        /// </summary>
        /// <param name="key">data key</param>
        /// <param name="value">data value</param>
        /// <returns>success boolean</returns>
        public bool PutFloat(string key, float value)
        {
            editor.PutFloat(key, value);
            return editor.Commit();
        }

        /// <summary>
        /// used to read a float value
        /// </summary>
        /// <param name="key">data key</param>
        /// <param name="defValue">default value</param>
        /// <returns>the stored value or the default value</returns>
        public float GetFloat(string key, float defValue)
        {
            return sp.GetFloat(key, defValue);
        }

        /// <summary>
        /// used to save a long value in the shared preference file
        /// </summary>
        /// <param name="key">data key</param>
        /// <param name="value">data value</param>
        /// <returns>success boolean</returns>
        public bool PutLong(string key, long value)
        {
            editor.PutLong(key, value);
            return editor.Commit();
        }


        /// <summary>
        /// used to read a long value
        /// </summary>
        /// <param name="key">data key</param>
        /// <param name="defValue">default value</param>
        /// <returns>the stored value or the default value</returns>
        public long GetLong(string key, long defValue)
        {
            return sp.GetLong(key, defValue);
        }

        /// <summary>
        /// used to save an integer value in the shared preference file
        /// </summary>
        /// <param name="key">data key</param>
        /// <param name="value">data value</param>
        /// <returns>success boolean</returns>
        public bool PutInt(string key, int value)
        {
            editor.PutInt(key, value);
            return editor.Commit();
        }


        /// <summary>
        /// used to read an integer value
        /// </summary>
        /// <param name="key">data key</param>
        /// <param name="defValue">default value</param>
        /// <returns>the stored value or the default value</returns>
        public int GetInt(string key, int defValue)
        {
            return sp.GetInt(key, defValue);
        }

        /// <summary>
        /// used to save a boolean value in the shared preference file
        /// </summary>
        /// <param name="key">data key</param>
        /// <param name="value">data value</param>
        /// <returns>success boolean</returns>
        public bool PutBool(string key, bool value)
        {
            editor.PutBoolean(key, value);
            return editor.Commit();
        }


        /// <summary>
        /// used to read a boolean value
        /// </summary>
        /// <param name="key">data key</param>
        /// <param name="defValue">default value</param>
        /// <returns>the stored value or the default value</returns>
        public bool GetBool(string key, bool defValue)
        {
            return sp.GetBoolean(key, defValue);
        }

        /// <summary>
        /// used to save a datetime value in the shared preference file
        /// it uses the datetime ticks long value to represent the saved datetime
        /// </summary>
        /// <param name="key">data key</param>
        /// <param name="value">data value</param>
        /// <returns>success boolean</returns>
        public bool PutDateTime(string key, DateTime value)
        {
            editor.PutLong(key, value.Ticks);
            return editor.Commit();
        }

        /// <summary>
        /// used to read a datetime value
        /// </summary>
        /// <param name="key">data key</param>
        /// <param name="defValue">default value</param>
        /// <returns>the datetime stored value or the default value 
        /// (the data in the file is the ticks value of the datetime)</returns>
        public DateTime GetDateTime(string key, DateTime defValue)
        {
            long ticks = sp.GetLong(key, defValue.Ticks);
            return new DateTime(ticks);
        }

        /// <summary>
        /// used to save a list of string values in the shared preference file
        /// </summary>
        /// <param name="key">data key</param>
        /// <param name="lst">list of string values</param>
        /// <returns>success boolean</returns>
        public bool PutStringList(string key, List<string> lst)
        {
            editor.PutStringSet(key, lst);
            return editor.Commit();
        }


        /// <summary>
        /// used to read a list of string values
        /// </summary>
        /// <param name="key">data key</param>
        /// <param name="defLst">default list of string values</param>
        /// <returns>the stored list of string values or the default list of string values</returns>
        public List<string> GetStringList(string key, List<string> defLst)
        {
            ICollection<string> collection = sp.GetStringSet(key, defLst);
            return collection.ToList();
        }
    }
}