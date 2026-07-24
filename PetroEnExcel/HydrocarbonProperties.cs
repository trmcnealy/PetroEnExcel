using System.Runtime.CompilerServices;

using ExcelDna.Integration;

namespace PetroEnExcel
{

    //γ gas=
    //Pipe Diameter[in]=
    //Area[ft^2]=
    //MD[ft]=

    //Wellhead
    //P[psig]=
    //T[°F]=
    //P[psia]=
    //T[°R]=
    //Z=
    //Bottomhole
    //T[°F]=
    //Pi
    //Static P[psia]=
    //error
    //T[°R]=
    //Z=
    //Avg
    //P[psia]=
    //T[°R]=
    //Z=


    public static class HydrocarbonProperties
    {

        //Public Function StandingGasPseudoCriticalPressure(GASGRAVITY As Double) As Double
        //    StandingGasPseudoCriticalPressure = 787.06 - (147.34 * GASGRAVITY) - (7.916 * GASGRAVITY ^ 2)
        //}

        //Public Function StandingGasPseudoCriticalTemperature(ByVal GASGRAVITY As Double) As Double
        //    StandingGasPseudoCriticalTemperature = 168// + (325// * GASGRAVITY) - (12.5 * GASGRAVITY ^ 2)
        //}


        //Public Function KatzGasPseudoCriticalPressure(ByVal GASGRAVITY As Double) As Double
        //    KatzGasPseudoCriticalPressure = 709.604 - (58.718 * GASGRAVITY)
        //}

        //Public Function KatzGasPseudoCriticalTemperature(ByVal GASGRAVITY As Double) As Double
        //    KatzGasPseudoCriticalTemperature = 170.491 + (307.344 * GASGRAVITY)
        //}

        [ExcelFunction(Description = "SuttonAssociatedGasPseudoCriticalPressure")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double SuttonAssociatedGasPseudoCriticalPressure(in double GASGRAVITY)
        {
            return 671.1 + (14.0 * GASGRAVITY) - (34.3 * Math.Pow(GASGRAVITY, 2));
        }

        [ExcelFunction(Description = "SuttonAssociatedGasPseudoCriticalTemperature")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double SuttonAssociatedGasPseudoCriticalTemperature(in double GASGRAVITY)
        {
            return 120.1 + (429.0 * GASGRAVITY) - (62.9 * Math.Pow(GASGRAVITY, 2));
        }

        [ExcelFunction(Description = "Gas ZFactor Katz Reduced")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GasZKatzReduced(in double PR, in double TR)
        {
            const double A1 = 0.31506237;
            const double A2 = 1.0467099;
            const double A3 = 0.57832729;
            const double A4 = 0.53530771;
            const double A5 = 0.61232032;
            const double A6 = 0.10488813;
            const double A7 = 0.68157001;
            const double A8 = 0.68446549;

            double DR=1.0, DR1, DR2, T1, T2, T3, T4, T5, T6, P, DP, Z=0.0;

            if ((TR >= 1.05) && (TR <= 3.0000000001))
            { }
            else
            {
                throw new IndexOutOfRangeException("(TR < 1.05) Or (TR > 3//).");
            }

            if (PR > 15.0)
            {
                throw new IndexOutOfRangeException("(PR < 0) Or (PR > 15//).");
            }

            if (PR <= 0)
            {
                return 1;
            }

            for (int i = 1; i < 10; ++i)
            {
                DR2 = DR * DR;
                T1 = ((A1 * TR) - A2 - (A3 / Math.Pow(TR, 2))) * DR;
                T2 = ((A4 * TR) - A5) * DR2;
                T3 = A5 * A6 * Math.Pow(DR, 5);
                T4 = A7 * (DR2 / Math.Pow(TR, 2));
                T5 = A8 * DR2;
                T6 = Math.Exp(-T5);
                P = ((TR + T1 + T2 + T3) * DR) + (T4 * DR * (1.0 + T5) * T6);
                DP = TR + (2.0 * T1) + (3.0 * T2) + (6.0 * T3) + (T4 * T6 * (3.0 + (3.0 * T5) - (2.0 * Math.Pow(T5, 2))));
                DR1 = DR - ((P - (0.27 * PR)) / DP);

                if (DR1 <= 0)
                {
                    DR1 = 0.5 * DR;
                }

                if (DR1 >= 2.2)
                {
                    DR1 = DR + 0.9 * (2.2 - DR);
                }

                if ((Math.Abs(DR - DR1) - 0.00001) < 0)
                {
                    Z = (0.27 * PR) / (DR1 * TR);
                    break;
                }

                DR = DR1;
            }

            return Z;
        }

        [ExcelFunction(Description = "Gas Z Factor Katz")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GasZKatz(in double PRESSURE, in double TEMPERATURE, in double GASGRAVITY, [ExcelArgument(Name="Ppc", AllowReference=true)] out double Ppc, [ExcelArgument(Name="Tpc", AllowReference=true)] out double Tpc, [ExcelArgument(Name="PR", AllowReference=true)] out double PR, [ExcelArgument(Name="TR", AllowReference=true)] out double TR)
        {
            Ppc = SuttonAssociatedGasPseudoCriticalPressure(GASGRAVITY);
            Tpc = SuttonAssociatedGasPseudoCriticalTemperature(GASGRAVITY);

            PR = (PRESSURE / Ppc);
            TR = (TEMPERATURE / Tpc);

            return GasZKatzReduced(PR, TR);

        }

        ////' EulerConstant
        private const double EulerConstant = 1.781; //gamma

        ////Processing Functions
        ////' RF
        private const double RF = 0.15;


        ////Functions
        ////' pow
        ////' @param x x
        ////' @param y y
        //pow = function(x, y) {
        //  z = x^y
        //  return(z)
        //}

        ////' FtoR
        ////' @param TempF TempF
        //FtoR = function(TempF) {
        //  return(TempF + 459.67)
        //}
        ////' CentralDifferenceMethod_h
        //CentralDifferenceMethod_h = 0.00001 //.Machine$double.eps

        ////' CentralDifferenceMethod
        ////' @param func func
        ////' @param x x
        ////' @param ... parameters
        //CentralDifferenceMethod = function(func, x, ...) {
        //  dF = fderiv(func,
        //               x,
        //               n = 1,
        //               h = 0,
        //               method = "central",
        //               ...)
        //  return (dF)
        //}

        ////' BulkVolume
        ////' @export
        ////' @param Xf [ft]
        ////' @param HorizontalLength [ft]
        ////' @param height [ft]
        ////' @returns a volume [ft^3]
        //BulkVolume = function(Xf, HorizontalLength, height) {
        //  Vb = ((2 * Xf) * HorizontalLength * height) //ft^3
        //  return (Vb) // / 5.615 bbl
        //}

        ////' PoreVolume
        ////' @export
        ////' @param Vb [ft^3]
        ////' @param porosity [V/V]
        ////' @returns a volume [ft^3]
        //PoreVolume = function(Vb, porosity) {
        //  return (Vb * porosity)
        //}

        ////' MolecularWeightOil
        ////' @export
        ////' @param T_R []
        ////' @param SGo []
        ////' @returns A numeric scalar
        //MolecularWeightOil = function(T_R, SGo) {
        //  return (4.5673E-5 * (T_R^2.1962) * (SGo^-1.0164))
        //  //Kw=Watson characterization factor, R1/3
        //  // Kw = ((T_R^3) / SGo)
        //  // return (((Kw * SGo^0.84573) / 4.5579)^6.58848)
        //}

        ////' OilDensity
        ////' @export
        ////' @param Rs []
        ////' @param Bo []
        ////' @param SGo []
        ////' @param SGg []
        ////' @returns A numeric scalar
        //OilDensity = function(Rs, Bo, SGo, SGg) {
        //  return ((((62.42796 * SGo) + (0.0136 * SGg * Rs)) / Bo))//*(5.615/42))
        //}

        ////' WolfcampBubblepointPressure(Pbp)
        ////' @export
        ////' @param T_F []
        ////' @param SGo_API []
        ////' @param SGg []
        ////' @param Rs []
        ////' @returns A numeric scalar
        //WolfcampBubblepointPressure = function(T_F, SGo_API, SGg, Rs) {
        //  a1 = 1.42828E-10
        //  a2 = 2.844591797
        //  a3 = -6.74896E-04
        //  a4 = 1.225226436
        //  a5 = 0.033383304
        //  a6 = -0.272945957
        //  a7 = -0.084226069
        //  a8 = 1.869979257
        //  a9 = 1.221486524
        //  a10 = 1.370508349
        //  a11 = 0.011688308

        //  A = ((a1 * (T_F^a2)) + (a3 * (SGo_API^a4))) / ((a5 + ((2 * (
        //    Rs^a6
        //  )) / (SGg^a7)))^2)

        //  Pbp = a8 * ((((Rs^a9) / (SGg^a10)) * 10^A) + a11)

        //  return (Pbp)
        //}

        ////' WolfcampFVF(Bobp)
        ////' @export
        ////' @param T_F []
        ////' @param SGo_API []
        ////' @param SGg []
        ////' @param Rs []
        ////' @returns A numeric scalar
        //WolfcampFVF = function(T_F, SGo_API, SGg, Rs) {
        //  a1 = 2.510755E+00
        //  a2 = -4.852538E+00
        //  a3 = 1.183500E+01
        //  a4 = 1.365428E+05
        //  a5 = 2.252880E+00
        //  a6 = 1.007190E+01
        //  a7 = 4.450849E-01
        //  a8 = 5.352624E+00
        //  a9 = -6.309052E-01
        //  a10 = 9.000749E-01
        //  a11 = 9.871766E-01
        //  a12 = 7.865146E-04
        //  a13 = 2.689173E-06
        //  a14 = 1.100001E-05

        //  SGo = APItoSG(SGo_API)
        //  T_60 = T_F - 60

        //  A = (((((
        //    Rs^a1
        //  ) * SGg^a2) / (SGo^a3)) + (a4 * (T_60)^a5) + (a6 * Rs))^a7) / (a8 + (((2 *
        //                                                                           Rs^a9) / (SGg^a10)) * (T_60)))^2

        //  Bobp = a11 + (a12 * A) + (a13 * A^2) + (a14 * (T_60) * (SGo_API / SGg))

        //  return (Bobp)
        //}

        ////' WolfcampOilCompressibility(Cobp)
        ////' @export
        ////' @param T_F []
        ////' @param SGo_API []
        ////' @param SGg []
        ////' @param Rs []
        ////' @returns A numeric scalar
        //WolfcampOilCompressibility = function(T_F, SGo_API, SGg, Rs) {
        //  a1 = 0.980922372
        //  a2 = 0.021003077
        //  a3 = 0.338486128
        //  a4 = 20.00006358
        //  a5 = 0.300001059
        //  a6 = -0.876813622
        //  a7 = 1.759732076
        //  a8 = 2.749114986
        //  a9 = -1.713572145
        //  a10 = 9.999932841
        //  a11 = 4.487462368
        //  a12 = 0.005197040
        //  a13 = 0.000012580

        //  SGo = APItoSG(SGo_API)

        //  A = ((((Rs^a1 * SGg^a2) / SGo^a3) + (a4 * (T_F - 60)^a5) + (a6 * Rs))^a7) /
        //    (a8 + (((2 * Rs^a9) / (SGg^a10)) * (T_F - 60)))^2

        //  Cobp = (a11 + (a12 * A) + (a13 * A^2)) * 10^-6

        //  return (Cobp)
        //}

        ////' WolfcampBoi(Boi)
        ////' @export
        ////' @param Bobp []
        ////' @param Cobp []
        ////' @param Pi []
        ////' @param Pbp []
        ////' @returns A numeric scalar
        //WolfcampBoi = function(Bobp, Cobp, Pi, Pbp) {
        //  return (Bobp * exp(-Cobp * (Pi - Pbp)))
        //}

        //' VelardeBubblePoint
        //' @export
        //' @param T_F []
        //' @param SGo_API []
        //' @param SGg []
        //' @param Rs []
        //' @returns A numeric scalar

        [ExcelFunction(Description = "VelardeBubblePoint")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double VelardeBubblePoint(double T_F, double SGo_API, double SGg, double Rs)
        {
            double x = (0.013098 * Math.Pow(T_F, 0.282372)) - (8.2E-6 * Math.Pow(SGo_API, 2.176124));
            double Pbp = 1091.47 * Math.Pow(((Math.Pow(Rs, 0.081465) * Math.Pow(SGg, -0.161488) * Math.Pow(10, x) - 0.740152)), 5.354891);
            return (Pbp);
        }

        //' VelardeSolutionGasOilRatioBubblePoint(Rsb)
        //' @export
        //' @param Temperature []
        //' @param Pbp []
        //' @param SGo []
        //' @param SGg []
        //' @returns A numeric scalar
        [ExcelFunction(Description = "VelardeSolutionGasOilRatioBubblePoint")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double VelardeSolutionGasOilRatioBubblePoint(double Temperature, double Pbp, double SGo, double SGg)
        {
            double x = ((0.013098 * Math.Pow(Temperature,0.282372)) - (8.2E-6 * Math.Pow(SGo,2.176124)));
            return (2 * Math.Pow(((Math.Pow((Pbp / 1091.47), (1 / 5.35489)) + 0.740152) / (Math.Pow(SGg, -0.161488) * Math.Pow(10, x))), (1 / 0.081465)));
        }

        //' VelardeSolutionGasOilRatio(Rs)
        //' @export
        //' @param Pressure []
        //' @param Temperature []
        //' @param Pbp []
        //' @param SGo_API []
        //' @param SGg []
        //' @returns A numeric scalar
        [ExcelFunction(Description = "VelardeSolutionGasOilRatio")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double VelardeSolutionGasOilRatio(double Pressure, double Temperature, double Pbp, double SGo_API, double SGg)
        {

            double Rsb = VelardeSolutionGasOilRatioBubblePoint(Temperature, Pbp, Methods.APItoSG(SGo_API), SGg);

            if (Pressure >= Pbp)
            {
                return (Rsb);
            }

            double pr = (Pressure / Pbp);
            // print(paste0("pr=",pr))

            const double A0 = 9.73E-7;
            const double A1 = 1.672608;
            const double A2 = 0.929870;
            const double A3 = 0.247235;
            const double A4 = 1.056052;

            const double B0 = 0.022339;
            const double B1 = -1.004750;
            const double B2 = 0.337711;
            const double B3 = 0.132795;
            const double B4 = 0.302065;

            const double C0 = 0.725167;
            const double C1 = -1.485480;
            const double C2 = -0.164741;
            const double C3 = -0.091330;
            const double C4 = 0.047094;

            double a_1 = A0 * Math.Pow(SGg, A1) * Math.Pow(SGo_API, A2) * Math.Pow(Temperature, A3) * Math.Pow(Pbp, A4);
            double a_2 = B0 * Math.Pow(SGg, B1) * Math.Pow(SGo_API, B2) * Math.Pow(Temperature, B3) * Math.Pow(Pbp, B4);
            double a_3 = C0 * Math.Pow(SGg, C1) * Math.Pow(SGo_API, C2) * Math.Pow(Temperature, C3) * Math.Pow(Pbp, C4);

            // print(paste0("a1=",a1))
            // print(paste0("a2=",a2))
            // print(paste0("a3=",a3))

            double Rsr = ((a_1 * Math.Pow(pr, a_2)) + ((1 - a_1) * Math.Pow(pr, a_3)));
            // print(paste0("Rsr=",Rsr))

            return (Rsr * Rsb);
        }


        ////' PetroskyFarshadOilCompressibility
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param Rs []
        ////' @param SGo_API []
        ////' @param SGg []
        ////' @returns A numeric scalar
        //PetroskyFarshadOilCompressibility = function(Pressure, Temperature, Rs, SGo_API, SGg) {
        //  a = 1.705e-7
        //  b = 0.69357
        //  c = 0.1885
        //  d = 0.3272
        //  e = 0.6729
        //  f = -0.5906

        //  return (a * pow(Rs, b) * pow(SGg, c) * pow(SGo_API, d) * pow(Temperature, e) * pow(Pressure, f))
        //}

        ////' PetroskySaturatedOilCompressibility
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param Rs []
        ////' @param SGo_API []
        ////' @param SGg []
        ////' @returns A numeric scalar
        //PetroskySaturatedOilCompressibility = function(Pressure, Temperature, Rs, SGo_API, SGg){
        //  T_R = FtoR(Temperature)

        //  return ((1.705e-7 * pow(Rs, 0.69357) * pow(SGg, 0.1885) * pow(SGo_API, 0.3272) * pow(T_R, 0.6729)) * pow(Pressure, -0.5906))
        //}

        ////API =Oil Gravity, 0API
        ////P =Pressure, psia
        ////Pb =Bubble Point Pressure, psia
        ////RS =Solution Gas-Oil Ratio, SCF/STB
        ////T =Temperature, F
        ////gammaG =Gas Relative Density (air = 1)
        ////gammaO =141.5/(0API + 131.5) =Oil Relative Density at 14.7 psia and 60F
        ////thetaR =(T+459.67)/459.67 =Relative Temperature
        ////muA =Oil Viscosity Above the Bubble Point, cp
        ////muB =Oil Viscosity Below the Bubble Point, cp
        ////muOB =Bubble Point Oil Viscosity, cp


        //' KhanOilViscosity_ob
        //' @export
        //' @param T_R []
        //' @param gamma_o []
        //' @param gamma_g []
        //' @param Rs []
        //' @returns A numeric scalar

        [ExcelFunction(Description = "KhanOilViscosity_ob")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double KhanOilViscosity_ob(double T_R, double gamma_o, double gamma_g, double Rs)
        {
            return ((0.09 * Math.Sqrt(gamma_g)) / ((Math.Pow(Rs, (1 / 3))) * (Math.Pow((T_R / 459.67), 4.5)) * Math.Pow((1.0 - gamma_o), 3)));
        }

        //Above BubblePoint
        //' KhanOilViscosity_a
        //' @export
        //' @param mu_ob []
        //' @param P []
        //' @param Pb []
        //' @returns A numeric scalar
        [ExcelFunction(Description = "KhanOilViscosity_a")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static double KhanOilViscosity_a(double mu_ob, double P, double Pb)
        {
            return (mu_ob * Math.Exp(9.6E-5 * (P - Pb)));
        }

        //Below BubblePoint
        //' KhanOilViscosity_b
        //' @export
        //' @param mu_ob []
        //' @param P []
        //' @param Pb []
        //' @returns A numeric scalar
        [ExcelFunction(Description = "KhanOilViscosity_b")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static double KhanOilViscosity_b(double mu_ob, double P, double Pb)
        {
            return (mu_ob * Math.Pow((P / Pb), -0.14) * Math.Exp(-2.5E-4 * (P - Pb)));
        }

        //' KhanOilViscosity
        //' @export
        //' @param P []
        //' @param Pb []
        //' @param T_R []
        //' @param gamma_o []
        //' @param gamma_g []
        //' @param Rs []
        //' @returns A numeric scalar
        [ExcelFunction(Description = "KhanOilViscosity")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double KhanOilViscosity(double P, double T_R, double Pb, double gamma_o, double gamma_g, double Rs)
        {
            double mu_ob = KhanOilViscosity_ob(T_R, gamma_o, gamma_g, Rs);

            if (P > Pb)
            {
                return KhanOilViscosity_a(mu_ob, P, Pb);
            }
            else
            {
                return KhanOilViscosity_b(mu_ob, P, Pb);
            }
        }


        ////' GlasoSaturatedOilFormationVolumeFactor
        ////' @export
        ////' @param Rs []
        ////' @param Temperature []
        ////' @param SGo_API []
        ////' @param SGg []
        ////' @returns A numeric scalar
        //GlasoSaturatedOilFormationVolumeFactor = function(Rs, Temperature, SGo_API, SGg) {
        //  SGo = APItoSG(SGo_API)

        //  Bost = Rs * pow((SGg / SGo), 0.526) + 0.968 * Temperature

        //  A = -6.58511 + 2.91329 * log10(Bost) - 0.27683 * pow(log10(Bost), 2.0)

        //  return (1.0 + pow(10, A))
        //}

        ////' StandingSaturatedOilFormationVolumeFactor
        ////' @export
        ////' @param Rs []
        ////' @param Temperature []
        ////' @param SGo_API []
        ////' @param SGg []
        ////' @returns A numeric scalar
        //StandingSaturatedOilFormationVolumeFactor = function(Rs, Temperature, SGo_API, SGg) {
        //  SGo = APItoSG(SGo_API)

        //  return (0.9759 + 0.000120 * pow(Rs * sqrt(SGg / SGo) + 1.25 * Temperature, 1.2))
        //}

        ////' PetroskyFarshadSaturatedOilFormationVolumeFactor
        ////' @export
        ////' @param Rs []
        ////' @param Temperature []
        ////' @param SGo_API []
        ////' @param SGg []
        ////' @returns A numeric scalar
        //PetroskyFarshadSaturatedOilFormationVolumeFactor = function(Rs, Temperature, SGo_API, SGg) {
        //  SGo = APItoSG(SGo_API)

        //  a = pow(Rs, 0.3738)
        //  b = pow(SGg, 0.2914) / pow(SGo, 0.6265)
        //  c = pow(Temperature, 0.5371)

        //  return (1.0113 + 7.2046e-5 * pow(a * b + 0.24626 * c, 3.0936))
        //}


        ////' DindorukChristmanSolutionGasOilRatio
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param Pbp []
        ////' @param SGo_API []
        ////' @param SGg []
        ////' @param Multiple =0.2
        ////' @returns A numeric scalar
        //DindorukChristmanSolutionGasOilRatio = function(Pressure, Temperature, Pbp, SGo_API, SGg, Multiple=0.2) {

        //  a1 = 0.0005121
        //  a2 = 0.8403
        //  a3 = 1.0891
        //  a4 = 0.5187
        //  a5 = 1.0000
        //  a6 = 1.2321

        //  if( Pressure > Pbp) {
        //    Rs = (a1*SGg^a2*SGo_API^a3*Temperature^a4*Pbp^a5)^a6
        //  } else {
        //    Rs = (a1*SGg^a2*SGo_API^a3*Temperature^a4*Pressure^a5)^a6
        //  }

        //  return (Multiple*Rs)
        //}




        //// GeneralUnderSaturatedOilFormationVolumeFactor = function(Pressure, Temperature, Pb, Rs, SGo_API, SGg) {
        ////   Co = PetroskyFarshadOilCompressibility(Pressure, Temperature, Rs, SGo_API, SGg)
        ////   // Pb  = WolfcampBubblepointPressure(Temperature, SGo_API, SGg, Rs)
        ////   Bob = WolfcampFVF(Temperature, SGo_API, SGg, Rs)
        ////   // Bob = PetroskyFarshadSaturatedOilFormationVolumeFactor(Rs, Temperature, SGo_API, SGg)
        ////   // Bob = StandingSaturatedOilFormationVolumeFactor(Rs, Temperature, SGo_API, SGg)
        ////
        ////   return (Bob * exp(-Co * (Pressure - Pb)))
        //// }

        ////' OilFormationVolumeFactor
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param Pb []
        ////' @param SGo_API []
        ////' @param SGg []
        ////' @returns A numeric scalar
        //OilFormationVolumeFactor = function(Pressure, Temperature, Pb, SGo_API, SGg) {
        //  SGo = APItoSG(SGo_API)

        //  Rsb = VelardeSolutionGasOilRatioBubblePoint(Temperature, Pb, SGo, SGg)

        //  Rs = DindorukChristmanSolutionGasOilRatio(Pressure, Temperature, Pb, SGo_API, SGg)


        //  // print(
        //  //   paste0(
        //  //     "Pressure=",      Pressure,
        //  //     " Temperature=",      Temperature,
        //  //     " Rs=",      Rs,
        //  //     " SGo_API=",      SGo_API,
        //  //     " SGg=",      SGg
        //  //   )
        //  // )

        //  if (Pressure <= Pb) {
        //    rho_po = 52.8 - (0.01 * Rsb)

        //    rho_a = (-49.8930 +
        //                (85.0149 * SGg) -
        //                (3.70373 * SGg * rho_po) +
        //                (0.047981 * SGg * rho_po^2) +
        //                (2.98914 * rho_po) -
        //                (0.035688 * rho_po^2))

        //    rho_po = ((Rs * SGg) + (4600 * SGo)) / (73.71 + ((Rs * SGg) / rho_a))

        //    rho_bs = rho_po + (0.167 + (16.181 * (10^(-0.0425 * rho_po)))) * (Pressure /1000) -
        //                       (0.01 * (0.299 + 263 * (10^(-0.0603 * rho_po))) * (Pressure / 1000)^2)

        //    rho_oR = rho_bs - ((0.00302 + 1.505 * Re(as.complex(rho_bs)^-0.951)) * (Temperature-60)^0.938) +
        //                        ((0.0233 * (10^(-0.0161 * rho_bs))) * (Temperature-60)^0.475)

        //    rho_sto = SGo * 62.31

        //    FVF = (rho_sto + 0.01357 * Rs * SGg) / rho_oR

        //    // print(paste0("UnderSaturated FVF=", FVF))
        //  } else {

        //    Bob = OilFormationVolumeFactor(Pb, Temperature, Pb, SGo_API, SGg)
        //    co = PetroskySaturatedOilCompressibility(Pressure, Temperature, Rs, SGo_API, SGg)
        //    FVF = (Bob*exp(co*(Pb-Pressure)))

        //   // FVF = PetroskyFarshadSaturatedOilFormationVolumeFactor(Rs, Temperature, SGo_API, SGg)
        //   // print(paste0("Saturated FVF=",FVF))
        //  }

        //  return (FVF)
        //}

        //// OilFormationVolumeFactor = function(Rs, Rhoo, SGo, SGg){
        ////   return ((((62.42796*SGo)+(0.0136*SGg*Rs))/Rhoo))//*(5.615/42))
        //// }

        ////' TotalFormationVolumeFactor
        ////' @export
        ////' @param Bo []
        ////' @param Bg []
        ////' @param Rsoi []
        ////' @param Rso []
        ////' @returns A numeric scalar
        //TotalFormationVolumeFactor = function(Bo, Bg, Rsoi, Rso) {
        //  return (Bo+(Bg*(Rsoi-Rso)))
        //}


        //' StandingSolutionGasOil
        //' @export
        //' @param SGo_API []
        //' @param SGg []
        //' @returns A numeric scalar
        [ExcelFunction(Description = "StandingSolutionGasOil")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double StandingSolutionGasOil(double SGo_API, double SGg)
        {
            double SGo = Methods.APItoSG(SGo_API);

            const double a1 = 725.32143;
            const double a2 = 16.0333;
            const double a3 = 0.09524;

            double Mo = a1 - (a2 * SGo_API) + (a3 * Math.Pow(SGo_API, 2));

            double Rs = (SGo * 132775* SGo) / (Mo * (1.0 - SGg));

            //X = 0.0125*SGo_API-0.00091*(T-459.67)
            //Rs = SGg*(((P/18.2)+1.4)*10^X)^1.2048

            return (Rs);
        }


        ////' VasquezBeggsOilSolutionGas
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param SGo_API []
        ////' @param SGg []
        ////' @returns A numeric scalar
        //VasquezBeggsOilSolutionGas = function(Pressure, Temperature, SGo_API, SGg)
        //{
        //  T_R = FtoR(Temperature)

        //  if (SGo_API <= 30.0)
        //  {
        //    a1 = 0.0362
        //    a2 = 1.0937
        //    a3 = 25.7240
        //  }
        //  else
        //  {
        //    a1 = 0.017838
        //    a2 = 1.1870
        //    a3 = 23.93508
        //  }

        //  return (a1 * SGg * pow(Pressure, a2) * exp((a3 * SGo_API) / T_R))
        //}

        //// OilSolutionGas = function(Pressure, Temperature, Pb, SGo_API, SGg) {
        ////   if (Pressure > Pb) {
        ////     return (VasquezBeggsOilSolutionGas(Pb, Temperature, SGo_API, SGg))
        ////   } else {
        ////     return (VasquezBeggsOilSolutionGas(Pressure, Temperature, SGo_API, SGg))
        ////   }
        //// }

        ////' DranchukAbuKassemGasCompressibilityFactor
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param Gammag []
        ////' @returns A numeric scalar
        //DranchukAbuKassemGasCompressibilityFactor = function(Pressure, Temperature, Gammag) {
        //  Ppc = (708.75 - (57.5 * Gammag))
        //  Ppr = Pressure / Ppc

        //  T_R = FtoR(Temperature)

        //  Tpc = (169.0 + (314.0 * Gammag))
        //  Tpr = T_R / Tpc


        //  t = 1 / Tpr

        //  A1  = 0.3265
        //  A2  = -1.07
        //  A3  = -0.5339
        //  A4  = 0.01569
        //  A5  = -0.05165
        //  A6  = 0.5475
        //  A7  = -0.7361
        //  A8  = 0.1844
        //  A9  = 0.1056
        //  A10 = 0.6134
        //  A11 = 0.7210
        //  Zc  = 0.27
        //  R1  = A1 + A2 * t + A3 * t * t * t + A4 * t * t * t * t + A5 * t * t * t * t * t
        //  R2  = Zc * Ppr * t
        //  R3  = A6 + A7 * t + A8 * t * t
        //  R4  = A9 * (A7 * t + A8 * t * t)
        //  R5  = A10 * t * t * t

        //  Y = Zc * Ppr / Tpr

        //  maxIter = 100
        //  tol     = 1E-10

        //  for (n in c(1,maxIter)) {
        //    Yk = Y
        //    Y2 = Y * Y
        //    Y3 = Y2 * Y
        //    Y4 = Y3 * Y
        //    Y5 = Y4 * Y

        //    F  = 1 - R2 / Y + R1 * Y + R3 * Y2 - R4 * Y5 + R5 * Y2 * (1 + A11 * Y2) * exp(-A11 * Y2)
        //    dF = R1 + R2 / Y2 + 2 * R3 * Y + 2 * A11 * exp(-A11 * Y2) * R5 * Y3 - 5 * R4 * Y4 + 2 * exp(-A11 * Y2) * R5 * Y * (1 + A11 * Y2) - 2 * A11 * exp(-A11 * Y2) * R5 * Y3 * (1 + A11 * Y2)
        //    dY = -F / dF

        //    Y = Yk + dY

        //    if (abs(dY) < tol && abs(F) < tol)
        //    {
        //      break
        //    }
        //  }

        //  Z = Zc * Ppr / (Tpr * Y)

        //  return (Z)
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //// ZGasCompressibility = function(Pressure, Temperature, Gammag) {
        ////   Ppc = (708.75 - (57.5 * Gammag))
        ////   Ppr = Pressure / Ppc
        ////
        ////   T_R = FtoR(Temperature)
        ////
        ////   Tpc = (169.0 + (314.0 * Gammag))
        ////   Tpr = T_R / Tpc
        ////
        ////   // print(paste0("Ppc=",Ppc," Ppr=",Ppr," Tpc=",Tpc," Tpr=",Tpr))
        ////
        ////   //Carr
        ////   // a0 = -2.462
        ////   // a1 = 2.97
        ////   // a2 = -0.2862
        ////   // a3 = 0.008054
        ////   // a4 = 2.808
        ////   // a5 = -3.498
        ////   // a6 = 0.3603
        ////   // a7 = -0.01044
        ////   // a8 = -0.7933
        ////   // a9 = 1.396
        ////   // a10 = -0.1491
        ////   // a11 = 0.00441
        ////   // a12 = 0.08393
        ////   // a13 = -0.1864
        ////   // a14 = 0.02033
        ////   // a15 = -0.0006095
        ////   //
        ////   // lnmgm1Tpr = a0 + a1 * Ppr + a2 * Ppr^2 + a3 * Ppr^3 + Tpr * (a4 + a5 * Ppr + a6 * Ppr^2 + a7 * Ppr^3) + Tpr^2 * (a8 + a9 * Ppr + a10 * Ppr^2 + a11 * Ppr^3) + Tpr^3 * (a12 + a13 * Ppr + a14 * Ppr^2 + a15 * Ppr^3)
        ////   // // print(paste0("lnmgm1Tpr=",lnmgm1Tpr))
        ////   //
        ////   // BaseViscosity = (1.709 / 100000 - 2.062 / 1000000 * Gammag) * Temperature + 8.188 / 1000 - 6.15 / 1000 * log(Gammag)
        ////   // // print(paste0("BaseViscosity=",BaseViscosity))
        ////   //
        ////   // GasViscosity = BaseViscosity / Tpr * exp(lnmgm1Tpr)
        ////   // // print(paste0("GasViscosity=",GasViscosity))
        ////
        ////   t1Tpr = 1 / Tpr
        ////
        ////   A_ = 0.06125 * t1Tpr * exp(-1.2 * (1 - t1Tpr)^2)
        ////   B_ = t1Tpr * (14.76 - 9.76 * t1Tpr + 4.58 * t1Tpr * t1Tpr)
        ////   C_ = t1Tpr * (90.7 - 242.2 * t1Tpr + 42.4 * t1Tpr * t1Tpr)
        ////   D_ = 2.18 + 2.82 * t1Tpr
        ////
        ////   // print(paste0("A_=",A_))
        ////   // print(paste0("B_=",B_))
        ////   // print(paste0("C_=",C_))
        ////   // print(paste0("D_=",D_))
        ////
        ////   // F_ = -A_*Ppr+(Y_+Y_*Y_+Y_^3-Y_^4)/(1-Y_)^3-B_*Y_^2+C_*Y_^Y_
        ////
        ////   F_ = function(y) {
        ////     return(-A_ * Ppr + (y + y * y + y^3 - y^4) / (1 - y)^3 - B_ * y^2 + C_ * y^D_)
        ////   }
        ////
        ////   //Goal Seek
        ////   //Change Y as F goes to zero.
        ////
        ////   // result = optim(0.1, F_)
        ////   // result = optimize(F_, lower = 0.0001, upper = 0.2)
        ////   result = uniroot(F_, lower = 0, upper = 0.5)
        ////   //print(result)
        ////
        ////   Y_ = result$root
        ////   //print(paste0("Y_=",Y_))
        ////
        ////   z_ = A_ * Ppr / Y_
        ////   //print(paste0("z_=",z_))
        ////
        ////   return (z_)
        //// }

        ////' StandingGasViscosity
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param SGg []
        ////' @param yCO2 = 0.0
        ////' @param yN2 = 0.0
        ////' @param yH2S = 0.0
        ////' @returns A numeric scalar
        //StandingGasViscosity = function(Pressure,
        //                                 Temperature,
        //                                 SGg,
        //                                 yCO2 = 0.0,
        //                                 yN2 = 0.0,
        //                                 yH2S = 0.0) {
        //  T_R = FtoR(Temperature)

        //  mu_yN2  = yN2 * (8.48E-3 * log10(SGg) + 9.59E-3)
        //  mu_yCO2 = yCO2 * (9.08E-3 * log10(SGg) + 6.24E-3)
        //  mu_yH2S = yH2S * (8.49E-3 * log10(SGg) + 3.73E-3)

        //  mu_uncorrected = (3.0764E-5 - (3.712E-6 * log10(SGg))) * (T_R - 256.0) + 8.188E-3 - (6.15E-3 * log10(SGg))

        //  return (mu_uncorrected + mu_yN2 + mu_yCO2 + mu_yH2S)
        //}

        ////' StandingGasPseudoCritical
        ////' @export
        ////' @param SGg []
        ////' @param yCO2 = 0.0
        ////' @param yN2 = 0.0
        ////' @param yH2S = 0.0
        ////' @returns A numeric scalar
        //StandingGasPseudoCritical = function(SGg, yCO2 = 0.0, yN2 = 0.0, yH2S = 0.0)
        //{
        //  Ppc = 787.06 - 147.34 * SGg - 7.916 * SGg * SGg

        //  Tpc = 168.0 + 325.0 * SGg - 12.5 * SGg * SGg

        //  return (list(Ppc=Ppc, Tpc=Tpc))
        //}

        //' CarrKobayashiBurrowsGasViscosity
        //' @export
        //' @param Pressure []
        //' @param Temperature []
        //' @param SGg []
        //' @param yCO2 = 0.0
        //' @param yN2 = 0.0
        //' @param yH2S = 0.0
        //' @returns A numeric scalar
        [ExcelFunction(Description = "CarrKobayashiBurrowsGasViscosity")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double CarrKobayashiBurrowsGasViscosity(double Pressure,
                                                              double T_R,
                                                              double SGg,
                                                              double yCO2 = 0.0,
                                                              double yN2 = 0.0,
                                                              double yH2S = 0.0)
        {
            double  mu1uc = (1.709e-5 - 2.062e-6 * SGg) * (T_R) + 8.118e-3 - 6.15e-3 * Math.Log10(SGg);
            double  dmuCO2 = yCO2 * (9.08e-3 * Math.Log10(SGg) + 6.24e-3);
            double  dmuN2 = yN2 * (8.48e-3 * Math.Log10(SGg) + 9.59e-3);
            double  dmuH2S = yH2S * (8.49e-3 * Math.Log10(SGg) + 3.73e-3);

            double  mu1 = mu1uc + dmuCO2 + dmuN2 + dmuH2S;

            const double a0 = -2.46211820;
            const double a1 = 2.970547414;
            const double a2 = -2.86264054e-1;
            const double a3 = 8.05420522e-3;
            const double a4 = 2.80860949;
            const double a5 = -3.49803305;
            const double a6 = 3.60373020e-1;
            const double a7 = -1.044324e-2;
            const double a8 = -7.93385648e-1;
            const double a9 = 1.39643306;
            const double a10 = -1.49144925e-1;
            const double a11 = 4.41015512e-3;
            const double a12 = 8.39387178e-2;
            const double a13 = -1.86408848e-1;
            const double a14 = 2.03367881e-2;
            const double a15 = -6.09579263e-4;

            double Ppc = SuttonAssociatedGasPseudoCriticalPressure(SGg);
            double Tpc = SuttonAssociatedGasPseudoCriticalTemperature(SGg);

            double Ppr = (Pressure / Ppc);
            double Tpr = (T_R / Tpc);

            double Ppr2 = Ppr * Ppr;
            double Ppr3 = Ppr2 * Ppr;

            double Tpr2 = Tpr * Tpr;
            double Tpr3 = Tpr2 * Tpr;

            double R = a0 + a1 * Ppr + a2 * Ppr2 + a3 * Ppr3 + Tpr * (a4 + a5 * Ppr + a6 * Ppr2 + a7 * Ppr3) + Tpr2 * (a8 + a9 * Ppr + a10 * Ppr2 + a11 * Ppr3) + Tpr3 * (a12 + a13 * Ppr + a14 * Ppr2 + a15 * Ppr3);

            double mu = Math.Exp(R) * mu1 / Tpr;

            return (mu);
        }





        ////' McCainGasCompressibility
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param Gammag []
        ////' @returns A numeric scalar
        //McCainGasCompressibility = function(Pressure, Temperature, Gammag) {

        //  Z = DranchukAbuKassemGasCompressibilityFactor(Pressure, Temperature, Gammag)

        //  Ppc = (708.75 - (57.5 * Gammag))
        //  Tpc = (169.0 + (314.0 * Gammag))


        //  T_R = FtoR(Temperature)

        //  A1  = 0.3265
        //  A2  = -1.0700
        //  A3  = -0.5339
        //  A4  = 0.01569
        //  A5  = -0.05165
        //  A6  = 0.5475
        //  A7  = -0.7361
        //  A8  = 0.1844
        //  A9  = 0.1056
        //  A10 = 0.6134
        //  A11 = 0.7210

        //  Ppr = Pressure / Ppc
        //  Tpr = T_R / Tpc

        //  rhopr = 0.27 * (Ppr / (Z * Tpr))

        //  dzdpr = (A1 + (A2 / Tpr) + (A3 / pow(Tpr, 3)) + (A4 / pow(Tpr, 4)) + (A5 / pow(Tpr, 5))) + 2 * rhopr * (A6 + (A7 / Tpr) + (A8 / pow(Tpr, 2))) - 5 * rhopr * (A7 + (A8 / pow(Tpr, 2))) + ((2 * A10 * rhopr) / pow(Tpr, 3)) * (1 + A11 * pow(rhopr, 2) - pow(A11, 2) * pow(rhopr, 4)) * exp((-A11 * pow(rhopr, 2)))

        //  cpr = ((1.0 / Ppr) - (0.27 / (pow(Z, 2) * Tpr)) * (dzdpr / (1.0 + ((
        //    Ppr / Z
        //  ) * dzdpr))))

        //  return (cpr / Ppc)
        //}



        ////' GasCompressibility
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param Z []
        ////' @returns A numeric scalar
        //GasCompressibility = function(Pressure, Temperature, Z) {
        //  return ((0.005035 * Z * Temperature) / (Pressure))
        //}


        ////' GasFormationVolumeFactor
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param Pb []
        ////' @param Z []
        ////' @param SGo_API []
        ////' @param SGg []
        ////' @returns A numeric scalar
        //GasFormationVolumeFactor = function(Pressure, Temperature, Pb, Z, SGo_API, SGg)
        //{
        //  T_R = FtoR(Temperature)

        //  ZT_P = (Z * T_R) / Pressure

        //  Bg_dry = 0.0282794851 * ZT_P

        //  if (SGo_API == 0.0)
        //  {
        //    return (Bg_dry)
        //  }

        //  Mo = 6084.0 / (SGo_API - 5.9)

        //  //Rs = DindorukChristmanSolutionGasOilRatio(Pressure, Temperature, Pb, SGo_API, SGg)
        //  Rs = VelardeSolutionGasOilRatio(Pressure, Temperature, Pb, SGo_API, SGg)

        //  SGo = APItoSG(SGo_API)

        //  Bg_wet = ZT_P * (0.0282 * Rs + ((3758.0 * SGo) / Mo)) / (Rs + 1330000.0 * (SGo / Mo))

        //  return (Bg_wet)
        //}

        ////' RGAS
        //RGAS = 10.7315
        ////' STDAIRMA
        //STDAIRMA = 28.967

        ////' GasDensity
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param SGg []
        ////' @returns A numeric scalar
        //GasDensity = function(Pressure, Temperature, SGg)
        //{
        //  T_R = FtoR(Temperature)

        //  z = DranchukAbuKassemGasCompressibilityFactor(Pressure, Temperature, SGg)

        //  // Compute density
        //  R    = RGAS
        //  Mg   = STDAIRMA * SGg
        //  rhog = Pressure * Mg / (z * R * T_R) / 62.37                  //gm/cc

        //  return (rhog)
        //}

        ////' GasDensityFromZ
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param SGg []
        ////' @param Z []
        ////' @returns A numeric scalar
        //GasDensityFromZ = function(Pressure, Temperature, SGg, Z)
        //{
        //  T_R = FtoR(Temperature)

        //  // Compute density
        //  R    = RGAS
        //  Mg   = STDAIRMA * SGg
        //  rhog = Pressure * Mg / (Z * R * T_R) / 62.37                  //gm/cc

        //  return (rhog)
        //}

        ////Water Properties

        ////' McCainWaterFormationVolumeFactor
        ////' Salinity Wt\% (gm/L)
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param Salinity []
        ////' @returns A numeric scalar
        //McCainWaterFormationVolumeFactor = function(Pressure, Temperature, Salinity = 0.0) {
        //  T_Sqrd = Temperature * Temperature

        //  P_Sqrd = Pressure * Pressure

        //  dVt = -1.0001e-2 + 1.33391e-4 * Temperature + 5.50654e-7 * T_Sqrd

        //  dVp = -1.95301e-9 * Pressure * Temperature - 1.72834e-13 * P_Sqrd * Temperature - 3.58922e-7 * Pressure - 2.25341e-10 * P_Sqrd

        //  return (1.0 + dVt) * (1.0 + dVp)
        //}

        //' McCainWaterViscosity
        //' Salinity Wt\% (gm/L)
        //' @export
        //' @param Pressure []
        //' @param Temperature []
        //' @param Salinity []
        [ExcelFunction(Description = "McCainWaterViscosity")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double McCainWaterViscosity(double Pressure, double Temperature, double Salinity = 0.0)
        {
            double S2 = Salinity * Salinity;
            double S3 = S2 * Salinity;
            double S4 = S3 * Salinity;

            double A = 109.574 - 8.40564 * Salinity + 0.313314 * S2 + 8.72213e-3 * S3;
            double B = 1.12166 - 2.63951e-2 * Salinity + 6.79461e-4 * S2 + 5.47119e-5 * S3 - 1.55586e-6 * S4;
            double mu1 = A * Math.Pow(Temperature, -B);

            return (mu1 * (0.9994 + 4.0295e-5 * Pressure + 3.1062e-9 * Pressure * Pressure));
        }

        ////' McCainWaterCompressibility
        ////' Salinity Wt\% (gm/L)
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param Salinity []
        //McCainWaterCompressibility = function(Pressure, Temperature, Salinity = 0.0) {
        //  return (1.0 / ((7.033 * Pressure) + (541 * Salinity) - (537.0 * Temperature) + 403300
        //  ))
        //}

        ////' McCainWaterDensity
        ////' Salinity Wt\% (gm/L)
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param Salinity []
        //McCainWaterDensity = function(Pressure, Temperature, Salinity = 0.0)
        //{
        //  Bw = McCainWaterFormationVolumeFactor(Pressure, Temperature, Salinity)

        //  standard = 62.368 + (0.438603 * Salinity) + (0.00160074 * Salinity * Salinity)

        //  return (standard / Bw)
        //}

        ////' McCainWaterSolutionGas
        ////' @export
        ////' @param Pressure []
        ////' @param Temperature []
        ////' @param Salinity = 0.0 []
        ////' @returns A numeric scalar
        //McCainWaterSolutionGas = function(Pressure, Temperature, Salinity = 0.0)
        //{
        //  T_R = FtoR(Temperature)

        //  T_R2 = T_R * T_R
        //  T_R3 = T_R2 * T_R
        //  T_R4 = T_R3 * T_R

        //  A = (8.15839 - 6.12265e-2 * T_R + 1.91663e-4 * T_R2 - 2.1654e-7 * T_R3)

        //  B = (1.01021e-2 - 7.44241e-5 * T_R + 3.05553e-7 * T_R2 - 2.94883e-10 * T_R3)

        //  C = (1e-7 * (9.02505 - 0.130237 * T_R + 8.53425e-4 * T_R2 - 2.34122e-6 * T_R3 + 2.37049e-9 * T_R4))

        //  Rs_water_pure = (A + B * Pressure + C * Pressure * Pressure)

        //  return (Rs_water_pure * pow(10, -0.0840655 * Salinity * pow(T_R, -0.285854)))
        //}

        ////////////////////////////////////////


        ////' SolutionGasOilRatioProduction
        ////' @export
        ////' @param Rs []
        ////' @param kro []
        ////' @param krg []
        ////' @param muo []
        ////' @param mug []
        ////' @param bo []
        ////' @param bg []
        ////' @returns A numeric scalar
        //SolutionGasOilRatioProduction = function(Rs, kro, krg, muo, mug, bo, bg) {
        //  term = ((krg * muo * bo) / (kro * mug * bg))
        //  // print(term)
        //  return (Rs + term)
        //}

        ////' SolutionWaterOilRatioProduction
        ////' @export
        ////' @param kro []
        ////' @param krw []
        ////' @param muo []
        ////' @param muw []
        ////' @param bo []
        ////' @param bw []
        ////' @returns A numeric scalar
        //SolutionWaterOilRatioProduction = function(kro, krw, muo, muw, bo, bw) {
        //  term = ((krw * muo * bo) / (kro * muw * bw))
        //  //print(term)
        //  return (term)
        //}


        ////////////////////////////////////

        ////' RelativePermV
        ////' @export
        ////' @param T_R []
        ////' @param Rsi []
        ////' @param Rs []
        ////' @param SGo []
        ////' @returns A numeric scalar
        //RelativePermV = function(T_R, Rsi, Rs, SGo) {
        //  MWOil = MolecularWeightOil(T_R, SGo)
        //  return (((Rsi - Rs) * 2638) / (((SGo / MWOil) * 350.5170) + (2638 * Rsi)))
        //}

        ////' ProductionIndex
        ////' @export
        ////' @param k []
        ////' @param h []
        ////' @param Muo []
        ////' @param Bo []
        ////' @param a []
        ////' @param L []
        ////' @param rwa []
        ////' @param skin = 0 []
        ////' @param D = 0 []
        ////' @param qo = 0 []
        ////' @returns A numeric scalar
        //ProductionIndex = function(k,
        //                            h,
        //                            Muo,
        //                            Bo,
        //                            a,
        //                            L,
        //                            rwa,
        //                            skin = 0,
        //                            D = 0,
        //                            qo = 0)
        //{
        //  kH = (k / 10)

        //  alpha = sqrt(k / kH)
        //  delta = 0.5

        //  return ((7.08 * k * h) / (Muo * Bo * log((a + sqrt(
        //    a^2 - (L / 2)^2
        //  )) / (L / 2)) + ((alpha * h) / L) * log((((alpha * h) / 2)^2 - (alpha *
        //                                                                    delta)^2
        //  ) / (0.5 * alpha * h * rwa)) + skin + (D * qo)))
        //}

        //// HydrocarbonProperties = function() {
        ////   HydrocarbonPropertiesPressure = seq(from = 100, to = 10000, by = 100)
        ////
        ////   HydrocarbonPropertiesPressure_length = length(HydrocarbonPropertiesPressure)
        ////
        ////   property = array(dim = c(HydrocarbonPropertiesPressure_length, 8))
        ////
        ////   propert_names = c("Rs","Muo","Bo","Rhoo","Z","Rhog","Mug","Bg")
        ////
        ////   // print(paste0("T=",Temperature, " Gammao=",Gammao, " oilAPI=",oilAPI, " Gammag=",Gammag))
        ////
        ////   for (i in seq_along(HydrocarbonPropertiesPressure)){
        ////     // print(paste0("P=",HydrocarbonPropertiesPressure[i]))
        ////
        ////     property[i , 1] = Rs = VelardeSolutionGasOilRatio(HydrocarbonPropertiesPressure[i], Temperature, Pb, oilAPI, Gammag)
        ////     // print(paste0("Rs=",Rs))
        ////
        ////     property[i , 2] = Muo = KhanOilViscosity(HydrocarbonPropertiesPressure[i], Pb, FtoR(Temperature), Gammao, Gammag, Rs)
        ////     // print(paste0("Muo=",Muo))
        ////
        ////     property[i , 3] = Bo = OilFormationVolumeFactor(HydrocarbonPropertiesPressure[i], Temperature, Pb, oilAPI, Gammag)
        ////     // print(paste0("Bo=",Bo))
        ////
        ////     property[i , 4] = Rhoo = OilDensity(Rs, Bo, Gammao, Gammag)
        ////     // print(paste0("Rhoo=",Rhoo))
        ////
        ////     property[i , 5] = Z = DranchukAbuKassemGasCompressibilityFactor(HydrocarbonPropertiesPressure[i], Temperature, Gammag)
        ////     //print(paste0("Z=",Z))
        ////
        ////     property[i , 6] = Rhog = GasDensityFromZ(HydrocarbonPropertiesPressure[i], Temperature, Gammag, Z)
        ////     // print(paste0("Rhog=",Rhog))
        ////
        ////     property[i , 7] = Mug = CarrKobayashiBurrowsGasViscosity(HydrocarbonPropertiesPressure[i], Temperature, Gammag)
        ////     // print(paste0("Mug=",Mug))
        ////
        ////     property[i , 8] = Bg = (0.005035*Z*FtoR(Temperature))/(HydrocarbonPropertiesPressure[i])
        ////     // print(paste0("Bg=",Bg))
        ////   }
        ////
        ////   min_x=0
        ////   min_y=0
        ////   max_x=10000
        ////   max_y=max(property)
        ////
        ////   bordercol = "//0000007F"
        ////   labelcex = 1
        ////
        ////   for (i in 1:8){
        ////
        ////     ylabel = propert_names[i]
        ////
        ////
        ////     plot_x=HydrocarbonPropertiesPressure
        ////     plot_y=property[, i]
        ////
        ////     //Hydrocarbon Properties Plot
        ////     plot(plot_x, plot_y, //log = "xy",
        ////          xlab="Pressure, psi", ylab=ylabel,
        ////          //xlim=c(min_x,max_x), ylim=c(min_y,max_y),
        ////          //xaxs = "i", yaxs = "i",
        ////          main="Hydrocarbon Property") //, axes = FALSE
        ////     grid()
        ////
        ////
        ////     //points(plot_x, plot_y, col = "red", lwd = 1, cex = 1)
        ////     lines(plot_x, plot_y, col = "red", lwd = 5)
        ////   }
        //// }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////' default_cf
        //default_cf = 3E-6 //1/psia

        ////' TotalCompressibility
        ////' @export
        ////' @param pressure []
        ////' @param temperature []
        ////' @param pb []
        ////' @param oilapi []
        ////' @param gammag []
        ////' @param so []
        ////' @param sg []
        ////' @param sw []
        ////' @param cf =default_cf
        ////' @param salinity =0
        ////' @returns A numeric scalar
        //TotalCompressibility = function(pressure, temperature, pb, oilapi, gammag, so, sg, sw, cf=default_cf, salinity=0) {

        //  bo = OilFormationVolumeFactor(pressure, temperature, pb, oilapi, gammag)
        //  bw = McCainWaterFormationVolumeFactor(pressure, temperature)
        //  z = DranchukAbuKassemGasCompressibilityFactor(pressure, temperature, gammag)
        //  bg =  GasFormationVolumeFactor(pressure, temperature, pb, z, oilapi, gammag)

        //  dBodP = CentralDifferenceMethod(OilFormationVolumeFactor, pressure, temperature, pb, oilapi, gammag)
        //  dRsdP = CentralDifferenceMethod(DindorukChristmanSolutionGasOilRatio, pressure, temperature, pb, oilapi, gammag)

        //  dBgdP = CentralDifferenceMethod(GasFormationVolumeFactor, pressure, temperature, pb, z, oilapi, gammag)

        //  dBwdP = CentralDifferenceMethod(McCainWaterFormationVolumeFactor, pressure, temperature)
        //  dRswdP = CentralDifferenceMethod(McCainWaterSolutionGas, pressure, temperature, salinity)

        //  co = abs(so*(((-1/bo)*dBodP)+((bg/bo)*dRsdP)))
        //  cw = sw*(((-1/bw)*dBwdP)+((bg/bw)*dRswdP))
        //  cg = sg*(((-1/bg)*dBgdP))

        //  ct = cf+ co + cg + cw

        //  // print(paste0("Pwf=",Pwf[i], " co=",co," bo=",bo," dBodP=",dBodP," bg=",bg," dRsdP=",dRsdP))

        //  return (list(co=co, cg=cg, cw=cw, cf=cf, ct=ct))
        //}

        ////' VolatileOilGasRatio by El-Banbi
        ////' @export
        ////' @param pressure []
        ////' @param temperature []
        ////' @param Pb []
        ////' @param CGR []
        ////' @param oilAPI []
        ////' @param Gammag []
        ////' @returns A numeric scalar
        //VolatileOilGasRatio = function(pressure, temperature, Pb, CGR, oilAPI, Gammag) {

        //  T_R = FtoR(temperature)

        //  Rs_sat = DindorukChristmanSolutionGasOilRatio(14.7, 60, Pb, oilAPI, Gammag)
        //  Bo_sat = OilFormationVolumeFactor(14.7, 60, Pb, oilAPI, Gammag)
        //  rho_osc = OilDensity(Rs_sat, Bo_sat, APItoSG(oilAPI), Gammag)

        //  Z_sat = DranchukAbuKassemGasCompressibilityFactor(14.7, 60, Gammag)
        //  rho_gsc = GasDensityFromZ(14.7, 60, Gammag, Z_sat)

        //  //Volatile Oil Standing
        //  // A1 =	47.23306
        //  // A2 =	-8.833514
        //  // A3 =	1.3251534
        //  // A4 =	0.0091756
        //  // A5 =	-0.000385524

        //  A1 =	1.225537042
        //  A2 =	0.000107257
        //  A3 = -0.194226755
        //  A4 = 240.549909
        //  A5 =	8.32137351

        //  return (((A1*rho_gsc*(A2*pressure^2+A3*pressure+A4))/Pb)*exp((A5*CGR*519.67)/(rho_osc*T_R*14.7)))
        //}

        ////' GasCondensateOilGasRatio
        ////' @export
        ////' @param pressure []
        ////' @param temperature []
        ////' @param Pb []
        ////' @param CGR []
        ////' @param oilAPI []
        ////' @param Gammag []
        ////' @returns A numeric scalar
        //GasCondensateOilGasRatio = function(pressure, temperature, Pb, CGR, oilAPI, Gammag) {

        //  T_R = FtoR(temperature)

        //  Rs_sat = DindorukChristmanSolutionGasOilRatio(14.7, 60, Pb, oilAPI, Gammag)
        //  Bo_sat = OilFormationVolumeFactor(14.7, 60, Pb, oilAPI, Gammag)
        //  rho_osc = OilDensity(Rs_sat, Bo_sat, APItoSG(oilAPI), Gammag)

        //  Z_sat = DranchukAbuKassemGasCompressibilityFactor(14.7, 60, Gammag)
        //  rho_gsc = GasDensityFromZ(14.7, 60, Gammag, Z_sat)


        //  //Gas Condensate Standing
        //  // A1 =	0.19408473
        //  // A2 =	-3709.4214
        //  // A3 =	1.06052098
        //  // A4 =	-0.05022324
        //  // A5 =	-0.003771627

        //  A1 =	3.45841109
        //  A2 =	6.89461E-5
        //  A3 = -0.03169875
        //  A4 = 251.0827307
        //  A5 =	4.174003053

        //  return (((A1*rho_gsc*(A2*pressure^2+A3*pressure+A4))/Pb)*exp((A5*CGR*519.67)/(rho_osc*T_R*14.7)))
        //}

    }
}
