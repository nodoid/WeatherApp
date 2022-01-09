using NUnit.Framework;

namespace Tests
{
    public class WeatherViewModelTest
    {
        [Test]
        public void IsConnectedTest()
        {
            var viewModel = new WeatherApp.ViewModels.WeatherViewModel();
            Assert.False(viewModel.IsConnected);
        }
    }
}
