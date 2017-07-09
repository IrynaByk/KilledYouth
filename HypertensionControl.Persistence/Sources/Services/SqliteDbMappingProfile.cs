using System;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using HypertensionControl.Domain.Models;
using HypertensionControl.Domain.Models.Values;
using HypertensionControl.Persistence.Entities;
using Newtonsoft.Json;

namespace HypertensionControl.Persistence.Services
{
    public class SqliteDbMappingProfile : Profile
    {
        #region Initialization

        public SqliteDbMappingProfile() : base( "SqliteDbMapping" )
        {
            CreateMap<ClinicEntity, Clinic>().ReverseMap();

            CreateMap<UserEntity, User>().ReverseMap();

            CreateMap<PatientVisitEntity, PatientVisit>()
                .ForMember( visit => visit.VisitDate, expr => expr.ResolveUsing( e => new DateTime( e.VisitDateTicks ) ) )
                .ForMember( visit => visit.SaltSensitivity, expr => expr.ResolveUsing( e => Deserialize<SaltSensitivityTest>( e.SaltSensitivitySerialized ) ) )
                .ForMember( visit => visit.Smoking, expr => expr.ResolveUsing( e => Deserialize<Smoking>( e.SmokingSeralized ) ) )
                .ForMember( visit => visit.BloodPressure, expr => expr.ResolveUsing( e => Deserialize<BloodPressure>( e.BloodPressureSerialized ) ) )
                .ForMember( visit => visit.DietaryHabits, expr => expr.ResolveUsing( e => Deserialize<DietaryHabits>( e.DietaryHabitsSerialized ) ) )
                .ReverseMap()
                .PreserveReferences()
                .EqualityComparison( ( visit, entity ) => visit.Patient.Id == entity.PatientId && visit.VisitDate.Ticks == entity.VisitDateTicks )
                .ForMember( entity => entity.Patient, expr => expr.Ignore() )
                .ForMember( entity => entity.PatientId, expr => expr.ResolveUsing( p => p.Patient.Id ) )
                .ForMember( entity => entity.VisitDateTicks, expr => expr.MapFrom( visit => visit.VisitDate.Ticks ) )
                .ForMember( entity => entity.SaltSensitivitySerialized, expr => expr.ResolveUsing( visit => Serialize( visit.SaltSensitivity ) ) )
                .ForMember( entity => entity.SmokingSeralized, expr => expr.ResolveUsing( visit => Serialize( visit.Smoking ) ) )
                .ForMember( entity => entity.BloodPressureSerialized, expr => expr.ResolveUsing( visit => Serialize( visit.BloodPressure ) ) )
                .ForMember( entity => entity.DietaryHabitsSerialized, expr => expr.ResolveUsing( visit => Serialize( visit.DietaryHabits ) ) );

            CreateMap<PatientEntity, Patient>()
                .ForMember( patient => patient.BirthDate, expr => expr.ResolveUsing( entity => new DateTime( entity.BirthDateTicks ) ) )
                .ForMember( patient => patient.Genes, expr => expr.ResolveUsing( entity => Deserialize<IDictionary<string, Gene>>( entity.GenesSerialized ) ) )
                .ForMember( patient => patient.Medicine,
                            expr => expr.ResolveUsing( entity => Deserialize<ICollection<Medicine>>( entity.MedicineSerialized ) ) )
                .ReverseMap()
                .ForMember( entity => entity.BirthDateTicks, expr => expr.MapFrom( patient => patient.BirthDate.Ticks ) )
                .ForMember( entity => entity.GenesSerialized, expr => expr.ResolveUsing( patient => Serialize( patient.Genes ) ) )
                .ForMember( entity => entity.MedicineSerialized, expr => expr.ResolveUsing( patient => Serialize( patient.Medicine ) ) );

            CreateMap<ClassificationModelEntity, ClassificationModel>()
                .ConstructUsing( entity => new ClassificationModel(
                                     entity.Name,
                                     entity.Description,
                                     Deserialize<IList<double>>( entity.LimitPointsSerialized ),
                                     Deserialize<ICollection<ClassificationModelProperty>>( entity.PropertiesSerialized ),
                                     entity.FreeCoefficient,
                                     entity.OptimalCutOff ) );
        }

        #endregion


        #region Non-public methods

        private static TType Deserialize<TType>( string serializedValue )
        {
            return JsonConvert.DeserializeObject<TType>( serializedValue );
        }

        private static string Serialize( object value )
        {
            return JsonConvert.SerializeObject( value, Formatting.None );
        }

        #endregion
    }
}
