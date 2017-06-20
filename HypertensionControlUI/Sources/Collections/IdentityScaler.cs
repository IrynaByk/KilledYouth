namespace HypertensionControlUI.Collections
{
    class IdentityScaler : IScaler
    {
        public double this[ double position ] => position;
    }
}