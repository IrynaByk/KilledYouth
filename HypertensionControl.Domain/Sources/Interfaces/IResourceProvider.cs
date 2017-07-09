using System.IO;

namespace HypertensionControl.Domain.Interfaces
{
    public interface IResourceProvider
    {
        Stream GetResourceStream(string name);
        string[] ReadAllResourceLines(string name);
        string ReadAllResourceText(string name);
    }
}
