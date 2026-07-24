using System.Runtime.CompilerServices;

using ExcelDna.Integration;

namespace PetroEnExcel
{
    public static partial class Methods
    {
        private const double gc = 32.174;

        //' SGtoAPI
        //' @export
        //' @param sg_oil []
        //' @returns A numeric scalar
        [ExcelFunction(Description = "SGtoAPI")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double SGtoAPI(double sg_oil)
        {
            return (((141.5 / sg_oil) - 131.5));
        }

        //' APItoSG
        //' @export
        //' @param oilAPI []
        //' @returns A numeric scalar
        [ExcelFunction(Description = "APItoSG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double APItoSG(double oilAPI)
        {
            return (141.5 / (131.5 + oilAPI));
        }


        internal static class HagedornBrownFuncs
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double MassFlowRate(double sGLiquid, double sGGas, double qLiquid, double qGas)
            {
                if (qLiquid == 0.0)
                { return (0.0765 * sGGas * qGas); }

                return (sGLiquid * 62.4 * qLiquid * 5.615) + (0.0765 * sGGas * qGas);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double SGLiquid(double sGOil, double sGWater, double qLiquid, double qWater, double woc)
            {
                if (qLiquid == 0.0)
                {
                    if (qWater > 0.0)
                    {
                        qLiquid = (qWater / (woc / 100)) + qWater;
                        return (((qLiquid - qWater) * sGOil) + (qWater * sGWater)) / qLiquid;
                    }
                    else
                    {
                        return ((sGOil * (1.0 - woc)) + (woc * sGWater));
                    }
                }

                return (((qLiquid - qWater) * sGOil) + (qWater * sGWater)) / qLiquid;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double usg(double Pressure, double Temperature, double Z, double tubingIDArea, double QGas)
            {
                return (QGas / (tubingIDArea * 86400.0)) * Z * ((459.67 + Temperature) / (459.67 + 60.0)) * (14.7 / Pressure);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double usl(double tubingIDArea, double qLiquid)
            {
                if (qLiquid == 0.0)
                { return 0.0; }

                return qLiquid * 5.615 / 86400.0 / tubingIDArea;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double um(double usg, double usl)
            {
                return usg + usl;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double Nvl(double sGliquid, double usl, double interfacialTension)
            {
                return 1.938 * usl * Math.Pow((62.4 * sGliquid / interfacialTension), 0.25);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double Nvg(double sGliquid, double interfacialTension, double usg)
            {
                return 1.938 * usg * Math.Pow((62.4 * sGliquid / interfacialTension), 0.25);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double ND(double tubingID, double sGliquid, double interfacialTension)
            {
                return 120.872 * tubingID / 12.0 * Math.Sqrt(62.4 * sGliquid / interfacialTension);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double MuLiquid(double WOC, double muOil, double muWater = 1.0)
            {
                return (muOil * (1.0 - WOC)) + (muWater * WOC);

                //if (qLiquid == 0.0)
                //{
                //    return (muOil + muWater) / 2;
                //} else if (QWater == 0.0)
                //{
                //    return muOil;
                //}

                //return ((muOil * (qLiquid - QWater)) + (0.5 * QWater)) / qLiquid;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double NL(double muLiquid, double sGliquid, double interfacialTension)
            {
                return 0.15726 * muLiquid * Math.Pow((1.0 / (62.4 * sGliquid * Math.Pow(interfacialTension, 3.0))), 0.25);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double CNL(double NL)
            {
                const double A1 = -2.698510E+00;
                const double A2 = 1.584095E-01;
                const double A3 = -5.509976E-01;
                const double A4 = 5.478492E-01;
                const double A5 = -1.219458E-01;

                return Math.Pow(10, (A1 +
                                     A2 * (Math.Log10(NL) + 3.0) +
                                     A3 * Math.Pow((Math.Log10(NL) + 3.0), 2.0) +
                                     A4 * Math.Pow((Math.Log10(NL) + 3.0), 3.0) +
                                     A5 * Math.Pow((Math.Log10(NL) + 3.0), 4.0)));
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double NvlNvg(double Pressure, double Nvl, double Nvg, double Nd, double CNl)
            {
                return Nvl / Math.Pow(Nvg, 0.575) * Math.Pow((Pressure / 14.7), 0.1) * CNl / Nd;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double HLphi(double NvlNvg)
            {
                const double A1 = -1.030658E-01;
                const double A2 = 6.177740E-01;
                const double A3 = -6.329460E-01;
                const double A4 = 2.959800E-01;
                const double A5 = -4.010000E-02;

                return A1 +
                       A2 * (Math.Log10(NvlNvg) + 6.0) +
                       A3 * Math.Pow((Math.Log10(NvlNvg) + 6.0), 2.0) +
                       A4 * Math.Pow((Math.Log10(NvlNvg) + 6.0), 3.0) +
                       A5 * Math.Pow((Math.Log10(NvlNvg) + 6.0), 4.0);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double NvgNd(double NL, double Nvg, double Nd)
            {
                return Nvg * Math.Pow(NL, 0.38) / Math.Pow(Nd, 2.14);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double Index(double NvgNd)
            {
                return (NvgNd - 0.012) / Math.Abs(NvgNd - 0.012);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double Modified(double NvgNd, double Index)
            {
                return (1.0 - Index) / 2.0 * 0.012 + (1.0 + Index) / 2.0 * NvgNd;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double phi(double Modified)
            {
                const double A1 = 9.116257E-01;
                const double A2 = -4.821756E+00;
                const double A3 = 1.232250E+03;
                const double A4 = -2.225358E+04;
                const double A5 = 1.161743E+05;

                return A1 +
                       A2 * Modified +
                       A3 * Math.Pow(Modified, 2.0) +
                       A4 * Math.Pow(Modified, 3.0) +
                       A5 * Math.Pow(Modified, 4.0);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double HL(double hLphi, double phi)
            {
                return hLphi * phi;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double NRe(double tubingID, double massRate, double muLiquid, double hL, double muGas)
            {
                //lbm/day
                return 0.022 * massRate / (tubingID * Math.Pow(muLiquid, hL) * Math.Pow(muGas, (1.0 - hL)));
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double f(double NRe, double roughness = 0.0006)
            {
                return 1.0 / Math.Pow((-4.0 * Math.Log10(roughness / 3.7065 - 5.0452 / NRe * Math.Log10(Math.Pow(roughness, 1.1098) / 2.8257 + Math.Pow((7.149 / NRe), 0.8981)))), 2.0);
            }

            // (lbm/ft3)
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double rhog(double Pressure, double Temperature, double Z, double sGGas)
            {

                return 28.97 * sGGas * Pressure / Z / 10.73 / (459.67 + Temperature);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static double AverageRho(double sGliquid, double hL, double rhog)
            {
                return (hL * sGliquid * 62.4) + ((1.0 - hL) * rhog);
            }
        }





        [ExcelFunction(Name = "HagedornBrowndPdZ",
                        Description = "dPdZ",
                        HelpTopic = "DocTest-AddIn.chm!1002")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal static double HagedornBrowndPdZ(double pressure, double temperature, double Z,
                                                 double tubingID, double tubingIDArea,
                                                 double sGliquid, double sGGas,
                                                 double muOil, double muGas, double muWater,
                                                 double interfacialTension, double WOC,
                                                 double QGas, double QLiquid, double QWater,
                                                 out double rhog, out double hL, out double usl, out double usg,
                                                 double roughness = 0.0006)
        {
            double massRate = HagedornBrownFuncs.MassFlowRate(sGliquid, sGGas, QLiquid, QGas);
            usl = HagedornBrownFuncs.usl(tubingIDArea, QLiquid);
            usg = HagedornBrownFuncs.usg(pressure, temperature, Z, tubingIDArea, QGas);
            double um = HagedornBrownFuncs.um(usg, usl);
            double Nvl = HagedornBrownFuncs.Nvl(sGliquid, usl, interfacialTension);
            double Nvg = HagedornBrownFuncs.Nvg(sGliquid, interfacialTension, usg);
            double ND = HagedornBrownFuncs.ND(tubingID, sGliquid, interfacialTension);
            double muLiquid = HagedornBrownFuncs.MuLiquid(WOC, muOil, muWater);
            double NL = HagedornBrownFuncs.NL(muLiquid, sGliquid, interfacialTension);
            double CNL = HagedornBrownFuncs.CNL(NL);
            double NvlNvg = HagedornBrownFuncs.NvlNvg(pressure, Nvl, Nvg, ND, CNL);
            double hLphi = HagedornBrownFuncs.HLphi(NvlNvg);
            double NvgNd = HagedornBrownFuncs.NvgNd(NL, Nvg, ND);
            double Index = HagedornBrownFuncs.Index(NvgNd);
            double Modified = HagedornBrownFuncs.Modified(NvgNd, Index);
            double phi = HagedornBrownFuncs.phi(Modified);
            hL = HagedornBrownFuncs.HL(hLphi, phi);
            double NRe = HagedornBrownFuncs.NRe(tubingID, massRate, muLiquid, hL, muGas);
            double f = HagedornBrownFuncs.f(NRe, roughness);
            rhog = HagedornBrownFuncs.rhog(pressure, temperature, Z, sGGas);
            double avgRho = HagedornBrownFuncs.AverageRho(sGliquid, hL, rhog);

            //Debug.WriteLine($"{massRate:F4} {usg:F4} {usl:F4} {um:F4} {Nvl:F4} {Nvg:F4} {ND:F4} {MuLiquid:F4} {NL:F4} {CNL:F4} {NvlNvg:F4} {hLphi:F4} {NvgNd:F4} {Index:F4} {Modified:F4} {phi:F4} {hL:F4} {NRe:F4} {f:F4} {rhog:F4} {avgRho:F4}");
            //(1.0 / 144.0)=0.006944444
            return 0.006944444 * (avgRho + f * Math.Pow(massRate, 2) / 7.413 / 10000000000.0 / Math.Pow((tubingID / 12.0), 5) / avgRho);
        }

        [ExcelFunction(Name = "HagedornBrown",
                       Description = "Hagedorn Brown")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double[,] HagedornBrown([ExcelArgument(Name = "totalMD", Description = "totalMD")] double totalMD,
                                              [ExcelArgument(Name = "tubingID")] double tubingID,
                                              [ExcelArgument(Name = "oilPb")] double oilPb,
                                              [ExcelArgument(Name = "Rsi")] double Rsi,
                                              [ExcelArgument(Name = "sGOil")] double sGOil,
                                              [ExcelArgument(Name = "sGGas")] double sGGas,
                                              [ExcelArgument(Name = "sGWater")] double sGWater,
                                              [ExcelArgument(Name = "interfacialTension")] double interfacialTension,
                                              [ExcelArgument(Name = "flowingTubingHeadPressure")] double flowingTubingHeadPressure,
                                              [ExcelArgument(Name = "flowingTubingHeadTemperature")] double flowingTubingHeadTemperature,
                                              [ExcelArgument(Name = "flowingBHTemperature")] double flowingBHTemperature,
                                              [ExcelArgument(Name = "qLiquid")] double qLiquid,
                                              [ExcelArgument(Name = "qWater")] double qWater,
                                              [ExcelArgument(Name = "qGas")] double qGas,
                                              [ExcelArgument(Name = "numberOfElements")] int numberOfElements = 100)
        {
            const int DEPTH = 0;
            const int PRESSURE = 1;
            const int TEMPERATURE = 2;
            const int DENSITY = 3;
            const int LHOLDUP = 4;
            const int USL = 5;
            const int USG = 6;
            const int COUNT = 7;

            double[,] Output = new double[numberOfElements, COUNT];
            //double[] Pwellbore = new double[numberOfElements];
            //double[] Twellbore = new double[numberOfElements];

            double Z;
            double dPdZ;
            double muGas;
            double muOil;
            double muWater;
            double muLiquid;
            double sGliquid;

            Output[0, PRESSURE] = flowingTubingHeadPressure;
            Output[0, TEMPERATURE] = flowingTubingHeadTemperature;

            double Salinity = 0.0;
            double roughness = 0.0006;

            double waterCut = (qWater==0.0||qLiquid==0.0)?0.5:(100*qWater/qLiquid);

            // ft^2
            double tubingIDArea = (Math.PI/4.0)*Math.Pow((tubingID/12.0),2);

            Z = HydrocarbonProperties.GasZKatz(Output[0, PRESSURE], (Output[0, TEMPERATURE] + 459.67), sGGas, out double Ppc, out double Tpc, out double PR, out double TR);

            muGas = HydrocarbonProperties.CarrKobayashiBurrowsGasViscosity(Output[0, PRESSURE], Output[0, TEMPERATURE] + 459.67, sGGas);
            muOil = HydrocarbonProperties.KhanOilViscosity(Output[0, PRESSURE], Output[0, TEMPERATURE] + 459.67, oilPb, sGOil, sGGas, Rsi);
            muWater = HydrocarbonProperties.McCainWaterViscosity(Output[0, PRESSURE], Output[0, TEMPERATURE] + 459.67, Salinity);

            sGliquid = HagedornBrownFuncs.SGLiquid(sGOil, sGWater, qLiquid, qWater, waterCut);
            muLiquid = HagedornBrownFuncs.MuLiquid(qWater / qLiquid, muOil, muWater);

            dPdZ = HagedornBrowndPdZ(Output[0, PRESSURE], Output[0, TEMPERATURE], Z,
                                     tubingID, tubingIDArea,
                                     sGliquid, sGGas,
                                     muOil, muGas, muWater,
                                     interfacialTension, waterCut,
                                     qGas, qLiquid, qWater,
                                     out double rhog,
                                     out double hL,
                                     out double usl,
                                     out double usg,
                                     roughness);

            Output[0, DENSITY] = rhog;
            Output[0, LHOLDUP] = hL;
            Output[0, USL] = usl;
            Output[0, USG] = usg;

            for (int i = 1; i < numberOfElements; i++)
            {
                Output[i, DEPTH] = Output[i - 1, DEPTH] + (totalMD / (numberOfElements - 1));

                Z = HydrocarbonProperties.GasZKatz(Output[i - 1, PRESSURE], (Output[i - 1, TEMPERATURE] + 459.67), sGGas, out _, out _, out _, out _);

                muGas = HydrocarbonProperties.CarrKobayashiBurrowsGasViscosity(Output[i - 1, PRESSURE], Output[i - 1, TEMPERATURE] + 459.67, sGGas);
                muOil = HydrocarbonProperties.KhanOilViscosity(Output[i - 1, PRESSURE], Output[i - 1, TEMPERATURE] + 459.67, oilPb, sGOil, sGGas, Rsi);

                sGliquid = HagedornBrownFuncs.SGLiquid(sGOil, sGWater, qLiquid, qWater, waterCut);
                muLiquid = HagedornBrownFuncs.MuLiquid(waterCut, muOil, muWater);

                dPdZ = HagedornBrowndPdZ(Output[i - 1, PRESSURE], Output[i - 1, TEMPERATURE], Z,
                                         tubingID,
                                         tubingIDArea,
                                         sGliquid,
                                         sGGas,
                                         muOil,
                                         muGas,
                                         muWater,
                                         interfacialTension,
                                         waterCut,
                                         qGas,
                                         qLiquid,
                                         qWater,
                                         out rhog,
                                         out hL,
                                         out usl,
                                         out usg,
                                         roughness);

                Output[i, PRESSURE] = Output[i - 1, PRESSURE] + (dPdZ * (Output[i, DEPTH] - Output[i - 1, DEPTH]));
                Output[i, TEMPERATURE] = (flowingTubingHeadTemperature + ((flowingBHTemperature - flowingTubingHeadTemperature) / totalMD) * Output[i, DEPTH]);
                Output[i, DENSITY] = rhog;
                Output[i, LHOLDUP] = hL;
                Output[i, USL] = usl;
                Output[i, USG] = usg;
            }

            return Output;
        }

        [ExcelFunction(Name = "Liquid.ReynoldsNumber",
                       Description = "Reynold's Number for Liquids")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double LiquidReynoldsNumber(double qLiquid_stbd, double rhoLiquid_lbmft3, double muLiquid_cP, double pipeDiameter_in)
        {
            return ((1.48 * qLiquid_stbd * rhoLiquid_lbmft3) / (muLiquid_cP * pipeDiameter_in));
        }

        [ExcelFunction(Description = "Moody/Chen Friction Factor")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double FrictionFactor(double NRe, double roughness = 0.0006)
        {
            return Math.Pow(1.0 / (-4.0 * Math.Log10((roughness / 3.7065) - ((5.0452 / NRe) * Math.Log10((Math.Pow(roughness, 1.1098) / 2.8257) + Math.Pow((7.149 / NRe), 0.8981))))), 2);
        }

        [ExcelFunction(Description = "NewtonianPipeFriction")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double NewtonianPipeFriction(double NRe, double rho_lbmft3, double qLiquid_bblmin, double pipeDiameter_ft, double pipeLength_ft, double roughness = 0.0006)
        {
            double ff = FrictionFactor(NRe, roughness);

            double area_ft2 = (Math.PI/4)*Math.Pow(pipeDiameter_ft, 2);

            double velocity_fts = (5.615*(qLiquid_bblmin/60.0)) / area_ft2;

            return (2.0 * ff * rho_lbmft3 * Math.Pow(velocity_fts, 2) * pipeLength_ft) / (144.0 * gc * pipeDiameter_ft);
        }


        [ExcelFunction(Description = "HazenWilliamsPipeFriction")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double HazenWilliamsPipeFriction(double q_bblday, double pipeDiameter_in, double pipeLength_ft, double roughness = 100)
        {
            double area_ft2 = (Math.PI/4)*Math.Pow(pipeDiameter_in/12.0, 2);

            double velocity_fts = q_bblday*(5.615/86400.0)/area_ft2;

            return 0.433 * pipeLength_ft * Math.Pow((velocity_fts) / (1.32 * roughness * Math.Pow((pipeDiameter_in / 48.0), 0.63)), 1.851851);
        }
    }
}
