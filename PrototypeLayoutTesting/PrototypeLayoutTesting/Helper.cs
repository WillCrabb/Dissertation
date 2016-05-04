using System.Xml.Linq;

namespace PrototypeLayoutTesting
{
    public static class Helper
    {

        public enum MatrixType
        {
            PathMatrix,
            GradientMatrix
        }

        public static string[] GetMatrixValuesFromXml(XElement path, MatrixType matrixType)
        {
            string attribute;

            if (matrixType == MatrixType.PathMatrix)
            {
                attribute = "transform";
            }
            else
            {
                attribute = "gradientTransform";
            }

            string input = path.Attribute(attribute).Value;
            return input.Split('(', ')')[1].Split(' ');
        }
    }
}