OpenDataAnalysis
================

Data Analysis tool

Main libraries:

Old body of code that's not well organized for public consumption.

DataElements - Models
  
   PivotFunctions are static functions for creating pivot tables off of lists.
   
   ex: If you have a table of data like 'date', and eletricity usage per day, you can get the average useage by day of week with one (admittedly complicated) line of code.
   
   (Where TimedMeasurement is an IEnumerbale of Date and Decimal that exposes an Average function).
   
   Dictionary<DayOfWeek, decimal> byDayOfWeek3 = PivotFunctions.ProducePivotAnalytic<TimedMeasurement, DayOfWeek, decimal>(
    table,
    (x) => (x.MeasurementTime.DayOfWeek),
    (z) => (z.Average(m => m.GetByIndex(1));


.....also contains a bunch of code I wrote to play around with graph theory and pathfinding.  There's an A* implementation in here somewhere...

....also contains the boilerplat for file access that I often copy down to do quick one-off projects.
