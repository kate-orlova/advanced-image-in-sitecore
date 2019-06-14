using Glass.Mapper.Sc.IoC;

namespace AdvancedImage.GlassMapper.Models
{
    public abstract class BaseGlassMapperFluentRegistration
    {
        public abstract void RegisterFluently(IDependencyResolver glassDependencyResolver);
    }
}