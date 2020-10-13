The master branch is the unaltered code as created by the AWS-provided template `serverless.AspNetCore30WebAPI`. 

The branch `features/ImplementOdataAndSwagger` has been upgraded to 3.1 and has ODATA and Swagger configured. 
It displays the issues below:

- Will not display the swagger documentation through the API gateway/lambda.
    + Returns a 404 not found for the `swagger.json` file and for `index.html`
    + Swagger works fine when run locally.
- Does not correctly respond to ODATA query string parameters in the unit tests or when deployed as a Lambda.
    + It does return ODATA when the path `odata/WeatherForecast` is hit
    + Any of the expected behaviors like $filter, $select, $count, $skip, $top do not work
    + When run locally, the ODATA query-enabled endpoint operates correctly, i.e. performing a GET request to
    https://localhost:5001/odata/WeatherForecast?$select=id,temperatureC&$skip=2&$top=5&$count=true will return
    5 items starting with item #3. It will only return the Id and TemperatureC properties. It also returns the
    total count of 20 items.
    
```json
{
  "@odata.context":"https://localhost:5001/odata/$metadata#WeatherForecast(Id,TemperatureC)",
  "@odata.count":20,
  "value":[
    {"Id":3,"TemperatureC":41},
    {"Id":4,"TemperatureC":4},
    {"Id":5,"TemperatureC":40},
    {"Id":6,"TemperatureC":1},
    {"Id":7,"TemperatureC":9}
  ]
}
```
