using System.Text;

namespace CrazySharpLib
{
    public class ToCItem
    {
        internal ToCItem(byte[] data)
        {
            Type = (LogDataType)data[2];

            string s = Encoding.ASCII.GetString(data, 3, data.Length - 3);

            var split = s.Split('\0');
            GroupName = split[0];
            VariableName = split[1];
        }

        public readonly LogDataType Type;
        public readonly string GroupName;
        public readonly string VariableName;

        public override string ToString()
        {
            return string.Format("[{0}] {1} - {2} ", Type, GroupName, VariableName);
        }
    }
}