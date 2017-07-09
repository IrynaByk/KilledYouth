using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using HypertensionControl.Domain.Models;
using HypertensionControl.Persistence.Interfaces;

namespace HypertensionControl.Persistence.Services
{
    public class ClassificationModelsRepository : IClassificationModelsRepository
    {
        #region Fields

        private readonly SqliteDbContext _dbContext;
        private readonly IMapper _mapper;

        #endregion


        #region Initialization

        public ClassificationModelsRepository( SqliteDbContext dbContext, IMapper mapper )
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        #endregion


        #region Public methods

        public ICollection<ClassificationModel> GetAllClassificationModels()
        {
            var classificationModelEntities = _dbContext.ClassificationModels.ToList();
            return _mapper.Map<ICollection<ClassificationModel>>( classificationModelEntities );
        }

        #endregion
    }
}
