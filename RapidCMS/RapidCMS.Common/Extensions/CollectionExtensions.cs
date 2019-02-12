﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Common.Data;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Interfaces;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;
using RapidCMS.Common.Services;

namespace RapidCMS.Common.Extensions
{
    public static class RapidCMSMiddleware
    {
        public static IServiceCollection AddRapidCMS(this IServiceCollection services, Action<Root> configure)
        {
            var root = new Root();

            configure.Invoke(root);

            services.AddSingleton(root);

            return services;
        }
    }

    public static class ICollectionRootExtensions
    {
        public static ICollectionRoot AddCollection<TEntity>(this ICollectionRoot root, string alias, string name, Action<CollectionConfig<TEntity>> configure)
            where TEntity : IEntity
        {
            var collection = new Collection
            {
                Name = name,
                Alias = alias
            };

            var configReceiver = new CollectionConfig<TEntity>();

            configure.Invoke(configReceiver);

            collection.RepositoryType = configReceiver.RepositoryType;

            if (configReceiver.TreeView != null)
            {
                var prop = PropertyMetadataHelper.Create(configReceiver.TreeView.NameGetter);

                collection.TreeView = new TreeView
                {
                    Name = configReceiver.TreeView.Name,
                    EntityViewType = configReceiver.TreeView.ViewType,
                    NameGetter = prop.Getter
                };
            }

            if (configReceiver.ListView != null)
            {
                collection.ListView = new ListView
                {
                    Buttons = configReceiver.ListView.Buttons.ToList(button => button switch
                    {
                        DefaultButtonConfig defaultButton => new DefaultButton
                        {
                            Id = Guid.NewGuid().ToString(),
                            DefaultButtonType = defaultButton.ButtonType,
                            Icon = defaultButton.Icon,
                            Label = defaultButton.Label
                        },
                        _ => default(Button)
                    }),
                    ViewPanes = configReceiver.ListView.ListViewPanes.ToList(pane =>
                    {
                        return new ViewPane<ListViewProperty>
                        {
                            Properties = pane.Properties.ToList(property => new ListViewProperty
                            {
                                Description = property.Description,
                                Name = property.Name,
                                NodeProperty = property.NodeProperty,
                                ValueMapper = property.ValueMapper ?? new DefaultViewValueMapper(),
                                ValueMapperType = property.ValueMapperType
                            })
                        };
                    })
                };
            }

            if (configReceiver.NodeEditor != null)
            {
                collection.NodeEditor = new NodeEditor
                {
                    Buttons = configReceiver.NodeEditor.Buttons.ToList(button => button switch
                    {
                        DefaultButtonConfig defaultButton => new DefaultButton
                        {
                            Id = Guid.NewGuid().ToString(),
                            DefaultButtonType = defaultButton.ButtonType,
                            Icon = defaultButton.Icon,
                            Label = defaultButton.Label
                        },
                        _ => default(Button)
                    }),

                    EditorPanes = configReceiver.NodeEditor.EditorPanes.ToList(pane =>
                    {
                        return new EditorPane<Field>
                        {
                            Fields = pane.Fields.ToList(field => {
                                
                                return new Field
                                {
                                    DataType = field.Type,
                                    Description = field.Description,
                                    Name = field.Name,
                                    NodeProperty = field.NodeProperty,
                                    ValueMapper = field.ValueMapper ?? new DefaultEditorValueMapper(),
                                    ValueMapperType = field.ValueMapperType  
                                };
                            })
                        };
                    })
                };
            }

            collection.Collections = configReceiver.Collections;
            
            root.Collections.Add(collection);

            return root;
        }
    }
}
