using System;
using System.Data;
using System.IO;
using Excel;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEditor;

public static class ExcelConverter {
    static string XmlDataPath = Application.dataPath + "/Resources/XmlData";
    static string xmlfilepath = "/Resources/XmlData/";
    public static string ExcelDataPath = Application.dataPath + "/Resources/Excel"; //ԴExcel�ļ���,xlsx��ʽ
    static string CsClassPath = Application.dataPath + "/Scripts/GamePlay/DataClass"; //���ɵ�c#�ű��ļ���
    static string CsDataClassPath = Application.dataPath + "/Scripts/GamePlay/DataTable"; //���ɵ�c#�ű��ļ���
    static string AllCsHead = "all"; //���л��ṹ���������.����ǰ׺
    static char ArrayTypeSplitChar = ','; //��������ֵ��ַ�: int[] 1#2#34 string[] ���#�ټ� bool[] true#false ...

    [MenuItem("CustomEditor/Excel/Step3-Cs2DataCs]")]
    static void Cs2Data() {
        Init();
        Cs2DataCs();
    }

    [MenuItem("CustomEditor/Excel/Step1-ExcelToXml")]
    static void Excel2Xml2Bytes() {
        Init();
        //�����м��ļ�xml
        Excel2CsOrXml(false);
        //����bytes
        //WriteBytes();
    }

    [MenuItem("CustomEditor/Excel/Step2-XMLtoScript]")]
    static void Excel2Cs_Xr() {
        Init();
        Excel2CsOrXmlByXr(true);
    }


    static void Init() {
        if (!Directory.Exists(CsClassPath)) {
            Directory.CreateDirectory(CsClassPath);
        }

        if (!Directory.Exists(XmlDataPath)) {
            Directory.CreateDirectory(XmlDataPath);
        }

        if (!Directory.Exists(CsDataClassPath)) {
            Directory.CreateDirectory(CsDataClassPath);
        }

        Debug.Log("InitDicSuccess");
    }

    public static void ConvertExcelToClass(string excelDirectory, string classDirectory) {
        string[] filePaths = Directory.GetFiles(excelDirectory, "*.xlsx");
        foreach (string filePath in filePaths) {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet dataSet = excelReader.AsDataSet();

            string className = Path.GetFileNameWithoutExtension(filePath);
            using (StreamWriter writer = new StreamWriter(classDirectory + "/" + className + ".cs")) {
                writer.WriteLine("using System;");
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine("");
                writer.WriteLine("[Serializable]");
                writer.WriteLine("public class " + className);
                writer.WriteLine("{");

                foreach (DataTable dataTable in dataSet.Tables) {
                    foreach (DataColumn column in dataTable.Columns) {
                        writer.WriteLine("    [SerializeField]");
                        writer.WriteLine("    public " + GetTypeName(column.DataType) + " " + column.ColumnName + ";");
                    }
                }

                writer.WriteLine("}");
            }
        }
    }

    static void Excel2CsOrXml(bool isCs) {
        if (!isCs) {
            Directory.Delete(XmlDataPath, true);
            Directory.CreateDirectory(XmlDataPath);
        }
        else {
            Directory.Delete(CsClassPath, true);
            Directory.CreateDirectory(CsClassPath);
        }

        string[] excelPaths = Directory.GetFiles(ExcelDataPath, "*.xlsx");
        for (int e = 0; e < excelPaths.Length; e++) {
            //0.��Excel
            string className; //������
            string[] names; //�ֶ���
            string[] types; //�ֶ�����
            string[] descs; //�ֶ�����
            List<string[]> datasList; //����

            try {
                string excelPath = excelPaths[e]; //excel·��  
                className = Path.GetFileNameWithoutExtension(excelPath).ToLower();
                FileStream fileStream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                // �������ȫ����ȡ��result��
                DataSet result = excelDataReader.AsDataSet();
                // ��ȡ�������
                int columns = result.Tables[0].Columns.Count;
                // ��ȡ�������
                int rows = result.Tables[0].Rows.Count;
                // �����������ζ�ȡ����е�ÿ������
                names = new string[columns];
                types = new string[columns];
                descs = new string[columns];
                datasList = new List<string[]>();
                for (int r = 0; r < rows; r++) {
                    string[] curRowData = new string[columns];
                    for (int c = 0; c < columns; c++) {
                        //��������ȡ��һ�������ָ����ָ���е�����
                        string value = result.Tables[0].Rows[r][c].ToString();
                        if (value.StartsWith("^")) {
                            value = "cehuaUse" + c;
                        }

                        //���ǰ���еı��������������� ��β�ո�
                        if (r < 2) {
                            value = value.TrimStart(' ').TrimEnd(' ');
                        }

                        curRowData[c] = value;
                    }

                    //��������һ���������
                    if (r == 0) {
                        names = curRowData;
                    } //�������ڶ������������
                    else if (r == 1) {
                        types = curRowData;
                    } //���������������������
                    else if (r == 2) {
                        descs = curRowData;
                    } //�����������п�ʼ������
                    else {
                        datasList.Add(curRowData);
                    }
                }
            }
            catch (System.Exception exc) {
                Debug.LogError("��ر�Excel:" + exc.Message);
                return;
            }

            if (isCs) {
                //дCs
                WriteCs(className, names, types, descs);
            }
            else {
                //дXml
                WriteXml(className, names, types, datasList);
            }
        }

        AssetDatabase.Refresh();
    }

    static void Excel2CsOrXmlByXr(bool isCs) {
        if (!isCs) {
            Directory.Delete(XmlDataPath, true);
            Directory.CreateDirectory(XmlDataPath);
        }
        else {
            Directory.Delete(CsClassPath, true);
            Directory.CreateDirectory(CsClassPath);
        }

        string[] excelPaths = Directory.GetFiles(ExcelDataPath, "*.xlsx");
        for (int e = 0; e < excelPaths.Length; e++) {
            //0.��Excel
            string className; //������
            string[] names; //�ֶ���
            string[] types; //�ֶ�����
            string[] descs; //�ֶ�����
            List<string[]> datasList; //����

            try {
                string excelPath = excelPaths[e]; //excel·��  
                className = Path.GetFileNameWithoutExtension(excelPath).ToLower();
                FileStream fileStream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                // �������ȫ����ȡ��result��
                DataSet result = excelDataReader.AsDataSet();
                // ��ȡ�������
                int columns = result.Tables[0].Columns.Count;
                // ��ȡ�������
                int rows = result.Tables[0].Rows.Count;
                // �����������ζ�ȡ����е�ÿ������
                names = new string[columns];
                types = new string[columns];
                descs = new string[columns];
                datasList = new List<string[]>();
                for (int r = 0; r < rows; r++) {
                    string[] curRowData = new string[columns];
                    for (int c = 0; c < columns; c++) {
                        //��������ȡ��һ�������ָ����ָ���е�����
                        string value = result.Tables[0].Rows[r][c].ToString();
                        if (value.StartsWith("^")) {
                            value = "cehuaUse" + c;
                        }

                        //���ǰ���еı��������������� ��β�ո�
                        if (r < 2) {
                            value = value.TrimStart(' ').TrimEnd(' ');
                        }

                        curRowData[c] = value;
                    }

                    //��������һ���������
                    if (r == 0) {
                        names = curRowData;
                    } //�������ڶ������������
                    else if (r == 1) {
                        string[] rawTypes = new string[curRowData.Length];
                        for (int i = 0; i < curRowData.Length; i++) {
                            rawTypes[i] = curRowData[i].ToLower();
                        }

                        // ��Сд����תΪ��Ӧ�� .NET ����
                        Type[] _types = new Type[curRowData.Length];
                        for (int i = 0; i < rawTypes.Length; i++) {
                            // �ж��Ƿ�Ϊ��������
                            bool isArray = rawTypes[i].EndsWith("[]");

                            // �ж��Ƿ�Ϊ�б�����
                            bool isList = rawTypes[i].StartsWith("list<") && rawTypes[i].EndsWith(">");

                            Debug.Log($"i: {i}, isList: {isList}, isArray: {isArray}, rawTypes[i]: {rawTypes[i]}");

                            string typeName = "";
                            /*string typeName = isArray || isList
                                ? rawTypes[i].Substring(isList ? 5 : 0, rawTypes[i].Length - (isArray ? 2 : (isList ? 1 : 0)))
                                : rawTypes[i];*/
                            if (isArray || isList) {
                                if (isArray) {
                                    typeName = rawTypes[i].Substring(0, rawTypes[i].Length - 2);
                                }
                                else {
                                    typeName = rawTypes[i].Substring(5, rawTypes[i].Length - 6);
                                }
                            }
                            else {
                                typeName = rawTypes[i];
                            }

                            Debug.Log($"rawTypes[i]: {rawTypes[i]}, isList: {isList}, typeName: {typeName}");
                            Debug.Log($"i: {i}");
                            switch (typeName) {
                                case "int":
                                    _types[i] = isArray ? typeof(int[]) : (isList ? GetListType<int>() : typeof(int));
                                    break;
                                case "string":
                                    _types[i] = isArray ? typeof(string[]) : (isList ? GetListType<string>() : typeof(string));
                                    break;
                                case "float":
                                    _types[i] = isArray ? typeof(float[]) : (isList ? GetListType<float>() : typeof(float));
                                    break;
                                case "bool":
                                    _types[i] = isArray ? typeof(bool[]) : (isList ? GetListType<bool>() : typeof(bool));
                                    break;
                                // ���Ը�����Ҫ����������͵Ĵ���
                                default:
                                    Debug.LogWarning($"δ֪�����ͣ�{rawTypes[i]}��Ĭ��Ϊ object ���͡�");
                                    _types[i] = isArray ? typeof(object[]) : (isList ? GetListType<object>() : typeof(object));
                                    break;
                            }
                        }

                        string[] typeNames = _types.Select(GetFriendlyTypeName).ToArray();
                        types = typeNames;
                    } //���������������������
                    else if (r == 2) {
                        descs = curRowData;
                    } //�����������п�ʼ������
                    else {
                        datasList.Add(curRowData);
                    }
                }
            }
            catch (System.Exception exc) {
                Debug.LogError("��ر�Excel:" + exc.Message);
                Debug.LogError("��ջ��Ϣ��" + exc.StackTrace);
                return;
            }

            if (isCs) {
                //дCs
                WriteCsByXr(className, names, types, descs);
            }
            else {
                //дXml
                WriteXml(className, names, types, datasList);
            }
        }

        AssetDatabase.Refresh();
    }

    static Type GetListType<T>() {
        return typeof(List<T>);
    }

    private static string GetFriendlyTypeName(Type type) {
        // �ж��Ƿ�Ϊ float ����
        if (type == typeof(float) || type == typeof(Single)) {
            return "float";
        }

        if (type == typeof(Int32)) {
            return "int";
        }

        if (!type.IsGenericType) {
            return type.Name;
        }
        string typeName = type.Name;
        int backtickIndex = typeName.IndexOf('`');
        if (backtickIndex > 0) {
            typeName = typeName.Substring(0, backtickIndex);
        }

        Type[] genericArguments = type.GetGenericArguments();
        string genericArgumentsString = string.Join(", ", genericArguments.Select(GetFriendlyTypeName));

        return $"{typeName}<{genericArgumentsString}>";
    }

    static void Cs2DataCs() {
        if (!Directory.Exists(CsDataClassPath)) {
            Directory.CreateDirectory(CsDataClassPath);
        }

        string[] CsPaths = Directory.GetFiles(CsClassPath, "*.cs");
        if (CsPaths.Length <= 0) {
            Debug.LogError(CsClassPath + "·����û��Cs�ļ�");
        }

        for (int e = 0; e < CsPaths.Length; e++) {
            //0.��Excel
            string className; //������
            try {
                string excelPath = CsPaths[e]; //cs·��  
                className = Path.GetFileNameWithoutExtension(excelPath).ToLower();
            }
            catch (System.Exception exc) {
                Debug.LogError("��ر�Excel:" + exc.Message);
                return;
            }

            WriteCsByXrOperation(className);
        }

        AssetDatabase.Refresh();
    }

    static string MapTypeToName(Type type) {
        // �ж��Ƿ�Ϊ float ����
        if (type == typeof(float) || type == typeof(Single)) {
            return "float";
        }
        // ����������͵�ӳ��
        // else if (type == typeof(int) || type == typeof(Int32))
        // {
        //     return "int";
        // }
        // �������͵�ӳ��...

        // ���û��ƥ�䵽�ض����ͣ��������͵�Ĭ������
        return type.Name;
    }

    static void WriteCsByXr(string className, string[] names, string[] types, string[] descs) {
        try {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;");
            stringBuilder.AppendLine("using System.Collections.Generic;");
            stringBuilder.AppendLine("using System.IO;");
            stringBuilder.AppendLine("using System.Runtime.Serialization.Formatters.Binary;");
            stringBuilder.AppendLine("using System.Xml.Serialization;");
            stringBuilder.AppendLine("using System.Xml;");
            stringBuilder.AppendLine("using UnityEngine;");
            stringBuilder.Append("\n");
            stringBuilder.AppendLine("namespace ThGold.Table");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    [Serializable]");
            stringBuilder.AppendLine("    public class " + className + " : DefaultDataBase");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        private static DefaultDataBase _inst;");
            stringBuilder.AppendLine("        public static DefaultDataBase Instance");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            get");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                if (_inst == null)");
            stringBuilder.AppendLine("                {");
            stringBuilder.AppendLine("                    _inst = new " + className + "();");
            stringBuilder.AppendLine("                }");
            stringBuilder.AppendLine("                return _inst;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("        }");
            for (int i = 0; i < names.Length; i++) {
                stringBuilder.AppendLine("        /// <summary>");
                stringBuilder.AppendLine("        /// " + descs[i]);
                stringBuilder.AppendLine("        /// </summary>");
                stringBuilder.AppendLine("        [XmlAttribute(\"" + names[i] + "\")]");

                string type = types[i];
                if (type.Contains("[]")) {
                    //type = type.Replace("[]", "");
                    //stringBuilder.AppendLine("        public List<" + type + "> " + names[i] + ";");

                    //��ѡ���룺
                    //��_name�ֶ�ȥ�����л���nameȡ_name.item��ֵ,ֱ�ӷ���list<type>��
                    //��Ϊxmlÿ�п����ж�������ֶΣ������Ͷ���һ�����item�����Է��ʵ�ʱ����Ҫ.item����ȡ��list<type>
                    //����ö����һ������ֱ�ӷ���List<type>��
                    type = type.Replace("[]", "");
                    stringBuilder.AppendLine("        public List<" + type + "> " + names[i] + "");
                    stringBuilder.AppendLine("        {");
                    stringBuilder.AppendLine("            get");
                    stringBuilder.AppendLine("            {");
                    stringBuilder.AppendLine("                if (_" + names[i] + " != null)");
                    stringBuilder.AppendLine("                {");
                    stringBuilder.AppendLine("                    return _" + names[i] + ".item;");
                    stringBuilder.AppendLine("                }");
                    stringBuilder.AppendLine("                return null;");
                    stringBuilder.AppendLine("            }");
                    stringBuilder.AppendLine("        }");
                    stringBuilder.AppendLine("        [XmlElementAttribute(\"" + names[i] + "\")]");
                    stringBuilder.AppendLine("        public " + type + "Array _" + names[i] + ";");
                }
                else {
                    if (names[i] == "ID")
                        stringBuilder.AppendLine("        public override " + type + " " + names[i] + "{ get; set; }");
                    else
                        stringBuilder.AppendLine("        public " + type + " " + names[i] + ";");
                }

                stringBuilder.Append("\n");
            }

            stringBuilder.AppendLine("        protected override void LoadBytesInfo()");
            stringBuilder.AppendLine("        {");
            // stringBuilder.AppendLine("            Utils.LoadXMLByBundleByThreadAsync(\"" + className + ".xml\", loadData, CustPackageName.DataTable, CustPackageName.PlatformLobby);");
            
            stringBuilder.AppendLine("           string xmlPath = Application.dataPath + \"" + xmlfilepath + className + ".xml\";");
            stringBuilder.AppendLine("            XmlDocument xmlDoc = new XmlDocument();");
            stringBuilder.AppendLine("           xmlDoc.Load(xmlPath);");
            stringBuilder.AppendLine("             string xmlText = xmlDoc.InnerXml;");
            stringBuilder.AppendLine("            XmlReaderSettings settings = new XmlReaderSettings();");
            stringBuilder.AppendLine("           settings.IgnoreComments = true;");
            stringBuilder.AppendLine("           settings.IgnoreWhitespace = true;");
            stringBuilder.AppendLine("           settings.Async = true;");
            stringBuilder.AppendLine("           loadData(XmlReader.Create(new StringReader(xmlText), settings));");
            //
            stringBuilder.AppendLine("        }");
            stringBuilder.Append("\n");
            stringBuilder.AppendLine("        protected async override void loadDataInfo(XmlReader reader)");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            if (reader == null)");
            stringBuilder.AppendLine("                return;");
            stringBuilder.AppendLine("            while (await reader.ReadAsync())");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("            try");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                 if (reader.NodeType != XmlNodeType.Element || !reader.HasAttributes)");
            stringBuilder.AppendLine("                     continue;");
            stringBuilder.AppendLine("                 " + className + " data = new " + className + "();");
            for (int i = 0; i < names.Length; i++) {
                string type = types[i];
                if (type == "int") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= int.Parse(reader.GetAttribute(\"" + names[i] + "\"));");
                }
                else if (type == "long") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= long.Parse(reader.GetAttribute(\"" + names[i] + "\"));");
                }
                else if (type == "float") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= float.Parse(reader.GetAttribute(\"" + names[i] + "\"));");
                }
                else if (type == "string") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= reader.GetAttribute(\"" + names[i] + "\").ToString();");

                    stringBuilder.AppendLine("                 if(data." + names[i] + " == \"0\")");
                    stringBuilder.AppendLine("                     data." + names[i] + " = string.Empty;");
                }
                else if (type == "bool") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= bool.Parse(reader.GetAttribute(\"" + names[i] + "\"));");
                }
                else if (type.EndsWith("[]")) {
                    // ������������
                    string elementType = type.Substring(0, type.Length - 2); // �Ƴ� "[]" ��ȡԪ������
                    stringBuilder.AppendLine($"                 data.{names[i]}= reader.GetAttribute(\"{names[i]}\").Split(',').Select(x => {ParseValue(elementType, "x")}).ToArray();");
                }
                else if (type.StartsWith("list<") && type.EndsWith(">")) {
                    // �����б�����
                    string elementType = type.Substring(5, type.Length - 6); // �Ƴ� "list<" �� ">"
                    stringBuilder.AppendLine($"                 data.{names[i]}= reader.GetAttribute(\"{names[i]}\").Split(',').Select(x => {ParseValue(elementType, "x")}).ToList();");
                }
            }

            stringBuilder.AppendLine("                 lock (datas)");
            stringBuilder.AppendLine("                 {");
            stringBuilder.AppendLine("                     datas.Add(data);");
            stringBuilder.AppendLine("                 };");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            catch (Exception e)");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                FailDispatchEvent();");
            stringBuilder.AppendLine("                return;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            IsLoadSucceed = true;");
            stringBuilder.AppendLine("            SucceedDispatchEvent(this.GetType().Name);");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string csPath = CsClassPath + "/" + className + ".cs";
            if (File.Exists(csPath)) {
                File.Delete(csPath);
            }

            using (StreamWriter sw = new StreamWriter(csPath)) {
                sw.Write(stringBuilder);
                Debug.Log("����:" + csPath);
            }
        }
        catch (System.Exception e) {
            Debug.LogError("д��CSʧ��:" + e.Message);
            throw;
        }
    }
    private static string ParseValue(string elementType, string value)
    {
        switch (elementType.ToLower())
        {
            case "int":
                return $"int.Parse({value})";
            case "long":
                return $"long.Parse({value})";
            case "float":
                return $"float.Parse({value})";
            case "string":
                return $"{value}";
            case "bool":
                return $"bool.Parse({value})";
            // ����������͵Ĵ���
            default:
                return $"({elementType})({value})"; // Ĭ�����������ֱ��ת��
        }
    }
    static void WriteCsByXrOperation(string className) {
        try {
            string dataname = className + "data";
            string datanames = className + "datas";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;");
            stringBuilder.AppendLine("using System.Collections.Generic;");
            stringBuilder.AppendLine("using System.IO;");
            stringBuilder.AppendLine("using System.Runtime.Serialization.Formatters.Binary;");
            stringBuilder.AppendLine("using System.Xml.Serialization;");
            stringBuilder.AppendLine("using System.Xml;");
            stringBuilder.AppendLine("using UnityEngine;");
            stringBuilder.AppendLine("using ThGold.Event;");
            stringBuilder.AppendLine("using EventHandler = ThGold.Event.EventHandler;");
            stringBuilder.Append("\n");
            stringBuilder.AppendLine("namespace ThGold.Table");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    [Serializable]");
            stringBuilder.AppendLine("public class " + dataname + " : LoadDataBase");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    public Dictionary<int, " + className + "> Datas;");
            stringBuilder.AppendLine("    public static " + dataname + " Instance;");
            stringBuilder.AppendLine("    public override void Init()");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        Instance = this;");
            stringBuilder.AppendLine("        Datas = new Dictionary<int, " + className + ">();");
            stringBuilder.AppendLine("        EventHandler.Instance.EventDispatcher.AddEventListener(\"" + className +
                                     "\"+CustomEvent.DataTableLoadSucceed,LoadDataComplete,EventDispatcherAddMode.SINGLE_SHOT);");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("    public override void InitLoadDataConfig()");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        ConfigList.Add(" + className + ".Instance);");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("    public override void LoadDataComplete(IEvent e)");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        var iter = ConfigList.GetEnumerator();");
            stringBuilder.AppendLine("        List<DefaultDataBase> ls;");
            stringBuilder.AppendLine("        DefaultDataBase ddb;");
            stringBuilder.AppendLine("        while (iter.MoveNext())");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            ddb = iter.Current;");
            stringBuilder.AppendLine("            ls = iter.Current.getdatas();");
            stringBuilder.AppendLine("            if (ls == null)");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                continue;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            if (ddb is " + className + ")");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                for (int i = 0; i < ls.Count; i++)");
            stringBuilder.AppendLine("                {");
            stringBuilder.AppendLine("                    Datas.Add(ls[i].ID, (" + className + ")ls[i]);");
            //����Debug
            stringBuilder.AppendLine("                    Debug.Log(Datas[ls[i].ID].ID);");
            stringBuilder.AppendLine("                }");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("        ls = null;");
            stringBuilder.AppendLine("        ddb = null;");
            stringBuilder.AppendLine("        iter.Dispose();");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("    public override void Reset()");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("   }");
            stringBuilder.AppendLine("  }");
            string csPath = CsDataClassPath + "/" + datanames + ".cs";
            if (File.Exists(csPath)) {
                File.Delete(csPath);
            }

            using (StreamWriter sw = new StreamWriter(csPath)) {
                sw.Write(stringBuilder);
                Debug.Log("����:" + csPath);
            }
        }
        catch (System.Exception e) {
            Debug.LogError("д��CSʧ��:" + e.Message);
            throw;
        }
    }

    private static string GetTypeName(Type type) {
        if (type == typeof(int)) {
            return "int";
        }
        else if (type == typeof(float)) {
            return "float";
        }
        else if (type == typeof(string)) {
            return "string";
        }
        else {
            return "UnknownType";
        }
    }

    static void WriteCs(string className, string[] names, string[] types, string[] descs) {
        try {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;");
            stringBuilder.AppendLine("using System.Collections.Generic;");
            stringBuilder.AppendLine("using System.IO;");
            stringBuilder.AppendLine("using System.Runtime.Serialization.Formatters.Binary;");
            stringBuilder.AppendLine("using System.Xml.Serialization;");
            stringBuilder.AppendLine("using System.Xml;");
            stringBuilder.AppendLine("using UnityEngine;");
            stringBuilder.Append("\n");
            stringBuilder.AppendLine("namespace ThGold.Table");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    [Serializable]");
            stringBuilder.AppendLine("    public class " + className + " : DefaultDataBase");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        private static DefaultDataBase _inst;");
            stringBuilder.AppendLine("        public static DefaultDataBase Instance");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            get");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                if (_inst == null)");
            stringBuilder.AppendLine("                {");
            stringBuilder.AppendLine("                    _inst = new " + className + "();");
            stringBuilder.AppendLine("                }");
            stringBuilder.AppendLine("                return _inst;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("        }");
            for (int i = 0; i < names.Length; i++) {
                stringBuilder.AppendLine("        /// <summary>");
                stringBuilder.AppendLine("        /// " + descs[i]);
                stringBuilder.AppendLine("        /// </summary>");
                stringBuilder.AppendLine("        [XmlAttribute(\"" + names[i] + "\")]");

                string type = types[i];
                if (type.Contains("[]")) {
                    //type = type.Replace("[]", "");
                    //stringBuilder.AppendLine("        public List<" + type + "> " + names[i] + ";");

                    //��ѡ���룺
                    //��_name�ֶ�ȥ�����л���nameȡ_name.item��ֵ,ֱ�ӷ���list<type>��
                    //��Ϊxmlÿ�п����ж�������ֶΣ������Ͷ���һ�����item�����Է��ʵ�ʱ����Ҫ.item����ȡ��list<type>
                    //����ö����һ������ֱ�ӷ���List<type>��
                    type = type.Replace("[]", "");
                    stringBuilder.AppendLine("        public List<" + type + "> " + names[i] + "");
                    stringBuilder.AppendLine("        {");
                    stringBuilder.AppendLine("            get");
                    stringBuilder.AppendLine("            {");
                    stringBuilder.AppendLine("                if (_" + names[i] + " != null)");
                    stringBuilder.AppendLine("                {");
                    stringBuilder.AppendLine("                    return _" + names[i] + ".item;");
                    stringBuilder.AppendLine("                }");
                    stringBuilder.AppendLine("                return null;");
                    stringBuilder.AppendLine("            }");
                    stringBuilder.AppendLine("        }");
                    stringBuilder.AppendLine("        [XmlElementAttribute(\"" + names[i] + "\")]");
                    stringBuilder.AppendLine("        public " + type + "Array _" + names[i] + ";");
                }
                else {
                    if (names[i] == "ID")
                        stringBuilder.AppendLine("        public override " + type + " " + names[i] + "{ get; set; }");
                    else
                        stringBuilder.AppendLine("        public " + type + " " + names[i] + ";");
                }

                stringBuilder.Append("\n");
            }

            stringBuilder.AppendLine("        protected override void LoadBytesInfo()");
            stringBuilder.AppendLine("        {");
            //stringBuilder.AppendLine("            Utils.LoadXMLByBundle(\"" + className + ".xml\", loadData, CustPackageName.DataTable, CustPackageName.PlatformLobby);");
            //stringBuilder.AppendLine("            Utils.LoadXMLByBundleThread(\"" + className + ".xml\", loadData, CustPackageName.DataTable, CustPackageName.PlatformLobby);");

            //��ʱ û�а�ab���������
            stringBuilder.AppendLine("           string xmlPath = Application.dataPath + \"" + xmlfilepath + className + ".xml\";");
            stringBuilder.AppendLine("            XmlDocument xmlDoc = new XmlDocument();");
            stringBuilder.AppendLine("           xmlDoc.Load(xmlPath);");
            stringBuilder.AppendLine("             string xmlText = xmlDoc.InnerXml;");
            stringBuilder.AppendLine("            XmlReaderSettings settings = new XmlReaderSettings();");
            stringBuilder.AppendLine("           settings.IgnoreComments = true;");
            stringBuilder.AppendLine("           settings.IgnoreWhitespace = true;");
            stringBuilder.AppendLine("           settings.Async = true;");
            stringBuilder.AppendLine("           loadData(XmlReader.Create(new StringReader(xmlText), settings));");
            //
            stringBuilder.AppendLine("        }");
            stringBuilder.Append("\n");
            stringBuilder.AppendLine("        protected override void loadDataInfo(XmlDocument doc)");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            XmlNode xn = doc.SelectSingleNode(\"" + AllCsHead + className + "\");");
            stringBuilder.AppendLine("            XmlNodeList xnl = xn.ChildNodes;");
            stringBuilder.AppendLine("            for (int i = 0; i < xnl.Count; i++)");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                 " + className + " data = new " + className + "();");
            stringBuilder.AppendLine("                 XmlElement xe = (XmlElement)xnl[i];");
            for (int i = 0; i < names.Length; i++) {
                string type = types[i];
                if (type == "int") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= int.Parse(xe.GetAttribute(\"" + names[i] + "\"));");
                }
                else if (type == "float") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= float.Parse(xe.GetAttribute(\"" + names[i] + "\"));");
                }
                else if (type == "string") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= xe.GetAttribute(\"" + names[i] + "\").ToString();");

                    stringBuilder.AppendLine("                 if(data." + names[i] + " == \"0\")");
                    stringBuilder.AppendLine("                     data." + names[i] + " = string.Empty;");
                }
                else if (type == "bool") {
                    stringBuilder.AppendLine("                 data." + names[i] + "= bool.Parse(xe.GetAttribute(\"" + names[i] + "\"));");
                }
            }

            stringBuilder.AppendLine("                 lock (datas)");
            stringBuilder.AppendLine("                 {");
            stringBuilder.AppendLine("                     datas.Add(data);");
            stringBuilder.AppendLine("                 };");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string csPath = CsClassPath + "/" + className + ".cs";
            if (File.Exists(csPath)) {
                File.Delete(csPath);
            }

            using (StreamWriter sw = new StreamWriter(csPath)) {
                sw.Write(stringBuilder);
                Debug.Log("����:" + csPath);
            }
        }
        catch (System.Exception e) {
            Debug.LogError("д��CSʧ��:" + e.Message);
            throw;
        }
    }

    static void WriteXml(string className, string[] names, string[] types, List<string[]> datasList) {
        try {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            stringBuilder.AppendLine("<" + AllCsHead + className + ">");
            //stringBuilder.AppendLine("<" + className + "s>");
            for (int d = 0; d < datasList.Count; d++) {
                stringBuilder.Append("\t<" + className + " ");
                //��������
                string[] datas = datasList[d];
                //������Խڵ�
                for (int c = 0; c < datas.Length; c++) {
                    string type = types[c];
                    if (!type.Contains("[]")) {
                        string name = names[c];
                        string value = datas[c];
                        value = ValueReplaceLinefeed(value);
                        stringBuilder.Append(name + "=\"" + value + "\"" + (c == datas.Length - 1 ? "" : " "));
                    }
                }

                stringBuilder.Append(">\n");
                //�����Ԫ�ؽڵ�(���������ֶ�)
                for (int c = 0; c < datas.Length; c++) {
                    string type = types[c];
                    if (type.Contains("[]")) {
                        string name = names[c];
                        string value = datas[c];
                        string[] values = value.Split(ArrayTypeSplitChar);
                        stringBuilder.AppendLine("\t\t<" + name + ">");
                        for (int v = 0; v < values.Length; v++) {
                            stringBuilder.AppendLine("\t\t\t<item>" + ValueReplaceLinefeed(values[v]) + "</item>");
                        }

                        stringBuilder.AppendLine("\t\t</" + name + ">");
                    }
                }

                stringBuilder.AppendLine("\t</" + className + ">");
            }

            //stringBuilder.AppendLine("</" + className + "s>");
            stringBuilder.AppendLine("</" + AllCsHead + className + ">");

            string xmlPath = XmlDataPath + "/" + className + ".xml";
            if (File.Exists(xmlPath)) {
                File.Delete(xmlPath);
            }

            using (StreamWriter sw = new StreamWriter(xmlPath)) {
                sw.Write(stringBuilder);
                Debug.Log("�����ļ�:" + xmlPath);
            }
        }
        catch (System.Exception e) {
            Debug.LogError("д��Xmlʧ��:" + e.Message);
        }
    }

    static string ValueReplaceLinefeed(string value) {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        return value.Replace("\\n", "&#x000A;");
    }
}