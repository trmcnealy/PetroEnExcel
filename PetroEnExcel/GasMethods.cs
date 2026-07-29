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
        internal static double GasPipeFriction_S(double sGGas, double L, double avgZ, double avgT_R, double inclineRad = 0.0)
        {
            return ((0.0375 * sGGas * Math.Sin(1.5707963267948966192313216916398 - inclineRad) * L) / (avgZ * avgT_R));
        }

        [ExcelFunction(Name = "Gas.SinglePhaseBHFP",
                       Description = "GasSinglePhaseBHFP")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GasSinglePhaseBHFP(double sGGas, double md, double D_inch, double flowingWHPressure, double qGas_scfd, double ff, double avgT_R, double inclineRad = 0.0)
        {
            double avgZ = HydrocarbonProperties.GasZKatz((flowingWHPressure+md)/2.0, avgT_R, sGGas, out _, out _, out _, out _);

            double S = GasPipeFriction_S(sGGas, md, avgZ, avgT_R, inclineRad);
            double expS = Math.Exp(-S);
            double P1 = Math.Sqrt((expS*Math.Pow(flowingWHPressure,2)) - (2.685E-3*((ff*avgZ*avgT_R*Math.Pow(qGas_scfd/1000.0,2))/(Math.Sin(1.5707963267948966192313216916398- inclineRad)*Math.Pow(D_inch,5)))*(1.0-expS)));
            double P1New;

            for (int i = 0; i < 10; i++)
            {
                avgZ = HydrocarbonProperties.GasZKatz((flowingWHPressure + P1) / 2.0, avgT_R, sGGas, out _, out _, out _, out _);

                S = GasPipeFriction_S(sGGas, md, avgZ, avgT_R, inclineRad);
                expS = Math.Exp(-S);
                P1New = Math.Sqrt((expS * Math.Pow(flowingWHPressure, 2)) - (2.685E-3 * ((ff * avgZ * avgT_R * Math.Pow(qGas_scfd / 1000.0, 2)) / (Math.Sin(1.5707963267948966192313216916398 - inclineRad) * Math.Pow(D_inch, 5))) * (1.0 - expS)));

                if (Math.Abs(P1 - P1New) < 0.001)
                {
                    P1 = P1New;
                    break;
                }

                P1 = P1New;
            }

            return P1;
        }


        [ExcelFunction(Name = "Gas.SinglePhaseWHFP",
                       Description = "GasSinglePhaseWHFP")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GasSinglePhaseWHFP(double sGGas, double md, double D_inch, double flowingBHPressure, double qGas_scfd, double ff, double avgT_R, double inclineRad = 0.0)
        {
            double avgZ = HydrocarbonProperties.GasZKatz(flowingBHPressure/2.0, avgT_R, sGGas, out _, out _, out _, out _);

            double S = GasPipeFriction_S(sGGas, md, avgZ, avgT_R, inclineRad);
            double expS = Math.Exp(S);

            double P2 = Math.Sqrt((expS*Math.Pow(flowingBHPressure,2))+(((32.0*ff)/(Math.Pow(Math.PI,2)*Math.Pow(D_inch, 5)*gc*Math.Sin(1.5707963267948966192313216916398 - inclineRad))))*((avgZ*avgT_R*qGas_scfd*14.7)/(60+459.67))*(expS-1.0));

            double P2New;

            for (int i = 0; i < 10; i++)
            {
                avgZ = HydrocarbonProperties.GasZKatz((flowingBHPressure + P2) / 2.0, avgT_R, sGGas, out _, out _, out _, out _);

                S = GasPipeFriction_S(sGGas, md, avgZ, avgT_R, inclineRad);
                expS = Math.Exp(S);
                P2New = Math.Sqrt((expS*Math.Pow(flowingBHPressure,2))+(((32.0*ff)/(Math.Pow(Math.PI,2)*Math.Pow(D_inch, 5)*gc*Math.Sin(1.5707963267948966192313216916398 - inclineRad))))*((avgZ*avgT_R*qGas_scfd*14.7)/(60+459.67))*(expS-1.0));

                if (Math.Abs(P2 - P2New) < 0.001)
                {
                    P2 = P2New;
                    break;
                }

                P2 = P2New;
            }

            return P2;
        }


        [ExcelFunction(Name = "Gas.ReynoldsNumber",
                       Description = "Reynold's Number for Gas only")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GasReynoldsNumber(double sGgas, double qGas_scfd, double D_inch, double muGas)
        {
            return (0.02009 * sGgas * qGas_scfd) / (D_inch * muGas);
        }

        [ExcelFunction(Name = "Gas.MoodyFrictionFactor",
                       Description = "Moody's Friction Factor for Gas")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GasMoodyFrictionFactor(double D_inch, double roughness = 0.0006)
        {
            return (2.0 * Math.Log10(3.71 / (roughness / D_inch)));
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
