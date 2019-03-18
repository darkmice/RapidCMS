﻿using System;
using System.Threading.Tasks;

namespace RapidCMS.UI.Components.Buttons
{
    public class ButtonContext<TContext>
    {
        public string Label { get; set; }
        public string Icon { get; set; }
        public string ButtonId { get; set; }
        public Func<string, TContext, Task> CallbackAsync { get; set; }
        public TContext Context { get; set; }
    }
}
