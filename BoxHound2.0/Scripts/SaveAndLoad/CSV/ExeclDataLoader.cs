using UnityEngine;
using System.Collections.Generic;

public static class ExcelDataLoader
{
    // Load the text config file.
	public static void LoadExcelFormCSV (string cfgName,out Dictionary<string,Dictionary<string,string>> DicContent)
	{
        if (string.IsNullOrEmpty(cfgName)) {
            DicContent = null;
            return;
        }
        // Create a text asset and load the file form resources folder
        TextAsset fileContext = Resources.Load("ConfigFiles/"+ cfgName) as TextAsset;


        if (fileContext == null) {
#if UNITY_EDITOR
            Debug.LogError("Load Excel file faild, file name: " + cfgName);
#endif
            DicContent = null;
            return;
        } 

        // Store all the content.
        string strLine = fileContext.text;
        // Create a dictionary for store data form the config file.
        Dictionary<string, string[]> content = new Dictionary<string, string[]>();
        // Explain the strings
        ExplainString(strLine, content, out DicContent );
	}
    // Explain the strings
    public static void ExplainString(string strLine,  Dictionary<string, string[]> content, out Dictionary<string, Dictionary<string, string>> DicContent)
	{
        // Create a dictionary for store default data
		Dictionary<string, string> initNum = new Dictionary<string, string> ();
        // Use "\n" to split file context
		string[] lineArray = strLine.Split (new char[]{'\n'});
        // Get the count of rows.
        int rows = lineArray.Length;
        // Get the count of columns
        int Columns = lineArray[0].Split(new char[] { ',' }).Length;
        // Set a array for all the headers.
        string[] ColumnName = new string[Columns];
        for (int i = 0; i < rows; i++)
        {
            // Every data split by "," mark.
            string[] Array = lineArray[i].Split(new char[] { ',' });
            for (int j = 0; j < Columns; j++)
            {
                // Get array's data.
                string nvalue = Array[j].Trim();
                // The first row.
                if (i == 0)
                {
                    // Get the header name
                    ColumnName[j] = nvalue;
                    // Read form the 3 row.
                    content[ColumnName[j]] = new string[rows - 3];
                }
                // Default data
                else if (i == 2)
                {
                    // Store the default data.
                    initNum[ColumnName[j]] = nvalue;
                }
                // Actul data
                else if (i >= 3)
                {
                    content[ColumnName[j]][i - 3] = nvalue;
                    // If the data is empty, set as default data.
                    if (nvalue == "")
                    {
                        content[ColumnName[j]][i - 3] = initNum[ColumnName[j]];
                    }
                }

            }
        }
        // Explain the Dictionary
        ExplainDictionary(content, out DicContent);
	}

     public static void ExplainDictionary(Dictionary<string,string[]> content,out Dictionary<string, Dictionary<string, string>> DicContent)
     {
         // Create a new dictionary, it contains header, ID and data.
         DicContent = new Dictionary<string, Dictionary<string, string>>();
         // Get all the data by its header
         Dictionary<string, string[]>.KeyCollection Allkeys = content.Keys;
         // loop through all the header
         foreach (string key in Allkeys)
         {
             Dictionary<string, string> hasData = new Dictionary<string, string>();
             string[] Data = content[key];
             for (int i = 0; i < Data.Length; i++)
             {
                 hasData[content["ID"][i]] = Data[i];
             }
             DicContent[key] = hasData;
         }
     }
 
}
