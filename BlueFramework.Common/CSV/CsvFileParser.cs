#region title
// ==============================================================================
// 公用组件:CSV文件解析类
// ==============================================================================
// CSV文件解析类 
// author:xuyong
// date:2018/1/22
// History:2018/1/22 create
// ==============================================================================
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace BlueFramework.Common.CSV
{
    /// <summary>
    /// CSV文件解析类
    /// <remarks>提供CSV文件的读取方法，支持引号和不同分隔符分隔</remarks>
    /// </summary>
    public class CsvFileParser
    {
        #region fields
        private char Separator = ','; //分隔符
        private Encoding FileEncoding = System.Text.Encoding.Default;
        private bool IgnoreQuotes = false; //是否忽略双引号，文件解析第一行数据时初始化该值；如果忽略双引号，则直接用逗号分隔。
        const string DEFAULT_ENCODING = "gb2312";
        private int CurrentIndex = 0;//当前读取行
        private string SeparatorString = string.Empty;//分隔符字符串
        private string SeparatorQuotesString = string.Empty;//引号分隔符
        #endregion

        #region construct
        /// <summary>
        /// CSV文件解析类
        /// </summary>
        public CsvFileParser()
        {
            init();
        }

        /// <summary>
        /// CSV文件解析类
        /// </summary>
        /// <param name="separator">分隔符</param>
        public CsvFileParser(char separator)
        {
            this.Separator = separator;
            init();
        }

        /// <summary>
        /// CSV文件解析类
        /// </summary>
        /// <param name="separator">分隔符</param>
        /// <param name="fileEncoding">文件编码</param>
        /// <param name="ignoreQuotes">是否忽略双引号，如果是否系统会从第一行数据识别是否有引号，否则直接用分隔符处理</param>
        public CsvFileParser(char separator, Encoding fileEncoding,bool ignoreQuotes=false)
        {
            this.Separator = separator;
            this.FileEncoding = fileEncoding;
            this.IgnoreQuotes = ignoreQuotes;
            init();
        }

        /// <summary>
        /// CSV文件解析类
        /// </summary>
        /// <param name="fileEncoding">文件编码类型</param>
        public CsvFileParser(Encoding fileEncoding)
        {
            this.FileEncoding = fileEncoding;
            init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        void init()
        {
            SeparatorString = Separator.ToString();
            SeparatorQuotesString = "\"" + Separator;
        }
        #endregion

        #region public method
        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件数据流</returns>
        protected StreamReader LoadFile(string filePath)
        {
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(filePath, FileEncoding);
            }
            catch (IOException ex)
            {
                info(string.Format("{0} has IOException： {1}", filePath, ex.Message));
            }
            catch (Exception ex)
            {
                info(string.Format("{0} has  Exception： {1}", filePath, ex.Message));
            }
            return reader;
        }

        /// <summary>
        /// 读取并解析CSV文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="message">文件解析时的异常消息</param>
        /// <returns>成功返回数据集，失败为空</returns>
        public DataTable TryParse(string filePath, out string message)
        {
            StreamReader reader = LoadFile(filePath);
            message = string.Empty;
            if (reader == null)
            {
                message = "读取文件发生异常，请查看日志文件。";
                return null;
            }
            DataTable table = TryParse(reader);
            reader.Close();
            //reader.Dispose();
            if (table == null)
            {
                message = "解析数据流是发生异常，请查看日志文件";
            }
            return table;
        }

        /// <summary>
        /// 解析CSV的文件流
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="message">文件解析时的异常消息</param>
        /// <returns>成功返回数据集，失败为空</returns>
        public DataTable TryParse(Stream fileStream, out string message)
        {
            StreamReader reader = null;
            message = string.Empty;
            DataTable dt = null;
            try
            {
                reader = new StreamReader(fileStream, FileEncoding);
                dt = Parse(reader);
            }
            catch (ArgumentException ex)
            {
                message = string.Format("stream to StreamReader  has  ArgumentException： {0} ,rowindex in {1}", ex.Message, CurrentIndex);
            }
            catch (IOException ex)
            {
                message = string.Format("stream parse to table  has  IOException： {0} ,rowindex in {1}", ex.Message, CurrentIndex);
            }
            catch (Exception ex)
            {
                message = string.Format("stream parse to table  has  Exception： {0} ,rowindex in {1}", ex.Message, CurrentIndex);
            }
            finally
            {
                reader.Close();
                //reader.Dispose();
            }
            return dt;
        }

        /// <summary>
        /// 解析文件流
        /// </summary>
        /// <param name="streamReader">内存文件流</param>
        /// <returns>成功返回数据集，失败返回空，并写入日志</returns>
        public DataTable TryParse(StreamReader streamReader)
        {
            DataTable table = null;
            try
            {
                table = Parse(streamReader);
            }
            catch (IOException ex)
            {
                info(string.Format("parse csv had IOException： {0}", ex.Message));
            }
            catch (OutOfMemoryException ex)
            {
                info(string.Format("parse csv had OutOfMemoryException： {0}", ex.Message));
            }
            catch (Exception ex)
            {
                info(string.Format("parse csv had Exception： {0}", ex.Message));
            }
            return table;
        }

        /// <summary>
        /// 读取并解析数据流
        /// </summary>
        /// <param name="streamReader">数据流</param>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <returns>成功返回数据集，失败抛出系统异常</returns>
        public DataTable Parse(StreamReader streamReader)
        {
            string line;
            string[] cells = null;
            DataTable table = new DataTable();
            CurrentIndex = 0;

            #region [读取表头]
            line = streamReader.ReadLine();
            if (!string.IsNullOrEmpty(line))
            {
                cells = SplitLine(line);
                for (int i = 0; i < cells.Length; i++)
                {
                    DataColumn dc = new DataColumn(cells[i]);
                    table.Columns.Add(dc);
                }
            }
            #endregion

            #region [第一行数据处理]
            if (streamReader.BaseStream.CanRead)
            {
                line = streamReader.ReadLine();
            }
            if (this.IgnoreQuotes && !string.IsNullOrEmpty(line))
            {
                if (line.IndexOf('"')>-1)
                {
                    this.IgnoreQuotes = false;
                }
            }
            #endregion

            #region 读取所有数据行
            string multiLine = string.Empty;
            bool isAppend = false; //是否增加当前数据
            while (true)
            {
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }
                // 判断需要追加数据
                if (!isAppend)
                {
                    multiLine = line;
                    isAppend = IsMultiLine(line);
                }
                else
                {
                    isAppend = IsEndInLine(line)?false:true;
                    multiLine = multiLine + System.Environment.NewLine + line;
                }

                if (!isAppend)
                {
                    cells = SplitLine(multiLine);
                    table.Rows.Add(cells);
                    multiLine = string.Empty;
                    isAppend = false;
                    CurrentIndex++;
                }
                // 读取数据行
                if (streamReader.BaseStream.CanRead)
                {
                    line = streamReader.ReadLine();
                }
                else
                {
                    line = string.Empty;
                }
            }
            #endregion

            return table;
        }

        /// <summary>
        /// 获取文件读取的最后行
        /// </summary>
        /// <returns></returns>
        public int GetLastRowIndex()
        {
            return this.CurrentIndex;
        }

        /// <summary>
        /// 获取CSV数据行分隔后的字符串数组
        /// </summary>
        /// <param name="cellsString">由分隔符组成的CSV数据行</param>
        /// <exception cref="System.OutOfMemoryException"></exception>
        /// <exception cref="System.Exception"></exception>
        /// <returns>返回数组</returns>
        public string[] SplitLine(string cellsString)
        {
            if(this.IgnoreQuotes)
                return cellsString.Split(this.Separator);
            else
                return Split(cellsString);
        }


        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
            byte curByte; //当前分析的字节. 
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前 
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1 
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }

        /// <summary>
        /// 获取文件的编码
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>返回文件编码</returns>
        public static System.Text.Encoding GetEncoding(string filename)
        {
            FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Encoding r = GetEncoding(fileStream);
            fileStream.Close();
            return r;
        }

        /// <summary>
        /// 获取数据流编码
        /// </summary>
        /// <param name="fileStream">数据流</param>
        /// <returns></returns>
        public static System.Text.Encoding GetEncoding(Stream fileStream)
        {
            Encoding reVal = Encoding.Default;
            try
            {
                byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
                byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
                byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM 

                BinaryReader r = new BinaryReader(fileStream, System.Text.Encoding.Default);
                int i = 0;
                int.TryParse(fileStream.Length.ToString(), out i);
                byte[] ss = r.ReadBytes(i);
                if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
                {
                    reVal = Encoding.UTF8;
                }
                else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
                {
                    reVal = Encoding.BigEndianUnicode;
                }
                else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
                {
                    reVal = Encoding.Unicode;
                }
                r.Close();
            }
            catch
            {
                
                //UltraPower.Common.Log4Helper.Info(typeof(CsvFileParser), string.Format("[警告]文件编码判断时发出异常，采用默认编码{0}：{1}",reVal.EncodingName,ex.Message ));                          
            }
            return reVal;

        }

        /// <summary>
        /// 获取数据表格的CSV文本
        /// </summary>
        /// <param name="table">CSV数据表</param>
        /// <param name="isIncludeHeader">是否输出列头</param>
        /// <param name="addQuotes">数据单元格是否增加引号包括</param>
        /// <returns></returns>
        public string GetText(DataTable table, bool isIncludeHeader=true,bool addQuotes=false)
        {
            if (table == null)
            {
                return string.Empty;
            }
            string lineString = string.Empty;
            StringBuilder sbText = new StringBuilder();
            // 输出列头
            if (isIncludeHeader)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    lineString += table.Columns[i].ColumnName.ToString();
                    if (i < table.Columns.Count - 1)
                    {
                        lineString += this.Separator;
                    }
                    else
                    {
                        lineString += System.Environment.NewLine;
                    }
                }
                sbText.Append(lineString);
            }
            //输出行数据
            for (int i = 0; i < table.Rows.Count; i++)
            {
                lineString = string.Empty;
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    if(addQuotes)
                        lineString += "\""+ table.Rows[i][j].ToString()+"\"";
                    else
                        lineString += table.Rows[i][j].ToString();
                    if (j < table.Columns.Count - 1)
                    {
                        lineString += this.Separator;
                    }
                    else
                    {
                        lineString += System.Environment.NewLine;
                    }
                }
                sbText.Append(lineString);
            }
            return sbText.ToString();
        }

        /// <summary>
        /// 将数据表保存为CSV文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="csvTable">CSV数据表</param>
        /// <param name="append">是否追加到已有文件</param>
        /// <returns></returns>
        public bool Save(string filePath, DataTable csvTable, bool append = false)
        {
            return Save(filePath,csvTable,false,append);
        }

        /// <summary>
        /// 将数据表保存为CSV文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="csvTable">CSV数据表</param>
        /// <param name="addQuotes">是否增加引号包入数据</param>
        /// <param name="append">是否追加到已有文件</param>
        /// <returns>成功返回true,否则false</returns>
        public bool Save(string filePath, DataTable csvTable,bool addQuotes=false, bool append=false)
        {
            bool success = false;
            string content = GetText(csvTable,!append,addQuotes);
            byte[] myByte = System.Text.Encoding.GetEncoding(DEFAULT_ENCODING).GetBytes(content);
            FileStream fileStream = null;
            try
            {
                info("format file's path：" + filePath);
                filePath = System.IO.Path.GetFullPath(filePath);
                info("save table to file：" + filePath);

                if (append)
                    fileStream = new FileStream(filePath, FileMode.Append);
                else
                    fileStream = new FileStream(filePath, FileMode.Create);
                fileStream.Write(myByte, 0, myByte.Length);
                success = true;
            }
            catch(Exception ex)
            {
                info(filePath + " save error：" +ex.Message);
            }
            finally
            {
                if(fileStream!=null) fileStream.Close();
            }
            return success;
        }

        #endregion

        #region private methods
        /// <summary>
        /// 返回引号和逗号组合分隔后的数组
        /// </summary>
        /// <param name="cellsString"></param>
        /// <returns>字符串数组</returns>
        private string[] Split(String cellsString)
        {
            int curIndex = 0; //当前字符位置
            int curEnd = 0;//结束字符位置
            int len = cellsString.Length;
            //int qtIndex = -1; //引号和逗号位置
            //int spIndex = -1; //分隔符位置
            List<string> arr = new List<string>();
            while (true)
            {
                if (curIndex >= len) break;
                if (cellsString.Substring(curIndex, 1) == "\"")
                {//如果是双引号开始
                    curIndex = curIndex + 1;
                    curEnd = cellsString.IndexOf(this.SeparatorQuotesString, curIndex);
                    if (curEnd == -1) curEnd = len - 1;
                    if (curEnd == -1) break;
                    arr.Add(cellsString.Substring(curIndex, curEnd - curIndex));
                    curIndex = curEnd + 2;
                }
                else
                {//如果是非双引号开始
                    curEnd = cellsString.IndexOf(this.Separator, curIndex);
                    if (curEnd == -1) curEnd = len;
                    arr.Add(cellsString.Substring(curIndex, curEnd - curIndex));
                    curIndex = curEnd + 1;
                }
            }
            return arr.ToArray();
        }

        /// <summary>
        /// 判断是否为多行数据
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool IsMultiLine(string line)
        {
            int lastQuote = line.LastIndexOf('\"');
            if (lastQuote == -1) return false;
            if (lastQuote == 0) return true;
            if (line.Substring(lastQuote - 1, 1) == this.SeparatorString)
            {
                if (lastQuote == line.Length - 1)
                    return false;
                else
                    return true;
            }
            else
            {
                return false;
            }


        }

        /// <summary>
        /// 判断多行数据是否结束
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool IsEndInLine(string line)
        {
            int len = line.Length;
            int lastQuote = line.LastIndexOf('"');
            if (lastQuote == len - 1||lastQuote==0) return true;
            if (lastQuote == -1) return false;
            if (IsMultiLine(line)) return false;
            return true;
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="message"></param>
        void info(string message)
        {
            //UltraPower.Common.Log4Helper.Info(typeof(CsvFileParser), message);
        }
        #endregion
    }
}
