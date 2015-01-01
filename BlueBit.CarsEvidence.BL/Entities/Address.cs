using FluentNHibernate.Automapping;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace BlueBit.CarsEvidence.BL.Entities
{
    [DataContract(Namespace = Consts.NamespaceEntities, IsReference = true)]
    public class Address :
        EntityWithCodeBase
    {
        [Required]
        [MaxLength(Configuration.Consts.LengthPostalCode)]
        [DataMember]
        public virtual string PostalCode { get; set; }
        [Required]
        [DataMember]
        public virtual string City { get; set; }
        [Required]
        [DataMember]
        public virtual string Street { get; set; }
        [Required]
        [MaxLength(Configuration.Consts.LengthCode)]
        [DataMember]
        public virtual string BuildingNo { get; set; }
        [MaxLength(Configuration.Consts.LengthCode)]
        [DataMember]
        public virtual string LocalNo { get; set; }

        public virtual ISet<Route> Routes { get; set; }

        public override void Init()
        {
            Routes = Routes ?? new HashSet<Route>();
        }
    }

    public static class AddressExtensions
    {
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
        }
    }
}
