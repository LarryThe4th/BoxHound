using UnityEngine;
using System.Collections.Generic;

public class ExeclDataManager {

    private Dictionary<string, Dictionary<string, string>> m_DataDictionary = new Dictionary<string, Dictionary<string, string>>();

    public void LoadFile(string cfgFileName) {
        ExcelDataLoader.LoadExcelFormCSV(cfgFileName, out m_DataDictionary);
    }

    // Load the data form the library
    public string LoadDataFormFile(string dataKey, string idKey) {
        if (m_DataDictionary.ContainsKey(dataKey)) {
            if (m_DataDictionary[dataKey].ContainsKey(idKey)) {
                return m_DataDictionary[dataKey][idKey];
            }
        }
        return null;
    }

    // Get amount of data in the dirctionary
    public int GetDataCount() {
        int count = 0;
        foreach (var item in m_DataDictionary)
        {
            count = item.Value.Count;
            return count;
        }
        return -1;
    }
}
