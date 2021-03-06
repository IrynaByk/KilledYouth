using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using HypertensionControl.Domain.Models;
using HypertensionControl.Persistence.Entities;
using HypertensionControl.Persistence.Interfaces;

namespace HypertensionControl.Persistence.Services
{
    public class PatientsRepository : IPatientsRepository
    {
        #region Fields

        private readonly SqliteDbContext _dbContext;
        private readonly IMapper _mapper;

        #endregion


        #region Initialization

        public PatientsRepository( SqliteDbContext dbContext, IMapper mapper )
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        #endregion


        #region Public methods

        public ICollection<Patient> GetAllPatients()
        {
            var patientEntities = _dbContext.Patients.Include( p => p.VisitHistory ).ToList();
            var allPatients = _mapper.Map<ICollection<Patient>>( patientEntities );
            return allPatients;
        }

        public void SavePatient( Patient patient )
        {
            var patientEntity = _dbContext.Patients.Include( p => p.VisitHistory ).SingleOrDefault( p => p.Id == patient.Id.ToString() );
            if ( patientEntity != null ) //  existing entity
            {
                _mapper.Map( patient, patientEntity );
            }
            else //  new entity
            {
                patientEntity = _mapper.Map<PatientEntity>( patient );
                _dbContext.Patients.Add( patientEntity );
            }
        }

        public Patient ClonePatient( Patient patient )
        {
            var patientEntity = _mapper.Map<PatientEntity>( patient );
            foreach ( var patientVisitEntity in patientEntity.VisitHistory )
            {
                patientVisitEntity.Patient = patientEntity;
                patientVisitEntity.PatientId = patientEntity.Id;
            }
            return _mapper.Map<Patient>( patientEntity );
        }

        #endregion
    }
}
