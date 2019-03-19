﻿using System;
using System.Collections.Generic;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Interfaces;

namespace RapidCMS.Common.Models.Config
{
    public class ListViewConfig
    {
        public List<ButtonConfig> Buttons { get; set; } = new List<ButtonConfig>();
        public List<ListViewPaneConfig> ListViewPanes { get; set; } = new List<ListViewPaneConfig>();
    }

    public class ListViewConfig<TEntity> : ListViewConfig
        where TEntity : IEntity
    {
        public ListViewConfig<TEntity> AddDefaultButton(DefaultButtonType type, string label = null, string icon = null)
        {
            var button = new DefaultButtonConfig
            {
                ButtonType = type,
                Icon = icon ?? type.GetCustomAttribute<DefaultIconLabelAttribute>().Icon,
                Label = label ?? type.GetCustomAttribute<DefaultIconLabelAttribute>().Label
            };

            Buttons.Add(button);

            return this;
        }

        public ListViewConfig<TEntity> AddCustomButton(string alias, CrudType crudType, Action action, string label = null, string icon = null)
        {
            var button = new CustomButtonConfig(alias)
            {
                Action = action,
                CrudType = crudType,
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        public ListViewConfig<TEntity> AddCustomButton<TActionHandler>(string alias, string label = null, string icon = null)
        {
            var button = new CustomButtonConfig(alias)
            {
                ActionHandler = typeof(TActionHandler),
                Icon = icon,
                Label = label
            };

            Buttons.Add(button);

            return this;
        }

        // TODO: add polymorphism
        public ListViewConfig<TEntity> AddListPane(Action<ListViewPaneConfig<TEntity>> configure)
        {
            var config = new ListViewPaneConfig<TEntity>();

            configure.Invoke(config);

            ListViewPanes.Add(config);

            return this;
        }
    }
}