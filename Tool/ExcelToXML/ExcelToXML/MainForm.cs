using LitJson;
using System;
using System.IO;
using System.Windows.Forms;

namespace ExcelToXML
{
    public partial class MainForm : Form
    {
        class PathCache
        {
            public string projectPath;
        };

        PathCache cache;

        string jsonFile = "cache.json";

        private string luaFolderPath;
        private string xmlFolderPath;
        private string excelFolderPath;

        private string excelFilePathCache;

        private string luaTempStr;

        public MainForm()
        {
            InitializeComponent();
            if (File.Exists(jsonFile))
            {
                cache = JsonMapper.ToObject<PathCache>(File.ReadAllText(jsonFile));
            }
            else
            {
                cache = new PathCache();
            }
            projectPathInput.Text = cache.projectPath;
            SetOutputPath();
            luaTempStr = File.ReadAllText("LuaCfgTemplate.txt");
        }

        private void SetOutputPath()
        {
            string projectPath = cache.projectPath;
            if (string.IsNullOrEmpty(projectPath))
            {
                return;
            }
            excelFolderPath = projectPath + "/doc/配置表/";
            luaFolderPath = projectPath + "/Client/AzoneClient/Assets/StreamingAssets/Configs/";
            xmlFolderPath = projectPath + "/Client/AzoneClient/Assets/StreamingAssets/Configs/";
        }

        private void SelectProjectPath(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                projectPathInput.Text = cache.projectPath = fbd.SelectedPath;
                SetOutputPath();
                SavingJsonData();
            }
        }

        private void BtnSelectOneExcelFile(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cache.projectPath))
            {
                MessageBox.Show("请先指定项目目录！");
                return;
            }
            OpenFileDialog fbd = new OpenFileDialog();
            fbd.InitialDirectory = excelFolderPath;
            fbd.Multiselect = false;
            fbd.Filter = "*.xls |*.xlsx";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                excelFileInput.Text = excelFilePathCache = fbd.FileName;
            }
        }

        private void GenerateOneXML(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cache.projectPath))
            {
                MessageBox.Show("请先指定项目目录！");
                return;
            }
            bool result = ExcelSerialization.SerExcel2XML(excelFilePathCache, xmlFolderPath);
            if (result)
            {
                MessageBox.Show("生成配置完成");
            }
        }

        private void GenerateAllXML(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cache.projectPath))
            {
                MessageBox.Show("请先指定项目目录！");
                return;
            }
            string[] allFile = Directory.GetFiles(excelFolderPath, "*", SearchOption.AllDirectories);
            bool success = true;
            for (int i = 0; i < allFile.Length; i++)
            {
                string fileName = allFile[i];
                if (fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    bool result = ExcelSerialization.SerExcel2XML(fileName, xmlFolderPath);
                    if (!result)
                    {
                        success = false;
                    }
                }
            }
            if (success)
            {
                MessageBox.Show("生成配置完成");
            }
            else
            {
                MessageBox.Show("生成配置过程中出现错误，请检查！");
            }

        }

        private void GenerateOneLua(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cache.projectPath))
            {
                MessageBox.Show("请先指定项目目录！");
                return;
            }
            bool result = ExcelSerialization.SerExcel2Lua(excelFilePathCache, luaFolderPath, luaTempStr);
            if (result)
            {
                MessageBox.Show("生成配置完成");
            }
        }

        private void GenerateAllLua(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cache.projectPath))
            {
                MessageBox.Show("请先指定项目目录！");
                return;
            }
            bool success = true;
            string[] allFile = Directory.GetFiles(excelFolderPath);
            for (int i = 0; i < allFile.Length; i++)
            {
                string fileName = allFile[i];
                if (fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    bool result = ExcelSerialization.SerExcel2Lua(fileName, luaFolderPath, luaTempStr);
                    if (!result)
                    {
                        success = false;
                    }
                }
            }
            if (success)
            {
                MessageBox.Show("生成配置完成");
            }
            else
            {
                MessageBox.Show("生成配置过程中出现错误，请检查！");
            }
        }

        private void CommitLua(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cache.projectPath))
            {
                MessageBox.Show("请先指定项目目录！");
                return;
            }
            SVNOperation.SVNCommand("commit", luaFolderPath);
        }

        private void CommitXML(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cache.projectPath))
            {
                MessageBox.Show("请先指定项目目录！");
                return;
            }
            SVNOperation.SVNCommand("commit", xmlFolderPath);
        }

        private void SavingJsonData()
        {
            File.Create(jsonFile).Dispose();
            File.WriteAllText(jsonFile, JsonMapper.ToJson(cache));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
