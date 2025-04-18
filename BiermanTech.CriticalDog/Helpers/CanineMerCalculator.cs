namespace BiermanTech.CriticalDog.Helpers
{
    /// <summary>
    /// Provides methods to calculate a dog's Resting Energy Requirement (RER) and Maintenance Energy Requirement (MER)
    /// based on weight and life stage, including unit conversions between pounds and kilograms.
    /// </summary>
    public class CanineMerCalculator
    {
        /// <summary>
        /// The conversion factor from kilograms to pounds (1 kg = 2.20462 lbs).
        /// </summary>
        private const double PoundsPerKilogram = 2.20462;

        /// <summary>
        /// Converts a weight from kilograms to pounds.
        /// </summary>
        /// <param name="kilograms">The weight in kilograms to convert.</param>
        /// <returns>The weight in pounds.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="kilograms"/> is negative.
        /// </exception>
        public static double KgToLbs(double kilograms)
        {
            if (kilograms < 0)
                throw new ArgumentOutOfRangeException(nameof(kilograms), "Weight cannot be negative.");
            return kilograms * PoundsPerKilogram;
        }

        /// <summary>
        /// Converts a weight from pounds to kilograms.
        /// </summary>
        /// <param name="pounds">The weight in pounds to convert.</param>
        /// <returns>The weight in kilograms.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="pounds"/> is negative.
        /// </exception>
        public static double LbsToKg(double pounds)
        {
            if (pounds < 0)
                throw new ArgumentOutOfRangeException(nameof(pounds), "Weight cannot be negative.");
            return pounds / PoundsPerKilogram;
        }

        /// <summary>
        /// Calculates the Resting Energy Requirement (RER) for a dog based on its body weight in kilograms.
        /// RER represents the energy needed at rest in a neutral environment.
        /// </summary>
        /// <param name="bodyWeightKg">The dog's body weight in kilograms.</param>
        /// <returns>The RER in kilocalories per day.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="bodyWeightKg"/> is less than or equal to zero.
        /// </exception>
        public static double CalculateRer(double bodyWeightKg)
        {
            if (bodyWeightKg <= 0)
                throw new ArgumentOutOfRangeException(nameof(bodyWeightKg), "Body weight must be positive.");
            return 70 * Math.Pow(bodyWeightKg, 0.75); // Simplified from nested Sqrt
        }

        /// <summary>
        /// Calculates the Maintenance Energy Requirement (MER) range and Resting Energy Requirement (RER)
        /// for a dog based on its life stage and body weight.
        /// </summary>
        /// <param name="lifeStage">The life stage of the dog, determining the MER range factors.</param>
        /// <param name="bodyWeightKg">The dog's body weight in kilograms.</param>
        /// <returns>A tuple containing the RER, lower MER bound, and upper MER bound in kilocalories per day.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="bodyWeightKg"/> is less than or equal to zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="lifeStage"/> is not a valid value of <see cref="LifeStageFactorsEnum"/>.
        /// </exception>
        public static (double rer, double lowerMer, double upperMer) CalculateMer(LifeStageFactorsEnum lifeStage, double bodyWeightKg)
        {
            double rer = CalculateRer(bodyWeightKg);
            (double lowerFactor, double upperFactor) = GetMerRange(lifeStage);
            return (rer, rer * lowerFactor, rer * upperFactor);
        }

        /// <summary>
        /// Retrieves the MER range factors (lower and upper bounds) based on the dog's life stage.
        /// These factors are multiplied by the RER to determine the MER range.
        /// </summary>
        /// <param name="lifeStage">The life stage of the dog.</param>
        /// <returns>A tuple containing the lower and upper MER factors.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="lifeStage"/> is not a valid value of <see cref="LifeStageFactorsEnum"/>.
        /// </exception>
        public static (double lowerFactor, double upperFactor) GetMerRange(LifeStageFactorsEnum lifeStage)
        {
            switch (lifeStage)
            {
                case LifeStageFactorsEnum.NeuteredAdult: // 1.4–1.6: Typical for neutered adults
                case LifeStageFactorsEnum.IntactAdult:   // 1.4–1.6: Slightly low for intact, but combined for simplicity
                    return (1.4, 1.6);
                case LifeStageFactorsEnum.InactiveObeseProne: // 1.2–1.4: Reduced for obesity management
                    return (1.2, 1.4);
                case LifeStageFactorsEnum.WeightLoss: // 1.0: Restricted for weight loss
                    return (1.0, 1.0);
                case LifeStageFactorsEnum.WeightGain: // 1.2–1.8: Increased for growth
                    return (1.2, 1.8);
                case LifeStageFactorsEnum.ActiveWorkingDog: // 2.0–5.0: Wide range for high activity
                    return (2.0, 5.0);
                case LifeStageFactorsEnum.Puppy0To4Months: // 3.0: High for early growth
                    return (3.0, 3.0);
                case LifeStageFactorsEnum.Puppy4MonthsToAdult: // 1.5–2.0: Transition to adult needs
                    return (1.5, 2.0);
                case LifeStageFactorsEnum.Gestation: // 1.6–2.0: Increased for pregnancy
                    return (1.6, 2.0);
                case LifeStageFactorsEnum.Lactation: // 2.0–5.0: High for milk production
                    return (2.0, 5.0);
                default:
                    throw new ArgumentException($"Invalid life stage: {lifeStage}", nameof(lifeStage));
            }
        }

        /// <summary>
        /// Calculates the mean Maintenance Energy Requirement (MER) for a dog based on its life stage and body weight.
        /// The mean is the average of the lower and upper MER bounds.
        /// </summary>
        /// <param name="lifeStage">The life stage of the dog, determining the MER range factors.</param>
        /// <param name="bodyWeightKg">The dog's body weight in kilograms.</param>
        /// <returns>The mean MER in kilocalories per day.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="bodyWeightKg"/> is less than or equal to zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="lifeStage"/> is not a valid value of <see cref="LifeStageFactorsEnum"/>.
        /// </exception>
        public static double CalculateMeanMer(LifeStageFactorsEnum lifeStage, double bodyWeightKg)
        {
            (double rer, double lowerMer, double upperMer) = CalculateMer(lifeStage, bodyWeightKg);
            return (lowerMer + upperMer) / 2;
        }
    }
}