using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using System;

namespace AdvancedImage.Extensions
{
    public static class CommonExtensions
    {
        public static string ToSchemaOrgJsonString<TModel, TSchemaOrgModel>(this TModel model)
            where TSchemaOrgModel : MXTires.Microdata.Thing
            where TModel : class
        {
            try
            {
                var mapper = ServiceLocator.ServiceProvider?.GetService<IMapper>();
                var schema = mapper?.Map<TModel, TSchemaOrgModel>(model);
                return schema?.ToJson();
            }
            catch (Exception exception)
            {
                Sitecore.Diagnostics.Error.LogError($"{exception.Message}\n{exception.StackTrace}");
            }

            return null;
        }
    }
}