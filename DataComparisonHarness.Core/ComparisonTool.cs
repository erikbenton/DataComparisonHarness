using DataComparisonEngine;
using DataComparisonHarness.Core.Loggers;
using DataComparisonHarness.Core.Utilities;
using System;
using System.Data;
using System.Reflection;

namespace DataComparisonHarness.Core;

public class ComparisonTool<T>(T configuration, ILogger logger) where T : BaseConfiguration
{
    public T Configuration { get; set; } = configuration;

    public ILogger Logger { get; set; } = logger;

    public BaseReport CompareDataSources<TReport>(DataTable source, DataTable target, TReport? report = null)
        where TReport : BaseReport
    {
        // Configure report if necessary
        report ??= (TReport)new BaseReport();

        // Initialize report
        InitializeReport(ref report,
            Configuration.TargetDataSourceName,
            Configuration.SourceDataSourceName,
            target.Rows.Count,
            source.Rows.Count);

        // Join the data together
        var joinedData = JoinDataSources(source.Select(), target.Select());

        // Determine if there are any unjoined rows
        if (joinedData.Count < report.TargetCount)
        {
            var joinedTargetRows = joinedData.Select(set => Configuration.TargetJoinObject(set.Target));
            ReportUnjoinedData(ref report.TargetOnly, joinedTargetRows, target.Select(), Configuration.TargetJoinObject);
        }

        if (joinedData.Count < report.SourceCount)
        {
            var joinedSourceRows = joinedData.Select(set => Configuration.SourceJoinObject(set.Source));
            ReportUnjoinedData(ref report.SourceOnly, joinedSourceRows, source.Select(), Configuration.SourceJoinObject);
        }

        // Get the Column names from the data sources
        var targetColumns = GetColumns(target.Columns);
        var sourceColumns = GetColumns(source.Columns);

        // Remove columns to skip in Target data source
        var filteredTargetColumns = targetColumns
            .Where(column => !Configuration.TargetColumnsToSkip.Select(s => s.ToLower()).Contains(column.Name.ToLower()));

        // Get the column mappings for comparing
        List<(string targetColumnName, string sourceColumnName)> mappedColumns = GetColumnMappingForComparing(
            filteredTargetColumns.Select(x => x.Name),
            sourceColumns.Select(x => x.Name));

        try
        {
            // Go through each joined row for comparing
            foreach (var row in joinedData)
            {

                foreach (var (targetColumnName, sourceColumnName) in mappedColumns)
                {
                    var targetColumn = targetColumns.First(col => col.Name == targetColumnName);
                    var sourceColumn = sourceColumns.First(col => col.Name == sourceColumnName);
                    var result = Compare(row, targetColumn, sourceColumn);

                    if (!result)
                    {
                        LogComparisonFailure(ref report.Fails, row, targetColumnName, sourceColumnName);
                    }

                    report.Pass &= result;
                }
            }

            foreach (var (targetColumName, sourceColumName) in mappedColumns)
            {
                report.ComparedColumns.Add($"{targetColumName} | {sourceColumName}");
            }
        }
        catch (Exception ex)
        {
            // Error comparing data
            // can't continue with comparison
            // but still want to see the report
            report.Errors.Add(
                Logger.Log(ex.Message, LogLevel.Error));

            report.Pass = false;
        }

        return report;
    }

    private void LogComparisonFailure(ref List<string> reportFails, DataRowPair row, string targetColumnName, string sourceColumnName)
    {
        var targetValue = row.Target[targetColumnName];
        var sourceValue = row.Source[sourceColumnName];

        var joinedValues = "";
        var joinObject = Configuration.TargetJoinObject(row.Target);
        var properties = joinObject.GetType().GetProperties();
        foreach (PropertyInfo property in properties)
        {
            joinedValues += $"{property.Name} = {property.GetValue(joinObject)},";
        }

        reportFails.Add(Logger.Log(
            $"CompareColumns: Value Mismatch: Join Values: {joinedValues} | Column: {targetColumnName}  Target Value: '{targetValue}'  Source Value: '{sourceValue}'.",
            LogLevel.Fail));
    }

    private bool Compare(DataRowPair row, ComparisonColumn targetColumn, ComparisonColumn sourceColumn)
    {
        // Check column types
        if (targetColumn.DataType != sourceColumn.DataType)
        {
            throw new Exception($"Column types do not match. Target: {targetColumn.DataType}, Source: {sourceColumn.DataType}.");
        }

        var targetValue = row.Target[targetColumn.Name];
        var sourceValue = row.Source[sourceColumn.Name];

        // Handle nulls case
        if (targetValue == null || sourceColumn == null)
        {
            return targetValue == sourceValue;
        }

        var comparisonType = GetComparisonType(targetColumn.DataType);

        return comparisonType switch
        {
            ComparisonType.Integer => ValueComparer.CompareIntegers(targetValue, sourceValue),
            ComparisonType.Double => ValueComparer.CompareDoubles(targetValue, sourceValue, Configuration.Tolerance),
            ComparisonType.Decimal => ValueComparer.CompareDecimals(targetValue, sourceValue, (decimal)Configuration.Tolerance),
            ComparisonType.Boolean => ValueComparer.CompareBools(targetValue, sourceValue),
            ComparisonType.DateTime => ValueComparer.CompareDateTime(targetValue, sourceValue),
            ComparisonType.String => ValueComparer.CompareStrings(targetValue, sourceValue),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private ComparisonType GetComparisonType(Type dataType)
    {
        var typeCode = Type.GetTypeCode(dataType);
        return typeCode switch
        {
            TypeCode.Boolean => ComparisonType.Boolean,

            TypeCode.SByte or TypeCode.Byte
                or TypeCode.Int16 or TypeCode.UInt16
                or TypeCode.Int32 or TypeCode.UInt32
                or TypeCode.Int64 or TypeCode.UInt64
                or TypeCode.Single => ComparisonType.Integer,

            TypeCode.Double => ComparisonType.Double,

            TypeCode.Decimal => ComparisonType.Decimal,

            TypeCode.DateTime or TypeCode.Char
                or TypeCode.String or TypeCode.Empty
                or TypeCode.Object or TypeCode.DBNull => ComparisonType.String,

            _ => throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, null),
        };
    }

    private List<(string targetColumn, string sourceColumn)> GetColumnMappingForComparing(IEnumerable<string> targetColumns, IEnumerable<string> sourceColumns)
    {
        List<(string targetColumn, string sourceColumn)> mappedColumns = new();
        foreach (var targetColumn in targetColumns)
        {
            // Get mapped column
            var sourceColumn = GetTargetSourceColumnMap(targetColumn);

            // Make sure mapped column exists in Source data
            if (!sourceColumns.Contains(sourceColumn))
            {
                throw new Exception($"Unable to find {sourceColumn} in {Configuration.SourceDataSourceName}.");
            }

            mappedColumns.Add(new(targetColumn, sourceColumn));
        }

        return mappedColumns;
    }

    private string GetTargetSourceColumnMap(string targetColumn)
    {
        // Check for any specified column mapping
        var columnMap = Configuration.SpecificColumnMapping
                .FirstOrDefault(map => map.targetColumn.Equals(map.sourceColumn));

        return string.IsNullOrEmpty(columnMap.sourceColumn)
            ? targetColumn
            : columnMap.sourceColumn;
    }

    private static void InitializeReport<TReport>(ref TReport report, string targetName, string sourceName, int targetCount, int sourceCount)
        where TReport : BaseReport
    {
        report.Pass = true;

        report.TargetName = targetName;
        report.SourceName = sourceName;

        report.TargetCount = targetCount;
        report.SourceCount = sourceCount;
    }

    private ComparisonColumn[] GetColumns(DataColumnCollection columns) => [.. columns.Cast<DataColumn>().Select(x => new ComparisonColumn(x.ColumnName, x.DataType))];

    private void ReportUnjoinedData(ref List<string> reportList, IEnumerable<object> allJoinedRows, DataRow[] dataRows, Func<DataRow, object> joinObject)
    {
        var unjoinedData = dataRows
            .Select(row => joinObject(row))
            .Except(allJoinedRows, new JoinedDataRowComparer());

        foreach (var unjoinedRow in unjoinedData)
        {
            reportList.Add(unjoinedRow.ToPrettyString());
        }
    }

    private List<DataRowPair> JoinDataSources(IEnumerable<DataRow> source, IEnumerable<DataRow> target)
    {
        var joinedData = target.Join(source,
            Configuration.TargetJoinObject,
            Configuration.SourceJoinObject,
            (target, source) => new DataRowPair(target, source))
            .ToList();

        return joinedData;
    }

    private ICollection<DataRowPair> ZipDataSources(DataRow[] source, DataRow[] target)
    {
        ICollection<DataRowPair> joinedData = new List<DataRowPair>();
        for (var i = 0; i < target.Length && i < source.Length; i++)
        {
            joinedData.Add(new DataRowPair(target[i], source[i]));
        }

        return joinedData;
    }
}
