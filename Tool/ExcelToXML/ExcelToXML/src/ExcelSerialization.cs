using OfficeOpenXml;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

public enum EValueBelong
{
    Basic = 0,          //基本归属（客户端服务器都需要使用）
    Client = 1,          //仅客户端使用
    Server = 2,          //仅服务器端使用
    ConfigDes = 3,      //仅用作配置描述(客户端服务器都不用)
}

public enum EValueType
{
    Str = 0,
    Int = 1,
    Float = 2,
    Bool,
    V2,
    V3,
}


public class ExcelSerialization
{
    public const int EXCEL_PROPERTY_TYPE_ROW = 4;          //第几行定义了值类型
    public const int EXCEL_PROPERTY_BELONG_ROW = 3;          //第几行定义了属性归属(服务器或者客户端读的)
    public const int EXCEL_PROPERTY_NAME_ROW = 5;          //第几行定义了属性名
    public const int ID_ROW = 2;       //第几列为ID值

    public const int EXCEL_DATA_COL_START = 2;        //从第几列开始有正式数据（下标从1开始）
    public const int EXCEL_DATA_ROW_START = 6;        //从第几行开始有正式数据


    public static bool SerExcel2XML(string excelPath, string outPath)
    {
        try
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string cfgName = string.Empty;
            string cfgPath = string.Empty;
            using (ExcelPackage package = new ExcelPackage(excelPath))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                cfgName = workSheet.Cells[1, 1].Value.ToString();
                if (cfgName.Equals("temp", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                cfgPath = workSheet.Cells[1, 2].Value.ToString();
                string savingPath = Path.Combine(outPath, cfgPath + cfgName + ".xml");
                if (File.Exists(savingPath))
                {
                    File.Delete(savingPath);
                }
                object[,] validCells = workSheet.Cells.Value as object[,];
                int totalRow = validCells.GetUpperBound(0) + 1;
                int totalColu = validCells.GetUpperBound(1) + 1;

                int validColCount = totalColu - (EXCEL_DATA_COL_START - 1);

                //记录属性名及属性归属
                EValueBelong[] belongs = new EValueBelong[validColCount];
                string[] propertyNames = new string[validColCount];

                //从有数据列开始从左往右读取每个数据的属性值归属        
                for (int col = EXCEL_DATA_COL_START; col <= totalColu; col++)
                {
                    int curIndex = col - EXCEL_DATA_COL_START;
                    var belongCell = workSheet.Cells[EXCEL_PROPERTY_BELONG_ROW, col];

                    EValueBelong belong = EValueBelong.ConfigDes;
                    if (belongCell.Value != null)
                    {
                        int iB = int.Parse(belongCell.Value.ToString());
                        belong = (EValueBelong)iB;
                    }
                    belongs[curIndex] = belong;

                    var cellPN = workSheet.Cells[EXCEL_PROPERTY_NAME_ROW, col];
                    string propertyName = cellPN.Value == null ? "" : cellPN.Value.ToString();
                    propertyNames[curIndex] = propertyName;
                }

                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmlDoc.AppendChild(declaration);
                xmlDoc.AppendChild(xmlDoc.CreateElement("XML"));
                for (int row = EXCEL_DATA_ROW_START; row <= totalRow; row++)
                {
                    object idValue = workSheet.Cells[row, ID_ROW].Value;
                    string id = idValue == null ? string.Empty : idValue.ToString();
                    if (string.IsNullOrEmpty(id))
                    {
                        continue;
                    }
                    XmlElement data = xmlDoc.CreateElement("Data");
                    for (int col = EXCEL_DATA_COL_START; col <= totalColu; col++)
                    {
                        int colIndex = col - EXCEL_DATA_COL_START;

                        string propertyName = propertyNames[colIndex];         //属性名字
                        propertyName = propertyName.Trim();     //容错处理

                        //属性名为空则不处理
                        if (string.IsNullOrEmpty(propertyName))
                        {
                            continue;
                        }

                        if (belongs[colIndex] != EValueBelong.Basic && belongs[colIndex] != EValueBelong.Client)
                        {
                            continue;
                        }
                        object objValue = workSheet.Cells[row, col].Value;
                        string value = objValue == null ? string.Empty : objValue.ToString();
                        if (value  != string.Empty)
                        {
                            data.SetAttribute(propertyName, value);       //xml write
                        }

                    }

                    xmlDoc.DocumentElement.AppendChild(data);
                }
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.OmitXmlDeclaration = false;
                settings.NewLineOnAttributes = false;
                settings.Encoding = new UTF8Encoding(false);

                using (var writer =  XmlTextWriter.Create(savingPath, settings))
                {
                    xmlDoc.Save(writer);
                }
               // xmlDoc.Save(savingPath);

                return true;
            }
        }
        catch (Exception e)
        {
            Console.Write(e.Message);
            MessageBox.Show(e.Message, "出错");
            return false;
        }
    }

    public static bool SerExcel2Lua(string excelPath, string luaPath, string luaTemp)
    {
        try
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            StringBuilder luaTableBuilder = new StringBuilder();
            StringBuilder luaIDListBuilder = new StringBuilder();
            string cfgName = string.Empty;
            int dataCount = 0;
            using (ExcelPackage package = new ExcelPackage(excelPath))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                cfgName = workSheet.Cells[1, 1].Value.ToString();
                cfgName = cfgName.ToLower();
                if (cfgName.Equals("temp", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                string savingPath = Path.Combine(luaPath, cfgName + "_cfg.lua");
                if (File.Exists(savingPath))
                {
                    File.Delete(savingPath);
                }
                object[,] validCells = workSheet.Cells.Value as object[,];
                int totalRow = validCells.GetUpperBound(0) + 1;
                int totalColu = validCells.GetUpperBound(1) + 1;

                int validColCount = totalColu - (EXCEL_DATA_COL_START - 1);

                //记录属性名及属性归属
                EValueBelong[] belongs = new EValueBelong[validColCount];
                string[] propertyNames = new string[validColCount];


                EValueType[] valueType = new EValueType[validColCount];

                //从有数据列开始从左往右读取每个数据的属性值归属        
                for (int col = EXCEL_DATA_COL_START; col <= totalColu; col++)
                {
                    int curIndex = col - EXCEL_DATA_COL_START;
                    var belongCell = workSheet.Cells[EXCEL_PROPERTY_BELONG_ROW, col];

                    EValueBelong belong = EValueBelong.ConfigDes;
                    if (belongCell.Value != null)
                    {
                        int iB = int.Parse(belongCell.Value.ToString());
                        belong = (EValueBelong)iB;
                    }
                    belongs[curIndex] = belong;

                    var cellPN = workSheet.Cells[EXCEL_PROPERTY_NAME_ROW, col];
                    string propertyName = cellPN.Value == null ? "" : cellPN.Value.ToString();
                    propertyNames[curIndex] = propertyName;

                    var valueTypeCell = workSheet.Cells[EXCEL_PROPERTY_TYPE_ROW, col];
                    if (valueTypeCell.Value != null)
                    {
                        int vType = int.Parse(valueTypeCell.Value.ToString());
                        valueType[curIndex] = (EValueType)vType;
                    }
                }

                for (int row = EXCEL_DATA_ROW_START; row <= totalRow; row++)
                {
                    object idValue = workSheet.Cells[row, ID_ROW].Value;
                    string id = idValue == null ? string.Empty : idValue.ToString();
                    if (string.IsNullOrEmpty(id))
                    {
                        continue;
                    }

                    //判断ID的类型
                    EValueType idType = valueType[0];

                    luaTableBuilder.Append("\t\t[");
                    if (idType != EValueType.Float || idType != EValueType.Int)
                    {
                        luaTableBuilder.Append("\"");
                    }
                    luaTableBuilder.Append(id);
                    if (idType != EValueType.Float || idType != EValueType.Int)
                    {
                        luaTableBuilder.Append("\"");
                    }
                    luaTableBuilder.Append("] = {");

                    luaIDListBuilder.Append("\t\t");
                    if (idType != EValueType.Float || idType != EValueType.Int)
                    {
                        luaIDListBuilder.Append("\"");
                    }
                    luaIDListBuilder.Append(id);
                    if (idType != EValueType.Float || idType != EValueType.Int)
                    {
                        luaIDListBuilder.Append("\"");
                    }
                    luaIDListBuilder.Append(",\n");

                    for (int col = EXCEL_DATA_COL_START; col <= totalColu; col++)
                    {
                        int colIndex = col - EXCEL_DATA_COL_START;

                        string propertyName = propertyNames[colIndex];         //属性名字
                        propertyName = propertyName.Trim();     //容错处理

                        //属性名为空则不处理
                        if (string.IsNullOrEmpty(propertyName))
                        {
                            continue;
                        }
                        //if (propertyName.Equals("ID", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    propertyName = "ConfigID";
                        //}

                        if (belongs[colIndex] != EValueBelong.Basic && belongs[colIndex] != EValueBelong.Client)
                        {
                            continue;
                        }
                        object objValue = workSheet.Cells[row, col].Value;
                        string value = objValue == null ? string.Empty : objValue.ToString();


                        luaTableBuilder.Append("\n\t\t\t");
                        luaTableBuilder.Append(propertyName);
                        luaTableBuilder.Append(" = ");

                        //luabuilder
                        switch (valueType[colIndex])
                        {
                            case EValueType.V2:
                            case EValueType.V3:
                                string[] vec = value.Split(',');

                                luaTableBuilder.Append("{\n\t\t\t\tx = ");
                                luaTableBuilder.Append(vec[0]);
                                luaTableBuilder.Append(",");
                                luaTableBuilder.Append("\n\t\t\t\ty = ");
                                luaTableBuilder.Append(vec[1]);
                                luaTableBuilder.Append(",");

                                if (vec.Length == 3)
                                {
                                    luaTableBuilder.Append("\n\t\t\t\tz = ");
                                    luaTableBuilder.Append(vec[2]);
                                    luaTableBuilder.Append(",");
                                }

                                luaTableBuilder.Append("\n\t\t\t}");
                                break;
                            case EValueType.Str:
                                luaTableBuilder.Append("\"");
                                luaTableBuilder.Append(value);
                                luaTableBuilder.Append("\"");
                                break;
                            case EValueType.Int:
                            case EValueType.Float:
                                if (string.IsNullOrEmpty(value))
                                {
                                    luaTableBuilder.Append(0);
                                }
                                else
                                {
                                    luaTableBuilder.Append(value);
                                }
                                break;
                            case EValueType.Bool:
                                if (value.Equals("1", StringComparison.OrdinalIgnoreCase) || value.Equals("true", StringComparison.OrdinalIgnoreCase))
                                {
                                    luaTableBuilder.Append("true");
                                }
                                else
                                {
                                    luaTableBuilder.Append("false");
                                }
                                break;
                        }
                        luaTableBuilder.Append(",");
                    }
                    luaTableBuilder.Append("\n\t\t},\n");
                    dataCount++;
                }

                //移除换行符
                luaTableBuilder.Remove(luaTableBuilder.Length - 1, 1);
                luaIDListBuilder.Remove(luaIDListBuilder.Length - 1, 1);

                //生成配置
                string excelName = string.Empty;
                excelName = excelPath.Replace("/", "\\");
                int subIndex = excelName.LastIndexOf("\\");
                excelName = excelName.Substring(subIndex + 1);
                luaTemp = luaTemp.Replace("#EXCEL#", excelName);
                luaTemp = luaTemp.Replace("#NAME#", cfgName + "_cfg");
                luaTemp = luaTemp.Replace("#IDLIST#", luaIDListBuilder.ToString());
                luaTemp = luaTemp.Replace("#COUNT#", dataCount.ToString());
                luaTemp = luaTemp.Replace("#CONTENT#", luaTableBuilder.ToString());

                //加载Lua模版配置
                //保存到当前目录
                File.Create(savingPath).Dispose();
                File.WriteAllText(savingPath, luaTemp);
                return true;
            }
        }
        catch (Exception e)
        {
            Console.Write(e.Message);
            MessageBox.Show(e.Message, "出错");
            return false;
        }
    }


}
