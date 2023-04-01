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