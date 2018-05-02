using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class AccountingPriceListMap : ClassMap<AccountingPriceList>
    {
        public AccountingPriceListMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.Number).Length(25).Not.Nullable();
            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.EndDate);            
            Map(x => x.Reason).CustomType<AccountingPriceListReason>().Column("ReasonId").Not.Nullable();
            Map(x => x.ReasonReceiptWaybillId);
            Map(x => x.ReasonReceiptWaybillNumber).Length(25);
            Map(x => x.ReasonReceiptWaybillDate);
            Map(x => x.State).CustomType<AccountingPriceListState>().Column("AccountingPriceListStateId").Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.AcceptanceDate);
            Map(x => x.IsRevaluationOnStartCalculated).Not.Nullable();
            Map(x => x.IsRevaluationOnEndCalculated).Not.Nullable();

            Component<AccountingPriceCalcRule>(x => x.AccountingPriceCalcRule).ColumnPrefix("AccountingPriceCalcRule");
            Component<LastDigitCalcRule>(x => x.LastDigitCalcRule).ColumnPrefix("LastDigitCalcRule");

            Map(x => x.DeletionDate);

            HasMany(x => x.ArticlePrices)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("AccountingPriceListId")
                .Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");

            HasManyToMany<Storage>(x => x.Storages)
                .AsSet().Access.CamelCaseField()
                .Table("AccountingPriceListStorage")
                .ParentKeyColumn("AccountingPriceListId")
                .ChildKeyColumn("StorageId").Cascade.All();

            References(x => x.Curator).Column("CuratorId").Not.Nullable();

            BatchSize(30);
        }
    }
}
