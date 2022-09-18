using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.Models
{
    /// <summary>
    /// 发放列表详情
    /// </summary>
    public class PayDetailInfo
    {
        /// <summary>
        /// 人员ID
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// 应发工资-基本工资
        /// </summary>
        public decimal BasicWage { get; set; }

        /// <summary>
        /// 应发工资-辅助工资1
        /// </summary>
        public decimal AidWageOne { get; set; }

        /// <summary>
        /// 应发工资-辅助工资2
        /// </summary>
        public decimal AidWageTwo { get; set; }

        /// <summary>
        /// 应发工资-辅助工资3
        /// </summary>
        public decimal AidWageThree { get; set; }

        /// <summary>
        /// 应发工资-小计
        /// </summary>
        public decimal WageCount { get; set; }

        /// <summary>
        /// 个人应扣-养老
        /// </summary>
        public decimal Pension { get; set; }

        /// <summary>
        /// 个人应扣-失业
        /// </summary>
        public decimal Unemployment { get; set; }

        /// <summary>
        /// 个人应扣-医疗
        /// </summary>
        public decimal Health { get; set; }

        /// <summary>
        /// 个人应扣-小计
        /// </summary>
        public decimal BuckleCount { get; set; }

        /// <summary>
        /// 实发工资
        /// </summary>
        public decimal TrueWage { get; set; }

        /// <summary>
        /// 保险单位部分
        /// </summary>
        public decimal Insurance { get; set; }

        /// <summary>
        /// 公积金单位部分
        /// </summary>
        public decimal Fund { get; set; }

        /// <summary>
        /// 派遣服务费
        /// </summary>
        public decimal ServiceWage { get; set; }

        /// <summary>
        /// 费用总计
        /// </summary>
        public decimal Count { get; set; }
    }
}