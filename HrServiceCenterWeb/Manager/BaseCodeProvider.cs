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
            using (EntityContext context = Session.CreateContext())
            {
                current.baseCodes = context.SelectList<BaseCodeInfo>("hr.basecode.findBasecodes", null);
                current.positions = context.SelectList<PositionInfo>("hr.basecode.findPositions", null);
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

        
    }
}