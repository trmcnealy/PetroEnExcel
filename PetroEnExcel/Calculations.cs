using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroEnExcel
{
    internal static class Calculations
    {

        internal delegate double IntegratorFunc(double arg, params double[] args);

        private static double RungeKuttaIntegrator(IntegratorFunc func, params double[] args)
        {
            double result = func(0.0, args);





            return 0.0;
        }
    }
}
