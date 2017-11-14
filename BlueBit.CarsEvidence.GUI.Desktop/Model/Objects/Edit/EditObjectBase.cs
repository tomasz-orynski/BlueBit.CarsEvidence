using AutoMapper;
using BlueBit.CarsEvidence.BL.Alghoritms;
using BlueBit.CarsEvidence.BL.Repositories;
using BlueBit.CarsEvidence.Commons.Diagnostics;
using BlueBit.CarsEvidence.Commons.Templates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit
{
    public interface IEditObject :
        IObject,
        IObjectWithGetID,
        INotifyDataErrorInfo
    {
    }

    public interface IEditObjectWithCode :
        IEditObject,
        IObjectWithGetCode
    {
        new string Code { get; set; }
    }

    public interface IEditObjectWithInfo :
        IEditObject,
        IObjectWithGetInfo
    {
        new string Info { get; set; }
    }

    public abstract class EditObjectBase :
        ObjectWithIDBase,
        IEditObject
    {
        private readonly Dictionary<string, List<string>> _validationErrors = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors { get { return _validationErrors.Values.Any(v => v != null && v.Count > 0); } }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return null;

            List<string> errors;
            _validationErrors.TryGetValue(propertyName, out errors);
            return errors;
        }

        protected override void OnSet(string propertyName)
        {
            base.OnSet(propertyName);
            Validate(propertyName);
        }

        public void Validate(string propertyName = null)
        {
            Validate(new ValidationContext(this, null, null), propertyName);
        }

        private void Validate(ValidationContext validationContext, string propertyName)
        {
            validationContext.MemberName = propertyName;
            var validationResults = TryValidate(validationContext);

            _validationErrors.Keys.ToList().ForEach(k =>
            {
                if (validationResults.All(r => r.MemberNames.All(m => m != k)))
                {
                    _validationErrors.Remove(k);
                    RaiseErrorsChanged(k);
                }
            });

            var q = from r in validationResults
                    from m in r.MemberNames
                    group r by m into g
                    select g;

            foreach (var prop in q)
            {
                var messages = prop.Select(r => r.ErrorMessage).ToList();
                _validationErrors[prop.Key] = messages;
                RaiseErrorsChanged(prop.Key);
            }
        }
        protected IEnumerable<ValidationResult> TryValidate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(this, validationContext, validationResults, true);
            return validationResults;
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }

    public class EditObjectConverter<TObject, TEntity> :
        IConverterInitiator,
        IConverterFromEntity<TObject, TEntity>,
        IConverterToEntity<TObject, TEntity>
        where TObject : EditObjectBase
        where TEntity : BL.Entities.EntityBase
    {
        protected enum Mode
        {
            Create,
            Update,
        }

#if DEBUG
        private readonly object _dbgSc1 = new SingletonChecker<IMappingExpression<TObject, TEntity>>();
        private readonly object _dbgSc2 = new SingletonChecker<IMappingExpression<TEntity, TObject>>();
#endif

        public void Initialize()
        {
            OnInitialize(Mapper
                .CreateMap<TEntity, TObject>()
                .ConstructUsingServiceLocator());
            OnInitialize(Mapper
                .CreateMap<TObject, TEntity>());
        }

        protected virtual void OnInitialize(IMappingExpression<TEntity, TObject> mapingExpr) { }
        protected virtual void OnInitialize(IMappingExpression<TObject, TEntity> mapingExpr) { }

        public TObject Create(TEntity src)
        {
            Contract.Assert(src != null);

            var result = Mapper.Map<TEntity, TObject>(
                src,
                opt => { 
                    opt.ConstructServicesUsing(Configuration.Settings.ResolveType); 
                    opt.BeforeMap(OnBeforeMap); 
                });
            OnAfterMap(src, result, Mode.Create);
            return result;
        }
        public TEntity Create(TObject src)
        {
            Contract.Assert(src != null);

            var result = Mapper.Map<TObject, TEntity>(
                src, 
                opt => opt.BeforeMap(OnBeforeMap));
            OnAfterMap(src, result, Mode.Create);
            return result;
        }

        public TObject Update(TEntity src, TObject dst)
        {
            var result = Mapper.Map<TEntity, TObject>(src, dst);
            OnAfterMap(src, result, Mode.Update);
            return result;
        }
        public TEntity Update(TObject src, TEntity dst)
        {
            var result = Mapper.Map<TObject, TEntity>(src, dst);
            OnAfterMap(src, result, Mode.Update);
            return result;
        }

        protected void OnBeforeMap(TEntity src, TObject dst)
        {
            OnBeforeMap(dst);
        }
        protected void OnBeforeMap(TObject src, TEntity dst) 
        { 
            dst.Init();
            OnBeforeMap(dst);
        }

        protected virtual void OnBeforeMap(TEntity dst) { }
        protected virtual void OnBeforeMap(TObject dst) { }

        protected virtual void OnAfterMap(TEntity src, TObject dst, Mode mode) { }
        protected virtual void OnAfterMap(TObject src, TEntity dst, Mode mode) { }
    }

    public class EditObjectConverterWithContext<TContext, TObject, TEntity> :
        IConverterInitiator,
        IConverterFromEntityChild<TContext, TObject, TEntity>,
        IConverterToEntityChild<TContext, TObject, TEntity>
        where TObject : EditObjectBase
        where TEntity : IObjectInRepository
    {
#if DEBUG
        private readonly object _dbgSc1 = new SingletonChecker<IMappingExpression<TObject, TEntity>>();
        private readonly object _dbgSc2 = new SingletonChecker<IMappingExpression<TEntity, TObject>>();
#endif

        public void Initialize()
        {
            OnInitialize(Mapper.CreateMap<TEntity, TObject>().ConstructUsingServiceLocator());
            OnInitialize(Mapper.CreateMap<TObject, TEntity>());
        }

        protected virtual IMappingExpression<TEntity, TObject> OnInitialize(IMappingExpression<TEntity, TObject> mapingExpr) 
        {
            return mapingExpr;
        }
        protected virtual IMappingExpression<TObject, TEntity> OnInitialize(IMappingExpression<TObject, TEntity> mapingExpr) 
        {
            return mapingExpr
                .ForMember(
                    _ => _.ID,
                    cfg => cfg.MapFrom(
                        _ => _.ID > 0 ? _.ID : 0
                        ));
        }

        public TObject Create(TContext ctx, TEntity src)
        {
            var result = Mapper.Map<TEntity, TObject>(src, opt => opt.ConstructServicesUsing(Configuration.Settings.ResolveType));
            OnCreateUpdate(ctx, src, result);
            return result;
        }
        public TEntity Create(TContext ctx, TObject src)
        {
            var result = Mapper.Map<TObject, TEntity>(src);
            OnCreateUpdate(ctx, src, result);
            return result;
        }
        public TObject Update(TContext ctx, TEntity src, TObject dst)
        {
            var result = Mapper.Map<TEntity, TObject>(src, dst);
            OnCreateUpdate(ctx, src, result);
            return result;
        }
        public TEntity Update(TContext ctx, TObject src, TEntity dst)
        {
            var result = Mapper.Map<TObject, TEntity>(src, dst);
            OnCreateUpdate(ctx, src, result);
            return result;
        }

        protected virtual void OnCreateUpdate(TContext ctx, TEntity src, TObject dst)
        { }
        protected virtual void OnCreateUpdate(TContext ctx, TObject src, TEntity dst)
        { }
    }
}
