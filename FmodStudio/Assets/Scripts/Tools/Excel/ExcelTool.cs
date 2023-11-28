using Excel;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace EditorTool
{
#if UNITY_EDITOR
    public class ExcelConfig
    {
        /// <summary>
        /// ���excel���ļ��еĵ�·��������xecel�������"Assets/Excels/"����
        /// </summary>
        public static readonly string excelsFolderPath = Application.dataPath + "/Excels/";
        public static string ExcelDataPath = Application.dataPath + "/../Excel";//ԴExcel�ļ���,xlsx��ʽ
        /// <summary>
        /// ���Excelת��CS�ļ����ļ���·��
        /// </summary>
        public static readonly string assetPath = "Assets/Resources/DataAssets/";
    }
    public class ExcelTool
    {

        public class ExcelBuild : Editor
        {

            
            public static void CreateItemAsset()
            {
                ExcelConverter.ConvertExcelToClass(ExcelConfig.ExcelDataPath, "Assets/Scripts/DataTable");
            }
        }

    }
#endif
}
