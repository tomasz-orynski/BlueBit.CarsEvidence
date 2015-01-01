﻿using AutoMapper;
using BlueBit.CarsEvidence.BL.Repositories;
using BlueBit.CarsEvidence.Commons.Diagnostics;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.Edit
{
    public abstract class EditObjectBase :
        ObjectBase,
        INotifyDataErrorInfo
        //IValidatableObject
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

    public abstract class EditObjectWithCodeBase :
        EditObjectBase
    {
        private string _code;
        [Required]
        [MaxLength(BL.Configuration.Consts.LengthCode)]
        [Key]
        public string Code { get { return _code; } set { Set(ref _code, value); } }
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
            OnInitialize(Mapper.CreateMap<TEntity, TObject>().ConstructUsingServiceLocator());
            OnInitialize(Mapper.CreateMap<TObject, TEntity>());
        }

        protected virtual void OnInitialize(IMappingExpression<TEntity, TObject> mapingExpr) { }
        protected virtual void OnInitialize(IMappingExpression<TObject, TEntity> mapingExpr) { }

        public TObject Create(TEntity src)
        {
            Contract.Assert(src != null);

            var result = Mapper.Map<TEntity, TObject>(OnCreate(src), opt => opt.ConstructServicesUsing(Configuration.Settings.ResolveType));
            OnCreateUpdate(src, result, Mode.Create);
            return result;
        }
        public TEntity Create(TObject src)
        {
            Contract.Assert(src != null);

            var result = Mapper.Map<TObject, TEntity>(OnCreate(src));
            OnCreateUpdate(src, result, Mode.Create);
            return result;
        }

        public TObject Update(TEntity src, TObject dst)
        {
            var result = Mapper.Map<TEntity, TObject>(src, dst);
            OnCreateUpdate(src, result, Mode.Update);
            return result;
        }
        public TEntity Update(TObject src, TEntity dst)
        {
            var result = Mapper.Map<TObject, TEntity>(src, dst);
            OnCreateUpdate(src, result, Mode.Update);
            return result;
        }

        protected virtual TEntity OnCreate(TEntity src) { return src; }
        protected virtual TObject OnCreate(TObject src) { return src; }

        protected virtual void OnCreateUpdate(TEntity src, TObject dst, Mode mode) { }
        protected virtual void OnCreateUpdate(TObject src, TEntity dst, Mode mode) { }
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
