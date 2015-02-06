using FluentNHibernate.Automapping;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace BlueBit.CarsEvidence.BL.Entities
{
    public class Address :
        EntityWithCodeBase
    {
        [MaxLength(Configuration.Consts.LengthPostalCode)]
        public virtual string PostalCode { get; set; }
        [Required]
        [MaxLength(Configuration.Consts.LengthText)]
        public virtual string City { get; set; }
        [Required]
        [MaxLength(Configuration.Consts.LengthText)]
        public virtual string Street { get; set; }
        [Required]
        [MaxLength(Configuration.Consts.LengthCode)]
        public virtual string BuildingNo { get; set; }
        [MaxLength(Configuration.Consts.LengthCode)]
        public virtual string LocalNo { get; set; }
        [MaxLength(Configuration.Consts.LengthInfo)]
        public virtual string Info { get; set; }

        public virtual ISet<Company> Companies { get; set; }
        public virtual ISet<Route> Routes { get; set; }

        public override void Init()
        {
            Companies = Companies ?? new HashSet<Company>();
            Routes = Routes ?? new HashSet<Route>();
        }
    }

    public static class AddressExtensions
    {
        public static Company AddCompany(this Address @this, Company company)
        {
            Contract.Assert(@this != null);
            Contract.Assert(company != null);
            Contract.Assert(company.Address == null);

            company.Address = @this;
            @this.Companies.Add(company);
            return company;
        }
        public static Route AddRouteFrom(this Address @this, Route route)
        {
            Contract.Assert(@this != null);
            Contract.Assert(route != null);
            Contract.Assert(route.AddressFrom == null);

            route.AddressFrom = @this;
            @this.Routes.Add(route);
            return route;
        }
        public static Route AddRouteTo(this Address @this, Route route)
        {
            Contract.Assert(@this != null);
            Contract.Assert(route != null);
            Contract.Assert(route.AddressTo == null);

            route.AddressTo = @this;
            @this.Routes.Add(route);
            return route;
        }
    }

    public class AddressCfg :
        EntityCfgBase<Address>
    {
        protected override void ConfigureMapping(AutoMapping<Address> map)
        {
            map.HasMany(_ => _.Routes)
                .Cascade.All();
            map.HasMany(_ => _.Companies)
                .Cascade.All();
        }
    }
}
