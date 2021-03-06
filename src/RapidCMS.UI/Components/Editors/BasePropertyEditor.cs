﻿using System;
using System.ComponentModel;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.UI.Components.Editors
{
    public class BasePropertyEditor : BaseEditor
    {
        private new IFullPropertyMetadata Property
        {
            get
            {
                return base.Property as IFullPropertyMetadata ?? throw new InvalidOperationException($"{nameof(BasePropertyEditor)} requires usable Getter and Setter");
            }
        }

        protected void SetValueFromObject(object value)
        {
            if (!IsDisabled)
            {
                Property.Setter(Entity, value);
            }

            EditContext.NotifyPropertyChanged(Property);
        }

        protected void SetValueFromString(string value)
        {
            var obj = TypeDescriptor.GetConverter(Property.PropertyType).ConvertFromString(value);
            SetValueFromObject(obj);
        }
    }
}
