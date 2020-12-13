"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/forecasthub").build();



connection.on("newForecast", function (reading) {
    console.log(reading);
});

