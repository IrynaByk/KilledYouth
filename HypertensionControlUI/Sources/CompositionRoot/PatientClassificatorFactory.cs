using HypertensionControlUI.Models;
using HypertensionControlUI.Services;
using SimpleInjector;

namespace HypertensionControlUI.CompositionRoot
{
    public class PatientClassificatorFactory
    {
        private readonly Container _container;

        public PatientClassificatorFactory( Container container )
        {
            _container = container;
        }

        public PatientClassificator GetClassificator(ClassificationModel classificationModel)
        {
            return new PatientClassificator(classificationModel, _container.GetInstance<PatientPropertyProvider>());
        }
    }
}