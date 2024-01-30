using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReadSpreadSheet : MonoBehaviour
{
    public readonly string ADDRESS  = "https://docs.google.com/spreadsheets/d/16slVFqeg2egBHNcS-NPDRZzizFQwPH1oyr9AVtt9U2k";
    public readonly string RANGE    = "B2:E";
    public readonly long SHEET_ID   = 0;
    public int rowCount;

    private void Start()
    {
        StartCoroutine(LoadData());
    }

    private IEnumerator LoadData()
    {
        UnityWebRequest www = UnityWebRequest.Get(GetTSVAddress(ADDRESS, RANGE, SHEET_ID));
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Error loading data : " + www.error);
        }
        else
        {
            string sheetData = www.downloadHandler.text;
            //행 별로 데이터 분할
            string[] rows = sheetData.Split('\n');

            rowCount  = rows.Length -1;
        }

        //Debug.Log(www.downloadHandler.text);
        //Debug.Log(rowCount);
    }

    public static string GetTSVAddress(string address, string range, long sheetID)
    {
        return $"{address}/export?format=tsv&range={range}&gid={sheetID}";
    }

}
