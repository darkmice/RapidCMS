﻿using System;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models
{
    internal class ExpressionField : Field
    {
        public ExpressionField(IExpressionMetadata expression)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public ExpressionField(IPropertyMetadata expression)
        {
            Expression = PropertyMetadataHelper.GetExpressionMetadata(expression ?? throw new ArgumentNullException(nameof(expression)));
        }
        
        internal DisplayType DisplayType { get; set; } = DisplayType.Label;
        internal IExpressionMetadata Expression { get; set; }
    }
}
