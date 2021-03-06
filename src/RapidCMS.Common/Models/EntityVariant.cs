﻿using System;

namespace RapidCMS.Common.Models
{
    public class EntityVariant
    {
        public static EntityVariant Undefined = new EntityVariant();

        public string Name { get; internal set; }
        public string? Icon { get; internal set; }
        public Type Type { get; internal set; }
        public string Alias { get; internal set; }
    }
}
