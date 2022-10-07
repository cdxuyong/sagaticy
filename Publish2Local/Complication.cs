using System;

namespace Publish2Local
{
    public  class Complication
    {
        public const string Spot = "DEMO";
        public static string SpotName = "";
        public static string Version = "2.2.0";

        static Complication()
        {
            if ("DEMO".Equals(Spot))
            {
                SpotName = "成都市某人才服务有限公司";
            }
            if ("EM".Equals(Spot))
            {
                SpotName = "峨眉山市新世纪人才服务有限公司";
            }
            if ("WT".Equals(Spot))
            {
                SpotName = "乐山市XXXX人才服务有限公司";
            }
        }
    }
}
