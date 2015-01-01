using AutoMapper;
using BlueBit.CarsEvidence.Commons.Diagnostics;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Diagnostics;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View
{
    [DebuggerDisplay("ID={ID}")]
    public abstract class ViewObjectBase :
        ObjectBase
    {
    }

    public abstract class ViewObjectWithCodeBase :
        ViewObjectBase
    {
        private string _code;
        public string Code { get { return _code; } set { Set(ref _code, value); } }
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
