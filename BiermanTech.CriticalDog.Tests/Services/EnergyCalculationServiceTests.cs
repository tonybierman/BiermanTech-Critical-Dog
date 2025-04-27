using BiermanTech.CanineHealth;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BiermanTech.CriticalDog.Tests.Services
{
    public class EnergyCalculationServiceTests
    {
        private readonly Mock<IUnitConverter> _unitConverterMock;
        private readonly EnergyCalculationService _service;

        public EnergyCalculationServiceTests()
        {
            _unitConverterMock = new Mock<IUnitConverter>();
            _service = new EnergyCalculationService(_unitConverterMock.Object);
        }

        [Fact]
        public void Constructor_NullUnitConverter_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EnergyCalculationService(null));
        }

        #region ProjectIdealWeightDate Tests

        [Fact]
        public void ProjectIdealWeightDate_ZeroDailyWeightChange_ReturnsNull()
        {
            var result = _service.ProjectIdealWeightDate(10m, 12m, DateTime.Today, 0m);
            Assert.Null(result);
        }

        [Fact]
        public void ProjectIdealWeightDate_AlreadyAtIdealWeight_ReturnsLastRecordedDate()
        {
            var lastRecordedDate = DateTime.Today;
            var result = _service.ProjectIdealWeightDate(10m, 10m, lastRecordedDate, 0.1m);
            Assert.Equal(lastRecordedDate, result);
        }

        [Fact]
        public void ProjectIdealWeightDate_WeightLossPositiveChange_ReturnsNull()
        {
            var result = _service.ProjectIdealWeightDate(8m, 10m, DateTime.Today, 0.1m);
            Assert.Null(result);
        }

        [Fact]
        public void ProjectIdealWeightDate_WeightGainNegativeChange_ReturnsNull()
        {
            var result = _service.ProjectIdealWeightDate(12m, 10m, DateTime.Today, -0.1m);
            Assert.Null(result);
        }

        [Fact]
        public void ProjectIdealWeightDate_WeightLoss_CalculatesCorrectDate()
        {
            var lastRecordedDate = new DateTime(2025, 4, 1);
            var result = _service.ProjectIdealWeightDate(8m, 10m, lastRecordedDate, -0.5m);
            var expectedDate = lastRecordedDate.AddDays(4); // 2kg / 0.5kg/day = 4 days
            Assert.Equal(expectedDate, result);
        }

        [Fact]
        public void ProjectIdealWeightDate_WeightGain_CalculatesCorrectDate()
        {
            var lastRecordedDate = new DateTime(2025, 4, 1);
            var result = _service.ProjectIdealWeightDate(12m, 10m, lastRecordedDate, 0.25m);
            var expectedDate = lastRecordedDate.AddDays(8); // 2kg / 0.25kg/day = 8 days
            Assert.Equal(expectedDate, result);
        }

        #endregion

        #region CalculateEnergyRequirementsAsync Tests

        [Fact]
        public async Task CalculateEnergyRequirementsAsync_NullInput_ReturnsInvalidResult()
        {
            var result = await _service.CalculateEnergyRequirementsAsync(null);
            Assert.False(result.IsValid);
            Assert.Equal(0, result.WeightInKgs);
            Assert.Equal(0, result.WeightInLbs);
            Assert.Equal(0, result.Rer);
            Assert.Equal(0, result.MeanMer);
            Assert.Equal(default(LifeStageFactorsEnum), result.LifeStage);
        }

        [Fact]
        public async Task CalculateEnergyRequirementsAsync_NullWeightMetricValue_ReturnsInvalidResult()
        {
            var input = new EnergyCalculationInput
            {
                WeightMetricValue = null,
                UnitName = "Kilograms",
                LifeStageMetricValue = 1
            };
            var result = await _service.CalculateEnergyRequirementsAsync(input);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task CalculateEnergyRequirementsAsync_NullUnitName_ReturnsInvalidResult()
        {
            var input = new EnergyCalculationInput
            {
                WeightMetricValue = 10m,
                UnitName = null,
                LifeStageMetricValue = 1
            };
            var result = await _service.CalculateEnergyRequirementsAsync(input);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task CalculateEnergyRequirementsAsync_NullLifeStageMetricValue_ReturnsInvalidResult()
        {
            var input = new EnergyCalculationInput
            {
                WeightMetricValue = 10m,
                UnitName = "Kilograms",
                LifeStageMetricValue = null
            };
            var result = await _service.CalculateEnergyRequirementsAsync(input);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task CalculateEnergyRequirementsAsync_InvalidLifeStage_ReturnsInvalidResult()
        {
            var input = new EnergyCalculationInput
            {
                WeightMetricValue = 10m,
                UnitName = "Kilograms",
                LifeStageMetricValue = 999 // Invalid enum value
            };
            var result = await _service.CalculateEnergyRequirementsAsync(input);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task CalculateEnergyRequirementsAsync_ValidInput_ReturnsCorrectResult()
        {
            var input = new EnergyCalculationInput
            {
                WeightMetricValue = 10m,
                UnitName = "Kilograms",
                LifeStageMetricValue = (int)LifeStageFactorsEnum.IntactAdult
            };

            _unitConverterMock.Setup(x => x.ConvertAsync("Kilograms", "Kilograms", 10.0))
                .ReturnsAsync(10.0);
            _unitConverterMock.Setup(x => x.ConvertAsync("Kilograms", "Pounds", 10.0))
                .ReturnsAsync(22.046);

            var result = await _service.CalculateEnergyRequirementsAsync(input);

            Assert.True(result.IsValid);
            Assert.Equal(10.0, result.WeightInKgs);
            Assert.Equal(22.046, result.WeightInLbs, 3);
            Assert.True(result.Rer > 0);
            Assert.True(result.MeanMer > 0);
            Assert.Equal(LifeStageFactorsEnum.IntactAdult, result.LifeStage);
        }

        [Fact]
        public async Task CalculateEnergyRequirementsAsync_ConversionThrowsException_ReturnsInvalidResult()
        {
            var input = new EnergyCalculationInput
            {
                WeightMetricValue = 10m,
                UnitName = "Kilograms",
                LifeStageMetricValue = (int)LifeStageFactorsEnum.IntactAdult
            };

            _unitConverterMock.Setup(x => x.ConvertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
                .ThrowsAsync(new Exception("Conversion error"));

            var result = await _service.CalculateEnergyRequirementsAsync(input);
            Assert.False(result.IsValid);
        }

        #endregion
    }
}