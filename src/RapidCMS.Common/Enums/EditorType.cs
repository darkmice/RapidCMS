﻿using System;
using RapidCMS.Common.Attributes;

namespace RapidCMS.Common.Enums
{
    public enum EditorType
    {
        None = -99,
        Custom = -1,

        TextBox = 0,
        TextArea,

        Readonly,

        [DefaultType(typeof(int), typeof(long), typeof(uint), typeof(ulong), typeof(int?), typeof(long?), typeof(uint?), typeof(ulong?))]
        Numeric,

        [DefaultType(typeof(bool), typeof(bool?))]
        Checkbox,

        [DefaultType(typeof(DateTime))]
        Date,

        [Relation(RelationType.One)]
        Dropdown,

        [Relation(RelationType.One)]
        Select,

        [Relation(RelationType.Many)]
        MultiSelect
    }
}
