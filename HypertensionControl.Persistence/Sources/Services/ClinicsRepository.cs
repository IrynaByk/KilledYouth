using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using HypertensionControl.Domain.Models;
using HypertensionControl.Persistence.Entities;
using HypertensionControl.Persistence.Interfaces;

namespace HypertensionControl.Persistence.Services
{
    public class ClinicsRepository : IClinicsRepository
    {
        #region Fields

        private readonly SqliteDbContext _dbContext;
        private readonly IMapper _mapper;

        #endregion


        #region Initialization

        public ClinicsRepository( SqliteDbContext dbContext, IMapper mapper )
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        #endregion


        #region Public methods

        public ICollection<Clinic> GetAllClinics()
        {
            var clinicEntities = _dbContext.Clinics.ToList();
            return _mapper.Map<ICollection<Clinic>>( clinicEntities );
        }

        public void SaveClinic( Clinic clinic )
        {
            var clinicEntity = _dbContext.Clinics.Find( clinic.Id );

            //  Existing entity
            if ( clinicEntity != null )
            {
                _mapper.Map( clinic, clinicEntity );
            }

            //  New entity
            else
            {
                clinicEntity = _mapper.Map<ClinicEntity>( clinic );
                _dbContext.Clinics.Add( clinicEntity );
            }
        }

        #endregion
    }
}
