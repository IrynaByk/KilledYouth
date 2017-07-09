using System.Collections.Generic;
using HypertensionControl.Domain.Models;

namespace HypertensionControl.Persistence.Interfaces
{
    public interface IUsersRepository
    {
        #region Public methods

        User FindUserByLoginAndPassword( string login, string password );

        #endregion
    }

    public interface IPatientsRepository
    {
        #region Public methods

        ICollection<Patient> GetAllPatients();
        void SavePatient( Patient patient );

        #endregion
    }

    public interface IClinicsRepository
    {
        #region Public methods

        ICollection<Clinic> GetAllClinics();
        void SaveClinic( Clinic clinic );

        #endregion
    }

    public interface IClassificationModelsRepository
    {
        #region Public methods

        ICollection<ClassificationModel> GetAllClassificationModels();

        #endregion
    }
}
