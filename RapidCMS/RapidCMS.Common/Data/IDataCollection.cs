﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Data
{
    // TODO: paginate etc
    public interface IDataCollection
    {
        Task<IEnumerable<IElement>> GetAvailableElementsAsync();
    }

    public interface IRelationDataCollection : IDataCollection
    {
        void SetElementMetadata(IRepository repository, Type relatedEntityType, IPropertyMetadata idProperty, IExpressionMetadata labelProperty);
        Task SetRelationMetadataAsync(IEntity entity, IPropertyMetadata collectionProperty);

        Task<IEnumerable<IElement>> GetRelatedElementsAsync();

        Task AddElementAsync(IElement option);
        Task RemoveElementAsync(IElement option);

        IEnumerable<IElement> GetCurrentRelatedElements();

        Type GetRelatedEntityType();
    }
}