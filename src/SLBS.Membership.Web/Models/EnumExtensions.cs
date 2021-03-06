﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace SLBS.Membership.Web.Models
{
    public static class EnumExtensions
        {
            public static string GetDescription<T>(this T enumerationValue) where T : struct
            {
                var type = enumerationValue.GetType();
                if (!type.IsEnum)
                {
                    return string.Empty;
                }
                var memberInfo = type.GetMember(enumerationValue.ToString());
                if (memberInfo.Length > 0)
                {
                    var attrs = memberInfo[0].GetCustomAttributes(typeof (DescriptionAttribute), false);
                    if (attrs.Length > 0)
                    {
                        return ((DescriptionAttribute) attrs[0]).Description;
                    }
                }
                return enumerationValue.ToString();
            }
        }
}