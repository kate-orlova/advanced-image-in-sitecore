using AdvancedImage.GlassMapper.DataMappers;
using AdvancedImage.GlassMapper.Models;
using Glass.Mapper.Sc.IoC;

namespace AdvancedImage.Pipeline
{
    public class RegisterCustomGlassMapper : BaseGlassMapperFluentRegistration
    {
        public override void RegisterFluently(IDependencyResolver glassDependencyResolver)
        {
            glassDependencyResolver.DataMapperFactory.Insert(0, () => new SitecoreFieldAdvanceImageMapper());
            glassDependencyResolver.DataMapperFactory.Insert(0, () => new SitecoreFieldAdvanceImageGalleryMapper());
        }
    }
}