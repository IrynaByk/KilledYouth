using System;
using HypertensionControl.Persistence.Interfaces;
using HypertensionControl.Persistence.Services;

namespace HypertensionControlUI.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();

        IUsersRepository UsersRepository { get; }
        IPatientsRepository PatientsRepository { get; }
        IClinicsRepository ClinicsRepository { get; }
        IClassificationModelsRepository ClassificationModelsRepository { get; }
    }

}
