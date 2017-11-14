using AutoMapper;
using BlueBit.CarsEvidence.Commons.Diagnostics;
using System.Diagnostics;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View
{
    public interface IViewObject :
        IObject
    {
    }

    [DebuggerDisplay("ID={ID}")]
    public abstract class ViewObjectBase :
        ObjectWithIDBase,
        IViewObject
    {
    }

    public class ViewObjectConverter<TObject, TEntity> :
        IConverterInitiator,
        IConverterFromEntity<TObject, TEntity>
        where TObject : ViewObjectBase
        where TEntity : BL.Entities.EntityBase
    {
#if DEBUG
        private readonly object _dbgSc = new SingletonChecker<IMappingExpression<TEntity, TObject>>();
#endif

        public void Initialize()
        {
            OnInitialize(Mapper.CreateMap<TEntity, TObject>().ConstructUsingServiceLocator());
        }
        protected virtual void OnInitialize(IMappingExpression<TEntity, TObject> mapingExpr) { }

        public TObject Create(TEntity src) 
        {
            var result = Mapper.Map<TEntity, TObject>(src, opt => opt.ConstructServicesUsing(Configuration.Settings.ResolveType));
            return result; 
        }
        public TObject Update(TEntity src, TObject dst)
        {
            return Mapper.Map<TEntity, TObject>(src, dst);
        }
    }
}
