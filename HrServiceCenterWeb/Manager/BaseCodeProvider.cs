using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HrServiceCenterWeb.Models;
using BlueFramework.Blood;

namespace HrServiceCenterWeb.Manager
{
    public class BaseCodeProvider
    {
       
        private static BaseCodeProvider current = null;
        public static BaseCodeProvider Current
        {
            get
            {
                return current;
            }
        }


        private List<BaseCodeInfo> baseCodes = null;
        private List<PositionInfo> positions = null;

        public static void Init()
        {
            current = new BaseCodeProvider();
            current.ReLoad();
        }

        private void ReLoad()
        {
            using (EntityContext context = Session.CreateContext())
            {
                baseCodes = context.SelectList<BaseCodeInfo>("hr.basecode.findBasecodes", null);
                baseCodes.ForEach(x =>
                {
                    if (x.CategoryId == 1) x.CategoryName = "性别";
                    if (x.CategoryId == 2) x.CategoryName = "学历";
                });
                positions = context.SelectList<PositionInfo>("hr.basecode.findPositions", null);
            }
        }

        public BaseCodeProvider()
        {
            baseCodes = new List<BaseCodeInfo>();
        }

        public List<BaseCodeInfo> GetSexCodes()
        {
            List<BaseCodeInfo> codes = baseCodes.Where(o => o.CategoryId == 1 && o.IsLeaf==true).ToList();
            return codes;
        }

        public List<BaseCodeInfo> GetEduciationCodes()
        {
            List<BaseCodeInfo> codes = baseCodes.Where(o => o.CategoryId == 2 && o.IsLeaf == true).ToList();
            return codes;
        }

        /// <summary>
        /// 获取所有编码
        /// </summary>
        /// <returns></returns>
        public List<BaseCodeInfo> GetCodes(string key = "")
        {
            List<BaseCodeInfo> codes = new List<BaseCodeInfo>();
            if (string.IsNullOrEmpty(key))
                codes = baseCodes.OrderBy(x => x.CategoryId).OrderBy(x => x.Code).ToList();
            else
                codes = baseCodes.Where(x => x.CategoryName != null && x.CategoryName.IndexOf(key)>-1).OrderBy(x => x.CategoryId).OrderBy(x => x.Code).ToList();

            return codes;
        }

        public bool SaveBaseCode(BaseCodeInfo codeInfo)
        {
            if(codeInfo==null) return false;
            if(codeInfo.CategoryId==0) return false;
            if(codeInfo.CategoryId==0 || string.IsNullOrEmpty(codeInfo.ObjectValue)) return false;
            using (EntityContext context = Session.CreateContext())
            {
                var succ = true;
                if(codeInfo.Id == 0)
                {
                    codeInfo.Id = baseCodes.Max(x => x.Id) + 1;
                    succ = context.Save("hr.basecode.insertBaseCode", codeInfo);
                }
                else
                {
                    succ = context.Save("hr.basecode.updateBaseCode", codeInfo);
                }
                if (succ)
                {
                    ReLoad();
                }
                return succ;
            }
        }

        public BaseCodeInfo NewCode(int parentId)
        {
            var p = baseCodes.FirstOrDefault(x => x.Id == parentId);
            if(p == null) return null;
            var n = new BaseCodeInfo();
            n.CategoryId = p.CategoryId;
            n.ParentId = parentId;
            n.Id = 0;
            n.Text = "";
            try
            {
                var max = baseCodes.Where(x => x.CategoryId == p.CategoryId).Max(x => int.Parse(x.ObjectValue)) + 1;
                n.ObjectValue = max.ToString();
                n.Code = $"{p.Code}-{n.ObjectValue.PadLeft(3,'0')}";
            }
            catch
            {
            }

            n.IsLeaf = true;
            return n ;
        }

        public List<PositionInfo> GetPositions()
        {
            return this.positions;
        }

        /// <summary>
        /// 新增岗位至基础编码
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddPosition(string name,out string message)
        {
            message = "";
            if (positions.FirstOrDefault(x => x.PositionName.Equals(name)) != null)
            {
                message = "选项已经存在，请刷新页面后从列表中选择";
                return true;
            }

            int id = positions.Max(x => x.PositionId) + 1;
            using (EntityContext context = Session.CreateContext())
            {
                var p = new
                {
                    id = id,
                    name = name
                };
                var succ = context.Save("hr.basecode.insertPosition", p);
                if (succ)
                {
                    ReLoad();
                }
                return succ;
            }
        }
        
    }
}