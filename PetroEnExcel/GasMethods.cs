using System.Runtime.CompilerServices;

using ExcelDna.Integration;

namespace PetroEnExcel
{
    public static partial class Methods
    {
        [ExcelFunction(Description = "StaticBHP")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double StaticBHP(double bhpGuess, double surfacePressure, double sGGas, double depth, double avgT_R)
        {
            double avgZ = HydrocarbonProperties.GasZKatz(((bhpGuess+surfacePressure)/2.0), avgT_R, sGGas, out _, out _, out _, out _);

            double Pbhp = (surfacePressure*Math.Exp((sGGas*depth)/(53.34*avgT_R*avgZ)));
            double PbhpNew;

            for (int i = 0; i < 10; i++)
            {
                avgZ = HydrocarbonProperties.GasZKatz(((Pbhp + surfacePressure) / 2.0), avgT_R, sGGas, out _, out _, out _, out _);
                PbhpNew = (surfacePressure * Math.Exp((sGGas * depth) / (53.34 * avgT_R * avgZ)));

                if (Math.Abs(Pbhp - PbhpNew) < 0.001)
                {
                    Pbhp = PbhpNew;
                    break;
                }

                Pbhp = PbhpNew;
            }

            return Pbhp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal static double GasPipeFriction_S(double sGGas, double L, double avgZ, double avgT, double inclineRad = 1.5707963267948966192313216916398)
        {
            return ((-0.0375 * sGGas * Math.Sin(inclineRad) * L) / (avgZ * avgT));
        }

        [ExcelFunction(Name = "Gas.PipeFriction",
                       Description = "GasPipeFriction")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GasPipeFriction(double sGGas, double md, double D, double flowingWHPressure, double q, double ff, double avgT, double inclineRad = 1.5707963267948966192313216916398)
        {
            double avgZ = HydrocarbonProperties.GasZKatz((flowingWHPressure+md)/2.0, avgT, sGGas, out _, out _, out _, out _);

            double S = GasPipeFriction_S(sGGas, md, avgZ, avgT, inclineRad);
            double expS = Math.Exp(-S);
            double P1 = Math.Sqrt((expS*Math.Pow(flowingWHPressure,2)) - (2.685E-3*((ff*avgZ*avgT*Math.Pow(q/1000.0,2))/(Math.Sin(inclineRad)*Math.Pow(D,5)))*(1.0-expS)));
            double P1New;

            for (int i = 0; i < 10; i++)
            {
                avgZ = HydrocarbonProperties.GasZKatz((flowingWHPressure + P1) / 2.0, avgT, sGGas, out _, out _, out _, out _);

                S = GasPipeFriction_S(sGGas, md, avgZ, avgT, inclineRad);
                expS = Math.Exp(-S);
                P1New = Math.Sqrt((expS * Math.Pow(flowingWHPressure, 2)) - (2.685E-3 * ((ff * avgZ * avgT * Math.Pow(q / 1000.0, 2)) / (Math.Sin(inclineRad) * Math.Pow(D, 5))) * (1.0 - expS)));

                if (Math.Abs(P1 - P1New) < 0.001)
                {
                    P1 = P1New;
                    break;
                }

                P1 = P1New;
            }

            return P1;
        }


        [ExcelFunction(Name = "Gas.ReynoldsNumber",
                       Description = "Reynold's Number for Gas only")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GasReynoldsNumber(double sGgas, double qGas, double D, double muGas)
        {
            return (0.02009 * sGgas * qGas) / (D * muGas);
        }

        [ExcelFunction(Name = "Gas.VolumeToStd",
                       Description = "GasVolumeToStd")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GasVolumeToStd(double gasVolume, double sGGas, double pressure, double temperature)
        {
            const double stdPressure = 14.7;
            const double stdTemperature = 60.0+459.67;

            double stdZ = HydrocarbonProperties.GasZKatz(stdPressure, stdTemperature, sGGas, out _, out _, out _, out _);
            double Z = HydrocarbonProperties.GasZKatz(pressure, temperature, sGGas, out _, out _, out _, out _);

            return ((gasVolume * pressure) / (temperature * Z)) * ((stdTemperature * stdZ) / (stdPressure));
        }

        [ExcelFunction(Name = "Gas.VolumeFromStd",
                       Description = "GasVolumeFromStd")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GasVolumeFromStd(double gasStdVolume, double sGGas, double pressure, double temperature)
        {
            const double stdPressure = 14.7;
            const double stdTemperature = 60.0+459.67;

            double stdZ = HydrocarbonProperties.GasZKatz(stdPressure, stdTemperature, sGGas, out _, out _, out _, out _);
            double Z = HydrocarbonProperties.GasZKatz(pressure, temperature, sGGas, out _, out _, out _, out _);


            return (gasStdVolume * (stdPressure / (stdTemperature * stdZ)) * ((temperature * Z) / pressure));
        }

        [ExcelFunction(Name = "Gas.VoidFraction",
                       Description = "GasVoidFraction")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GasVoidFraction(double sGGas, double pressure, double temperature, double gasStdVolume, double liquidVolume)
        {
            double gasVolume_bbl = GasVolumeFromStd(gasStdVolume, sGGas, pressure, temperature)/5.615;

            return (gasVolume_bbl / (gasVolume_bbl + liquidVolume));
        }

    }
}
