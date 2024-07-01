using System;

namespace Publish2Local
{
    public  class Complication
    {
        public const string Spot = "WT";
        public static string SpotName = "";
        public static string Version = "2.9.3";
        public static string Theme = "base";

        static Complication()
        {
            if ("DEMO".Equals(Spot))
            {
                SpotName = "成都市某人才服务有限公司";
            }
            if ("EM".Equals(Spot))
            {
                SpotName = "峨眉山市新世纪人才服务有限公司";
                Theme = "em";
            }
            if ("WT".Equals(Spot))
            {
                SpotName = "乐山新纪元人才服务有限公司";
            }
        }
    }
}
