using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using ExcelDna.Integration;

//using MathNet.Numerics.LinearAlgebra;

namespace PetroEnExcel
{
    public static class LimitedEntryPerforating
    {
        //[ExcelFunction(Description = "SGtoAPI")]
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //public static double SGtoAPI(double sg_oil)
        //{
        //    return (((141.5 / sg_oil) - 131.5));
        //}

        private static double PerforationCoefficient(double densityOfFluid, double diameter, double C, double numberOf)
        {
            return ((0.2369*densityOfFluid)/(Math.Pow(diameter, 4)*Math.Pow(C, 2)*Math.Pow(numberOf, 2)));
        }

        public static void Test()
        {

            //MatrixBuilder<double> M = Matrix<double>.Build;

            double C = 0.8;
            double densityOfFluid = 62.41756; //[lbm/ft^3]



            double Pp1_diameter = 0.4/12.0; //[ft]
            double Pp1_numberOf = 6; //#



            double Pp1 = PerforationCoefficient(densityOfFluid, Pp1_diameter, C, Pp1_numberOf);







        }
    }
}
