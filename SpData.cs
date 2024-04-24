using Android.App;
using Android.Content;

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
    }
}