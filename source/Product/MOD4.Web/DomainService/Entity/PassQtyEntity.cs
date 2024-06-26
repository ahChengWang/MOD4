﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace MOD4.Web.DomainService.Entity
{
    public class PassQtyEntity
    {
        //public int NodeNo { get; set; }
        public int Node { get; set; }
        public string NodeName { get; set; }
        public List<PassQtyDetailEntity> DetailList { get; set; }
        public int SubTarget
        {
            get
            {
                return this.DetailList.Sum(sum => sum.Target);
            }
            set
            {
                this.SubTarget = 0;
            }
        }
        public int SubTheoreticValue
        {
            get
            {
                return this.DetailList.Sum(sum => sum.TheoreticValue);
            }
            set
            {
                this.SubTheoreticValue = 0;
            }
        }
        public int SubIn
        {
            get
            {
                return this.DetailList.Sum(sum => sum.InQty);
            }
            set
            {
                this.SubIn = 0;
            }
        }
        public int SubOK
        {
            get
            {
                return this.DetailList.Sum(sum => sum.OKQty);
            }
            set
            {
                this.SubIn = 0;
            }
        }
        public int SubNG
        {
            get
            {
                return this.DetailList.Sum(sum => sum.NGQty);
            }
            set
            {
                this.SubIn = 0;
            }
        }
        public string SubYield
        {
            get
            {
                return this.SubIn == 0 ? "0.00%" : (((float)this.SubOK / (float)this.SubIn) * 100).ToString("0.00") + "%";
            }
            set
            {
                this.SubYield = "";
            }
        }
        public string SubDefectRate
        {
            get
            {
                return this.SubIn == 0 ? "0.00%" : (((float)this.SubNG / (float)this.SubIn) * 100).ToString("0.00") + "%";
            }
            set
            {
                this.SubDefectRate = "";
            }
        }
        public int SubDiff
        {
            get
            {
                return this.SubIn - this.SubTarget;
            }
            set
            {
                this.SubDiff = 0;
            }
        }
    }

    public class PassQtyDetailEntity
    {
        public string Time { get; set; }
        public int TheoreticValue
        {
            get
            {
                return this.TimeTarget == 0 ? 0 : Convert.ToInt32(((60 - this.EqHistoryList.Sum(s => Convert.ToDecimal(s.Time))) * 60) / this.TimeTarget);
            }
            set
            {
                this.TheoreticValue = 0;
            }
        }
        public int Target { get; set; }
        public int TimeTarget { get; set; }
        public int InQty { get; set; }
        public int OKQty { get; set; }
        public int NGQty { get; set; }
        public string Yield { get; set; }
        public int Diff { get; set; }
        public string DefectRate { get; set; }
        public string eqpInfoTOP1 { get; set; }
        public string eqpInfoTOP2 { get; set; }
        public string eqpInfoTOP3 { get; set; }

        public List<EqHistory> EqHistoryList { get; set; }
        public string LineTTTOP1 { get; set; }
        public string LineTTTOP2 { get; set; }
    }

    public class EqHistory
    {
        public string Status { get; set; }
        public string Desc { get; set; }
        public string Time { get; set; }
    }

}
