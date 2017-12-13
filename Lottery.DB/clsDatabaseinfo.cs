using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.DB
{
    public class clTuijianhaomalan_info
    {
        public string wanfazhonglei { get; set; }//玩法种类
        public string tuijianhaoma { get; set; }//推荐号码
        public string nizhuihaoqishu { get; set; }//拟追号期数
        public string dangriqihao { get; set; }//当日期号
        public string zhongjiangqishu { get; set; }//中奖期数


        public DateTime Input_Date { get; set; }
        //新增的标记
        public string xinzeng { get; set; }

        //
        public string haomaileixing { get; set; }

        public string chuxiancishu { get; set; }

        public string pingjunyilou { get; set; }

        public string zuidayilou { get; set; }

        public string diwuyilou { get; set; }

        public string disiyilou { get; set; }

        public string disanyilou { get; set; }

        public string dieryilou { get; set; }

        public string shangciyilou { get; set; }
        public string dangqianyilou { get; set; }
        public string yuchujilv { get; set; }
        //投入金额（利润〉1%）
        public string tourujine { get; set; }
        //中奖金额
        public string zhongjiangjine { get; set; }
        //利润金额
        public string lirunjine { get; set; }


    }
    public class clsJisuanqi_info
    {
        public string qishu { get; set; }//期数 
        public string tourubeishu { get; set; }//投入倍数 
        public string benqitouru { get; set; }//本期投入  
        public string leijitouru { get; set; }//累计投入 
        public string benqishouyi { get; set; }//本期收益  
        public string yilishouyi  { get; set; }//盈利收益  
        public string shouyilv { get; set; }//收益率  
    
        public DateTime Input_Date { get; set; }// 
    }



}
