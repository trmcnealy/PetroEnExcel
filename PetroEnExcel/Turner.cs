using System.Runtime.CompilerServices;

using ExcelDna.Integration;

namespace PetroEnExcel
{
    public static class Turner
    {
        [ExcelFunction(Name = "Turner.TerminalVelocity")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double TerminalVelocity(double sigma, double rho_l, double rho_g)
        {
            return (20.4 * ((Math.Pow(sigma, 0.25) * Math.Pow((rho_l - rho_g), 0.25)) / Math.Pow(rho_g, 0.5)));
        }

        [ExcelFunction(Name = "Turner.GasVelocityWater")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GasVelocityWater(double sGGas, double pressure, double temperature_R)
        {
            double z = HydrocarbonProperties.GasZKatz(pressure, temperature_R, sGGas, out _, out _, out _, out _);

            double k = GasVelocityWaterK(sGGas, z, temperature_R);

            return (((5.62 * Math.Pow((67.0 - (k * pressure)), 0.25)) / (Math.Pow((k * pressure), 0.5))));
            //(5.62 * (Math.Pow(67 - (0.0031 * pressure), 0.25) / Math.Pow(0.0031 * pressure, 0.5)));

            double GasVelocityWaterK(double sGGas, double z, double temperature_R)
            {
                return ((2.693 * sGGas) / (z * temperature_R));
            }
        }

        [ExcelFunction(Name = "Turner.GasVelocityCondensate")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GasVelocityCondensate(double sGGas, double pressure, double temperature_R)
        {
            double z = HydrocarbonProperties.GasZKatz(pressure, temperature_R, sGGas, out _, out _, out _, out _);

            double k = GasVelocityWaterK(sGGas, z, temperature_R);

            return (((4.02 * Math.Pow((45.0 - (k * pressure)), 0.25)) / (Math.Pow((k * pressure), 0.5))));
            //return (4.02 * (Math.Pow(45 - (0.0031 * pressure), 0.25) / Math.Pow(0.0031 * pressure, 0.5)));

            double GasVelocityWaterK(double sGGas, double z, double temperature_R)
            {
                return ((2.693 * sGGas) / (z * temperature_R));
            }
        }

        [ExcelFunction(Name = "Turner.GasRateStd",
                       Description = "MMscf/day")]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GasRateStd(double ug_fts, double area_ft2, double pressure, double temperature_R, double sGGas)
        {
            double z = HydrocarbonProperties.GasZKatz(pressure, temperature_R, sGGas, out _, out _, out _, out _);

            return ((3.06 * ug_fts * area_ft2 * pressure) / (temperature_R * z));
        }
    }
}
