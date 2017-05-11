using System;
using System.Collections.Generic;
using System.ComponentModel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace SLBS.Membership.Web.Models
{
    public class NoticeViewModel
    {
        public NoticeViewModel()
        {
            NoticeType = EnumNoticeTypes.PaymentStatus;
        }

        public IEnumerable<Domain.Membership> Receipients { get; set; }
        public EnumNoticeTypes NoticeType { get; set; }

        public string NoticeTypeDisplay
        {
            get { return NoticeType.GetDescription(); }
        }
    }
}