﻿using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;

namespace RapidCMS.Common.Models
{
    internal class List
    {
        internal int? PageSize { get; set; }
        internal bool? SearchBarVisible { get; set; }
        internal ListType ListType { get; set; }
        internal EmptyVariantColumnVisibility EmptyVariantColumnVisibility { get; set; }
        internal List<Pane>? Panes { get; set; }
        internal List<Button>? Buttons { get; set; }

        internal Button? FindButton(string buttonId)
        {
            return GetAllButtons()?.FirstOrDefault(x => x.ButtonId == buttonId);
        }
        internal IEnumerable<Button>? GetAllButtons()
        {
            if (Buttons != null)
            {
                foreach (var button in Buttons.GetAllButtons())
                {
                    yield return button;
                }
            }
            if (Panes != null)
            {
                foreach (var button in Panes.SelectMany(pane => pane.Buttons.GetAllButtons()))
                {
                    yield return button;
                }
            }
        }
    }
}
