using System;
using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Bizpulse.Infrastructure.FluentNHibernate
{
    /// <summary>
    /// Преобразование типа данных nvarchar в varchar
    /// </summary>
    public class AnsiStringConvention : IPropertyConvention
    {
        private static Type stringType = typeof(string);
        public void Apply(IPropertyInstance instance)
        {
            if (instance.Property.PropertyType == stringType)
            {
                instance.CustomType("AnsiString");
            }
        }
    }

    /// <summary>
    /// Преобразование имен внешних ключей
    /// </summary>
    public class ForeignKeyConstraintNameConvention : IHasManyConvention 
    { 
        public void Apply(IOneToManyCollectionInstance instance) 
        {
            if (instance.Key.Columns.FirstOrDefault() != null)
            {
                // исключаем дубли в названиях FK
                string extra = instance.Key.Columns.FirstOrDefault().Name;
                instance.Key.ForeignKey(string.Format("FK_{0}_{1}_{2}", instance.EntityType.Name, instance.ChildType.Name, extra));
            }
            else
            {
                instance.Key.ForeignKey(string.Format("FK_{0}_{1}", instance.EntityType.Name, instance.ChildType.Name));
            }
        }
    }

    public class ForeignKeyJoinedSubclassConstraintNameConvention : IJoinedSubclassConvention
    {
        public void Apply(IJoinedSubclassInstance instance)
        {
            instance.Key.ForeignKey(string.Format("PFK_{0}", instance.EntityType.Name));
        }
    }

    public class ReferenceConvention : IReferenceConvention 
    { 
        public void Apply(IManyToOneInstance instance) 
        {
            instance.ForeignKey(string.Format("FK_{0}_{1}", instance.EntityType.Name, instance.Name)); 
        } 
    }

    public class ManyToManyConvention : IHasManyToManyConvention
    {
        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Key.ForeignKey(string.Format("PFK_{0}_{1}", instance.ChildType.Name, instance.EntityType.Name));  //Формирует имя внешнего ключа

            if (instance.OtherSide == null) //Какая сторона связи многие ко многим обрабатывается?
            {
                instance.Relationship.ForeignKey(string.Format("PFK_{0}_{1}", instance.EntityType.Name, instance.Relationship.Class.Name));  //Формирует имя внешнего ключа
            }
            else
            {
                instance.OtherSide.Key.ForeignKey(string.Format("PFK_{0}_{1}", instance.OtherSide.ChildType.Name, instance.OtherSide.EntityType.Name));  //Формирует имя внешнего ключа
            }
        }
    }
        
    /// <summary>
    /// Хранение enum как целое значение (а не как строка по умолчанию)
    /// </summary>
    public class EnumConvention : IPropertyConvention, IPropertyConventionAcceptance 
    {     
        public void Apply(IPropertyInstance instance)
        {
            instance.CustomType(instance.Property.PropertyType);     
        }       
         
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)     
        {         
            criteria.Expect(x => x.Property.PropertyType.IsEnum);     
        } 
    } 

}
