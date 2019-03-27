using System.Collections;

namespace Codes
{
    public interface ICode
    {
        BitArray Code(string text, out int codeLength);
        string Decode(BitArray code, out int codeLength);
    }
}
