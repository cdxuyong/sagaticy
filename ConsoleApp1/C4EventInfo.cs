using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPC.Importor.Models
{
    /*
     * C4_event	samplled_time	line_name	pole_direction	position_name	km_mark	pole_no
     */ 
     /// <summary>
     /// C4文件导入对象
     /// </summary>
    public class C4EventInfo
    {
        public string Id { get; set; }
        public string SamplledTime { get; set; }
        public string LineCode { get; set; } = string.Empty;
        public string LineName { get; set; } = string.Empty;
        public string PoleDirection { get; set; }
        public string PositionName { get; set; }
        public string PositionCode { get; set; } = string.Empty;
        public int KmMark { get; set; }
        public string PoleNo { get; set; }
        public string PoleCode { get; set; } = string.Empty;

        public string BrgTunCode { get; set; } = string.Empty;
        public string BrgTunName { get; set; } = string.Empty;

        public string BureauCode { get; set; } = string.Empty;
        public string BureauName { get; set; } = string.Empty;

        public string PowerSecCode { get; set; } = string.Empty;
        public string PowerSecName { get; set; } = string.Empty;

        public string WorkshopCode { get; set; } = string.Empty;
        public string WorkshopName { get; set; } = string.Empty;

        public string OrgCode { get; set; } = string.Empty;
        public string OrgName { get; set; } = string.Empty;

        public string Path { get; set; }

        public string FilePrex { get; set; }
        /// <summary>
        /// mdb文件名（一次区站数据）
        /// </summary>
        public string FileName { get; set; } = string.Empty;
        /// <summary>
        /// 存储所有图片名称
        /// </summary>
        public string AllImages { get; set; }
        /// <summary>
        /// 正面接触悬挂
        /// </summary>
        public List<string> FrontHangs { get; set; } = new List<string>();
        /// <summary>
        /// 反面接触悬挂
        /// </summary>
        public List<string> BackHangs { get; set; } = new List<string>();
        /// <summary>
        /// 一次巡检名称
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 巡检开始日期
        /// </summary>
        public DateTime SDate { get; set; }
        /// <summary>
        /// 分析用时毫秒
        /// </summary>
        public long AnalysisMs { get; set; }
        /// <summary>
        /// 数据状态
        /// 0 未处理 1已处理
        /// </summary>
        public int DataState { get; set; }

        /// <summary>
        /// 分段任务id
        /// </summary>
        public string TaskId { get; set; }
        /// <summary>
        /// 检测计划任务id
        /// </summary>
        public string DetectId { get; set; }
        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; }
        /// <summary>
        /// 检测时间日期
        /// </summary>
        public DateTime _dt { get; set; }

    }
}
