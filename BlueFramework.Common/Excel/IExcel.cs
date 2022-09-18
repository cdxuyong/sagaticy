using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace BlueFramework.Common.Excel
{
    /// <summary>
    /// excel read/write
    /// </summary>
    public interface IExcel
    {

        /// <summary>
        /// read <see cref="System.Data.DataSet"/> from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        DataSet Read(Stream stream);

        /// <summary>
        /// read <see cref="DataSet"/> from stream excel after rownum
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="rownum"></param>
        /// <returns></returns>
        DataSet Read(Stream stream, int rownum);

        /// <summary>
        /// read <see cref="System.Data.DataSet"/> from excel file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        DataSet Read(string filePath);

        /// <summary>
        /// read <see cref="System.Data.DataTable"/> from excel file's sheet that is limit by regin
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="dataIndex"></param>
        /// <returns></returns>
        DataTable Read(string filePath, int sheetIndex, int dataIndex);

        /// <summary>
        /// write <see cref="System.Data.DataSet"/> to <see cref="System.IO.Stream"/>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="ds"></param>
        /// <param name="extendType"><see cref="ExcelExtendType"/></param>
        void Write(Stream stream, DataSet ds,ExcelExtendType extendType);

        /// <summary>
        /// write <see cref="System.Data.DataSet"/> to <see cref="System.IO.Stream"/>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="template"></param>
        /// <param name="ds"></param>
        /// <param name="extendType"></param>
        void Write(Stream stream,TTemplate template,DataSet ds, ExcelExtendType extendType);

        /// <summary>
        /// write <see cref="System.Data.DataSet"/> to <see cref="System.IO.Stream"/>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="templatePath"></param>
        /// <param name="ds"></param>
        /// <param name="extendType"></param>
        void Write(Stream stream, string templatePath, DataSet ds, ExcelExtendType extendType);

        /// <summary>
        /// write <see cref="Stream"/> to excel file
        /// </summary>
        /// <param name="outputFilePath"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        bool WriteFile(string outputFilePath, Stream stream);

        /// <summary>
        /// write <see cref="DataSet"/> to excel file
        /// </summary>
        /// <param name="outputFilePath"></param>
        /// <param name="ds"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        bool WriteFile(string outputFilePath, DataSet ds, TTemplate template);

        /// <summary>
        /// WriteFile
        /// </summary>
        /// <param name="outputFilePath"></param>
        /// <param name="ds"></param>
        /// <param name="templateName"></param>
        /// <returns></returns>
        bool WriteFile(string outputFilePath, DataSet ds, string templateName);
    }
}
