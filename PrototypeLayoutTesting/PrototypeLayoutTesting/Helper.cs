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

        /// <summary>
        /// Extracts all of the 6 values from a Transformation Matrix
        /// </summary>
        /// <param name="path">The XML containing the Transformation Matrix</param>
        /// <param name="matrixType">The type of attribute to be extracted</param>
        /// <returns></returns>
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