using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;


namespace DotNetLambda.Tests
{
    public class WeatherForecastControllerTests
    {
        [Fact]
        public async Task TestGet()
        {
            LambdaEntryPoint lambdaFunction = new LambdaEntryPoint();

            string requestStr = await File.ReadAllTextAsync("./SampleRequests/WeatherForecastController-Get.json");
            APIGatewayProxyRequest request = JsonConvert.DeserializeObject<APIGatewayProxyRequest>(requestStr);
            TestLambdaContext context = new TestLambdaContext();
            APIGatewayProxyResponse response = await lambdaFunction.FunctionHandlerAsync(request, context);

            Assert.Equal(200, response.StatusCode);
            Assert.True(response.MultiValueHeaders.ContainsKey("Content-Type"));
            Assert.Equal("application/json; charset=utf-8", response.MultiValueHeaders["Content-Type"][0]);
            Assert.NotNull(response.Body);
            List<WeatherForecast> forecasts = JsonConvert.DeserializeObject<List<WeatherForecast>>(response.Body);
            Assert.NotEmpty(forecasts);
            Assert.Equal(20, forecasts.Count);
            Assert.Equal(1, forecasts.First().Id);
            Assert.Equal(20, forecasts.Last().Id);
        }

        [Fact]
        public async Task TestGetOdata()
        {
            LambdaEntryPoint lambdaFunction = new LambdaEntryPoint();

            string requestStr = await File.ReadAllTextAsync("./SampleRequests/WeatherForecastController-GetOdata.json");
            APIGatewayProxyRequest request = JsonConvert.DeserializeObject<APIGatewayProxyRequest>(requestStr);
            TestLambdaContext context = new TestLambdaContext();
            APIGatewayProxyResponse response = await lambdaFunction.FunctionHandlerAsync(request, context);

            Assert.Equal(200, response.StatusCode);
            Assert.True(response.MultiValueHeaders.ContainsKey("Content-Type"));
            Assert.Equal("application/json; odata.metadata=minimal; odata.streaming=true",
                response.MultiValueHeaders["Content-Type"][0]);
            Assert.NotNull(response.Body);
            OdataGetResponse<WeatherForecast> odata = JsonConvert.DeserializeObject<OdataGetResponse<WeatherForecast>>(response.Body);
            Assert.NotNull(odata);
            Assert.False(odata.Count.HasValue);
            Assert.NotNull(odata.Value);
            Assert.Equal(20, odata.Value.Count);
            Assert.Equal(1, odata.Value.First().Id);
            Assert.Equal(20, odata.Value.Last().Id);
        }

        [Fact]
        public async Task TestGetOdataWithQuery()
        {
            LambdaEntryPoint lambdaFunction = new LambdaEntryPoint();

            string requestStr = await File.ReadAllTextAsync("./SampleRequests/WeatherForecastController-GetOdata.json");
            APIGatewayProxyRequest request = JsonConvert.DeserializeObject<APIGatewayProxyRequest>(requestStr);
            TestLambdaContext context = new TestLambdaContext();
            request.QueryStringParameters = new Dictionary<string, string>{{"$count", "true"}, {"$skip", "2"}, {"$top", "5"}};
            APIGatewayProxyResponse response = await lambdaFunction.FunctionHandlerAsync(request, context);

            Assert.Equal(200, response.StatusCode);
            Assert.True(response.MultiValueHeaders.ContainsKey("Content-Type"));
            Assert.Equal("application/json; odata.metadata=minimal; odata.streaming=true",
                response.MultiValueHeaders["Content-Type"][0]);
            Assert.NotNull(response.Body);
            OdataGetResponse<WeatherForecast> odata = JsonConvert.DeserializeObject<OdataGetResponse<WeatherForecast>>(response.Body);
            Assert.NotNull(odata);
            Assert.True(odata.Count.HasValue);
            Assert.Equal(20, odata.Count.Value);
            Assert.NotNull(odata.Value);
            Assert.Equal(5, odata.Value.Count);
            Assert.Equal(3, odata.Value.First().Id);
            Assert.Equal(7, odata.Value.Last().Id);
        }

        private class OdataGetResponse<T>
        {
            [JsonProperty("odata.count")] public int? Count { get; set; }

            public List<T> Value { get; set; }
        }
    }
}