using System;

namespace Publish2Local
{
    public  class Complication
    {
        public const string Spot = "WT";
        public static string SpotName = "";
        public static string Version = "2.3.3";

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
                SpotName = "乐山新纪元人才服务有限公司";
            }
        }
    }
}
