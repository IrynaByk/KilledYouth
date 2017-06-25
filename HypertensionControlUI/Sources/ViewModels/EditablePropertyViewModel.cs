using System;
using HypertensionControlUI.Models;
using HypertensionControlUI.Services;

namespace HypertensionControlUI.ViewModels
{
    /// <summary>
    ///     Represents a single patient property which value can be corrected.
    /// </summary>
    public class EditablePropertyViewModel : ViewModelBase
    {
        #region Auto-properties

        /// <summary>
        ///     Original propery value.
        /// </summary>
        public object OriginalValue { get; }

        /// <summary>
        ///     The property path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     The object which provides properties.
        /// </summary>
        private object DataSource { get; }

        #endregion


        #region Properties

        /// <summary>
        ///     Edited property value.
        /// </summary>
        public object CorrectedValue
        {
            get => PatientPropertyProvider.GetPropertyValue( DataSource, Path );
            set
            {
                PatientPropertyProvider.UpdatePatientByProperty( Path, DataSource, value );
                CorrectedValueChanged?.Invoke();
            }
        }

        #endregion


        #region Events and invocation

        /// <summary>
        ///     Notifies about change in the property value.
        /// </summary>
        public event Action CorrectedValueChanged;

        #endregion


        #region Initialization

        /// <summary>
        ///     Creates the view-model for the correctable property.
        /// </summary>
        /// <param name="path">The property path.</param>
        /// <param name="patient">The patient reference.</param>
        /// <param name="patientVisitData">The patient visit data reference.</param>
        public EditablePropertyViewModel( string path, Patient patient, PatientVisitData patientVisitData )
        {
            DataSource = new { Patient = patient, PatientVisitData = patientVisitData };
            Path = path;
            OriginalValue = PatientPropertyProvider.GetPropertyValue( DataSource, path );
        }

        #endregion
    }
}
