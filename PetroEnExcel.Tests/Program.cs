
using System.Security.Cryptography;

using static PetroEnExcel.Methods;

namespace PetroEnExcel.Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double MD = 10000;//	 ft
            double TubingInnerDiameter = 2.259;//	 in.

            double InterfacialTension = 30;//	 dynes/cm

            double oilAPI = 45.38;//	oAPI
            double sGOil = 141.5/(131.5+oilAPI);
            double sGGas = 0.709;//	 air =1
            double sGWater = 1.05;//	 H2O=1

            double flowingWHPressure = 800;//	 psia
            double flowingWHTemperature = 90;//	oF
            double flowingBHTemperature = 175;//	oF
            double T_R;

            double avgT_R = ((flowingWHTemperature+flowingBHTemperature)/2.0) + 459.67;

            double waterCut = 0;//	 %

            double qLiquid = 2000;//	 stb/day
            double qOil = qLiquid*(1-(waterCut/100.0));
            double qWater = qLiquid-qOil;
            double qGas = 1000000;

            double sGLiquid = (sGOil*(1-(waterCut/100)) + (sGWater*(waterCut/100)));
            double rhoLiquid_lbmft3 = 62.4*sGLiquid;

            //double z = HydrocarbonProperties.GasZKatz(flowingWHPressure, flowingWHTemperature, sGGas);
            //Console.WriteLine($"{z}");

            double Rsi = HydrocarbonProperties.StandingSolutionGasOil(oilAPI, sGGas)/2.0;

            double staticBHP = StaticBHP(MD*0.5, flowingWHPressure, sGGas, MD, avgT_R);

            double oilPb = HydrocarbonProperties.VelardeBubblePoint(flowingWHTemperature, oilAPI, sGGas, Rsi);
            //double Rs = HydrocarbonProperties.VelardeSolutionGasOilRatio(flowingWHPressure, flowingWHTemperature, oilPb, oilAPI, sGGas);

            double muOil = HydrocarbonProperties.KhanOilViscosity(flowingWHPressure, flowingWHTemperature + 459.67, oilPb, sGOil, sGGas, Rsi);
            double muWater = HydrocarbonProperties.McCainWaterViscosity(flowingWHPressure, flowingWHTemperature + 459.67);

            double muLiquid_cP = (muOil*(1-(waterCut/100)) + (sGWater*(waterCut/100)));


            const int numberOfElements = 101;


            double[,] HB = HagedornBrown(MD, TubingInnerDiameter,
                                                   oilPb,
                                                   Rsi,
                                                   sGOil,
                                                   sGGas,
                                                   sGWater,
                                                   InterfacialTension,
                                                   flowingWHPressure,
                                                   flowingWHTemperature,
                                                   flowingBHTemperature,
                                                   qLiquid,
                                                   qWater,
                                                   qGas,
                                                   numberOfElements);


            const int DEPTH = 0;
            const int PRESSURE = 1;
            const int TEMPERATURE = 2;
            const int DENSITY = 3;
            const int LHOLDUP = 4;
            const int USL = 5;
            const int USG = 6;
            //const int COUNT = 7;

            Console.WriteLine("Depth    BHP       BHT       DENSITY   LHOLDUP   USL       USG       ");

            for (int i = 0; i < HB.GetLength(0); i++)
            {
                Console.WriteLine($"{HB[i, DEPTH],-10:N2} {HB[i, PRESSURE],-10:N2} {HB[i, TEMPERATURE],-10:N2} {HB[i, DENSITY],-10:N4} {HB[i, LHOLDUP],-10:N4} {HB[i, USL],-10:N4} {HB[i, USG],-10:N4}");
            }


            {
                double staticBHP2 = StaticBHP(MD, flowingWHPressure, sGGas, MD, avgT_R);

                Console.WriteLine($"BHP={staticBHP2,-10:N2}, Hydro={staticBHP2 - flowingWHPressure,-10:N2}");

                double muGas = muGas = HydrocarbonProperties.CarrKobayashiBurrowsGasViscosity(flowingWHPressure, avgT_R, sGGas);

                double NRe = GasReynoldsNumber(sGGas, qGas, TubingInnerDiameter, muGas);

                double ff = FrictionFactor(NRe);

                Console.WriteLine($"muGas={muGas,-10:N8}, NRe={NRe,-10:N2}, ff={ff,-10:N8}");

                double Pbhp = GasSinglePhaseBHFP(sGGas, MD, TubingInnerDiameter, flowingWHPressure, qGas, ff, avgT_R);

                Console.WriteLine($"avgT={avgT_R,-10:N8}, Gas Pbhp={Pbhp,-10:N8}");
            }

            {
                double gasVolume = 500.0;

                double gasStdVolume = GasVolumeToStd(gasVolume, sGGas, flowingWHPressure, flowingWHTemperature + 459.67);

                double gasVolumeFromStd = GasVolumeFromStd(gasStdVolume, sGGas, flowingWHPressure, flowingWHTemperature + 459.67);

                Console.WriteLine($"gasStdVolume={gasStdVolume,-10:N8}, gasVolumeFromStd={gasVolumeFromStd,-10:N8}");

            }

            {
                double gvf = GasVoidFraction(sGGas, flowingWHPressure, flowingWHTemperature + 459.67, 500000.0, qLiquid);

                Console.WriteLine($"qstd=500000.0 @{flowingWHPressure}, gvf={Math.Round(gvf * 100, 2):N}%");

                T_R = flowingBHTemperature + 459.67;
                gvf = GasVoidFraction(sGGas, HB[HB.GetLength(0) - 1, PRESSURE], T_R, 500000.0, qLiquid);

                Console.WriteLine($"qstd=500000.0 @{HB[HB.GetLength(0) - 1, PRESSURE]}, gvf={Math.Round(gvf * 100, 2):N}%");
            }

            {
                T_R = flowingBHTemperature + 459.67;
                double vg = Turner.GasVelocityWater(sGGas, HB[HB.GetLength(0) - 1, PRESSURE], T_R);

                Console.WriteLine($"vg={vg:N}");
            }



            double NreL = LiquidReynoldsNumber(qLiquid, rhoLiquid_lbmft3, muLiquid_cP, TubingInnerDiameter);
            Console.WriteLine($"NreL={NreL:N}");


            double qLiquid_bblmin = qLiquid/1440.0;
            Console.WriteLine($"qLiquid_bblmin={qLiquid_bblmin:N}");

            double Ppf = NewtonianPipeFriction(NreL, rhoLiquid_lbmft3, qLiquid_bblmin, TubingInnerDiameter/12.0, MD);
            Console.WriteLine($"Ppf={Ppf:N}");



            double q_galmin = qLiquid_bblmin*42;
            Console.WriteLine($"q_galmin={q_galmin:N}");

            Ppf = HazenWilliamsPipeFriction(qLiquid, TubingInnerDiameter, MD);
            Console.WriteLine($"Ppf={Ppf:N}");






            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
